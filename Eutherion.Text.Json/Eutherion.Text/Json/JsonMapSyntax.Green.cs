#region License
/*********************************************************************************
 * JsonMapSyntax.Green.cs
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
    /// Represents a syntax node which contains a map.
    /// </summary>
    public sealed class GreenJsonMapSyntax : GreenJsonValueSyntax
    {
        /// <summary>
        /// Gets the non-empty list of key-value nodes.
        /// </summary>
        public ReadOnlySeparatedSpanList<GreenJsonKeyValueSyntax, GreenJsonCommaSyntax> KeyValueNodes { get; }

        /// <summary>
        /// Gets if the map is not terminated by a closing curly brace.
        /// </summary>
        public bool MissingCurlyClose { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonMapSyntax"/> from an <see cref="ArrayBuilder{T}"/>.
        /// This empties the array builder.
        /// </summary>
        /// <param name="keyValueNodesBuilder">
        /// The non-empty builder with key-value nodes.
        /// </param>
        /// <param name="missingCurlyClose">
        /// False if the list is terminated by a closing curly brace, otherwise true.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyValueNodesBuilder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="keyValueNodesBuilder"/> is an empty enumeration.
        /// </exception>
        public GreenJsonMapSyntax(ArrayBuilder<GreenJsonKeyValueSyntax> keyValueNodesBuilder, bool missingCurlyClose)
            : this(ReadOnlySeparatedSpanList<GreenJsonKeyValueSyntax, GreenJsonCommaSyntax>.FromBuilder(keyValueNodesBuilder, GreenJsonCommaSyntax.Value), missingCurlyClose)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonMapSyntax"/>.
        /// </summary>
        /// <param name="keyValueNodes">
        /// The non-empty enumeration of key-value nodes.
        /// </param>
        /// <param name="missingCurlyClose">
        /// False if the list is terminated by a closing curly brace, otherwise true.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyValueNodes"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="keyValueNodes"/> is an empty enumeration.
        /// </exception>
        public GreenJsonMapSyntax(IEnumerable<GreenJsonKeyValueSyntax> keyValueNodes, bool missingCurlyClose)
            : this(ReadOnlySeparatedSpanList<GreenJsonKeyValueSyntax, GreenJsonCommaSyntax>.Create(keyValueNodes, GreenJsonCommaSyntax.Value), missingCurlyClose)
        {
        }

        private GreenJsonMapSyntax(ReadOnlySeparatedSpanList<GreenJsonKeyValueSyntax, GreenJsonCommaSyntax> keyValueNodes, bool missingCurlyClose)
        {
            KeyValueNodes = keyValueNodes;

            if (KeyValueNodes.Count == 0)
            {
                throw new ArgumentException($"{nameof(keyValueNodes)} cannot be empty", nameof(keyValueNodes));
            }

            MissingCurlyClose = missingCurlyClose;

            Length = JsonSpecialCharacter.SingleCharacterLength
                   + KeyValueNodes.Length
                   + (missingCurlyClose ? 0 : JsonSpecialCharacter.SingleCharacterLength);
        }

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitMapSyntax(this, arg);
    }
}
