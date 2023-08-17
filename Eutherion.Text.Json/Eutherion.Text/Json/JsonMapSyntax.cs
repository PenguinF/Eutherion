#region License
/*********************************************************************************
 * JsonMapSyntax.cs
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

using Eutherion.Collections;
using Eutherion.Threading;
using System;
using System.Collections.Generic;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains a map.
    /// </summary>
    public sealed class JsonMapSyntax : JsonValueSyntax
    {
        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonMapSyntax Green { get; }

        private readonly SafeLazyObject<JsonCurlyOpenSyntax> curlyOpen;

        /// <summary>
        /// Gets the <see cref="JsonCurlyOpenSyntax"/> node at the start of this map syntax node.
        /// </summary>
        public JsonCurlyOpenSyntax CurlyOpen => curlyOpen.Object;

        /// <summary>
        /// Gets the non-empty collection of key-value nodes separated by comma characters.
        /// </summary>
        public SafeLazyObjectCollection<JsonKeyValueSyntax> KeyValueNodes { get; }

        /// <summary>
        /// Gets the child comma syntax node collection.
        /// </summary>
        public SafeLazyObjectCollection<JsonCommaSyntax> Commas { get; }

        private readonly SafeLazyObject<Maybe<JsonCurlyCloseSyntax>> curlyClose;

        /// <summary>
        /// Gets the <see cref="JsonCurlyCloseSyntax"/> node at the end of this map syntax node, if it exists.
        /// </summary>
        public Maybe<JsonCurlyCloseSyntax> CurlyClose => curlyClose.Object;

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        /// <summary>
        /// Returns <see langword="true"/> because this sytnax node represents a potentially valid JSON value.
        /// </summary>
        public override bool IsValidValue => true;

        /// <summary>
        /// Gets the number of children of this syntax node.
        /// </summary>
        public override int ChildCount => KeyValueNodes.Count + Commas.Count + (Green.MissingCurlyClose ? 1 : 2);

        /// <summary>
        /// Initializes the child at the given <paramref name="index"/> and returns it.
        /// </summary>
        /// <param name="index">
        /// The index of the child node to return.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="ChildCount"/>.
        /// </exception>
        public override JsonSyntax GetChild(int index)
        {
            if (index == 0) return CurlyOpen;

            index--;
            int keyValueAndCommaCount = KeyValueNodes.Count + Commas.Count;

            if (index < keyValueAndCommaCount)
            {
                if ((index & 1) == 0) return KeyValueNodes[index >> 1];
                return Commas[index >> 1];
            }

            if (index == keyValueAndCommaCount && CurlyClose.IsJust(out var jsonCurlyClose))
            {
                return jsonCurlyClose;
            }

            throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the start position of the child at the given <paramref name="index"/>, without initializing it.
        /// </summary>
        /// <param name="index">
        /// The index of the child node for which to return the start position.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="ChildCount"/>.
        /// </exception>
        public override int GetChildStartPosition(int index)
        {
            if (index == 0) return 0;

            index--;
            int keyValueAndCommaCount = KeyValueNodes.Count + Commas.Count;

            if (index < keyValueAndCommaCount)
            {
                return Green.KeyValueNodes.GetElementOrSeparatorOffset(index) + JsonSpecialCharacter.SingleCharacterLength;
            }

            if (index == keyValueAndCommaCount && !Green.MissingCurlyClose)
            {
                return Length - JsonSpecialCharacter.SingleCharacterLength;
            }

            throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        /// <summary>
        /// Enumerates all semantically defined key-value pairs of this <see cref="JsonMapSyntax"/>.
        /// These are all key-value pairs where its key is a string literal, and its value is available.
        /// The returned value node may still contain errors or be a <see cref="JsonMissingValueSyntax"/>.
        /// </summary>
        public IEnumerable<(JsonStringLiteralSyntax keyNode, JsonValueSyntax valueNode)> DefinedKeyValuePairs()
        {
            foreach (var keyValueNode in KeyValueNodes)
            {
                if (keyValueNode.ValidKey.IsJust(out var stringLiteral) && keyValueNode.FirstValueNode.IsJust(out var valueNode))
                {
                    yield return (stringLiteral, valueNode);
                }
            }
        }

        internal JsonMapSyntax(JsonValueWithBackgroundSyntax parent, GreenJsonMapSyntax green) : base(parent)
        {
            Green = green;

            curlyOpen = new SafeLazyObject<JsonCurlyOpenSyntax>(() => new JsonCurlyOpenSyntax(this));

            int keyValueNodeCount = green.KeyValueNodes.Count;
            KeyValueNodes = new SafeLazyObjectCollection<JsonKeyValueSyntax>(
                keyValueNodeCount,
                index => new JsonKeyValueSyntax(this, index));

            Commas = new SafeLazyObjectCollection<JsonCommaSyntax>(
                keyValueNodeCount - 1,
                index => new JsonCommaSyntax(this, index));

            curlyClose = new SafeLazyObject<Maybe<JsonCurlyCloseSyntax>>(
                () => green.MissingCurlyClose
                ? Maybe<JsonCurlyCloseSyntax>.Nothing
                : new JsonCurlyCloseSyntax(this));
        }

        internal override TResult Accept<T, TResult>(JsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitMapSyntax(this, arg);
    }
}
