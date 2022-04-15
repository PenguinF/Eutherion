#region License
/*********************************************************************************
 * StringKey.cs
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

#if !NET472
#nullable enable
#endif

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
        /// <paramref name="key"/> is null.
        /// </exception>
        public StringKey(string key)
            => Key = key ?? throw new ArgumentNullException(nameof(key));

#if NET472
        public bool Equals(StringKey<T> other)
#else
        public bool Equals(StringKey<T>? other)
#endif
            => this == other
            || !(other is null) && Key == other.Key;

#if NET472
        public override bool Equals(object obj)
#else
        public override bool Equals(object? obj)
#endif
            => Equals(obj as StringKey<T>);

        public override int GetHashCode()
            => Key.GetHashCode();

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
