#region License
/*********************************************************************************
 * UIAction.cs
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

namespace Eutherion.UIActions
{
    /// <summary>
    /// Represents a user action, with a set of default suggested ways to expose the action.
    /// </summary>
    public sealed class UIAction
    {
        /// <summary>
        /// Gets the key which identifies this <see cref="UIAction"/>.
        /// </summary>
        public StringKey<UIAction> Key { get; }

        /// <summary>
        /// Gets the suggested default <see cref="ImplementationSet{TInterface}"/> which defines how the action is exposed to the user interface.
        /// </summary>
        public ImplementationSet<IUIActionInterface> DefaultInterfaces { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UIAction"/>.
        /// </summary>
        /// <param name="action">
        /// The key to identify this <see cref="UIAction"/>.
        /// </param>
        /// <param name="defaultInterfaces">
        /// The suggested default <see cref="ImplementationSet{TInterface}"/> which defines how the action is exposed to the user interface.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> and/or <paramref name="defaultInterfaces"/> are null.
        /// </exception>
        public UIAction(StringKey<UIAction> action, ImplementationSet<IUIActionInterface> defaultInterfaces)
        {
            Key = action ?? throw new ArgumentNullException(nameof(action));
            DefaultInterfaces = defaultInterfaces ?? throw new ArgumentNullException(nameof(defaultInterfaces));
        }
    }
}
