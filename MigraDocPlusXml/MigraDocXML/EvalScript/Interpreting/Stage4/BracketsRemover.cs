using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// In places where brackets aren't preceded by a function name or proceeded by a lambda operator, replace the brackets token with its contents
    /// </summary>
    public class BracketsRemover
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                if(input[i].Type == Stage3Types.Brackets)
                {
                    if((i > 0 && input[i - 1].Type != Stage2Types.FunctionName) || (i < input.Count - 1 && input[i + 1].Type != Stage2Types.LambdaOperator))
                    {
                        var parentToken = input[i] as ParentToken;
                        input.RemoveAt(i);
                        input.InsertRange(i, parentToken.Children);
                    }
                }
            }
        }
    }
}
