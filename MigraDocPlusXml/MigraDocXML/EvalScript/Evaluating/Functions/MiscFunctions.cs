using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace EvalScript.Evaluating.Functions
{
    public static class MiscFunctions
    {
        public static object TypeName(object[] args)
        {
            if (args.Length != 1)
                throw new Exception("TypeName expects 1 argument");

            return args[0].GetType().FullName;
        }

		public static object Try(object[] args)
		{
			if(args.Length == 0 || args.Length > 2 || !(args[0] is EvaluableLambda) || (args.Length == 2 && !(args[1] is EvaluableLambda)))
				throw new Exception("Try expects parameters: LambdaExpression expression, LambdaExpression onFailure");
			EvaluableLambda expression = args[0] as EvaluableLambda;
			EvaluableLambda catchLambda = (args.Length < 2) ? null : args[1] as EvaluableLambda;
			if (expression.Token.Parameters.Count() != 0)
				throw new Exception("Try expression must be a lambda expression that takes 0 parameters");
			if (catchLambda != null && catchLambda.Token.Parameters.Count() != 0)
				throw new Exception("Try catch must be a lambda expression that takes 0 parameters");
			
			try
			{
				return expression.Evaluator.Run(expression.Token.Body, expression.VariableLookup);
			}
			catch
			{
				return catchLambda?.Evaluator.Run(catchLambda.Token.Body, expression.VariableLookup);
			}
		}

		public static List<EvalObject> LoadCsvData(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("LoadCsvData expects 1 argument: data");
			return EvalObject.PopulateFromCsv(args[0].ToString());
		}

		public static List<EvalObject> LoadCsvFile(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("LoadCsvFile expects 1 argument: filepath");
			return EvalObject.PopulateFromCsv(System.IO.File.ReadAllText(args[0].ToString()));
		}

		public static EvalObject LoadJsonData(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("LoadJsonData expects 1 argument: data");
			return EvalObject.PopulateFromJson(args[0].ToString());
		}

		public static EvalObject LoadJsonFile(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("LoadJsonFile expects 1 argument: filepath");
			return EvalObject.PopulateFromJson(System.IO.File.ReadAllText(args[0].ToString()));
		}

		public static XmlEvalObject LoadXmlData(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("LoadXmlData expects 1 argument: data");
			return new XmlEvalObject(XElement.Parse(args[0].ToString()));
		}

		public static XmlEvalObject LoadXmlFile(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("LoadXmlFile expects 1 argument: filepath");
			return new XmlEvalObject(XElement.Parse(System.IO.File.ReadAllText(args[0].ToString())));
		}
        
        public static object Run(object[] args)
        {
            if (args.Length == 0 || !(args[0] is EvaluableLambda))
                throw new Exception("Run expects arguments: LambdaExpression expression, [object, object, ...]");
            EvaluableLambda lambda = args[0] as EvaluableLambda;
            if (lambda.Token.Parameters.Count() != args.Length - 1)
                throw new Exception($"Lambda function expects {lambda.Token.Parameters.Count()} parameters, received {args.Length - 1}");

            Func<string, object> variableLookup = s =>
            {
                var lambdaParams = lambda.Token.Parameters.ToList();
                for(int i = 0; i < lambdaParams.Count; i++)
                {
                    if (s == lambdaParams[i])
                        return args[i + 1];
                }
                return lambda.VariableLookup(s);
            };
            
            return lambda.Evaluator.Run(lambda.Token.Body, variableLookup);
        }
        
        public static object Clone(object[] args)
        {
            if (args.Length == 0 || !(args[0] is EvalObject))
                throw new Exception("Clone expects at least one argument with the object to be cloned");

            EvalObject input = args[0] as EvalObject;
            EvalObject output = new EvalObject();
            foreach (var prop in input)
                output[prop.Key] = prop.Value;

            for (int i = 1; i < args.Length; i++)
            {
                if (!(args[i] is KeyValuePair<string, object>))
                    throw new Exception("Calls to the Clone function must be of the form: Clone(object, [keyValue, keyValue, ...])");
                var keyValue = (KeyValuePair<string, object>)args[i];
                output[keyValue.Key] = keyValue.Value;
            }
            return output;
        }

		public static List<EvalScriptPropertyInfo> GetProperties(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("GetProperties expects 1 argument");
			object obj = args[0];
			Type objType = obj.GetType();

			//Use reflection to get all properties on the object
			List<EvalScriptPropertyInfo> output = new List<EvalScriptPropertyInfo>();
			if(!(obj is EvalObject))
			{
				foreach (var propInfo in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
				{
					if (propInfo.GetIndexParameters().Length == 0)
					{
						output.Add(new EvalScriptPropertyInfo()
						{
							Name = propInfo.Name,
							Type = propInfo.PropertyType.Name,
							Value = propInfo.GetValue(obj, null)
						});
					}
				}
			}
			
			//If the object is an EvalObject, go through getting its key value pairs
			if (obj is EvalObject)
			{
				var dict = obj as IDictionary;
				if (dict != null)
				{
					ICollection keysColl = dict.Keys;
					ICollection valuesColl = dict.Values;
					List<object> keysList = new List<object>();
					foreach (var key in keysColl)
						keysList.Add(key);
					if (keysList.All(x => x is string))
					{
						List<object> valuesList = new List<object>();
						foreach (var val in valuesColl)
							valuesList.Add(val);

						for (int i = 0; i < keysList.Count; i++)
						{
							output.Add(new EvalScriptPropertyInfo()
							{
								Name = keysList[i].ToString(),
								Type = valuesList[i]?.GetType()?.Name,
								Value = valuesList[i]
							});
						}
					}
				}
			}
			return output;
		}

		public static List<EvalScriptMethodInfo> GetMethods(object[] args)
		{
			if (args.Length != 1)
				throw new Exception("GetMethods expects 1 argument");
			object obj = args[0];
			Type objType = obj.GetType();

			//Use reflection to get all methods on the object
			List<EvalScriptMethodInfo> output = new List<EvalScriptMethodInfo>();
			foreach (var methodInfo in objType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(x => !x.IsSpecialName))
			{
				if (methodInfo.ReturnType != typeof(void))
				{
					var evalMethod = new EvalScriptMethodInfo()
					{
						Name = methodInfo.Name,
						ReturnType = methodInfo.ReturnType.Name
					};
					foreach(var param in methodInfo.GetParameters())
					{
						evalMethod.Parameters.Add(new EvalScriptMethodInfo.ParameterInfo()
						{
							Name = param.Name,
							Type = param.ParameterType.Name
						});
					}
					output.Add(evalMethod);
				}
			}

			return output;
		}


		public class EvalScriptPropertyInfo
		{
			public string Name { get; set; }
			public string Type { get; set; }
			public object Value { get; set; }
		}

		public class EvalScriptMethodInfo
		{
			public string Name { get; set; }
			public string ReturnType { get; set; }
			public List<ParameterInfo> Parameters { get; private set; } = new List<ParameterInfo>();

			public class ParameterInfo
			{
				public string Name { get; set; }
				public string Type { get; set; }
			}
		}


	}
}
