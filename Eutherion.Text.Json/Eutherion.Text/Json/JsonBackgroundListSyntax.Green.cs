#region License
/*********************************************************************************
 * JsonBackgroundListSyntax.Green.cs
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

using System;
using System.Collections.Generic;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a node with background symbols in an abstract syntax tree.
    /// </summary>
    public sealed class GreenJsonBackgroundListSyntax : ISpan
    {
        /// <summary>
        /// Gets the empty <see cref="GreenJsonBackgroundListSyntax"/>.
        /// </summary>
        public static readonly GreenJsonBackgroundListSyntax Empty
#if NET5_0_OR_GREATER
            = new(ReadOnlySpanList<GreenJsonBackgroundSyntax>.Empty);
#else
            = new GreenJsonBackgroundListSyntax(ReadOnlySpanList<GreenJsonBackgroundSyntax>.Empty);
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonBackgroundListSyntax"/>.
        /// </summary>
        /// <param name="source">
        /// The source enumeration of <see cref="GreenJsonBackgroundSyntax"/>.
        /// </param>
        /// <returns>
        /// The new <see cref="GreenJsonBackgroundListSyntax"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static GreenJsonBackgroundListSyntax Create(IEnumerable<GreenJsonBackgroundSyntax> source)
        {
            var readOnlyBackground = ReadOnlySpanList<GreenJsonBackgroundSyntax>.Create(source);
            if (readOnlyBackground.Count == 0) return Empty;
            return new GreenJsonBackgroundListSyntax(readOnlyBackground);
        }

        /// <summary>
        /// Gets the read-only list with background nodes.
        /// </summary>
        public ReadOnlySpanList<GreenJsonBackgroundSyntax> BackgroundNodes { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public int Length => BackgroundNodes.Length;

        private GreenJsonBackgroundListSyntax(ReadOnlySpanList<GreenJsonBackgroundSyntax> backgroundNodes) => BackgroundNodes = backgroundNodes;
    }
}
