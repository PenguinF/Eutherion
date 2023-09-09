# Eutherion.Common

Contains extensions to the .NET standard libraries:

  * Extra LINQ extensions, such as _Any(out TValue value)_, _ForEach(Action\<T\>)_, and _ToArrayEx()_ which has a clause for IReadOnlyCollection\<T\>.
  * Optional values (_Maybe\<TValue\>_).
  * Discriminated unions, implemented without branching (_Union\<T1...Tn\>_).
  * _ReadOnlyList\<T\>_, a light-weight wrapper around an array to make it read-only/immutable.
  * _ArrayBuilder\<T\>_, an append-only array builder that is cleared every time a result array is obtained from it.
  * Bit vector (uint/ulong) extensions, such as _BitExtensions.SetBits()_, to enumerate over all set bits of a value.
  * Specialized IEnumerable\<T\> implementations for zero or one elements.
  * A way to create string keys of different types without having to copy the implementation (_StringKey\<T\>_ where 'T' is an otherwise meaningless type value to add type safety).
  * Other, miscellaneous members (_void_, _ExceptionUtility_, etc.)
  