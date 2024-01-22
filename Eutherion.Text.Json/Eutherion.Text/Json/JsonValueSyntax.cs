#region License
/*********************************************************************************
 * JsonValueSyntax.cs
 *
 * Copyright (c) 2004-2024 Henk Nicolai
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
    /// Represents a node containing a single json value in an abstract json syntax tree.
    /// Use <see cref="JsonValueSyntaxVisitor{T, TResult}"/> overrides to distinguish between implementations of this type.
    /// </summary>
    public abstract class JsonValueSyntax : JsonSyntax
    {
        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public JsonValueWithBackgroundSyntax Parent { get; }

        /// <summary>
        /// Gets the start position of this syntax node relative to its parent's start position.
        /// </summary>
        public sealed override int Start => Parent.BackgroundBefore.Length;

        /// <summary>
        /// Gets the parent syntax node of this instance.
        /// </summary>
        public sealed override JsonSyntax ParentSyntax => Parent;

        /// <summary>
        /// Returns if this syntax node represents a potentially valid JSON value.
        /// </summary>
        /// <remarks>
        /// This property returns <see langword="true"/> if and only if this is a
        /// <see cref="JsonBooleanLiteralSyntax"/>,
        /// <see cref="JsonIntegerLiteralSyntax"/>,
        /// <see cref="JsonStringLiteralSyntax"/>,
        /// <see cref="JsonListSyntax"/>,
        /// or a <see cref="JsonMapSyntax"/>.
        /// </remarks>
        public virtual bool IsValidValue => false;

        internal JsonValueSyntax(JsonValueWithBackgroundSyntax parent) => Parent = parent;

        internal abstract TResult Accept<T, TResult>(JsonValueSyntaxVisitor<T, TResult> visitor, T arg);
    }
}
