﻿#region License
/*********************************************************************************
 * JsonMultiValueSyntax.Green.cs
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

using System;
using System.Collections.Generic;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains one or more value nodes together with all background syntax that precedes and follows it.
    /// </summary>
    public sealed class GreenJsonMultiValueSyntax : ISpan
    {
        // This syntax is generated everywhere a single value is expected.
        // It is a parse error if zero values, or two or more values are given, the exception being
        // an optional value between a comma following elements of an array/object and its closing
        // ']' or '}' character. (E.g. [0,1,2,] is not an error.)
        //
        // Below are a couple of examples to show in what states a GreenJsonMultiValueSyntax node can be.
        // The given example json represents an array with one enclosing GreenJsonMultiValueSyntax.
        // For clarity, the surrounding brackets are given, though they are not part of this syntax node.
        //
        // []            - ValueNode.BackgroundBefore.BackgroundSymbols.Count == 0
        //                 ValueNode.ContentNode.IsMissingValue
        //                 ValueNodes.Count == 1
        //                 BackgroundAfter.BackgroundSymbols.Count == 0
        //
        // [/**/]        - ValueNode.BackgroundBefore.BackgroundSymbols.Count == 1  (one JsonComment)
        //                 ValueNode.ContentNode.IsMissingValue
        //                 ValueNodes.Count == 1
        //                 BackgroundAfter.BackgroundSymbols.Count == 0
        //
        // [/**/0]       - ValueNode.BackgroundBefore.BackgroundSymbols.Count == 1
        //                 ValueNode.ContentNode is GreenJsonIntegerLiteralSyntax
        //                 ValueNodes.Count == 1
        //                 BackgroundAfter.BackgroundSymbols.Count == 0
        //
        // [/**/0/**/]   - ValueNode.BackgroundBefore.BackgroundSymbols.Count == 1
        //                 ValueNode.ContentNode is GreenJsonIntegerLiteralSyntax
        //                 ValueNodes.Count == 1
        //                 BackgroundAfter.BackgroundSymbols.Count == 1
        //
        // [0 ]          - ValueNode.BackgroundBefore.BackgroundSymbols.Count == 0
        //                 ValueNode.ContentNode is GreenJsonIntegerLiteralSyntax
        //                 ValueNodes.Count == 1
        //                 BackgroundAfter.BackgroundSymbols.Count == 1             (one JsonWhitespace)
        //
        // [ 0 false ]   - ValueNode.BackgroundBefore.BackgroundSymbols.Count == 1
        //                 ValueNode.ContentNode is GreenJsonIntegerLiteralSyntax
        //                 ValueNodes.Count == 2
        //                 ValueNodes[1].BackgroundSymbols.Count == 1
        //                 ValueNodes[1].ContentNode is GreenJsonBooleanLiteralSyntax
        //                 BackgroundAfter.BackgroundSymbols.Count == 1
        //
        // Only the first ValueNode (ValueNodes[0]) can be GreenJsonMissingValueSyntax, and if it is, ValueNodes.Count is always 1,
        // ValueNode.BackgroundBefore may still be non-empty, and BackgroundAfter is always empty.
        //
        // The main reason this structure exists like this is because there needs to be a place where
        // background symbols between two control symbols can be stored.

        /// <summary>
        /// Gets the syntax node containing the first value.
        /// Is <see cref="GreenJsonMissingValueSyntax"/> if a value was expected but none given. (E.g. in "[0,,2]", between both commas.)
        /// </summary>
        public GreenJsonValueWithBackgroundSyntax ValueNode => ValueNodes[0];

        /// <summary>
        /// Gets the non-empty list of value nodes.
        /// </summary>
        public ReadOnlySpanList<GreenJsonValueWithBackgroundSyntax> ValueNodes { get; }

        /// <summary>
        /// Gets the background symbols which directly trail the value nodes.
        /// </summary>
        public GreenJsonBackgroundListSyntax BackgroundAfter { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public int Length => ValueNodes.Length + BackgroundAfter.Length;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonMultiValueSyntax"/> from an <see cref="ArrayBuilder{T}"/>.
        /// This empties the array builder.
        /// </summary>
        /// <param name="valueNodesBuilder">
        /// The non-empty builder with value nodes.
        /// </param>
        /// <param name="backgroundAfter">
        /// The background symbols which directly trail the value nodes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueNodesBuilder"/> and/or <paramref name="backgroundAfter"/> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="valueNodesBuilder"/> is an empty enumeration.
        /// </exception>
        public GreenJsonMultiValueSyntax(ArrayBuilder<GreenJsonValueWithBackgroundSyntax> valueNodesBuilder, GreenJsonBackgroundListSyntax backgroundAfter)
            : this(ReadOnlySpanList<GreenJsonValueWithBackgroundSyntax>.FromBuilder(valueNodesBuilder), backgroundAfter)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonMultiValueSyntax"/>.
        /// </summary>
        /// <param name="valueNodes">
        /// The non-empty enumeration of value nodes.
        /// </param>
        /// <param name="backgroundAfter">
        /// The background symbols which directly trail the value nodes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueNodes"/> and/or <paramref name="backgroundAfter"/> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="valueNodes"/> is an empty enumeration.
        /// </exception>
        public GreenJsonMultiValueSyntax(IEnumerable<GreenJsonValueWithBackgroundSyntax> valueNodes, GreenJsonBackgroundListSyntax backgroundAfter)
            : this(ReadOnlySpanList<GreenJsonValueWithBackgroundSyntax>.Create(valueNodes), backgroundAfter)
        {
        }

        private GreenJsonMultiValueSyntax(ReadOnlySpanList<GreenJsonValueWithBackgroundSyntax> valueNodes, GreenJsonBackgroundListSyntax backgroundAfter)
        {
            ValueNodes = valueNodes;

            if (ValueNodes.Count == 0)
            {
                throw new ArgumentException($"{nameof(valueNodes)} cannot be empty", nameof(valueNodes));
            }

            BackgroundAfter = backgroundAfter ?? throw new ArgumentNullException(nameof(backgroundAfter));
        }
    }
}
