using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Builds function calls by looking for Stage2 FunctionName tokens, then using the contents of the brackets token to the right to get parameters
    /// </summary>
    public class FunctionCallBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i + 1 < input.Count; i++)
            {
                if (input[i].Type == Stage2Types.FunctionName && input[i + 1].Type == Stage3Types.Brackets)
                {
                    var bracketChildren = (input[i + 1] as ParentToken).Children;
                    if (bracketChildren.Count == 0 || bracketChildren.Count % 2 == 1)
                    {
                        var parameters = new List<EvaluableToken>();
                        for (int j = 0; j < bracketChildren.Count; j++)
                        {
                            if (j % 2 == 0)
                            {
                                var parameter = bracketChildren[j] as EvaluableToken;
                                if (parameter == null)
                                    throw new SyntaxException("Invalid function parameter");
                                parameters.Add(parameter);
                            }
                            else if (bracketChildren[j].Type != Stage2Types.Comma)
                                throw new SyntaxException("Function arguments must be a comma separated list");
                        }
                        input[i] = new FunctionCallToken(input[i].Value.ToString(), parameters);
                        input.RemoveAt(i + 1);
                    }
                }
            }
        }
    }
}
