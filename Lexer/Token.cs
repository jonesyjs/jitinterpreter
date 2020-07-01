namespace Tokeniser
{
    public partial class Token
    {
        public Token(TokenType type, string literal)
        {
            this.Type = type;
            this.Literal = literal;
        }

        public TokenType Type { get; set; }
        public string Literal { get; set; }
    }
}
