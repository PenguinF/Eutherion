﻿#region License
/*********************************************************************************
 * JsonStringLiteralSyntax.Green.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a string literal value syntax node.
    /// </summary>
    public sealed class GreenJsonStringLiteralSyntax : GreenJsonValueSyntax, IGreenJsonSymbol
    {
        /// <summary>
        /// Gets the string value represented by this literal syntax.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length { get; }

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public JsonSymbolType SymbolType => JsonSymbolType.StringLiteral;

        /// <summary>
        /// Returns <see langword="true"/> because this is a <see cref="GreenJsonStringLiteralSyntax"/> instance.
        /// </summary>
        public override bool IsStringLiteral => true;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonStringLiteralSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The string value to be represented by this literal syntax.
        /// </param>
        /// <param name="length">
        /// The length of the text span corresponding with this syntax node.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is 0 or lower.
        /// </exception>
        public GreenJsonStringLiteralSyntax(string value, int length)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            Length = length;
        }

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitStringLiteralSyntax(this, arg);
    }
}
