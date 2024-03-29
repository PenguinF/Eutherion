﻿#region License
/*********************************************************************************
 * _void.cs
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

using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Represents the empty/void type, which means that its values contain no information.
    /// This type is intended to be used as a generic type parameter.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "'_' prefix chosen so type name is as close as possible to the regular 'void' keyword")]
    public struct _void
    {
        /// <summary>
        /// Returns a void value.
        /// </summary>
        public static _void _ => default;
    }
}
