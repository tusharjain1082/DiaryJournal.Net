using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage1
{
    /// <summary>
    /// Split out a list of tokens into smaller tokens, where each split occurs at one or more whitespace character
    /// </summary>
    public class WhitespaceSeparator
    {
        public List<Token> Run(Interpreter interpreter, List<Token> input)
        {
            var output = new List<Token>();
            foreach(var token in input)
            {
                if (token.Type == Stage1Types.Text)
                    output.AddRange(ProcessText(token.Value.ToString()));
                else
                    output.Add(token);
            }
            return output;
        }

        private List<Token> ProcessText(string text)
        {
            var output = new List<Token>();

            //Keep track of whether we're currently in whitespace or not
            bool inWhitespace = (text.Length > 0 && char.IsWhiteSpace(text[0]));
            //The index which the current block started at
            int startI = 0;

            //Loop through each character in string
            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];

                //If we're not currently in whitespace, look for when whitespace starts
                if (!inWhitespace)
                {
                    if (char.IsWhiteSpace(chr))
                    {
                        //Add anything leading up to the whitespace as text
                        if (i > startI)
                            output.Add(new Token(Stage1Types.Text, text.Substring(startI, i - startI)));
                        //Mark that we're in whitespace, update startI to the start of the whitespace
                        inWhitespace = true;
                        startI = i;
                    }
                }
                //If we are currently in whitespace, look for when whitespace stops
                else
                {
                    if (!char.IsWhiteSpace(chr))
                    {
                        output.Add(new Token(Stage1Types.Whitespace));
                        inWhitespace = false;
                        startI = i;
                    }
                }
            }

            //Add anything at the end of the string which hasn't been added yet
            if (startI < text.Length)
            {
                if (inWhitespace)
                    output.Add(new Token(Stage1Types.Whitespace));
                else
                    output.Add(new Token(Stage1Types.Text, text.Substring(startI, text.Length - startI)));
            }

            return output;
        }
    }
}
