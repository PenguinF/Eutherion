#region License
/*********************************************************************************
 * ReadOnlySeparatedSpanList.cs
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
    /// Represents a read-only list of spanned elements that can be accessed by index and that are separated by a separator.
    /// </summary>
    /// <typeparam name="TSpan">
    /// The type of spanned elements in the read-only list.
    /// </typeparam>
    /// <typeparam name="TSeparator">
    /// The type of separator between successive elements in the read-only list.
    /// </typeparam>
    public abstract class ReadOnlySeparatedSpanList<TSpan, TSeparator> : IReadOnlyList<TSpan>, ISpan where TSpan : ISpan where TSeparator : ISpan
    {
        private class ZeroElements : ReadOnlySeparatedSpanList<TSpan, TSeparator>
        {
            public override TSpan this[int index] => throw ExceptionUtil.ThrowListIndexOutOfRangeException();

            public ZeroElements() : base(Array.Empty<TSpan>(), 0, 0, 0) { }

            public override IEnumerable<Union<TSpan, TSeparator>> AllElements => EmptyEnumerable<Union<TSpan, TSeparator>>.Instance;

            public override int GetElementOffset(int index) => throw ExceptionUtil.ThrowListIndexOutOfRangeException();

            public override int GetSeparatorOffset(int index) => throw ExceptionUtil.ThrowListIndexOutOfRangeException();

            public override int GetElementOrSeparatorOffset(int index) => throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        private class OneOrMoreElements : ReadOnlySeparatedSpanList<TSpan, TSeparator>
        {
            // Static because of necessary preprocessing.
            public static OneOrMoreElements Create(TSpan[] source, int count, TSeparator separator)
            {
                if (source[0] == null) throw new ArgumentException($"One or more elements in {nameof(source)} is null", nameof(source));
                int length = source[0].Length;
                int separatorLength = separator.Length;
                int[] arrayElementOffsets = new int[count];  // First element == 0, no need to initialize it explicitly.

                for (int i = 1; i < count; i++)
                {
                    TSpan arrayElement = source[i];
                    if (arrayElement == null) throw new ArgumentException($"One or more elements in {nameof(source)} is null", nameof(source));
                    length += separatorLength;
                    arrayElementOffsets[i] = length;
                    length += arrayElement.Length;
                }

                return new OneOrMoreElements(source, count, separator, arrayElementOffsets, length);
            }

            private readonly TSeparator separator;
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

            private OneOrMoreElements(TSpan[] source, int count, TSeparator separator, int[] arrayElementOffsets, int length)
                : base(source, count, length, count * 2 - 1)
            {
                this.separator = separator;
                this.arrayElementOffsets = arrayElementOffsets;
            }

            public override IEnumerable<Union<TSpan, TSeparator>> AllElements
            {
                get
                {
                    yield return array[0];

                    for (int i = 1; i < array.Length; i++)
                    {
                        yield return separator;
                        yield return array[i];
                    }
                }
            }

            public override int GetElementOffset(int index)
            {
                if ((uint)index < (uint)Count)
                {
                    return arrayElementOffsets[index];
                }

                throw ExceptionUtil.ThrowListIndexOutOfRangeException();
            }

            public override int GetSeparatorOffset(int index)
            {
                index++;

                if ((uint)index < (uint)Count)
                {
                    return arrayElementOffsets[index] - separator.Length;
                }

                throw ExceptionUtil.ThrowListIndexOutOfRangeException();
            }

            public override int GetElementOrSeparatorOffset(int index)
            {
                if ((uint)index < (uint)AllElementCount)
                {
                    int offset = arrayElementOffsets[(index + 1) >> 1];
                    if ((index & 1) != 0) offset -= separator.Length;
                    return offset;
                }

                throw ExceptionUtil.ThrowListIndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the empty <see cref="ReadOnlySeparatedSpanList{TSpan, TSeparator}"/>.
        /// </summary>
        public static readonly ReadOnlySeparatedSpanList<TSpan, TSeparator> Empty = new ZeroElements();

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlySeparatedSpanList{TSpan, TSeparator}"/>.
        /// </summary>
        /// <param name="source">
        /// The elements of the list.
        /// </param>
        /// <param name="separator">
        /// The separator between successive elements of the list.
        /// </param>
        /// <returns>
        /// The initialized <see cref="ReadOnlySeparatedSpanList{TSpan, TSeparator}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> and/or <paramref name="separator"/> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more elements in <paramref name="source"/> are <see langword="null"/>.
        /// </exception>
        public static ReadOnlySeparatedSpanList<TSpan, TSeparator> Create(IEnumerable<TSpan> source, TSeparator separator)
        {
            var array = source.ToArrayEx();
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            int count = array.Length;
            if (count == 0) return Empty;
            return OneOrMoreElements.Create(array, count, separator);
        }

        private readonly TSpan[] array;

        /// <summary>
        /// Gets the number of spanned elements in the list, excluding the separators.
        /// See also: <seealso cref="AllElementCount"/>.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the length of this <see cref="ReadOnlySeparatedSpanList{TSpan, TSeparator}"/>.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the number of spanned elements in the list, including the separators.
        /// See also: <seealso cref="Count"/>.
        /// </summary>
        public int AllElementCount { get; }

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

        private ReadOnlySeparatedSpanList(TSpan[] source, int count, int length, int allElementCount)
        {
            array = source;
            Count = count;
            Length = length;
            AllElementCount = allElementCount;
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
        /// Enumerates all elements of the list, including separators.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> that enumerates all elements of the list, including separators.
        /// </returns>
        public abstract IEnumerable<Union<TSpan, TSeparator>> AllElements { get; }

        /// <summary>
        /// Gets the start position of the spanned element at the specified index
        /// relative to the start position of the first element.
        /// See also: <seealso cref="GetSeparatorOffset"/>, <seealso cref="GetElementOrSeparatorOffset"/>.
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

        /// <summary>
        /// Gets the start position of the separator at the specified index
        /// relative to the start position of the first element.
        /// The number of separators is always one less than the number of elements.
        /// See also: <seealso cref="GetElementOffset"/>, <seealso cref="GetElementOrSeparatorOffset"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the separator.
        /// </param>
        /// <returns>
        /// The start position of the separator relative to the start position of the first element.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="Count"/> - 1.
        /// </exception>
        public abstract int GetSeparatorOffset(int index);

        /// <summary>
        /// Gets the start position of the spanned element or separator at the specified index
        /// relative to the start position of the first element.
        /// See also: <seealso cref="GetElementOffset"/>, <seealso cref="GetSeparatorOffset"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the spanned element or separator.
        /// </param>
        /// <returns>
        /// The start position of the spanned element or separator relative to the start position of the first element.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="AllElementCount"/>.
        /// </exception>
        public abstract int GetElementOrSeparatorOffset(int index);
    }
}
