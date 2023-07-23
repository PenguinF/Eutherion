#region License
/*********************************************************************************
 * BitExtensionTests.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Eutherion.Tests
{
    public class BitExtensionTests
    {
        [Fact]
        public void TestTestExtension()
        {
#pragma warning disable IDE0004
            Assert.False(BitExtensions.Test((byte)0));
            Assert.False(BitExtensions.Test((ushort)0));
            Assert.False(BitExtensions.Test((uint)0));
            Assert.False(BitExtensions.Test((ulong)0));

            Assert.False(BitExtensions.Test((byte)0, (byte)0));
            Assert.False(BitExtensions.Test((ushort)0, (ushort)0));
            Assert.False(BitExtensions.Test((uint)0, (uint)0));
            Assert.False(BitExtensions.Test((ulong)0, (ulong)0));

            Assert.False(BitExtensions.Test((byte)0, byte.MaxValue));
            Assert.False(BitExtensions.Test((ushort)0, ushort.MaxValue));
            Assert.False(BitExtensions.Test((uint)0, uint.MaxValue));
            Assert.False(BitExtensions.Test((ulong)0, ulong.MaxValue));

            Assert.True(BitExtensions.Test((byte)1));
            Assert.True(BitExtensions.Test((ushort)1));
            Assert.True(BitExtensions.Test((uint)1));
            Assert.True(BitExtensions.Test((ulong)1));

            Assert.False(BitExtensions.Test((byte)1, (byte)0));
            Assert.False(BitExtensions.Test((ushort)1, (ushort)0));
            Assert.False(BitExtensions.Test((uint)1, (uint)0));
            Assert.False(BitExtensions.Test((ulong)1, (ulong)0));

            Assert.True(BitExtensions.Test((byte)1, byte.MaxValue));
            Assert.True(BitExtensions.Test((ushort)1, ushort.MaxValue));
            Assert.True(BitExtensions.Test((uint)1, uint.MaxValue));
            Assert.True(BitExtensions.Test((ulong)1, ulong.MaxValue));

            Assert.True(BitExtensions.Test(unchecked((byte)sbyte.MinValue)));
            Assert.True(BitExtensions.Test(unchecked((ushort)short.MinValue)));
            Assert.True(BitExtensions.Test(unchecked((uint)int.MinValue)));
            Assert.True(BitExtensions.Test(unchecked((ulong)long.MinValue)));

            Assert.False(BitExtensions.Test(unchecked((byte)sbyte.MinValue), (byte)0));
            Assert.False(BitExtensions.Test(unchecked((ushort)short.MinValue), (ushort)0));
            Assert.False(BitExtensions.Test(unchecked((uint)int.MinValue), (uint)0));
            Assert.False(BitExtensions.Test(unchecked((ulong)long.MinValue), (ulong)0));

            Assert.True(BitExtensions.Test(unchecked((byte)sbyte.MinValue), byte.MaxValue));
            Assert.True(BitExtensions.Test(unchecked((ushort)short.MinValue), ushort.MaxValue));
            Assert.True(BitExtensions.Test(unchecked((uint)int.MinValue), uint.MaxValue));
            Assert.True(BitExtensions.Test(unchecked((ulong)long.MinValue), ulong.MaxValue));

            Assert.True(BitExtensions.Test(byte.MaxValue));
            Assert.True(BitExtensions.Test(ushort.MaxValue));
            Assert.True(BitExtensions.Test(uint.MaxValue));
            Assert.True(BitExtensions.Test(ulong.MaxValue));

            Assert.False(BitExtensions.Test(byte.MaxValue, (byte)0));
            Assert.False(BitExtensions.Test(ushort.MaxValue, (ushort)0));
            Assert.False(BitExtensions.Test(uint.MaxValue, (uint)0));
            Assert.False(BitExtensions.Test(ulong.MaxValue, (ulong)0));

            Assert.True(BitExtensions.Test(byte.MaxValue, byte.MaxValue));
            Assert.True(BitExtensions.Test(ushort.MaxValue, ushort.MaxValue));
            Assert.True(BitExtensions.Test(uint.MaxValue, uint.MaxValue));
            Assert.True(BitExtensions.Test(ulong.MaxValue, ulong.MaxValue));
#pragma warning restore IDE0004
        }

        private static IEnumerable<uint> AllOneBitValues32Bits()
        {
            uint x = 1;
            int index = 0;

            while (index < 32)
            {
                yield return x;
                x <<= 1;
                index++;
            }
        }

        private static IEnumerable<ulong> AllOneBitValues64Bits()
        {
            ulong x = 1;
            int index = 0;

            while (index < 64)
            {
                yield return x;
                x <<= 1;
                index++;
            }
        }

        private static IEnumerable<(uint value, bool expectedResult)> OneBitTestCases32Bits()
        {
            yield return (0, true);

            foreach (var oneBitValue in AllOneBitValues32Bits()) yield return (oneBitValue, true);

            uint x = 3;
            int index = 0;

            while (index < 31)
            {
                yield return (x, false);
                x <<= 1;
                index++;
            }

            foreach (var oneBitValue in AllOneBitValues32Bits()) yield return (uint.MaxValue ^ oneBitValue, false);

            yield return (uint.MaxValue, false);
        }

        private static IEnumerable<(ulong value, bool expectedResult)> OneBitTestCases64Bits()
        {
            yield return (0, true);

            foreach (var oneBitValue in AllOneBitValues64Bits()) yield return (oneBitValue, true);

            ulong x = 3;
            int index = 0;

            while (index < 63)
            {
                yield return (x, false);
                x <<= 1;
                index++;
            }

            foreach (var oneBitValue in AllOneBitValues64Bits()) yield return (ulong.MaxValue ^ oneBitValue, false);

            yield return (ulong.MaxValue, false);
        }

        public static IEnumerable<object?[]> WrappedOneBitTestCases32Bits() => TestUtilities.Wrap(OneBitTestCases32Bits());
        public static IEnumerable<object?[]> WrappedOneBitTestCases64Bits() => TestUtilities.Wrap(OneBitTestCases64Bits());

        [Theory]
        [MemberData(nameof(WrappedOneBitTestCases32Bits))]
        public void TestIsMaxOneBit32(uint value, bool expectedResult) => Assert.Equal(expectedResult, value.IsMaxOneBit());

        [Theory]
        [MemberData(nameof(WrappedOneBitTestCases64Bits))]
        public void TestIsMaxOneBit64(ulong value, bool expectedResult) => Assert.Equal(expectedResult, value.IsMaxOneBit());

        private static IEnumerable<(uint value, uint[] expectedValues)> SetBitsTestCases32Bits()
        {
            yield return (0b0, Array.Empty<uint>());

            foreach (var oneBitValue in AllOneBitValues32Bits()) yield return (oneBitValue, new[] { oneBitValue });

            yield return (0b10001, new uint[] { 0b00001, 0b10000 });
            yield return (0b110010001, new uint[] { 0b000000001, 0b000010000, 0b010000000, 0b100000000 });
            yield return (0b1010110010000, new uint[] { 0b0000000010000, 0b0000010000000, 0b0000100000000, 0b0010000000000, 0b1000000000000 });

            yield return (uint.MaxValue, AllOneBitValues32Bits().ToArray());
        }

        private static IEnumerable<(ulong value, ulong[] expectedValues)> SetBitsTestCases64Bits()
        {
            yield return (0b0, Array.Empty<ulong>());

            foreach (var oneBitValue in AllOneBitValues64Bits()) yield return (oneBitValue, new[] { oneBitValue });

            yield return (0b101, new ulong[] { 0b001, 0b100 });
            yield return (0b1010101101, new ulong[] { 0b0000000001, 0b0000000100, 0b0000001000, 0b0000100000, 0b0010000000, 0b1000000000 });
            yield return (0b1000000000000000000000000000000001, new ulong[] { 0b0000000000000000000000000000000001, 0b1000000000000000000000000000000000 });

            yield return (ulong.MaxValue, AllOneBitValues64Bits().ToArray());
        }

        public static IEnumerable<object?[]> WrappedSetBitsTestCases32Bits() => TestUtilities.Wrap(SetBitsTestCases32Bits());

        [Theory]
        [MemberData(nameof(WrappedSetBitsTestCases32Bits))]
        public void TestSetBits32(uint value, uint[] expectedValues)
            => Assert.Collection(
                value.SetBits(),
                expectedValues.Select(
                    expectedValue => new Action<uint>(actualValue => Assert.Equal(expectedValue, actualValue))).ToArray());

        public static IEnumerable<object?[]> WrappedSetBitsTestCases64Bits() => TestUtilities.Wrap(SetBitsTestCases64Bits());

        [Theory]
        [MemberData(nameof(WrappedSetBitsTestCases64Bits))]
        public void TestSetBits64(ulong value, ulong[] expectedValues)
            => Assert.Collection(
                value.SetBits(),
                expectedValues.Select(
                    expectedValue => new Action<ulong>(actualValue => Assert.Equal(expectedValue, actualValue))).ToArray());
    }
}
