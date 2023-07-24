#region License
/*********************************************************************************
 * JsonSymbolVisitor.cs
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
    /// Represents a visitor that visits a single <see cref="IJsonSymbol"/>, which is a <see cref="JsonSyntax"/> node without any child nodes.
    /// See also: https://en.wikipedia.org/wiki/Visitor_pattern
    /// </summary>
    /// <typeparam name="T">A parameter type.</typeparam>
    /// <typeparam name="TResult">The type of value to return from each implementation.</typeparam>
    public abstract class JsonSymbolVisitor<T, TResult>
    {
        /// <summary>Provides a default implementation for a pattern.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult DefaultVisit(IJsonSymbol node, T arg) => throw new InvalidPatternMatchException();
        /// <summary>Matches a pattern and calls the matching implementation for it.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns><exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public virtual TResult Visit(IJsonSymbol node, T arg) => node == null ? throw new ArgumentNullException(nameof(node)) : node.Accept(this, arg);

        /// <summary>Provides an implementation for <see cref="JsonBooleanLiteralSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitBooleanLiteralSyntax(JsonBooleanLiteralSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonColonSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitColonSyntax(JsonColonSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonCommaSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitCommaSyntax(JsonCommaSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonCommentSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitCommentSyntax(JsonCommentSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonCurlyCloseSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitCurlyCloseSyntax(JsonCurlyCloseSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonCurlyOpenSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitCurlyOpenSyntax(JsonCurlyOpenSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonErrorStringSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitErrorStringSyntax(JsonErrorStringSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonIntegerLiteralSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitIntegerLiteralSyntax(JsonIntegerLiteralSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonRootLevelValueDelimiterSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitRootLevelValueDelimiterSyntax(JsonRootLevelValueDelimiterSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonSquareBracketCloseSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitSquareBracketCloseSyntax(JsonSquareBracketCloseSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonSquareBracketOpenSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitSquareBracketOpenSyntax(JsonSquareBracketOpenSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonStringLiteralSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitStringLiteralSyntax(JsonStringLiteralSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonUndefinedValueSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitUndefinedValueSyntax(JsonUndefinedValueSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonUnknownSymbolSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitUnknownSymbolSyntax(JsonUnknownSymbolSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonUnterminatedMultiLineCommentSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitUnterminatedMultiLineCommentSyntax(JsonUnterminatedMultiLineCommentSyntax node, T arg) => DefaultVisit(node, arg);
        /// <summary>Provides an implementation for <see cref="JsonWhitespaceSyntax"/>.</summary><param name="node">The pattern to match.</param><param name="arg">A parameter containing extra information.</param><returns>The result for the matched pattern.</returns>
        public virtual TResult VisitWhitespaceSyntax(JsonWhitespaceSyntax node, T arg) => DefaultVisit(node, arg);
    }

    /// <summary>
    /// Represents a visitor that visits a single <see cref="IJsonSymbol"/>, which is a <see cref="JsonSyntax"/> node without any child nodes.
    /// See also: https://en.wikipedia.org/wiki/Visitor_pattern
    /// </summary>
    /// <typeparam name="TResult">The type of value to return from each implementation.</typeparam>
    public abstract class JsonSymbolVisitor<TResult> : JsonSymbolVisitor<_void, TResult>
    {
        /// <summary>Matches a pattern and calls the matching implementation for it.</summary><param name="node">The pattern to match.</param><returns>The result for the matched pattern.</returns><exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public virtual TResult Visit(IJsonSymbol node) => Visit(node, _void._);
    }

    /// <summary>
    /// Represents a visitor that visits a single <see cref="IJsonSymbol"/>, which is a <see cref="JsonSyntax"/> node without any child nodes.
    /// See also: https://en.wikipedia.org/wiki/Visitor_pattern
    /// </summary>
    public abstract class JsonSymbolVisitor : JsonSymbolVisitor<_void, _void>
    {
        /// <summary>Matches a pattern and calls the matching implementation for it.</summary><param name="node">The pattern to match.</param><exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public virtual void Visit(IJsonSymbol node) => Visit(node, _void._);
    }
}
