﻿#region License
/*********************************************************************************
 * JsonParseErrors.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Contains methods to generate parser errors.
    /// </summary>
    public static class JsonParseErrors
    {
        /// <summary>
        /// Creates a <see cref="JsonErrorInfo"/> for when a symbol character is not recognized.
        /// </summary>
        /// <param name="unexpectedCharacter">
        /// The unexpected symbol character.
        /// </param>
        /// <param name="position">
        /// The position of the text span where the error occurred.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonErrorInfo"/>.
        /// </returns>
        public static JsonErrorInfo UnexpectedSymbol(char unexpectedCharacter, int position)
#if NET5_0_OR_GREATER
            => new(
#else
            => new JsonErrorInfo(
#endif
                JsonErrorCode.UnexpectedSymbol, position, JsonSpecialCharacter.SingleCharacterLength,
                new JsonErrorInfoParameter<char>(unexpectedCharacter));

        /// <summary>
        /// Creates a <see cref="JsonErrorInfo"/> for when a multi-line comment is not terminated before the end of the file.
        /// </summary>
        /// <param name="position">
        /// The position of the text span where the error occurred.
        /// </param>
        /// <param name="length">
        /// The length of the text span where the error occurred.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonErrorInfo"/>.
        /// </returns>
        public static JsonErrorInfo UnterminatedMultiLineComment(int position, int length)
#if NET5_0_OR_GREATER
            => new(
#else
            => new JsonErrorInfo(
#endif
                JsonErrorCode.UnterminatedMultiLineComment, JsonErrorLevel.Warning, position, length);

        /// <summary>
        /// Creates a <see cref="JsonErrorInfo"/> for when a string literal is not terminated before the end of the file.
        /// </summary>
        /// <param name="position">
        /// The position of the text span where the error occurred.
        /// </param>
        /// <param name="length">
        /// The length of the text span where the error occurred.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonErrorInfo"/>.
        /// </returns>
        public static JsonErrorInfo UnterminatedString(int position, int length)
#if NET5_0_OR_GREATER
            => new(
#else
            => new JsonErrorInfo(
#endif
                JsonErrorCode.UnterminatedString, position, length);

        /// <summary>
        /// Creates a <see cref="JsonErrorInfo"/> for when an escape sequence in a string literal is not recognized.
        /// </summary>
        /// <param name="escapeSequence">
        /// The escape sequence which generated the error.
        /// </param>
        /// <param name="position">
        /// The position of the text span where the error occurred.
        /// </param>
        /// <param name="length">
        /// The length of the text span where the error occurred.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonErrorInfo"/>.
        /// </returns>
        public static JsonErrorInfo UnrecognizedEscapeSequence(string escapeSequence, int position, int length)
#if NET5_0_OR_GREATER
            => new(
#else
            => new JsonErrorInfo(
#endif
                JsonErrorCode.UnrecognizedEscapeSequence, position, length,
                new JsonErrorInfoParameter<string>(escapeSequence));

        /// <summary>
        /// Creates a <see cref="JsonErrorInfo"/> for when control characters appear in a string literal, which should be represented by an escape sequence instead.
        /// </summary>
        /// <param name="illegalControlCharacter">
        /// The control character which appeared in the string literal.
        /// </param>
        /// <param name="position">
        /// The position of the text span where the error occurred.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonErrorInfo"/>.
        /// </returns>
        public static JsonErrorInfo IllegalControlCharacterInString(char illegalControlCharacter, int position)
#if NET5_0_OR_GREATER
            => new(
#else
            => new JsonErrorInfo(
#endif
                JsonErrorCode.IllegalControlCharacterInString, position, JsonSpecialCharacter.SingleCharacterLength,
                new JsonErrorInfoParameter<char>(illegalControlCharacter));

        /// <summary>
        /// Creates a <see cref="JsonErrorInfo"/> for when a value is not recognized.
        /// </summary>
        /// <param name="unrecognizedValue">
        /// The value which was not recognized.
        /// </param>
        /// <param name="position">
        /// The position of the text span where the error occurred.
        /// </param>
        /// <param name="length">
        /// The length of the text span where the error occurred.
        /// </param>
        /// <returns>
        /// The new <see cref="JsonErrorInfo"/>.
        /// </returns>
        public static JsonErrorInfo UnrecognizedValue(string unrecognizedValue, int position, int length)
#if NET5_0_OR_GREATER
            => new(
#else
            => new JsonErrorInfo(
#endif
                JsonErrorCode.UnrecognizedValue, position, length,
                new JsonErrorInfoParameter<string>(unrecognizedValue));
    }
}
