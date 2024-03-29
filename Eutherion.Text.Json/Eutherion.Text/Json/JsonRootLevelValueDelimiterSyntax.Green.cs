﻿#region License
/*********************************************************************************
 * JsonRootLevelValueDelimiterSyntax.Green.cs
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
using System.Diagnostics;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains a value delimiter symbol at the root level.
    /// Example json "}" generates this syntax node.
    /// </summary>
    public sealed class GreenJsonRootLevelValueDelimiterSyntax : GreenJsonBackgroundSyntax
    {
        /// <summary>
        /// Gets the value delimiter symbol.
        /// </summary>
        public IGreenJsonSymbol ValueDelimiter { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => ValueDelimiter.Length;

        internal GreenJsonRootLevelValueDelimiterSyntax(IGreenJsonSymbol valueDelimiter)
        {
            ValueDelimiter = valueDelimiter;
            Debug.Assert(ValueDelimiter.SymbolType >= JsonParser.ValueDelimiterThreshold);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonRootLevelValueDelimiterSyntax"/>.
        /// </summary>
        /// <param name="valueDelimiter">
        /// The value delimiter symbol.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueDelimiter"/> is <see langword="null"/>.
        /// </exception>
        public GreenJsonRootLevelValueDelimiterSyntax(GreenJsonCurlyCloseSyntax valueDelimiter)
            => ValueDelimiter = valueDelimiter ?? throw new ArgumentNullException(nameof(valueDelimiter));

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonRootLevelValueDelimiterSyntax"/>.
        /// </summary>
        /// <param name="valueDelimiter">
        /// The value delimiter symbol.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueDelimiter"/> is <see langword="null"/>.
        /// </exception>
        public GreenJsonRootLevelValueDelimiterSyntax(GreenJsonSquareBracketCloseSyntax valueDelimiter)
            => ValueDelimiter = valueDelimiter ?? throw new ArgumentNullException(nameof(valueDelimiter));

        internal override TResult Accept<T, TResult>(GreenJsonBackgroundSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitRootLevelValueDelimiterSyntax(this, arg);
    }
}
