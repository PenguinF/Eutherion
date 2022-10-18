# Eutherion.UIActions

Contains a basic framework to model user actions:

  * _UIActionState_ and _UIActionVisibility_ represent a common way in which the state of an action is represented to an end user in e.g. menu items.
  * _UIActionHandlerFunc_ is a delegate to calculate an action's current state, as well as subsequently invoking the action if its 'perform' parameter is true.
  * _IUIActionInterface_ is an empty interface to mark ways in which an action is exposed to a user interface, e.g. shortcut keys, menu items, client areas, etc.
  * _UIAction_ is the basic definition of an action, with a key to identify the action (to facilitate scenarios in which actions with bindings in different user interface elements can be accessed in the same way, e.g. through a main menu), as well as a default suggested set of ways to expose the action.
  * _UIActionBinding_ represents a runtime binding of an action to a user interface element. It contains the action itself, a definition of how it is exposed, and an attached handler to invoke the action.
  * _UIActionHandler_ manages a collection of action bindings for a single user interface element.
  * _IUIActionHandlerProvider_ is implemented by user interface elements to indicate they can bind and handle actions.
