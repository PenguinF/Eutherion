#region License
/*********************************************************************************
 * JsonKeyValueSyntax.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a single key-value pair in a <see cref="JsonMapSyntax"/>.
    /// </summary>
    public sealed class JsonKeyValueSyntax : JsonSyntax
    {
        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public JsonMapSyntax Parent { get; }

        /// <summary>
        /// Gets the index of this key-value pair in the key-value pair collection of its parent.
        /// </summary>
        public int ParentKeyValueNodeIndex { get; }

        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonKeyValueSyntax Green { get; }

        /// <summary>
        /// Gets the value section node collection separated by colon characters.
        /// </summary>
        public SafeLazyObjectCollection<JsonMultiValueSyntax> ValueSectionNodes { get; }

        /// <summary>
        /// Gets the child colon syntax node collection.
        /// </summary>
        public SafeLazyObjectCollection<JsonColonSyntax> Colons { get; }

        /// <summary>
        /// Gets the start position of this syntax node relative to its parent's start position.
        /// </summary>
        public override int Start => JsonSpecialCharacter.SingleCharacterLength + Parent.Green.KeyValueNodes.GetElementOffset(ParentKeyValueNodeIndex);

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public override JsonSyntax ParentSyntax => Parent;

        /// <summary>
        /// Gets the number of children of this syntax node.
        /// </summary>
        public override int ChildCount => ValueSectionNodes.Count + Colons.Count;

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
            // '>>' has the happy property that (-1) >> 1 evaluates to -1, which correctly throws an ArgumentOutOfRangeException.
            if ((index & 1) == 0) return ValueSectionNodes[index >> 1];
            return Colons[index >> 1];
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
        public override int GetChildStartPosition(int index) => Green.ValueSectionNodes.GetElementOrSeparatorOffset(index);

        /// <summary>
        /// Gets the syntax node containing the key of this <see cref="JsonKeyValueSyntax"/>.
        /// </summary>
        public JsonValueSyntax KeyNode => ValueSectionNodes[0].ValueNode;

        private readonly SafeLazy<Maybe<JsonStringLiteralSyntax>> LazyValidKey;

        /// <summary>
        /// If <see cref="KeyNode"/> contains a valid key, returns it.
        /// </summary>
        public Maybe<JsonStringLiteralSyntax> ValidKey => LazyValidKey.Value;

        private readonly SafeLazy<Maybe<JsonValueSyntax>> LazyFirstValueNode;

        /// <summary>
        /// Returns the first value node containing the value of this <see cref="JsonKeyValueSyntax"/>, if it was provided.
        /// </summary>
        /// <remarks>
        /// If this value node is a <see cref="JsonMissingValueSyntax"/>, <see cref="Maybe{T}.Nothing"/> is returned, e.g. for key &quot;x&quot;
        /// in json '{ &quot;x&quot; : , &quot;y&quot;: 0 }';
        /// </remarks>
        public Maybe<JsonValueSyntax> FirstValueNode => LazyFirstValueNode.Value;

        internal JsonKeyValueSyntax(JsonMapSyntax parent, int parentKeyValueNodeIndex)
        {
            Parent = parent;
            ParentKeyValueNodeIndex = parentKeyValueNodeIndex;
            Green = parent.Green.KeyValueNodes[parentKeyValueNodeIndex];

            int valueSectionNodeCount = Green.ValueSectionNodes.Count;

            ValueSectionNodes = new SafeLazyObjectCollection<JsonMultiValueSyntax>(
                valueSectionNodeCount,
                index => new JsonMultiValueSyntax(this, index));

            Colons = new SafeLazyObjectCollection<JsonColonSyntax>(
                valueSectionNodeCount - 1,
                index => new JsonColonSyntax(this, index));

            // Create SafeLazy instances so child nodes only get initialized when evaluated.
            LazyValidKey = new SafeLazy<Maybe<JsonStringLiteralSyntax>>(
                () => KeyNode is JsonStringLiteralSyntax stringLiteral ? stringLiteral : Maybe<JsonStringLiteralSyntax>.Nothing);

            LazyFirstValueNode = new SafeLazy<Maybe<JsonValueSyntax>>(
                () =>
                {
                    if (ValueSectionNodes.Count > 1)
                    {
                        JsonValueSyntax firstValueNode = ValueSectionNodes[1].ValueNode;
                        if (!(firstValueNode is JsonMissingValueSyntax))
                        {
                            return firstValueNode;
                        }
                    }

                    return Maybe<JsonValueSyntax>.Nothing;
                });
        }
    }
}
