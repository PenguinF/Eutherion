#region License
/*********************************************************************************
 * ReadOnlyListTests.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eutherion.Tests
{
    public class ReadOnlyListTests
    {
        private static readonly Random random = new();

        private static Func<int> GenerateRandomInt(int maxValue) => () => random.Next(maxValue);

        private static IEnumerable<int> CreateRandomIntSequence(int length) => GenerateRandomInt(10).Sequence().Take(length);

        [Fact]
        public void NullSourceThrows()
        {
            Assert.Throws<ArgumentNullException>(() => ReadOnlyList<int>.Create(null!));
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
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void CorrectCount(int length)
        {
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(CreateRandomIntSequence(length));
            Assert.Equal(length, list.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void ExpectedElements(int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(expectedList);
            for (int i = 0; i < length; i++) Assert.Equal(expectedList[i], list[i]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void Enumeration(int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(expectedList);

            int i = 0;
            foreach (int value in list)
            {
                Assert.Equal(expectedList[i], value);
                i++;
            }

            Assert.Equal(expectedList.Length, i);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void GenericEnumeration(int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(expectedList);

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
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void NonGenericEnumeration(int length)
        {
            int[] expectedList = CreateRandomIntSequence(length).ToArray();
            ReadOnlyList<int> list = ReadOnlyList<int>.Create(expectedList);

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
    }
}
