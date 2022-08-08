using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    public static class TokenExtensions
    {
        public static bool IsStage2(this Token token) => Stage2Types.IsStage2(token.Type);

        public static bool IsStage2Literal(this Token token) => Stage2Types.IsStage2Literal(token.Type);

        public static bool IsStage2BinaryOperator(this Token token) => Stage2Types.IsStage2BinaryOperator(token.Type);

        public static bool IsStage2Comparison(this Token token) => Stage2Types.IsStage2Comparison(token.Type);

        public static bool IsStage2Symbol(this Token token) => Stage2Types.IsStage2Symbol(token.Type);

        public static bool IsStage2Name(this Token token) => Stage2Types.IsStage2Name(token.Type);

        public static bool IsStage2UnaryOperator(this Token token) => Stage2Types.IsStage2UnaryOperator(token.Type);
    }
}
