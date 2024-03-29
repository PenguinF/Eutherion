﻿#region License
/*********************************************************************************
 * JsonValueWithBackgroundSyntax.cs
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

using Eutherion.Threading;
using System;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a <see cref="JsonValueSyntax"/> node together with the background symbols directly before it in an abstract syntax tree.
    /// </summary>
    public sealed class JsonValueWithBackgroundSyntax : JsonSyntax
    {
        private class JsonValueSyntaxCreator : GreenJsonValueSyntaxVisitor<JsonValueWithBackgroundSyntax, JsonValueSyntax>
        {
            public static readonly JsonValueSyntaxCreator Instance
#if NET5_0_OR_GREATER
                = new();
#else
                = new JsonValueSyntaxCreator();
#endif

            private JsonValueSyntaxCreator() { }

            public override JsonValueSyntax VisitBooleanLiteralSyntax(GreenJsonBooleanLiteralSyntax green, JsonValueWithBackgroundSyntax parent)
                => green.Match<JsonValueSyntax>(
                    whenFalse: () => new JsonBooleanLiteralSyntax.False(parent),
                    whenTrue: () => new JsonBooleanLiteralSyntax.True(parent));

            public override JsonValueSyntax VisitErrorStringSyntax(GreenJsonErrorStringSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonErrorStringSyntax(parent, green);

            public override JsonValueSyntax VisitIntegerLiteralSyntax(GreenJsonIntegerLiteralSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonIntegerLiteralSyntax(parent, green);

            public override JsonValueSyntax VisitListSyntax(GreenJsonListSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonListSyntax(parent, green);

            public override JsonValueSyntax VisitMapSyntax(GreenJsonMapSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonMapSyntax(parent, green);

            public override JsonValueSyntax VisitMissingValueSyntax(GreenJsonMissingValueSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonMissingValueSyntax(parent);

            public override JsonValueSyntax VisitStringLiteralSyntax(GreenJsonStringLiteralSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonStringLiteralSyntax(parent, green);

            public override JsonValueSyntax VisitUndefinedValueSyntax(GreenJsonUndefinedValueSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonUndefinedValueSyntax(parent, green);

            public override JsonValueSyntax VisitUnknownSymbolSyntax(GreenJsonUnknownSymbolSyntax green, JsonValueWithBackgroundSyntax parent)
                => new JsonUnknownSymbolSyntax(parent, green);
        }

        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public JsonMultiValueSyntax Parent { get; }

        /// <summary>
        /// Gets the index of this background-value pair in the background-value pair collection of its parent.
        /// </summary>
        public int ParentValueNodeIndex { get; }

        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonValueWithBackgroundSyntax Green { get; }

        private readonly SafeLazyObject<JsonBackgroundListSyntax> backgroundBefore;
        private readonly SafeLazyObject<JsonValueSyntax> contentNode;

        /// <summary>
        /// Gets the background symbols which directly precede the content value node.
        /// </summary>
        public JsonBackgroundListSyntax BackgroundBefore => backgroundBefore.Object;

        /// <summary>
        /// Gets the content node containing the actual value.
        /// </summary>
        public JsonValueSyntax ContentNode => contentNode.Object;

        /// <summary>
        /// Gets the start position of this syntax node relative to its parent's start position.
        /// </summary>
        public override int Start => Parent.Green.ValueNodes.GetElementOffset(ParentValueNodeIndex);

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
        public override int ChildCount => 2;  // BackgroundBefore and ContentNode.

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
            if (index == 0) return BackgroundBefore;
            if (index == 1) return ContentNode;
            throw ExceptionUtility.ThrowListIndexOutOfRangeException();
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
            if (index == 1) return Green.BackgroundBefore.Length;
            throw ExceptionUtility.ThrowListIndexOutOfRangeException();
        }

        internal JsonValueWithBackgroundSyntax(JsonMultiValueSyntax parent, int parentValueNodeIndex)
        {
            Parent = parent;
            ParentValueNodeIndex = parentValueNodeIndex;
            Green = parent.Green.ValueNodes[parentValueNodeIndex];

            backgroundBefore = new SafeLazyObject<JsonBackgroundListSyntax>(() => new JsonBackgroundListSyntax(this));
            contentNode = new SafeLazyObject<JsonValueSyntax>(() => JsonValueSyntaxCreator.Instance.Visit(Green.ContentNode, this));
        }
    }
}
