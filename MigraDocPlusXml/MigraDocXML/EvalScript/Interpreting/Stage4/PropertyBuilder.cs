using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Maps Stage2 PropertyName tokens into evaluable property tokens
    /// </summary>
    public class PropertyBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                if (input[i].Type == Stage2Types.PropertyName)
                    input[i] = new PropertyToken(input[i].Value.ToString());
            }
        }
    }
}
