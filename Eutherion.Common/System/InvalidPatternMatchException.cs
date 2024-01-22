#region License
/*********************************************************************************
 * InvalidPatternMatchException.cs
 *
 * Copyright (c) 2004-2024 Henk Nicolai
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

using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// Occurs when there is no specified implementation for a pattern to match.
    /// </summary>
    [Serializable]
    public class InvalidPatternMatchException : Exception
    {
        /// <summary>
        /// Gets the default message for an <see cref="InvalidPatternMatchException"/>.
        /// </summary>
        public static string InvalidPatternMatchMessage { get; } = "Missing matching implementation for pattern.";

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidPatternMatchException"/> with a default message that describes the error.
        /// </summary>
        public InvalidPatternMatchException() : base(InvalidPatternMatchMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidPatternMatchException"/> with a message that describes the error.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InvalidPatternMatchException(string message) : base(message ?? InvalidPatternMatchMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidPatternMatchException"/> with a message that describes the error.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the exception.
        /// </param>
        public InvalidPatternMatchException(string message, Exception innerException) : base(message ?? InvalidPatternMatchMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidPatternMatchException"/> with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// The class name is <see langword="null"/> or <see cref="Exception.HResult"/> is zero (0).
        /// </exception>
        protected InvalidPatternMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
