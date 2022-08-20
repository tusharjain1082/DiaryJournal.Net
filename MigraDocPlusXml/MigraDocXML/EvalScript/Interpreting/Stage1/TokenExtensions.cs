using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage1
{
    public static class TokenExtensions
    {
        public static bool IsStage1(this Token token) => Stage1Types.IsStage1(token.Type);

        public static bool IsStage1Symbol(this Token token) => Stage1Types.IsStage1Symbol(token.Type);
    }
}
