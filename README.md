# LoxSharp

A C# implementation of a tree-walk interpreter for the Lox programming language described in Bob Nystrom's [Crafting Interpreters](https://craftinginterpreters.com/).

The code in this project is heavily inspired from Bob's Java implementation in the said book.

#### Differences with the book's Java implementation:

- I opted for an [`IErrorReporter`](LoxSharp/ErrorHandling/IErrorReporter.cs) abstraction for error handling as suggested in the [Error handling](https://craftinginterpreters.com/scanning.html#error-handling) part of the Scanning chapter. Bob saw it a bit overkill for the interpreter he was writing in the book. I thought it would be better to keep it, as it would make testing error handling easier.

- I implemented the [Scanner](LoxSharp/Scanning/Scanner.cs) as a lazy iterator (i.e, an Enumerable in C#).

- I added support for [multi-line comments](LoxSharp/Scanning/Scanner.cs#L111).
