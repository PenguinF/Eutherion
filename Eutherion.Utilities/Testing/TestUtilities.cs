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

namespace Eutherion.Testing
{
    /// <summary>
    /// Contains utilities for building automated unit tests.
    /// </summary>
    public static class TestUtilities
    {
        /// <summary>
        /// Creates and returns an array with both boolean values.
        /// </summary>
        public static bool[] AllBooleanValues() => new bool[] { false, true };

        /// <summary>
        /// Converts a strongly typed parameter sequence to a standard type usable for xunit MemberData attribute declarations.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to wrap.</typeparam>
        /// <param name="parameterSequence">The parameter sequence to wrap.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of object arrays which can be used for xunit MemberData attribute declarations.
        /// </returns>
#if NET472
        public static IEnumerable<object[]> Wrap<T>(IEnumerable<T> parameterSequence)
            => parameterSequence.Select(x => new object[] { x });
#else
        public static IEnumerable<object?[]> Wrap<T>(IEnumerable<T> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x });
#endif

        /// <summary>
        /// Converts a strongly typed parameter sequence to a standard type usable for xunit MemberData attribute declarations.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter to wrap.</typeparam>
        /// <typeparam name="T2">The type of the second parameter to wrap.</typeparam>
        /// <param name="parameterSequence">The parameter sequence to wrap.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of object arrays which can be used for xunit MemberData attribute declarations.
        /// </returns>
#if NET472
        public static IEnumerable<object[]> Wrap<T1, T2>(IEnumerable<(T1, T2)> parameterSequence)
            => parameterSequence.Select(x => new object[] { x.Item1, x.Item2 });
#else
        public static IEnumerable<object?[]> Wrap<T1, T2>(IEnumerable<(T1, T2)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2 });
#endif

        /// <summary>
        /// Converts a strongly typed parameter sequence to a standard type usable for xunit MemberData attribute declarations.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter to wrap.</typeparam>
        /// <typeparam name="T2">The type of the second parameter to wrap.</typeparam>
        /// <typeparam name="T3">The type of the third parameter to wrap.</typeparam>
        /// <param name="parameterSequence">The parameter sequence to wrap.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of object arrays which can be used for xunit MemberData attribute declarations.
        /// </returns>
#if NET472
        public static IEnumerable<object[]> Wrap<T1, T2, T3>(IEnumerable<(T1, T2, T3)> parameterSequence)
            => parameterSequence.Select(x => new object[] { x.Item1, x.Item2, x.Item3 });
#else
        public static IEnumerable<object?[]> Wrap<T1, T2, T3>(IEnumerable<(T1, T2, T3)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2, x.Item3 });
#endif

        /// <summary>
        /// Converts a strongly typed parameter sequence to a standard type usable for xunit MemberData attribute declarations.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter to wrap.</typeparam>
        /// <typeparam name="T2">The type of the second parameter to wrap.</typeparam>
        /// <typeparam name="T3">The type of the third parameter to wrap.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter to wrap.</typeparam>
        /// <param name="parameterSequence">The parameter sequence to wrap.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of object arrays which can be used for xunit MemberData attribute declarations.
        /// </returns>
#if NET472
        public static IEnumerable<object[]> Wrap<T1, T2, T3, T4>(IEnumerable<(T1, T2, T3, T4)> parameterSequence)
            => parameterSequence.Select(x => new object[] { x.Item1, x.Item2, x.Item3, x.Item4 });
#else
        public static IEnumerable<object?[]> Wrap<T1, T2, T3, T4>(IEnumerable<(T1, T2, T3, T4)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2, x.Item3, x.Item4 });
#endif

        /// <summary>
        /// Converts a strongly typed parameter sequence to a standard type usable for xunit MemberData attribute declarations.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter to wrap.</typeparam>
        /// <typeparam name="T2">The type of the second parameter to wrap.</typeparam>
        /// <typeparam name="T3">The type of the third parameter to wrap.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter to wrap.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter to wrap.</typeparam>
        /// <param name="parameterSequence">The parameter sequence to wrap.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of object arrays which can be used for xunit MemberData attribute declarations.
        /// </returns>
