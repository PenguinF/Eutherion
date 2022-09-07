#region License
/*********************************************************************************
 * UIActionHandler.cs
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

using Eutherion.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eutherion.UIActions
{
    /// <summary>
    /// Manages a set of actions and their associated handlers.
    /// </summary>
    public class UIActionHandler
    {
        private readonly Dictionary<StringKey<UIAction>, UIActionHandlerFunc> handlers
#if NET5_0_OR_GREATER
            = new();
#else
            = new Dictionary<StringKey<UIAction>, UIActionHandlerFunc>();
#endif

        private readonly List<(ImplementationSet<IUIActionInterface>, StringKey<UIAction>)> interfaceSets
#if NET5_0_OR_GREATER
            = new();
#else
            = new List<(ImplementationSet<IUIActionInterface>, StringKey<UIAction>)>();
#endif

        /// <summary>
        /// Enumerates all sets of interfaces which map to an invokable <see cref="UIAction"/> of this handler.
        /// </summary>
        public IEnumerable<(ImplementationSet<IUIActionInterface>, StringKey<UIAction>)> InterfaceSets => interfaceSets.Enumerate();

        /// <summary>
        /// Binds a handler function for a <see cref="UIAction"/> to this <see cref="UIActionHandler"/>,
        /// and specifies how this <see cref="UIAction"/> is exposed to the user interface.
        /// </summary>
        /// <param name="binding">
        /// The <see cref="UIActionBinding"/> containing the <see cref="UIAction"/> to bind, its interfaces, and its handler.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="binding"/> is null.
        /// </exception>
        public void BindAction(UIActionBinding binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            handlers.Add(binding.Action, binding.Handler);
            interfaceSets.Add((binding.Interfaces, binding.Action));

            Invalidate();
        }

        /// <summary>
        /// Occurs when action states are invalidated, and the result of <see cref="TryPerformAction"/> is expected to have changed
        /// for at least one action in this handler.
        /// </summary>
#if NET472
        public event Action<UIActionHandler> UIActionsInvalidated;
#else
        public event Action<UIActionHandler>? UIActionsInvalidated;
#endif

        /// <summary>
        /// Raises the <see cref="UIActionsInvalidated"/> event. 
        /// </summary>
        protected virtual void OnUIActionsInvalidated()
        {
            UIActionsInvalidated?.Invoke(this);
        }

        /// <summary>
        /// Verifies if an action can be performed, and optionally performs it.
        /// </summary>
        /// <param name="action">
        /// The <see cref="UIAction"/> to perform.
        /// </param>
        /// <param name="perform">
        /// Whether or not to perform the action.
        /// </param>
        /// <returns>
        /// A complete <see cref="UIActionState"/> if <paramref name="perform"/> is false,
        /// or a <see cref="UIActionState"/> indicating whether or not the action was performed successfully, if <paramref name="perform"/> is true.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is null.
        /// </exception>
        public UIActionState TryPerformAction(StringKey<UIAction> action, bool perform)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (handlers.TryGetValue(action, out var handler))
            {
                // Call the handler.
                UIActionState result = handler(perform);

                // Raise event if an action has been performed.
                if (perform)
                {
                    Invalidate();
                }

                return result;
            }

            // Default is to look at parent controls for unsupported actions.
            // (The default value has UIActionVisibility.Parent).
            return default;
        }

        /// <summary>
        /// Invalidates this <see cref="UIActionHandler"/> manually.
        /// </summary>
        public void Invalidate()
        {
            OnUIActionsInvalidated();
        }
    }
}
