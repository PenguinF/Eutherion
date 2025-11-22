#region License
/*********************************************************************************
 * JsonStringSegmentSyntax.cs
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
using System.Text;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains a segment of a string literal.
    /// </summary>
    /// <remarks>
    /// These are not considered to be separate tokens as they don't play a separate role in a grammar.
    /// Nor are they significant outside of the context of a string literal, and so they do not implement <see cref="IGreenJsonSymbol"/>.
    /// However, these classes do expose a structure similar to regular syntax nodes, so they follow some of their conventions.
    /// </remarks>
    public abstract class JsonStringSegmentSyntax : ISpan
    {
        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public abstract int Length { get; }

        internal JsonStringSegmentSyntax() { }

        // Would conceptually be a visitor, except ReadOnlySpan<char> is not accepted as a generic type parameter.
        internal abstract void AppendToStringLiteralValue(StringBuilder valueBuilder, ReadOnlySpan<char> source);

        internal abstract TResult Accept<T, TResult>(JsonStringSegmentSyntaxVisitor<T, TResult> visitor, T arg);
    }

    /// <summary>
    /// Represents a syntax node which contains a string literal segment with only regular unescaped characters.
    /// </summary>
    public sealed class JsonRegularStringSegmentSyntax : JsonStringSegmentSyntax
    {
        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonRegularStringSegmentSyntax"/> with a specified length.
        /// </summary>
        /// <param name="length">
        /// The length of the text span corresponding with the node to create.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonRegularStringSegmentSyntax"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is 0 or lower.
        /// </exception>
        internal static JsonRegularStringSegmentSyntax Create(int length)
            => length <= 0 ? throw new ArgumentOutOfRangeException(nameof(length))
            : new JsonRegularStringSegmentSyntax(length);

        private JsonRegularStringSegmentSyntax(int length) => Length = length;

        internal override void AppendToStringLiteralValue(StringBuilder valueBuilder, ReadOnlySpan<char> source)
        {
#if NET472
            // .NET 4.7.2 doesn't have the desired overload yet.
            for (int i = 0; i < source.Length; i++) valueBuilder.Append(source[i]);
#else
            valueBuilder.Append(source);
#endif
        }

        internal override TResult Accept<T, TResult>(JsonStringSegmentSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitRegularStringSegmentSyntax(this, arg);
    }

    /// <summary>
    /// Represents a syntax node containing a single escaped character.
    /// </summary>
    public sealed class JsonSimpleEscapeSequenceSyntax : JsonStringSegmentSyntax
    {
        /// <summary>
        /// Represents the length of a simple escape sequence within a string literal.
        /// </summary>
        public const int SimpleEscapeSequenceLength = 2;

        /// <summary>
        /// Returns the singleton instance.
        /// </summary>
        public static JsonSimpleEscapeSequenceSyntax Value { get; }
#if NET5_0_OR_GREATER
            = new();
#else
            = new JsonSimpleEscapeSequenceSyntax();
#endif

        private static char SimpleEscapeSequenceValue(ReadOnlySpan<char> source)
        {
            char escapedChar = source[1];
            switch (escapedChar)
            {
                case CStyleStringLiteral.QuoteCharacter:
                case CStyleStringLiteral.EscapeCharacter:
                case '/':
                    return escapedChar;
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case 'v':
                    return '\v';
                default:
                    // JsonParser makes sure this never happens.
                    Debug.Assert(false);
                    throw new UnreachableException();
            }
        }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => SimpleEscapeSequenceLength;

        private JsonSimpleEscapeSequenceSyntax() { }

        internal override void AppendToStringLiteralValue(StringBuilder valueBuilder, ReadOnlySpan<char> source)
            => valueBuilder.Append(SimpleEscapeSequenceValue(source));

        internal override TResult Accept<T, TResult>(JsonStringSegmentSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitSimpleEscapeSequenceSyntax(this, arg);
    }

    /// <summary>
    /// Represents a syntax node containing a unicode escape sequence.
    /// </summary>
    public sealed class JsonUnicodeEscapeSequenceSyntax : JsonStringSegmentSyntax
    {
        /// <summary>
        /// Represents the expected length of the character sequence after the "\u" in a string literal.
        /// </summary>
        public const int ExpectedHexValueLength = 4;

        /// <summary>
        /// Represents the length of a unicode escape sequence within a string literal.
        /// </summary>
        // "\u" plus 4 hexadecimal digits.
        public const int UnicodeEscapeSequenceLength = JsonSpecialCharacter.SingleCharacterLength * 2 + ExpectedHexValueLength;

        /// <summary>
        /// Returns the singleton instance.
        /// </summary>
        public static JsonUnicodeEscapeSequenceSyntax Value { get; }
#if NET5_0_OR_GREATER
            = new();
#else
            = new JsonUnicodeEscapeSequenceSyntax();
#endif

        private static char UnicodeEscapeSequenceValue(ReadOnlySpan<char> source)
        {
            int unicodeValue = 0;
            for (int i = 0; i < ExpectedHexValueLength; i++)
            {
                // 1 hex character = 4 bits.
                unicodeValue <<= 4;
                char hexChar = source[i + 2];
                if ('0' <= hexChar && hexChar <= '9')
                {
                    unicodeValue = unicodeValue + hexChar - '0';
                }
                else if ('a' <= hexChar && hexChar <= 'f')
                {
                    const int aValue = 'a' - 10;
                    unicodeValue = unicodeValue + hexChar - aValue;
                }
                else
                {
                    Debug.Assert('A' <= hexChar && hexChar <= 'F');
                    const int aValue = 'A' - 10;
                    unicodeValue = unicodeValue + hexChar - aValue;
                }
            }
            return (char)unicodeValue;
        }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => UnicodeEscapeSequenceLength;

        private JsonUnicodeEscapeSequenceSyntax() { }

        internal override void AppendToStringLiteralValue(StringBuilder valueBuilder, ReadOnlySpan<char> source)
            => valueBuilder.Append(UnicodeEscapeSequenceValue(source));

        internal override TResult Accept<T, TResult>(JsonStringSegmentSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitUnicodeEscapeSequenceSyntax(this, arg);
    }
}
