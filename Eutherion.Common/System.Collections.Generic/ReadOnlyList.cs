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
        // Disable null analysis here, since setting current to 'default' triggers it.
        // Normal 'foreach' use will never return the default value for a type though.
#if !NET472
#nullable disable
#endif
        /// <summary>
        /// Enumerates the elements of a <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] array;
            private readonly int count;

            private int index;
            private T current;

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public T Current => current;

            internal Enumerator(T[] array, int count)
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

        /// <summary>
        /// Collects a list of elements to be put into a <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        // Implement IEnumerable<T> to enable collection initializer syntax.
        public sealed class Builder : IReadOnlyCollection<T>
        {
            private const int DefaultCapacity = 4;

            // Benchmarked using Array.Empty<T> rather than this, and it turns out having a static local performs much better.
#if NET5_0_OR_GREATER
#pragma warning disable CA1825 // Avoid zero-length array allocations
#endif
            private static readonly T[] EmptyArray = new T[0];
#if NET5_0_OR_GREATER
#pragma warning restore CA1825 // Avoid zero-length array allocations
#endif

            private T[] array;
            private int count;

            /// <summary>
            /// Gets the current number of elements added to the builder.
            /// </summary>
            public int Count => count;

            /// <summary>
            /// Initializes a new instance of <see cref="Builder"/>.
            /// </summary>
            public Builder() => array = EmptyArray;

            /// <summary>
            /// Adds an element to the builder.
            /// </summary>
            /// <param name="element">
            /// The element to be added to the builder.
            /// </param>
            public void Add(T element)
            {
                int currentCapacity = array.Length;

                if (count == currentCapacity)
                {
                    if (currentCapacity == 0)
                    {
                        array = new T[DefaultCapacity];
                    }
                    else
                    {
                        int newCapacity = currentCapacity * 2;
                        T[] array = new T[newCapacity];
                        Array.Copy(this.array, 0, array, 0, currentCapacity);
                        this.array = array;
                    }
                }

                int size = count;
                count = size + 1;
                array[size] = element;
            }

            /// <summary>
            /// Converts the builder to a <see cref="ReadOnlyList{T}"/> which contains the elements added to this builder
            /// in the order in which they were added. The builder is then cleared.
            /// </summary>
            /// <returns>
            /// The <see cref="ReadOnlyList{T}"/> contains the elements added to this builder.
            /// </returns>
            public ReadOnlyList<T> Commit()
            {
                if (this.count == 0) return ReadOnlyList<T>.Empty;
                T[] array = this.array;
                int count = this.count;
                this.array = EmptyArray;
                this.count = 0;
                return new ReadOnlyList<T>(array, count);
            }

            /// <summary>
            /// Returns an enumerator that iterates through the elements added to the builder.
            /// </summary>
            /// <returns>
            /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the elements added to the builder.
            /// </returns>
#if NET5_0_OR_GREATER
            public Enumerator GetEnumerator() => new(array, Count);
#else
            public Enumerator GetEnumerator() => new Enumerator(array, count);
#endif

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(array, count);

            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(array, count);
        }

        /// <summary>
        /// Gets the empty <see cref="ReadOnlyList{T}"/>.
        /// </summary>
#if NET5_0_OR_GREATER
        public static readonly ReadOnlyList<T> Empty = new(Array.Empty<T>(), 0);
#else
        public static readonly ReadOnlyList<T> Empty = new ReadOnlyList<T>(Array.Empty<T>(), 0);
#endif

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

        private readonly T[] ReadOnlyArray;

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
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="Count"/>.
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
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the list.
        /// </returns>
#if NET5_0_OR_GREATER
        public Enumerator GetEnumerator() => new(ReadOnlyArray, Count);
#else
        public Enumerator GetEnumerator() => new Enumerator(ReadOnlyArray, Count);
#endif

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(ReadOnlyArray, Count);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(ReadOnlyArray, Count);
    }
}
