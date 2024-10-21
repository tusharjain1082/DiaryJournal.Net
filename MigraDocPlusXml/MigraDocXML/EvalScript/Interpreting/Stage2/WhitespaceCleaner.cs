using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Remove all whitespace
    /// </summary>
    public class WhitespaceCleaner
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = input.Count - 1; i >= 0; i--)
            {
                if (input[i].Type == Stage1Types.Whitespace)
                    input.RemoveAt(i);
            }
        }
    }
}
