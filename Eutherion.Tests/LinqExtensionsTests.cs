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
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.PrependIfAny((null as IEnumerable<int>)!, 0));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.PrependIfAny((null as IEnumerable<int>)!, Array.Empty<int>()));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.PrependIfAny(Array.Empty<int>(), null!));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.AppendIfAny((null as IEnumerable<int>)!, 0));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.AppendIfAny((null as IEnumerable<int>)!, Array.Empty<int>()));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.AppendIfAny(Array.Empty<int>(), null!));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.SurroundIfAny((null as IEnumerable<int>)!, 0, 0));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.SurroundIfAny((null as IEnumerable<int>)!, Array.Empty<int>(), Array.Empty<int>()));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.SurroundIfAny(Array.Empty<int>(), (null as IEnumerable<int>)!, Array.Empty<int>()));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.SurroundIfAny(Array.Empty<int>(), Array.Empty<int>(), (null as IEnumerable<int>)!));
            Assert.Throws<ArgumentNullException>(() => LinqExtensions.Intercalate((null as IEnumerable<int>)!, 0));
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

        private static char[] JoinCharacters() => new char[] { '\0', '-' };

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

        public static IEnumerable<object?[]> PrependAppendIntercalateParameters()
            => TestUtilities.Wrap(TestUtilities.CrossJoin(CharCollections(), JoinCharacters()));

        [Theory]
        [MemberData(nameof(PrependAppendIntercalateParameters))]
        public void TestPrependIfAny(IEnumerable<char> charCollection, char prependChar)
        {
            char[] charArray = charCollection.ToArray();
            string expectedString = charArray.Length == 0
                ? string.Empty
                : prependChar + new string(charCollection.ToArray());

            Assert.Equal(expectedString, new string(charCollection.PrependIfAny(prependChar).ToArray()));

            string expectedString2 = charArray.Length == 0
                ? string.Empty
                : prependChar + (prependChar + new string(charCollection.ToArray()));

            Assert.Equal(expectedString2, new string(charCollection.PrependIfAny(new string(prependChar, 2)).ToArray()));
        }

        [Theory]
        [MemberData(nameof(PrependAppendIntercalateParameters))]
        public void TestAppendIfAny(IEnumerable<char> charCollection, char appendChar)
        {
            char[] charArray = charCollection.ToArray();
            string expectedString = charArray.Length == 0
                ? string.Empty
                : new string(charCollection.ToArray()) + appendChar;

            Assert.Equal(expectedString, new string(charCollection.AppendIfAny(appendChar).ToArray()));

            string expectedString2 = charArray.Length == 0
                ? string.Empty
                : new string(charCollection.ToArray()) + appendChar + appendChar;

            Assert.Equal(expectedString2, new string(charCollection.AppendIfAny(new string(appendChar, 2)).ToArray()));
        }
        
        public static IEnumerable<object?[]> SurroundIfAnyParameters()
            => TestUtilities.Wrap(TestUtilities.CrossJoin(CharCollections(), JoinCharacters(), JoinCharacters()));

        [Theory]
        [MemberData(nameof(SurroundIfAnyParameters))]
        public void TestSurroundIfAny(IEnumerable<char> charCollection, char prependChar, char appendChar)
        {
            char[] charArray = charCollection.ToArray();
            string expectedString = charArray.Length == 0
                ? string.Empty
                : prependChar + new string(charCollection.ToArray()) + appendChar;

            Assert.Equal(expectedString, new string(charCollection.SurroundIfAny(prependChar, appendChar).ToArray()));

            string expectedString2 = charArray.Length == 0
                ? string.Empty
                : prependChar + (prependChar + new string(charCollection.ToArray()) + appendChar + appendChar);

            Assert.Equal(expectedString2, new string(charCollection.SurroundIfAny(new string(prependChar, 2), new string(appendChar, 2)).ToArray()));
        }

        [Theory]
        [MemberData(nameof(PrependAppendIntercalateParameters))]
        public void TestIntercalate(IEnumerable<char> charCollection, char separator)
        {
            string[] strArray = charCollection.Select(c => new string(c, 1)).ToArray();
            string expectedString = string.Join(separator, strArray);

            Assert.Equal(expectedString, new string(charCollection.Intercalate(separator).ToArray()));
        }
    }
}
