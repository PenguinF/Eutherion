﻿#region License
/*********************************************************************************
 * JsonErrorCode.cs
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
    /// Enumerates classes of json errors.
    /// </summary>
    public enum JsonErrorCode
    {
        /// <summary>
        /// The error code is unspecified, or unknown. This is the default value.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Occurs when a symbol character is not recognized.
        /// This error has a single <see cref="char"/> <see cref="JsonErrorInfoParameter"/> representing the unrecognized symbol character.
        /// </summary>
        UnexpectedSymbol,

        /// <summary>
        /// Occurs when a multi-line comment is not terminated before the end of the file.
        /// </summary>
        UnterminatedMultiLineComment,

        /// <summary>
        /// Occurs when a string literal is not terminated before the end of the file.
        /// </summary>
        UnterminatedString,

        /// <summary>
        /// Occurs when an escape sequence in a string literal is not recognized.
        /// This error has a single <see cref="string"/> <see cref="JsonErrorInfoParameter"/> representing the unrecognized escape sequence.
        /// </summary>
        UnrecognizedEscapeSequence,

        /// <summary>
        /// Occurs when control characters appear in a string literal, which should be represented by an escape sequence instead.
        /// This error has a single <see cref="char"/> <see cref="JsonErrorInfoParameter"/> representing the illegal control character.
        /// </summary>
        IllegalControlCharacterInString,

        /// <summary>
        /// Occurs when multiple values are defined at the root level.
        /// </summary>
        ExpectedEof,

        /// <summary>
        /// Occurs when an object is not closed before the end of the file.
        /// </summary>
        UnexpectedEofInObject,

        /// <summary>
        /// Occurs when an array is not closed before the end of the file.
        /// </summary>
        UnexpectedEofInArray,

        /// <summary>
        /// Occurs when a control symbol is encountered within an object where a ',', '}' or ':' symbol would be expected.
        /// </summary>
        ControlSymbolInObject,

        /// <summary>
        /// Occurs when a control symbol is encountered within an array where a ',' or ']' symbol would be expected.
        /// </summary>
        ControlSymbolInArray,

        /// <summary>
        /// Occurs when a property key is not a string.
        /// </summary>
        InvalidPropertyKey,

        /// <summary>
        /// Occurs when a property key is missing before a ':' symbol.
        /// </summary>
        MissingPropertyKey,

        /// <summary>
        /// Occurs when a value is missing after a ':' or ',' symbol.
        /// </summary>
        MissingValue,

        /// <summary>
        /// Occurs when a value is not recognized.
        /// This error has a single <see cref="string"/> <see cref="JsonErrorInfoParameter"/> representing the unrecognized value.
        /// </summary>
        UnrecognizedValue,

        /// <summary>
        /// Occurs when multiple property key sections are specified.
        /// </summary>
        MultiplePropertyKeySections,

        /// <summary>
        /// Occurs when multiple property keys are specified before a ':' symbol.
        /// </summary>
        MultiplePropertyKeys,

        /// <summary>
        /// Occurs when multiple values are defined within an object or array.
        /// </summary>
        MultipleValues,

        /// <summary>
        /// Occurs when a Json parse tree exceeds the maximum allowed depth.
        /// </summary>
        ParseTreeTooDeep,
    }
}
