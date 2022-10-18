#region License
/*********************************************************************************
 * UIActionVisibility.cs
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
    /// Represents one of three types of visibility to a <see cref="UIAction"/>.
    /// </summary>
    public enum UIActionVisibility
    {
        /// <summary>
        /// The <see cref="UIAction"/> is unknown and therefore its state is undetermined.
        /// </summary>
        Undetermined,
        /// <summary>
        /// The <see cref="UIAction"/> is currently not visible.
        /// </summary>
        Hidden,
        /// <summary>
        /// The <see cref="UIAction"/> is currently visible, but disabled.
        /// </summary>
        Disabled,
        /// <summary>
        /// The <see cref="UIAction"/> is currently visible and enabled.
        /// </summary>
        Enabled,
    }
}
