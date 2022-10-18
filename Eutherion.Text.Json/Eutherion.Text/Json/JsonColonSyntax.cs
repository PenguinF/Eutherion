﻿#region License
/*********************************************************************************
 * JsonColonSyntax.cs
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

using System.Diagnostics.CodeAnalysis;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a colon syntax node.
    /// </summary>
    public sealed class JsonColonSyntax : JsonSyntax, IJsonSymbol
    {
        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public JsonKeyValueSyntax Parent { get; }

        /// <summary>
        /// Gets the index of this colon in the colon collection of its parent.
        /// </summary>
        public int ColonIndex { get; }

        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
#if NET5_0_OR_GREATER
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This for a library-wide convention that every syntax node has its corresponding Green property.")]
#endif
        public GreenJsonColonSyntax Green => GreenJsonColonSyntax.Value;

        /// <summary>
        /// Gets the start position of this syntax node relative to its parent's start position.
        /// </summary>
        public override int Start => Parent.Green.ValueSectionNodes.GetSeparatorOffset(ColonIndex);

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => JsonSpecialCharacter.SingleCharacterLength;

        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public override JsonSyntax ParentSyntax => Parent;

        internal JsonColonSyntax(JsonKeyValueSyntax parent, int colonIndex)
        {
            Parent = parent;
            ColonIndex = colonIndex;
        }

        TResult IJsonSymbol.Accept<T, TResult>(JsonSymbolVisitor<T, TResult> visitor, T arg) => visitor.VisitColonSyntax(this, arg);
    }
}
