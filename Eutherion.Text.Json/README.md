# Eutherion.Text.Json

Lightweight Json parser with error correction and recovery.

It accomplishes this by:

  * Only allocating bare syntactic structures when parsing, maximizing reuse of existing instances with a shared length ('Green' classes).
  * Lazily creating full syntax nodes when they are actually needed.
  * Not storing references to the source string, and only taking substrings from it to create meaningful values.
  * Incorporating syntactical errors into special types of syntax nodes.

Note that this implementation is stack-based, with the disadvantage that it imposes a maximum depth to the syntax tree. This makes it useful for parsing e.g. settings files, but not for deeply nested data structures.

This implementation is fully tested, however does not expose a feature-complete public API. In particular, it is not possible to create syntax trees from scratch that contain all information necessary to serialize to Json.
