#region License
/*********************************************************************************
 * JsonColonSyntax.Green.cs
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

namespace Eutherion.Text.Json
{
    /// <summary>
    /// Represents a colon syntax node.
    /// </summary>
    public sealed class GreenJsonColonSyntax : IGreenJsonSymbol
    {
        /// <summary>
        /// Returns the singleton instance.
        /// </summary>
        public static GreenJsonColonSyntax Value { get; }
#if NET5_0_OR_GREATER
            = new();
#else
            = new GreenJsonColonSyntax();
#endif

        /// <summary>
        /// Gets the length of the text span corresponding with this syntax node.
        /// </summary>
        public int Length => JsonSpecialCharacter.SingleCharacterLength;

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public JsonSymbolType SymbolType => JsonSymbolType.Colon;

        private GreenJsonColonSyntax() { }
    }
}
