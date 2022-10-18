#region License
/*********************************************************************************
 * JsonIntegerLiteralSyntax.cs
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

using System.Numerics;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents an integer literal value syntax node.
    /// </summary>
    public sealed class JsonIntegerLiteralSyntax : JsonValueSyntax, IJsonSymbol
    {
        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonIntegerLiteralSyntax Green { get; }

        /// <summary>
        /// Gets the integer value represented by this literal syntax.
        /// </summary>
        public BigInteger Value => Green.Value;

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        internal JsonIntegerLiteralSyntax(JsonValueWithBackgroundSyntax parent, GreenJsonIntegerLiteralSyntax green) : base(parent) => Green = green;

        internal override TResult Accept<T, TResult>(JsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitIntegerLiteralSyntax(this, arg);
        TResult IJsonSymbol.Accept<T, TResult>(JsonSymbolVisitor<T, TResult> visitor, T arg) => visitor.VisitIntegerLiteralSyntax(this, arg);
    }
}
