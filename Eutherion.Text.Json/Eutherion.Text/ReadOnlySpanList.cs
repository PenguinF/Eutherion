#region License
/*********************************************************************************
 * ReadOnlySpanList.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Eutherion.Text
{
    /// <summary>
    /// Represents a read-only list of spanned elements that can be accessed by index.
    /// </summary>
    /// <typeparam name="TSpan">
    /// The type of spanned elements in the read-only list.
    /// </typeparam>
    public abstract class ReadOnlySpanList<TSpan> : IReadOnlyList<TSpan>, ISpan where TSpan : ISpan
    {
        private class ZeroElements : ReadOnlySpanList<TSpan>
        {
            public override TSpan this[int index] => throw ExceptionUtil.ThrowListIndexOutOfRangeException();

            public ZeroElements() : base(Array.Empty<TSpan>(), 0, 0) { }

            public override int GetElementOffset(int index) => throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        private class OneOrMoreElements : ReadOnlySpanList<TSpan>
        {
            // Static because of necessary preprocessing.
            public static OneOrMoreElements Create(TSpan[] source, int count)
            {
                if (source[0] == null) throw new ArgumentException($"One or more elements in {nameof(source)} is null", nameof(source));
                int length = source[0].Length;
                int[] arrayElementOffsets = new int[count - 1];

                for (int i = 1; i < count; i++)
                {
                    TSpan arrayElement = source[i];
                    if (arrayElement == null) throw new ArgumentException($"One or more elements in {nameof(source)} is null", nameof(source));
                    arrayElementOffsets[i - 1] = length;
                    length += arrayElement.Length;
                }

                return new OneOrMoreElements(source, count, arrayElementOffsets, length);
            }

            private readonly int[] arrayElementOffsets;

            public override TSpan this[int index]
            {
                get
                {
                    // Cast to uint so negative values get flagged by this check too.
                    if ((uint)index < (uint)Count)
                    {
                        return array[index];
                    }

                    throw ExceptionUtil.ThrowListIndexOutOfRangeException();
                }
            }

            private OneOrMoreElements(TSpan[] source, int count, int[] arrayElementOffsets, int length)
                : base(source, count, length)
            {
                this.arrayElementOffsets = arrayElementOffsets;
            }

            public override int GetElementOffset(int index)
            {
                if ((uint)index < (uint)Count)
                {
                    return index == 0 ? 0 : arrayElementOffsets[index - 1];
                }

                throw ExceptionUtil.ThrowListIndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the empty <see cref="ReadOnlySpanList{TSpan}"/>.
        /// </summary>
        public static readonly ReadOnlySpanList<TSpan> Empty = new ZeroElements();

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlySpanList{TSpan}"/>.
        /// </summary>
        /// <param name="source">
        /// The elements of the list.
        /// </param>
        /// <returns>
        /// The initialized <see cref="ReadOnlySpanList{TSpan}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more elements in <paramref name="source"/> are <see langword="null"/>.
        /// </exception>
        public static ReadOnlySpanList<TSpan> Create(IEnumerable<TSpan> source)
        {
            if (source is ReadOnlySpanList<TSpan> readOnlySpanList) return readOnlySpanList;
            var array = source.ToArrayEx();
            int count = array.Length;
            if (count == 0) return Empty;
            return OneOrMoreElements.Create(array, count);
        }

        private readonly TSpan[] array;

        /// <summary>
        /// Gets the number of spanned elements in the list.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the length of this <see cref="ReadOnlySpanList{TSpan}"/>.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the spanned element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the spanned element to get.
        /// </param>
        /// <returns>
        /// The spanned element at the specified index in the read-only list.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="Count"/>.
        /// </exception>
        public abstract TSpan this[int index] { get; }

        private ReadOnlySpanList(TSpan[] source, int count, int length)
        {
            array = source;
            Count = count;
            Length = length;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the list.
        /// </returns>
#if NET5_0_OR_GREATER
        public ArrayEnumerator<TSpan> GetEnumerator() => new(array, Count);
#else
        public ArrayEnumerator<TSpan> GetEnumerator() => new ArrayEnumerator<TSpan>(array, Count);
#endif

        IEnumerator<TSpan> IEnumerable<TSpan>.GetEnumerator() => new ArrayEnumerator<TSpan>(array, Count);

        IEnumerator IEnumerable.GetEnumerator() => new ArrayEnumerator<TSpan>(array, Count);

        /// <summary>
        /// Gets the start position of the spanned element at the specified index
        /// relative to the start position of the first element.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the spanned element.
        /// </param>
        /// <returns>
        /// The start position of the spanned element relative to the start position of the first element.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="Count"/>.
        /// </exception>
        public abstract int GetElementOffset(int index);
    }
}
