namespace Tokeniser
{
    public enum TokenType
        {
            EOF,
            //single symbol
            ASSIGN,
            PLUS,
            MINUS,
            BANG,
            SLASH,
            ASTERISK,
            LT,
            GT,
            COMMA,
            SEMICOLON,
            LPAREN,
            RPAREN,
            LBRACE,
            RBRACE,
            //two symbol
            EQ,
            NOT_EQ,
            GTEQ,
            LTEQ,
            //Identity
            IDENT,
            //Illegal
            ILLEGAL,
            //Keywords
            FUNCTION,
            LET,
            TRUE,
            FALSE,
            IF,
            ELSE,
            RETURN,
            INT,
            NULL,
        }
}
