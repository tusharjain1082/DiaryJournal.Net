using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting.Stage3
{
    public static class Stage3Types
    {
        public const int Brackets = 2001;
        public const int SquareBrackets = 2002;
        public const int Operation = 2003;
        public const int CommaSeparated = 2004;

        public static bool IsStage3(int type) => type >= 2000 && type < 3000;


        public static string ToString(Token token)
        {
            var parent = token as ParentToken;

            switch (token.Type)
            {
                case Brackets: return "(" + string.Join("", parent.Children.Select(x => x.ToString()).ToArray()) + ")";
                case SquareBrackets: return "[" + string.Join("", parent.Children.Select(x => x.ToString()).ToArray()) + "]";
                case Operation: return string.Join("", parent.Children.Select(x => x.ToString()).ToArray());
                default: throw new ArgumentException("Unrecognised token type");
            }
        }
    }
}
