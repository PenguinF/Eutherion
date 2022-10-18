#region License
/*********************************************************************************
 * SafeLazy.cs
 *
 * Copyright (c) 2004-2022 Henk Nicolai
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
**********************************************************************************/
#endregion

using System;

namespace Eutherion.Threading
{
    /// <summary>
    /// Represents the thread-safe lazy evaluation of a parameterless function.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the evaluated value.
    /// </typeparam>
    public class SafeLazy<TValue>
    {
        private readonly object Sentinel;
#if NET472
        private Func<TValue> Func;
#else
        private Func<TValue>? Func;
#endif
        private TValue EvaluatedValue;

        /// <summary>
        /// Initializes a new instance of <see cref="SafeLazy{TValue}"/>.
        /// </summary>
        /// <param name="func">
        /// The function to evaluate when the <see cref="Value"/> property is first accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func"/> is null.
        /// </exception>
        /// <remarks>
        /// N.B.: The provided function is evaluated while holding an internal lock.
        /// </remarks>
        public SafeLazy(Func<TValue> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
            Sentinel = new object();

#if !NET472
            // Use the bang operator, as EvaluatedValue is guaranteed to be initialized after calling 'Value'.
            EvaluatedValue = default!;
#endif
        }

        /// <summary>
        /// Gets the result of the evaluation of the function.
        /// The function is evaluated only once, when it is first needed.
        /// </summary>
        public TValue Value
        {
            get
            {
                // Use well known pattern for implementing thread safety.
                // It avoids taking a lock in most cases when Func was already null.
                if (Func != null)
                {
                    lock (Sentinel)
                    {
                        // Check Func again because multiple threads may have been racing for the lock.
                        if (Func != null)
                        {
                            // Evaluate the function inside the lock so any other threads
                            // racing for the evaluated value must wait until it's assigned.
                            EvaluatedValue = Func();

                            // If Func() threw, then it will be re-evaluated by the next caller,
                            // and it will likely throw again.
                            // This also releases any references a closure may have held implicitly.
                            Func = null;
                        }
                    }
                }

                return EvaluatedValue;
            }
        }
    }
}
