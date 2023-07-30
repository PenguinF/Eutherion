#region License
/*********************************************************************************
 * JsonListSyntax.Green.cs
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
    /// Represents a syntax node which contains a list.
    /// </summary>
    public sealed class GreenJsonListSyntax : GreenJsonValueSyntax
    {
        /// <summary>
        /// Gets the non-empty list of list item nodes.
        /// </summary>
        public ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonCommaSyntax> ListItemNodes { get; }

        /// <summary>
        /// Gets if the list is not terminated by a closing square bracket.
        /// </summary>
        public bool MissingSquareBracketClose { get; }

        /// <summary>
        /// Returns ListItemNodes.Count, or one less if the last element is a <see cref="GreenJsonMissingValueSyntax"/>.
        /// </summary>
        public int FilteredListItemNodeCount
        {
            get
            {
                int count = ListItemNodes.Count;

                // Discard last item if it's a missing value, so that a trailing comma is ignored.
                if (ListItemNodes[count - 1].ValueNode.ContentNode is GreenJsonMissingValueSyntax)
                {
                    return count - 1;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonListSyntax"/> from an <see cref="ArrayBuilder{T}"/>.
        /// This empties the array builder.
        /// </summary>
        /// <param name="listItemNodesBuilder">
        /// The non-empty builder with list item nodes.
        /// </param>
        /// <param name="missingSquareBracketClose">
        /// False if the list is terminated by a closing square bracket, otherwise true.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="listItemNodesBuilder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="listItemNodesBuilder"/> is an empty enumeration.
        /// </exception>
        public GreenJsonListSyntax(ArrayBuilder<GreenJsonMultiValueSyntax> listItemNodesBuilder, bool missingSquareBracketClose)
            : this(ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonCommaSyntax>.FromBuilder(listItemNodesBuilder, GreenJsonCommaSyntax.Value), missingSquareBracketClose)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonListSyntax"/>.
        /// </summary>
        /// <param name="listItemNodes">
        /// The non-empty enumeration of list item nodes.
        /// </param>
        /// <param name="missingSquareBracketClose">
        /// False if the list is terminated by a closing square bracket, otherwise true.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="listItemNodes"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="listItemNodes"/> is an empty enumeration.
        /// </exception>
        public GreenJsonListSyntax(IEnumerable<GreenJsonMultiValueSyntax> listItemNodes, bool missingSquareBracketClose)
            : this(ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonCommaSyntax>.Create(listItemNodes, GreenJsonCommaSyntax.Value), missingSquareBracketClose)
        {
        }

        internal GreenJsonListSyntax(ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonCommaSyntax> listItemNodes, bool missingSquareBracketClose)
        {
            ListItemNodes = listItemNodes;

            if (ListItemNodes.Count == 0)
            {
                throw new ArgumentException($"{nameof(listItemNodes)} cannot be empty", nameof(listItemNodes));
            }

            MissingSquareBracketClose = missingSquareBracketClose;

            Length = JsonSpecialCharacter.SingleCharacterLength
                   + ListItemNodes.Length
                   + (missingSquareBracketClose ? 0 : JsonSpecialCharacter.SingleCharacterLength);
        }

        /// <summary>
        /// Gets the start position of an element node relative to the start position of this <see cref="GreenJsonListSyntax"/>.
        /// </summary>
        public int GetElementNodeStart(int index) => JsonSpecialCharacter.SingleCharacterLength + ListItemNodes.GetElementOffset(index);

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitListSyntax(this, arg);
    }
}
