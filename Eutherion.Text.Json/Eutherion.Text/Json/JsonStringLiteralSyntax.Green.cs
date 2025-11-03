#region License
/*********************************************************************************
 * JsonStringLiteralSyntax.Green.cs
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
using System.Text;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a string literal value syntax node.
    /// </summary>
    public sealed class GreenJsonStringLiteralSyntax : GreenJsonValueSyntax, IGreenJsonSymbol
    {
        /// <summary>
        /// Returns the list of string segments contained in this string literal.
        /// </summary>
        public ReadOnlySpanList<JsonStringSegmentSyntax> Segments { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Segments.Length + JsonSpecialCharacter.SingleCharacterLength * 2;  // Add opening and closing quote characters.

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public JsonSymbolType SymbolType => JsonSymbolType.StringLiteral;

        /// <summary>
        /// Returns <see langword="true"/> because this is a <see cref="GreenJsonStringLiteralSyntax"/> instance.
        /// </summary>
        public override bool IsStringLiteral => true;

        private GreenJsonStringLiteralSyntax(ReadOnlySpanList<JsonStringSegmentSyntax> segments) => Segments = segments;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonStringLiteralSyntax"/> from an <see cref="ArrayBuilder{T}"/>.
        /// This empties the array builder.
        /// </summary>
        /// <param name="source">
        /// The builder with string segments contained in this string literal.
        /// </param>
        /// <returns>
        /// The new <see cref="GreenJsonStringLiteralSyntax"/> representing the string literal with the given segments.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more elements in <paramref name="source"/> are <see langword="null"/>.
        /// </exception>
        public static GreenJsonStringLiteralSyntax FromBuilder(ArrayBuilder<JsonStringSegmentSyntax> source)
        {
            return new GreenJsonStringLiteralSyntax(ReadOnlySpanList<JsonStringSegmentSyntax>.FromBuilder(source));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonStringLiteralSyntax"/>.
        /// </summary>
        /// <param name="source">
        /// The enumeration of string segments contained in this string literal.
        /// </param>
        /// <returns>
        /// The new <see cref="GreenJsonStringLiteralSyntax"/> representing the string literal with the given segments.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more elements in <paramref name="source"/> are <see langword="null"/>.
        /// </exception>
        public static GreenJsonStringLiteralSyntax Create(IEnumerable<JsonStringSegmentSyntax> source)
        {
            return new GreenJsonStringLiteralSyntax(ReadOnlySpanList<JsonStringSegmentSyntax>.Create(source));
        }

        internal string CalculateValue(ReadOnlySpan<char> source)
        {
            // Skip opening quote.
            int currentIndex = JsonSpecialCharacter.SingleCharacterLength;

            StringBuilder valueBuilder = new StringBuilder();

            foreach (var segment in Segments)
            {
                int length = segment.Length;
                segment.AppendToStringLiteralValue(valueBuilder, source.Slice(currentIndex, length));
                currentIndex += length;
            }

            return valueBuilder.ToString();
        }

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitStringLiteralSyntax(this, arg);
    }
}
