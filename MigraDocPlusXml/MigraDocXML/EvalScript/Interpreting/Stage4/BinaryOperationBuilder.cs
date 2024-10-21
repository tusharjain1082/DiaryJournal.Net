using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Build binary operations by looking for Stage2 binary operation tokens, taking the tokens to the left and right of the operator as its operands
    /// </summary>
    public class BinaryOperationBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 1; i + 1 < input.Count; i++)
            {
                //Added check that token isn't binary operation token, since they still return the type of the operation they contain
                //This was causing an interpreter error when binary ops are used in the middle of a comma-separated list of values such as function parameters
                if(input[i].IsStage2BinaryOperator() && !(input[i] is BinaryOperationToken))
                {
                    var left = input[i - 1] as EvaluableToken;
                    var right = input[i + 1] as EvaluableToken;
                    if (left == null || right == null)
                        throw new SyntaxException("Binary operation defined on non-evaluable operand");

                    input[i - 1] = new BinaryOperationToken(left, input[i].Type, right);
                    input.RemoveRange(i, 2);
                }
            }
        }
    }
}
