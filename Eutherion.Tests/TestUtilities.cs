#region License
/*********************************************************************************
 * TestUtilities.cs
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

using System.Collections.Generic;
using System.Linq;

namespace Eutherion.Tests
{
    public class TestUtilities
    {
        public static readonly IEnumerable<bool> AllBooleanValues = new bool[] { false, true };

        public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second)
            => first.SelectMany(w => second.Select(x => (w, x)));

        public static IEnumerable<(T1, T2, T3)> CrossJoin<T1, T2, T3>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third)
            => first.SelectMany(w => second.SelectMany(x => third.Select(y => (w, x, y))));

        public static IEnumerable<(T1, T2, T3, T4)> CrossJoin<T1, T2, T3, T4>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, IEnumerable<T4> fourth)
            => first.SelectMany(w => second.SelectMany(x => third.SelectMany(y => fourth.Select(z => (w, x, y, z)))));

        public static IEnumerable<object?[]> Wrap<T1, T2, T3, T4>(IEnumerable<(T1, T2, T3, T4)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2, x.Item3, x.Item4 });

        public static IEnumerable<object?[]> Wrap<T1, T2, T3>(IEnumerable<(T1, T2, T3)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2, x.Item3 });

        public static IEnumerable<object?[]> Wrap<T1, T2>(IEnumerable<(T1, T2)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2 });

        public static IEnumerable<object?[]> Wrap<T>(IEnumerable<T> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x });
    }
}
