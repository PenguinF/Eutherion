﻿#region License
/*********************************************************************************
 * JsonTokenizerTests.cs
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

using Eutherion.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Eutherion.Text.Json.Tests
{
    public class JsonTokenizerTests
    {
        /// <summary>
        /// Indicates if two symbols of the same type should combine into one.
        /// </summary>
        private static bool WillCombine(Type tokenType1, Type tokenType2, [AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out Type resultTokenType)
        {
            if (tokenType1 == typeof(GreenJsonWhitespaceSyntax))
            {
                if (tokenType1 == tokenType2)
                {
                    resultTokenType = tokenType1;
                    return true;
                }
            }
            else if (tokenType1 == typeof(GreenJsonBooleanLiteralSyntax.False)
                || tokenType1 == typeof(GreenJsonBooleanLiteralSyntax.True)
                || tokenType1 == typeof(GreenJsonUndefinedValueSyntax))
            {
                // This only works if two undefined values don't add up to 'true' or 'false'.
                if (tokenType2 == typeof(GreenJsonBooleanLiteralSyntax.False)
                    || tokenType2 == typeof(GreenJsonBooleanLiteralSyntax.True)
                    || tokenType2 == typeof(GreenJsonIntegerLiteralSyntax)
                    || tokenType2 == typeof(GreenJsonUndefinedValueSyntax))
                {
                    resultTokenType = typeof(GreenJsonUndefinedValueSyntax);
                    return true;
                }
            }
            else if (tokenType1 == typeof(GreenJsonIntegerLiteralSyntax))
            {
                if (tokenType1 == tokenType2)
                {
                    // This obviously only works if the literal is numbers only.
                    // See JsonTestSymbols below.
                    resultTokenType = tokenType1;
                    return true;
                }
                else if (tokenType2 == typeof(GreenJsonBooleanLiteralSyntax.False)
                    || tokenType2 == typeof(GreenJsonBooleanLiteralSyntax.True)
                    || tokenType2 == typeof(GreenJsonUndefinedValueSyntax))
                {
                    resultTokenType = typeof(GreenJsonUndefinedValueSyntax);
                    return true;
                }
            }

            resultTokenType = default;
            return false;
        }

        private static void AssertTokens(string json, params Action<IGreenJsonSymbol>[] tokenInspectors)
        {
            Assert.Collection(JsonParser.TokenizeAll(json).Item1, tokenInspectors);
        }

        private static Action<IGreenJsonSymbol> ExpectToken(Type expectedTokenType, int expectedLength)
        {
            return symbol =>
            {
                Assert.NotNull(symbol);
                Assert.IsType(expectedTokenType, symbol);
                Assert.Equal(expectedLength, symbol.Length);
            };
        }

        private static Action<IGreenJsonSymbol> ExpectToken<TExpected>(int expectedLength)
        {
            return symbol =>
            {
                Assert.NotNull(symbol);
                Assert.IsType<TExpected>(symbol);
                Assert.Equal(expectedLength, symbol.Length);
            };
        }

        [Fact]
        public void NullSourceThrows()
        {
            Assert.Throws<ArgumentNullException>(() => JsonParser.TokenizeAll(null!));
            Assert.Throws<ArgumentNullException>(() => JsonParser.Parse(null!));
        }

        [Theory]
        [InlineData("//", null)]
        [InlineData("//\n", "//")]
        [InlineData("//\r\n", "//")]
        [InlineData("//\t\tComment \r\n", "//\t\tComment ")]
        [InlineData("/**/", null)]
        [InlineData("/*  */", null)]
        [InlineData("/*\r\n*/", null)]
        [InlineData("/*/**/", null)]
        [InlineData("/*//\r\n*/\r\n", "/*//\r\n*/")]
        [InlineData("///**/\r\n", "///**/")]
        public void Comment(string json, string alternativeCommentText)
        {
            string expectedCommentText = alternativeCommentText ?? json;

            void firstTokenAssert(IGreenJsonSymbol symbol)
            {
                Assert.NotNull(symbol);
                Assert.IsType<GreenJsonCommentSyntax>(symbol);
                Assert.Equal(expectedCommentText.Length, symbol.Length);
                Assert.Equal(expectedCommentText, json.Substring(0, symbol.Length));
            }

            if (alternativeCommentText == null)
            {
                AssertTokens(json, firstTokenAssert);
            }
            else
            {
                // Expect some whitespace at the end.
                AssertTokens(
                    json,
                    firstTokenAssert,
                    symbol => Assert.IsType<GreenJsonWhitespaceSyntax>(symbol));
            }
        }

        [Fact]
        public void EmptySourceNoTokens()
        {
            AssertTokens("");
        }

        [Theory]
        [InlineData("\0")]
        [InlineData("                              ")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t\v\u000c\u0085\u00a0")]
        [InlineData("\r\n")]
        [InlineData("\r\n\r\n")]
        [InlineData("\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u2028\u2029\u202f\u205f\u3000")]
        public void WhiteSpace(string ws)
        {
            // Exactly one whitespace token.
            AssertTokens(ws, ExpectToken<GreenJsonWhitespaceSyntax>(ws.Length));
        }

        [Theory]
        [InlineData(typeof(GreenJsonCurlyOpenSyntax), '{')]
        [InlineData(typeof(GreenJsonCurlyCloseSyntax), '}')]
        [InlineData(typeof(GreenJsonSquareBracketOpenSyntax), '[')]
        [InlineData(typeof(GreenJsonSquareBracketCloseSyntax), ']')]
        [InlineData(typeof(GreenJsonColonSyntax), ':')]
        [InlineData(typeof(GreenJsonCommaSyntax), ',')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '*')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '€')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '≥')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '¿')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '°')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '╣')]
        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '∙')]

        [InlineData(typeof(GreenJsonUnknownSymbolSyntax), '▓')]
        public void SpecialCharacter(Type tokenType, char specialCharacter)
        {
            string json = Convert.ToString(specialCharacter);
            AssertTokens(json, ExpectToken(tokenType, json.Length));
        }

        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("10.8")]
        [InlineData("-9.00001")]
        [InlineData("+00001")]
        [InlineData("-00001")]
        [InlineData("-1e+10")]
        [InlineData("1.9E-5")]
        [InlineData("0b01011001")]
        [InlineData("0xffff")]
        [InlineData("_")]
        [InlineData("x80")]
        [InlineData("189")]
        [InlineData("x²")]
        [InlineData("x₁")]
        [InlineData("Grüßen")]
        // Shamelessly plugged from that online translation tool. These are all 'thing's.
        [InlineData("شيء")]
        [InlineData("вещь")]
        [InlineData("事情")]
        [InlineData("もの")]
        [InlineData("πράγμα")]
        [InlineData("맡은일")]
        [InlineData("चीज़")]
        [InlineData("Điều")]
        [InlineData("דָבָר")]
        [InlineData("สิ่ง")]
        [InlineData("విషయం")]
        [InlineData("விஷயம்")]
        [InlineData("දෙයක්")]
        [InlineData("ڳالھ")]
        public void ValueSymbol(string json)
        {
            AssertTokens(json, symbol =>
            {
                Assert.NotNull(symbol);
                Assert.Equal(json.Length, symbol.Length);

                if (symbol is GreenJsonBooleanLiteralSyntax booleanLiteral)
                {
                    Assert.Equal(json, booleanLiteral.LiteralJsonValue);
                }
                else if (symbol is GreenJsonIntegerLiteralSyntax integerLiteral)
                {
                    Assert.Equal(int.Parse(json), integerLiteral.Value);
                }
                else
                {
                    Assert.IsType<GreenJsonUndefinedValueSyntax>(symbol);
                }
            });
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\" \"", " ")]
        [InlineData("\"xxx\"", "xxx")]

        // Escape sequences.
        [InlineData("\"\\\"\"", "\"")]
        [InlineData("\"\\/\"", "/")]
        [InlineData("\"\\b\"", "\b")]
        [InlineData("\"\\f\"", "\f")]
        [InlineData("\"\\n\"", "\n")]
        [InlineData("\"\\r\"", "\r")]
        [InlineData("\"\\t\"", "\t")]
        [InlineData("\"\\v\"", "\v")]  // Support \v, contrary to JSON specification.

        // \u escape sequences.
        [InlineData("\"\\u000d\\u000a\"", "\r\n")]
        [InlineData("\"\\u0020\\u004E\"", " N")]
        [InlineData("\"\\u00200\"", " 0")] // last 0 is not part of the \u escape sequence
        public void StringValue(string json, string expectedValue)
        {
            AssertTokens(json, symbol =>
            {
                Assert.NotNull(symbol);
                var stringSymbol = Assert.IsType<GreenJsonStringLiteralSyntax>(symbol);
                Assert.Equal(json.Length, symbol.Length);
                Assert.Equal(expectedValue, stringSymbol.Value);
            });
        }

        internal static IEnumerable<(string, Type)> JsonTestSymbols()
        {
            yield return (" ", typeof(GreenJsonWhitespaceSyntax));
            yield return ("/**/", typeof(GreenJsonCommentSyntax));
            yield return ("/***/", typeof(GreenJsonCommentSyntax));
            yield return ("/*/*/", typeof(GreenJsonCommentSyntax));
            yield return ("{", typeof(GreenJsonCurlyOpenSyntax));
            yield return ("}", typeof(GreenJsonCurlyCloseSyntax));
            yield return ("[", typeof(GreenJsonSquareBracketOpenSyntax));
            yield return ("]", typeof(GreenJsonSquareBracketCloseSyntax));
            yield return (":", typeof(GreenJsonColonSyntax));
            yield return (",", typeof(GreenJsonCommaSyntax));
            yield return ("*", typeof(GreenJsonUnknownSymbolSyntax));
            yield return ("false", typeof(GreenJsonBooleanLiteralSyntax.False));
            yield return ("true", typeof(GreenJsonBooleanLiteralSyntax.True));
            yield return ("0", typeof(GreenJsonIntegerLiteralSyntax));
            yield return ("_", typeof(GreenJsonUndefinedValueSyntax));
            yield return ("\"\"", typeof(GreenJsonStringLiteralSyntax));
            yield return ("\" \"", typeof(GreenJsonStringLiteralSyntax));  // Have to check if the space isn't interpreted as whitespace.
            yield return ("\"\n\\ \n\"", typeof(GreenJsonErrorStringSyntax));
            yield return ("\"\\u0\"", typeof(GreenJsonErrorStringSyntax));
        }

        internal static IEnumerable<(string, Type)> UnterminatedJsonTestSymbols()
        {
            yield return ("//", typeof(GreenJsonCommentSyntax));
            yield return ("/*", typeof(GreenJsonUnterminatedMultiLineCommentSyntax));
            yield return ("\"", typeof(GreenJsonErrorStringSyntax));
        }

        internal static IEnumerable<(string json, Type type)> OneSymbolOfEachType()
            => JsonTestSymbols().Concat(UnterminatedJsonTestSymbols());

        internal static IEnumerable<(string json1, Type type1, string json2, Type type2)> TwoSymbolsOfEachType()
        {
            // Unterminated strings/comments mess up the tokenization, skip those if they're the first key.
            foreach (var (key1, type1) in JsonTestSymbols())
            {
                foreach (var (key2, type2) in OneSymbolOfEachType())
                {
                    yield return (key1, type1, key2, type2);
                }
            }
        }

        public static IEnumerable<object?[]> WrappedTwoSymbolsOfEachType() => TestUtilities.Wrap(TwoSymbolsOfEachType());

        /// <summary>
        /// Tests all combinations of three symbols.
        /// </summary>
        [Theory]
        [MemberData(nameof(WrappedTwoSymbolsOfEachType))]
        public void Transition(string json1, Type type1, string json2, Type type2)
        {
            // Instead of having a gazillion separate tests over 3 tokens,
            // first test the combination of 2 tokens, and then if that succeeds
            // test every other token that could precede it in a loop.
            {
                if (WillCombine(type1, type2, out var type12))
                {
                    AssertTokens(
                        json1 + json2,
                        ExpectToken(type12, json1.Length + json2.Length));
                }
                else
                {
                    AssertTokens(
                        json1 + json2,
                        ExpectToken(type1, json1.Length),
                        ExpectToken(type2, json2.Length));
                }
            }

            // Here Assert.Collection is used so if such a test fails,
            // it gives the index of the third token that was tested.
            Assert.Collection(
                JsonTestSymbols(),
                Enumerable.Repeat<Action<(string, Type)>>(x0 =>
                {
                    // Put the third symbol in front, because the last symbol may eat it.
                    string json0 = x0.Item1;
                    Type type0 = x0.Item2;

                    // Exceptions for when 2 or 3 symbols go together and make 1.
                    if (WillCombine(type0, type1, out var type01))
                    {
                        if (WillCombine(type01, type2, out var type012))
                        {
                            AssertTokens(
                                json0 + json1 + json2,
                                ExpectToken(type012, json0.Length + json1.Length + json2.Length));
                        }
                        else
                        {
                            AssertTokens(
                                json0 + json1 + json2,
                                ExpectToken(type01, json0.Length + json1.Length),
                                ExpectToken(type2, json2.Length));
                        }
                    }
                    else if (WillCombine(type1, type2, out var type12))
                    {
                        AssertTokens(
                            json0 + json1 + json2,
                            ExpectToken(type0, json0.Length),
                            ExpectToken(type12, json1.Length + json2.Length));
                    }
                    else
                    {
                        AssertTokens(
                            json0 + json1 + json2,
                            ExpectToken(type0, json0.Length),
                            ExpectToken(type1, json1.Length),
                            ExpectToken(type2, json2.Length));
                    }
                }, JsonTestSymbols().Count()).ToArray());
        }

        public static IEnumerable<object?[]> WrappedOneSymbolOfEachType() => TestUtilities.Wrap(OneSymbolOfEachType());

        /// <summary>
        /// Tests all combinations of a comment followed by another symbol.
        /// </summary>
        [Theory]
        [MemberData(nameof(WrappedOneSymbolOfEachType))]
        public void SingleLineCommentTransition(string json, Type type)
        {
            // Double slash + symbol should be lexed as a comment.
            // Do not take a dependency upon SpecialCharacter, because tests should fail if the SpecialCharacter definition is changed.
            var commentThenSymbolText = $"//{json}";

            // Special treatment if 'symbolText' contains a line break.
            int linefeedIndex = json.IndexOf('\n');
            if (linefeedIndex >= 0)
            {
                // Only assert the single line comment, and that the next symbol is whitespace ('\n').
                var symbols = JsonParser.TokenizeAll(commentThenSymbolText).Item1;
                Assert.True(symbols.Count >= 2);
                ExpectToken<GreenJsonCommentSyntax>(2 + linefeedIndex)(symbols[0]);
                Assert.IsType<GreenJsonWhitespaceSyntax>(symbols[1]);
            }
            else
            {
                // Comment should eat everything otherwise.
                AssertTokens(
                    commentThenSymbolText,
                    ExpectToken<GreenJsonCommentSyntax>(2 + json.Length));
            }

            // Comment + newline + symbol.
            var commentThenNewLineThenSymbolText = $"//\n{json}";

            if (type == typeof(GreenJsonWhitespaceSyntax))
            {
                // Test this separately because the '\n' is included in the second symbol.
                AssertTokens(
                    commentThenNewLineThenSymbolText,
                    ExpectToken<GreenJsonCommentSyntax>(2),
                    ExpectToken<GreenJsonWhitespaceSyntax>(1 + json.Length));
            }
            else
            {
                // Three symbols: comment, single newline whitespace symbol, test symbol from parameter of the expected type.
                AssertTokens(
                    commentThenNewLineThenSymbolText,
                    ExpectToken<GreenJsonCommentSyntax>(2),
                    ExpectToken<GreenJsonWhitespaceSyntax>(1),
                    ExpectToken(type, json.Length));
            }

            // Comment + windows newline + symbol.
            commentThenNewLineThenSymbolText = $"//\r\n{json}";

            if (type == typeof(GreenJsonWhitespaceSyntax))
            {
                // Second symbol should include the '\r' as well as the '\n'.
                AssertTokens(
                    commentThenNewLineThenSymbolText,
                    ExpectToken<GreenJsonCommentSyntax>(2),
                    ExpectToken<GreenJsonWhitespaceSyntax>(2 + json.Length));
            }
            else
            {
                // Three symbols: comment, whitespace symbol, test symbol from parameter of the expected type.
                AssertTokens(
                    commentThenNewLineThenSymbolText,
                    ExpectToken<GreenJsonCommentSyntax>(2),
                    ExpectToken<GreenJsonWhitespaceSyntax>(2),
                    ExpectToken(type, json.Length));
            }
        }

        private static IEnumerable<(string json, JsonErrorInfo[] expectedErrors)> GetErrorStrings()
        {
            yield return ("*", new[] { JsonParseErrors.UnexpectedSymbol('*', 0) });
            yield return (" *", new[] { JsonParseErrors.UnexpectedSymbol('*', 1) });
            yield return ("  °  ", new[] { JsonParseErrors.UnexpectedSymbol('°', 2) });

            // Unterminated comments.
            yield return ("/*", new[] { JsonParseErrors.UnterminatedMultiLineComment(0, 2) });
            yield return ("/*\n\n", new[] { JsonParseErrors.UnterminatedMultiLineComment(0, 4) });
            yield return ("  /*\n\n*", new[] { JsonParseErrors.UnterminatedMultiLineComment(2, 5) });
            yield return ("  /*\n\n* /", new[] { JsonParseErrors.UnterminatedMultiLineComment(2, 7) });

            // Invalid strings.
            yield return ("\"", new[] { JsonParseErrors.UnterminatedString(0, 1) });
            yield return ("\"\\", new[] { JsonParseErrors.UnterminatedString(0, 2) });

            // Unterminated because the closing " is escaped.
            yield return ("\"\\\"", new[] { JsonParseErrors.UnterminatedString(0, 3) });
            yield return ("\"\\ \"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\ ", 1, 2) });
            yield return ("\"\\e\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\e", 1, 2) });

            // Unicode escape sequences.
            yield return ("\"\\u\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u", 1, 2) });
            yield return ("\"\\ux\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u", 1, 2) });
            yield return ("\"\\uxxxx\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u", 1, 2) });
            yield return ("\"\\u0\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u0", 1, 3) });
            yield return ("\"\\u00\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u00", 1, 4) });
            yield return ("\"\\u000\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u000", 1, 5) });
            yield return ("\"\\u000g\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u000", 1, 5) });

            // Prevent int.TryParse hacks.
            yield return ("\"\\u-1000\"", new[] { JsonParseErrors.UnrecognizedEscapeSequence("\\u", 1, 2) });

            // Disallow control characters.
            yield return ("\"\n\"", new[] { JsonParseErrors.IllegalControlCharacterInString('\n', 1) });
            yield return ("\"\t\"", new[] { JsonParseErrors.IllegalControlCharacterInString('\t', 1) });
            yield return ("\"\0\"", new[] { JsonParseErrors.IllegalControlCharacterInString('\u0000', 1) });
            yield return ("\"\u0001\"", new[] { JsonParseErrors.IllegalControlCharacterInString('\u0001', 1) });
            yield return ("\"\u007f\"", new[] { JsonParseErrors.IllegalControlCharacterInString('\u007f', 1) });

            // Multiple errors.
            yield return (" ∙\"∙\"\"", new JsonErrorInfo[] {
                JsonParseErrors.UnexpectedSymbol('∙', 1),
                JsonParseErrors.UnterminatedString(5, 1) });
            yield return ("\"\r\n\"", new[] {
                JsonParseErrors.IllegalControlCharacterInString('\r', 1),
                JsonParseErrors.IllegalControlCharacterInString('\n', 2) });
            yield return ("\"\\ ", new[] {
                JsonParseErrors.UnrecognizedEscapeSequence("\\ ", 1, 2),
                JsonParseErrors.UnterminatedString(0, 3) });
            yield return ("\"\r\n∙\"∙", new[] {
                JsonParseErrors.IllegalControlCharacterInString('\r', 1),
                JsonParseErrors.IllegalControlCharacterInString('\n', 2),
                JsonParseErrors.UnexpectedSymbol('∙', 5) });
            yield return ("\"\t\n", new[] {
                JsonParseErrors.IllegalControlCharacterInString('\t', 1),
                JsonParseErrors.IllegalControlCharacterInString('\n', 2),
                JsonParseErrors.UnterminatedString(0, 3) });
            yield return ("\" \\ \n\"", new[] {
                JsonParseErrors.UnrecognizedEscapeSequence("\\ ", 2, 2),
                JsonParseErrors.IllegalControlCharacterInString('\n', 4) });
            yield return ("\"\n\\ \n\"", new[] {
                JsonParseErrors.IllegalControlCharacterInString('\n', 1),
                JsonParseErrors.UnrecognizedEscapeSequence("\\ ", 2, 2),
                JsonParseErrors.IllegalControlCharacterInString('\n', 4) });
            yield return ("\"\\u", new[] {
                JsonParseErrors.UnrecognizedEscapeSequence("\\u", 1, 2),
                JsonParseErrors.UnterminatedString(0, 3) });
            yield return ("\"\\uA", new[] {
                JsonParseErrors.UnrecognizedEscapeSequence("\\uA", 1, 3),
                JsonParseErrors.UnterminatedString(0, 4) });
            yield return ("\"\\u\n", new[] {
                JsonParseErrors.UnrecognizedEscapeSequence("\\u", 1, 2),
                JsonParseErrors.IllegalControlCharacterInString('\n', 3),
                JsonParseErrors.UnterminatedString(0, 4) });
            yield return ("\"\\ufff\n", new[] {
                JsonParseErrors.UnrecognizedEscapeSequence("\\ufff", 1, 5),
                JsonParseErrors.IllegalControlCharacterInString('\n', 6),
                JsonParseErrors.UnterminatedString(0, 7) });
            yield return ("\"\n\\ ∙\"∙", new[] {
                JsonParseErrors.IllegalControlCharacterInString('\n', 1),
                JsonParseErrors.UnrecognizedEscapeSequence("\\ ", 2, 2),
                JsonParseErrors.UnexpectedSymbol('∙', 6) });
            yield return ("∙\"\n\\ ∙\"", new[] {
                JsonParseErrors.UnexpectedSymbol('∙', 0),
                JsonParseErrors.IllegalControlCharacterInString('\n', 2),
                JsonParseErrors.UnrecognizedEscapeSequence("\\ ", 3, 2) });

            // Know what's unterminated.
            yield return ("\"/*", new[] { JsonParseErrors.UnterminatedString(0, 3) });
            yield return ("/*\"", new[] { JsonParseErrors.UnterminatedMultiLineComment(0, 3) });
            yield return ("///*\n\"", new[] { JsonParseErrors.UnterminatedString(5, 1) });
            yield return ("///*\"\n/*", new[] { JsonParseErrors.UnterminatedMultiLineComment(6, 2) });
        }

        public static IEnumerable<object?[]> WrappedErrorStrings() => TestUtilities.Wrap(GetErrorStrings());

        [Theory]
        [MemberData(nameof(WrappedErrorStrings))]
        public void Errors(string json, JsonErrorInfo[] expectedErrors)
        {
            var tokensAndErrors = JsonParser.TokenizeAll(json);
            var generatedErrors = tokensAndErrors.Item2;

            Assert.Collection(generatedErrors, expectedErrors.Select(expectedError => new Action<JsonErrorInfo>(generatedError =>
            {
                Assert.NotNull(generatedError);
                Assert.Equal(expectedError.ErrorCode, generatedError.ErrorCode);
                Assert.Equal(expectedError.Start, generatedError.Start);
                Assert.Equal(expectedError.Length, generatedError.Length);
                JsonErrorTests.AssertErrorInfoParameters(generatedError, expectedError.Parameters.ToArrayEx());
            })).ToArrayEx());
        }
    }
}
