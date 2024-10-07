using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Builds Indexer call token by finding Stage2 IndexerName token, then using the square brackets token to the right to get the parameter
    /// </summary>
    public class IndexerCallBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 1; i < input.Count; i++)
            {
                if(input[i].Type == Stage3Types.SquareBrackets)
                {
                    if (input[i - 1] is PropertyToken || input[i - 1].Type == Stage3Types.SquareBrackets || input[i - 1].Type == Stage3Types.Brackets)
                    {
                        var bracketChildren = (input[i] as ParentToken).Children;
                        if (bracketChildren.Count == 1)
                        {
                            input[i] = new IndexerCallToken(bracketChildren[0] as EvaluableToken);
                            input.Insert(i, new Token(Stage2Types.Dot));
                        }
                        else
                            throw new SyntaxException("Invalid indexer parameter");
                    }
                }
            }
        }
    }
}
