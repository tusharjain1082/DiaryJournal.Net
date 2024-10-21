using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Replace Stage1 text tokens whose value is 'null' with Stage2 null literal tokens
    /// </summary>
    public class NullLiteralIdentifier
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage1Types.Text)
                {
                    var text = token.Value.ToString();
                    if (text == "null")
                        input[i] = new Token(Stage2Types.NullLiteral, null);
                }
            }
        }
    }
}
