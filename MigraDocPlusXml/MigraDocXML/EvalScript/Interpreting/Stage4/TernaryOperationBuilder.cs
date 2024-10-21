using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    public class TernaryOperationBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 1; i + 3 < input.Count; i++)
            {
                if(input[i].Type == Stage2.Stage2Types.QuestionMark)
                {
                    if(input[i + 2].Type == Stage2.Stage2Types.KeyValueOperator)
                    {
                        var test = input[i - 1] as EvaluableToken;
                        var trueResult = input[i + 1] as EvaluableToken;
                        var falseResult = input[i + 3] as EvaluableToken;
                        if (test == null || trueResult == null || falseResult == null)
                            throw new SyntaxException("Ternary operation defined on non-evaluable operand");

                        input[i - 1] = new TernaryOperationToken(test, trueResult, falseResult);
                        input.RemoveRange(i, 4);
                    }
                }
            }
        }
    }
}
