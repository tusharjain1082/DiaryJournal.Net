using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage3
{
    public static class TokenExtensions
    {
        public static bool IsStage3(this Token token) => Stage3Types.IsStage3(token.Type);
    }
}
