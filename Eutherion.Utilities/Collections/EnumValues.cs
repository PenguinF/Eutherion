#region License
/*********************************************************************************
 * EnumValues.cs
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

namespace Eutherion.Collections
{
    /// <summary>
    /// Contains a list of all distinct explicitly declared values for an enumeration type.
    /// </summary>
    public sealed class EnumValues<TEnum> : IReadOnlyList<TEnum>
        // Last time I checked in around 2020 I think, including 'Enum' in the where constraint here did not compile.
        // Not sure under what conditions this syntax is allowed or not.
        where TEnum : Enum
    {
        /// <summary>
        /// Gets the singleton <see cref="EnumValues{TEnum}"/> instance.
        /// </summary>
#if NET5_0_OR_GREATER
        public static EnumValues<TEnum> List { get; } = new();
#else
        public static EnumValues<TEnum> List { get; } = new EnumValues<TEnum>();
#endif
        private readonly TEnum[] distinctValues;

        private EnumValues()
        {
            TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));
            distinctValues = values.Distinct().OrderBy(x => x).ToArray();

            // FYI: There's no hidden information in TEnum values. Both versions of code below yield the same result,
            // in case [B] also regardless of whether First() or Last() is used:

            // [A]
            //distinctValues = values.Distinct().OrderBy(x => x).ToArray();
            //Console.WriteLine(string.Join(", ", distinctValues));

            // [B]
            //distinctValues = values
            //    .Zip(Enumerable.Range(0, values.Length - 1), (x, y) => (x, y))
            //    .GroupBy<(TEnum, int), TEnum>(x => x.Item1)
            //    .OrderBy(g => g.Key)
            //    .Select(g => values[g.First().Item2])
            //    .ToArray();
            //Console.WriteLine(string.Join(", ", distinctValues));

            // I was wondering because:
            // (a) Enum is shown to be an abstract class (when using Go To Definition in Visual Studio);
            // (b) Whatever ToString(TEnum) returns seems to depend on some weird combination of
            //     lexicographical order of member names as well of order of declaration:
            //
            //     enum ChessPiece { MinPieceIndex = 0, Pawn = 0, Knight, Bishop, Rook, Queen, King, MaxPieceIndex = King }
            //     string.Join(", ", distinctValues) => "Pawn, Knight, Bishop, Rook, Queen, King"
            //
            //     enum ChessPiece { MaxPieceIndex = 5, Pawn = 0, Knight, Bishop, Rook, Queen, King, MinPieceIndex = 0 }
            //     string.Join(", ", distinctValues) => "MinPieceIndex, Knight, Bishop, Rook, Queen, King"
            //
            //     enum ChessPiece { Pawn = 0, Knight, Bishop, Rook, Queen, MaxPieceIndex, MinPieceIndex = 0, King = 5 }
            //     string.Join(", ", distinctValues) => "MinPieceIndex, Knight, Bishop, Rook, Queen, MaxPieceIndex"
            //
            // If TEnum values _had_ contained hidden information, it would have made sense to have this class store
            // TEnum values in a more deterministic order than what a simple Distinct() would have allowed.
            //
            // (Disclaimer: I am aware that having different members with identical values within an enumeration may be considered 'bad practice',
            // however, this is neither a soap box nor a hill I want to die on arguing why expressing intent in code takes precedence over
            // everything else. Be that as it may, I would still research how code behaves regardless of whoever uses it, so there are never any
            // nasty surprises. Other than that, let's agree to disagree.)
        }

        /// <summary>
        /// Gets the value at the specified index in the enumeration.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the value to get.
        /// </param>
        /// <returns>
        /// The value at the specified index in the enumeration.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/>is less than 0 or greater than or equal to <see cref="Count"/>.
        /// </exception>
        public TEnum this[int index] => distinctValues[index];

        /// <summary>
        /// Gets the number of distinct declared values in the enumeration.
        /// </summary>
        public int Count => distinctValues.Length;

        /// <summary>
        /// Returns an enumerator that iterates through the enumeration values.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the enumeration values.
        /// </returns>
        public IEnumerator<TEnum> GetEnumerator() => ((ICollection<TEnum>)distinctValues).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
