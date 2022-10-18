#region License
/*********************************************************************************
 * UIActionBinding.cs
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
    /// Defines a set of <see cref="IUIActionInterface"/>s and a <see cref="UIActionHandlerFunc"/> for a given action,
    /// which determines how an action is exposed and implemented by a <see cref="UIActionHandler"/>.
    /// </summary>
    public sealed class UIActionBinding
    {
        /// <summary>
        /// Gets the identifier for the bound <see cref="UIAction"/>.
        /// </summary>
        public StringKey<UIAction> ActionKey { get; }

        /// <summary>
        /// Gets the <see cref="IUIActionInterface"/> set which defines how the action is exposed to the user interface.
        /// </summary>
        public ImplementationSet<IUIActionInterface> Interfaces { get; }

        /// <summary>
        /// Gets the handler function used to invoke the action and determine its <see cref="UIActionState"/>.
        /// </summary>
        public UIActionHandlerFunc Handler { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UIActionBinding"/>.
        /// </summary>
        /// <param name="action">
        /// An action with its set of default suggested interfaces.
        /// </param>
        /// <param name="handler">
        /// The handler function used to invoke the action and determine its <see cref="UIActionState"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> and/or <paramref name="handler"/> are null.
        /// </exception>
        public UIActionBinding(UIAction action, UIActionHandlerFunc handler)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            ActionKey = action.Key;
            Interfaces = action.DefaultInterfaces;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UIActionBinding"/>.
        /// </summary>
        /// <param name="actionKey">
        /// The <see cref="UIAction"/> identifier to bind to a <see cref="UIActionHandler"/>.
        /// </param>
        /// <param name="interfaces">
        /// The <see cref="IUIActionInterface"/> set which defines how the action is exposed to the user interface.
        /// </param>
        /// <param name="handler">
        /// The handler function used to invoke the action and determine its <see cref="UIActionState"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="actionKey"/> and/or <paramref name="interfaces"/> and/or <paramref name="handler"/> are null.
        /// </exception>
        public UIActionBinding(StringKey<UIAction> actionKey, ImplementationSet<IUIActionInterface> interfaces, UIActionHandlerFunc handler)
        {
            ActionKey = actionKey ?? throw new ArgumentNullException(nameof(actionKey));
            Interfaces = interfaces ?? throw new ArgumentNullException(nameof(interfaces));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}
