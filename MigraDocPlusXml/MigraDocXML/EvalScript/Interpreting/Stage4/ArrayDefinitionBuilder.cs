using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Identifies array definitions by finding square brackets that contain entirely comma separated evaluable tokens
    /// </summary>
    public class ArrayDefinitionBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage3Types.SquareBrackets)
                {
                    var children = (token as ParentToken).Children;
                    if(children.Count == 0 || children.Count % 2 == 1)
                    {
                        var items = new List<EvaluableToken>();
                        for(int j = 0; j < children.Count; j++)
                        {
                            if (j % 2 == 0)
                            {
                                var item = children[j] as EvaluableToken;
                                if (item == null)
                                    throw new SyntaxException("Invalid type in array definition");
                                items.Add(item);
                            }
                            else if (children[j].Type != Stage2Types.Comma)
                                throw new SyntaxException("Array definition must be made up of a comma separated list");
                        }

                        input[i] = new ArrayDefinitionToken(items);
                    }
                }
            }
        }
    }
}
