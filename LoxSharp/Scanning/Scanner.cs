using LoxSharp.ErrorHandling;
using LoxSharp.Helpers;
using System.Collections;
using System.Collections.Immutable;

namespace LoxSharp.Scanning
{
    public sealed class Scanner : IEnumerable<Token>
    {
        private readonly string source;
        private readonly IErrorReporter errorReporter;
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private static readonly ImmutableDictionary<string, TokenType?> keywords;

        static Scanner()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, TokenType?>();
            builder.Add("and", TokenType.And);
            builder.Add("class", TokenType.Class);
            builder.Add("else", TokenType.Else);
            builder.Add("false", TokenType.False);
            builder.Add("for", TokenType.For);
            builder.Add("fun", TokenType.Fun);
            builder.Add("if", TokenType.If);
            builder.Add("nil", TokenType.Nil);
            builder.Add("or", TokenType.Or);
            builder.Add("print", TokenType.Print);
            builder.Add("return", TokenType.Return);
            builder.Add("super", TokenType.Super);
            builder.Add("this", TokenType.This);
            builder.Add("true", TokenType.True);
            builder.Add("var", TokenType.Var);
            builder.Add("while", TokenType.While);
            keywords = builder.ToImmutable();
        }

        public Scanner(string source, IErrorReporter errorReporter)
        {
            this.source = source;
            this.errorReporter = errorReporter;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            while (!IsAtEnd())
            {
                start = current;
                var token = ScanToken();
                if (token != NullToken.Instance)
                {
                    yield return token;
                }
            }
            yield return new Token(TokenType.EOF, "", null, line);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Token ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': return GenerateToken(TokenType.LeftParen);
                case ')': return GenerateToken(TokenType.RightParen);
                case '}': return GenerateToken(TokenType.RightBrace);
                case '{': return GenerateToken(TokenType.LeftBrace);
                case ',': return GenerateToken(TokenType.Comma);
                case '.': return GenerateToken(TokenType.Dot);
                case '-': return GenerateToken(TokenType.Minus);
                case '+': return GenerateToken(TokenType.Plus);
                case ';': return GenerateToken(TokenType.Semicolon);
                case '*': return GenerateToken(TokenType.Star);
                case '!': return GenerateToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                case '=': return GenerateToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                case '<': return GenerateToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                case '>': return GenerateToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                case '/': return ParseComment();
                case '"': return ParseString();
                // Skip whitespace characters
                case ' ':
                case '\r':
                case '\t':
                    return NullToken.Instance;
                case '\n':
                    line++;
                    return NullToken.Instance;
                default:
                    if (c.IsDigit())
                    {
                        return ParseNumber();
                    }
                    else if (c.IsAlpha())
                    {
                        return ParseIdentifier();
                    }
                    else
                    {
                        errorReporter.Error(line, $"Unexpected character '{c}'.");
                        return NullToken.Instance;
                    }

            }
        }

        private Token ParseComment()
        {
            if (Match('/'))
            {
                ParseSingleLineComment();
                return NullToken.Instance;
            }
            else if (Match('*'))
            {
                ParseMultilineComment();
                return NullToken.Instance;
            }
            else
            {
                return GenerateToken(TokenType.Slash);
            }
        }

        private void ParseMultilineComment()
        {
            while (!IsAtEnd())
            {
                if (Peek() == '\n') line++;
                if (Peek() == '*')
                {
                    if (PeekNext() == '/') break;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                errorReporter.Error(line, "Unterminated block comment.");
                return;
            }

            // Consume *
            Advance();
            // Consume /
            Advance();
        }

        private void ParseSingleLineComment()
        {
            while (Peek() != '\n' && !IsAtEnd()) Advance();
        }

        private Token ParseIdentifier()
        {
            while (Peek().IsAlphaNumeric()) Advance();
            var text = source[start..current];
            keywords.TryGetValue(text, out var type);
            return GenerateToken(type ?? TokenType.Identifier);
        }

        private Token ParseNumber()
        {
            while (Peek().IsDigit()) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && PeekNext().IsDigit())
            {
                // Consume the '.'
                Advance();

                while (Peek().IsDigit()) Advance();
            }

            return AddToken(TokenType.Number, Double.Parse(source[start..current]));
        }

        private Token ParseString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                errorReporter.Error(line, "Unterminated string.");
                return NullToken.Instance;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            var value = source[(start + 1)..(current - 1)];
            return AddToken(TokenType.String, value);
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool Match(char expected)
        {
            if (Peek() != expected) return false;
            current++;
            return true;
        }

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private char Advance()
        {
            return source[current++];
        }

        private Token GenerateToken(TokenType type)
        {
            return AddToken(type, null);
        }

        private Token AddToken(TokenType type, object? literal)
        {
            var text = source[start..current];
            return new Token(type, text, literal, line);
        }

        private sealed record NullToken : Token
        {
            public static readonly Token Instance = new NullToken(TokenType.EOF, "", null, -1);
            private NullToken(TokenType Type, string Lexeme, object? Literal, int Line) : base(Type, Lexeme, Literal, Line)
            {
            }
        }
    }
}
