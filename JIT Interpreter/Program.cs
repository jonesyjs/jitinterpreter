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
        }
    }

}