#region License
/*********************************************************************************
 * SpecializedEnumerableTests.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Eutherion.Tests
{
    public class SpecializedEnumerableTests
    {
        private static string EnumerationMethod1<T>(IEnumerable<T> enumerable)
        {
            return string.Concat(enumerable.Select(x => Convert.ToString(x)));
        }

        private static string EnumerationMethod2<T>(IEnumerable<T> enumerable)
        {
            StringBuilder sb = new();
            foreach (var x in enumerable) sb.Append(Convert.ToString(x));
            return sb.ToString();
        }

        private static string EnumerationMethod3<T>(IEnumerable<T> enumerable)
        {
            // This is how the compiler translates a foreach statement.
            StringBuilder sb = new();
            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext()) sb.Append(Convert.ToString(enumerator.Current));
            }
            return sb.ToString();
        }

        private static string EnumerationMethod4<T>(IEnumerable<T> enumerable)
        {
            // Without the using statement.
            StringBuilder sb = new();
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) sb.Append(Convert.ToString(enumerator.Current));
            return sb.ToString();
        }

        private static string EnumerationMethod5<T>(IEnumerable<T> enumerable)
        {
            StringBuilder sb = new();
            IEnumerator enumerator = ((IEnumerable)enumerable).GetEnumerator();
            while (enumerator.MoveNext()) sb.Append(Convert.ToString(enumerator.Current));
            return sb.ToString();
        }

        private static string EnumerationMethod6<T>(IEnumerable<T> enumerable)
        {
            StringBuilder sb = new();
            IEnumerator enumerator = ((IEnumerable)enumerable).GetEnumerator();
            // The first call to 'Current' should throw.
            do sb.Append(Convert.ToString(enumerator.Current)); while (enumerator.MoveNext());
            return sb.ToString();
        }

        private static string EnumerationMethod7<T>(IEnumerable<T> enumerable)
        {
            StringBuilder sb = new();
            IEnumerator enumerator = ((IEnumerable)enumerable).GetEnumerator();
            while (enumerator.MoveNext()) sb.Append(Convert.ToString(enumerator.Current));
            // This call to 'Current' should throw.
            sb.Append(Convert.ToString(enumerator.Current));
            return sb.ToString();
        }

        private static void TestEnumerableBehavior<T>(IEnumerable<T> enumerable, string resultString)
        {
            Assert.Equal(resultString, EnumerationMethod1(enumerable));
            Assert.Equal(resultString, EnumerationMethod2(enumerable));
            Assert.Equal(resultString, EnumerationMethod3(enumerable));
            Assert.Equal(resultString, EnumerationMethod4(enumerable));
            Assert.Equal(resultString, EnumerationMethod5(enumerable));
            Assert.Throws<InvalidOperationException>(() => EnumerationMethod6(enumerable));
            Assert.Throws<InvalidOperationException>(() => EnumerationMethod7(enumerable));
        }

        public static object?[][] IntEnumerables() => new object?[][]
        {
            new object?[] { EmptyEnumerable<int>.Instance, "" },
            new object?[] { new SingleElementEnumerable<int>(0), "0" },
        };

        public static object?[][] NullIntEnumerables() => new object?[][]
        {
            new object?[] { EmptyEnumerable<int?>.Instance, "" },
            new object?[] { new SingleElementEnumerable<int?>(null), "" },
        };

        public static object?[][] StringEnumerables() => new object?[][]
        {
            new object?[] { EmptyEnumerable<string>.Instance, "" },
            new object?[] { new SingleElementEnumerable<string>("x"), "x" },
        };

        public static object?[][] NullStringEnumerables() => new object?[][]
        {
            new object?[] { EmptyEnumerable<string?>.Instance, "" },
            new object?[] { new SingleElementEnumerable<string?>(null), "" },
        };

        [Theory]
        [MemberData(nameof(IntEnumerables))]
        public void IntEnumerableBehavior(IEnumerable<int> ints, string resultString) => TestEnumerableBehavior(ints, resultString);

        [Theory]
        [MemberData(nameof(NullIntEnumerables))]
        public void NullIntEnumerableBehavior(IEnumerable<int?> ints, string resultString) => TestEnumerableBehavior(ints, resultString);

        [Theory]
        [MemberData(nameof(StringEnumerables))]
        public void StringEnumerableBehavior(IEnumerable<string> strings, string resultString) => TestEnumerableBehavior(strings, resultString);

        [Theory]
        [MemberData(nameof(NullStringEnumerables))]
        public void NullStringEnumerableBehavior(IEnumerable<string?> strings, string resultString) => TestEnumerableBehavior(strings, resultString);
    }
}
