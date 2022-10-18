#region License
/*********************************************************************************
 * IFuncExtensions.cs
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

namespace Eutherion
{
    /// <summary>
    /// Defines extension methods to convert a <see cref="Func{TResult}"/> to a <see cref="IFunc{TResult}"/> and vice versa.
    /// </summary>
    public static class IFuncExtensions
    {
        private struct EncapsulatedFunc<TResult> : IFunc<TResult>
        {
            public readonly Func<TResult> Fn;

            public EncapsulatedFunc(Func<TResult> fn) => Fn = fn;

            public TResult Eval() => Fn();
        }

        /// <summary>
        /// Converts a <see cref="Func{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="func">
        /// The <see cref="Func{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="func"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this Func<TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            return new EncapsulatedFunc<TResult>(func);
        }

        /// <summary>
        /// Converts a <see cref="IFunc{TResult}"/> to an <see cref="Func{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="func">
        /// The <see cref="IFunc{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Func{TResult}"/> wrapping <paramref name="func"/>.
        /// </returns>
        public static Func<TResult> ToFunc<TResult>(this IFunc<TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            // Prevent multiple levels of IFuncs wrapped within Funcs and so on.
            return func is EncapsulatedFunc<TResult> encapsulated ? encapsulated.Fn : func.Eval;
        }
    }
}
