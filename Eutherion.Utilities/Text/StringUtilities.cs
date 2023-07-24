#region License
/*********************************************************************************
 * StringUtilities.cs
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
using System.Collections.Generic;
using System.Linq;

namespace Eutherion.Text
{
    /// <summary>
    /// Contains utility methods for strings.
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        /// Generates a display string from an array of parameters in the format "({0}, {1}, ...)".
        /// </summary>
        /// <param name="parameters">
        /// The parameters to format.
        /// </param>
        /// <returns>
        /// If <paramref name="parameters"/> is <see langword="null"/> or empty, returns an empty string.
        /// If <paramref name="parameters"/> has exactly one element, returns "({0})" where {0} is replaced by the single element.
        /// If <paramref name="parameters"/> has more than one element, returns "({0}, {1}, ...)" where {0}, {1}... are replaced by these elements in order.
        /// </returns>
        public static string ToDefaultParameterListDisplayString(params string[] parameters)
            => parameters == null || parameters.Length == 0
            ? string.Empty
            : $"({string.Join(", ", parameters)})";

        /// <summary>
        /// Generates a display string from an array of parameters in the format "({0}, {1}, ...)".
        /// </summary>
        /// <param name="parameters">
        /// The parameters to format.
        /// </param>
        /// <returns>
        /// If <paramref name="parameters"/> is <see langword="null"/> or empty, returns an empty string.
        /// If <paramref name="parameters"/> has exactly one element, returns "({0})" where {0} is replaced by the single element.
        /// If <paramref name="parameters"/> has more than one element, returns "({0}, {1}, ...)" where {0}, {1}... are replaced by these elements in order.
        /// </returns>
        public static string ToDefaultParameterListDisplayString(IEnumerable<string> parameters)
            => parameters == null || !parameters.Any()
            ? string.Empty
            : $"({string.Join(", ", parameters)})";

        /// <summary>
        /// Predicts how many arguments <see cref="string.Format(string, object[])"/> needs to not throw a <see cref="FormatException"/>
        /// from its argument array being too small.
        /// </summary>
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// <param name="wouldThrowFormatException">
        /// Returns a prediction on whether or not <see cref="string.Format(string, object[])"/> will throw a <see cref="FormatException"/>
        /// from <paramref name="format"/> being invalid.
        /// </param>
        /// <returns>
        /// The required minimum number of arguments.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> is <see langword="null"/>.
        /// </exception>

        // Control flow exactly the same as in StringBuilder.AppendFormatHelper().
        // Introduces brittleness because source may change without notice, but this risk is minimal and acceptable.
        // https://referencesource.microsoft.com/#mscorlib/system/text/stringbuilder.cs
        public static int FormatStringRequiredArgumentCount(string format, out bool wouldThrowFormatException)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));

            wouldThrowFormatException = true;

            // Keep track of the maximum used value that StringBuilder.AppendFormatHelper() would use to index into 'args',
            // in order to determine how many parameters must be appended to 'args' such that it has enough parameters
            // to fulfill the number of parameters required by 'format'.
            // E.g. if 'format' was "{0}{1}{2}{0}{5}" then requiredCount is 6.
            int requiredCount = 0;
            int pos = 0;
            int len = format.Length;

            while (true)
            {
                char ch;

                while (pos < len)
                {
                    ch = format[pos];
                    pos++;

                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}')
                        {
                            pos++;
                        }
                        else
                        {
                            return requiredCount;
                        }
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{')
                        {
                            pos++;
                        }
                        else
                        {
                            pos--;
                            break;
                        }
                    }
                }

                if (pos == len) break;
                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9') return requiredCount;
                int index = 0;

                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    if (pos == len) return requiredCount;
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && index < 1000000);

                while (pos < len && (ch = format[pos]) == ' ') pos++;

                int width = 0;

                if (ch == ',')
                {
                    pos++;

                    while (pos < len && format[pos] == ' ') pos++;

                    if (pos == len) return requiredCount;
                    ch = format[pos];
                    
                    if (ch == '-')
                    {
                        pos++;
                        if (pos == len) return requiredCount;
                        ch = format[pos];
                    }

                    if (ch < '0' || ch > '9') return requiredCount;
                    
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len) return requiredCount;
                        ch = format[pos];
                    } while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ') pos++;

                if (index >= requiredCount) requiredCount = index + 1;

                if (ch == ':')
                {
                    pos++;

                    while (true)
                    {
                        if (pos == len) return requiredCount;
                        ch = format[pos];
                        pos++;

                        if (ch == '{')
                        {
                            if (pos < len && format[pos] == '{')
                            {
                                pos++;
                            }
                            else
                            {
                                return requiredCount;
                            }
                        }
                        else if (ch == '}')
                        {
                            if (pos < len && format[pos] == '}')
                            {
                                pos++;
                            }
                            else
                            {
                                pos--;
                                break;
                            }
                        }
                    }
                }

                if (ch != '}') return requiredCount;
                pos++;
            }

            // Only exit point where string.Format() would not throw.
            wouldThrowFormatException = false;
            return requiredCount;
        }
    }
}
