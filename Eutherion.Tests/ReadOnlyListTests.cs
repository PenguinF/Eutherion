#region License
/*********************************************************************************
 * ReadOnlyListTests.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eutherion.Tests
{
    public class ReadOnlyListTests
    {
        public struct ListCreationMethod
        {
            public Func<IEnumerable<int>, ReadOnlyList<int>> Create;

            public ListCreationMethod(Func<IEnumerable<int>, ReadOnlyList<int>> create)
            {
                Create = create;
            }
        }

        private static readonly Random random = new();

        private static Func<int> GenerateRandomInt(int maxValue) => () => random.Next(maxValue);

        private static IEnumerable<int> CreateRandomIntSequence(int length) => GenerateRandomInt(10).Sequence().Take(length);

        // For all unit tests, it shouldn't matter in what way a ReadOnlyList is created, so cross join all unit tests with creation methods.
        private static IEnumerable<ListCreationMethod> CreationMethods()
        {
            yield return new ListCreationMethod(ReadOnlyList<int>.Create);

            yield return new ListCreationMethod(x =>
            {
                var builder = new ReadOnlyList<int>.Builder();
                x.ForEach(builder.Add);
                return builder.Commit();
            });
        }

        private static readonly int[] LengthTestCases = new int[] { 0, 1, 4, 15, 16, 100 };

        public static IEnumerable<object?[]> WrappedLengths()
            => TestUtilities.Wrap(LengthTestCases);

        public static IEnumerable<object?[]> WrappedCreationMethodsAndLengths()
            => TestUtilities.Wrap(TestUtilities.CrossJoin(CreationMethods(), LengthTestCases));

        [Fact]
        public void NullArgumentChecks()
        {
            Assert.Throws<ArgumentNullException>(() => ReadOnlyList<int>.Create(null!));
            Assert.Throws<ArgumentNullException>(() => IReadOnlyListExtensions.FindIndex<int>(null!, n => false));
            Assert.Throws<ArgumentNullException>(() => IReadOnlyListExtensions.FindIndex<int>(Array.Empty<int>(), null!));
        }

        [Fact]
        public void EmptyReadOnlyListIsAlwaysSameInstance()
        {
            Assert.Same(ReadOnlyList<int>.Empty, ReadOnlyList<int>.Empty);
            Assert.Same(ReadOnlyList<int>.Empty, ReadOnlyList<int>.Create(ReadOnlyList<int>.Empty));
            Assert.Same(ReadOnlyList<int>.Empty, ReadOnlyList<int>.Create(Array.Empty<int>()));
            Assert.Same(ReadOnlyList<int>.Empty, ReadOnlyList<int>.Create(CreateRandomIntSequence(0)));
        }

        [Fact]
        public void ReuseReadOnlyList()
        {
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(CreateRandomIntSequence(random.Next(10) + 10));
            Assert.Same(list, ReadOnlyList<int>.Create(list));
        }

        [Theory]
        [MemberData(nameof(WrappedCreationMethodsAndLengths))]
        public void CorrectCount(ListCreationMethod creationMethod, int length)
        {
            ReadOnlyList<int> list = creationMethod.Create(CreateRandomIntSequence(length));
            Assert.Equal(length, list.Count);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void OutOfBoundsThrowsArgumentOutOfRange(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { int x = ReadOnlyList<int>.Empty[index]; });
        }

        [Theory]
        [MemberData(nameof(WrappedCreationMethodsAndLengths))]
        public void ExpectedElements(ListCreationMethod creationMethod, int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = creationMethod.Create(expectedList);
            for (int i = 0; i < length; i++) Assert.Equal(expectedList[i], list[i]);
        }

        [Fact]
        public void EmptyEnumerationThrowsException()
        {
            IEnumerator enumerator = ReadOnlyList<int>.Empty.GetEnumerator();
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => { object x = enumerator.Current; });
        }

        [Theory]
        [MemberData(nameof(WrappedCreationMethodsAndLengths))]
        public void Enumeration(ListCreationMethod creationMethod, int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = creationMethod.Create(expectedList);

            int i = 0;
            foreach (int value in list)
            {
                Assert.Equal(expectedList[i], value);
                i++;
            }

            Assert.Equal(expectedList.Length, i);
        }

        [Theory]
        [MemberData(nameof(WrappedCreationMethodsAndLengths))]
        public void GenericEnumeration(ListCreationMethod creationMethod, int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = creationMethod.Create(expectedList);

            int i = 0;
            using IEnumerator<int> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.Equal(expectedList[i], enumerator.Current);
                i++;
            }

            Assert.Equal(expectedList.Length, i);
        }

        [Theory]
        [MemberData(nameof(WrappedCreationMethodsAndLengths))]
        public void NonGenericEnumeration(ListCreationMethod creationMethod, int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = creationMethod.Create(expectedList);

            int i = 0;
            IEnumerator enumerator = ((IEnumerable)list).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.Equal(expectedList[i], enumerator.Current);
                i++;
            }

            Assert.Equal(expectedList.Length, i);
        }

        [Fact]
        public void TestSharedReferences()
        {
            Box<int> value1 = new(1);
            Box<int> value2 = new(2);
            Box<int>[] array = new Box<int>[] { value1 };
            ReadOnlyList<Box<int>> list = ReadOnlyList<Box<int>>.Create(array);

            Assert.Same(value1, list[0]);
            Assert.Same(array[0], list[0]);

            // This is to test that the ReadOnlyList doesn't use the passed in array to store its values,
            // which would allow modification after it's been created.
            array[0] = value2;
            Assert.Same(value1, list[0]);
            Assert.NotSame(array[0], list[0]);

            value1.Value = 2;
            Assert.Equal(array[0].Value, list[0].Value);
        }

        [Fact]
        public void EmptyBuilderReturnsEmptyList()
        {
            Assert.Same(ReadOnlyList<int>.Empty, new ReadOnlyList<int>.Builder().Commit());
        }

        [Fact]
        public void BuilderWithOneElement()
        {
            // Basically to ensure collection initializer syntax works.
            var list = new ReadOnlyList<int>.Builder { 0 }.Commit();
            Assert.Collection(list, x => Assert.Equal(0, x));
        }

        [Theory]
        [MemberData(nameof(WrappedLengths))]
        public void BuilderWithElements(int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();

            // Create the builder.
            var builder = new ReadOnlyList<int>.Builder();
            expectedList.ForEach(builder.Add);

            // Assert the elements are all there.
            Assert.Collection(builder, expectedList.Select(expected => new Action<int>(actual => Assert.Equal(expected, actual))).ToArray());
            Assert.Equal(expectedList.Length, builder.Count);

            // Commit, then assert that the builder is empty again.
            builder.Commit();
            Assert.Empty(builder);

            // Also assert that the non-generic enumerator throws appropriately.
            IEnumerator enumerator = builder.GetEnumerator();
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => { object x = enumerator.Current; });
        }

        private static IEnumerable<(IEnumerable<int> haystack, int needle, int expectedIndex)> HaystacksAndNeedles() => new (IEnumerable<int>, int, int)[]
        {
            (Array.Empty<int>(), 0, -1),
            (new int[] { 1 }, 0, -1),
            (new int[] { 2 }, 0, -1),
            (new int[] { 1, 2, 3 }, 1, 0),
            (new int[] { 1, 2, 3 }, 2, 1),
            (new int[] { 1, 2, 3 }, 3, 2),
            (new int[] { 1, 1, 1 }, 1, 0),
        };

        public static IEnumerable<object?[]> WrappedHaystacksAndNeedles() => TestUtilities.Wrap(HaystacksAndNeedles());

        [Theory]
        [MemberData(nameof(WrappedHaystacksAndNeedles))]
        public void TestFindIndex(IEnumerable<int> haystack, int needle, int expectedIndex)
        {
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(haystack);

            if (expectedIndex < 0)
            {
                Assert.True(list.FindIndex(n => n == needle).IsNothing);
            }
            else
            {
                Assert.True(list.FindIndex(n => n == needle).IsJust(out int foundIndex));
                Assert.Equal(expectedIndex, foundIndex);
            }
        }
    }
}
