using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Identify Stage1 text tokens, replace them with either a property name, indexer name or function name token, based on the following token type
    /// </summary>
    public class NameIdentifier
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                if(input[i].Type == Stage1Types.Text)
                {
                    var text = input[i].Value.ToString();

                    //Text which is empty, starts with a numeric value or contains any characters that aren't alphanumeric or an underscore can't be converted to names
                    if (text.Length == 0)
                        continue;
                    if (char.IsNumber(text[0]))
                        continue;
                    if (text.Substring(1).Any(x => !char.IsLetter(x) && !char.IsNumber(x) && x != '_'))
                        continue;
                    if (!char.IsLetter(text[0]) && text[0] != '_' && text[0] != '~' && text[0] != '@')
                        continue;

                    //Use the next token type to figure out if the name should be that of a function call, an indexer or a property
                    int? nextType = (i + 1 < input.Count) ? (int?)input[i + 1].Type : null;

                    if (nextType == Stage1Types.LeftBracket)
                        input[i] = new Token(Stage2Types.FunctionName, text);
                    
                    else
                        input[i] = new Token(Stage2Types.PropertyName, text);
                }
            }
        }
    }
}
