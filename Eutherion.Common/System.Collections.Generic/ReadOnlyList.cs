#region License
/*********************************************************************************
 * ReadOnlyList.cs
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
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a read-only list of elements that can be accessed by index.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the read-only list.
    /// </typeparam>
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets the empty <see cref="ReadOnlyList{T}"/>.
        /// </summary>
#if NET5_0_OR_GREATER
        public static readonly ReadOnlyList<T> Empty = new(Array.Empty<T>(), 0);
#else
        public static readonly ReadOnlyList<T> Empty = new ReadOnlyList<T>(Array.Empty<T>(), 0);
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyList{T}"/> from an <see cref="ArrayBuilder{T}"/>.
        /// This empties the array builder.
        /// </summary>
        /// <param name="builder">
        /// The builder containing the elements of the list.
        /// </param>
        /// <returns>
        /// The initialized <see cref="ReadOnlyList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static ReadOnlyList<T> FromBuilder(ArrayBuilder<T> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var (array, count) = builder.Commit();
            if (count == 0) return Empty;
            return new ReadOnlyList<T>(array, count);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The elements of the list.
        /// </param>
        /// <returns>
        /// The initialized <see cref="ReadOnlyList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static ReadOnlyList<T> Create(IEnumerable<T> source)
        {
            if (source is ReadOnlyList<T> readOnlyList) return readOnlyList;
            var array = source.ToArrayEx();
            int length = array.Length;
            return length == 0 ? Empty : new ReadOnlyList<T>(array, length);
        }

        private T[] ReadOnlyArray;

        private ReadOnlyList(T[] array, int count)
        {
            ReadOnlyArray = array;
            Count = count;
        }

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get.
        /// </param>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
        /// </exception>
        public T this[int index]
        {
            get
            {
                // Cast to uint so negative values get flagged by this check too.
                if ((uint)index < (uint)Count)
                {
                    return ReadOnlyArray[index];
                }

                throw ExceptionUtil.ThrowListIndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the number of elements in the list.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Reduces the size of the internal array to its minimum, saving memory but involving an extra copy step.
        /// </summary>
        public void Truncate()
        {
            if (Count < ReadOnlyArray.Length)
            {
                T[] largeArray = ReadOnlyArray;
                ReadOnlyArray = new T[Count];
                Array.Copy(largeArray, 0, ReadOnlyArray, 0, Count);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the list.
        /// </returns>
#if NET5_0_OR_GREATER
        public ArrayEnumerator<T> GetEnumerator() => new(ReadOnlyArray, Count);
#else
        public ArrayEnumerator<T> GetEnumerator() => new ArrayEnumerator<T>(ReadOnlyArray, Count);
#endif

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ArrayEnumerator<T>(ReadOnlyArray, Count);

        IEnumerator IEnumerable.GetEnumerator() => new ArrayEnumerator<T>(ReadOnlyArray, Count);
    }
}
