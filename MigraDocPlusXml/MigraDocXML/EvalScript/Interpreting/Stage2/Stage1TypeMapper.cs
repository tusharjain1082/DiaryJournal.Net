using EvalScript.Interpreting.Stage1;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    /// <summary>
    /// Map a subset of the Stage1 types to Stage2 types
    /// </summary>
    public class Stage1TypeMapper
    {
        private Dictionary<int, int> _map = new Dictionary<int, int>()
        {
            { Stage1Types.StringLiteral, Stage2Types.StringLiteral },
            { Stage1Types.Dot, Stage2Types.Dot },
            { Stage1Types.LeftBracket, Stage2Types.LeftBracket },
            { Stage1Types.RightBracket, Stage2Types.RightBracket },
            { Stage1Types.LeftSquareBracket, Stage2Types.LeftSquareBracket },
            { Stage1Types.RightSquareBracket, Stage2Types.RightSquareBracket },
            { Stage1Types.Comma, Stage2Types.Comma },
            { Stage1Types.QuestionMark, Stage2Types.QuestionMark }
        };


        public void Run(Interpreter interpreter, List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                var type = input[i].Type;
                if (Stage1Types.IsStage1(type))
                {
                    if (_map.ContainsKey(type))
                        input[i] = new Token(_map[type], input[i].Value);
                    else
                        throw new SyntaxException($"Unrecognised code token: {input[i].ToString()}");
                }
            }
        }
    }
}
