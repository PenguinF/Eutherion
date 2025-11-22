# Eutherion.Utilities

Contains miscellaneous .NET utilities:

  * _TextFormatter_, allowing formatting methods to be defined for keys and a variable number of parameters.
  * _StringKey\<T\>_, to be able to distinguish between different types of string keys, allowing the compiler to detect when keys are confused between different types.
  * _SafeLazy\<TValue\>_, _SafeLazyObject\<TObject\>_, and _SafeLazyObjectCollection\<TObject\>_, which lazily initialize values based on two different asynchronous initialization patterns.
  * _EnumValues\<TEnum\>_, which stores a list of declared enumeration values in memory and behaves like a regular list.
  * _IFunc\<out TResult\>_, to be able to model a _Func\<TResult\>_ as an interface.
  * _ObservableValue\<TValue\>_, which adds events to a value to observe updates to it.
  * _ImplementationSet\<TInterface\>_, to manage a set of objects keyed by their type, that all share a common base class or interface.
  * _DisposableResourceCollection_, to deterministically dispose of a variable set of disposable resources.
  * _ExceptionSink_, a shared point of reference to help track suppressed Exceptions.
  * _TestUtilities_, which has methods to wrap unit test cases in a way xunit can consume them, and to create cross joins (Cartesian products) of independent parameter sets.
