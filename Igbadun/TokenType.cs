namespace Igbadun
{
    public enum TokenType
    {
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, LEFT_SQUARE, RIGHT_SQUARE,
        COMMA, SEMI_COLON, COLON, DOT, HASH,
        PLUS, MINUS, STAR, FORWARD_SLASH,
        
        BACK_SLASH, BANG,
        
        PERCENT,
        POWER,
        
        EQUAL, GREATER, LESS, NOT_EQUAL, GREATER_OR_EQUAL, LESS_OR_EQUAL,

        IDENTIFIER, STRING, NUMERIC,
        
        AND, OR, TRUE, FALSE, IF, ELSE, FUN, NIL,
        
        VAL, MUTABLE,
        
        RETURN,
        
        PRINT,

        EOF
    }
}