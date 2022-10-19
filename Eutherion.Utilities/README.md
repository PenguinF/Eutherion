# Eutherion.Utilities

Contains miscellaneous .NET utilities:

  * _TextFormatter_, allowing formatting methods to be defined for keys and a variable number of parameters.
  * _SafeLazy<TValue>_, _SafeLazyObject<TObject>_, and _SafeLazyObjectCollection<TObject>_, which lazily initialize values based on two different asynchronous initialization patterns.
  * _EnumValues<TEnum>_, which stores a list of declared enumeration values in memory and behaves like a regular list.
  * _IFunc<out TResult>_, to be able to model a _Func<TResult>_ as an interface.
  * _ObservableValue\<TValue\>_, which adds events to a value to observe updates to it.
  * _ImplementationSet<TInterface>_, to manage a set of objects keyed by their type, that all share a common base class or interface.
