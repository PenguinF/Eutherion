﻿#region License
/*********************************************************************************
 * IUIActionHandlerProvider.cs
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

namespace Eutherion.UIActions
{
    /// <summary>
    /// Interface implemented by user interface elements to hook into the <see cref="UIAction"/> framework.
    /// </summary>
    public interface IUIActionHandlerProvider
    {
        /// <summary>
        /// Returns the <see cref="UIActionHandler"/> for the user interface element, to which handlers for <see cref="UIAction"/>s can be attached.
        /// </summary>
        UIActionHandler ActionHandler { get; }
    }
}
