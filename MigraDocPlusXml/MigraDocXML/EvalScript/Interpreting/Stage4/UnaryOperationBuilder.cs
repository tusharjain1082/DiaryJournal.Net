using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Build unary operations by looking for Stage2 unary operation tokens, taking the right token as its operand
    /// </summary>
    public class UnaryOperationBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i + 1 < input.Count; i++)
            {
                if(input[i].IsStage2UnaryOperator() && !(input[i] is EvaluableToken))
                {
                    var right = input[i + 1] as EvaluableToken;
                    if (right == null)
                        throw new SyntaxException("Unary operation defined on non-evaluable operand");

                    input[i] = new UnaryOperationToken(right, input[i].Type);
                    input.RemoveAt(i + 1);
                }
            }
        }
    }
}
