using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Maps all Stage2 Literal tokens into evaluable literal tokens
    /// </summary>
    public class LiteralBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                if (input[i].IsStage2Literal())
                    input[i] = new LiteralToken(input[i].Value);
            }
        }
    }
}
