﻿#region License
/*********************************************************************************
 * Program.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Eutherion.Example472
{
    /// <summary>
    /// Contains sample code for the Eutherion package.
    /// </summary>
    class Program
    {
        // This provides a unique type value for StringKey<>.
        struct Dummy { }

        static void Main()
        {
            // Between '/*' and '*/' below is commented out what would have to be added in later C# versions and .NET targets.

            const string prettyNullString = "<null>";
            const string prettyVoidString = "<void>";

            try
            {
                Console.WriteLine($"hashes of \"1\", \"2\": {new StringKey<Dummy>("1").GetHashCode()}, {new StringKey<Dummy>("2").GetHashCode()}");

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

                var unionValues = new Union<_void, int, string>[] { _void._, 8, "x", _void._, -1, "-1" };

                string PrintUnionValue(Union<_void, int, string> x) => x.Match(
                    whenOption1: _ => prettyVoidString,
                    whenOption2: i => $"int: {i}",
                    whenOption3: s => $"string: \"{s}\"");

                Console.WriteLine("Union values: " + string.Join(", ", unionValues.Select(PrintUnionValue)));

                string CheckNumberOfElements(IEnumerable<Union<_void, int, string>> collection, int count)
                    => collection.Skip(count - 1).Any(out var element)
                    ? $"There are at least {count} elements, such as this {PrintUnionValue(element)}"
                    : $"There are {count - 1} or fewer elements.";

                new[] { 5, 6, 7 }.ForEach(n => Console.WriteLine(CheckNumberOfElements(unionValues, n)));
                Console.WriteLine();

                Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                Console.WriteLine("GetOrAdd(1): " + dictionary.GetOrAdd("1", key => "one"));
                Console.WriteLine("GetOrAdd(2): " + dictionary.GetOrAdd("2", key => "two"));
                Console.WriteLine("GetOrAdd(1): " + dictionary.GetOrAdd("1", key => "another one"));
                Console.WriteLine($"Dictionary.Count: {dictionary.Count}");
                Console.WriteLine();

                Console.Write("Writing 20 X'es: ");
                20.Times(() => Console.Write("X"));
                Console.WriteLine();
                Console.WriteLine();

                Console.Write("Enumerating set bits in 89: ");
                Console.Write(string.Join(" + ", 89u.SetBits().Select(bit => $"{bit} (index: {BitOperations.Log2(bit)})")));
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
