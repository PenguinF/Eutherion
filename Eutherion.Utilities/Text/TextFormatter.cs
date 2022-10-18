#region License
/*********************************************************************************
 * TextFormatter.cs
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
using System.Collections.Generic;
using System.Linq;

namespace Eutherion.Text
{
    /// <summary>
    /// Defines an abstract method to provide formatted text for a <see cref="StringKey{T}"/> of <see cref="ForFormattedText"/>.
    /// </summary>
    public abstract class TextFormatter
    {
        private sealed class DefaultTextFormatter : TextFormatter
        {
            public override string Format(StringKey<ForFormattedText> key, params string[] parameters)
            {
                if (key == null) return string.Empty;
                return "{" + key.Key + StringUtilities.ToDefaultParameterListDisplayString(parameters) + "}";
            }
        }

        /// <summary>
        /// Gets a reference to a default <see cref="TextFormatter"/>, which uses <see cref="StringUtilities.ToDefaultParameterListDisplayString(IEnumerable{string})"/>
        /// to provide formatted text for any <see cref="StringKey{T}"/> of <see cref="ForFormattedText"/>. If the key is null, it returns an empty string.
        /// </summary>
        public static readonly TextFormatter Default = new DefaultTextFormatter();

        /// <summary>
        /// Formats text given a <see cref="StringKey{T}"/> of <see cref="ForFormattedText"/> and parameters.
        /// </summary>
        /// <param name="key">
        /// The <see cref="StringKey{T}"/> of <see cref="ForFormattedText"/> for which to generate the formatted text.
        /// </param>
        /// <param name="parameters">
        /// The parameters of the formatted text to generate.
        /// </param>
        /// <returns>
        /// The formatted text.
        /// </returns>
        public string Format(StringKey<ForFormattedText> key, IEnumerable<string> parameters)
            => Format(key, parameters == null ? Array.Empty<string>() : parameters.ToArrayEx());

        /// <summary>
        /// Formats text given a <see cref="StringKey{T}"/> of <see cref="ForFormattedText"/> and parameters.
        /// </summary>
        /// <param name="key">
        /// The <see cref="StringKey{T}"/> of <see cref="ForFormattedText"/> for which to generate the formatted text.
        /// </param>
        /// <param name="parameters">
        /// The parameters of the formatted text to generate.
        /// </param>
        /// <returns>
        /// The formatted text.
        /// </returns>
        public abstract string Format(StringKey<ForFormattedText> key, params string[] parameters);
    }
}
