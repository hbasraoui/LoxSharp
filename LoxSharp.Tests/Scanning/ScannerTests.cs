using LoxSharp.ErrorHandling;
using LoxSharp.Scanning;

namespace LoxSharp.Tests.Scanning
{
    [TestFixture]
    internal sealed class ScannerTests
    {
        [Test]
        public void Test_Identifiers()
        {
            var source = @"
                andy formless fo _ _123 _abc ab123
                abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_
            ";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.Identifier, "andy", null, 2),
                new Token(TokenType.Identifier, "formless", null, 2),
                new Token(TokenType.Identifier, "fo", null, 2),
                new Token(TokenType.Identifier, "_", null, 2),
                new Token(TokenType.Identifier, "_123", null, 2),
                new Token(TokenType.Identifier, "_abc", null, 2),
                new Token(TokenType.Identifier, "ab123", null, 2),
                new Token(TokenType.Identifier, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_", null, 3),
                new Token(TokenType.EOF, "", null, 4),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_Keywords()
        {
            var source = "and class else false for fun if nil or return super this true var while";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.And, "and", null, 1),
                new Token(TokenType.Class, "class", null, 1),
                new Token(TokenType.Else, "else", null, 1),
                new Token(TokenType.False, "false", null, 1),
                new Token(TokenType.For, "for", null, 1),
                new Token(TokenType.Fun, "fun", null, 1),
                new Token(TokenType.If, "if", null, 1),
                new Token(TokenType.Nil, "nil", null, 1),
                new Token(TokenType.Or, "or", null, 1),
                new Token(TokenType.Return, "return", null, 1),
                new Token(TokenType.Super, "super", null, 1),
                new Token(TokenType.This, "this", null, 1),
                new Token(TokenType.True, "true", null, 1),
                new Token(TokenType.Var, "var", null, 1),
                new Token(TokenType.While, "while", null, 1),
                new Token(TokenType.EOF, "", null, 1),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_Numbers()
        {
            var source = @"
                123
                123.456
                .456
                123.
            ";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.Number, "123", 123d, 2),
                new Token(TokenType.Number, "123.456", 123.456d, 3),
                new Token(TokenType.Dot, ".", null, 4),
                new Token(TokenType.Number, "456", 456d, 4),
                new Token(TokenType.Number, "123", 123d, 5),
                new Token(TokenType.Dot, ".", null, 5),
                new Token(TokenType.EOF, "", null, 6),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_Punctuators()
        {
            var source = "(){};,+-*!===<=>=!=<>/.=!";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.LeftParen, "(", null, 1),
                new Token(TokenType.RightParen, ")", null, 1),
                new Token(TokenType.LeftBrace, "{", null, 1),
                new Token(TokenType.RightBrace, "}", null, 1),
                new Token(TokenType.Semicolon, ";", null, 1),
                new Token(TokenType.Comma, ",", null, 1),
                new Token(TokenType.Plus, "+", null, 1),
                new Token(TokenType.Minus, "-", null, 1),
                new Token(TokenType.Star, "*", null, 1),
                new Token(TokenType.BangEqual, "!=", null, 1),
                new Token(TokenType.EqualEqual, "==", null, 1),
                new Token(TokenType.LessEqual, "<=", null, 1),
                new Token(TokenType.GreaterEqual, ">=", null, 1),
                new Token(TokenType.BangEqual, "!=", null, 1),
                new Token(TokenType.Less, "<", null, 1),
                new Token(TokenType.Greater, ">", null, 1),
                new Token(TokenType.Slash, "/", null, 1),
                new Token(TokenType.Dot, ".", null, 1),
                new Token(TokenType.Bang, "!", null, 1),
                new Token(TokenType.Equal, "=", null, 1),
                new Token(TokenType.EOF, "", null, 1),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_Strings()
        {
            var source = @"
                """"
                ""string""
                ""hey
there""
            ";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.String, "\"\"", "", 2),
                new Token(TokenType.String, "\"string\"", "string", 3),
                new Token(TokenType.String, $"\"hey{Environment.NewLine}there\"", $"hey{Environment.NewLine}there", 5),
                new Token(TokenType.EOF, "", null, 6)
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_Whitespace()
        {
            var source = @"space    tabs				newlines




end";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.Identifier, "space", null, 1),
                new Token(TokenType.Identifier, "tabs", null, 1),
                new Token(TokenType.Identifier, "newlines", null, 1),
                new Token(TokenType.Identifier, "end", null, 6),
                new Token(TokenType.EOF, "", null, 6),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_Comments()
        {
            var source = @"
                // this is a single line comment
                // this is another one
                hey
                /* this is a multiline comment
                 *  because why not?
                 */  
            ";
            var scanner = new Scanner(source, NoErrorReporter.Instance);
            var expected = new[]
            {
                new Token(TokenType.Identifier, "hey", null, 4),
                new Token(TokenType.EOF, "", null, 8),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
        }

        [Test]
        public void Test_UnsupportedTokens()
        {
            var source = ".$.";
            var errorReporter = new MemoryErrorReporter();
            var scanner = new Scanner(source, errorReporter);
            var expected = new[]
            {
                new Token(TokenType.Dot, ".", null, 1),
                new Token(TokenType.Dot, ".", null, 1),
                new Token(TokenType.EOF, "", null, 1),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
            Assert.That(errorReporter.HadError, Is.True);
        }

        [Test]
        public void Test_UnterminatedString()
        {
            var source = @"""string

something";
            var errorReporter = new MemoryErrorReporter();
            var scanner = new Scanner(source, errorReporter);
            var expected = new[]
            {
                new Token(TokenType.EOF, "", null, 3),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
            Assert.That(errorReporter.HadError, Is.True);
        }

        [Test]
        public void Test_UnterminatedBlockComment()
        {
            var source = @"/* A block comment";
            var errorReporter = new MemoryErrorReporter();
            var scanner = new Scanner(source, errorReporter);
            var expected = new[]
            {
                new Token(TokenType.EOF, "", null, 1),
            };
            CollectionAssert.AreEquivalent(expected, scanner);
            Assert.That(errorReporter.HadError, Is.True);
        }

        private sealed class NoErrorReporter : IErrorReporter
        {
            private static readonly string message = "Shouldn't have been called! Check if the test is correct.";
            public static readonly IErrorReporter Instance = new NoErrorReporter();

            private NoErrorReporter()
            {
            }

            public void Error(int line, string message)
            {
                throw new NotImplementedException(message);
            }

            public bool HadError {
                get => throw new NotImplementedException(message);
            }

            public void Reset()
            {
                throw new NotImplementedException(message);
            }
        }
        private sealed class MemoryErrorReporter : IErrorReporter
        {
            public void Error(int line, string message)
            {
                HadError = true;
            }
   
            public void Reset()
            {
                HadError = false;
            }

            public bool HadError { get; private set; }
        }
    }
}
