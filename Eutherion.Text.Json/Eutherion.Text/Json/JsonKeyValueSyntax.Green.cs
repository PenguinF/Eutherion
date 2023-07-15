#region License
/*********************************************************************************
 * JsonKeyValueSyntax.Green.cs
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

using System;
using System.Collections.Generic;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a single key-value pair in a <see cref="GreenJsonMapSyntax"/>.
    /// </summary>
    public sealed class GreenJsonKeyValueSyntax : ISpan
    {
        /// <summary>
        /// Gets the syntax node containing the key of this <see cref="GreenJsonKeyValueSyntax"/>.
        /// </summary>
        public GreenJsonMultiValueSyntax KeyNode => ValueSectionNodes[0];

        /// <summary>
        /// If <see cref="KeyNode"/> contains a valid key, returns it.
        /// </summary>
        public Maybe<GreenJsonStringLiteralSyntax> ValidKey { get; }

        /// <summary>
        /// Returns the first value node containing the value of this <see cref="GreenJsonKeyValueSyntax"/>, if it was provided.
        /// </summary>
        public Maybe<GreenJsonMultiValueSyntax> FirstValueNode => ValueSectionNodes.Count > 1 ? ValueSectionNodes[1] : Maybe<GreenJsonMultiValueSyntax>.Nothing;

        /// <summary>
        /// Gets the list of value section nodes separated by colon characters.
        /// </summary>
        public ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonColonSyntax> ValueSectionNodes { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax.
        /// </summary>
        public int Length => ValueSectionNodes.Length;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonKeyValueSyntax"/>.
        /// </summary>
        /// <param name="validKey">
        /// Nothing if no valid key was found, just the valid key otherwise.
        /// The string literal is expected to be the same as the first value node's content node in <paramref name="valueSectionNodes"/>.
        /// </param>
        /// <param name="valueSectionNodes">
        /// The enumeration of syntax nodes containing the key and values.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validKey"/> and/or <paramref name="valueSectionNodes"/> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="validKey"/> is not the expected syntax node -or- <paramref name="valueSectionNodes"/> is an empty enumeration.
        /// </exception>
        public GreenJsonKeyValueSyntax(Maybe<GreenJsonStringLiteralSyntax> validKey, IEnumerable<GreenJsonMultiValueSyntax> valueSectionNodes)
        {
            ValidKey = validKey ?? throw new ArgumentNullException(nameof(validKey));
            ValueSectionNodes = ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonColonSyntax>.Create(valueSectionNodes, GreenJsonColonSyntax.Value);

            if (ValueSectionNodes.Count == 0)
            {
                throw new ArgumentException($"{nameof(valueSectionNodes)} cannot be empty", nameof(valueSectionNodes));
            }

            // If a valid key node is given, the node must always be equal to keyNode.ValueNode.Node.
            if (validKey.IsJust(out var validKeyNode)
                && validKeyNode != ValueSectionNodes[0].ValueNode.ContentNode)
            {
                throw new ArgumentException("validKey and ValueSectionNodes[0].ValueNode.ContentNode should be the same", nameof(validKey));
            }
        }

        /// <summary>
        /// Gets the start position of <see cref="FirstValueNode"/> relative to the start position of this <see cref="GreenJsonKeyValueSyntax"/>.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// There is no first value node, i.e. this <see cref="GreenJsonKeyValueSyntax"/> has a key only.
        /// </exception>
        public int GetFirstValueNodeStart() => ValueSectionNodes.GetElementOffset(1);
    }
}
