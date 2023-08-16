#region License
/*********************************************************************************
 * RootJsonSyntax.cs
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

using Eutherion.Threading;
using System;
using System.Collections.Generic;

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Contains the syntax tree and list of parse errors which are the result of parsing source text in the JSON format.
    /// The root node of any abstract syntax tree is of this type.
    /// </summary>
    public sealed class RootJsonSyntax : JsonSyntax
    {
        /// <summary>
        /// Gets a reference to the JSON string that generated this parse tree.
        /// </summary>
        public string Json { get; }

        /// <summary>
        /// Gets the root <see cref="JsonMultiValueSyntax"/> node.
        /// </summary>
        public JsonMultiValueSyntax Syntax { get; }

        /// <summary>
        /// Gets the list containing all errors generated during a parse.
        /// </summary>
        public ReadOnlyList<JsonErrorInfo> Errors { get; }

        /// <summary>
        /// Returns 0 because this syntax node is the root node.
        /// </summary>
        public override int Start => 0;

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public override int Length => Syntax.Length;

        /// <summary>
        /// Gets the parent syntax node of this instance. Returns <see langword="null"/> for the root node.
        /// </summary>
#if NET472
        public override JsonSyntax ParentSyntax => null;
#else
        public override JsonSyntax? ParentSyntax => null;
#endif

        /// <summary>
        /// Gets the root node of this syntax tree, which is this syntax node.
        /// </summary>
        public override RootJsonSyntax Root => this;

        /// <summary>
        /// Gets the number of children of this syntax node.
        /// </summary>
        public override int ChildCount => 1;

        /// <summary>
        /// Initializes the child at the given <paramref name="index"/> and returns it.
        /// </summary>
        /// <param name="index">
        /// The index of the child node to return.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="ChildCount"/>.
        /// </exception>
        public override JsonSyntax GetChild(int index)
        {
            if (index == 0) return Syntax;
            throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the start position of the child at the given <paramref name="index"/>, without initializing it.
        /// </summary>
        /// <param name="index">
        /// The index of the child node for which to return the start position.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than or equal to <see cref="ChildCount"/>.
        /// </exception>
        public override int GetChildStartPosition(int index)
        {
            if (index == 0) return 0;
            throw ExceptionUtil.ThrowListIndexOutOfRangeException();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RootJsonSyntax"/>.
        /// </summary>
        /// <param name="json">
        /// A reference to the JSON string that generated this parse tree.
        /// </param>
        /// <param name="syntax">
        /// The root <see cref="GreenJsonMultiValueSyntax"/> node from which to construct a <see cref="JsonMultiValueSyntax"/> abstract syntax tree.
        /// </param>
        /// <param name="errors">
        /// The list containing all errors generated during a parse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="json"/> and/or <paramref name="syntax"/> and/or <paramref name="errors"/> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The lengths of <paramref name="json"/> and <paramref name="syntax"/> are different.
        /// </exception>
        public RootJsonSyntax(string json, GreenJsonMultiValueSyntax syntax, ReadOnlyList<JsonErrorInfo> errors)
            // Accept overhead that for only the root node this creates an unnecessary extra check if evaluated.
            : base(new SafeLazy<int>(() => 0))
        {
            Json = json ?? throw new ArgumentNullException(nameof(json));
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));

            if (json.Length != syntax.Length)
            {
                throw new ArgumentException(
                    $"The lengths of {nameof(json)} ({json.Length}) and {nameof(syntax)} ({syntax.Length}) are not equal",
                    nameof(json));
            }

            Syntax = new JsonMultiValueSyntax(syntax, this);
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
