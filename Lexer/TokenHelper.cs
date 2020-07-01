namespace Tokeniser
{
    public partial class Token
    {
        public bool IsKeywordType(TokenType tokenType)
        {
            return (int)tokenType >= 21;
        }
        public bool IsIdentityType(TokenType tokenType)
        {
            return tokenType.Equals(TokenType.IDENT);
        }
        public bool IsIllegalType(TokenType tokenType)
        {
            return tokenType.Equals(TokenType.ILLEGAL);
        }
    }
}
