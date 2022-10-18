#region License
/*********************************************************************************
 * JsonValueWithBackgroundSyntax.Green.cs
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
    /// Represents a <see cref="GreenJsonValueSyntax"/> node together with the background symbols directly before it in an abstract syntax tree.
    /// </summary>
    public sealed class GreenJsonValueWithBackgroundSyntax : ISpan
    {
        /// <summary>
        /// Gets the background symbols which directly precede the content value node.
        /// </summary>
        public GreenJsonBackgroundListSyntax BackgroundBefore { get; }

        /// <summary>
        /// Gets the content node containing the actual value.
        /// </summary>
        public GreenJsonValueSyntax ContentNode { get; }

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="GreenJsonValueWithBackgroundSyntax"/>.
        /// </summary>
        /// <param name="backgroundBefore">
        /// The background symbols which directly precede the content value node.
        /// </param>
        /// <param name="contentNode">
        /// The content node containing the actual value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="backgroundBefore"/> and/or <paramref name="contentNode"/> are null.
        /// </exception>
        public GreenJsonValueWithBackgroundSyntax(GreenJsonBackgroundListSyntax backgroundBefore, GreenJsonValueSyntax contentNode)
        {
            BackgroundBefore = backgroundBefore ?? throw new ArgumentNullException(nameof(backgroundBefore));
            ContentNode = contentNode ?? throw new ArgumentNullException(nameof(contentNode));
            Length = BackgroundBefore.Length + ContentNode.Length;
        }
    }
}
