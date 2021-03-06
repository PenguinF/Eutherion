#region License
/*********************************************************************************
 * UtilityExtensions.cs
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

#if !NET472
#nullable enable
#endif

using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Contains utility extension methods.
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// Returns a locale invariant representation of an integer.
        /// </summary>
        /// <param name="value">
        /// The integer to convert.
        /// </param>
        /// <returns>
        /// The locale invariant representation
        /// </returns>
        public static string ToStringInvariant(this int value)
            => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns a locale invariant representation of an integer.
        /// </summary>
        /// <param name="value">
        /// The integer to convert.
        /// </param>
        /// <returns>
        /// The locale invariant representation
        /// </returns>
        public static string ToStringInvariant(this BigInteger value)
            => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Sets a single value at each index of the array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements of the array.
        /// </typeparam>
        /// <param name="array">
        /// The one-dimensional, zero-based array to fill.
        /// </param>
        /// <param name="value">
        /// The value to set at each index of the array.
        /// </param>
#if !NET472
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Fill<T>(this T[] array, T value)
        {
#if NET472
            for (int i = array.Length - 1; i >= 0; --i)
            {
                array[i] = value;
            }
#else
            Array.Fill(array, value);
#endif
        }

        /// <summary>
        /// Sets a single value at each index of the array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements of the array.
        /// </typeparam>
        /// <param name="array">
        /// The two-dimensional, zero-based array to fill.
        /// </param>
        /// <param name="value">
        /// The value to set at each index of the array.
        /// </param>
        public static void Fill<T>(this T[,] array, T value)
        {
            for (int i = array.GetLength(0) - 1; i >= 0; --i)
            {
                for (int j = array.GetLength(1) - 1; j >= 0; --j)
                {
                    array[i, j] = value;
                }
            }
        }

        /// <summary>
        /// Sets a value at each index of the array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements of the array.
        /// </typeparam>
        /// <param name="array">
        /// The one-dimensional, zero-based array to fill.
        /// </param>
        /// <param name="valueFunc">
        /// The function which given an index returns the value to set at that index of the array.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFunc"/> is null.
        /// </exception>
        public static void Fill<T>(this T[] array, Func<int, T> valueFunc)
        {
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = valueFunc(i);
            }
        }

        /// <summary>
        /// Sets a value at each index of the array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements of the array.
        /// </typeparam>
        /// <param name="array">
        /// The two-dimensional, zero-based array to fill.
        /// </param>
        /// <param name="valueFunc">
        /// The function which given indices i and j returns the value to set at array[i, j].
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFunc"/> is null.
        /// </exception>
        public static void Fill<T>(this T[,] array, Func<int, int, T> valueFunc)
        {
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            for (int i = array.GetLength(0) - 1; i >= 0; --i)
            {
                for (int j = array.GetLength(1) - 1; j >= 0; --j)
                {
                    array[i, j] = valueFunc(i, j);
                }
            }
        }

        /// <summary>
        /// Iterates an action a fixed number of times.
        /// If the number of iterations is zero or lower, nothing happens.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is null.
        /// </exception>
        public static void Times(this int numberOfIterations, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            for (int i = numberOfIterations; i > 0; --i) action();
        }

        /// <summary>
        /// Suppresses an <see cref="Exception"/>, and in debug mode writes it to <see cref="Debug"/>.  
        /// </summary>
        /// <param name="exception">
        /// The <see cref="Exception"/> to suppress.
        /// </param>
        public static void Trace(this Exception exception)
        {
            if (exception != null)
            {
                Debug.WriteLine($"{exception.GetType().FullName}: {exception.Message}");
            }
        }
    }
}
