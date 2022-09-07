#region License
/*********************************************************************************
 * UIActionSample.cs
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
using Eutherion.UIActions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Eutherion.Example472
{
    #region Exposing actions in different ways

    // To expose actions to a UI, provide a IUIActionInterface implementation for each separate way.
    // If you'd like to generate buttons for actions in a ribbon for example, you'd define a RibbonUIActionInterface
    // which presumably contains properties to indicate its location in the ribbon among other things.
    // This however is just the declarative part (the 'model'). You would also have to write code which given
    // a ribbon and a UIActionHandler containing RibbonUIActionInterfaces will generate buttons for it.

    public struct Keys
    {
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public ConsoleKey Key { get; set; }

        public static bool EqualKeys(Keys x, Keys y)
            => x.Control == y.Control
            && x.Alt == y.Alt
            && x.Shift == y.Shift
            && x.Key == y.Key;
    }

    /// <summary>
    /// Defines how a <see cref="UIAction"/> can be invoked by a keyboard shortcut.
    /// </summary>
    public class ShortcutKeysUIActionInterface : IUIActionInterface
    {
        /// <summary>
        /// Array of shortcut keys which will invoke the action.
        /// </summary>
        public Keys[] Shortcuts { get; set; }
    }

    // Note that implementations can be shared by different components. It makes sense for a context menu
    // generator to not just use an action's ContextMenuUIActionInterface, but also look for a
    // ShortcutKeysUIActionInterface so it can display an action's shortcut key.

    /// <summary>
    /// Defines how a <see cref="UIAction"/> is shown in a context menu.
    /// </summary>
    public class ContextMenuUIActionInterface : IUIActionInterface
    {
        /// <summary>
        /// Gets the display text for the menu item.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Defines the image to display for the menu item.
        /// </summary>
        public Image Icon { get; set; }
    }

    #endregion Exposing actions in different ways

    #region Shared action definitions

    // Knowing now how to define actions, the only thing remaining is the definitions themselves,
    // and they probably look something like this:

    public static class ShardUIActions
    {
        public static readonly UIAction Close = new UIAction(
            new StringKey<UIAction>(nameof(Close)),
            new ImplementationSet<IUIActionInterface>
            {
                new ShortcutKeysUIActionInterface { Shortcuts = new[] { new Keys { Alt = true, Key = ConsoleKey.F4 } }, },
                new ContextMenuUIActionInterface { Caption = "Close" },
            });

        public static readonly UIAction SelectAll = new UIAction(
            new StringKey<UIAction>(nameof(SelectAll)),
            new ImplementationSet<IUIActionInterface>
            {
                new ShortcutKeysUIActionInterface { Shortcuts = new[] { new Keys { Control = true, Key = ConsoleKey.A } }, },
                new ContextMenuUIActionInterface { Caption = "Select All", },
            });

        public static readonly UIAction Copy = new UIAction(
            new StringKey<UIAction>(nameof(Copy)),
            new ImplementationSet<IUIActionInterface>
            {
                new ShortcutKeysUIActionInterface { Shortcuts = new[]
                {
                    new Keys { Control = true, Key = ConsoleKey.C },
                    // If you want to be fancy and declare additional shortcuts:
                    new Keys { Control = true, Key = ConsoleKey.Insert },
                }},
                new ContextMenuUIActionInterface { Caption = "Copy", /*Icon = Properties.Resources.CopyIcon,*/ },
            });
    }

    // The advantage of this approach is that actions which are common between several types of components
    // can share their action definitions, without having to share the implementation of how the action is performed.

    // In a ribbon for example, you may have a button at a fixed position for 'Select all', and then the button's click event handler
    // could check if the focused control implements IUIActionHandlerProvider, look for a Select All handler, and invoke it.

    #endregion Shared action definitions

    #region Stubs for System.Windows.Forms

    // This test project does not reference System.Windows.Forms, so to show the example properly, add some stubs.

    public struct Message { }

    public class Control
    {
        public static Control GetFocusedControl() => new Control();

        public Control Parent { get; set; }

        public List<Control> Controls { get; }
    }

    public class Form : Control
    {
        protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData) => false;
    }

    #endregion Stubs for System.Windows.Forms

    #region Attaching handlers to the UI

    public class TextBox : Control, IUIActionHandlerProvider
    {
        // Stub
        public void Copy() { /* Copies selected text to the clipboard */ }

        // Stub
        public void SelectAll() { /* Selects all text */ }

        // Stub
        public int SelectionLength { get; set; /* Gets/sets the current selection length */ }

        // The control should only expose the action handler.
        // It allows code that declares the control to also declare what actions should be available for it.
        // (Analogous to how controls only define defaults for most of their properties.)
        public UIActionHandler ActionHandler { get; } = new UIActionHandler();

        // However, it can definitely expose public methods with the right signature to suggest
        // that certain actions can be bound to it.
        public UIActionState TryCopy(bool perform)
        {
            if (SelectionLength == 0) return UIActionVisibility.Disabled;
            if (perform) Copy();
            return UIActionVisibility.Enabled;
        }

        public UIActionState TrySelectAll(bool perform)
        {
            if (perform) SelectAll();
            return UIActionVisibility.Enabled;
        }
    }

    // It is useful to be able to define methods for classes that are both a Control AND a IUIActionHandlerProvider.
    // This can simplify syntax quite a bit.
    // Fortunately, extension methods are exactly the kind of tool that can make this work.
    public static class UIActionHandlerProviderExtensions
    {
        // This method has the effect of pretending UIActionHandler.BindAction() can be called on a Control itself,
        // as long as it implements IUIActionHandlerProvider.
        public static void BindAction<T>(this T provider, UIActionBinding binding)
            where T : Control, IUIActionHandlerProvider
        {
            if (provider != null && provider.ActionHandler != null)
            {
                provider.ActionHandler.BindAction(binding);
            }
        }

        // Extra overloads for creating UIActionBindings before calling the actual BindAction().

        public static void BindAction<T>(this T provider, StringKey<UIAction> action, ImplementationSet<IUIActionInterface> interfaces, UIActionHandlerFunc handler)
            where T : Control, IUIActionHandlerProvider
            => BindAction(provider, new UIActionBinding(action, interfaces, handler));

        public static void BindAction<T>(this T provider, UIAction action, UIActionHandlerFunc handler)
            where T : Control, IUIActionHandlerProvider
            => BindAction(provider, new UIActionBinding(action, handler));
    }


    public class MainForm : Form
    {
        public MainForm()
        {
            // When instantiating a TextBox, also declare what actions it exposes.
            TextBox textBox = new TextBox();

            // These two lines of code now have the effect that the textbox's action handler registers
            // methods to handle two actions. If e.g. Ctrl+C is pressed, event handler code can then
            // iterate through controls, and discover this text box has a handler registered
            // for Ctrl+C, which it can then invoke.
            // Using this overload of BindAction() means that the default suggested ways of exposing an action are used,
            // as they are defined in ShardUIActions. A different overload allows you to override the default, for example
            // if you wouldn't want this particular instance to show in a context menu, but would still like to have
            // Ctrl+C work as expected.
            textBox.BindAction(ShardUIActions.Copy, textBox.TryCopy);
            textBox.BindAction(ShardUIActions.SelectAll, textBox.TrySelectAll);

            Controls.Add(textBox);
        }
    }

    #endregion Attaching handlers to the UI

    #region Connecting the dots

    // The only thing remaining is to tie everything together by making sure user events trigger the right action handler.

    public class UIActionForm : Form
    {
        // This helper method starts with a focused control, then goes up in the control tree
        // until it reaches the topmost control which is a Form.
        // For each control, it checks if it implements UIActionHandler which could potentially handle a shortcut key.
        private static IEnumerable<UIActionHandler> EnumerateUIActionHandlers(Control startControl)
        {
            Control control = startControl;

            while (control != null)
            {
                if (control is IUIActionHandlerProvider provider && provider.ActionHandler != null)
                {
                    yield return provider.ActionHandler;
                }

                control = control.Parent;
            }
        }

        // One option to implement shortcut keys is to override ProcessCmdKey().
        // It could then traverse the control tree to look for action handlers to invoke an action.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                Control bottomLevelControl = Control.GetFocusedControl();

                // Try to find an action with given shortcut.
                if ((from actionHandler in EnumerateUIActionHandlers(bottomLevelControl)
                     from interfaceActionPair in actionHandler.InterfaceSets
                         // Enumerate over all bound shortcut keys in the action handler.
                         // Note how only a specific interface is selected from the implementation set.
                         // So if an action exposes only a ContextMenuUIActionInterface for example,
                         // it can never be invoked from this method.
                     let shortcuts = interfaceActionPair.Item1.Get<ShortcutKeysUIActionInterface>()?.Shortcuts
                     where shortcuts != null
                     from registeredShortcut in shortcuts
                         // If the shortcut matches, then try to perform the action.
                         // If the handler does not return UIActionVisibility.Undetermined, then assume it
                         // knows about the action and decides not to invoke it, so then make sure no parent control
                         // handles the shortcut either by returning true.
                     where Keys.EqualKeys(registeredShortcut, keyData)
                     select actionHandler.TryPerformAction(interfaceActionPair.Item2, true).UIActionVisibility)
                        // Using Any() here ensures that at most one handler will handle the shortcut.
                        .Any(x => x != UIActionVisibility.Undetermined))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                // Exception handler not relevant to the example.
                Console.WriteLine(e.Message);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    #endregion Connecting the dots
}
