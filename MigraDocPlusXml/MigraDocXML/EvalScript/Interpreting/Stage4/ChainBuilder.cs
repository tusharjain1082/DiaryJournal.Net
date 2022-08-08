using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Uses Stage2 Dot tokens to find evaluable tokens that are chained together
    /// </summary>
    public class ChainBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 1; i + 1 < input.Count; i++)
            {
                if(input[i].Type == Stage2Types.Dot)
                {
                    var items = new List<EvaluableToken>();

                    int startIndex = i - 1;
                    if(!(input[i - 1] is EvaluableToken))
                        throw new SyntaxException("Invalid use of dot operator");
                    items.Add(input[i - 1] as EvaluableToken);

                    int endIndex = i + 1;
                    for(int j = i; j + 1 < input.Count; j += 2)
                    {
                        if (input[j].Type != Stage2Types.Dot)
                            break;
                        if (!(input[j + 1] is EvaluableToken))
                            throw new SyntaxException("Invalid use of dot operator");
                        items.Add(input[j + 1] as EvaluableToken);
                        endIndex = j + 1;
                    }

                    input[startIndex] = new ChainToken(items);
                    input.RemoveRange(startIndex + 1, endIndex - startIndex);
                }
            }
        }
    }
}
