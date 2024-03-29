﻿#region License
/*********************************************************************************
 * SpecializedEnumerable.cs
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

using Eutherion;

namespace System.Collections.Generic
{
    /// <summary>
    /// Utility <see cref="IEnumerable{T}"/> that enumerates no elements of the given type.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the enumerated elements.
    /// </typeparam>
    public class EmptyEnumerable<TResult> : IReadOnlyList<TResult>
    {
        /// <summary>
        /// Gets the only <see cref="EmptyEnumerable{TResult}"/> instance.
        /// </summary>
#if NET5_0_OR_GREATER
        public static readonly EmptyEnumerable<TResult> Instance = new();
#else
        public static readonly EmptyEnumerable<TResult> Instance = new EmptyEnumerable<TResult>();
#endif

        private EmptyEnumerable() { }

        /// <summary>
        /// Returns an <see cref="EmptyEnumerator{TResult}"/> which enumerates no elements of the given type.
        /// </summary>
        public IEnumerator<TResult> GetEnumerator() => EmptyEnumerator<TResult>.Instance;

        int IReadOnlyCollection<TResult>.Count => 0;
        TResult IReadOnlyList<TResult>.this[int index] => throw new IndexOutOfRangeException();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Utility <see cref="IEnumerator{T}"/> that enumerates no elements of the given type.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the enumerated elements.
    /// </typeparam>
    public sealed class EmptyEnumerator<TResult> : IEnumerator<TResult>
    {
        /// <summary>
        /// Gets the only <see cref="EmptyEnumerator{TResult}"/> instance.
        /// </summary>
#if NET5_0_OR_GREATER
        public static readonly EmptyEnumerator<TResult> Instance = new();
#else
        public static readonly EmptyEnumerator<TResult> Instance = new EmptyEnumerator<TResult>();
#endif

        private EmptyEnumerator() { }

        void IEnumerator.Reset() { }
        bool IEnumerator.MoveNext() => false;
        TResult IEnumerator<TResult>.Current => throw ExceptionUtility.ThrowInvalidEnumerationOperationException();
        object IEnumerator.Current => throw ExceptionUtility.ThrowInvalidEnumerationOperationException();
        void IDisposable.Dispose() { }
    }

    /// <summary>
    /// Utility <see cref="IEnumerable{T}"/> that enumerates a single element of the given type.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the enumerated elements.
    /// </typeparam>
    public struct SingleElementEnumerable<TResult> : IReadOnlyList<TResult>
    {
        /// <summary>
        /// Gets or sets the element to enumerate.
        /// </summary>
        public TResult Element;

        /// <summary>
        /// Initializes a new instance of <see cref="SingleElementEnumerable{TResult}"/>.
        /// </summary>
        /// <param name="element">
        /// The single element to enumerate.
        /// </param>
        public SingleElementEnumerable(TResult element) => Element = element;

        /// <summary>
        /// Returns a <see cref="SingleElementEnumerator{TResult}"/> which enumerates <see cref="Element"/>.
        /// </summary>
        public IEnumerator<TResult> GetEnumerator() => new SingleElementEnumerator<TResult>(Element);

        int IReadOnlyCollection<TResult>.Count => 1;
        TResult IReadOnlyList<TResult>.this[int index] => index == 0 ? Element : throw new IndexOutOfRangeException();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Utility <see cref="IEnumerator{T}"/> that enumerates a single element of the given type.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the enumerated elements.
    /// </typeparam>
    public sealed class SingleElementEnumerator<TResult> : IEnumerator<TResult>
    {
        private readonly TResult element;
        private int index = -1;

        /// <summary>
        /// Initializes a new instance of <see cref="SingleElementEnumerator{TResult}"/>.
        /// </summary>
        /// <param name="element">
        /// The single element to enumerate.
        /// </param>
        public SingleElementEnumerator(TResult element) => this.element = element;

        // When called from a foreach construct, it goes:
        // MoveNext() Current MoveNext() Dispose()
        void IEnumerator.Reset() => index = -1;
        bool IEnumerator.MoveNext() { index++; return index == 0; }
        TResult IEnumerator<TResult>.Current => index == 0 ? element : throw ExceptionUtility.ThrowInvalidEnumerationOperationException();
#if NET472
        object IEnumerator.Current => index == 0 ? element : throw ExceptionUtility.ThrowInvalidEnumerationOperationException();
#else
        object? IEnumerator.Current => index == 0 ? element : throw ExceptionUtility.ThrowInvalidEnumerationOperationException();
#endif
        void IDisposable.Dispose() { }
    }
}
