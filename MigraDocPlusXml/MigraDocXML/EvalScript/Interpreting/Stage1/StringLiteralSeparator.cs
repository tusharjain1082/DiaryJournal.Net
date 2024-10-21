using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage1
{
    /// <summary>
    /// Split a list of tokens out into smaller tokens, where each split occurs on a string literal
    /// It's very important this be run first, since otherwise string literal text could be interpreted as code
    /// </summary>
    public class StringLiteralSeparator
    {
        public List<Token> Run(Interpreter interpreter, List<Token> input)
        {
            var output = new List<Token>();
            foreach(var token in input)
            {
                if (token.Type == Stage1Types.Text)
                    output.AddRange(ProcessText(interpreter, token.Value.ToString()));
                else
                    output.Add(token);
            }
            return output;
        }

        private List<Token> ProcessText(Interpreter interpreter, string text)
        {
            var output = new List<Token>();

            //Keep track of whether we're currently inside/outside of a string literal
            bool inString = false;
            //The index which the current block started at
            int startI = 0;

            //Loop through each character in string
            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];

                //If we're not currently in a string literal, look for an apostrophe to signify a string start
                if (!inString)
                {
                    if (chr == interpreter.StringLiteralChar)
                    {
                        //Add anything leading up to the string as a text token
                        if (i > startI)
                            output.Add(new Token(Stage1Types.Text, text.Substring(startI, i - startI)));
                        //Mark that we're in a string literal, update startI to the start of the string literal
                        inString = true;
                        startI = i;
                    }
                }
                //If we are currently in a string literal, look for a non-escaped apostrophe to signify a string end
                else
                {
                    if (chr == interpreter.StringLiteralChar && text[i - 1] != '\\')
                    {
                        //Add the string contents as a new StringLiteralToken
                        output.Add(new Token(Stage1Types.StringLiteral, text.Substring(startI + 1, i - startI - 1).Replace("\\\'", "\'")));
                        //Mark that we're not in a string, update startI to the next char which will be out of the string literal
                        inString = false;
                        startI = i + 1;
                    }
                }
            }

            //Add anything at the end of a string which is not a string literal
            if (startI < text.Length)
            {
                if (inString)
                    throw new SyntaxException("Unclosed string literal");
                output.Add(new Token(Stage1Types.Text, text.Substring(startI, text.Length - startI)));
            }

            return output;
        }
    }
}
