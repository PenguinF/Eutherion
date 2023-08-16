#region License
/*********************************************************************************
 * JsonSyntaxTests.cs
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
using System.Numerics;
using Xunit;

namespace Eutherion.Text.Json.Tests
{
    public class JsonSyntaxTests
    {
        internal static GreenJsonMultiValueSyntax CreateMultiValue(GreenJsonValueSyntax valueContentNode)
        {
            return new GreenJsonMultiValueSyntax(
                new SingleElementEnumerable<GreenJsonValueWithBackgroundSyntax>(new GreenJsonValueWithBackgroundSyntax(
                    GreenJsonBackgroundListSyntax.Empty,
                    valueContentNode)),
                GreenJsonBackgroundListSyntax.Empty);
        }

        [Fact]
        public void ArgumentChecks()
        {
            Assert.Throws<ArgumentNullException>("source", () => GreenJsonBackgroundListSyntax.Create(null!));

            Assert.Throws<ArgumentOutOfRangeException>("length", () => GreenJsonCommentSyntax.Create(-1));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => GreenJsonCommentSyntax.Create(0));

            Assert.Throws<ArgumentOutOfRangeException>("length", () => new GreenJsonErrorStringSyntax(0));

            Assert.Throws<ArgumentOutOfRangeException>("length", () => new GreenJsonIntegerLiteralSyntax(0, -1));

            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonKeyValueSyntax((ArrayBuilder<GreenJsonMultiValueSyntax>)null!));
            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonKeyValueSyntax((IEnumerable<GreenJsonMultiValueSyntax>)null!));
            Assert.Throws<ArgumentException>("valueSectionNodes", () => new GreenJsonKeyValueSyntax(EmptyEnumerable<GreenJsonMultiValueSyntax>.Instance));

            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonListSyntax((ArrayBuilder<GreenJsonMultiValueSyntax>)null!, false));
            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonListSyntax((IEnumerable<GreenJsonMultiValueSyntax>)null!, false));
            Assert.Throws<ArgumentException>("listItemNodes", () => new GreenJsonListSyntax(EmptyEnumerable<GreenJsonMultiValueSyntax>.Instance, false));

            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonMapSyntax((ArrayBuilder<GreenJsonKeyValueSyntax>)null!, false));
            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonMapSyntax((IEnumerable<GreenJsonKeyValueSyntax>)null!, false));
            Assert.Throws<ArgumentException>("keyValueNodes", () => new GreenJsonMapSyntax(EmptyEnumerable<GreenJsonKeyValueSyntax>.Instance, false));

            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonMultiValueSyntax((ArrayBuilder<GreenJsonValueWithBackgroundSyntax>)null!, GreenJsonBackgroundListSyntax.Empty));
            Assert.Throws<ArgumentNullException>("source", () => new GreenJsonMultiValueSyntax((IEnumerable<GreenJsonValueWithBackgroundSyntax>)null!, GreenJsonBackgroundListSyntax.Empty));
            Assert.Throws<ArgumentException>("valueNodes", () => new GreenJsonMultiValueSyntax(EmptyEnumerable<GreenJsonValueWithBackgroundSyntax>.Instance, GreenJsonBackgroundListSyntax.Empty));
            Assert.Throws<ArgumentNullException>("backgroundAfter", () => new GreenJsonMultiValueSyntax(
                new SingleElementEnumerable<GreenJsonValueWithBackgroundSyntax>(new GreenJsonValueWithBackgroundSyntax(
                    GreenJsonBackgroundListSyntax.Empty, GreenJsonMissingValueSyntax.Value)), null!));

            Assert.Throws<ArgumentNullException>("valueDelimiter", () => new GreenJsonRootLevelValueDelimiterSyntax((GreenJsonCurlyCloseSyntax)null!));
            Assert.Throws<ArgumentNullException>("valueDelimiter", () => new GreenJsonRootLevelValueDelimiterSyntax((GreenJsonSquareBracketCloseSyntax)null!));

            Assert.Throws<ArgumentNullException>("value", () => new GreenJsonStringLiteralSyntax(null!, 2));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new GreenJsonStringLiteralSyntax(string.Empty, -1));

            Assert.Throws<ArgumentOutOfRangeException>("length", () => new GreenJsonUndefinedValueSyntax(0));

            Assert.Throws<ArgumentOutOfRangeException>("length", () => new GreenJsonUnterminatedMultiLineCommentSyntax(-1));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new GreenJsonUnterminatedMultiLineCommentSyntax(0));

            Assert.Throws<ArgumentNullException>("backgroundBefore", () => new GreenJsonValueWithBackgroundSyntax(null!, GreenJsonBooleanLiteralSyntax.False.Instance));
            Assert.Throws<ArgumentNullException>("contentNode", () => new GreenJsonValueWithBackgroundSyntax(GreenJsonBackgroundListSyntax.Empty, null!));

            Assert.Throws<ArgumentOutOfRangeException>("length", () => GreenJsonWhitespaceSyntax.Create(-1));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => GreenJsonWhitespaceSyntax.Create(0));

            Assert.Throws<ArgumentNullException>("value", () => JsonValue.TryCreate((string)null!));
            Assert.Throws<ArgumentException>("value", () => JsonValue.TryCreate(string.Empty));

            Assert.Throws<ArgumentNullException>("json", () => new RootJsonSyntax(
                null!,
                CreateMultiValue(new GreenJsonIntegerLiteralSyntax(0, 1)),
                ReadOnlyList<JsonErrorInfo>.Empty));
            Assert.Throws<ArgumentNullException>("syntax", () => new RootJsonSyntax(
                string.Empty,
                null!,
                ReadOnlyList<JsonErrorInfo>.Empty));
            Assert.Throws<ArgumentNullException>("errors", () => new RootJsonSyntax(
                "0",
                CreateMultiValue(new GreenJsonIntegerLiteralSyntax(0, 1)),
                null!));

            // Lengths should be equal.
            Assert.Throws<ArgumentException>("json", () => new RootJsonSyntax(
                string.Empty,
                CreateMultiValue(new GreenJsonIntegerLiteralSyntax(0, 1)),
                ReadOnlyList<JsonErrorInfo>.Empty));
            Assert.Throws<ArgumentException>("json", () => new RootJsonSyntax(
                "-0",
                CreateMultiValue(new GreenJsonIntegerLiteralSyntax(0, 1)),
                ReadOnlyList<JsonErrorInfo>.Empty));
        }

        private const int SharedInstanceLengthMinusTwo = GreenJsonWhitespaceSyntax.SharedInstanceLength - 2;
        private const int SharedInstanceLengthMinusOne = GreenJsonWhitespaceSyntax.SharedInstanceLength - 1;
        private const int SharedInstanceLengthPlusOne = GreenJsonWhitespaceSyntax.SharedInstanceLength + 1;
        private const int SharedInstanceLengthPlusTwo = GreenJsonWhitespaceSyntax.SharedInstanceLength + 2;

        [Theory]
        [InlineData(1)]
        [InlineData(SharedInstanceLengthMinusTwo)]
        [InlineData(SharedInstanceLengthMinusOne)]
        [InlineData(GreenJsonWhitespaceSyntax.SharedInstanceLength)]
        [InlineData(SharedInstanceLengthPlusOne)]
        [InlineData(SharedInstanceLengthPlusTwo)]
        public void SharedInstanceHasCorrectLength(int length)
        {
            Assert.Equal(length, GreenJsonWhitespaceSyntax.Create(length).Length);
            Assert.Equal(length, GreenJsonCommentSyntax.Create(length).Length);
        }

        [Fact]
        public void JsonSymbolsWithLengthOne()
        {
            // As long as there's code around which depends on these symbols having length 1, this unit test is needed.
            Assert.Equal(1, GreenJsonColonSyntax.Value.Length);
            Assert.Equal(1, GreenJsonCommaSyntax.Value.Length);
            Assert.Equal(1, GreenJsonCurlyCloseSyntax.Value.Length);
            Assert.Equal(1, GreenJsonCurlyOpenSyntax.Value.Length);
            Assert.Equal(1, GreenJsonSquareBracketCloseSyntax.Value.Length);
            Assert.Equal(1, GreenJsonSquareBracketOpenSyntax.Value.Length);
            Assert.Equal(1, GreenJsonUnknownSymbolSyntax.Value.Length);
        }

        [Theory]
        [InlineData("\"", 10)]
        [InlineData("{}", 3)]
        // No newline conversions.
        [InlineData("\n", 1)]
        [InlineData("\r\n", 2)]
        public void UnchangedStringLiteralValueParameter(string value, int length)
        {
            // Length includes quotes for json strings.
            var jsonString = new GreenJsonStringLiteralSyntax(value, length + 2);
            Assert.Equal(value, jsonString.Value);
            Assert.Equal(length + 2, jsonString.Length);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(255)]
        [InlineData(2000)]
        public void UnchangedParametersInUnterminatedMultiLineComment(int commentTextLength)
        {
            var symbol = new GreenJsonUnterminatedMultiLineCommentSyntax(commentTextLength);
            Assert.Equal(commentTextLength, symbol.Length);
        }

        [Theory]
        [InlineData("false", false)]
        [InlineData("true", true)]
        public void BooleanJsonValues(string value, bool expectedBooleanValue)
        {
            var node = JsonValue.TryCreate(value);
            GreenJsonBooleanLiteralSyntax boolNode;
            if (expectedBooleanValue)
            {
                boolNode = Assert.IsType<GreenJsonBooleanLiteralSyntax.True>(node);
            }
            else
            {
                boolNode = Assert.IsType<GreenJsonBooleanLiteralSyntax.False>(node);
            }
            Assert.Equal(expectedBooleanValue, boolNode.Value);
            Assert.Equal(value, boolNode.LiteralJsonValue);
            Assert.Equal(value.Length, boolNode.Length);
        }

        private static IEnumerable<(string value, int expectedIntegerValue)> GetIntegerJsonValues()
        {
            yield return ("0", 0);
            yield return ("+1", 1);
            yield return ("-0", 0);
            yield return ("-1", -1);
            yield return ("-9", -9);
            yield return ("20", 20);
            yield return ("2147483647", 2147483647);
            yield return ("+2147483647", 2147483647);
            yield return ("000002147483647", 2147483647);
            yield return ("-2147483648", -2147483648);
            yield return ("-000002147483648", -2147483648);
        }

        public static IEnumerable<object?[]> WrappedIntegerJsonValues() => TestUtilities.Wrap(GetIntegerJsonValues());

        [Theory]
        [MemberData(nameof(WrappedIntegerJsonValues))]
        public void IntegerJsonValues(string value, int expectedIntegerValue)
        {
            var node = JsonValue.TryCreate(value);
            var intNode = Assert.IsType<GreenJsonIntegerLiteralSyntax>(node);
            Assert.Equal(expectedIntegerValue, (int)intNode.Value);
            Assert.Equal(value.Length, intNode.Length);
        }

        private static IEnumerable<(string value, ulong expectedIntegerValue)> UnsignedLongValues
        {
            get
            {
                yield return ("0000021474836470", 21474836470);
                yield return ("18446744073709551599", 18446744073709551599);
                yield return ("18446744073709551600", 18446744073709551600);
                yield return ("18446744073709551609", 18446744073709551609);
                yield return ("18446744073709551610", 18446744073709551610);
                yield return ("18446744073709551615", 18446744073709551615);
            }
        }

        public static IEnumerable<object?[]> WrappedUnsignedLongJsonValues() => TestUtilities.Wrap(UnsignedLongValues);

        [Theory]
        [MemberData(nameof(WrappedUnsignedLongJsonValues))]
        public void UnsignedLongJsonValues(string value, ulong expectedIntegerValue)
        {
            var node = JsonValue.TryCreate(value);
            var intNode = Assert.IsType<GreenJsonIntegerLiteralSyntax>(node);
            Assert.Equal(expectedIntegerValue, (ulong)intNode.Value);
            Assert.Equal(value.Length, intNode.Length);
        }

        [Fact]
        public void BigIntegerJsonValue()
        {
            // ulong.MaxValue + 1
            var node = JsonValue.TryCreate("18446744073709551616");
            var intNode = Assert.IsType<GreenJsonIntegerLiteralSyntax>(node);
            BigInteger bigInteger = intNode.Value - 1;
            Assert.Equal(ulong.MaxValue, (ulong)bigInteger);
        }

        [Theory]
        [InlineData("++0")]
        [InlineData("--0")]
        [InlineData("0.0")]
        [InlineData("1e+1")]
        [InlineData("{}")]
        [InlineData("\"\"")]
        public void UnknownJsonValues(string value)
        {
            // Assert that none of these create a legal json value.
            Assert.Null(JsonValue.TryCreate(value));
        }

        private static IEnumerable<(IEnumerable<GreenJsonValueSyntax> valueNodes, int expectedFilteredItemNodesCount)> ListValuesTestCases()
        {
            yield return (new GreenJsonValueSyntax[] { GreenJsonMissingValueSyntax.Value }, 0);
            yield return (new GreenJsonValueSyntax[] { new GreenJsonIntegerLiteralSyntax(1, 1) }, 1);

            yield return (new GreenJsonValueSyntax[] { GreenJsonMissingValueSyntax.Value, new GreenJsonIntegerLiteralSyntax(1, 1) }, 2);
            yield return (new GreenJsonValueSyntax[] { new GreenJsonIntegerLiteralSyntax(1, 1), GreenJsonMissingValueSyntax.Value }, 1);
            yield return (new GreenJsonValueSyntax[] { GreenJsonMissingValueSyntax.Value, new GreenJsonIntegerLiteralSyntax(1, 1), GreenJsonMissingValueSyntax.Value }, 2);
            yield return (new GreenJsonValueSyntax[] { new GreenJsonIntegerLiteralSyntax(1, 1), GreenJsonMissingValueSyntax.Value, GreenJsonMissingValueSyntax.Value }, 2);
        }

        public static IEnumerable<object?[]> WrappedListValuesTestCases() => TestUtilities.Wrap(ListValuesTestCases());

        [Theory]
        [MemberData(nameof(WrappedListValuesTestCases))]
        public void CorrectFilteredListItemNodesCount(IEnumerable<GreenJsonValueSyntax> valueNodes, int expectedFilteredItemNodesCount)
        {
            GreenJsonListSyntax list = new(valueNodes.Select(CreateMultiValue).ToArray(), false);
            Assert.Equal(expectedFilteredItemNodesCount, list.FilteredListItemNodeCount);
        }
    }
}
