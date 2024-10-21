using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EvalScript.Evaluating.Functions;
using EvalScript.Interpreting.Stage4;

namespace EvalScript.Evaluating
{
    public class StandardEvaluator : EvaluatorBase
    {

        public bool AllowObjectMethodAccess { get; set; }


		public StandardEvaluator()
		{
			AllowObjectMethodAccess = Settings.AllowObjectMethodAccess;
		}


        protected override object RunChain(object previousObj, PropertyToken token, Func<string, object> variableLookup)
        {
            try
            {
                var propName = token.Name;
                
                if (previousObj is EvalObject)
                    return (previousObj as EvalObject)[propName];

                if (previousObj is XmlEvalObject)
                    return (previousObj as XmlEvalObject)[propName];

                var previousObjType = previousObj.GetType();
                return previousObjType.GetProperty(propName).GetValue(previousObj, null);
            }
            catch
            {
                throw new Exception("Unrecognised property name: " + token.Name);
            }
        }


        protected override object RunChain(object previousObj, IndexerCallToken token, Func<string, object> variableLookup)
        {
            try
            {
                var paramResult = Run(token.Parameter, variableLookup);

                if (previousObj is Array || previousObj is IList)
                    return EnumerableFunctions.Index(new object[2] { previousObj, paramResult });

                if (previousObj is IDictionary)
                    return (previousObj as IDictionary)[paramResult];

                if (previousObj is XmlEvalObject && paramResult is string)
                    return (previousObj as XmlEvalObject)[paramResult as string];

                var previousObjType = previousObj.GetType();
                try
                {
                    return previousObjType.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public).Invoke(previousObj, new object[] { paramResult });
                }
                catch { }

                if(paramResult is string)
                {
                    try
                    {
                        return previousObjType.GetProperty(paramResult as string, BindingFlags.Instance | BindingFlags.Public).GetValue(previousObj, null);
                    }
                    catch { }
                }

                throw new Exception("Invalid indexer");
            }
            catch
            {
                throw new Exception("Invalid indexer");
            }
        }


        protected override object RunChain(object previousObj, FunctionCallToken token, Func<string, object> variableLookup)
        {
            var args = new List<object>();
            bool hasLambda = false;
            foreach(var parameter in token.Parameters)
            {
                if (parameter is LambdaExpressionToken)
                {
                    args.Add(new EvaluableLambda(parameter as LambdaExpressionToken, this, variableLookup));
                    hasLambda = true;
                }
                else
                    args.Add(Run(parameter, variableLookup));
            }

            if (!hasLambda && AllowObjectMethodAccess)
            {
                try
                {
                    return previousObj.GetType().GetMethod(token.Name, BindingFlags.Public | BindingFlags.Instance).Invoke(previousObj, args.ToArray());
                }
                catch { }
            }

            args.Insert(0, previousObj);
            if (!Functions.ContainsKey(token.Name))
                throw new Exception("Unrecognised function name: " + token.Name);
            return Functions[token.Name](args.ToArray());
        }
    }
}
