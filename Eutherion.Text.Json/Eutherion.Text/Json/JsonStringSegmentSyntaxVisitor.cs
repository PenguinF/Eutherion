#region License
/*********************************************************************************
 * JsonStringSegmentSyntaxVisitor.cs
 *
 * Copyright (c) 2004-2025 Henk Nicolai
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
    /// Represents a visitor that visits a single <see cref="JsonStringSegmentSyntax"/>.
    /// See also: https://en.wikipedia.org/wiki/Visitor_pattern
    /// </summary>
    /// <typeparam name="T">A parameter type.</typeparam>
    /// <typeparam name="TResult">The type of value to return from each implementation.</typeparam>
    public abstract class JsonStringSegmentSyntaxVisitor<T, TResult>
    {
        /// <summary>Provides a default implementation for a pattern.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult DefaultVisit(JsonStringSegmentSyntax node, T arg) => throw new InvalidPatternMatchException();
        /// <summary>Matches a pattern and calls the matching implementation for it.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns><exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public virtual TResult Visit(JsonStringSegmentSyntax node, T arg) => node == null ? throw new ArgumentNullException(nameof(node)) : node.Accept(this, arg);

        /// <summary>Provides an implementation for <see cref="JsonRegularStringSegmentSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitRegularStringSegmentSyntax(JsonRegularStringSegmentSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonSimpleEscapeSequenceSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitSimpleEscapeSequenceSyntax(JsonSimpleEscapeSequenceSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonUnicodeEscapeSequenceSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitUnicodeEscapeSequenceSyntax(JsonUnicodeEscapeSequenceSyntax node, T arg) => DefaultVisit(node, arg);
    }

    /// <summary>
    /// Represents a visitor that visits a single <see cref="JsonStringSegmentSyntax"/>.
    /// See also: https://en.wikipedia.org/wiki/Visitor_pattern
    /// </summary>
    /// <typeparam name="TResult">The type of value to return from each implementation.</typeparam>
    public abstract class JsonStringSegmentSyntaxVisitor<TResult> : JsonStringSegmentSyntaxVisitor<_void, TResult>
    {
        /// <summary>Matches a pattern and calls the matching implementation for it.</summary><param name="node">The pattern to match.</param><returns>The result for the matched pattern.</returns><exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public virtual TResult Visit(JsonStringSegmentSyntax node) => Visit(node, _void._);
    }

    /// <summary>
    /// Represents a visitor that visits a single <see cref="JsonStringSegmentSyntax"/>.
    /// See also: https://en.wikipedia.org/wiki/Visitor_pattern
    /// </summary>
    public abstract class JsonStringSegmentSyntaxVisitor : JsonStringSegmentSyntaxVisitor<_void, _void>
    {
        /// <summary>Matches a pattern and calls the matching implementation for it.</summary><param name="node">The pattern to match.</param><exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public virtual void Visit(JsonStringSegmentSyntax node) => Visit(node, _void._);
    }
}
