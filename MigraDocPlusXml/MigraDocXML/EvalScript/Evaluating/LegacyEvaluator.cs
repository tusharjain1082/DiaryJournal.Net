using EvalScript.Interpreting;
using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EvalScript.Evaluating.Functions;

namespace EvalScript.Evaluating
{
    public class LegacyEvaluator : EvaluatorBase
    {
        public List<PropAccessor> PropertyCache { get; private set; } = new List<PropAccessor>();

        protected override object RunChain(object previousObj, PropertyToken token, Func<string, object> variableLookup)
        {
            try
            {
                var previousObjType = previousObj.GetType();
                var propName = token.Name;

                PropAccessor accessor = PropertyCache.FirstOrDefault(x => x.Type == previousObjType && x.PropertyName.Equals(propName));
                if (accessor != null)
                {
                    accessor.TouchCount++;
                    return accessor.Getter(previousObj);
                }

                PropertyInfo propInfo = previousObjType.GetProperty(propName);
                if (propInfo != null)
                {
                    accessor = new PropAccessor(previousObjType, propName, (x) => propInfo.GetValue(x, null));
                    PropertyCache.Add(accessor);
                    return accessor.Getter(previousObj);
                }

                MethodInfo methodInfo = previousObjType.GetMethod(token.Name, new Type[0]);
                if (methodInfo != null && methodInfo.IsPublic && !methodInfo.IsStatic)
                {
                    accessor = new PropAccessor(previousObjType, propName, (x) => methodInfo.Invoke(x, null));
                    PropertyCache.Add(accessor);
                    return accessor.Getter(previousObj);
                }

                methodInfo = previousObjType.GetMethod("get_Item", new Type[] { typeof(string) });
                if(methodInfo != null && methodInfo.IsPublic && !methodInfo.IsStatic)
                {
                    accessor = new PropAccessor(previousObjType, propName, (x) => methodInfo.Invoke(x, new object[] { propName }));
                    PropertyCache.Add(accessor);
                    return accessor.Getter(previousObj);
                }
                
                throw new Exception("Unrecognised property name: " + token.Name);
            }
            catch
            {
                throw new Exception("Unrecognised property name: " + token.Name);
            }
        }

        public List<PropAccessor> IndexerCache { get; private set; } = new List<PropAccessor>();

        protected override object RunChain(object previousObj, IndexerCallToken token, Func<string, object> variableLookup)
        {
            var tokenResult = Run(token.Parameter, variableLookup);

            if(previousObj is IEnumerable && tokenResult is int)
            {
                try
                {
                    return Functions["Index"](new object[2] { previousObj, tokenResult });
                }
                catch { }
            }
            
            try
            {
                var previousObjType = previousObj.GetType();
                var tokenResultString = tokenResult?.ToString();

                PropAccessor accessor = IndexerCache.FirstOrDefault(x => x.Type == previousObjType && x.PropertyName.Equals(tokenResultString));
                if (accessor != null)
                {
                    accessor.TouchCount++;
                    return accessor.Getter(previousObj);
                }

                MethodInfo methodInfo = previousObjType.GetMethod("get_Item", new Type[] { typeof(string) });
                if (methodInfo != null && methodInfo.IsPublic && !methodInfo.IsStatic)
                {
                    accessor = new PropAccessor(previousObjType, tokenResultString, (x) => methodInfo.Invoke(x, new object[] { tokenResultString }));
                    IndexerCache.Add(accessor);
                    return accessor.Getter(previousObj);
                }
                
                PropertyInfo propInfo = previousObjType.GetProperty(tokenResultString);
                if (propInfo != null)
                {
                    accessor = new PropAccessor(previousObjType, tokenResultString, (x) => propInfo.GetValue(x, null));
                    IndexerCache.Add(accessor);
                    return accessor.Getter(previousObj);
                }

                methodInfo = previousObjType.GetMethod(tokenResultString, new Type[0]);
                if (methodInfo != null && methodInfo.IsPublic && !methodInfo.IsStatic)
                {
                    accessor = new PropAccessor(previousObjType, tokenResultString, (x) => methodInfo.Invoke(x, null));
                    IndexerCache.Add(accessor);
                    return accessor.Getter(previousObj);
                }

                throw new Exception("Unrecognised property name: " + tokenResult);
            }
            catch
            {
                throw new Exception("Unrecognised property name: " + tokenResult);
            }
        }

        protected override object RunChain(object previousObj, FunctionCallToken token, Func<string, object> variableLookup)
        {
            if (!Functions.ContainsKey(token.Name))
                throw new SyntaxException("Unrecognised function name: " + token.Name);

			var paramList = token.Parameters.ToList();
            var args = new object[paramList.Count + 1];
            args[0] = previousObj;
            for(int i = 0; i < paramList.Count; i++)
            {
				var parameter = paramList[i];
                if (parameter is LambdaExpressionToken)
                    args[i + 1] = new EvaluableLambda(parameter as LambdaExpressionToken, this, variableLookup);
                else
                    args[i + 1] = Run(parameter, variableLookup);
            }

            return Functions[token.Name](args);
        }


        public class PropAccessor
        {
            public Type Type { get; private set; }

            public object PropertyName { get; private set; }

            public Func<object, object> Getter { get; private set; }

            /// <summary>
            /// When the accessor was first created
            /// </summary>
            public DateTime DateCreated { get; private set; }

            /// <summary>
            /// How many times the accessor has been used
            /// </summary>
            public int TouchCount { get; set; } = 1;

            public PropAccessor(Type type, object propertyName, Func<object, object> getter)
            {
                Type = type;
                PropertyName = propertyName;
                Getter = getter;
                DateCreated = DateTime.Now;
            }
        }
    }
}
