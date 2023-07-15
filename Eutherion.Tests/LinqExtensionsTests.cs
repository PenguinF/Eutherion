#region License
/*********************************************************************************
 * LinqExtensionsTests.cs
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
using System.Linq;
using Xunit;

namespace Eutherion.Tests
{
    public class LinqExtensionsTests
    {
        [Fact]
        public void ArgumentTests()
        {
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Any((null as IEnumerable<int>)!, out _));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Any((null as IEnumerable<int>)!, x => true, out _));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Any(Array.Empty<int>(), null!, out _));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Enumerate((null as IEnumerable<int>)!));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Sequence((null as Func<int>)!));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Iterate((null as Func<int, int>)!, 0));
        }

        // Implicit predicate is x => x == 0.
        private static readonly IEnumerable<(int[] haystack, int expectedIndex)> IntArrays = new (int[], int)[]
        {
            (Array.Empty<int>(), -1),
            (new int[] { 0 }, 0),
            (new int[] { 1 }, -1),
            (new int[] { 0, 0 }, 0),
            (new int[] { 0, 1 }, 0),
            (new int[] { 1, 0 }, 1),
            (new int[] { 1, 1 }, -1),
            (new int[] { 0, 1, 0 }, 0),
            (new int[] { int.MinValue, int.MaxValue, -1, 1, 0 }, 4),
            (new int[] { int.MinValue, int.MaxValue, -1, 1, 0, -1 }, 4),
            (new int[] { int.MinValue, int.MaxValue, -1, 1, 1, -1 }, -1),
        };

        public static IEnumerable<object?[]> WrappedIntArrays() => TestUtilities.Wrap(IntArrays);

        [Theory]
        [MemberData(nameof(WrappedIntArrays))]
        public void TestAny(int[] haystack, int expectedIndex)
        {
            // First test predicate-less overload.
            if (haystack.Length == 0)
            {
                Assert.False(haystack.Any(out int value));
                Assert.Equal(default, value);
            }
            else
            {
                Assert.True(haystack.Any(out int value));
                Assert.Equal(haystack[0], value);
            }

            static bool isZero(int x) => x == 0;

            if (expectedIndex < 0)
            {
                Assert.False(haystack.Any(isZero, out int value));
                Assert.Equal(default, value);
            }
            else
            {
                Assert.True(haystack.Any(isZero, out int value));
                Assert.Equal(haystack[expectedIndex], value);
            }
        }

        // Important to include collections of sizes 0, 1, >= 2.
        private static IEnumerable<char>[] CharCollections() => new IEnumerable<char>[]
        {
            Array.Empty<char>(),
            new char[] { '\0' },
            new List<char>(),
            new List<char> { '1', '2', '3' },
            ReadOnlyList<char>.Empty,
            ReadOnlyList<char>.Create(" "),
            string.Empty,
            " ",
        };

        public static IEnumerable<object?[]> WrappedCharCollections() => TestUtilities.Wrap(CharCollections());

        [Theory]
        [MemberData(nameof(WrappedCharCollections))]
        public void TestEnumerate(IEnumerable<char> charCollection)
        {
            IEnumerable<char> enumeration = charCollection.Enumerate();

            // This assertion is the reason why Enumerate() exists.
            Assert.NotSame(charCollection, enumeration);

            // Create three enumerators, and test if they correspond.
            // Two from the Enumerate() enumerable, to test that enumerators are independent.
            using var collectionEnumerator = charCollection.GetEnumerator();
            using var enumerator1 = enumeration.GetEnumerator();
            using var enumerator2 = enumeration.GetEnumerator();

            while (collectionEnumerator.MoveNext())
            {
                Assert.True(enumerator1.MoveNext());
                Assert.True(enumerator2.MoveNext());
                Assert.Equal(collectionEnumerator.Current, enumerator1.Current);
                Assert.Equal(collectionEnumerator.Current, enumerator2.Current);
            }

            Assert.False(enumerator1.MoveNext());
            Assert.False(enumerator2.MoveNext());
        }

        [Fact]
        public void TestSequence()
        {
            int index = 0;
            Func<int> closure = () => index++;

            using var enumerator1 = closure.Sequence().GetEnumerator();
            using var enumerator2 = closure.Sequence().GetEnumerator();

            Assert.True(enumerator1.MoveNext());
            Assert.Equal(0, enumerator1.Current);
            Assert.True(enumerator2.MoveNext());
            Assert.Equal(1, enumerator2.Current);
            Assert.True(enumerator1.MoveNext());
            Assert.Equal(2, enumerator1.Current);
            Assert.True(enumerator2.MoveNext());
            Assert.Equal(3, enumerator2.Current);

            Assert.Equal(4, index);
        }

        [Fact]
        public void TestIterate()
        {
            int index = 2;
            Func<int, int> closure = x => { index++; return x + 2; };

            using var enumerator1 = closure.Iterate(0).GetEnumerator();
            using var enumerator2 = closure.Iterate(1).GetEnumerator();

            Assert.True(enumerator1.MoveNext());
            Assert.Equal(0, enumerator1.Current);
            Assert.Equal(2, index);

            Assert.True(enumerator2.MoveNext());
            Assert.Equal(1, enumerator2.Current);
            Assert.Equal(2, index);

            Assert.True(enumerator1.MoveNext());
            Assert.Equal(2, enumerator1.Current);
            Assert.Equal(3, index);

            Assert.True(enumerator2.MoveNext());
            Assert.Equal(3, enumerator2.Current);
            Assert.Equal(4, index);

            Assert.True(enumerator1.MoveNext());
            Assert.Equal(4, enumerator1.Current);
            Assert.Equal(5, index);

            Assert.True(enumerator2.MoveNext());
            Assert.Equal(5, enumerator2.Current);
            Assert.Equal(6, index);
        }
    }
}
