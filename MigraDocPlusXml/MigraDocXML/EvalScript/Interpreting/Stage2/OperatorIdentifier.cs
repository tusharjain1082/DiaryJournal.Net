using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Map reserved Stage1 symbols into Stage2 operators
    /// </summary>
    public class OperatorIdentifier
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                if (input[i].IsStage1Symbol())
                {
                    int thisType = input[i].Type;
                    int? nextType = (i + 1 < input.Count) ? (int?)input[i + 1].Type : null;

                    //LessThan & LessThanOrEqual
                    if (thisType == Stage1Types.LeftAngleBracket)
                    {
                        if (nextType == Stage1Types.Equal)
                        {
                            input[i] = new Token(Stage2Types.LessThanOrEqualOperator);
                            input.RemoveAt(i + 1);
                        }
                        else
                            input[i] = new Token(Stage2Types.LessThanOperator);
                    }

                    //GreaterThan & GreaterThanOrEqual
                    else if (thisType == Stage1Types.RightAngleBracket)
                    {
                        if (nextType == Stage1Types.Equal)
                        {
                            input[i] = new Token(Stage2Types.GreaterThanOrEqualOperator);
                            input.RemoveAt(i + 1);
                        }
                        else
                            input[i] = new Token(Stage2Types.GreaterThanOperator);
                    }

                    //Equal & Lambda
                    else if (thisType == Stage1Types.Equal)
                    {
                        if (nextType == Stage1Types.RightAngleBracket)
                        {
                            input[i] = new Token(Stage2Types.LambdaOperator);
                            input.RemoveAt(i + 1);
                        }
                        else
                        {
                            if (nextType == Stage1Types.Equal)
                                input.RemoveAt(i + 1);
                            input[i] = new Token(Stage2Types.EqualOperator);
                        }
                    }

                    //Not & NotEqual
                    else if (thisType == Stage1Types.ExclamationMark)
                    {
                        if (nextType == Stage1Types.Equal)
                        {
                            input[i] = new Token(Stage2Types.NotEqualOperator);
                            input.RemoveAt(i + 1);
                        }
                        else
                            input[i] = new Token(Stage2Types.NotOperator);
                    }

                    //Plus
                    else if (thisType == Stage1Types.Plus)
                        input[i] = new Token(Stage2Types.PlusOperator);

                    //Minus
                    else if (thisType == Stage1Types.Hyphen)
                        input[i] = new Token(Stage2Types.MinusOperator);

                    //Multiply
                    else if (thisType == Stage1Types.Asterisk)
                        input[i] = new Token(Stage2Types.MultiplyOperator);

                    //Divide
                    else if (thisType == Stage1Types.ForwardSlash)
                        input[i] = new Token(Stage2Types.DivideOperator);

                    //Power
                    else if (thisType == Stage1Types.Caret)
                        input[i] = new Token(Stage2Types.PowerOperator);

					//Lambda operator alternative
					else if(thisType == Stage1Types.Colon && nextType == Stage1Types.Colon)
					{
						input[i] = new Token(Stage2Types.LambdaOperator);
						input.RemoveAt(i + 1);
					}

                    //Key Value
                    else if (thisType == Stage1Types.Colon)
						input[i] = new Token(Stage2Types.KeyValueOperator);

                    //Null Coalesce
                    else if (thisType == Stage1Types.QuestionMark && nextType == Stage1Types.QuestionMark)
                    {
                        input[i] = new Token(Stage2Types.NullCoalesceOperator);
                        input.RemoveAt(i + 1);
                    }

                    //Modulus
                    else if (thisType == Stage1Types.Percent)
                        input[i] = new Token(Stage2Types.ModulusOperator);

                    //And
                    else if(thisType == Stage1Types.Ampersand && nextType == Stage1Types.Ampersand)
                    {
                        input[i] = new Token(Stage2Types.AndOperator);
                        input.RemoveAt(i + 1);
                    }

                    //Or
                    else if(thisType == Stage1Types.Pipe && nextType == Stage1Types.Pipe)
                    {
                        input[i] = new Token(Stage2Types.OrOperator);
                        input.RemoveAt(i + 1);
                    }
                }
                else if(input[i].Type == Stage1Types.Text)
                {
                    string text = input[i].Value.ToString();
                    string uText = text.ToUpper();
                    int? nextType = (i + 1 < input.Count) ? (int?)input[i + 1].Type : null;

					//As
					if (uText == "AS")
						input[i] = new Token(Stage2Types.AsOperator);

					//And
					else if (uText == "AND")
						input[i] = new Token(Stage2Types.AndOperator);
					
					//Or
					else if (uText == "OR")
						input[i] = new Token(Stage2Types.OrOperator);

					//GreaterThan & GreaterThanOrEqual
					else if (text == "GT")
					{
						if (nextType == Stage1Types.Equal)
						{
							input[i] = new Token(Stage2Types.GreaterThanOrEqualOperator);
							input.RemoveAt(i + 1);
						}
						else
							input[i] = new Token(Stage2Types.GreaterThanOperator);
					}

					//GreaterThan & GreaterThanOrEqual
					else if (text == "LT")
					{
						if (nextType == Stage1Types.Equal)
						{
							input[i] = new Token(Stage2Types.LessThanOrEqualOperator);
							input.RemoveAt(i + 1);
						}
						else
							input[i] = new Token(Stage2Types.LessThanOperator);
					}
                }
            }
        }
    }
}
