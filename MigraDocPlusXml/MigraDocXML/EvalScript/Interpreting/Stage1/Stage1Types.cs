using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage1
{
    public static class Stage1Types
    {
        //Literals
        public const int StringLiteral = 1;

        //Symbols
        public const int Hyphen = 101;
        public const int LeftBracket = 102;
        public const int RightBracket = 103;
        public const int LeftAngleBracket = 104;
        public const int RightAngleBracket = 105;
        public const int LeftSquareBracket = 106;
        public const int RightSquareBracket = 107;
        public const int Plus = 108;
        public const int Asterisk = 110;
        public const int ForwardSlash = 111;
        public const int Caret = 112;
        public const int QuestionMark = 113;
        public const int ExclamationMark = 114;
        public const int Dot = 115;
        public const int Comma = 116;
        public const int Colon = 117;
        public const int Percent = 118;
        public const int Equal = 119;
        public const int Ampersand = 120;
        public const int Pipe = 121;

        //Text/whitespace
        public const int Text = 201;
        public const int Whitespace = 202;

        public static bool IsStage1(int type) => type < 1000;
        public static bool IsStage1Symbol(int type) => type >= 100 && type < 200;


        public static string ToString(Token token)
        {
            switch (token.Type)
            {
                case Hyphen: return "-";
                case LeftBracket: return "(";
                case RightBracket: return ")";
                case LeftAngleBracket: return "<";
                case RightAngleBracket: return ">";
                case LeftSquareBracket: return "[";
                case RightSquareBracket: return "]";
                case Plus: return "+";
                case Asterisk: return "*";
                case ForwardSlash: return "/";
                case Caret: return "^";
                case QuestionMark: return "?";
                case ExclamationMark: return "!";
                case Dot: return ".";
                case Comma: return ",";
                case Colon: return ":";
                case Percent: return "%";
                case Equal: return "=";
                case Text: return token.Value.ToString();
                case Whitespace: return " ";
                default: throw new ArgumentException("Unrecognised token type");
            }
        }
    }
}
