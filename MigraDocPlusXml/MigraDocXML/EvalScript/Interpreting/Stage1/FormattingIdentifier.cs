using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage1
{
    /// <summary>
    /// Identify the formatting character, return all text after the character as formatting text, remove the operator and all formatting text from the list of tokens
    /// </summary>
    public class FormattingIdentifier
    {
        public string Run(Interpreter interpreter, List<Token> input)
        {
            //The format text that gets returned, if no formatting then remains as null
            string output = null;

            //Loop through all tokens, look for any Text tokens
            for(int i = 0; i < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage1Types.Text)
                {
                    //If the text contains a hash, start the process of splitting the tokens into code and formatting
                    var text = token.Value.ToString();
                    var hashIndex = text.IndexOf('#');
                    if(hashIndex >= 0)
                    {
                        output = text.Substring(hashIndex);
                        input[i] = new Token(Stage1Types.Text, text.Substring(0, hashIndex));

                        for(int j = i + 1; j < input.Count;)
                        {
                            token = input[j];
                            if (token.Type == Stage1Types.Whitespace)
                                output += " ";
                            else
                                output += token.Value.ToString();
                            input.RemoveAt(j);
                        }
                        break;
                    }
                }
            }

            if(output != null)
            {
                //Remove the hash from the start of the output text, then trim the result
                if (output.Length > 0)
                    output = output.Substring(1);
                output = output.Trim();
            }
            return output;
        }
    }
}
