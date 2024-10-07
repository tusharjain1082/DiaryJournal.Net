using EvalScript.Interpreting.Stage2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage3
{
    /// <summary>
    /// Separate out tokens that are part of a comma separated list of items
    /// </summary>
    public class CommaSeparatedNester
    {
        public void Run(Interpreter interpreter, List<Token> input)
        {
            foreach (var parent in input.OfType<ParentToken>())
                Run(interpreter, parent.Children);

            if(input.Any(x => x.Type == Stage2Types.Comma))
            {
                int endIndex = input.Count - 1;
                for(int i = input.Count - 1; i >= 0; i--)
                {
                    if(input[i].Type == Stage2Types.Comma)
                    {
                        MergeListSection(input, i + 1, endIndex);
                        endIndex = i - 1;
                    }
                }

                if(endIndex >= 0)
                    MergeListSection(input, 0, endIndex);
            }
        }

        private void MergeListSection(List<Token> tokens, int startIndex, int endIndex)
        {
            int count = endIndex - startIndex + 1;
            if(count > 0)
            {
                tokens[startIndex] = new ParentToken(Stage3Types.CommaSeparated, tokens.GetRange(startIndex, count));
                if (count > 1)
                    tokens.RemoveRange(startIndex + 1, count - 1);
            }
        }
    }
}
