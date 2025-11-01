#region License
/*********************************************************************************
 * DisposableResourceCollection.cs
 *
 * Copyright (c) 2004-2025 Henk Nicolai
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
using System.Collections.Generic;

namespace Eutherion
{
    /// <summary>
    /// Contains a collection of disposable resources which are disposed exactly
    /// when the collection is disposed, in reverse order of addition.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public sealed class DisposableResourceCollection : IDisposable
    {
        private readonly object Sentinel = new object();
        private readonly List<IDisposable> DisposableResources = new List<IDisposable>();

        /// <summary>
        /// Gets if this collection has been disposed of.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Adds a resource to be disposed when this <see cref="DisposableResourceCollection"/> is disposed.
        /// </summary>
        /// <param name="disposableResource">
        /// The resource to register for disposal.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// This collection is already disposed of.
        /// </exception>
        public void Add(IDisposable disposableResource)
        {
            lock (Sentinel)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(DisposableResourceCollection));
                }

                DisposableResources.Add(disposableResource);
            }
        }

        /// <summary>
        /// Disposes of all resources that are added to this collection in reverse order of addition.
        /// If any of those resources throws an exception while being disposed, the remaining resources
        /// will still be disposed, after which an <see cref="AggregateException"/> is thrown containing
        /// all caught exceptions.
        /// </summary>
        /// <exception cref="AggregateException">
        /// Occurs when one or more resources threw an exception during disposal.
        /// The <see cref="AggregateException"/> contains all exceptions that are caught in this manner.
        /// </exception>
        public void Dispose()
        {
            bool mustDispose;

            lock (Sentinel)
            {
                mustDispose = !IsDisposed;
                IsDisposed = true;
            }

            if (mustDispose)
            {
                // Instead of failing the entire dispose chain if one Dispose() method throws,
                // collect all caught exceptions and throw AggregateException when everything is done.
#if NET472
                List<Exception> caughtExceptions = null;
#else
                List<Exception>? caughtExceptions = null;
#endif
                for (int i = DisposableResources.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        DisposableResources[i]?.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (caughtExceptions == null) caughtExceptions = new List<Exception>();
                        caughtExceptions.Add(e);
                    }
                }

                // In this particular case, don't keep references around to disposed resources,
                // since technically we don't know what this object's lifetime will be.
                DisposableResources.Clear();

                GC.SuppressFinalize(this);

                if (caughtExceptions != null)
                {
                    throw new AggregateException(caughtExceptions);
                }
            }
        }

        /// <summary>
        /// Finalizes this <see cref="DisposableResourceCollection"/>.
        /// </summary>
        // Class is sealed, so it's ok to not follow the recommended Dispose(bool disposing) design pattern.
        ~DisposableResourceCollection() { }
    }
}
