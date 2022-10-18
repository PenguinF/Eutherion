#region License
/*********************************************************************************
 * ExceptionUtil.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Eutherion
{
    internal static class ExceptionUtil
    {
        // Also 'return' the exception so caller has something to throw as well making e.g. conditional statements syntactically valid.
        [DoesNotReturn]
        internal static InvalidOperationException ThrowInvalidEnumerationOperationException()
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
    }
}
