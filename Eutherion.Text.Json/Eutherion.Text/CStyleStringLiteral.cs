﻿#region License
/*********************************************************************************
 * CStyleStringLiteral.cs
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

using System.Runtime.CompilerServices;

namespace Eutherion.Text
{
    /// <summary>
    /// Contains utility methods to deal with string literals, in particular to support the implementation
    /// of an escape character standard, which is shared by multiple languages based on C.
    /// </summary>
    public static class CStyleStringLiteral
    {
        /// <summary>
        /// The character used to delimit string literals.
        /// </summary>
        public const char QuoteCharacter = '"';

        /// <summary>
        /// The character used to escape characters in string literals.
        /// </summary>
        public const char EscapeCharacter = '\\';

        /// <summary>
        /// Generates the escape sequence string for a character.
        /// </summary>
        /// <param name="c">
        /// The character to escape.
        /// </param>
        /// <returns>
        /// The escape sequence string.
        /// </returns>
        public static string EscapedCharacterString(char c)
        {
            // Disable IDE0066 as syntax is not yet available in C# 7, used for .NET 4.7.2 target.
#if !NET472
#pragma warning disable IDE0066 // Convert switch statement to expression
#endif
            switch (c)
#if !NET472
#pragma warning restore IDE0066 // Convert switch statement to expression
#endif
            {
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                case '\t': return "\\t";
                case '\v': return "\\v";
                case QuoteCharacter: return "\\\"";
                case EscapeCharacter: return "\\\\";
                default: return $"\\u{(int)c:x4}";
            }
        }

        private const char HighestControlCharacter = '\u009f';
        private const int ControlCharacterIndexLength = HighestControlCharacter + 1;

        // An index in memory is as fast as it gets for determining whether or not a character should be escaped.
        private static readonly bool[] CharacterMustBeEscapedIndex;

        static CStyleStringLiteral()
        {
            // Will be initialized with all false values.
            CharacterMustBeEscapedIndex = new bool[ControlCharacterIndexLength];

            //https://www.compart.com/en/unicode/category/Cc
            for (int i = 0; i < ' '; i++) CharacterMustBeEscapedIndex[i] = true;
            for (int i = '\u007f'; i <= HighestControlCharacter; i++) CharacterMustBeEscapedIndex[i] = true;

            // Individual characters.
            CharacterMustBeEscapedIndex[QuoteCharacter] = true;
            CharacterMustBeEscapedIndex[EscapeCharacter] = true;
        }

        /// <summary>
        /// Returns whether or not a character must be escaped when in a JSON string.
        /// </summary>
        /// <param name="c">
        /// The character to check.
        /// </param>
        /// <returns>
        /// Whether or not this character must be escaped when in a JSON string.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CharacterMustBeEscaped(char c)
        {
            if (c < ControlCharacterIndexLength) return CharacterMustBeEscapedIndex[c];

            // Express this as two inequality conditions so second condition may not have to be evaluated.
            //https://www.compart.com/en/unicode/category/Zl - line separator
            //https://www.compart.com/en/unicode/category/Zp - paragraph separator
            return c >= '\u2028' && c <= '\u2029';
        }
    }
}
