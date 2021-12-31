namespace Igbadun
{
    public class Token
    {
        public readonly int Line;
        public readonly string Lexeme;
        public readonly TokenType TokenType;
        public readonly object Literal;

        public Token(TokenType tokenType, string lexeme, object literal, int line)
        {
            this.TokenType = tokenType;
            Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        public override string ToString() => $"{TokenType} {Lexeme} {Literal}";
    }
}