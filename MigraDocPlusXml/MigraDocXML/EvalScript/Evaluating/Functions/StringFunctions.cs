using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Evaluating.Functions
{
    public static class StringFunctions
    {
        public static string Trim(object[] args)
        {
            if (args.Length != 1 || !(args[0] is string))
                throw new Exception("Trim expects one argument of type string");
            return (args[0] as string).Trim();
        }


        public static string ToUpper(object[] args)
        {
            if (args.Length != 1 || !(args[0] is string))
                throw new Exception("ToUpper expects one argument of type string");
            return (args[0] as string).ToUpper();
        }


        public static string ToLower(object[] args)
        {
            if (args.Length != 1 || !(args[0] is string))
                throw new Exception("ToLower expects one argument of type string");
            return (args[0] as string).ToLower();
        }


        public static string ToTitle(object[] args)
        {
            if (args.Length != 1 || !(args[0] is string))
                throw new Exception("ToTitle expects one argument of type string");
            char[] chars = (args[0] as string).ToLower().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 0 || char.IsWhiteSpace(chars[i - 1]))
                    chars[i] = char.ToUpper(chars[i]);
            }
            return new string(chars);
        }


        public static string Substring(object[] args)
        {
            if (args.Length < 2 || args.Length > 3)
                throw new Exception("Substring expects arguments: sourceString, startIndex, [length = -1]");

            string sourceString = args[0].ToString();
            int startIndex = Convert.ToInt32(args[1]);
            if (args.Length == 2)
                return sourceString.Substring(startIndex);
            int length = (args.Length > 2) ? Convert.ToInt32(args[2]) : -1;
            return sourceString.Substring(startIndex, length);
        }


        public static string Replace(object[] args)
        {
            if (args.Length != 3)
                throw new Exception("Replace expects 3 arguments: sourceString, findString, replaceString");

            string sourceString = args[0].ToString();
            string findString = args[1].ToString();
            string replaceString = args[2].ToString();
            return sourceString.Replace(findString, replaceString);
        }


        public static object StartsWith(object[] args)
        {
            if (args.Length != 2)
                throw new Exception("StartsWith expects 2 arguments: text, find");

            return args[0].ToString().StartsWith(args[1].ToString());
        }


        public static object EndsWith(object[] args)
        {
            if (args.Length != 2)
                throw new Exception("EndsWith expects 2 arguments: text, find");

            return args[0].ToString().EndsWith(args[1].ToString());
        }


        public static object Contains(object[] args)
        {
			if (args.Length == 2 && args[0] is IEnumerable && !(args[0] is string))
				return EnumerableFunctions.Contains(args);
            if (args.Length != 2)
                throw new Exception("Contains expects 2 arguments: text, find");

            return args[0].ToString().Contains(args[1].ToString());
        }


        public static string LeftBrace(object[] args)
        {
            if (args.Length != 0)
                throw new Exception("LeftBrace expects no arguments");
            return "{";
        }


        public static string RightBrace(object[] args)
        {
            if (args.Length != 0)
                throw new Exception("RightBrace expects no arguments");
            return "}";
        }


        public static string NewLine(object[] args)
        {
            if (args.Length != 0)
                throw new Exception("NewLine expects no arguments");
            return Environment.NewLine;
        }


        public static string PadLeft(object[] args)
        {
            if (args.Length != 3)
                throw new Exception("PadLeft expects 3 arguments: object text, int padLength, char padChar");
            string text = args[0].ToString();
            int padLength = Convert.ToInt32(args[1]);
            string padChar = args[2].ToString();
            if (padChar.Length != 1)
                throw new Exception("PadLeft expects the 3rd argument to be a char or string of length 1");
            return text.PadLeft(padLength, padChar[0]);
        }


        public static string PadRight(object[] args)
        {
            if (args.Length != 3)
                throw new Exception("PadRight expects 3 arguments: object text, int padLength, char padChar");
            string text = args[0].ToString();
            int padLength = Convert.ToInt32(args[1]);
            string padChar = args[2].ToString();
            if (padChar.Length != 1)
                throw new Exception("PadRight expects the 3rd argument to be a char or string of length 1");
            return text.PadRight(padLength, padChar[0]);
        }


        public static string Pad(object[] args)
        {
            if (args.Length != 3)
                throw new Exception("Pad expects 3 arguments: object text, int padLength, char padChar");
            string text = args[0].ToString();
            int padLength = Convert.ToInt32(args[1]);
            string padChar = args[2].ToString();
            if (padChar.Length != 1)
                throw new Exception("Pad expects the 3rd argument to be a char or string of length 1");
            if (text.Length < padLength)
            {
                int paddingReqd = padLength - text.Length;
                return new string(padChar[0], (int)Math.Ceiling(paddingReqd / 2.0)) + text + new string(padChar[0], (int)Math.Floor(paddingReqd / 2.0));
            }
            return text;
        }


        public static string FormatStr(object[] args)
        {
            if (args.Length != 2)
                throw new Exception("Format expects 2 arguments: object value, string formatText");
            return Format(args[0], args[1].ToString());
        }

        public static string Format(object value, string formatText)
        {
            if (value == null)
                return "";
            return ((dynamic)value).ToString(formatText);
        }


        public static string TruncateStart(object[] args)
        {
            if (args.Length != 2)
                throw new Exception("TruncateStart expects 2 arguments: object text, int length");
            string text = args[0].ToString();
            int length = Convert.ToInt32(args[1]);
            if (text.Length > length)
                return text.Substring(text.Length - length);
            return text;
        }


        public static string TruncateEnd(object[] args)
        {
            if (args.Length != 2)
                throw new Exception("TruncateEnd expects 2 arguments: object text, int length");
            string text = args[0].ToString();
            int length = Convert.ToInt32(args[1]);
            if (text.Length > length)
                return text.Substring(0, length);
            return text;
        }


        public static string Space(object[] args)
        {
            if (args.Length > 1)
                throw new Exception("Space accepts 1 optional argument: int length");
            int length = 1;
            if (args.Length == 1)
                length = Convert.ToInt32(args[0]);
            if (length <= 0)
                throw new Exception("The length argument of Space must be greater than 0");
            return new string(Convert.ToChar(160), length);
        }


        public static object IsNullOrEmpty(object[] args)
        {
            if (args.Length != 1)
                throw new Exception("IsNullOrEmpty expects one argument");
            if (args[0] == null)
                return true;
            string str = args[0].ToString();
            return str == "";
        }

        public static object IsNullOrWhitespace(object[] args)
        {
            if (args.Length != 1)
                throw new Exception("IsNullOrWhitespace expects one argument");
            if (args[0] == null)
                return true;
            string str = args[0].ToString();
            return str.Trim() == "";
        }

		public static object Char(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("Char expects one argument: int utf8Code");
			return ((char)Convert.ToInt32(args[0])).ToString();
		}

        public static object IndexOf(object[] args)
        {
            if (args.Length < 2 || args.Length > 3 || args[0] == null || !(args[0] is string) || args[1] == null)
                throw new Exception("IndexOf expects arguments: string text, string find, [int startIndex = -1]");
            int startIndex = -1;
            if (args.Length == 3)
                startIndex = Convert.ToInt32(args[2]);

            if (startIndex < 0)
                return args[0].ToString().IndexOf(args[1].ToString());
            else
                return args[0].ToString().IndexOf(args[1].ToString(), startIndex);
		}

		public static object LastIndexOf(object[] args)
		{
			if (args.Length < 2 || args.Length > 3 || args[0] == null || !(args[0] is string) || args[1] == null)
				throw new Exception("LastIndexOf expects arguments: string text, string find, [int startIndex = -1]");
			int startIndex = -1;
			if (args.Length == 3)
				startIndex = Convert.ToInt32(args[2]);

			if (startIndex < 0)
				return args[0].ToString().LastIndexOf(args[1].ToString());
			else
				return args[0].ToString().LastIndexOf(args[1].ToString(), startIndex);
		}

		public static object Split(object[] args)
        {
            if (args.Length < 2 || args.Any(x => x == null))
                throw new Exception("Split expects arguments: string text, string splitter1, [string splitter2], [string splitter3]...");
            string text = args[0].ToString();
            string[] splitters = new string[args.Length - 1];
            for(int i = 1; i < args.Length; i++)
                splitters[i - 1] = args[i].ToString();
            return text.Split(splitters, StringSplitOptions.None).ToList();
        }

		public static object Insert(object[] args)
		{
			if (args.Length != 3 || args.Any(x => x == null))
				throw new Exception("Insert expects arguments: string text, int insertIndex, string insertText");
			string text = args[0].ToString();
			int insertIndex = Convert.ToInt32(args[1]);
			string insertText = args[2].ToString();
			return text.Insert(insertIndex, insertText);
		}

		public static object Remove(object[] args)
		{
			if (args.Length < 2 || args.Length > 3 || args.Any(x => x == null))
				throw new Exception("Remove expects arguments: string text, int startIndex, [int count = -1]");
			string text = args[0].ToString();
			int startIndex = Convert.ToInt32(args[1]);
			int count = -1;
			if (args.Length == 3)
				count = Convert.ToInt32(args[2]);
			if (args.Length == 2)
				return text.Remove(startIndex);
			else
				return text.Remove(startIndex, count);
		}
    }
}
