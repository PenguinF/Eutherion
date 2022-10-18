#region License
/*********************************************************************************
 * UIActionState.cs
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
    /// Encodes a current state of a <see cref="UIAction"/> in how it is represented in e.g. menu items.
    /// </summary>
    public struct UIActionState
    {
        /// <summary>
        /// Gets the current visibility of the <see cref="UIAction"/>.
        /// </summary>
        public UIActionVisibility UIActionVisibility { get; }

        /// <summary>
        /// Gets if the <see cref="UIAction"/> is currently in a checked state.
        /// </summary>
        public bool Checked { get; }

        /// <summary>
        /// Gets if the <see cref="UIAction"/> is currently visible.
        /// </summary>
        public bool Visible => UIActionVisibility == UIActionVisibility.Disabled || Enabled;

        /// <summary>
        /// Gets if the <see cref="UIAction"/> is currently enabled.
        /// </summary>
        public bool Enabled => UIActionVisibility == UIActionVisibility.Enabled;

        /// <summary>
        /// Initializes a new instance of <see cref="UIActionState"/> with a given <see cref="UIActions.UIActionVisibility"/>
        /// and which is unchecked.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="UIActions.UIActionVisibility"/> of this <see cref="UIActionState"/>.
        /// </param>
        public UIActionState(UIActionVisibility visibility)
        {
            UIActionVisibility = visibility;
            Checked = false;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UIActionState"/> with a given <see cref="UIActions.UIActionVisibility"/>
        /// and checked state.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="UIActions.UIActionVisibility"/> of this <see cref="UIActionState"/>.
        /// </param>
        /// <param name="isChecked">
        /// The checked state of this <see cref="UIActionState"/>.
        /// </param>
        public UIActionState(UIActionVisibility visibility, bool isChecked)
        {
            UIActionVisibility = visibility;
            Checked = isChecked;
        }

        /// <summary>
        /// Implicitly converts a <see cref="UIActions.UIActionVisibility"/> to an instance of <see cref="UIActionState"/>
        /// in which <see cref="Checked"/> is false.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="UIActions.UIActionVisibility"/> to convert.
        /// </param>
        public static implicit operator UIActionState(UIActionVisibility visibility)
#if NET5_0_OR_GREATER
            => new(visibility);
#else
            => new UIActionState(visibility);
#endif
    }
}
