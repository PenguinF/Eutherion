#region License
/*********************************************************************************
 * EnumValuesTests.cs
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

using Eutherion.Collections;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Eutherion.Tests
{
    public class EnumValuesTests
    {
        enum EmptyTestEnum { }

        [Fact]
        public void EmptyEnum()
        {
            Assert.Empty(EnumValues<EmptyTestEnum>.List);
        }

        enum TestEnumWithNegativeValue
        {
            Zero = 0,
            One = 1,
            MinusOne = -1,
        }

        [Fact]
        public void EnumWithNegativeValue()
        {
            Assert.Collection(EnumValues<TestEnumWithNegativeValue>.List,
                value1 => Assert.Equal(TestEnumWithNegativeValue.MinusOne, value1),
                value2 => Assert.Equal(TestEnumWithNegativeValue.Zero, value2),
                value3 => Assert.Equal(TestEnumWithNegativeValue.One, value3));
        }

        enum TestDiscontinuousEnum
        {
            MinusOne = -1,
            Zero = 0,
            Two = 2,
        }

        [Fact]
        public void DiscontinuousEnum()
        {
            Assert.Collection(EnumValues<TestDiscontinuousEnum>.List,
                value1 => Assert.Equal(TestDiscontinuousEnum.MinusOne, value1),
                value2 => Assert.Equal(TestDiscontinuousEnum.Zero, value2),
                value3 => Assert.Equal(TestDiscontinuousEnum.Two, value3));
        }

        [SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "This is for testing purposes.")]
        enum TestEnumWithDuplicates
        {
            A1 = 0,
            B2 = 1,
            C = 2,
            A2 = 0,
            B1 = 1,
        }

        [Fact]
        public void EnumWithDuplicates()
        {
            Assert.Collection(EnumValues<TestEnumWithDuplicates>.List,
                value1 => Assert.Equal(TestEnumWithDuplicates.A1, value1),
                value2 => Assert.Equal(TestEnumWithDuplicates.B1, value2),
                value3 => Assert.Equal(TestEnumWithDuplicates.C, value3));
        }
    }
}
