using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Interpreting.Stage2
{
    public static class Stage2Types
    {
        //Literals
        public const int StringLiteral = 1001;
        public const int NullLiteral = 1002;
        public const int NumericLiteral = 1003;
        public const int BooleanLiteral = 1004;

        //Binary operators
        public const int PlusOperator = 1101;
        public const int MinusOperator = 1102;
        public const int MultiplyOperator = 1103;
        public const int DivideOperator = 1104;
        public const int PowerOperator = 1105;
        public const int NullCoalesceOperator = 1106;
        public const int ModulusOperator = 1107;
        public const int AsOperator = 1109;
        public const int LambdaOperator = 1110;

        //Comparison operators
        public const int EqualOperator = 1201;
        public const int NotEqualOperator = 1202;
        public const int LessThanOperator = 1203;
        public const int LessThanOrEqualOperator = 1204;
        public const int GreaterThanOperator = 1205;
        public const int GreaterThanOrEqualOperator = 1206;
        public const int AndOperator = 1207;
        public const int OrOperator = 1208;
        public const int KeyValueOperator = 1209;

        //Misc symbols
        public const int Dot = 1301;
        public const int LeftBracket = 1302;
        public const int RightBracket = 1303;
        public const int LeftSquareBracket = 1304;
        public const int RightSquareBracket = 1305;
        public const int Comma = 1306;
        public const int QuestionMark = 1307;

        //Names
        public const int PropertyName = 1401;
        public const int FunctionName = 1402;

        //Unary operators
        public const int NotOperator = 1501;
        public const int NegativeOperator = 1502;

        public static bool IsStage2(int type) => type >= 1000 && type < 2000;
        public static bool IsStage2Literal(int type) => type >= 1000 && type < 1100;
        public static bool IsStage2BinaryOperator(int type) => type >= 1100 && type < 1300;
        public static bool IsStage2Comparison(int type) => type >= 1200 && type < 1300;
        public static bool IsStage2Symbol(int type) => type >= 1300 && type < 1400;
        public static bool IsStage2Name(int type) => type >= 1400 && type < 1500;
        public static bool IsStage2UnaryOperator(int type) => type >= 1500 && type < 1600;


        public static string ToString(Token token)
        {
            switch (token.Type)
            {
                case StringLiteral: return token.Value.ToString();
                case NullLiteral: return "null";
                case NumericLiteral: return token.Value.ToString();
                case BooleanLiteral: return ((bool)token.Value == true) ? "true" : "false";
                case PlusOperator: return "+";
                case MinusOperator: return "-";
                case MultiplyOperator: return "*";
                case DivideOperator: return "/";
                case PowerOperator: return "^";
                case NullCoalesceOperator: return "??";
                case ModulusOperator: return "%";
                case AsOperator: return "as";
                case LambdaOperator: return "=>";
                case EqualOperator: return "=";
                case NotEqualOperator: return "!=";
                case LessThanOperator: return "<";
                case LessThanOrEqualOperator: return "<=";
                case GreaterThanOperator: return ">";
                case GreaterThanOrEqualOperator: return ">=";
                case AndOperator: return "AND";
                case OrOperator: return "OR";
                case Dot: return ".";
                case LeftBracket: return "(";
                case RightBracket: return ")";
                case LeftSquareBracket: return "[";
                case RightSquareBracket: return "]";
                case Comma: return ",";
				case QuestionMark: return "?";
                case KeyValueOperator: return ":";
                case PropertyName: return token.Value.ToString();
                case FunctionName: return token.Value.ToString();
                case NotOperator: return "!";
                case NegativeOperator: return "-";
                default: throw new ArgumentException("Unrecognised token type");
            }
        }
    }
}
