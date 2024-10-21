using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage3
{
    /// <summary>
    /// Find ternary operations and put them into their own nesting
    /// </summary>
    public class TernaryNester
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            //This ensures we start from the most deeply nested tokens and work our way up
            foreach (var parent in input.OfType<ParentToken>())
                Run(interpreter, parent.Children);

            //Loop through each token, excluding the first one and last three, looking for the question mark symbol as our starting point
            for(int i = input.Count - 4; i >= 1; i--)
            {
                if(input[i].Type == Stage2Types.QuestionMark)
                {
					for(int j = i + 2; j < input.Count - 2; j++)
					{
						if(input[j].Type == Stage2Types.KeyValueOperator)
						{
							int start = i - 1;
							while(start - 1 >= 0)
							{
								if (input[start - 1].IsStage2BinaryOperator())
									break;
								start--;
							}
							int end = j + 1;
							while(end + 1 < input.Count)
							{
								if (input[end + 1].IsStage2BinaryOperator())
									break;
								end++;
							}
							input[start] = new ParentToken(Stage3Types.Operation, input.GetRange(start, end - start + 1));
							input.RemoveRange(start + 1, end - start);
						}
					}
                }
            }
        }
    }
}
