using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage3
{
    /// <summary>
    /// Find left/right bracket tokens, take everything inside and put them into a new brackets token, so they are nested
    /// </summary>
    public class BracketNester
    {
        public enum Types { Brackets, SquareBrackets }

        public Types BracketType { get; set; }

        private int LeftBracketType => (BracketType == Types.Brackets) ?
            Stage2Types.LeftBracket :
            Stage2Types.LeftSquareBracket;

        private int RightBracketType => (BracketType == Types.Brackets) ?
            Stage2Types.RightBracket :
            Stage2Types.RightSquareBracket;

        public BracketNester(Types bracketType)
        {
            BracketType = bracketType;
        }

        public void Run(Interpreter interpreter, List<Token> input)
        {
            foreach (var parent in input.OfType<ParentToken>())
                Run(interpreter, parent.Children);

            while (true)
            {
                int i = input.LastIndexOf(x => x.Type == LeftBracketType);
                if (i < 0)
                    break;

                int j = input.IndexOf(x => x.Type == RightBracketType, startIndex: i);
                if (j < 0)
                    throw new Exception("Unclosed bracket detected");

                int bracketType = (BracketType == Types.Brackets) ?
                    Stage3Types.Brackets :
                    Stage3Types.SquareBrackets;

                input[i] = new ParentToken(bracketType, input.GetRange(i + 1, j - i - 1));
                input.RemoveRange(i + 1, j - i);
            }
        }
    }
}
