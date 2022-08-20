using EvalScript.Evaluating.Functions;
using EvalScript.Interpreting;
using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Evaluating
{
    public abstract class EvaluatorBase : IEvaluator
    {
        public Dictionary<string, Func<object[], object>> Functions { get; private set; } = new Dictionary<string, Func<object[], object>>()
        {
			//String functions
			{ "Char", StringFunctions.Char },
            { "Contains", StringFunctions.Contains },
            { "EndsWith", StringFunctions.EndsWith },
            { "Format", StringFunctions.FormatStr },
            { "IndexOf", StringFunctions.IndexOf },
			{ "Insert", StringFunctions.Insert },
            { "IsNullOrEmpty", StringFunctions.IsNullOrEmpty },
            { "IsNullOrWhitespace", StringFunctions.IsNullOrWhitespace },
            { "IsNullOrWhiteSpace", StringFunctions.IsNullOrWhitespace },
			{ "LastIndexOf", StringFunctions.LastIndexOf },
            { "LeftBrace", StringFunctions.LeftBrace },
            { "NewLine", StringFunctions.NewLine },
            { "Pad", StringFunctions.Pad },
            { "PadLeft", StringFunctions.PadLeft },
            { "PadRight", StringFunctions.PadRight },
			{ "Remove", StringFunctions.Remove },
            { "Replace", StringFunctions.Replace },
            { "RightBrace", StringFunctions.RightBrace },
            { "Space", StringFunctions.Space },
            { "Split", StringFunctions.Split },
            { "StartsWith", StringFunctions.StartsWith },
            { "Substring", StringFunctions.Substring },
            { "ToLower", StringFunctions.ToLower },
            { "ToTitle", StringFunctions.ToTitle },
            { "ToUpper", StringFunctions.ToUpper },
            { "Trim", StringFunctions.Trim },
            { "TruncateEnd", StringFunctions.TruncateEnd },
            { "TruncateStart", StringFunctions.TruncateStart },

			//Enumerable functions
            { "Any", EnumerableFunctions.Any },
            { "All", EnumerableFunctions.All },
            { "Count", EnumerableFunctions.Count },
			{ "Distinct", EnumerableFunctions.Distinct },
            { "First", EnumerableFunctions.First },
            { "FirstOrNull", EnumerableFunctions.FirstOrNull },
            { "Group", EnumerableFunctions.Group },
			{ "GroupBy", EnumerableFunctions.Group },
            { "Index", EnumerableFunctions.Index },
            { "Join", EnumerableFunctions.Join },
            { "Last", EnumerableFunctions.Last },
            { "LastOrNull", EnumerableFunctions.LastOrNull },
			{ "Map", EnumerableFunctions.Select },
			{ "MapMany", EnumerableFunctions.SelectMany },
            { "MergeToString", EnumerableFunctions.MergeToString },
            { "Order", EnumerableFunctions.Order },
			{ "OrderBy", EnumerableFunctions.Order },
            { "OrderDesc", EnumerableFunctions.OrderDesc },
			{ "OrderByDesc", EnumerableFunctions.OrderDesc },
			{ "Range", EnumerableFunctions.Range },
            { "Reverse", EnumerableFunctions.Reverse },
			{ "Partition", EnumerableFunctions.Partition },
            { "Select", EnumerableFunctions.Select },
			{ "SelectMany", EnumerableFunctions.SelectMany },
            { "Sum", EnumerableFunctions.Sum },
            { "Where", EnumerableFunctions.Where },

			//DateTime functions
            { "AddYears", DateFunctions.AddYears },
            { "AddMonths", DateFunctions.AddMonths },
            { "AddDays", DateFunctions.AddDays },
            { "AddHours", DateFunctions.AddHours },
            { "AddMinutes", DateFunctions.AddMinutes },
            { "AddSeconds", DateFunctions.AddSeconds },
            { "AddMilliseconds", DateFunctions.AddMilliseconds },
            { "DateTime", DateFunctions.DateTime },
            { "Now", DateFunctions.Now },

			//Numeric functions
            { "Abs", NumericFunctions.Abs },
            { "Ceiling", NumericFunctions.Ceiling },
            { "Floor", NumericFunctions.Floor },
            { "Min", NumericFunctions.Min },
            { "Max", NumericFunctions.Max },
            { "Round", NumericFunctions.Round },
			{ "RandomInt", NumericFunctions.RandomInt },
			{ "RandomDouble", NumericFunctions.RandomDouble },

			//Misc functions
            { "Clone", MiscFunctions.Clone },
			{ "GetProperties", MiscFunctions.GetProperties },
			{ "GetMethods", MiscFunctions.GetMethods },
            { "Run", MiscFunctions.Run },
			{ "Try", MiscFunctions.Try },
            { "TypeName", MiscFunctions.TypeName },
			{ "LoadCsvData", MiscFunctions.LoadCsvData },
			{ "LoadCsvFile", MiscFunctions.LoadCsvFile },
			{ "LoadJsonData", MiscFunctions.LoadJsonData },
			{ "LoadJsonFile", MiscFunctions.LoadJsonFile },
			{ "LoadXmlData", MiscFunctions.LoadXmlData },
			{ "LoadXmlFile", MiscFunctions.LoadXmlFile },
		};


        public virtual object Run(EvaluableToken token, Func<string, object> variableLookup)
        {
            dynamic dynToken = token;
            return Run(dynToken, variableLookup);
        }

        protected virtual object Run(LiteralToken token, Func<string, object> variableLookup)
        {
            return token.Value;
        }

        protected virtual object Run(UnaryOperationToken token, Func<string, object> variableLookup)
        {
            switch (token.Type)
            {
                case Stage2Types.NegativeOperator:
                    return -((dynamic)Run(token.Operand, variableLookup));

                case Stage2Types.NotOperator:
                    return !((dynamic)Run(token.Operand, variableLookup));

                default:
                    throw new SyntaxException($"Invalid unary operation: {new Token(token.Type).ToString()}");
            }
        }

        protected virtual object Run(TernaryOperationToken token, Func<string, object> variableLookup)
        {
            var testResult = Run(token.Test, variableLookup);

            bool boolVal;
            if (testResult is bool)
                boolVal = (bool)testResult;
            else
                boolVal = (bool)Convert.ChangeType(testResult, typeof(bool));

            if (boolVal)
                return Run(token.TrueResult, variableLookup);
            else
                return Run(token.FalseResult, variableLookup);
        }

        protected virtual object Run(BinaryOperationToken token, Func<string, object> variableLookup)
        {
            //First do operations where we don't know if we'll need the right operand
            dynamic l = Run(token.Left, variableLookup);
            switch (token.Type)
            {
                case Stage2Types.AndOperator:
                    if (l != true)
                        return false;
                    return (dynamic)Run(token.Right, variableLookup) == true;

                case Stage2Types.OrOperator:
                    if (l == true)
                        return true;
                    return (dynamic)Run(token.Right, variableLookup) == true;

                case Stage2Types.NullCoalesceOperator:
                    if (l != null)
                        return l;
                    return Run(token.Right, variableLookup);

                case Stage2Types.AsOperator:
                    string asTarget = token.Right.ToString();
                    switch (asTarget)
                    {
                        case "DateTime":
                            return Convert.ToDateTime(l);
                        case "decimal":
                            return Convert.ToDecimal(l);
                        case "double":
                            return Convert.ToDouble(l);
                        case "float":
                            return Convert.ToSingle(l);
                        case "int":
                            return Convert.ToInt32(l);
                        case "long":
                            return Convert.ToInt64(l);
                        case "short":
                            return Convert.ToInt16(l);
                        case "string":
                            return Convert.ToString(l);
                        default:
                            throw new Exception("Unrecognised conversion type: " + asTarget);
                    }

            }

            //Now do all operations where both operands are required
            dynamic r = Run(token.Right, variableLookup);
            switch (token.Type)
            {
                case Stage2Types.DivideOperator:
                    if (l == null || r == null)
                        return null;
                    return Operations.Divide.Run(l, r);

                case Stage2Types.EqualOperator:
                    return Operations.Equal.Run(l, r);

                case Stage2Types.GreaterThanOperator:
                    if (l == null || r == null)
                        return false;
                    return Operations.GreaterThan.Run(l, r);

                case Stage2Types.GreaterThanOrEqualOperator:
                    if (l == null || r == null)
                        return l == r;
                    return Operations.GreaterThanOrEqual.Run(l, r);

                case Stage2Types.LessThanOperator:
                    if (l == null || r == null)
                        return false;
					return Operations.LessThan.Run(l, r);

                case Stage2Types.LessThanOrEqualOperator:
                    if (l == null || r == null)
                        return l == r;
                    return Operations.LessThanOrEqual.Run(l, r);

                case Stage2Types.MinusOperator:
                    if (l == null || r == null)
                        return null;
                    return Operations.Subtract.Run(l, r);

                case Stage2Types.ModulusOperator:
                    if (l == null || r == null)
                        return null;
                    return Operations.Modulus.Run(l, r);

                case Stage2Types.MultiplyOperator:
                    if (l == null || r == null)
                        return null;
                    return Operations.Multiply.Run(l, r);

                case Stage2Types.NotEqualOperator:
                    return !Operations.Equal.Run(l, r);

                case Stage2Types.PlusOperator:
					if (l is string || r is string || l is char? || r is char?)
					{
					}
					else if (l == null || r == null)
						return null;
                    return Operations.Plus.Run(l, r);

                case Stage2Types.PowerOperator:
                    if (l == null || r == null)
                        return null;
                    return Math.Pow((double)l, (double)r);

                default:
                    throw new SyntaxException($"Invalid binary operation: {new Token(token.Type).ToString()}");
            }
        }

        protected virtual object Run(ArrayDefinitionToken token, Func<string, object> variableLookup)
        {
            var tokenItems = token.Items.ToArray();
            var output = new object[tokenItems.Length];
            for (int i = 0; i < tokenItems.Length; i++)
                output[i] = Run(tokenItems[i], variableLookup);
            return output;
        }

        protected virtual object Run(DictionaryDefinitionToken token, Func<string, object> variableLookup)
        {
            var output = new EvalObject();
            foreach (var keyValue in token.Items)
                output[keyValue.Key] = Run(keyValue.ValueToken, variableLookup);
            return output;
        }

        protected virtual object Run(KeyValuePairToken token, Func<string, object> variableLookup)
        {
            return new KeyValuePair<string, object>(token.Key, Run(token.ValueToken, variableLookup));
        }

        protected virtual object Run(PropertyToken token, Func<string, object> variableLookup)
        {
            return variableLookup(token.Name);
        }

        protected virtual object Run(FormattingToken token, Func<string, object> variableLookup)
        {
            return StringFunctions.Format(Run(token.Input, variableLookup), token.Format);
        }

        protected virtual object Run(FunctionCallToken token, Func<string, object> variableLookup)
        {
            if (!Functions.ContainsKey(token.Name))
                throw new SyntaxException("Unrecognised function name: " + token.Name);

			var paramList = token.Parameters.ToList();
            var args = new object[paramList.Count];
			for(int i = 0; i < token.Parameters.Count(); i++)
			{
				var parameter = paramList[i];
				if (parameter is LambdaExpressionToken)
					args[i] = new EvaluableLambda(parameter as LambdaExpressionToken, this, variableLookup);
				else
					args[i] = Run(parameter, variableLookup);
			}

            return Functions[token.Name](args);
        }

        protected virtual object Run(LambdaExpressionToken token, Func<string, object> variableLookup)
        {
            return new EvaluableLambda(token, this, variableLookup);
        }

        protected virtual object Run(ChainToken token, Func<string, object> variableLookup)
        {
            var items = token.Items.ToList();
            var output = Run(items[0], variableLookup);
            for (int i = 1; i < items.Count; i++)
            {
                if (output == null)
                    return null;

                output = RunChain(output, (dynamic)items[i], variableLookup);
            }
            return output;
        }

        protected abstract object RunChain(object previousObj, PropertyToken token, Func<string, object> variableLookup);

        protected abstract object RunChain(object previousObj, IndexerCallToken token, Func<string, object> variableLookup);

        protected abstract object RunChain(object previousObj, FunctionCallToken token, Func<string, object> variableLookup);

    }
}
