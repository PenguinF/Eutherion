#region License
/*********************************************************************************
 * SafeLazyObject.cs
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
using System.Threading;

namespace Eutherion.Threading
{
    /// <summary>
    /// Represents the thread-safe lazy evaluation of a parameterless constructor which creates an object.
    /// </summary>
    /// <typeparam name="TObject">
    /// The type of object to create.
    /// </typeparam>
    /// <remarks>
    /// This class uses the race-to-initialize pattern. This guarantees that <see cref="Object"/> always returns the same object,
    /// but it is possible that multiple threads race and evaluate the constructor function, which should therefore be cheap.
    /// </remarks>
    public struct SafeLazyObject<TObject> where TObject : class
    {
#if NET472
        private volatile Func<TObject> Constructor;
        private TObject ConstructedObject;
#else
        private volatile Func<TObject>? Constructor;
        private TObject? ConstructedObject;
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="SafeLazyObject{TValue}"/>.
        /// </summary>
        /// <param name="constructor">
        /// The object constructor. It must not return null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constructor"/> is <see langword="null"/>.
        /// </exception>
        public SafeLazyObject(Func<TObject> constructor)
        {
            Constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            ConstructedObject = null;
        }

        /// <summary>
        /// Gets the result of the constructor.
        /// Note that if multiple threads race to construct the object, they will all call the constructor.
        /// Only one of the created objects is kept, and henceforth returned.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Constructor"/> returned null.
        /// </exception>
        public TObject Object
        {
            get
            {
                // Avoid calling the constructor and CompareExchange() if ConstructedObject is already available.
                if (ConstructedObject == null)
                {
                    // Create a local copy of Constructor, so that if another thread succeeds in setting ConstructedObject 
                    // and also resetting the Constructor field before this thread reaches the CompareExchange statement,
                    // it will not generate a NullReferenceException. The object created on this thread is then discarded.
#if NET472
                    Func<TObject> constructor = Constructor;
#else
                    Func<TObject>? constructor = Constructor;
#endif

                    // Perform CompareExchange on the ConstructedObject rather than the constructor, otherwise threads
                    // that lost the race might return a null ConstructedObject while the winning thread is still busy
                    // constructing it.
                    // If constructor() throws, then it will be re-evaluated by the next caller, and throw again.
                    if (constructor != null)
                    {
                        TObject constructedObject = constructor();

                        // Validate that constructor() returned a non-null value.
                        if (constructedObject == null)
                        {
                            throw new InvalidOperationException(
                                $"{nameof(Constructor)} returned null but should have returned an initialized object.");
                        }

                        if (Interlocked.CompareExchange(ref ConstructedObject, constructedObject, null) == null)
                        {
                            // For a brief interval here, both Constructor and ConstructedObject != null, which allows
                            // a different thread to evaluate the constructor.

                            // Setting the constructor to null releases any references the constructor may have held implicitly, had it been a closure.
                            Constructor = null;
                        }
                    }
                    else if (ConstructedObject == null)
                    {
                        // The Constructor field can only be null after ConstructedObject has been set to a non-null value,
                        // but the static code analyzer cannot see this.
                        // So this is dead code, and can throw an UnreachableException here to close off this branch to the code analyzer.
                        throw new UnreachableException();
                    }
                }

                return ConstructedObject;
            }
        }
    }
}
