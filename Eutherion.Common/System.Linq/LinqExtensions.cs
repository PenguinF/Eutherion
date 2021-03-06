#region License
/*********************************************************************************
 * LinqExtensions.cs
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    /// <summary>
    /// Contains extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Determines whether there is any element in a sequence, and returns that element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// An <see cref="IEnumerable{TSource}"/> whose elements to check.
        /// </param>
        /// <param name="value">
        /// Returns the found element if true was returned, otherwise a default value.
        /// </param>
        /// <returns>
        /// true if the source sequence contains an element, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null (Nothing in Visual Basic).
        /// </exception>

        // The static code analyzer for nullable code sometimes misses that 'value' is not-null if TSource is a non-nullable type and Any() returns true.
        // This seems to be related to IEnumerable being defined as covariant (IEnumerable<out T>). On an interface with a regular type parameter,
        // the analyzer does pick it up.
        //
        // What's also interesting is that it matters what syntax is being used:
        //
        // IEnumerable<string> strings = new string[] { "1" };
        // if (strings.Any(out string x)) Console.WriteLine(x.Length);    -> Warning CS8600: Converting null literal or possible null value to non-nullable type.
        // if (strings.Any(out string? x)) Console.WriteLine(x.Length);   -> Warning CS8602: Dereference of a possibly null reference.
        // if (strings.Any(out var x)) Console.WriteLine(x.Length);       -> Fine!
        //
        // I do not know what of this is by design, a missing but planned feature, or a bug. Probably needs further research.

        public static bool Any<TSource>(this IEnumerable<TSource> source, [MaybeNullWhen(false)] out TSource value)
        {
            // TODO: Use null validation operator '!!' when migrating to C# 11.
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-parameter-check
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
            {
                value = element;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition, and returns such an element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// An <see cref="IEnumerable{TSource}"/> whose elements to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        /// A function to test each element for a condition.
        /// </param>
        /// <param name="value">
        /// Returns the found element if true was returned, otherwise a default value.
        /// </param>
        /// <returns>
        /// true if any elements in the source sequence pass the test in the specified predicate, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null (Nothing in Visual Basic).
        /// </exception>
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, [MaybeNullWhen(false)] out TSource value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var element in source)
            {
                if (predicate(element))
                {
                    value = element;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Enumerates each element of a sequence. This is useful to protect references to mutable collections
        /// from being leaked. Instead, only the elements of a mutable collection are enumerated, and casts
        /// to mutable destination collection types will fail.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of values.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> whose elements are the same as the elements in <paramref name="source"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null (Nothing in Visual Basic).
        /// </exception>
        public static IEnumerable<TSource> Enumerate<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Calls a generator function an infinite amount of times to generate a sequence.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the value to be generated in the result sequence.
        /// </typeparam>
        /// <param name="generator">
        /// The function to generate values in the sequence.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is null (Nothing in Visual Basic).
        /// </exception>
        public static IEnumerable<TResult> Sequence<TResult>(this Func<TResult> generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            for (; ; ) yield return generator();
        }

        /// <summary>
        /// Applies a generator function on a seed value an infinite amount of times to generate a sequence.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the value to be generated in the result sequence.
        /// </typeparam>
        /// <param name="generator">
        /// The function to apply repeatedly on the seed value to generate values in the sequence.
        /// </param>
        /// <param name="seed">
        /// The initial seed value for the first call to <paramref name="generator"/>.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> with the generated values.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is null (Nothing in Visual Basic).
        /// </exception>
        public static IEnumerable<TResult> Iterate<TResult>(this Func<TResult, TResult> generator, TResult seed)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            for (; ; )
            {
                seed = generator(seed);
                yield return seed;
            }
        }

        /// <summary>
        /// Performs an action on each element of a sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="action">
        /// The action to perform on each element of the sequence.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="action"/> is null (Nothing in Visual Basic).
        /// </exception>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var element in source)
            {
                action(element);
            }
        }

        /// <summary>
        /// Creates an array from a <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <returns>
        /// An array that contains the elements from the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null (Nothing in Visual Basic).
        /// </exception>
        /// <remarks>
        /// This is similar to the regular <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>,
        /// but contains more code targeted to input sequences of a specific type.
        /// </remarks>
        public static TSource[] ToArrayEx<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            // Use ICollection.CopyTo() if possible, to ensure only one array is allocated.
            TSource[] array;
            int length;
            switch (source)
            {
                // This case covers TSource[] too; expression (ICollection<TSource>)(new TSource[0]) is valid.
                case ICollection<TSource> collection:
                    length = collection.Count;
                    if (length > 0)
                    {
                        array = new TSource[length];
                        collection.CopyTo(array, 0);
                        return array;
                    }
                    break;
                case IReadOnlyCollection<TSource> readOnlyCollection:
                    length = readOnlyCollection.Count;
                    if (length > 0)
                    {
                        array = new TSource[length];
                        int index = 0;

                        try
                        {
                            foreach (var element in readOnlyCollection)
                            {
                                array[index] = element;

                                // Don't check if index >= length, assume that readOnlyCollection
                                // satisfies the contract that the number of enumerated elements is always equal to Count.
                                // Do catch IndexOutOfRangeExceptions in case the readOnlyCollection returns more
                                // elements than it should.
                                index++;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // Ignore this exception, the IReadOnlyCollection does not honor its contract.
                        }

                        return array;
                    }
                    break;
                default:
                    array = source.ToArray();
                    if (array.Length > 0) return array;
                    break;
            }

            // Default case if empty.
            return Array.Empty<TSource>();
        }
    }
}
