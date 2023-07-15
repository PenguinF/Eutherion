#region License
/*********************************************************************************
 * StringUtilitiesTests.cs
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

using Eutherion.Text;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eutherion.Tests
{
    public class StringUtilitiesTests
    {
        private static IEnumerable<(IEnumerable<string> parameters, string expectedResult)> ParameterLists() => new (IEnumerable<string>, string)[]
        {
            // Even though signature says null values are unexpected, still test those.
            (null!, ""),
            (Array.Empty<string>(), ""),
            (new string[] { null! }, "()"),
            (new string[] { "" }, "()"),
            (new string[] { "0" }, "(0)"),
            (new string[] { "\n" }, "(\n)"),
            (new string[] { "x", "y", "z" }, "(x, y, z)"),
            (new string[] { "\"x\"", "\"y\"", "\"z\"" }, "(\"x\", \"y\", \"z\")"),
        };

        public static IEnumerable<object?[]> WrappedParameterLists() => TestUtilities.Wrap(ParameterLists());

        [Theory]
        [MemberData(nameof(WrappedParameterLists))]
        public void TestDefaultParameterDisplayString(IEnumerable<string> parameters, string expectedResult)
        {
            Assert.Equal(expectedResult, StringUtilities.ToDefaultParameterListDisplayString(parameters));
        }

        private static IEnumerable<(string format, int expectedCount, bool expectedThrowsException)> FormatStringRequiredArgumentCountCases() => new (string, int, bool)[]
        {
            ("{a}", 0, true),
            ("{0}}", 1, true),
            ("{0:}}", 1, true),
            ("{0}{1", 1, true),
            ("{-1}", 0, true),
            ("{5}{-1}", 6, true),
            ("{5}{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}{-1}", 6, true),

            ("z{0}z", 1, false),
            ("z{10}z", 11, false),
            ("z{1}z{3,20}z", 4, false),
            ("z{1,-20}z{1,20}z{1:X2}z{1:{{X2,-1}{{ }}", 2, false),
        };

        [Fact]
        public void FormatStringRequiredArgumentCountArgumentCheck()
        {
            Assert.Throws<ArgumentNullException>(() => StringUtilities.FormatStringRequiredArgumentCount(null!, out _));
        }

        public static IEnumerable<object?[]> WrappedFormatStringRequiredArgumentCountCases() => TestUtilities.Wrap(FormatStringRequiredArgumentCountCases());

        [Theory]
        [MemberData(nameof(WrappedFormatStringRequiredArgumentCountCases))]
        public void FormatStringRequiredArgumentCounts(string format, int expectedCount, bool expectedThrowsException)
        {
            Assert.Equal(expectedCount, StringUtilities.FormatStringRequiredArgumentCount(format, out bool wouldThrowFormatException));
            Assert.Equal(expectedThrowsException, wouldThrowFormatException);
        }
    }
}
