#region License
/*********************************************************************************
 * SafeLazyObjectCollection.cs
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
using System.Threading;

namespace Eutherion.Collections
{
    /// <summary>
    /// Represents a collection of objects which are initialized lazily with thread-safety.
    /// </summary>
    /// <typeparam name="TObject">
    /// The type of objects to create.
    /// </typeparam>
    /// <remarks>
    /// This class uses the race-to-initialize pattern. This guarantees that the internal array always returns the same object
    /// for a certain index, but it is possible that multiple threads race and evaluate the constructor function,
    /// which should therefore be cheap.
    /// </remarks>
    public class SafeLazyObjectCollection<TObject> : IReadOnlyList<TObject> where TObject : class
    {
        // Initializes each object during enumeration.
        private class SafeEnumerator : IEnumerator<TObject>
        {
            private readonly SafeLazyObjectCollection<TObject> collection;
            private int index;

#if NET472
            public TObject Current { get; private set; } = null;
#else
            // Override static null analysis by relying on contract that 'Current' is only evaluated after MoveNext() returns true.
            public TObject Current { get; private set; } = null!;
#endif

            public SafeEnumerator(SafeLazyObjectCollection<TObject> collection) => this.collection = collection;

            public bool MoveNext()
            {
                if (index < collection.Count)
                {
                    Current = collection[index];
                    index++;
                    return true;
                }
                index++;
#if NET472
                Current = null;
#else
                Current = null!;
#endif
                return false;
            }

            object IEnumerator.Current => Current;
            void IDisposable.Dispose() { }
            void IEnumerator.Reset() { }
        }

        private readonly TObject[] Arr;
        private readonly Func<int, TObject> ElementConstructor;

        /// <summary>
        /// Gets the number of objects in this collection.
        /// </summary>
        public int Count => Arr.Length;

        /// <summary>
        /// Initializes a new instance of <see cref="SafeLazyObjectCollection{TObject}"/>.
        /// </summary>
        /// <param name="count">
        /// The number of objects in the collection.
        /// </param>
        /// <param name="elementConstructor">
        /// The constructor with which to initialize an object at a given index.
        /// Note that if multiple threads race to construct an object, they will all call the constructor.
        /// Only one of the created objects is stored in the collection, and henceforth returned.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="elementConstructor"/> is <see langword="null"/>.
        /// </exception>
        public SafeLazyObjectCollection(int count, Func<int, TObject> elementConstructor)
        {
            Arr = count > 0 ? new TObject[count] : Array.Empty<TObject>();
            ElementConstructor = elementConstructor ?? throw new ArgumentNullException(nameof(elementConstructor));
        }

        /// <summary>
        /// Gets the object at the specified index in the collection.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the object to get.
        /// </param>
        /// <returns>
        /// The initialized object at the specified index in the collection.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="Count"/>.
        /// </exception>
        public TObject this[int index]
        {
            get
            {
                // Cast to uint so negative values get flagged by this check too.
                if ((uint)index < (uint)Count)
                {
                    if (Arr[index] == null)
                    {
                        // Validate that constructor() returned a non-null value.
                        TObject constructedObject = ElementConstructor(index);
                        if (constructedObject == null)
                        {
                            throw new InvalidOperationException(
                                $"{nameof(ElementConstructor)} returned null for index {index} but should have returned an initialized object.");
                        }

                        // Replace with an initialized value as an atomic operation.
#if NET472
                        Interlocked.CompareExchange(ref Arr[index], constructedObject, null);
#else
                        // Static null analysis ignores that arrays are initialized with null values and so flags 'null' as a false positive.
                        Interlocked.CompareExchange(ref Arr[index], constructedObject, null!);
#endif
                    }

                    return Arr[index];
                }

                throw ExceptionUtil.ThrowListIndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the list.
        /// </returns>
        public IEnumerator<TObject> GetEnumerator() => new SafeEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