#if NET472
        public static IEnumerable<object[]> Wrap<T1, T2, T3, T4, T5>(IEnumerable<(T1, T2, T3, T4, T5)> parameterSequence)
            => parameterSequence.Select(x => new object[] { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5 });
#else
        public static IEnumerable<object?[]> Wrap<T1, T2, T3, T4, T5>(IEnumerable<(T1, T2, T3, T4, T5)> parameterSequence)
            => parameterSequence.Select(x => new object?[] { x.Item1, x.Item2, x.Item3, x.Item4, x.Item5 });
#endif

        /// <summary>
        /// Joins two sequences into a single sequence with their Cartesian product.
        /// </summary>
        /// <typeparam name="T1">The type of elements in the first sequence.</typeparam>
        /// <typeparam name="T2">The type of elements in the second sequence.</typeparam>
        /// <param name="first">The first sequence to join.</param>
        /// <param name="second">The second sequence to join.</param>
        /// <returns>The Cartesian product of both sequences.</returns>
        public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second)
            => first.SelectMany(w => second.Select(x => (w, x)));

        /// <summary>
        /// Joins three sequences into a single sequence with their Cartesian product.
        /// </summary>
        /// <typeparam name="T1">The type of elements in the first sequence.</typeparam>
        /// <typeparam name="T2">The type of elements in the second sequence.</typeparam>
        /// <typeparam name="T3">The type of elements in the third sequence.</typeparam>
        /// <param name="first">The first sequence to join.</param>
        /// <param name="second">The second sequence to join.</param>
        /// <param name="third">The third sequence to join.</param>
        /// <returns>The Cartesian product of the three sequences.</returns>
        public static IEnumerable<(T1, T2, T3)> CrossJoin<T1, T2, T3>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third)
            => first.SelectMany(w => second.SelectMany(x => third.Select(y => (w, x, y))));

        /// <summary>
        /// Joins four sequences into a single sequence with their Cartesian product.
        /// </summary>
        /// <typeparam name="T1">The type of elements in the first sequence.</typeparam>
        /// <typeparam name="T2">The type of elements in the second sequence.</typeparam>
        /// <typeparam name="T3">The type of elements in the third sequence.</typeparam>
        /// <typeparam name="T4">The type of elements in the fourth sequence.</typeparam>
        /// <param name="first">The first sequence to join.</param>
        /// <param name="second">The second sequence to join.</param>
        /// <param name="third">The third sequence to join.</param>
        /// <param name="fourth">The fourth sequence to join.</param>
        /// <returns>The Cartesian product of the four sequences.</returns>
        public static IEnumerable<(T1, T2, T3, T4)> CrossJoin<T1, T2, T3, T4>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, IEnumerable<T4> fourth)
            => first.SelectMany(w => second.SelectMany(x => third.SelectMany(y => fourth.Select(z => (w, x, y, z)))));

        /// <summary>
        /// Joins five sequences into a single sequence with their Cartesian product.
        /// </summary>
        /// <typeparam name="T1">The type of elements in the first sequence.</typeparam>
        /// <typeparam name="T2">The type of elements in the second sequence.</typeparam>
        /// <typeparam name="T3">The type of elements in the third sequence.</typeparam>
        /// <typeparam name="T4">The type of elements in the fourth sequence.</typeparam>
        /// <typeparam name="T5">The type of elements in the fifth sequence.</typeparam>
        /// <param name="first">The first sequence to join.</param>
        /// <param name="second">The second sequence to join.</param>
        /// <param name="third">The third sequence to join.</param>
        /// <param name="fourth">The fourth sequence to join.</param>
        /// <param name="fifth">The fifth sequence to join.</param>
        /// <returns>The Cartesian product of the five sequences.</returns>
        public static IEnumerable<(T1, T2, T3, T4, T5)> CrossJoin<T1, T2, T3, T4, T5>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, IEnumerable<T4> fourth, IEnumerable<T5> fifth)
            => first.SelectMany(v => second.SelectMany(w => third.SelectMany(x => fourth.SelectMany(y => fifth.Select(z => (v, w, x, y, z))))));
    }
}
