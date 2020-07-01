using System;

namespace Tokeniser
{
    public partial class Lexer
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
    }
}
