#region License
/*********************************************************************************
 * JsonBooleanLiteralSyntax.Green.cs
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
    /// Represents a boolean literal value syntax node.
    /// </summary>
    public abstract class GreenJsonBooleanLiteralSyntax : GreenJsonValueSyntax, IGreenJsonSymbol
    {
        /// <summary>
        /// Represents a 'false' literal value syntax node.
        /// </summary>
        public sealed class False : GreenJsonBooleanLiteralSyntax
        {
            /// <summary>
            /// Returns the singleton instance.
            /// </summary>
            public static False Instance { get; }
#if NET5_0_OR_GREATER
                = new();
#else
                = new False();
#endif

            private False() { }

            /// <summary>
            /// Gets the boolean value represented by this literal syntax.
            /// </summary>
            public override bool Value => false;

            /// <summary>
            /// Gets the representation of this literal value in JSON source text.
            /// </summary>
            public override string LiteralJsonValue => JsonValue.FalseString;

            /// <summary>
            /// Gets the length of the text span corresponding with this syntax node.
            /// </summary>
            public override int Length => JsonValue.FalseSymbolLength;

            /// <summary>
            /// Invokes a <see cref="Func{TResult}"/> based on whether this instance represents a false or true value, and returns its result.
            /// </summary>
            /// <typeparam name="TResult">
            /// Type of the value to return.
            /// </typeparam>
            /// <param name="whenFalse">
            /// The <see cref="Func{TResult}"/> to invoke if this instance represents a false value.
            /// </param>
            /// <param name="whenTrue">
            /// The <see cref="Func{TResult}"/> to invoke if this instance represents a true value.
            /// </param>
            /// <returns>
            /// The result of the invoked <see cref="Func{TResult}"/>, or a default value if the passed in function is null.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="whenFalse"/> and/or <paramref name="whenTrue"/> are <see langword="null"/>.
            /// </exception>
            public override TResult Match<TResult>(Func<TResult> whenFalse, Func<TResult> whenTrue)
                => whenFalse != null ? whenFalse() : throw new ArgumentNullException(nameof(whenFalse));
        }

        /// <summary>
        /// Represents a 'true' literal value syntax node.
        /// </summary>
        public sealed class True : GreenJsonBooleanLiteralSyntax
        {
            /// <summary>
            /// Returns the singleton instance.
            /// </summary>
            public static True Instance { get; }
#if NET5_0_OR_GREATER
                = new();
#else
                = new True();
#endif

            private True() { }

            /// <summary>
            /// Gets the boolean value represented by this literal syntax.
            /// </summary>
            public override bool Value => true;

            /// <summary>
            /// Gets the representation of this literal value in JSON source text.
            /// </summary>
            public override string LiteralJsonValue => JsonValue.TrueString;

            /// <summary>
            /// Gets the length of the text span corresponding with this syntax node.
            /// </summary>
            public override int Length => JsonValue.TrueSymbolLength;

            /// <summary>
            /// Invokes a <see cref="Func{TResult}"/> based on whether this instance represents a false or true value, and returns its result.
            /// </summary>
            /// <typeparam name="TResult">
            /// Type of the value to return.
            /// </typeparam>
            /// <param name="whenFalse">
            /// The <see cref="Func{TResult}"/> to invoke if this instance represents a false value.
            /// </param>
            /// <param name="whenTrue">
            /// The <see cref="Func{TResult}"/> to invoke if this instance represents a true value.
            /// </param>
            /// <returns>
            /// The result of the invoked <see cref="Func{TResult}"/>, or a default value if the passed in function is null.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="whenFalse"/> and/or <paramref name="whenTrue"/> are <see langword="null"/>.
            /// </exception>
            public override TResult Match<TResult>(Func<TResult> whenFalse, Func<TResult> whenTrue)
                => whenTrue != null ? whenTrue() : throw new ArgumentNullException(nameof(whenTrue));
        }

        /// <summary>
        /// Gets the boolean value represented by this literal syntax.
        /// </summary>
        public abstract bool Value { get; }

        /// <summary>
        /// Gets the representation of this literal value in JSON source text.
        /// </summary>
        public abstract string LiteralJsonValue { get; }

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public JsonSymbolType SymbolType => JsonSymbolType.BooleanLiteral;

        private GreenJsonBooleanLiteralSyntax() { }

        /// <summary>
        /// Invokes a <see cref="Func{TResult}"/> based on whether this instance represents a false or true value, and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenFalse">
        /// The <see cref="Func{TResult}"/> to invoke if this instance represents a false value.
        /// </param>
        /// <param name="whenTrue">
        /// The <see cref="Func{TResult}"/> to invoke if this instance represents a true value.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{TResult}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="whenFalse"/> and/or <paramref name="whenTrue"/> are <see langword="null"/>.
        /// </exception>
        public abstract TResult Match<TResult>(Func<TResult> whenFalse, Func<TResult> whenTrue)
#if !NET472
            where TResult : notnull
#endif
            ;

        internal override TResult Accept<T, TResult>(GreenJsonValueSyntaxVisitor<T, TResult> visitor, T arg) => visitor.VisitBooleanLiteralSyntax(this, arg);
    }
}
