﻿#region License
/*********************************************************************************
 * JsonListSyntax.cs
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

using Eutherion.Collections;
using System;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains a list.
    /// </summary>
    public sealed class JsonListSyntax : JsonValueSyntax
    {
        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonListSyntax Green { get; }

        /// <summary>
        /// Gets the <see cref="JsonSquareBracketOpenSyntax"/> node at the start of this list syntax node.
        /// </summary>
        // Always create the [ and ], avoid overhead of SafeLazyObject.
        public JsonSquareBracketOpenSyntax SquareBracketOpen { get; }

        /// <summary>
        /// Gets the non-empty collection of list item nodes separated by comma characters.
        /// </summary>
        public SafeLazyObjectCollection<JsonMultiValueSyntax> ListItemNodes { get; }

        /// <summary>
        /// Gets the child comma syntax node collection.
        /// </summary>
        public SafeLazyObjectCollection<JsonCommaSyntax> Commas { get; }

        /// <summary>
        /// Gets the <see cref="JsonSquareBracketCloseSyntax"/> node at the end of this list syntax node, if it exists.
        /// </summary>
        // Always create the [ and ], avoid overhead of SafeLazyObject.
        public Maybe<JsonSquareBracketCloseSyntax> SquareBracketClose { get; }

        /// <summary>
        /// Returns ListItemNodes.Count, or one less if the last element is a <see cref="JsonMissingValueSyntax"/>.
        /// </summary>
        public int FilteredListItemNodeCount { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        /// <summary>
        /// Gets the number of children of this syntax node.
        /// </summary>
        public override int ChildCount => ListItemNodes.Count + Commas.Count + (Green.MissingSquareBracketClose ? 1 : 2);

        /// <summary>
        /// Initializes the child at the given <paramref name="index"/> and returns it.
        /// </summary>
        public override JsonSyntax GetChild(int index)
        {
            if (index == 0) return SquareBracketOpen;

            index--;
            int itemAndCommaCount = ListItemNodes.Count + Commas.Count;

            if (index < itemAndCommaCount)
            {
                if ((index & 1) == 0) return ListItemNodes[index >> 1];
                return Commas[index >> 1];
            }

            if (index == itemAndCommaCount && SquareBracketClose.IsJust(out var jsonSquareBracketClose))
            {
                return jsonSquareBracketClose;
            }

            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the start position of the child at the given <paramref name="index"/>, without initializing it.
        /// </summary>
        public override int GetChildStartPosition(int index)
        {
            if (index == 0) return 0;

            index--;
            int itemAndCommaCount = ListItemNodes.Count + Commas.Count;

            if (index < itemAndCommaCount)
            {
                return Green.ListItemNodes.GetElementOrSeparatorOffset(index) + JsonSpecialCharacter.SingleCharacterLength;
            }

            if (index == itemAndCommaCount && !Green.MissingSquareBracketClose)
            {
                return Length - JsonSpecialCharacter.SingleCharacterLength;
            }

            throw new IndexOutOfRangeException();
        }

        internal JsonListSyntax(JsonValueWithBackgroundSyntax parent, GreenJsonListSyntax green) : base(parent)
        {
            Green = green;

            SquareBracketOpen = new JsonSquareBracketOpenSyntax(this);

            int listItemNodeCount = green.ListItemNodes.Count;
            ListItemNodes = new SafeLazyObjectCollection<JsonMultiValueSyntax>(
                listItemNodeCount,
                index => new JsonMultiValueSyntax(this, index));

            Commas = new SafeLazyObjectCollection<JsonCommaSyntax>(
                listItemNodeCount - 1,
                index => new JsonCommaSyntax(this, index));

            SquareBracketClose = green.MissingSquareBracketClose
                               ? Maybe<JsonSquareBracketCloseSyntax>.Nothing
                               : new JsonSquareBracketCloseSyntax(this);

            FilteredListItemNodeCount = Green.FilteredListItemNodeCount;
        }

        internal override TResult Accept<T, TResult>(JsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitListSyntax(this, arg);
    }
}
