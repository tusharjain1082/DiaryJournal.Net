using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Evaluating.Functions
{
    public static class NumericFunctions
    {
        public static object Abs(object[] args)
        {
            if (args.Length != 1)
                throw new Exception("Trim expects one argument of numeric type");
            return Math.Abs(Convert.ToDecimal(args[0]));
        }


        public static object Min(object[] args)
        {
            if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
			{
                args = EnumerableFunctions.Select(args).OfType<object>().ToArray();
				if (args.Length == 1)
					return args[0];
				else if (args.Length == 0)
					return null;
			}

            if (args.Length < 2)
                throw new Exception("Min expects two or more arguments of numeric type");
            return args.OfType<IComparable>().Min();
        }


        public static object Max(object[] args)
        {
            if (args.Length == 2 && (args[0] is IEnumerable) && (args[1] is EvaluableLambda))
			{
                args = EnumerableFunctions.Select(args).OfType<object>().ToArray();
				if (args.Length == 1)
					return args[0];
				else if (args.Length == 0)
					return null;
			}

            if (args.Length < 2)
                throw new Exception("Max expects two or more arguments of numeric type");
            return args.OfType<IComparable>().Max();
        }


        public static object Floor(object[] args)
        {
            if (args.Length != 1)
                throw new Exception("Floor expects one argument of numeric type");
            return Math.Floor(Convert.ToDecimal(args[0]));
        }


        public static object Ceiling(object[] args)
        {
            if (args.Length != 1)
                throw new Exception("Ceil expects one argument of numeric type");
            return Math.Ceiling(Convert.ToDecimal(args[0]));
        }


        public static object Round(object[] args)
        {
            if (args.Length == 1)
                return Math.Round(Convert.ToDecimal(args[0]));
            else if (args.Length == 2)
                return Math.Round(Convert.ToDecimal(args[0]), Convert.ToInt32(args[1]));
            throw new Exception("Round expects the first parameter of any numeric type and optional second parameter of int type");
        }


		private static Random _random = new Random();
		private static object _lock = new object();

		public static object RandomInt(object[] args)
		{
			if (args.Length == 0)
			{
				lock (_lock) { return _random.Next(); }
			}
			if(args.Length == 1)
			{
				int max = Convert.ToInt32(args[0]);
				if (max <= 0)
					throw new ArgumentOutOfRangeException("excMax must be greater than 0");
				lock (_lock) { return _random.Next(max); }
			}
			if(args.Length == 2)
			{
				int min = Convert.ToInt32(args[0]);
				int max = Convert.ToInt32(args[1]);
				if (max <= min)
					throw new ArgumentOutOfRangeException("excMax must be greater than incMin");
				lock (_lock) { return _random.Next(min, max); }
			}
			throw new ArgumentException("RandomInt expects either no parameters, or parameter (intnt excMax), or parameters (int incMin, int excMax)");
		}


		public static object RandomDouble(object[] args)
		{
			if(args.Length > 2)
				throw new ArgumentException("RandomDouble expects either no parameters, or parameter (double excMax), or parameters (double incMin, double excMax)");
			double min = 0;
			double max = 1;
			if (args.Length == 1)
			{
				max = Convert.ToDouble(args[0]);
				if (max <= 0)
					throw new ArgumentOutOfRangeException("excMax must be greater than 0");
			}
			if (args.Length == 2)
			{
				min = Convert.ToDouble(args[0]);
				max = Convert.ToDouble(args[1]);
				if (max <= min)
					throw new ArgumentOutOfRangeException("excMax must be greater than incMin");
			}
			lock (_lock) { return (_random.NextDouble() * (max - min)) + min; }
		}
	}
}
