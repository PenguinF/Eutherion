#region License
/*********************************************************************************
 * IReadOnlyListExtensions.cs
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

namespace System.Collections.Generic
{
    /// <summary>
    /// Contains extension methods for <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        /// <summary>
        /// Searches for an element in a list that satisfies a predicate, and returns the lowest possible index of all such elements.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of the list.
        /// </typeparam>
        /// <param name="list">
        /// The read-only list to search. It is assumed to have zero-based indices.
        /// </param>
        /// <param name="match">
        /// The <see cref="Predicate{T}"/> that returns true for matching elements, and false for non-matching elements.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of a matching element, if such an element is found;
        /// otherwise <see cref="Maybe{T}.Nothing"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> and/or <paramref name="match"/> are <see langword="null"/>.
        /// </exception>
        public static Maybe<int> FindIndex<TSource>(this IReadOnlyList<TSource> list, Predicate<TSource> match)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (match == null) throw new ArgumentNullException(nameof(match));

            int n = list.Count;

            for (int i = 0; i < n; i++)
            {
                if (match(list[i])) return i;
            }

            return Maybe<int>.Nothing;
        }
    }
}
