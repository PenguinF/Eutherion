﻿#region License
/*********************************************************************************
 * JsonParser.cs
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Eutherion.Text.Json
{
#if NET5_0_OR_GREATER
    // Just ignore this warning for this file.
#pragma warning disable IDE0090 // Use 'new(...)'
#endif

    /// <summary>
    /// Represents a individual generation of a parse tree and errors from a source text in the JSON format.
    /// Based on https://www.json.org/.
    /// </summary>
    public sealed class JsonParser
    {
        /// <summary>
        /// Helper class which is used after the last <see cref="IGreenJsonSymbol"/>,
        /// to reduce nullchecks during parsing, and reduce the chance of programming errors.
        /// </summary>
        private sealed class EofSymbol : IGreenJsonSymbol
        {
            public static readonly EofSymbol Value = new EofSymbol();

            public JsonSymbolType SymbolType => JsonSymbolType.Eof;

            private EofSymbol() { }

            int ISpan.Length => 0;
        }

        private class MaximumDepthExceededException : Exception { }

        /// <summary>
        /// Gets the maximum allowed depth of any Json parse tree. As this is a simple recursive parser,
        /// it would otherwise be vulnerable to StackOverflowExceptions.
        /// </summary>
        public const int DefaultMaximumDepth = 40;

        /// <summary>
        /// Generates a parse tree and errors from a source text in the JSON format.
        /// </summary>
        /// <param name="json">
        /// The source text to parse.
        /// </param>
        /// <returns>
        /// A <see cref="RootJsonSyntax"/> instance which contains a parse tree and list of parse errors.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="json"/> is <see langword="null"/>.
        /// </exception>
        public static RootJsonSyntax Parse(string json) => Parse(json, DefaultMaximumDepth);

        /// <summary>
        /// Generates a parse tree and errors from a source text in the JSON format.
        /// </summary>
        /// <param name="json">
        /// The source text to parse.
        /// </param>
        /// <param name="maximumDepth">
        /// The maximum allowed depth of the parse tree.
        /// </param>
        /// <returns>
        /// A <see cref="RootJsonSyntax"/> instance which contains a parse tree and list of parse errors.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="json"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maximumDepth"/> is 0 or lower.
        /// </exception>
        public static RootJsonSyntax Parse(string json, int maximumDepth) => new JsonParser(json, maximumDepth).Parse();

        /// <summary>
        /// Tokenizes the source Json from start to end.
        /// </summary>
        /// <param name="json">
        /// The Json to tokenize.
        /// </param>
        /// <returns>
        /// A list of <see cref="IGreenJsonSymbol"/> instances together with a list of generated tokenization errors.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="json"/> is <see langword="null"/>.
        /// </exception>
        internal static (List<IGreenJsonSymbol>, ReadOnlyList<JsonErrorInfo>) TokenizeAll(string json)
        {
            var parser = new JsonParser(json, DefaultMaximumDepth);
            var tokens = parser.TokenizeAllHelper().ToList();
            return (tokens, ReadOnlyList<JsonErrorInfo>.FromBuilder(parser.Errors));
        }

        private static RootJsonSyntax CreateParseTreeTooDeepRootSyntax(string json, int startPosition)
            => new RootJsonSyntax(
                json,
                new GreenJsonMultiValueSyntax(
                    new[] { new GreenJsonValueWithBackgroundSyntax(
                        GreenJsonBackgroundListSyntax.Empty,
                        GreenJsonMissingValueSyntax.Value) },
                    GreenJsonBackgroundListSyntax.Create(
                        new GreenJsonBackgroundSyntax[] { GreenJsonWhitespaceSyntax.Create(json.Length) })),
                ReadOnlyList<JsonErrorInfo>.FromBuilder(new ArrayBuilder<JsonErrorInfo> { new JsonErrorInfo(JsonErrorCode.ParseTreeTooDeep, startPosition, 1) }));

        internal const JsonSymbolType ForegroundThreshold = JsonSymbolType.BooleanLiteral;
        internal const JsonSymbolType ValueDelimiterThreshold = JsonSymbolType.Colon;

        private IEnumerator<IGreenJsonSymbol> Tokens;
        private readonly string Json;
        private readonly int MaximumDepth;
        private readonly ArrayBuilder<JsonErrorInfo> Errors = new ArrayBuilder<JsonErrorInfo>();
        private readonly ArrayBuilder<GreenJsonBackgroundSyntax> BackgroundBuilder = new ArrayBuilder<GreenJsonBackgroundSyntax>();

        // Invariant is that this index is always at the start of the yielded symbol.
        private int SymbolStartIndex;

        private IGreenJsonSymbol CurrentToken;

        // Used for parse error reporting.
        private int CurrentLength;

        private int CurrentDepth;

        private JsonParser(string json, int maximumDepth)
        {
            Json = json ?? throw new ArgumentNullException(nameof(json));
            if (maximumDepth <= 0) throw new ArgumentOutOfRangeException(nameof(maximumDepth), maximumDepth, $"{maximumDepth} should be 1 or greater.");
            MaximumDepth = maximumDepth;

#if !NET472
            // Bypass static null analysis, CurrentToken is only used after ShiftToNextForegroundToken() assigns to it.
            // (The heavy lifting is done in Parse(), which assigns to these non-null fields) which is the only publicly accessible method.)
            Tokens = null!;
            CurrentToken = null!;
#endif
        }

        private void Report(JsonErrorInfo errorInfo) => Errors.Add(errorInfo);

        private JsonSymbolType ShiftToNextForegroundToken()
        {
            // Skip background until encountering something meaningful.
            for (; ; )
            {
                IGreenJsonSymbol newToken = Tokens.MoveNext() ? Tokens.Current : EofSymbol.Value;
                CurrentLength += newToken.Length;
                JsonSymbolType symbolType = newToken.SymbolType;

                if (symbolType >= ForegroundThreshold)
                {
                    CurrentToken = newToken;
                    return symbolType;
                }

                BackgroundBuilder.Add((GreenJsonBackgroundSyntax)newToken);
            }
        }

        private GreenJsonBackgroundListSyntax CaptureBackground()
        {
            return GreenJsonBackgroundListSyntax.Create(BackgroundBuilder);
        }

        private (GreenJsonValueSyntax, JsonSymbolType) ParseMap()
        {
            var mapBuilder = new ArrayBuilder<GreenJsonKeyValueSyntax>();
            var keyValueSyntaxBuilder = new ArrayBuilder<GreenJsonMultiValueSyntax>();

            for (; ; )
            {
                // Save CurrentLength for error reporting before parsing the key.
                int keyStart = CurrentLength;
                GreenJsonMultiValueSyntax multiKeyNode = ParseMultiValue(JsonErrorCode.MultiplePropertyKeys);
                GreenJsonValueWithBackgroundSyntax parsedKeyNodeWithBackground = multiKeyNode.ValueNode;
                GreenJsonValueSyntax parsedKeyNode = parsedKeyNodeWithBackground.ContentNode;

                // Analyze if this is an actual, unique property key.
                int parsedKeyNodeStart = keyStart + parsedKeyNodeWithBackground.BackgroundBefore.Length;

                // Expect the key node to be a string literal, so only do error analysis if this is not the case.
                bool gotKey = parsedKeyNode.IsStringLiteral;
                if (!gotKey)
                {
                    gotKey = !parsedKeyNode.IsMissingValue;

                    if (gotKey)
                    {
                        Report(new JsonErrorInfo(
                            JsonErrorCode.InvalidPropertyKey,
                            parsedKeyNodeStart,
                            parsedKeyNode.Length));
                    }
                }

                keyValueSyntaxBuilder.Add(multiKeyNode);

                // Keep parsing multi-values until encountering a non ':'.
                bool gotValueSomewhere = false;
                JsonSymbolType symbolType = CurrentToken.SymbolType;
                bool gotColon = false;
                while (symbolType == JsonSymbolType.Colon)
                {
                    if (gotColon)
                    {
                        // Multiple ':' without a ','.
                        Report(new JsonErrorInfo(
                            JsonErrorCode.MultiplePropertyKeySections,
                            CurrentLength - CurrentToken.Length,
                            CurrentToken.Length));
                    }
                    else
                    {
                        gotColon = true;
                    }

                    // ParseMultiValue() guarantees that the next symbol is never a ValueStartSymbol.
                    var multiValueNode = ParseMultiValue(JsonErrorCode.MultipleValues);
                    gotValueSomewhere |= !multiValueNode.ValueNode.ContentNode.IsMissingValue;
                    keyValueSyntaxBuilder.Add(multiValueNode);
                    symbolType = CurrentToken.SymbolType;
                }

                // One key-value section done.
                var jsonKeyValueSyntax = new GreenJsonKeyValueSyntax(keyValueSyntaxBuilder);

                mapBuilder.Add(jsonKeyValueSyntax);

                bool isComma = symbolType == JsonSymbolType.Comma;
                bool isCurlyClose = symbolType == JsonSymbolType.CurlyClose;

                // '}' directly following a ',' should not report errors.
                // '..., : }' however misses both a key and a value.
                if (isComma || (isCurlyClose && (gotKey || gotColon)))
                {
                    // Report missing property key and/or value.
                    if (!gotKey)
                    {
                        Report(new JsonErrorInfo(
                            JsonErrorCode.MissingPropertyKey,
                            CurrentLength - CurrentToken.Length,
                            CurrentToken.Length));
                    }

                    // Report missing value error from being reported if all value sections are empty.
                    // Example: { "key1":: 2, "key2": , }
                    // Skip the fist value section, it contains the key node.
                    if (!gotValueSomewhere)
                    {
                        Report(new JsonErrorInfo(
                            JsonErrorCode.MissingValue,
                            CurrentLength - CurrentToken.Length,
                            CurrentToken.Length));
                    }
                }

                if (!isComma)
                {
                    if (isCurlyClose)
                    {
                        return (new GreenJsonMapSyntax(mapBuilder, missingCurlyClose: false), ShiftToNextForegroundToken());
                    }

                    // ']', EOF; assume missing closing bracket '}'.
                    Report(new JsonErrorInfo(
                        symbolType == JsonSymbolType.Eof ? JsonErrorCode.UnexpectedEofInObject : JsonErrorCode.ControlSymbolInObject,
                        CurrentLength - CurrentToken.Length,
                        CurrentToken.Length));

                    return (new GreenJsonMapSyntax(mapBuilder, missingCurlyClose: true), symbolType);
                }
            }
        }

        private (GreenJsonValueSyntax, JsonSymbolType) ParseList()
        {
            var listBuilder = new ArrayBuilder<GreenJsonMultiValueSyntax>();

            for (; ; )
            {
                GreenJsonMultiValueSyntax parsedValueNode = ParseMultiValue(JsonErrorCode.MultipleValues);

                // Always add each value, because it may contain background symbols.
                listBuilder.Add(parsedValueNode);

                // ParseMultiValue() guarantees that the next symbol is never a ValueStartSymbol.
                JsonSymbolType symbolType = CurrentToken.SymbolType;
                if (symbolType == JsonSymbolType.Comma)
                {
                    if (parsedValueNode.ValueNode.ContentNode.IsMissingValue)
                    {
                        // Two commas or '[,'.
                        Report(new JsonErrorInfo(
                            JsonErrorCode.MissingValue,
                            CurrentLength - CurrentToken.Length,
                            CurrentToken.Length));
                    }
                }
                else if (symbolType == JsonSymbolType.BracketClose)
                {
                    return (new GreenJsonListSyntax(listBuilder, missingSquareBracketClose: false), ShiftToNextForegroundToken());
                }
                else
                {
                    // ':', '}', EOF; assume missing closing bracket ']'.
                    Report(new JsonErrorInfo(
                        symbolType == JsonSymbolType.Eof ? JsonErrorCode.UnexpectedEofInArray : JsonErrorCode.ControlSymbolInArray,
                        CurrentLength - CurrentToken.Length,
                        CurrentToken.Length));

                    return (new GreenJsonListSyntax(listBuilder, missingSquareBracketClose: true), symbolType);
                }
            }
        }

        private void ParseValues(ArrayBuilder<GreenJsonValueWithBackgroundSyntax> valueNodesBuilder, JsonErrorCode multipleValuesErrorCode)
        {
            JsonSymbolType symbolType = ShiftToNextForegroundToken();
            if (symbolType >= ValueDelimiterThreshold) return;

            // Invariant: discriminated != null && !discriminated.IsOption1().
            for (; ; )
            {
                // Have to clear the BackgroundBuilder before entering a recursive call.
                var backgroundBefore = CaptureBackground();

                GreenJsonValueSyntax valueNode;
                if (symbolType == JsonSymbolType.CurlyOpen)
                {
                    (valueNode, symbolType) = ParseMap();
                }
                else if (symbolType == JsonSymbolType.BracketOpen)
                {
                    (valueNode, symbolType) = ParseList();
                }
                else
                {
                    // All other IGreenJsonSymbols derive from GreenJsonValueSyntax.
                    valueNode = (GreenJsonValueSyntax)CurrentToken;
                    symbolType = ShiftToNextForegroundToken();
                }

                // Always add the value node, also if it contains an undefined value.
                valueNodesBuilder.Add(new GreenJsonValueWithBackgroundSyntax(backgroundBefore, valueNode));

                // If SymbolType >= JsonSymbolType.ValueDelimiterThreshold in the first iteration,
                // it means that exactly one value was parsed, as desired.
                if (symbolType >= ValueDelimiterThreshold) return;

                // Two or more consecutive values not allowed.
                Report(new JsonErrorInfo(
                    multipleValuesErrorCode,
                    CurrentLength - CurrentToken.Length,
                    CurrentToken.Length));
            }
        }

        private GreenJsonMultiValueSyntax CreateMultiValueNode(ArrayBuilder<GreenJsonValueWithBackgroundSyntax> valueNodesBuilder)
        {
            var background = CaptureBackground();
            if (valueNodesBuilder.Count == 0)
            {
                valueNodesBuilder.Add(new GreenJsonValueWithBackgroundSyntax(background, GreenJsonMissingValueSyntax.Value));
                return new GreenJsonMultiValueSyntax(valueNodesBuilder, GreenJsonBackgroundListSyntax.Empty);
            }
            return new GreenJsonMultiValueSyntax(valueNodesBuilder, background);
        }

        private GreenJsonMultiValueSyntax ParseMultiValue(JsonErrorCode multipleValuesErrorCode)
        {
            CurrentDepth++;
            if (CurrentDepth >= MaximumDepth) throw new MaximumDepthExceededException();

            var valueNodesBuilder = new ArrayBuilder<GreenJsonValueWithBackgroundSyntax>();
            ParseValues(valueNodesBuilder, multipleValuesErrorCode);

            CurrentDepth--;
            return CreateMultiValueNode(valueNodesBuilder);
        }

        // ParseMultiValue copy, except that it handles the SymbolType >= JsonSymbolType.ValueDelimiterThreshold case differently,
        // as it cannot go to a higher level in the stack to process value delimiter symbols.
        private RootJsonSyntax Parse()
        {
            Tokens = TokenizeAllHelper().GetEnumerator();

            var valueNodesBuilder = new ArrayBuilder<GreenJsonValueWithBackgroundSyntax>();

            for (; ; )
            {
                try
                {
                    ParseValues(valueNodesBuilder, JsonErrorCode.ExpectedEof);
                }
                catch (MaximumDepthExceededException)
                {
                    // Just ignore everything so far and return a default parse tree.
                    return CreateParseTreeTooDeepRootSyntax(Json, CurrentLength - 1);
                }

                if (CurrentToken.SymbolType == JsonSymbolType.Eof)
                {
                    return new RootJsonSyntax(
                        Json,
                        CreateMultiValueNode(valueNodesBuilder),
                        ReadOnlyList<JsonErrorInfo>.FromBuilder(Errors));
                }

                // ] } , : -- treat all of these at the top level as an undefined symbol without any semantic meaning.
                BackgroundBuilder.Add(new GreenJsonRootLevelValueDelimiterSyntax(CurrentToken));

                // Report an error if no value was encountered before the control symbol.
                Report(new JsonErrorInfo(
                    JsonErrorCode.ExpectedEof,
                    CurrentLength - CurrentToken.Length,
                    CurrentToken.Length));
            }
        }

        private IGreenJsonSymbol CreateValue(int currentIndex)
        {
            int length = currentIndex - SymbolStartIndex;
#if NET472
            IGreenJsonSymbol value
#else
            IGreenJsonSymbol? value
#endif
                = JsonValue.TryCreate(Json.AsSpan().Slice(SymbolStartIndex, length));

            if (value == null)
            {
                // Copy to a substring here, which is not necessary for JsonValue.TryCreate() anymore.
                Report(JsonParseErrors.UnrecognizedValue(Json.Substring(SymbolStartIndex, length), SymbolStartIndex, length));
                value = new GreenJsonUndefinedValueSyntax(length);
            }

            return value;
        }

        const int symbolClassValueChar = 0;
        const int symbolClassWhitespace = 1;
        const int symbolClassSymbol = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetSymbolClass(char c)
        {
            var category = char.GetUnicodeCategory(c);

#if NET5_0_OR_GREATER
#pragma warning disable IDE0066 // Convert switch statement to expression
#endif
            switch (category)
#if NET5_0_OR_GREATER
#pragma warning restore IDE0066 // Convert switch statement to expression
#endif
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.OtherNumber:
                case UnicodeCategory.Surrogate:
                case UnicodeCategory.ConnectorPunctuation:  // underscore-like characters
                case UnicodeCategory.DashPunctuation:
                    // Treat as part of a value.
                    return symbolClassValueChar;
                case UnicodeCategory.OpenPunctuation:
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.CurrencySymbol:
                case UnicodeCategory.ModifierSymbol:
                case UnicodeCategory.OtherSymbol:
                case UnicodeCategory.OtherNotAssigned:
                    return symbolClassSymbol;
                case UnicodeCategory.OtherPunctuation:
                    return c == '.' ? symbolClassValueChar : symbolClassSymbol;
                case UnicodeCategory.MathSymbol:
                    return c == '+' ? symbolClassValueChar : symbolClassSymbol;
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.Control:
                case UnicodeCategory.Format:
                case UnicodeCategory.PrivateUse:
                default:
                    // Whitespace is a separator.
                    return symbolClassWhitespace;
            }
        }

        private IEnumerable<IGreenJsonSymbol> TokenizeAllHelper()
        {
            // This tokenizer uses labels with goto to switch between modes of tokenization.

            int length = Json.Length;
            int currentIndex = SymbolStartIndex;
            StringBuilder valueBuilder = new StringBuilder();

        inWhitespace:

            while (currentIndex < length)
            {
                char c = Json[currentIndex];
                int symbolClass = GetSymbolClass(c);

                // Possibly yield a text element, or choose a different tokenization mode if the symbol class changed.
                if (symbolClass != symbolClassWhitespace)
                {
                    if (SymbolStartIndex < currentIndex)
                    {
                        yield return GreenJsonWhitespaceSyntax.Create(currentIndex - SymbolStartIndex);
                        SymbolStartIndex = currentIndex;
                    }

                    if (symbolClass == symbolClassSymbol)
                    {
                        switch (c)
                        {
                            case JsonSpecialCharacter.CurlyOpenCharacter:
                                yield return GreenJsonCurlyOpenSyntax.Value;
                                break;
                            case JsonSpecialCharacter.CurlyCloseCharacter:
                                yield return GreenJsonCurlyCloseSyntax.Value;
                                break;
                            case JsonSpecialCharacter.SquareBracketOpenCharacter:
                                yield return GreenJsonSquareBracketOpenSyntax.Value;
                                break;
                            case JsonSpecialCharacter.SquareBracketCloseCharacter:
                                yield return GreenJsonSquareBracketCloseSyntax.Value;
                                break;
                            case JsonSpecialCharacter.ColonCharacter:
                                yield return GreenJsonColonSyntax.Value;
                                break;
                            case JsonSpecialCharacter.CommaCharacter:
                                yield return GreenJsonCommaSyntax.Value;
                                break;
                            case CStyleStringLiteral.QuoteCharacter:
                                goto inString;
                            case JsonSpecialCharacter.CommentStartFirstCharacter:
                                // Look ahead 1 character to see if this is the start of a comment.
                                // In all other cases, treat as an unexpected symbol.
                                if (currentIndex + 1 < length)
                                {
                                    char secondChar = Json[currentIndex + 1];
                                    if (secondChar == JsonSpecialCharacter.SingleLineCommentStartSecondCharacter)
                                    {
                                        goto inSingleLineComment;
                                    }
                                    else if (secondChar == JsonSpecialCharacter.MultiLineCommentStartSecondCharacter)
                                    {
                                        goto inMultiLineComment;
                                    }
                                }
                                goto default;
                            default:
                                Report(JsonParseErrors.UnexpectedSymbol(c, currentIndex));
                                yield return GreenJsonUnknownSymbolSyntax.Value;
                                break;
                        }

                        // Increment to indicate the current character has been yielded.
                        SymbolStartIndex++;
                    }
                    else
                    {
                        goto inValue;
                    }
                }

                currentIndex++;
            }

            if (SymbolStartIndex < currentIndex)
            {
                yield return GreenJsonWhitespaceSyntax.Create(currentIndex - SymbolStartIndex);
            }

            yield break;

        inValue:

            // Eat the first symbol character, but leave SymbolStartIndex unchanged.
            currentIndex++;

            while (currentIndex < length)
            {
                char c = Json[currentIndex];
                int symbolClass = GetSymbolClass(c);

                // Possibly yield a text element, or choose a different tokenization mode if the symbol class changed.
                if (symbolClass != symbolClassValueChar)
                {
                    yield return CreateValue(currentIndex);
                    SymbolStartIndex = currentIndex;

                    if (symbolClass == symbolClassSymbol)
                    {
                        switch (c)
                        {
                            case JsonSpecialCharacter.CurlyOpenCharacter:
                                yield return GreenJsonCurlyOpenSyntax.Value;
                                break;
                            case JsonSpecialCharacter.CurlyCloseCharacter:
                                yield return GreenJsonCurlyCloseSyntax.Value;
                                break;
                            case JsonSpecialCharacter.SquareBracketOpenCharacter:
                                yield return GreenJsonSquareBracketOpenSyntax.Value;
                                break;
                            case JsonSpecialCharacter.SquareBracketCloseCharacter:
                                yield return GreenJsonSquareBracketCloseSyntax.Value;
                                break;
                            case JsonSpecialCharacter.ColonCharacter:
                                yield return GreenJsonColonSyntax.Value;
                                break;
                            case JsonSpecialCharacter.CommaCharacter:
                                yield return GreenJsonCommaSyntax.Value;
                                break;
                            case CStyleStringLiteral.QuoteCharacter:
                                goto inString;
                            case JsonSpecialCharacter.CommentStartFirstCharacter:
                                // Look ahead 1 character to see if this is the start of a comment.
                                // In all other cases, treat as an unexpected symbol.
                                if (currentIndex + 1 < length)
                                {
                                    char secondChar = Json[currentIndex + 1];
                                    if (secondChar == JsonSpecialCharacter.SingleLineCommentStartSecondCharacter)
                                    {
                                        goto inSingleLineComment;
                                    }
                                    else if (secondChar == JsonSpecialCharacter.MultiLineCommentStartSecondCharacter)
                                    {
                                        goto inMultiLineComment;
                                    }
                                }
                                goto default;
                            default:
                                Report(JsonParseErrors.UnexpectedSymbol(c, currentIndex));
                                yield return GreenJsonUnknownSymbolSyntax.Value;
                                break;
                        }

                        // Increment to indicate the current character has been yielded.
                        SymbolStartIndex++;
                    }

                    currentIndex++;
                    goto inWhitespace;
                }

                currentIndex++;
            }

            if (SymbolStartIndex < currentIndex)
            {
                yield return CreateValue(currentIndex);
            }

            yield break;

        inString:

            // Detect errors.
            bool hasStringErrors = false;

            // Eat " character, but leave SymbolStartIndex unchanged.
            currentIndex++;

            // Prepare for use.
            valueBuilder.Clear();

            while (currentIndex < length)
            {
                char c = Json[currentIndex];

                switch (c)
                {
                    case CStyleStringLiteral.QuoteCharacter:
                        // Closing quote character.
                        currentIndex++;
                        if (hasStringErrors)
                        {
                            yield return new GreenJsonErrorStringSyntax(currentIndex - SymbolStartIndex);
                        }
                        else
                        {
                            yield return new GreenJsonStringLiteralSyntax(valueBuilder.ToString(), currentIndex - SymbolStartIndex);
                        }
                        SymbolStartIndex = currentIndex;
                        goto inWhitespace;
                    case CStyleStringLiteral.EscapeCharacter:
                        // Escape sequence.
                        // Look ahead one character.
                        int escapeSequenceStart = currentIndex;
                        currentIndex++;

                        if (currentIndex < length)
                        {
                            char escapedChar = Json[currentIndex];

                            switch (escapedChar)
                            {
                                case CStyleStringLiteral.QuoteCharacter:
                                case CStyleStringLiteral.EscapeCharacter:
                                case '/':  // Weird one, but it's in the specification.
                                    valueBuilder.Append(escapedChar);
                                    break;
                                case 'b':
                                    valueBuilder.Append('\b');
                                    break;
                                case 'f':
                                    valueBuilder.Append('\f');
                                    break;
                                case 'n':
                                    valueBuilder.Append('\n');
                                    break;
                                case 'r':
                                    valueBuilder.Append('\r');
                                    break;
                                case 't':
                                    valueBuilder.Append('\t');
                                    break;
                                case 'v':
                                    valueBuilder.Append('\v');
                                    break;
                                case 'u':
                                    bool validUnicodeSequence = true;
                                    int unicodeValue = 0;

                                    // Expect exactly 4 hex characters.
                                    const int expectedHexLength = 4;
                                    for (int i = 0; i < expectedHexLength; i++)
                                    {
                                        currentIndex++;
                                        if (currentIndex < length)
                                        {
                                            // 1 hex character = 4 bits.
                                            unicodeValue <<= 4;
                                            char hexChar = Json[currentIndex];
                                            if ('0' <= hexChar && hexChar <= '9')
                                            {
                                                unicodeValue = unicodeValue + hexChar - '0';
                                            }
                                            else if ('a' <= hexChar && hexChar <= 'f')
                                            {
                                                const int aValue = 'a' - 10;
                                                unicodeValue = unicodeValue + hexChar - aValue;
                                            }
                                            else if ('A' <= hexChar && hexChar <= 'F')
                                            {
                                                const int aValue = 'A' - 10;
                                                unicodeValue = unicodeValue + hexChar - aValue;
                                            }
                                            else
                                            {
                                                currentIndex--;
                                                validUnicodeSequence = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            currentIndex--;
                                            validUnicodeSequence = false;
                                            break;
                                        }
                                    }

                                    if (validUnicodeSequence)
                                    {
                                        valueBuilder.Append(Convert.ToChar(unicodeValue));
                                    }
                                    else
                                    {
                                        hasStringErrors = true;
                                        int escapeSequenceLength = currentIndex - escapeSequenceStart + 1;
                                        Report(JsonParseErrors.UnrecognizedEscapeSequence(
                                            Json.Substring(escapeSequenceStart, escapeSequenceLength),
                                            escapeSequenceStart, escapeSequenceLength));
                                    }
                                    break;
                                default:
                                    hasStringErrors = true;
                                    Report(JsonParseErrors.UnrecognizedEscapeSequence(
                                        Json.Substring(escapeSequenceStart, 2),
                                        escapeSequenceStart, 2));
                                    break;
                            }
                        }
                        break;
                    default:
                        if (CStyleStringLiteral.CharacterMustBeEscaped(c))
                        {
                            // Generate user friendly representation of the illegal character in error message.
                            hasStringErrors = true;
                            Report(JsonParseErrors.IllegalControlCharacterInString(c, currentIndex));
                        }
                        else
                        {
                            valueBuilder.Append(c);
                        }
                        break;
                }

                currentIndex++;
            }

            // Use length rather than currentIndex; currentIndex is bigger after a '\'.
            int unterminatedStringLength = length - SymbolStartIndex;
            Report(JsonParseErrors.UnterminatedString(SymbolStartIndex, unterminatedStringLength));
            yield return new GreenJsonErrorStringSyntax(unterminatedStringLength);
            yield break;

        inSingleLineComment:

            // Eat both / characters, but leave SymbolStartIndex unchanged.
            currentIndex += 2;

            while (currentIndex < length)
            {
                char c = Json[currentIndex];

                switch (c)
                {
                    case '\r':
                        // Can already eat this whitespace character.
                        currentIndex++;

                        // Look ahead to see if the next character is a linefeed.
                        // Otherwise, the '\r' just becomes part of the comment.
                        if (currentIndex < length)
                        {
                            char secondChar = Json[currentIndex];
                            if (secondChar == '\n')
                            {
                                yield return GreenJsonCommentSyntax.Create(currentIndex - 1 - SymbolStartIndex);

                                // Eat the second whitespace character.
                                SymbolStartIndex = currentIndex - 1;
                                currentIndex++;
                                goto inWhitespace;
                            }
                        }
                        break;
                    case '\n':
                        yield return GreenJsonCommentSyntax.Create(currentIndex - SymbolStartIndex);

                        // Eat the '\n'.
                        SymbolStartIndex = currentIndex;
                        currentIndex++;
                        goto inWhitespace;
                }

                currentIndex++;
            }

            yield return GreenJsonCommentSyntax.Create(currentIndex - SymbolStartIndex);
            yield break;

        inMultiLineComment:

            // Eat /* characters, but leave SymbolStartIndex unchanged.
            currentIndex += 2;

            while (currentIndex < length)
            {
                if (Json[currentIndex] == '*')
                {
                    // Look ahead to see if the next character is a slash.
                    if (currentIndex + 1 < length)
                    {
                        char secondChar = Json[currentIndex + 1];

                        if (secondChar == '/')
                        {
                            // Increment so the closing '*/' is regarded as part of the comment.
                            currentIndex += 2;

                            yield return GreenJsonCommentSyntax.Create(currentIndex - SymbolStartIndex);

                            SymbolStartIndex = currentIndex;
                            goto inWhitespace;
                        }
                    }
                }

                currentIndex++;
            }

            int unterminatedCommentLength = length - SymbolStartIndex;
            Report(JsonParseErrors.UnterminatedMultiLineComment(SymbolStartIndex, unterminatedCommentLength));
            yield return new GreenJsonUnterminatedMultiLineCommentSyntax(unterminatedCommentLength);
        }
    }

#if NET5_0_OR_GREATER
#pragma warning restore IDE0090 // Use 'new(...)'
#endif
}
