using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    /// <summary>
    /// Identifies key value pairs by looking for colons, takes the token to the left as the key, and token to the right as the evaluable value
    /// </summary>
    public class KeyValuePairBuilder
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 1; i + 1 < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage2Types.KeyValueOperator)
                {
                    var left = input[i - 1];
                    var right = input[i + 1] as EvaluableToken;
                    if (left == null || right == null)
                        throw new SyntaxException("Invalid key value pair definition");

                    string key = null;
                    if (left.Type == Stage2Types.PropertyName)
                        key = left.Value.ToString();
                    else if (left is PropertyToken)
                        key = (left as PropertyToken).Name;
                    else
                        throw new SyntaxException("Invalid key value pair definition");

                    input[i - 1] = new KeyValuePairToken(key, right);
                    input.RemoveRange(i, 2);
                }
            }
        }
    }
}
