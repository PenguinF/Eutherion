#region License
/*********************************************************************************
 * JsonMultiValueSyntax.cs
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
using Eutherion.Threading;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains one or more value nodes together with all background syntax that precedes and follows it.
    /// The root node of any abstract syntax tree is of this type.
    /// </summary>
    public sealed class JsonMultiValueSyntax : JsonSyntax
    {
        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public Union<_void, JsonListSyntax, JsonKeyValueSyntax> Parent { get; }

        /// <summary>
        /// Gets the index of this syntax node in its parent's collection, or 0 if this syntax node is the root node.
        /// </summary>
        public int ParentIndex { get; }

        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonMultiValueSyntax Green { get; }

        /// <summary>
        /// Gets the non-empty collection of value nodes.
        /// </summary>
        public SafeLazyObjectCollection<JsonValueWithBackgroundSyntax> ValueNodes { get; }

        private readonly SafeLazyObject<JsonBackgroundListSyntax> backgroundAfter;

        /// <summary>
        /// Gets the background symbols which directly trail the value nodes.
        /// </summary>
        public JsonBackgroundListSyntax BackgroundAfter => backgroundAfter.Object;

        /// <summary>
        /// Gets the syntax node containing the first value.
        /// Is <see cref="JsonMissingValueSyntax"/> if a value was expected but none given. (E.g. in "[0,,2]", between both commas.)
        /// </summary>
        public JsonValueWithBackgroundSyntax ValueNode => ValueNodes[0];

        /// <summary>
        /// Gets the start position of this syntax node relative to its parent's start position, or 0 if this syntax node is the root node.
        /// </summary>
        public override int Start => Parent.Match(
            whenOption1: _ => 0,
            whenOption2: listSyntax => JsonSpecialCharacter.SingleCharacterLength + listSyntax.Green.ListItemNodes.GetElementOffset(ParentIndex),
            whenOption3: keyValueSyntax => keyValueSyntax.Green.ValueSectionNodes.GetElementOffset(ParentIndex));

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        /// <summary>
        /// Gets the parent syntax node of this instance. Returns null for the root node.
        /// </summary>
        [MaybeNull]
        public override JsonSyntax ParentSyntax => Parent.Match<JsonSyntax>(
            // Override Match() constraint, since a null JsonSyntax can be returned.
#if NET472
            whenOption1: _ => null,
#else
            whenOption1: _ => null!,
#endif
            whenOption2: x => x,
            whenOption3: x => x);

        /// <summary>
        /// Gets the absolute start position of this syntax node.
        /// </summary>
        public override int AbsoluteStart => Parent.IsOption1() ? 0 : base.AbsoluteStart;

        /// <summary>
        /// Gets the number of children of this syntax node.
        /// </summary>
        public override int ChildCount => ValueNodes.Count + 1;  // Extra 1 for BackgroundAfter.

        /// <summary>
        /// Initializes the child at the given <paramref name="index"/> and returns it.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="ChildCount"/>.
        /// </exception>
        public override JsonSyntax GetChild(int index)
        {
            if (index < ValueNodes.Count) return ValueNodes[index];
            if (index == ValueNodes.Count) return BackgroundAfter;
            throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the start position of the child at the given <paramref name="index"/>, without initializing it.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="ChildCount"/>.
        /// </exception>
        public override int GetChildStartPosition(int index)
        {
            if (index < ValueNodes.Count) return Green.ValueNodes.GetElementOffset(index);
            if (index == ValueNodes.Count) return Length - Green.BackgroundAfter.Length;
            throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        private JsonMultiValueSyntax(Union<_void, JsonListSyntax, JsonKeyValueSyntax> parent, GreenJsonMultiValueSyntax green)
        {
            Parent = parent;
            Green = green;

            ValueNodes = new SafeLazyObjectCollection<JsonValueWithBackgroundSyntax>(
                green.ValueNodes.Count,
                index => new JsonValueWithBackgroundSyntax(this, index));

            backgroundAfter = new SafeLazyObject<JsonBackgroundListSyntax>(() => new JsonBackgroundListSyntax(this));
        }

        // For root nodes.
        internal JsonMultiValueSyntax(GreenJsonMultiValueSyntax green)
            : this(_void._, green)
        {
            // Do not assign ParentIndex, its value is meaningless in this case.
        }

        internal JsonMultiValueSyntax(JsonListSyntax parent, int parentIndex)
            : this(parent, parent.Green.ListItemNodes[parentIndex])
        {
            ParentIndex = parentIndex;
        }

        internal JsonMultiValueSyntax(JsonKeyValueSyntax parent, int parentIndex)
            : this(parent, parent.Green.ValueSectionNodes[parentIndex])
        {
            ParentIndex = parentIndex;
        }
    }
}
