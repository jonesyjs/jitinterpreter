using System;
using System.Collections.Generic;
using Tokeniser;

namespace JIT_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            //Todo:
            //Build parser

            while (true)
                console(Console.ReadLine());
        }

        static void console(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            parser.NextToken(parser);
            parser.NextToken(parser);

            parser.ParseProgram(parser);

            //List<Token> tokenList = new List<Token>();

            //while (lexer.ReadPosition <= input.Length)
            //    tokenList.Add(lexer.NextToken(lexer));

            //foreach (var t in tokenList)
            //    Console.WriteLine($"Token Type: {t.Type}, Token Value: {t.Literal}");


        }
    }

    public class Parser
    {
        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;
            this.Errors = new List<string>();
        }

        public Lexer Lexer { get; set; }
        public Token CurrentToken { get; set; }
        public Token PeekToken { get; set; }
        public List<string> Errors { get; set; }

        public void NextToken(Parser parser)
        {
            parser.CurrentToken = parser.PeekToken;
            parser.PeekToken = parser.Lexer.NextToken(Lexer);
        }

        public MyProgram ParseProgram(Parser parser)
        {
            var program = new MyProgram();
            program.Statements = new List<Statement>();

            while(parser.CurrentToken.Type != TokenType.EOF)
            {
                var stmt = parser.ParseStatement(parser);

                if (stmt != null)
                {
                    program.Statements.Add(stmt);
                }
                parser.NextToken(parser);
            }

            return program;
        }

        public Statement ParseStatement(Parser parser)
        {
            switch (parser.CurrentToken.Type)
            {
                case TokenType.LET:
                    return parser.ParseLetStatement(parser);
                case TokenType.RETURN:
                    return parser.ParseReturnStatement(parser);
                default:
                    return null;
            }
        } 

        public ReturnStatement ParseReturnStatement(Parser parser)
        {
            var stmt = new ReturnStatement(parser.CurrentToken);

            parser.NextToken(parser);

            while (!parser.CurrentTokenIs(parser, TokenType.SEMICOLON))
            {
                parser.NextToken(parser);
            }

            return stmt;
        }

        public LetStatement ParseLetStatement(Parser parser) {
            var stmt = new LetStatement(parser.CurrentToken);

            if (!parser.ExpectPeek(parser, TokenType.IDENT))
                return null;

            stmt.Name = new Identifier(parser.CurrentToken, parser.CurrentToken.Literal);

            if (!parser.ExpectPeek(parser, TokenType.ASSIGN))
                return null;

            while (!parser.CurrentTokenIs(parser, TokenType.SEMICOLON))
            {
                parser.NextToken(parser);
            }

            return stmt;
        }

        public bool CurrentTokenIs(Parser parser, TokenType type)
        {
            return parser.CurrentToken.Type == type;
        }

        public bool PeekTokenIs(Parser parser, TokenType type)
        {
            return parser.PeekToken.Type == type;
        }

        public bool ExpectPeek(Parser parser, TokenType type)
        {
            if (parser.PeekTokenIs(parser, type))
            {
                parser.NextToken(parser);
                return true;
            } else
            {
                parser.PeekError(parser, type);
                return false;
            }
        }

        public void PeekError(Parser parser, TokenType type)
        {
            var msg = string.Format($"expected next token to be {type}, got {parser.PeekToken.Type} instead.");
            parser.Errors.Add(msg);
        }
    }

    public class MyProgram
    {
        public List<Statement> Statements { get; set; }

        public string TokenLiteral(MyProgram program)
        {
            if (program.Statements.Count > 0)
            {
                return program.Statements[0].Program.TokenLiteral(program);
            }
            else
            {
                return "";
            }
        }
    }

    public class Statement
    {
        public MyProgram Program { get; set; }
    }

    public class LetStatement : Statement
    {
        public LetStatement(Token token)
        {
            this.Token = token;
        }

        //Let token
        public Token Token { get; set; }
        public Identifier Name { get; set; }
        public Expression Expression { get; set; }
    }

    public class ReturnStatement : Statement
    {
        public ReturnStatement(Token token)
        {
            this.Token = token;
        }

        public Token Token { get; set; }
        public Expression ReturnValue { get; set; }
    }

    public class ExpressionStatement : Statement
    {
        public ExpressionStatement(Token token)
        {
            this.Token = token;
        }

        public Token Token { get; set; }
        public Expression ReturnValue { get; set; }
    }

    public class Identifier
    {
        public Identifier(Token token, string value)
        {
            this.Token = token;
            this.Value = value;
        }
        public Token Token { get; set; }
        public string Value { get; set; }
    }

    public class Expression
    {

    }
}