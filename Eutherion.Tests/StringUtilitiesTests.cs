#region License
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

#if !NET472
#nullable enable
#endif

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
    }
}
