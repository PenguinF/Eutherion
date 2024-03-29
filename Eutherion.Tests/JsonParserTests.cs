﻿#region License
/*********************************************************************************
 * JsonParserTests.cs
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
using System.Linq;
using Xunit;

namespace Eutherion.Text.Json.Tests
{
    public class JsonParserTests
    {
        private sealed class TerminalSymbolTester : JsonSymbolVisitor<IGreenJsonSymbol>
        {
            public static readonly TerminalSymbolTester Instance = new();

            private TerminalSymbolTester() { }

            public override IGreenJsonSymbol VisitBooleanLiteralSyntax(JsonBooleanLiteralSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitColonSyntax(JsonColonSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitCommaSyntax(JsonCommaSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitCommentSyntax(JsonCommentSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitCurlyCloseSyntax(JsonCurlyCloseSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitCurlyOpenSyntax(JsonCurlyOpenSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitErrorStringSyntax(JsonErrorStringSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitIntegerLiteralSyntax(JsonIntegerLiteralSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitRootLevelValueDelimiterSyntax(JsonRootLevelValueDelimiterSyntax node, _void arg) => node.Green.ValueDelimiter;
            public override IGreenJsonSymbol VisitSquareBracketCloseSyntax(JsonSquareBracketCloseSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitSquareBracketOpenSyntax(JsonSquareBracketOpenSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitStringLiteralSyntax(JsonStringLiteralSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitUndefinedValueSyntax(JsonUndefinedValueSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitUnknownSymbolSyntax(JsonUnknownSymbolSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitUnterminatedMultiLineCommentSyntax(JsonUnterminatedMultiLineCommentSyntax node, _void arg) => node.Green;
            public override IGreenJsonSymbol VisitWhitespaceSyntax(JsonWhitespaceSyntax node, _void arg) => node.Green;
        }

        private enum JsonSymbolTypeClass { Background, ValueStarter, ValueDelimiter }

        private static void AssertJsonSymbolType(JsonSymbolType jsonSymbolType, JsonSymbolTypeClass expectedJsonSymbolTypeClass)
        {
            switch (expectedJsonSymbolTypeClass)
            {
                case JsonSymbolTypeClass.Background:
                    Assert.True(jsonSymbolType < JsonParser.ForegroundThreshold);
                    Assert.True(jsonSymbolType < JsonParser.ValueDelimiterThreshold);
                    break;
                case JsonSymbolTypeClass.ValueStarter:
                    Assert.False(jsonSymbolType < JsonParser.ForegroundThreshold);
                    Assert.True(jsonSymbolType < JsonParser.ValueDelimiterThreshold);
                    break;
                case JsonSymbolTypeClass.ValueDelimiter:
                default:
                    Assert.False(jsonSymbolType < JsonParser.ForegroundThreshold);
                    Assert.False(jsonSymbolType < JsonParser.ValueDelimiterThreshold);
                    break;
            }
        }

        /// <summary>
        /// Checks if assumptions by <see cref="JsonParser"/> still apply.
        /// </summary>
        [Fact]
        public void JsonSymbolTypeAssumptions()
        {
            new[]
            {
                JsonSymbolType.Whitespace,
                JsonSymbolType.Comment,
                JsonSymbolType.UnterminatedMultiLineComment,
            }
            .ForEach(x => AssertJsonSymbolType(x, JsonSymbolTypeClass.Background));

            new[]
            {
                JsonSymbolType.BooleanLiteral,
                JsonSymbolType.IntegerLiteral,
                JsonSymbolType.StringLiteral,
                JsonSymbolType.ErrorString,
                JsonSymbolType.UndefinedValue,
                JsonSymbolType.UnknownSymbol,
                JsonSymbolType.CurlyOpen,
                JsonSymbolType.BracketOpen,
            }
            .ForEach(x => AssertJsonSymbolType(x, JsonSymbolTypeClass.ValueStarter));

            new[]
            {
                JsonSymbolType.Colon,
                JsonSymbolType.Comma,
                JsonSymbolType.CurlyClose,
                JsonSymbolType.BracketClose,
                JsonSymbolType.Eof,
            }
            .ForEach(x => AssertJsonSymbolType(x, JsonSymbolTypeClass.ValueDelimiter));
        }

        private static IEnumerable<(string json1, string json2)> TwoSymbolsWithoutType()
            => JsonTokenizerTests.TwoSymbolsOfEachType().Select(x => (x.json1, x.json2));

        public static IEnumerable<object?[]> WrappedTwoSymbolsWithoutType() => TestUtilities.Wrap(TwoSymbolsWithoutType());

        /// <summary>
        /// Tests if terminal symbols returned by a parsed <see cref="JsonSyntax"/> match those returned by its tokenizer.
        /// </summary>
        [Theory]
        [MemberData(nameof(WrappedTwoSymbolsWithoutType))]
        public void ParseTreeTokensMatch(string json1, string json2)
        {
            // Sane structure as JsonTokenizerTests.Transition: first check two symbols, then all combinations of three.
            {
                string json = json1 + json2;
                var expectedTokens = JsonParser.TokenizeAll(json).Item1;
                Action<IJsonSymbol>[] tokenInspectors = expectedTokens.Select<IGreenJsonSymbol, Action<IJsonSymbol>>((IGreenJsonSymbol expectedGreen) => (IJsonSymbol red) =>
                {
                    IGreenJsonSymbol actualGreen = TerminalSymbolTester.Instance.Visit(red);
                    Assert.IsType(expectedGreen.GetType(), actualGreen);
                    Assert.Equal(expectedGreen.Length, actualGreen.Length);
                    Assert.Equal(expectedGreen.Length, red.Length);
                }).ToArray();

                Assert.Collection(
                    JsonParser.Parse(json).Syntax.TerminalSymbolsInRange(0, json.Length),
                    tokenInspectors);
            }

            // Here Assert.Collection is used so if such a test fails,
            // it gives the index of the third token that was tested.
            Assert.Collection(
                JsonTokenizerTests.JsonTestSymbols(),
                Enumerable.Repeat<Action<(string, Type)>>(x0 =>
                {
                    string json = x0.Item1 + json1 + json2;
                    var expectedTokens = JsonParser.TokenizeAll(json).Item1;
                    Action<IJsonSymbol>[] tokenInspectors = expectedTokens.Select<IGreenJsonSymbol, Action<IJsonSymbol>>(expectedGreen => symbol =>
                    {
                        IGreenJsonSymbol actualGreen = TerminalSymbolTester.Instance.Visit(symbol);
                        Assert.IsType(expectedGreen.GetType(), actualGreen);
                        Assert.Equal(expectedGreen.Length, actualGreen.Length);
                        Assert.Equal(expectedGreen.Length, symbol.Length);
                    }).ToArray();

                    Assert.Collection(
                        JsonParser.Parse(json).Syntax.TerminalSymbolsInRange(0, json.Length),
                        tokenInspectors);

                }, JsonTokenizerTests.JsonTestSymbols().Count()).ToArray());
        }

        private static int AssertParseTree(ExpectedJsonTree expectedParseTree, JsonSyntax? expectedParent, int expectedStart, JsonSyntax actualParseTree)
        {
            Assert.IsType(expectedParseTree.ExpectedType, actualParseTree);
            Assert.Same(expectedParent, actualParseTree.ParentSyntax);
            Assert.Equal(expectedStart, actualParseTree.Start);

            int expectedChildCount = expectedParseTree.ChildNodes.Count;
            Assert.Equal(expectedChildCount, actualParseTree.ChildCount);

            Assert.Throws<ArgumentOutOfRangeException>(() => actualParseTree.GetChild(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => actualParseTree.GetChild(expectedChildCount));
            Assert.Throws<ArgumentOutOfRangeException>(() => actualParseTree.GetChildStartPosition(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => actualParseTree.GetChildStartPosition(expectedChildCount));
            Assert.Throws<ArgumentOutOfRangeException>(() => actualParseTree.GetChildStartOrEndPosition(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => actualParseTree.GetChildStartOrEndPosition(expectedChildCount + 1));

            int length = 0;

            if (expectedChildCount == 0)
            {
                if (actualParseTree.Length > 0)
                {
                    Assert.True(actualParseTree.IsTerminalSymbol(out var jsonSymbol));
                    length = jsonSymbol!.Length;
                }
                else
                {
                    Assert.False(actualParseTree.IsTerminalSymbol(out _));
                }
            }
            else
            {
                Assert.False(actualParseTree.IsTerminalSymbol(out _));

                for (int i = 0; i < expectedChildCount; i++)
                {
                    Assert.Equal(length, actualParseTree.GetChildStartOrEndPosition(i));
                    length += AssertParseTree(expectedParseTree.ChildNodes[i], actualParseTree, length, actualParseTree.GetChild(i));
                }
            }

            Assert.Equal(length, actualParseTree.GetChildStartOrEndPosition(expectedChildCount));

            return length;
        }

        private static IEnumerable<(string json, ExpectedJsonTree<RootJsonSyntax> parseTree, JsonErrorCode[] expectedErrors)> GetTestParseTrees()
            => ExpectedJsonTrees.TestParseTrees.Select(x => (x.Item1, new ExpectedJsonTree<RootJsonSyntax> { x.Item2 }, Array.Empty<JsonErrorCode>()))
            .Concat(ExpectedJsonTrees.TestParseTreesWithErrors.Select(x => (x.Item1, new ExpectedJsonTree<RootJsonSyntax> { x.Item2 }, x.Item3)));

        public static IEnumerable<object?[]> WrappedTestParseTrees() => TestUtilities.Wrap(GetTestParseTrees());

        [Theory]
        [MemberData(nameof(WrappedTestParseTrees))]
        public void ParseTreeTests(string json, ExpectedJsonTree<RootJsonSyntax> parseTree, JsonErrorCode[] expectedErrors)
        {
            RootJsonSyntax rootSyntax = JsonParser.Parse(json);
            AssertParseTree(parseTree, null, 0, rootSyntax);

            // Assert expected errors.
            Assert.Collection(
                rootSyntax.Errors,
                Enumerable.Range(0, expectedErrors.Length)
                          .Select<int, Action<JsonErrorInfo>>(i => errorInfo => Assert.Equal(expectedErrors[i], errorInfo.ErrorCode))
                          .ToArray());
        }
    }
}
