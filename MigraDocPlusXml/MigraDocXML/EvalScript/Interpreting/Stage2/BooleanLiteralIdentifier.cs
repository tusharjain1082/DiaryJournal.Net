using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Replace Stage1 text tokens whose value is 'true' or 'false' with Stage2 boolean literal tokens
    /// </summary>
    public class BooleanLiteralIdentifier
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage1Types.Text)
                {
                    var text = token.Value.ToString();
                    if (text == "true")
                        input[i] = new Token(Stage2Types.BooleanLiteral, true);
                    else if (text == "false")
                        input[i] = new Token(Stage2Types.BooleanLiteral, false);
                }
            }
        }
    }
}
