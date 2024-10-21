using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage4
{
    public static class TokenExtensions
    {
        public static bool IsStage4(this Token token) => Stage4Types.IsStage4(token.Type);
    }
}
