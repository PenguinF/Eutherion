#region License
/*********************************************************************************
 * JsonStringLiteralSyntax.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a string literal value syntax node.
    /// </summary>
    public sealed class JsonStringLiteralSyntax : JsonValueSyntax, IJsonSymbol
    {
        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonStringLiteralSyntax Green { get; }

        /// <summary>
        /// Gets the string value represented by this literal syntax.
        /// </summary>
        public string Value => Green.Value;

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        /// <summary>
        /// Returns <see langword="true"/> because this sytnax node represents a valid JSON value.
        /// </summary>
        public override bool IsValidValue => true;

        internal JsonStringLiteralSyntax(JsonValueWithBackgroundSyntax parent, GreenJsonStringLiteralSyntax green) : base(parent) => Green = green;

        internal override TResult Accept<T, TResult>(JsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitStringLiteralSyntax(this, arg);
        TResult IJsonSymbol.Accept<T, TResult>(JsonSymbolVisitor<T, TResult> visitor, T arg) => visitor.VisitStringLiteralSyntax(this, arg);
    }
}
