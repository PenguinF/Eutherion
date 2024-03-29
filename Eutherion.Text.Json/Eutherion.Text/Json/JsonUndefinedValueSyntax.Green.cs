﻿#region License
/*********************************************************************************
 * JsonUndefinedValueSyntax.Green.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node with an undefined or unsupported value.
    /// </summary>
    public sealed class GreenJsonUndefinedValueSyntax : GreenJsonValueSyntax, IGreenJsonSymbol
    {
        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length { get; }

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public JsonSymbolType SymbolType => JsonSymbolType.UndefinedValue;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonUndefinedValueSyntax"/>.
        /// </summary>
        /// <param name="length">
        /// The length of the text span corresponding with this syntax node.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is 0 or lower.
        /// </exception>
        public GreenJsonUndefinedValueSyntax(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            Length = length;
        }

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitUndefinedValueSyntax(this, arg);
    }
}
