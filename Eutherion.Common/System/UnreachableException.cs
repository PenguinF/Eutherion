#region License
/*********************************************************************************
 * UnreachableException.cs
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

namespace System
{
    /// <summary>
    /// Occurs on code paths which are supposed to be unreachable.
    /// </summary>
    /// <remarks>
    /// This exception exists to offer an alternative to writing and maintaining dead code which can never be executed.
    /// </remarks>
    public class UnreachableException : Exception
    {
        /// <summary>
        /// Gets the standard message for an <see cref="UnreachableException"/>. This message is never supposed to be shown, and so is not multilingual.
        /// </summary>
        public static string UnreachableExceptionMessage { get; } = "This part of the software was assumed to be unreachable.";

        /// <summary>
        /// Initializes a new instance of <see cref="UnreachableException"/>.
        /// </summary>
        public UnreachableException() : base(UnreachableExceptionMessage)
        {
        }
    }
}
