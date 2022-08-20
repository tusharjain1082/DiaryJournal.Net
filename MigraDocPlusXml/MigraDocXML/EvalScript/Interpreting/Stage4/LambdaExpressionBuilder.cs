using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Builds Lambda expressions by detecting Stage2 LambdaOperator tokens, then using the tokens to the left & right as parameters & body respectively
    /// </summary>
    public class LambdaExpressionBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i + 1 < input.Count; i++)
            {
                if(input[i].Type == Stage2Types.LambdaOperator)
                {
					bool removeIMinus1 = false;

                    var parameters = new List<string>();
                    if (i > 0 && input[i - 1].Type == Stage3Types.Brackets)
                    {
                        var bracketChildren = (input[i - 1] as ParentToken).Children;
                        if (bracketChildren.Count % 2 == 1)
                        {
                            for (int j = 0; j < bracketChildren.Count; j++)
                            {
                                if (j % 2 == 0)
                                {
                                    var property = bracketChildren[j] as PropertyToken;
                                    if (property == null)
                                        throw new SyntaxException("Invalid lambda parameter");
                                    else
                                        parameters.Add(property.Name);
                                }
                            }
                        }
						removeIMinus1 = true;
                    }
                    else if(i > 0)
                    {
                        var property = input[i - 1] as PropertyToken;
                        if (property == null)
                            throw new SyntaxException("Invalid lambda parameter");
                        else
                            parameters.Add(property.Name);
						removeIMinus1 = true;
                    }

                    input[i] = new LambdaExpressionToken(parameters, input[i + 1] as EvaluableToken);
					input.RemoveAt(i + 1);
					if (removeIMinus1)
						input.RemoveAt(i - 1);
                }
            }
        }
    }
}
