using System;
using System.Collections.Generic;
using System.Linq;

namespace Tokeniser
{
    public partial class Lexer
    {
        private Dictionary<Func<byte, bool>, Func<Lexer, Token>> tokens = new Dictionary<Func<byte, bool>, Func<Lexer, Token>>()
        {
            { (byte ch) => ch == 0, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.EOF); } },
            { (byte ch) => ch == 33, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.BANG); } },
            { (byte ch) => ch == 40, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.LPAREN); } },
            { (byte ch) => ch == 41, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.RPAREN); } },
            { (byte ch) => ch == 42, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.ASTERISK); } },
            { (byte ch) => ch == 43, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.PLUS); } },
            { (byte ch) => ch == 44, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.COMMA); } },
            { (byte ch) => ch == 45, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.MINUS); } },
            { (byte ch) => ch == 47, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.SLASH); } },
            { (byte ch) => ch == 59, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.SEMICOLON); } },
            { (byte ch) => ch == 60, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.LT); } },
            { (byte ch) => ch == 61, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.ASSIGN); } },
            { (byte ch) => ch == 62, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.GT); } },
            { (byte ch) => ch == 123, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.LBRACE); } },
            { (byte ch) => ch == 125, (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.RBRACE); } },
            { (byte ch) => IsDigit(ch), (Lexer lexer) => { return lexer.CreateToken(lexer, TokenType.INT); } },
            { (byte ch) => IsLetter(ch), (Lexer lexer) => { return lexer.CreateToken(lexer); } }
        };

        public Token NextToken(Lexer lexer)
        {
            SkipWhiteSpace(lexer);
            Token token;

            try { token = tokens.Single(c => c.Key(lexer.Ch)).Value(lexer); }
            catch { token = CreateToken(lexer); }

            if (!token.IsKeywordType(token.Type) && !token.IsIdentityType(token.Type) && !token.IsIllegalType(token.Type))
                ReadChar(lexer);

            return token;
        }

        private Token CreateToken(Lexer lexer, TokenType type = TokenType.ILLEGAL)
        {
            var twoCharTokenTypes = new Dictionary<TokenType, TokenType>()
            {
                { TokenType.ASSIGN, TokenType.EQ },
                { TokenType.BANG, TokenType.NOT_EQ },
                { TokenType.LT, TokenType.LTEQ },
                { TokenType.GT, TokenType.GTEQ }
            };

            if (IsTwoChar(lexer))
            {
                var ch = lexer.Ch;
                ReadChar(lexer);
                return new Token(twoCharTokenTypes[type], Convert.ToChar(ch).ToString() + Convert.ToChar(lexer.Ch).ToString());
            }
            else if (IsLetter(lexer.Ch))
            {
                var text = ReadIdentifier(lexer);
                type = LookupIdent(text);
                return new Token(type, text);
            }
            else if (IsDigit(lexer.Ch))
                return new Token(type, ReadNumber(lexer));
            else if (type == TokenType.ILLEGAL)
                return new Token(TokenType.ILLEGAL, string.Empty);
            else
                return new Token(type, Convert.ToChar(lexer.Ch).ToString());
        }

        private static void SkipWhiteSpace(Lexer lexer)
        {
            var whitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

            while (whitespaceChars.Contains(Convert.ToChar(lexer.Ch)))
                ReadChar(lexer);
        }

        private static string ReadIdentifier(Lexer lexer)
        {
            var position = lexer.Position;

            while (IsLetter(lexer.Ch))
                ReadChar(lexer);

            return lexer.Input.Substring(position, lexer.Position - position);
        }

        private static string ReadNumber(Lexer lexer)
        {
            var position = lexer.Position;

            while (IsDigit(lexer.Ch))
                ReadChar(lexer);

            return lexer.Input.Substring(position, lexer.Position - position);
        }

        private static void ReadChar(Lexer lexer)
        {
            if (lexer.ReadPosition >= lexer.Input.Length)
                lexer.Ch = 0;
            else
                lexer.Ch = Convert.ToByte(lexer.Input[Convert.ToInt32(lexer.ReadPosition)]);

            lexer.Position = lexer.ReadPosition;
            lexer.ReadPosition += 1;
        }

        private static byte PeekChar(Lexer lexer)
        {
            if (lexer.ReadPosition >= lexer.Input.Length)
                return 0;
            else
                return Convert.ToByte(lexer.Input[Convert.ToInt32(lexer.ReadPosition)]);
        }

        private static bool IsTwoChar(Lexer lexer)
        {
            return PeekChar(lexer) == '=';
        }

        private static TokenType LookupIdent(string identifier)
        {
            var keywords = new Dictionary<string, TokenType>
            {
                {"fn", TokenType.FUNCTION },
                {"let", TokenType.LET },
                {"true", TokenType.TRUE },
                {"false", TokenType.FALSE },
                {"if", TokenType.IF },
                {"else", TokenType.ELSE },
                {"return", TokenType.RETURN },
                {"null", TokenType.NULL }
            };

            return keywords.ContainsKey(identifier) ? keywords[identifier] : TokenType.IDENT;
        }

        private static bool IsLetter(byte ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
        }

        private static bool IsDigit(byte ch)
        {
            return '0' <= ch && ch <= '9';
        }
    }
}
