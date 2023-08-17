#region License
/*********************************************************************************
 * ConstantValue.cs
 *
 * Copyright (c) 2004-2023 Henk Nicolai
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

namespace Eutherion
{
    /// <summary>
    /// Provides an implementation for <see cref="IFunc{TResult}"/> that always returns the same value stored in memory.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to encapsulate.
    /// </typeparam>
    public class ConstantValue<T> : IFunc<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantValue{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value to encapsulate.
        /// </param>
        public ConstantValue(T value) => Value = value;

        /// <summary>
        /// Gets the constant value.
        /// </summary>
        public T Value { get; }

        T IFunc<T>.Eval() => Value;
    }
}
