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
            //1. Parser

            while (true)
                console(Console.ReadLine());
        }

        static void console(string input)
        {
            var lexer = new Lexer(input);

            List<Token> tokenList = new List<Token>();

            while (lexer.ReadPosition <= input.Length)
                tokenList.Add(lexer.NextToken(lexer));

            foreach (var t in tokenList)
                Console.WriteLine($"Token Type: {t.Type}, Token Value: {t.Literal}");

            var parser = new Parser(lexer);

        }
    }

    public class Parser
    {
        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;
        }

        public Lexer Lexer { get; set; }
        public Token CurrentToken { get; set; }
        public Token PeekToken { get; set; }

        public void NextToken(Parser parser)
        {
            parser.CurrentToken = parser.PeekToken;
            parser.PeekToken = parser.Lexer.NextToken(Lexer);
        }

        public MyProgram ParseProgram(Parser parser)
        {
            return null;
        }
    }

    public class MyProgram
    {
        public Statement[] Statements { get; set; }

        public string TokenLiteral(MyProgram program)
        {
            if (program.Statements.Length > 0)
            {
                return program.Statements[0].Program.TokenLiteral(program);
            }
            else
            {
                return "";
            }
        }
    }

    public class LetStatement
    {
        //Let token
        public Token Token { get; set; }
        public Identifier Name { get; set; }
        public Expression Expression { get; set; }
    }

    public class Identifier
    {
        public Token token { get; set; }
        public string value { get; set; }
    }

    public class Statement
    {
        public MyProgram Program { get; set; }
    }

    public class Expression
    {

    }
}