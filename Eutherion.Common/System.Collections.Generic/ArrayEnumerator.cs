#region License
/*********************************************************************************
 * ArrayEnumerator.cs
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

using Eutherion;

namespace System.Collections.Generic
{
    // Disable null analysis here, since setting current to 'default' triggers it.
    // Normal 'foreach' use will never return the default value for a type though.
#if !NET472
#nullable disable
#endif
    /// <summary>
    /// Enumerates the elements of an array which is partially filled.
    /// </summary>
    /// <remarks>
    /// This enumerator does not check its arguments. It expects the enumerated array to be not null, one-dimensional, and zero based.
    /// It expects the count of elements to be positive and less than or equal to the length of the array.
    /// Not satisfying this contract will crash the enumerator when used.
    /// </remarks>
    public struct ArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] array;
        private readonly int count;

        private int index;
        private T current;

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public T Current => current;

        /// <summary>
        /// Initializes a new instance of <see cref="ArrayEnumerator{T}"/> from an array with a specified number of elements.
        /// </summary>
        /// <param name="array">
        /// The array to enumerate.
        /// </param>
        /// <param name="count">
        /// The number of elements to enumerate.
        /// </param>
        public ArrayEnumerator(T[] array, int count)
        {
            this.array = array;
            this.count = count;
            index = 0;
            current = default;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the list.
        /// </returns>
        public bool MoveNext()
        {
            if (index < count)
            {
                current = array[index];
                index++;
                return true;
            }
            index = count + 1;
            current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            index = 0;
            current = default;
        }

        object IEnumerator.Current
        {
            get
            {
                if (index > 0 && index <= count) return Current;

                // Throw the appropriate exception.
                throw ExceptionUtil.ThrowInvalidEnumerationOperationException();
            }
        }

        /// <summary>
        /// Has no effect. Method is required by the <see cref="IDisposable"/> interface.
        /// </summary>
        public void Dispose() { }
    }
#if !NET472
#nullable enable
#endif
}
