﻿#region License
/*********************************************************************************
 * JsonUnterminatedMultiLineCommentSyntax.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a syntax node which contains an unterminated multi-line comment.
    /// </summary>
    public sealed class JsonUnterminatedMultiLineCommentSyntax : JsonBackgroundSyntax, IJsonSymbol
    {
        /// <summary>
        /// Gets the bottom-up only 'green' representation of this syntax node.
        /// </summary>
        public GreenJsonUnterminatedMultiLineCommentSyntax Green { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Green.Length;

        internal JsonUnterminatedMultiLineCommentSyntax(JsonBackgroundListSyntax parent, int backgroundNodeIndex, GreenJsonUnterminatedMultiLineCommentSyntax green)
            : base(parent, backgroundNodeIndex)
            => Green = green;

        TResult IJsonSymbol.Accept<T, TResult>(JsonSymbolVisitor<T, TResult> visitor, T arg) => visitor.VisitUnterminatedMultiLineCommentSyntax(this, arg);
    }
}
