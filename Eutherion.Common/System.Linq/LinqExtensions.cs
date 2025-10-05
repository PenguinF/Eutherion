#region License
/*********************************************************************************
 * LinqExtensions.cs
 *
 * Copyright (c) 2004-2025 Henk Nicolai
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
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
#if !NET472
#pragma warning disable IDE0063 // Use simple 'using' statement - not supported in .NET 4.7.2.
#endif
    /// <summary>
    /// Contains extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Flattens a sequence of sequences into a single sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of the sequences in <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of sequences of elements.
        /// </param>
        /// <returns>
        /// The flattened sequence.
        /// </returns>
        /// <remarks>
        /// This method is an alias for:
        /// <code>
        /// System.Linq.Enumerable.SelectMany(source, x => x)
        /// </code>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<IEnumerable<TSource>> source)
        {
            return source.SelectMany(x => x);
        }

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
        /// <paramref name="source"/> is <see langword="null"/>.
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
            // Argument check on 'source' to trigger the ArgumentNullException before the state machine is entered.
            // This is done for each method with 'yield return's.

            // TODO: Use null validation operator '!!' when migrating to C# 11.
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-parameter-check
            if (source == null) throw new ArgumentNullException(nameof(source));

            return AnyYielder(source, out value);
        }

        private static bool AnyYielder<TSource>(IEnumerable<TSource> source, [MaybeNullWhen(false)] out TSource value)
        {
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
        /// <paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.
        /// </exception>
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, [MaybeNullWhen(false)] out TSource value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return AnyYielder(source, predicate, out value);
        }

        private static bool AnyYielder<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, [MaybeNullWhen(false)] out TSource value)
        {
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
        /// Enumerates each element of a sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> whose elements are the same as the elements in <paramref name="source"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// The value returned from this method cannot be cast to (potentially mutable) destination collection types.
        /// This method is useful therefore to allow a protected collection to be enumerated by untrusted client code,
        /// without having to give such code a direct reference to that collection.
        /// </remarks>
        public static IEnumerable<TSource> Enumerate<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return EnumerateYielder(source);
        }

        private static IEnumerable<TSource> EnumerateYielder<TSource>(IEnumerable<TSource> source)
        {
            foreach (var element in source)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Enumerates an optional value.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the optional value.
        /// </typeparam>
        /// <param name="source">
        /// An optional value.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that enumerates the value in <paramref name="source"/> if it exists, and is otherwise empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// An intended use for this method is to allow natural integration of optional values in Enumerable.SelectMany() and LINQ expressions,
        /// so an <see cref="IEnumerable{T}"/> of <see cref="Maybe{T}"/> can be flattened into a regular <see cref="IEnumerable{T}"/>.
        /// </remarks>
        public static IEnumerable<TSource> Enumerate<TSource>(this Maybe<TSource> source)
#if !NET472
            where TSource : notnull
#endif
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Match(
                whenNothing: () => EmptyEnumerable<TSource>.Instance,
                whenJust: x => (IEnumerable<TSource>)new SingleElementEnumerable<TSource>(x));
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
        /// <paramref name="generator"/> is <see langword="null"/>.
        /// </exception>
        public static IEnumerable<TResult> Sequence<TResult>(this Func<TResult> generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            return SequenceYielder(generator);
        }

        private static IEnumerable<TResult> SequenceYielder<TResult>(Func<TResult> generator)
        {
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
        /// <paramref name="generator"/> is <see langword="null"/>.
        /// </exception>
        public static IEnumerable<TResult> Iterate<TResult>(this Func<TResult, TResult> generator, TResult seed)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            return IterateYielder(generator, seed);
        }

        private static IEnumerable<TResult> IterateYielder<TResult>(Func<TResult, TResult> generator, TResult seed)
        {
            for (; ; )
            {
                yield return seed;
                seed = generator(seed);
            }
        }

        /// <summary>
        /// Adds an element to the start of a sequence if and only if the sequence is non-empty.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="startElement">
        /// The element to prepend to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <returns>
        /// A new sequence that begins with <paramref name="startElement"/> followed by all elements of <paramref name="source"/>
        /// if <paramref name="source"/> is non-empty, otherwise an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static IEnumerable<TSource> PrependIfAny<TSource>(this IEnumerable<TSource> source, TSource startElement)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return PrependIfAnyYielder(source, startElement);
        }

        private static IEnumerable<TSource> PrependIfAnyYielder<TSource>(IEnumerable<TSource> source, TSource startElement)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    yield return startElement;
                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a sequence of elements to the start of a sequence if and only if the sequence is non-empty.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="startSequence">
        /// The sequence of elements to prepend to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <returns>
        /// A new sequence that begins with all elements of <paramref name="startSequence"/> followed by all elements of <paramref name="source"/>
        /// if <paramref name="source"/> is non-empty, otherwise an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="startSequence"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// To prepend a sequence regardless of whether or not <paramref name="source"/> is empty, use e.g:
        /// <code>
        /// <paramref name="startSequence"/>.Concat(<paramref name="source"/>);
        /// </code>
        /// </remarks>
        public static IEnumerable<TSource> PrependIfAny<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> startSequence)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startSequence == null) throw new ArgumentNullException(nameof(startSequence));

            return PrependIfAnyYielder(source, startSequence);
        }

        private static IEnumerable<TSource> PrependIfAnyYielder<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> startSequence)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    foreach (var element in startSequence)
                    {
                        yield return element;
                    }

                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Adds an element to the end of a sequence if and only if the sequence is non-empty.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="endElement">
        /// The element to append to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <returns>
        /// A new sequence that begins with all elements of <paramref name="source"/>
        /// and ends with <paramref name="endElement"/> if <paramref name="source"/> is non-empty, otherwise an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static IEnumerable<TSource> AppendIfAny<TSource>(this IEnumerable<TSource> source, TSource endElement)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return AppendIfAnyYielder(source, endElement);
        }

        private static IEnumerable<TSource> AppendIfAnyYielder<TSource>(IEnumerable<TSource> source, TSource endElement)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }

                    yield return endElement;
                }
            }
        }

        /// <summary>
        /// Adds a sequence of elements to the end of a sequence if and only if the sequence is non-empty.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="endSequence">
        /// The sequence of elements to append to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <returns>
        /// A new sequence that begins with all elements of <paramref name="source"/>
        /// and ends with all elements of <paramref name="endSequence"/> if <paramref name="source"/> is non-empty, otherwise an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="endSequence"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// To append a sequence regardless of whether or not <paramref name="source"/> is empty, use e.g:
        /// <code>
        /// <paramref name="source"/>.Concat(<paramref name="endSequence"/>);
        /// </code>
        /// </remarks>
        public static IEnumerable<TSource> AppendIfAny<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> endSequence)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (endSequence == null) throw new ArgumentNullException(nameof(endSequence));

            return AppendIfAnyYielder(source, endSequence);
        }

        private static IEnumerable<TSource> AppendIfAnyYielder<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> endSequence)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }

                    foreach (var element in endSequence)
                    {
                        yield return element;
                    }
                }
            }
        }

        /// <summary>
        /// Surrounds a sequence with start and end elements if and only if the sequence is non-empty.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="startElement">
        /// The element to prepend to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <param name="endElement">
        /// The element to append to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <returns>
        /// A new sequence that begins with <paramref name="startElement"/> followed by all elements of <paramref name="source"/>
        /// and ends with <paramref name="endElement"/> if <paramref name="source"/> is non-empty, otherwise an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// To surround <paramref name="source"/> with elements regardless of whether or not it is empty, use e.g:
        /// <code>
        /// <paramref name="source"/>.Prepend(<paramref name="startElement"/>).Append(<paramref name="endElement"/>);
        /// </code>
        /// </remarks>
        public static IEnumerable<TSource> SurroundIfAny<TSource>(this IEnumerable<TSource> source, TSource startElement, TSource endElement)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return SurroundIfAnyYielder(source, startElement, endElement);
        }

        private static IEnumerable<TSource> SurroundIfAnyYielder<TSource>(IEnumerable<TSource> source, TSource startElement, TSource endElement)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    yield return startElement;
                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }

                    yield return endElement;
                }
            }
        }

        /// <summary>
        /// Surrounds a sequence with start and end sequences if and only if the sequence is non-empty.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="startSequence">
        /// The sequence of elements to prepend to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <param name="endSequence">
        /// The sequence of elements to append to <paramref name="source"/> if <paramref name="source"/> is non-empty.
        /// </param>
        /// <returns>
        /// A new sequence that begins with all elements of <paramref name="startSequence"/> followed by all elements of <paramref name="source"/>
        /// and ends with all elements of <paramref name="endSequence"/> if <paramref name="source"/> is non-empty, otherwise an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="startSequence"/> or <paramref name="endSequence"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// To surround <paramref name="source"/> with sequences regardless of whether or not it is empty, use e.g:
        /// <code>
        /// <paramref name="startSequence"/>.Concat(<paramref name="source"/>).Concat(<paramref name="endSequence"/>);
        /// </code>
        /// </remarks>
        public static IEnumerable<TSource> SurroundIfAny<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> startSequence, IEnumerable<TSource> endSequence)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startSequence == null) throw new ArgumentNullException(nameof(startSequence));
            if (endSequence == null) throw new ArgumentNullException(nameof(endSequence));

            return SurroundIfAnyYielder(source, startSequence, endSequence);
        }

        private static IEnumerable<TSource> SurroundIfAnyYielder<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> startSequence, IEnumerable<TSource> endSequence)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    foreach (var element in startSequence)
                    {
                        yield return element;
                    }

                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }

                    foreach (var element in endSequence)
                    {
                        yield return element;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all elements of a sequence, using the specified separator between each pair of successive elements.
        /// See also <seealso cref="string.Join(string, IEnumerable{string})"/> for a similar feature.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <param name="separator">
        /// The element to use as a separator.
        /// <paramref name="separator"/> is included in the returned string if and only if <paramref name="source"/> has more than one element.
        /// </param>
        /// <returns>
        /// A new sequence that enumerates all elements of <paramref name="source"/>, and enumerating <paramref name="separator"/>
        /// between each pair of successive elements of <paramref name="source"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// For example, these two expressions have the same effect:
        /// <code>string.Join(separator, values);</code>
        /// <code>new string(values.Intercalate(separator).SelectMany(x => x).ToArray());</code>
        /// Note that the Intercalate() version needs a SelectMany() to flatten the result into a single IEnumerable&lt;char&gt;.
        /// </remarks>

        // Terminology-wise reusing 'Join' matching string.Join() would be nice, except that Enumerable.Join() exists already
        // which is an entirely different concept. Instead, go with the Haskell name and add see-also + example to the docs.
        public static IEnumerable<TSource> Intercalate<TSource>(this IEnumerable<TSource> source, TSource separator)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return IntercalateYielder(source, separator);
        }

        private static IEnumerable<TSource> IntercalateYielder<TSource>(IEnumerable<TSource> source, TSource separator)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return separator;
                        yield return enumerator.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all subsequences of a sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements.
        /// </param>
        /// <returns>
        /// A new sequence that enumerates all subsequences.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// A subsequence of a sequence can be seen as the same sequence, except that for each element
        /// one can choose whether or not to enumerate it. This means that the order of enumeration is preserved.
        /// For example, the subsequences of "abcd" would be:
        /// "", "a", "b", "ab", "c", "ac", "bc", "abc", "d", "ad", "bd", "abd", "cd", "acd", "bcd", "abcd".
        /// </remarks>
        public static IEnumerable<IEnumerable<TSource>> Subsequences<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return SubsequencesYielder(source);
        }

        private static IEnumerable<IEnumerable<TSource>> SubsequencesYielder<TSource>(IEnumerable<TSource> source)
        {
            yield return Enumerable.Empty<TSource>();

            int count = 0;

            foreach (var element in source)
            {
                foreach (var subsequence in SubsequencesYielder(source.Take(count)))
                {
                    yield return subsequence.Append(element);
                }

                count++;
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
        /// <paramref name="source"/> or <paramref name="action"/> is <see langword="null"/>.
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
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="source"/> is a <see cref="IReadOnlyCollection{T}"/>, but enumerates more elements
        /// than its <see cref="IReadOnlyCollection{T}.Count"/> property specifies.
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

                        foreach (var element in readOnlyCollection)
                        {
                            array[index] = element;

                            // Don't check if index >= length, assume that readOnlyCollection
                            // satisfies the contract that the number of enumerated elements is always equal to Count.
                            index++;
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
#if !NET472
#pragma warning restore IDE0063 // Use simple 'using' statement
#endif
}
