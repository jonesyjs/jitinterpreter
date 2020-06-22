using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIT_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            //this is practice!!!
            while (true)
                console(Console.ReadLine());
        }

        static void console(string input)
        {
            var lexer = new Lexer(input);

            List<Token> tokenList = new List<Token>();

            while (lexer.ReadPosition <= input.Length)
                tokenList.Add(lexer.NextToken());

            foreach (var t in tokenList)
                Console.WriteLine($"Token Type: {t.Type}, Token Value: {t.Literal}");
        }
    }

    public class Lexer
    {
        public Lexer(string input, int position = 0, int readPosition = 1)
        {
            this.Input = input;
            this.Position = position;
            this.ReadPosition = readPosition;
            this.Ch = Convert.ToByte(input[position]);
        }

        public string Input { get; }
        public int Position { get; set; }
        public int ReadPosition { get; set; }
        public byte Ch { get; set; }
    };

    public static class LexerExtensions
    {
        private static Dictionary<Func<byte, bool>, Func<Lexer, Token>> tokens = new Dictionary<Func<byte, bool>, Func<Lexer, Token>>()
        {
            { (byte ch) => ch == 0, (Lexer lexer) => { return lexer.CreateToken(TokenType.EOF); } },
            { (byte ch) => ch == 33, (Lexer lexer) => { return lexer.CreateToken(TokenType.BANG); } },
            { (byte ch) => ch == 40, (Lexer lexer) => { return lexer.CreateToken(TokenType.LPAREN); } },
            { (byte ch) => ch == 41, (Lexer lexer) => { return lexer.CreateToken(TokenType.RPAREN); } },
            { (byte ch) => ch == 42, (Lexer lexer) => { return lexer.CreateToken(TokenType.ASTERISK); } },
            { (byte ch) => ch == 43, (Lexer lexer) => { return lexer.CreateToken(TokenType.PLUS); } },
            { (byte ch) => ch == 44, (Lexer lexer) => { return lexer.CreateToken(TokenType.COMMA); } },
            { (byte ch) => ch == 45, (Lexer lexer) => { return lexer.CreateToken(TokenType.MINUS); } },
            { (byte ch) => ch == 47, (Lexer lexer) => { return lexer.CreateToken(TokenType.SLASH); } },
            { (byte ch) => ch == 59, (Lexer lexer) => { return lexer.CreateToken(TokenType.SEMICOLON); } },
            { (byte ch) => ch == 60, (Lexer lexer) => { return lexer.CreateToken(TokenType.LT); } },
            { (byte ch) => ch == 61, (Lexer lexer) => { return lexer.CreateToken(TokenType.ASSIGN); } },
            { (byte ch) => ch == 62, (Lexer lexer) => { return lexer.CreateToken(TokenType.GT); } },
            { (byte ch) => ch == 123, (Lexer lexer) => { return lexer.CreateToken(TokenType.LBRACE); } },
            { (byte ch) => ch == 125, (Lexer lexer) => { return lexer.CreateToken(TokenType.RBRACE); } },
            { (byte ch) => IsDigit(ch), (Lexer lexer) => { return lexer.CreateToken(TokenType.INT); } },
            { (byte ch) => IsLetter(ch), (Lexer lexer) => { return lexer.CreateToken(LookupIdent(lexer.ReadIdentifier())); } }
        };

        public static Token NextToken(this Lexer lexer)
        {
            lexer.SkipWhiteSpace();
            Token token;

            try { token = tokens.SingleOrDefault(c => c.Key(lexer.Ch)).Value(lexer); }
            catch { token = new Token(TokenType.ILLEGAL, string.Empty); }

            lexer.ReadChar();
            return token;
        }

        private static Token CreateToken(this Lexer lexer, TokenType type)
        {
            var twoCharTokenTypes = new Dictionary<TokenType, TokenType>() 
            {
                { TokenType.ASSIGN, TokenType.EQ },
                { TokenType.BANG, TokenType.NOT_EQ },
                { TokenType.LT, TokenType.LTEQ },
                { TokenType.GT, TokenType.GTEQ }
            };

            if (lexer.IsTwoChar())
            {
                var ch = lexer.Ch;
                lexer.ReadChar();
                return new Token(twoCharTokenTypes[type], Convert.ToChar(ch).ToString() + Convert.ToChar(lexer.Ch).ToString());
            } 
            else if (IsLetter(lexer.Ch))
                return new Token(type, lexer.ReadIdentifier());
            else if (IsDigit(lexer.Ch))
                return new Token(type, lexer.ReadNumber());
            else
                return new Token(type, Convert.ToChar(lexer.Ch).ToString());
        }

        private static void SkipWhiteSpace(this Lexer lexer)
        {
            var whitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

            while (whitespaceChars.Contains(Convert.ToChar(lexer.Ch)))
                lexer.ReadChar();
        }

        private static TokenType LookupIdent(string literal)
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

            return keywords.ContainsKey(literal) ? keywords[literal] : TokenType.IDENT;
        }

        private static string ReadIdentifier(this Lexer lexer)
        {
            var position = lexer.Position;

            while (IsLetter(lexer.Ch))
                lexer.ReadChar();

            return lexer.Input.Substring(position, lexer.Position - position);
        }

        private static string ReadNumber(this Lexer lexer)
        {
            var position = lexer.Position;

            while (IsDigit(lexer.Ch))
                lexer.ReadChar();

            return lexer.Input.Substring(position, lexer.Position - position);
        }

        public static void ReadChar(this Lexer lexer)
        {
            if (lexer.ReadPosition >= lexer.Input.Length)
                lexer.Ch = 0;
            else
                lexer.Ch = Convert.ToByte(lexer.Input[Convert.ToInt32(lexer.ReadPosition)]);

            lexer.Position = lexer.ReadPosition;
            lexer.ReadPosition += 1;
        }

        public static byte PeekChar(this Lexer lexer)
        {
            if (lexer.ReadPosition >= lexer.Input.Length)
                return 0;
            else
                return Convert.ToByte(lexer.Input[Convert.ToInt32(lexer.ReadPosition)]);
        }

        private static bool IsLetter(byte ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
        }

        private static bool IsDigit(byte ch)
        {
            return '0' <= ch && ch <= '9';
        }

        private static bool IsTwoChar(this Lexer lexer)
        {
            return lexer.PeekChar() == '=';
        }
    }

    public class Token
    {
        public Token(TokenType? type, string literal)
        {
            this.Type = type;
            this.Literal = literal;
        }

        public TokenType? Type { get; set; }
        public string Literal { get; set; }
    };

    public enum TokenType
    {
        ILLEGAL,
        EOF,
        IDENT,
        INT,
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
        FUNCTION,
        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,
        EQ,
        NOT_EQ,
        GTEQ,
        LTEQ,
        NULL
    }
}