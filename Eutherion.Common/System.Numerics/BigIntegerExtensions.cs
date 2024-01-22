#region License
/*********************************************************************************
 * BigIntegerExtensions.cs
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

using System.Globalization;

namespace System.Numerics
{
    /// <summary>
    /// Contains extension methods for the <see cref="BigInteger"/> type.
    /// </summary>
    public static class BigIntegerExtensions
    {
#if NET472
        /// <summary>
        /// Returns a locale invariant representation of a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The locale invariant representation.
        /// </returns>
        /// <remarks>
        /// On targets other than .NET 4.7.2, this method is named 'ToStringInvariant()'.
        /// </remarks>
        // Choosing a name for an extension method that also exists for regular System types will result in the compiler
        // complaining about not having a reference to System.Numerics at call sites even if the call resolves to an overload
        // that doesn't require it.
        // Pragmatic but ugly solution is to permute the name. Conceptually this single method would have to be moved to its
        // own assembly.
        // Other .NET targets don't have this problem, they contain an implicit reference to BigInteger.
        public static string ToInvariantString(this BigInteger value)
            => value.ToString(CultureInfo.InvariantCulture);
#else
        /// <summary>
        /// Returns a locale invariant representation of a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The locale invariant representation.
        /// </returns>
        public static string ToStringInvariant(this BigInteger value)
            => value.ToString(CultureInfo.InvariantCulture);
#endif
    }
}
