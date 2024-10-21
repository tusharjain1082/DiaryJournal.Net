using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage1
{
    /// <summary>
    /// Split the list of tokens into even smaller tokens, where each split occurs at the occurence of a reserved character
    /// </summary>
    public class SymbolSeparator
    {
        private Dictionary<char, int> _reservedChars = new Dictionary<char, int>()
        {
            { '-', Stage1Types.Hyphen },
            { '(', Stage1Types.LeftBracket },
            { ')', Stage1Types.RightBracket },
            { '<', Stage1Types.LeftAngleBracket },
            { '>', Stage1Types.RightAngleBracket },
            { '[', Stage1Types.LeftSquareBracket },
            { ']', Stage1Types.RightSquareBracket },
            { '+', Stage1Types.Plus },
            { '*', Stage1Types.Asterisk },
            { '/', Stage1Types.ForwardSlash },
            { '^', Stage1Types.Caret },
            { '?', Stage1Types.QuestionMark },
            { '!', Stage1Types.ExclamationMark },
            { '.', Stage1Types.Dot },
            { ',', Stage1Types.Comma },
            { ':', Stage1Types.Colon },
            { '%', Stage1Types.Percent },
            { '=', Stage1Types.Equal },
            { '&', Stage1Types.Ampersand },
            { '|', Stage1Types.Pipe }
        };

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
            List<Token> output = new List<Token>();

            //The index which the current block started at
            int startI = 0;

            //Loop through each character in string
            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];

                //If current char is reserved, optionally add a Token for previous text, add new token for reserved char
                if (_reservedChars.ContainsKey(chr))
                {
                    if (startI < i)
                        output.Add(new Token(Stage1Types.Text, text.Substring(startI, i - startI)));
                    output.Add(new Token(_reservedChars[chr]));
                    startI = i + 1;
                }
            }

            //Add any text at end of string as an UnknownToken
            if (startI < text.Length)
                output.Add(new Token(Stage1Types.Text, text.Substring(startI, text.Length - startI)));

            return output;
        }
    }
}
