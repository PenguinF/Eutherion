# Eutherion.Common

Contains extensions to the .NET standard libraries:

  * Extra LINQ extensions, such as _Any(out TValue value)_, _ForEach(Action\<T\>)_, and _ToArrayEx()_ which has a clause for IReadOnlyCollection\<T\>.
  * Optional values (_Maybe\<TValue\>_).
  * Discriminated unions, implemented without branching (_Union\<T1...Tn\>_).
  * A way to create string keys of different types without having to copy the implementation (_StringKey\<T\>_ where 'T' is an otherwise meaningless type value to add type safety).
  * Specialized IEnumerable\<T\> implementations for zero or one elements.
  * _ReadOnlyList\<T\>_, a light-weight wrapper around an array to make it read-only/immutable.
  * Bit vector (uint/ulong) extensions, such as _UIntExtensions.SetBits()_, to enumerate over all set bits of a value.
  * Other, miscellaneous members (_void_, _Box\<T\>_, etc.)
