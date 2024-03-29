﻿#region License
/*********************************************************************************
 * Program.cs
 *
 * Copyright (c) 2004-2024 Henk Nicolai
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

using Eutherion.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using BitExtensions = System.Numerics.BitExtensions;
using BitOperations = System.Numerics.BitOperations;

namespace Eutherion.Example472
{
    /// <summary>
    /// Contains sample code for the Eutherion package.
    /// </summary>
    class Program
    {
        // This provides a unique type value for StringKey<>.
        struct Dummy { }

        enum ChessPiece { Pawn = 0, Knight, Bishop, Rook, Queen, King }

        private static readonly Random random = new Random();

        static void Main()
        {
            // Between '/*' and '*/' below is commented out what would have to be added in later C# versions and .NET targets.

            const string prettyNullString = "<null>";
            const string prettyVoidString = "<void>";

            try
            {
                // StringKey<T>
                Console.WriteLine($"Hashes of \"1\", \"2\": {"1".GetHashCode()}, {"2".GetHashCode()}");
                Console.WriteLine($"Hashes of \"1\", \"2\" wrapped in {nameof(StringKey<Dummy>)}: {new StringKey<Dummy>("1").GetHashCode()}, {new StringKey<Dummy>("2").GetHashCode()}");
                Console.WriteLine();

                // Maybe<T>
                var possibleIntStrings = new Maybe<string>[] { Maybe<string>.Nothing, "-20", "x", "20" };

                Maybe<int> ParseInt(string input) => int.TryParse(input, out int result) ? result : Maybe<int>.Nothing;
                string DisplayWithPrettyNullString<T>(Maybe<T> maybeX) /*where T : notnull*/ => maybeX.Match(() => prettyNullString, x => Convert.ToString(x) ?? string.Empty);
                string DisplayNothingJust<T>(Maybe<T> maybeX) /*where T : notnull*/
                {
                    string s = $"[IsNothing: {maybeX.IsNothing} IsJust(): ";
                    if (maybeX.IsJust(out T/*?*/ someX)) s += $"{bool.TrueString}({someX})"; else s += bool.FalseString;
                    return s + "]";
                };
                string Join<T>(IEnumerable<T> collection) => string.Join(", ", collection);

                Console.WriteLine($"Direct string interpolation of inputs: {Join(possibleIntStrings)}");
                Console.WriteLine($"Using '{prettyNullString}' for Maybe<T>.Nothing: {Join(possibleIntStrings.Select(DisplayWithPrettyNullString))}");
                Console.WriteLine($"Of parsed integers: {Join(possibleIntStrings.Select(maybeStr => from str in maybeStr select ParseInt(str)))}");
                Console.WriteLine();

                foreach (Maybe<string> possibleIntString in possibleIntStrings)
                {
                    Console.WriteLine($"{DisplayWithPrettyNullString(possibleIntString)}:");
                    Console.Write($" {DisplayNothingJust(possibleIntString)}");
                    Console.Write($" {from str in possibleIntString from i in ParseInt(str) select i - 1}");
                    Console.Write($" {from str in possibleIntString select ParseInt(str)}");

                    var parsedInt = possibleIntString.Bind(ParseInt);
                    Console.Write($" {parsedInt}");
                    Console.Write($" {DisplayWithPrettyNullString(parsedInt)}");
                    Console.Write($" {DisplayNothingJust(parsedInt)}");
                    Console.Write($" {parsedInt.Bind(i => Maybe<string>.Just(Convert.ToString(i + 1)))}");
                    Console.WriteLine();
                }

                Console.WriteLine();

                // Mixing Maybe<> and IEnumerable<> in LINQ, ToStringInvariant

                // Create versions of the same sequence, normal vs one where its values are wrapped in Maybe<> and with extra Nothing values.
                IEnumerable<double> numbers = new[] { -1.01, 0, double.MaxValue };
                IEnumerable<Maybe<double>> maybeNumbers = numbers
                    .Select(x => (Maybe<double>)x)
                    .Intercalate(Maybe<double>.Nothing)
                    .SurroundIfAny(Maybe<double>.Nothing, Maybe<double>.Nothing);

                // Convert IEnumerable<double> to IEnumerable<string> using ToStringInvariant()
                // which returns the same result independent of the current UI culture.
                var numberDisplayStrings = from x in numbers
                                           select x.ToStringInvariant();

                // The same conversion, but now starting from a IEnumerable<Maybe<double>>.
                // Note that the outer LINQ expression depends on Enumerable.SelectMany(),
                // whereas the inner LINQ expression depends on Maybe.SelectMany().
                var maybeNumberDisplayStrings = from maybeX in maybeNumbers
                                                select (from x in maybeX
                                                        select x.ToStringInvariant());

                // Converting IEnumerable<Maybe<double>> to IEnumerable<double> by treating Maybe<> as having zero or one elements.
                var flattened = from maybeNumber in maybeNumbers
                                from number in maybeNumber.Enumerate()
                                select number;

                Console.WriteLine("IEnumerable<Maybe<>>:");
                Console.WriteLine($"  IEnumerable<double> x =         {{ {Join(numberDisplayStrings)} }}");
                Console.WriteLine($"  IEnumerable<Maybe<double>> mx = {{ {Join(maybeNumberDisplayStrings.Select(DisplayWithPrettyNullString))} }}");
                Console.WriteLine($"  Flattening mx:                  {{ {Join(flattened)} }}");
                Console.WriteLine();

                // Union<T1, T2, T3>, UnreachableException, _void
                var unionValues = new Union<_void, int, string>[] { _void._, 8, "x", _void._, -1, "-1" };

                string PrintUnionValue(Union<_void, int, string> x) => x.Match(
                    whenOption1: _ => prettyVoidString,
                    whenOption2: i => $"int: {i}",
                    whenOption3: s => $"string: \"{s}\"");

                string PrintUnionValue2(Union<_void, int, string> x)
                {
                    if (x.IsOption1(out _)) return prettyVoidString;
                    if (x.IsOption2(out int i)) return $"int: {i}";
                    if (x.IsOption3(out string s)) return $"string: \"{s}\"";
                    throw new UnreachableException();
                }

                Console.WriteLine("Union values: " + string.Join(", ", unionValues.Select(PrintUnionValue)));
                Console.WriteLine("Union values (2): " + string.Join(", ", unionValues.Select(PrintUnionValue2)));

                // LinqExtensions.ForEach
                string CheckNumberOfElements(IEnumerable<Union<_void, int, string>> collection, int count)
                    => collection.Skip(count - 1).Any(out var element)
                    ? $"There are at least {count} elements, such as this {PrintUnionValue(element)}"
                    : $"There are {count - 1} or fewer elements.";

                new[] { 5, 6, 7 }.ForEach(n => Console.WriteLine(CheckNumberOfElements(unionValues, n)));
                Console.WriteLine();

                // DictionaryExtensions.GetOrAdd
                Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                Console.WriteLine("GetOrAdd(1): " + dictionary.GetOrAdd("1", key => "one"));
                Console.WriteLine("GetOrAdd(2): " + dictionary.GetOrAdd("2", key => "two"));
                Console.WriteLine("GetOrAdd(1): " + dictionary.GetOrAdd("1", key => "another one"));
                Console.WriteLine($"Dictionary.Count: {dictionary.Count}");
                Console.WriteLine();

                // BitExtensions.SetBits
                Console.Write("Enumerating set bits in 89: ");
                Console.Write(string.Join(" + ", BitExtensions.SetBits(89u).Select(bit => $"{bit} (index: {BitOperations.Log2(bit)})")));
                Console.WriteLine();

                // LinqExtensions.Sequence, UtilityExtensions.Times
                5.Times(() =>
                {
                    Console.WriteLine();
                    Console.Write("Random values between 0 and 5, until a 0 is generated: ");
                    Func<int> generateRandomInt = () => random.Next(5);
                    Console.Write(string.Join(", ", generateRandomInt.Sequence().TakeWhile(i => i > 0)));
                });

                // LinqExtensions.Iterate, UtilityExtensions.Times
                5.Times(() =>
                {
                    Console.WriteLine();
                    Console.Write("Adding random values between 0 and 5, until reaching 20: ");
                    Func<int, int> addRandomInt = i => i + random.Next(5);
                    Console.Write(string.Join(", ", addRandomInt.Iterate(0).TakeWhile(i => i <= 20)));
                });
                Console.WriteLine();
                Console.WriteLine();

                // EnumValues<TEnum> (via EnumArray<TEnum>)
                EnumArray<ChessPiece, int> chessPieceValues = EnumArray<ChessPiece, int>.New();
                chessPieceValues[ChessPiece.Pawn] = 1;
                chessPieceValues[ChessPiece.Knight] = 3;
                chessPieceValues[ChessPiece.Bishop] = 3;
                chessPieceValues[ChessPiece.Rook] = 5;
                chessPieceValues[ChessPiece.Queen] = 9;

                for (ChessPiece piece = ChessPiece.Pawn; piece <= ChessPiece.King; piece++)
                {
                    Console.WriteLine($"{piece} is worth {chessPieceValues[piece]} points.");
                }

                Console.WriteLine();
                Console.WriteLine();

                // JsonParser
                string json = "{0, \"error\": [error,, true], \"ok\": -1 ] } /*";
                Console.Write("Parsing json: ");
                Console.WriteLine(json);
                Console.WriteLine();

                RootJsonSyntax rootSyntax = JsonParser.Parse(json);
                Console.WriteLine("Errors:");
                Console.Write(string.Join(Environment.NewLine, rootSyntax.Errors.Select(error => $"{error.ErrorLevel}: {error.ErrorCode} at {error.Start}-{error.Start + error.Length}")));
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Parse tree:");
                List<(string, string)> formattedSyntaxNodes = new List<(string, string)>(FormattedJsonSyntaxNodes(json, rootSyntax.Syntax, 0));
                int maxLength = formattedSyntaxNodes.Select(x => x.Item1.Length).Max();
                string formatString = "{0,-" + maxLength + "}{1}";
                foreach (var formatted in formattedSyntaxNodes)
                {
                    Console.WriteLine(string.Format(formatString, formatted.Item1, formatted.Item2 == null ? "" : $" '{formatted.Item2}'"));
                }
                Console.WriteLine();
                Console.WriteLine();

                // Subsequences & Intercalate & SurroundIfAny.
                string baseSequence = "abcd";
                Console.WriteLine($"Subsequences of \"{baseSequence}\":");
                baseSequence
                    // Generates all 16 subsequences.
                    .Subsequences()
                    // Surrounds every non-empty subsequence with "".
                    // This means that the empty string remains empty rather than shown as "",
                    // which is done purposefully to show the effects of SurroundIfAny().
                    .Select(x => x.SurroundIfAny("\"", "\""))
                    // Put ", " between each successive subsequence, but not at the start or end.
                    .Intercalate(", ")
                    // Flatten the entire sequence yielding a stream of characters.
                    .SelectMany(x => x)
                    // And output.
                    .ForEach(Console.Write);
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        // Returns type name and optional terminal character string for each node in the tree.
        // Indents nested syntax nodes.
        static IEnumerable<(string, string)> FormattedJsonSyntaxNodes(string json, JsonSyntax node, int indent)
        {
            int start = node.AbsoluteStart;
            int length = node.Length;
            string typeName = node.GetType().Name;
            string shortenedTypeName = typeName;
            if (shortenedTypeName.StartsWith("Json")) shortenedTypeName = shortenedTypeName.Substring(4);
            if (shortenedTypeName.EndsWith("Syntax")) shortenedTypeName = shortenedTypeName.Substring(0, shortenedTypeName.Length - 6);
            string typeNameAndSpan = length == 0
                ? $"{new string(' ', indent)}{shortenedTypeName}[]"
                : $"{new string(' ', indent)}{shortenedTypeName}[{start}-{start + node.Length - 1}]";

            if (node.IsTerminalSymbol(out _))
            {
                yield return (typeNameAndSpan, json.Substring(start, length));
            }
            else
            {
                yield return (typeNameAndSpan, null);

                int nestedIndent = indent + 2;
                for (int i = 0; i < node.ChildCount; i++)
                {
                    foreach (var formatted in FormattedJsonSyntaxNodes(json, node.GetChild(i), nestedIndent)) yield return formatted;
                }
            }
        }
    }
}
