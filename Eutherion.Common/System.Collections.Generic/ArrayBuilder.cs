#region License
/*********************************************************************************
 * ArrayBuilder.cs
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
    /// <summary>
    /// Collects a list of elements to be put into an array.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the array to build.
    /// </typeparam>
    public sealed class ArrayBuilder<T> : IReadOnlyList<T>
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
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>
        /// The element at the specified index.
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
                    return array[index];
                }

                throw ExceptionUtility.ThrowListIndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ArrayBuilder{T}"/>.
        /// </summary>
        public ArrayBuilder() => array = EmptyArray;

        /// <summary>
        /// Initializes a new instance of <see cref="ArrayBuilder{T}"/> with a sequence of elements.
        /// </summary>
        /// <param name="elements">
        /// The sequence of elements to be added to the builder.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="elements"/> is <see langword="null"/>.
        /// </exception>
        public ArrayBuilder(IEnumerable<T> elements)
        {
            array = EmptyArray;
            AddRange(elements);
        }

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
        /// Adds a sequence of elements to the builder.
        /// </summary>
        /// <param name="elements">
        /// The sequence of elements to be added to the builder.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="elements"/> is <see langword="null"/>.
        /// </exception>
        public void AddRange(IEnumerable<T> elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));

            int minimumCapacity;
            switch (elements)
            {
                case ICollection<T> collection:
                    minimumCapacity = count + collection.Count;
                    goto growAndFillArray;
                case IReadOnlyCollection<T> readOnlyCollection:
                    minimumCapacity = count + readOnlyCollection.Count;
                growAndFillArray:
                    // Grow the array just once.
                    if (minimumCapacity > 0)
                    {
                        int currentCapacity = array.Length;
                        if (currentCapacity < minimumCapacity)
                        {
                            int newCapacity = currentCapacity == 0 ? DefaultCapacity : currentCapacity * 2;
                            while (newCapacity < minimumCapacity) newCapacity *= 2;
                            T[] array = new T[newCapacity];
                            Array.Copy(this.array, 0, array, 0, count);
                            this.array = array;
                        }
                    }

                    // Can safely use foreach even if 'elements' is this very builder.
                    // This because Enumerator saves its own copy of 'count' when it is created.
                    int index = count;

                    foreach (var element in elements)
                    {
                        array[index] = element;
                        index++;
                    }

                    count = index;
                    break;
                default:
                    foreach (var element in elements)
                    {
                        Add(element);
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns the partially filled array which contains the elements added to this builder
        /// in the order in which they were added. The builder is then cleared.
        /// </summary>
        /// <returns>
        /// The partially filled array containing the elements added to this builder together
        /// with the total number of elements in the array.
        /// </returns>
        public (T[] array, int count) Commit()
        {
            T[] array = this.array;
            int count = this.count;
            this.array = EmptyArray;
            this.count = 0;
            return (array, count);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the elements added to the builder.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the elements added to the builder.
        /// </returns>
#if NET5_0_OR_GREATER
        public ArrayEnumerator<T> GetEnumerator() => new(array, Count);
#else
        public ArrayEnumerator<T> GetEnumerator() => new ArrayEnumerator<T>(array, count);
#endif

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ArrayEnumerator<T>(array, count);

        IEnumerator IEnumerable.GetEnumerator() => new ArrayEnumerator<T>(array, count);

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> region over this <see cref="ArrayBuilder{T}"/>.
        /// </summary>
        /// <returns>
        /// The read-only memory representation of this <see cref="ArrayBuilder{T}"/>.
        /// </returns>
        public ReadOnlyMemory<T> AsMemory() => array.AsMemory(0, count);

        /// <summary>
        /// Creates a new <see cref="ReadOnlySpan{T}"/> region over this <see cref="ArrayBuilder{T}"/>.
        /// </summary>
        /// <returns>
        /// The read-only span representation of this <see cref="ArrayBuilder{T}"/>.
        /// </returns>
        public ReadOnlySpan<T> AsSpan() => array.AsSpan(0, count);

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> region over an <see cref="ArrayBuilder{T}"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder for which to create the read-only memory representation.
        /// </param>
#if NET472
        public static implicit operator ReadOnlyMemory<T>(ArrayBuilder<T> builder) => builder == null ? default : builder.AsMemory();
#else
        public static implicit operator ReadOnlyMemory<T>(ArrayBuilder<T>? builder) => builder == null ? default : builder.AsMemory();
#endif

        /// <summary>
        /// Creates a new <see cref="ReadOnlySpan{T}"/> region over this <see cref="ArrayBuilder{T}"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder for which to create the read-only span representation.
        /// </param>
#if NET472
        public static implicit operator ReadOnlySpan<T>(ArrayBuilder<T> builder) => builder == null ? default : builder.AsSpan();
#else
        public static implicit operator ReadOnlySpan<T>(ArrayBuilder<T>? builder) => builder == null ? default : builder.AsSpan();
#endif
    }
}
