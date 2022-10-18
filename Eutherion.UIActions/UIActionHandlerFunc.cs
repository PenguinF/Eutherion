#region License
/*********************************************************************************
 * UIActionHandlerFunc.cs
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
    /// Delegate which verifies if an action can be performed, and optionally performs it.
    /// </summary>
    /// <param name="perform">
    /// Whether or not to actually perform the action.
    /// </param>
    /// <returns>
    /// An initialized <see cref="UIActionState"/> describing the current state of the action if <paramref name="perform"/> is false,
    /// or a <see cref="UIActionState"/> of which the <see cref="UIActionState.Enabled"/> property indicates whether or not the action was performed successfully,
    /// if <paramref name="perform"/> is true.
    /// </returns>
    public delegate UIActionState UIActionHandlerFunc(bool perform);

    // Author's note:
    //
    // At first sight it may seem weird to lump two entirely different methods into one delegate,
    // but I've found after years of experience that this design reduces the chance significantly of bugs that are caused by
    // subtle differences between calculating when actions are allowed to either generate the state of an action versus
    // when the action is actually performed. These result in either an action not being available even if it should be,
    // or nothing happening at all when the action is invoked, depending on which method has the correct predicate code.
    //
    // Timing issues may occur as well, when the predicate code depends on e.g. something that's running in the background.
    //
    // I've even tried refactoring a well known open source project to match this interface, and managed to find
    // a few beautiful crashes where predicates were mismatched.
    //
    // This also encourages a coding style in which information is gathered incrementally regardless of whether or not
    // an action will actually be performed, meaning that a returned state will automatically be more accurate.
    //
    // As long as checks are cheap, the additional cost of potentially evaluating the same predicate twice (once for
    // perform == false, then for perform == true) is negligible.
}
