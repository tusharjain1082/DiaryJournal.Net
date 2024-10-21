using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Identify Stage2 minus operators which don't fit within a binary operation, treat them as negative unary operators
    /// </summary>
    public class NegativeOperatorIdentifier
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage2Types.MinusOperator)
                {
                    if (i > 0)
                    {
                        var prevType = input[i - 1].Type;

                        if (input[i - 1].IsStage2BinaryOperator() ||
                            prevType == Stage2Types.LeftBracket ||
                            prevType == Stage2Types.LeftSquareBracket ||
                            prevType == Stage2Types.Comma ||
                            prevType == Stage2Types.KeyValueOperator)
                        {
                            input[i] = new Token(Stage2Types.NegativeOperator);
                        }
                    }
                    else
                        input[i] = new Token(Stage2Types.NegativeOperator);
                }
            }
        }
    }
}
