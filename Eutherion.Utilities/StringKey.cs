#region License
/*********************************************************************************
 * StringKey.cs
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
using System.Diagnostics;

namespace Eutherion
{
    /// <summary>
    /// Represents an immutable identifier for an arbitrary type.
    /// </summary>
    /// <typeparam name="T">
    /// A unique type value to distinguish between identifiers of different types.
    /// </typeparam>
    [DebuggerDisplay("{Key}")]
    public sealed class StringKey<T> : IEquatable<StringKey<T>>
    {
        /// <summary>
        /// Gets the string representation of this <see cref="StringKey{T}"/>. 
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// Constructs a new instance of <see cref="StringKey{T}"/>,
        /// in which the provided string key is used for equality comparison and hash code generation.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        public StringKey(string key)
            => Key = key ?? throw new ArgumentNullException(nameof(key));

        /// <summary>
        /// Indicates whether this <see cref="StringKey{T}"/> is equal to another <see cref="StringKey{T}"/>.
        /// </summary>
        /// <param name="other">
        /// A <see cref="StringKey{T}"/> to compare with this <see cref="StringKey{T}"/>.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="StringKey{T}"/> is equal to this <see cref="StringKey{T}"/>; otherwise, false.
        /// </returns>
#if NET472
        public bool Equals(StringKey<T> other)
#else
        public bool Equals(StringKey<T>? other)
#endif
            => this == other
            || !(other is null) && Key == other.Key;

        /// <summary>
        /// Indicates whether the specified object is equal to this <see cref="StringKey{T}"/>.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with this <see cref="StringKey{T}"/>.
        /// </param>
        /// <returns>
        /// true if the specified object is equal to this <see cref="StringKey{T}"/>; otherwise, false.
        /// </returns>
#if NET472
        public override bool Equals(object obj)
#else
        public override bool Equals(object? obj)
#endif
            => Equals(obj as StringKey<T>);

        /// <summary>
        /// Generates a hash code based on the stored internal string key.
        /// </summary>
        /// <returns>
        /// The generated hash code.
        /// </returns>
        public override int GetHashCode()
            => Key.GetHashCode();

        /// <summary>
        /// Indicates whether one <see cref="StringKey{T}"/> is equal to another <see cref="StringKey{T}"/>.
        /// </summary>
        /// <param name="first">
        /// The first <see cref="StringKey{T}"/> to compare.
        /// </param>
        /// <param name="second">
        /// The second <see cref="StringKey{T}"/> to compare.
        /// </param>
        /// <returns>
        /// true if both specified <see cref="StringKey{T}"/>s are equal; otherwise, false.
        /// </returns>
#if NET472
        public static bool operator ==(StringKey<T> first, StringKey<T> second)
#else
        public static bool operator ==(StringKey<T>? first, StringKey<T>? second)
#endif
        {
            if (first is null) return second is null;
            if (second is null) return false;
            return first.Key == second.Key;
        }

        /// <summary>
        /// Indicates whether one <see cref="StringKey{T}"/> is different from another <see cref="StringKey{T}"/>.
        /// </summary>
        /// <param name="first">
        /// The first <see cref="StringKey{T}"/> to compare.
        /// </param>
        /// <param name="second">
        /// The second <see cref="StringKey{T}"/> to compare.
        /// </param>
        /// <returns>
        /// true if both specified <see cref="StringKey{T}"/>s are different; otherwise, false.
        /// </returns>
#if NET472
        public static bool operator !=(StringKey<T> first, StringKey<T> second)
#else
        public static bool operator !=(StringKey<T>? first, StringKey<T>? second)
#endif
        {
            if (first is null) return !(second is null);
            if (second is null) return true;
            return first.Key != second.Key;
        }
    }
}
