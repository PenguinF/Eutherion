#region License
/*********************************************************************************
 * EnumArray.cs
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

using Eutherion.Collections;
using System;

namespace Eutherion.Example472
{
    /// <summary>
    /// Demonstrates the <see cref="EnumValues{TEnum}"/> class.
    /// </summary>
    public readonly struct EnumArray<TEnum, TValue>
        where TEnum : Enum
    {
        /// <summary>
        /// Initializes an empty array with default values.
        /// </summary>
        public static EnumArray<TEnum, TValue> New() => new EnumArray<TEnum, TValue>(default);

        private readonly TValue[] arr;

        // Employ a trick so 'arr' has a non-null default value when this constructor is used.
        // Sadly it seems there is no way to set a readonly field in a struct to something non-nullable, even if this field is declared as such.
        // See also: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references#known-pitfalls, where the question of how
        // to declare a struct or array in such a way that these pitfalls cannot occur is left open ended.
        public EnumArray(_void _) => arr = new TValue[EnumValues<TEnum>.List.Count];

        public int Length => arr.Length;

        public TValue this[TEnum index]
        {
            get => arr[(int)(object)index];
            set => arr[(int)(object)index] = value;
        }
    }
}
