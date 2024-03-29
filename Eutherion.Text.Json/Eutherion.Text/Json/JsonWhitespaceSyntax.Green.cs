﻿#region License
/*********************************************************************************
 * JsonWhitespaceSyntax.Green.cs
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
    /// Represents a syntax node which contains whitespace.
    /// </summary>
    public sealed class GreenJsonWhitespaceSyntax : GreenJsonBackgroundSyntax, IGreenJsonSymbol
    {
        /// <summary>
        /// Maximum length before new <see cref="GreenJsonWhitespaceSyntax"/> instances are always newly allocated.
        /// </summary>
        public const int SharedInstanceLength = 255;

        private static readonly GreenJsonWhitespaceSyntax[] SharedInstances;

        static GreenJsonWhitespaceSyntax()
        {
            SharedInstances = new GreenJsonWhitespaceSyntax[SharedInstanceLength];
            SharedInstances.Fill(i => new GreenJsonWhitespaceSyntax(i));
        }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length { get; }

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public JsonSymbolType SymbolType => JsonSymbolType.Whitespace;

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonWhitespaceSyntax"/> with a specified length.
        /// </summary>
        /// <param name="length">
        /// The length of the text span corresponding with the node to create.
        /// </param>
        /// <returns>
        /// The new <see cref="GreenJsonWhitespaceSyntax"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is 0 or lower.
        /// </exception>
        public static GreenJsonWhitespaceSyntax Create(int length)
            => length <= 0 ? throw new ArgumentOutOfRangeException(nameof(length))
            : length < SharedInstanceLength ? SharedInstances[length]
            : new GreenJsonWhitespaceSyntax(length);

        private GreenJsonWhitespaceSyntax(int length) => Length = length;

        internal override TResult Accept<T, TResult>(GreenJsonBackgroundSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitWhitespaceSyntax(this, arg);
    }
}
