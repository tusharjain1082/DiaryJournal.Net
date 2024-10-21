using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Identify Text tokens which are actually numeric and should be treated as numeric literals
    /// If the token is followed by a dot, then another numeric text token, then they are joined together
    /// </summary>
    public class NumericLiteralIdentifier
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                var token = input[i];
                if(token.Type == Stage1Types.Text)
                {
                    var text = token.Value.ToString();
                    if(text.Length > 0 && char.IsNumber(text[0]))
                    {
                        //Figure out what text to treat as part of the numeric text
                        string numberText = text;
                        bool useDecimalPoint = false;
                        if(numberText.Count(x => char.IsNumber(x)) == numberText.Length)
                        {
                            if(i + 2 < input.Count && input[i + 1].Type == Stage1Types.Dot)
                            {
                                if(input[i + 2].Type == Stage1Types.Text)
                                {
                                    var iPlus2Text = input[i + 2].Value.ToString();
                                    if (iPlus2Text.Length > 0 && char.IsNumber(iPlus2Text[0]))
                                    {
                                        numberText += "." + iPlus2Text;
                                        useDecimalPoint = true;
                                    }
                                }
                            }
                        }

                        //Determine if a specifier was used, if not, default to 'D'/' ' depending on whether a decimal point is being used
                        char specifier = numberText[numberText.Length - 1];
                        if (char.IsLetter(specifier))
                            numberText = numberText.Remove(numberText.Length - 1);
                        else
                        {
                            if (useDecimalPoint)
                                specifier = 'D';
                            else
                                specifier = ' ';
                        }

                        //Get the value of the numeric literal
                        object value = null;
                        try
                        {
                            if (specifier == 'D')
                                value = double.Parse(numberText, CultureInfo.InvariantCulture);
                            else if (specifier == 'L')
                                value = long.Parse(numberText);
                            else if (specifier == 'F')
                                value = float.Parse(numberText, CultureInfo.InvariantCulture);
                            else if (specifier == 'M')
                                value = decimal.Parse(numberText, CultureInfo.InvariantCulture);
                            else if (specifier == ' ')
                                value = int.Parse(numberText);
                        }
                        catch
                        {
                            throw new SyntaxException("Unrecognised numeric literal");
                        }
                        if (value == null)
                            throw new SyntaxException("Unrecognised numeric literal");

                        //Create the numeric literal token
                        input[i] = new Token(Stage2Types.NumericLiteral, value);
                        if (useDecimalPoint)
                            input.RemoveRange(i + 1, 2);
                    }
                }
            }
        }
    }
}
