#region License
/*********************************************************************************
 * ExceptionUtil.cs
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
using System.Diagnostics.CodeAnalysis;

namespace Eutherion
{
    /// <summary>
    /// Contains helper methods to throw common .NET exceptions to access their built-in string resources.
    /// </summary>
    /// <remarks>
    /// None of the methods actually return, but they do return an <see cref="Exception"/> or the expected type
    /// to provide a way to e.g. make conditional statements syntactically valid.
    /// </remarks>
    public static class ExceptionUtil
    {
        /// <summary>
        /// Throws the exception which would be generated if <see cref="IEnumerator.Current"/> is called
        /// on an enumerator with an index that is currently out of bounds.
        /// </summary>
        /// <returns>
        /// This method does not return.
        /// </returns>
        [DoesNotReturn]
        public static InvalidOperationException ThrowInvalidEnumerationOperationException()
        {
            // Referencing IEnumerator.Current throws an InvalidOperationException
            // which we'd like to simulate so it uses the built-in mscorlib resources.
            var enumerator = new List<int>().GetEnumerator();
#if NET472
            object current = ((IEnumerator)enumerator).Current;
#else
            object? current = ((IEnumerator)enumerator).Current;
#endif
            // Use KeepAlive() to increase scope of 'current' and ensure it's evaluated.
            // This is dead code already.
            GC.KeepAlive(current);

            // Default message from mscorlib looks like this:
            //throw new InvalidOperationException("Enumeration has either not started or has already finished.");
            throw new UnreachableException();
        }

        /// <summary>
        /// Throws the exception which would be generated if an element of a collection is accessed
        /// with an index that is currently out of bounds.
        /// </summary>
        /// <returns>
        /// This method does not return.
        /// </returns>
        [DoesNotReturn]
        public static ArgumentOutOfRangeException ThrowListIndexOutOfRangeException()
        {
            var x = new List<int>()[0];
            GC.KeepAlive(x);
            throw new UnreachableException();
        }
    }
}
