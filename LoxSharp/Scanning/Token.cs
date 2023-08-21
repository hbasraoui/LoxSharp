namespace LoxSharp.Scanning
{
    public record Token(TokenType Type, string Lexeme, object? Literal, int Line);
}
