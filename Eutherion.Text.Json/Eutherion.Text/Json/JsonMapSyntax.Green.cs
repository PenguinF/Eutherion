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
        {
            KeyValueNodes = ReadOnlySeparatedSpanList<GreenJsonKeyValueSyntax, GreenJsonCommaSyntax>.Create(keyValueNodes, GreenJsonCommaSyntax.Value);

            if (KeyValueNodes.Count == 0)
            {
                throw new ArgumentException($"{nameof(keyValueNodes)} cannot be empty", nameof(keyValueNodes));
            }

            MissingCurlyClose = missingCurlyClose;

            Length = JsonSpecialCharacter.SingleCharacterLength
                   + KeyValueNodes.Length
                   + (missingCurlyClose ? 0 : JsonSpecialCharacter.SingleCharacterLength);
        }

        /// <summary>
        /// Enumerates all semantically valid key-value pairs together with their starting positions relative to the start position of this <see cref="GreenJsonMapSyntax"/>.
        /// </summary>
        public IEnumerable<(int keyNodeStartPosition, GreenJsonStringLiteralSyntax keyNode, int valueNodeStartPosition, GreenJsonValueSyntax valueNode)> ValidKeyValuePairs()
        {
            for (int i = 0; i < KeyValueNodes.Count; i++)
            {
                var keyValueNode = KeyValueNodes[i];

                if (keyValueNode.ValidKey.IsJust(out var stringLiteral)
                    && keyValueNode.FirstValueNode.IsJust(out var multiValueNode)
                    && !(multiValueNode.ValueNode.ContentNode is GreenJsonMissingValueSyntax))
                {
                    // Only the first value can be valid, even if it's undefined.
                    int keyNodeStart = GetKeyValueNodeStart(i) + keyValueNode.KeyNode.ValueNode.BackgroundBefore.Length;
                    int valueNodeStart = GetKeyValueNodeStart(i) + keyValueNode.GetFirstValueNodeStart() + multiValueNode.ValueNode.BackgroundBefore.Length;

                    yield return (keyNodeStart, stringLiteral, valueNodeStart, multiValueNode.ValueNode.ContentNode);
                }
            }
        }

        /// <summary>
        /// Gets the start position of a key-value node relative to the start position of this <see cref="GreenJsonMapSyntax"/>.
        /// </summary>
        public int GetKeyValueNodeStart(int index) => JsonSpecialCharacter.SingleCharacterLength + KeyValueNodes.GetElementOffset(index);

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitMapSyntax(this, arg);
    }
}
