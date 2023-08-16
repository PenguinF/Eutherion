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
        /// Gets the list of value section nodes separated by colon characters.
        /// </summary>
        public ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonColonSyntax> ValueSectionNodes { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax.
        /// </summary>
        public int Length => ValueSectionNodes.Length;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonKeyValueSyntax"/> from an <see cref="ArrayBuilder{T}"/>.
        /// This empties the array builder.
        /// </summary>
        /// <param name="valueSectionNodesBuilder">
        /// The builder with syntax nodes containing the key and values.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueSectionNodesBuilder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="valueSectionNodesBuilder"/> is empty.
        /// </exception>
        public GreenJsonKeyValueSyntax(ArrayBuilder<GreenJsonMultiValueSyntax> valueSectionNodesBuilder)
            : this(ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonColonSyntax>.FromBuilder(valueSectionNodesBuilder, GreenJsonColonSyntax.Value))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonKeyValueSyntax"/>.
        /// </summary>
        /// <param name="valueSectionNodes">
        /// The enumeration of syntax nodes containing the key and values.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueSectionNodes"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="valueSectionNodes"/> is an empty enumeration.
        /// </exception>
        public GreenJsonKeyValueSyntax(IEnumerable<GreenJsonMultiValueSyntax> valueSectionNodes)
            : this(ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonColonSyntax>.Create(valueSectionNodes, GreenJsonColonSyntax.Value))
        {
        }

        private GreenJsonKeyValueSyntax(ReadOnlySeparatedSpanList<GreenJsonMultiValueSyntax, GreenJsonColonSyntax> valueSectionNodes)
        {
            ValueSectionNodes = valueSectionNodes;

            if (ValueSectionNodes.Count == 0)
            {
                throw new ArgumentException($"{nameof(valueSectionNodes)} cannot be empty", nameof(valueSectionNodes));
            }
        }
    }
}
