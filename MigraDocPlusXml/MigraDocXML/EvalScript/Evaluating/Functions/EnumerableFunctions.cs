using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EvalScript.Evaluating.Functions
{
    public static class EnumerableFunctions
    {
        public static object Count(object[] args)
        {
            if (args.Length == 1 && (args[0] is IEnumerable))
            {
                int count = 0;
                foreach (var item in (args[0] as IEnumerable))
                    count++;
                return count;
            }
            else if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
            {
                IEnumerable items = args[0] as IEnumerable;
                EvaluableLambda lambda = args[1] as EvaluableLambda;
                if (lambda.Token.Parameters.Count() != 1)
                    throw new SyntaxException("Count expects a lambda expression that takes one parameter");

                int count = 0;
                foreach (var item in items)
                {
                    Func<string, object> variableLookup = s =>
                    {
                        if (s == lambda.Token.Parameters.First())
                            return item;
                        return lambda.VariableLookup(s);
                    };

                    if (Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                        count++;
                }
                return count;
            }

            throw new SyntaxException("Count expects arguments: IEnumerable items, [LambdaExpression filter]");
        }


		public static object Contains(object[] args)
		{
			if (args.Length != 2 || !(args[0] is IEnumerable))
				throw new SyntaxException("Contains expects arguments: IEnumerable items, object searchItem");
			var items = args[0] as IEnumerable;
			var searchItem = args[1];
			foreach(var item in items)
			{
				if (Equals(item, searchItem))
					return true;
			}
			return false;
		}


        public static object Sum(object[] args)
        {
            if (args.Length < 1 || args.Length > 2 || !(args[0] is IEnumerable) || (args.Length == 2 && !(args[1] is EvaluableLambda)))
                throw new SyntaxException("Sum expects two arguments: IEnumerable items, optional LambdaExpression fieldSelector");

            IEnumerable items = args[0] as IEnumerable;
			EvaluableLambda lambda = null;
			if(args.Length == 2)
			{
				lambda = args[1] as EvaluableLambda;
				if (lambda.Token.Parameters.Count() != 1)
					throw new SyntaxException("Sum expects a lambda expression that takes one parameter");
			}
			
            object sum = null;
            foreach (var item in items)
            {
				object itemVal = item;
				if(lambda != null)
				{
					Func<string, object> variableLookup = s =>
					{
						if (s == lambda.Token.Parameters.First())
							return item;
						return lambda.VariableLookup(s);
					};
					itemVal = lambda.Evaluator.Run(lambda.Token.Body, variableLookup);
				}

                if (itemVal != null)
                {
                    if (sum == null)
                        sum = itemVal;
                    else
                        sum = Operations.Plus.Run((dynamic)sum, (dynamic)itemVal);
                }
            }
            return sum;
        }


        public static IEnumerable Order(object[] args)
        {
            if (args.Length < 2 || !(args[0] is IEnumerable))
                throw new SyntaxException("Order expects arguments: IEnumerable items, LambdaExpression orderSelector[, LambdaExpression thenBySelector, ...]");

            IEnumerable items = args[0] as IEnumerable;
			
			EvaluableLambda[] lambdas = new EvaluableLambda[args.Length - 1];
			for (int i = 1; i < args.Length; i++)
			{
				var lambda = args[i] as EvaluableLambda;
				if (lambda == null)
					throw new SyntaxException("OrderDesc expects arguments: IEnumerable items, LambdaExpression orderSelector[, LambdaExpression thenBySelector, ...]");
				if(lambda.Token.Parameters.Count() < 1 || lambda.Token.Parameters.Count() > 2)
					throw new SyntaxException("Order expects a lambda expression that takes one parameter");
				lambdas[i - 1] = lambda;
			}

			bool hasDescParam = false;
			bool hasAscParam = false;
			foreach(var lambda in lambdas)
			{
				var paramList = lambda.Token.Parameters.ToList();
				if(paramList.Count == 2)
				{
					if (paramList[1] == "desc")
						hasDescParam = true;
					else if (paramList[1] == "asc")
						hasAscParam = true;
					else
						throw new SyntaxException("Invalid 2nd lambda parameter in Order, can be called either asc or desc");
				}
			}

			Func<string, object> baseVariableLookup = lambdas.First().VariableLookup;
			if(hasAscParam || hasDescParam)
			{
				baseVariableLookup = s =>
				{
					if (hasAscParam && s == "asc")
						return null;
					if (hasDescParam && s == "desc")
						return null;
					return lambdas.First().VariableLookup;
				};
			}

			var keyedItems = new List<Tuple<IComparable[], object>>();
            foreach (var item in items)
            {
				var keyedItem = new Tuple<IComparable[], object>(new IComparable[lambdas.Length], item);
				for(int i = 0; i < lambdas.Length; i++)
				{
					var lambda = lambdas[i];
					Func<string, object> variableLookup = s =>
					{
						if (s == lambda.Token.Parameters.First())
							return item;
						return lambda.VariableLookup(s);
					};
					keyedItem.Item1[i] = lambda.Evaluator.Run(lambda.Token.Body, variableLookup) as IComparable;
				}

                keyedItems.Add(keyedItem);
            }

			IOrderedEnumerable<Tuple<IComparable[], object>> sorting = keyedItems.OrderBy(x => x.Item1[0]);
			for (int i = 1; i < lambdas.Length; i++)
			{
				int index = i;
				if (lambdas[i].Token.Parameters.Count() == 2 && lambdas[i].Token.Parameters.ToList()[1] == "desc")
					sorting = sorting.ThenByDescending(x => x.Item1[index]);
				else
					sorting = sorting.ThenBy(x => x.Item1[index]);
			}
            return sorting.Select(x => x.Item2).ToList();
        }


        public static IEnumerable OrderDesc(object[] args)
        {
			if (args.Length < 2 || !(args[0] is IEnumerable))
				throw new SyntaxException("OrderDesc expects arguments: IEnumerable items, LambdaExpression orderSelector[, LambdaExpression thenBySelector, ...]");

			EvaluableLambda[] lambdas = new EvaluableLambda[args.Length - 1];
			for (int i = 1; i < args.Length; i++)
			{
				var lambda = args[i] as EvaluableLambda;
				if(lambda == null)
					throw new SyntaxException("OrderDesc expects arguments: IEnumerable items, LambdaExpression orderSelector[, LambdaExpression thenBySelector, ...]");
				lambdas[i - 1] = lambda;
			}
			
			IEnumerable items = args[0] as IEnumerable;
			
			if (lambdas.Any(x => x.Token.Parameters.Count() < 1 || x.Token.Parameters.Count() > 2))
				throw new SyntaxException("OrderDesc expects a lambda expression that takes one parameter");

			bool hasDescParam = false;
			bool hasAscParam = false;
			foreach (var lambda in lambdas)
			{
				var paramList = lambda.Token.Parameters.ToList();
				if (paramList.Count == 2)
				{
					if (paramList[1] == "desc")
						hasDescParam = true;
					else if (paramList[1] == "asc")
						hasAscParam = true;
					else
						throw new SyntaxException("Invalid 2nd lambda parameter in OrderDesc, can be called either asc or desc");
				}
			}

			Func<string, object> baseVariableLookup = lambdas.First().VariableLookup;
			if (hasAscParam || hasDescParam)
			{
				baseVariableLookup = s =>
				{
					if (hasAscParam && s == "asc")
						return null;
					if (hasDescParam && s == "desc")
						return null;
					return lambdas.First().VariableLookup;
				};
			}

			var keyedItems = new List<Tuple<IComparable[], object>>();
            foreach (var item in items)
			{
				var keyedItem = new Tuple<IComparable[], object>(new IComparable[lambdas.Length], item);
				for (int i = 0; i < lambdas.Length; i++)
				{
					var lambda = lambdas[i];
					Func<string, object> variableLookup = s =>
					{
						if (s == lambda.Token.Parameters.First())
							return item;
						return lambda.VariableLookup(s);
					};
					keyedItem.Item1[i] = lambda.Evaluator.Run(lambda.Token.Body, variableLookup) as IComparable;
				}

				keyedItems.Add(keyedItem);
			}

			IOrderedEnumerable<Tuple<IComparable[], object>> sorting = keyedItems.OrderByDescending(x => x.Item1[0]);
			for (int i = 1; i < lambdas.Length; i++)
			{
				int index = i;
				if (lambdas[i].Token.Parameters.Count() == 2 && lambdas[i].Token.Parameters.ToList()[1] == "asc")
					sorting = sorting.ThenBy(x => x.Item1[index]);
				else
					sorting = sorting.ThenByDescending(x => x.Item1[index]);
			}
			return sorting.Select(x => x.Item2).ToList();
		}


        public static IEnumerable Where(object[] args)
        {
            if (args.Length != 2 || !(args[0] is IEnumerable) || !(args[1] is EvaluableLambda))
                throw new SyntaxException("Where expects two arguments: IEnumerable items, LambdaExpression filter");

            IEnumerable items = args[0] as IEnumerable;
            EvaluableLambda lambda = args[1] as EvaluableLambda;
            if (lambda.Token.Parameters.Count() != 1)
                throw new SyntaxException("Where expects a lambda expression that takes one parameter");

            var filteredItems = new List<object>();
            foreach (var item in items)
            {
                Func<string, object> variableLookup = s =>
                {
                    if (s == lambda.Token.Parameters.First())
                        return item;
                    return lambda.VariableLookup(s);
                };

                if (Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                    filteredItems.Add(item);
            }
            return filteredItems;
        }


        public static Dictionary<object, List<object>> Group(object[] args)
        {
            if (args.Length != 2 || !(args[0] is IEnumerable) || !(args[1] is EvaluableLambda))
                throw new SyntaxException("Group expects two arguments: IEnumerable items, LambdaExpression groupClause");

            IEnumerable items = args[0] as IEnumerable;
            EvaluableLambda lambda = args[1] as EvaluableLambda;
            if (lambda.Token.Parameters.Count() != 1)
                throw new SyntaxException("Group expects a lambda expression that takes one parameter");

            var output = new Dictionary<object, List<object>>();
            foreach (var item in items)
            {
                Func<string, object> variableLookup = s =>
                {
                    if (s == lambda.Token.Parameters.First())
                        return item;
                    return lambda.VariableLookup(s);
                };

                var key = lambda.Evaluator.Run(lambda.Token.Body, variableLookup);
                if (output.ContainsKey(key))
                    output[key].Add(item);
                else
                    output[key] = new List<object>() { item };
            }
            return output;
        }


		public static IEnumerable Distinct(object[] args)
		{
			if (args.Length != 1 || !(args[0] is IEnumerable))
				throw new SyntaxException("Distinct expects one argument: IEnumerable items");

			IEnumerable items = args[0] as IEnumerable;
			var output = new List<object>();
			foreach(var item in items)
			{
				if (!output.Contains(item))
					output.Add(item);
			}
			return output;
		}


        public static IEnumerable Reverse(object[] args)
        {
            if (args.Length != 1 || !(args[0] is IEnumerable))
                throw new SyntaxException("Reverse expects one argument of type IEnumerable");
            return (args[0] as IEnumerable).OfType<object>().Reverse();
        }


        public static object First(object[] args)
        {
            if (args.Length == 1 && (args[0] is IEnumerable))
                return (args[0] as IEnumerable).OfType<object>().First();

            else if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
            {
                IEnumerable items = args[0] as IEnumerable;
                EvaluableLambda lambda = args[1] as EvaluableLambda;
                if (lambda.Token.Parameters.Count() != 1)
                    throw new SyntaxException("First expects a lambda expression that takes one parameter");

                foreach (var item in items)
                {
                    Func<string, object> variableLookup = s =>
                    {
                        if (s == lambda.Token.Parameters.First())
                            return item;
                        return lambda.VariableLookup(s);
                    };

                    if (Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                        return item;
                }
                throw new InvalidOperationException("Sequence contains no elements");
            }

            throw new Exception("First expects arguments: IEnumerable items, [LambdaExpression filter]");
        }


        public static object FirstOrNull(object[] args)
        {
            if (args.Length == 1 && (args[0] is IEnumerable))
            {
                var items = (args[0] as IEnumerable).OfType<object>();
                return (items.Count() == 0) ? null : items.First();
            }
            else if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
            {
                IEnumerable items = args[0] as IEnumerable;
                EvaluableLambda lambda = args[1] as EvaluableLambda;
                if (lambda.Token.Parameters.Count() != 1)
                    throw new SyntaxException("FirstOrNull expects a lambda expression that takes one parameter");

                foreach (var item in items)
                {
                    Func<string, object> variableLookup = s =>
                    {
                        if (s == lambda.Token.Parameters.First())
                            return item;
                        return lambda.VariableLookup(s);
                    };

                    if (Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                        return item;
                }
                return null;
            }
            throw new Exception("FirstOrNull expects arguments: IEnumerable items, [LambdaExpression filter]");
        }


        public static object Last(object[] args)
        {
            if (args.Length == 1 && (args[0] is IEnumerable))
                return (args[0] as IEnumerable).OfType<object>().Last();

            else if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
            {
                IEnumerable items = args[0] as IEnumerable;
                EvaluableLambda lambda = args[1] as EvaluableLambda;
                if (lambda.Token.Parameters.Count() != 1)
                    throw new SyntaxException("Last expects a lambda expression that takes one parameter");

                var list = items.OfType<object>().ToList();
                for(int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    Func<string, object> variableLookup = s =>
                    {
                        if (s == lambda.Token.Parameters.First())
                            return item;
                        return lambda.VariableLookup(s);
                    };

                    if (Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                        return item;
                }
                throw new InvalidOperationException("Sequence contains no elements");
            }

            throw new Exception("Last expects arguments: IEnumerable items, [LambdaExpression filter]");
        }


        public static object LastOrNull(object[] args)
        {
            if (args.Length == 1 && (args[0] is IEnumerable))
            {
                var items = (args[0] as IEnumerable).OfType<object>();
                return (items.Count() == 0) ? null : items.Last();
            }
            else if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
            {
                IEnumerable items = args[0] as IEnumerable;
                EvaluableLambda lambda = args[1] as EvaluableLambda;
                if (lambda.Token.Parameters.Count() != 1)
                    throw new SyntaxException("LastOrNull expects a lambda expression that takes one parameter");

                var list = items.OfType<object>().ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    Func<string, object> variableLookup = s =>
                    {
                        if (s == lambda.Token.Parameters.First())
                            return item;
                        return lambda.VariableLookup(s);
                    };

                    if (Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                        return item;
                }
                return null;
            }

            throw new SyntaxException("LastOrNull expects arguments: IEnumerable items, [LambdaExpression filter]");
        }


        public static object Any(object[] args)
        {
            if (args.Length != 2 || !(args[0] is IEnumerable) || !(args[1] is EvaluableLambda))
                throw new Exception("Any expects two arguments: IEnumerable items, LambdaExpression filter");

            IEnumerable items = args[0] as IEnumerable;
            EvaluableLambda lambda = args[1] as EvaluableLambda;

            try
            {
                First(args);
                return true;
            }
            catch(InvalidOperationException ioex)
            {
                if (ioex.Message == "Sequence contains no elements")
                    return false;
                throw;
            }
        }


        public static object All(object[] args)
        {
            if (args.Length != 2 || !(args[0] is IEnumerable) || !(args[1] is EvaluableLambda))
                throw new SyntaxException("All expects two arguments: IEnumerable items, LambdaExpression filter");

            IEnumerable items = args[0] as IEnumerable;
            EvaluableLambda lambda = args[1] as EvaluableLambda;
            if (lambda.Token.Parameters.Count() != 1)
                throw new SyntaxException("All expects a lambda expression that takes one parameter");

            foreach(var item in items)
            {
                Func<string, object> variableLookup = s =>
                {
                    if (s == lambda.Token.Parameters.First())
                        return item;
                    return lambda.VariableLookup(s);
                };

                if (!Convert.ToBoolean(lambda.Evaluator.Run(lambda.Token.Body, variableLookup)))
                    return false;
            }
            return true;
        }


		public static IEnumerable<int> Range(object[] args)
		{
			if (args.Length != 2)
				throw new SyntaxException("Range expects two arguments: int start, int count");

			return Enumerable.Range(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
		}


        public static object Index(object[] args)
        {
            if (args.Length != 2)
                throw new SyntaxException("Index expects two arguments: List items, int index");

            if(args[0] is Array)
                return (args[0] as Array).GetValue(Convert.ToInt32(args[1]));
            
            var objType = args[0].GetType();
            return objType.GetMethod("get_Item").Invoke(args[0], new object[] { args[1] });
        }


        public static IEnumerable Select(object[] args)
        {
            if (args.Length != 2 || !(args[0] is IEnumerable) || !(args[1] is EvaluableLambda))
                throw new SyntaxException("Select/Map expects two arguments: IEnumerable items, LambdaExpression selector");

            IEnumerable items = args[0] as IEnumerable;
            EvaluableLambda lambda = args[1] as EvaluableLambda;
            if (lambda.Token.Parameters.Count() != 1)
                throw new SyntaxException("Select/Map expects a lambda expression that takes one parameter");

            var results = new List<object>();
            foreach(var item in items)
            {
                Func<string, object> variableLookup = s =>
                {
                    if (s == lambda.Token.Parameters.First())
                        return item;
                    return lambda.VariableLookup(s);
                };

                results.Add(lambda.Evaluator.Run(lambda.Token.Body, variableLookup));
            }
            return results;
        }


		public static IEnumerable SelectMany(object[] args)
		{
			if (args.Length != 2 || !(args[0] is IEnumerable) || !(args[1] is EvaluableLambda))
				throw new SyntaxException("SelectMany/MapMany expects two arguments: IEnumerable items, LambdaExpression selector");

			IEnumerable items = args[0] as IEnumerable;
			EvaluableLambda lambda = args[1] as EvaluableLambda;
			if (lambda.Token.Parameters.Count() != 1)
				throw new SyntaxException("SelectMany/MapMany expects a lambda expression that takes one parameter");

			var results = new List<object>();
			foreach(var item in items)
			{
				Func<string, object> variableLookup = s =>
				{
					if (s == lambda.Token.Parameters.First())
						return item;
					return lambda.VariableLookup(s);
				};

				var selectResult = lambda.Evaluator.Run(lambda.Token.Body, variableLookup);
				if (selectResult != null && selectResult is IEnumerable)
				{
					foreach (var result in selectResult as IEnumerable)
						results.Add(result);
				}
			}
			return results;
		}


        public static object[] Join(object[] args)
        {
            if (args.Length == 0 || args.Any(x => x is string || !(x is IEnumerable)))
                throw new Exception("Join expects one or more arguments, all of enumerable type");

            var output = new List<object>();
            foreach (var arg in args.OfType<IEnumerable>())
                output.AddRange(arg.OfType<object>());
            return output.ToArray();
        }


        public static string MergeToString(object[] args)
        {
            if (args.Length == 2 && args[0] is IEnumerable && args[1] is string)
            {
                IEnumerable items = args[0] as IEnumerable;
                string joiner = args[1] as string;
                return string.Join(joiner, items.OfType<object>().ToArray());
            }
            else if (args.Length == 3 && args[0] is IEnumerable && args[1] is string && args[2] is EvaluableLambda)
            {
                IEnumerable items = args[0] as IEnumerable;
                string joiner = args[1] as string;
                EvaluableLambda lambda = args[2] as EvaluableLambda;
                return string.Join(joiner, Select(new object[2] { items, lambda }).OfType<object>());
            }
            else
                throw new Exception("MergeToString expects arguments: IEnumerable items, string joiner, [LambdaExpression selector]");
        }


		public static object Partition(object[] args)
		{
			if (args.Length < 2 || args.Length > 3 || !(args[0] is IEnumerable))
				throw new Exception("Segment expects parameters: IEnumerable collection, int segmentSize, optional bool onlyFullSegments = false");
			IEnumerable items = (IEnumerable)args[0];
			int segmentSize = Convert.ToInt32(args[1]);
			if (segmentSize <= 0)
				throw new ArgumentException("segmentSize parameter must be greater than 0");
			bool onlyFullSegments = false;
			if (args.Length >= 3)
				onlyFullSegments = (bool)args[2];

			List<object[]> output = new List<object[]>();
			object[] lastSegment = null;
			int i = 0;
			int lastSegmentUpdates = 0;
			foreach (var item in items)
			{
				int iModulo = i % segmentSize;
				if (iModulo == 0)
				{
					if(lastSegment != null)
						output.Add(lastSegment);
					lastSegment = new object[segmentSize];
					lastSegmentUpdates = 0;
				}
				lastSegment[iModulo] = item;
				lastSegmentUpdates++;
				i++;
			}
			if (lastSegmentUpdates == segmentSize || (!onlyFullSegments && lastSegmentUpdates > 0))
				output.Add(lastSegment);
			return output;
		}
    }
}
