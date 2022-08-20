using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage3
{
    /// <summary>
    /// Find operators of the specified type, put the operator plus its surrounding tokens into a new token so that they're nested
    /// </summary>
    public class BinaryOperationNester
    {
        public int OperationTokenType { get; set; }

        public BinaryOperationNester(int operationTokenType)
        {
            OperationTokenType = operationTokenType;
        }

        public void Run(Interpreter interpreter, List<Token> input)
        {
            //This ensures we start from the most deeply nested tokens and work our way up
            foreach (var parent in input.OfType<ParentToken>())
                Run(interpreter, parent.Children);

            //Loop through each token, excluding the first and last ones, since we want to find the binary operator, which should never be in first or last position
            for (int i = 1; i + 1 < input.Count; i++)
            {
                if (input[i].Type == OperationTokenType)
                {
                    int start = i - 1;
                    while (start - 1 >= 0)
                    {
                        if (input[start - 1].IsStage2BinaryOperator())
                            break;
                        start--;
                    }
                    int end = i + 1;
                    while (end + 1 < input.Count)
                    {
                        if (input[end + 1].IsStage2BinaryOperator())
                            break;
                        end++;
                    }

                    input[start] = new ParentToken(Stage3Types.Operation, input.GetRange(start, end - start + 1));
                    input.RemoveRange(start + 1, end - start);
					i = start;
                }
            }
        }
    }
}
