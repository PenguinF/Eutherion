﻿#region License
/*********************************************************************************
 * StringUtilitiesTests.cs
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

using Eutherion.Text;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eutherion.Tests
{
    public class StringUtilitiesTests
    {
        public static object?[][] ParameterLists() => new object?[][]
        {
            new object?[] { null, "" },
            new object?[] { Array.Empty<string?>(), "" },
            new object?[] { new string?[] { null }, "()" },
            new object?[] { new string?[] { "" }, "()" },
            new object?[] { new string?[] { "0" }, "(0)" },
            new object?[] { new string?[] { "\n" }, "(\n)" },
            new object?[] { new string?[] { "x", "y", "z" }, "(x, y, z)" },
            new object?[] { new string?[] { "\"x\"", "\"y\"", "\"z\"" }, "(\"x\", \"y\", \"z\")" },
        };

        [Theory]
        [MemberData(nameof(ParameterLists))]
        public void TestDefaultParameterDisplayString(IEnumerable<string> parameters, string expectedResult)
        {
            Assert.Equal(expectedResult, StringUtilities.ToDefaultParameterListDisplayString(parameters));
        }

        public static object?[][] FormatStringRequiredArgumentCountCases() => new object?[][]
        {
            new object?[] { "{a}", 0, true },
            new object?[] { "{0}}", 1, true },
            new object?[] { "{0:}}", 1, true },
            new object?[] { "{0}{1", 1, true },
            new object?[] { "{-1}", 0, true },
            new object?[] { "{5}{-1}", 6, true },
            new object?[] { "{5}{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}{-1}", 6, true },

            new object?[] { "z{0}z", 1, false },
            new object?[] { "z{10}z", 11, false },
            new object?[] { "z{1}z{3,20}z", 4, false },
            new object?[] { "z{1,-20}z{1,20}z{1:X2}z{1:{{X2,-1}{{ }}", 2, false },
        };

        [Fact]
        public void FormatStringRequiredArgumentCountArgumentCheck()
        {
            Assert.Throws<ArgumentNullException>(() => StringUtilities.FormatStringRequiredArgumentCount(null!, out _));
        }

        [Theory]
        [MemberData(nameof(FormatStringRequiredArgumentCountCases))]
        public void FormatStringRequiredArgumentCounts(string format, int expectedCount, bool expectedThrowsException)
        {
            Assert.Equal(expectedCount, StringUtilities.FormatStringRequiredArgumentCount(format, out bool wouldThrowFormatException));
            Assert.Equal(expectedThrowsException, wouldThrowFormatException);
        }
    }
}
