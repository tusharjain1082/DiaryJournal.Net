using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Evaluating.Operations
{
	internal static class Equal
	{
		internal static bool Run(int a, int b) => a == b;

		internal static bool Run(int a, long b) => a == b;
		internal static bool Run(long a, int b) => a == b;

		internal static bool Run(int a, float b) => a == b;
		internal static bool Run(float a, int b) => a == b;

		internal static bool Run(int a, double b) => a == b;
		internal static bool Run(double a, int b) => a == b;

		internal static bool Run(int a, decimal b) => a == b;
		internal static bool Run(decimal a, int b) => a == b;

		internal static bool Run(long a, long b) => a == b;

		internal static bool Run(long a, float b) => a == b;
		internal static bool Run(float a, long b) => a == b;

		internal static bool Run(long a, double b) => a == b;
		internal static bool Run(double a, long b) => a == b;

		internal static bool Run(long a, decimal b) => a == b;
		internal static bool Run(decimal a, long b) => a == b;

		internal static bool Run(float a, float b) => a == b;

		internal static bool Run(float a, double b) => a == b;
		internal static bool Run(double a, float b) => a == b;

		internal static bool Run(float a, decimal b) => (decimal)a == b;
		internal static bool Run(decimal a, float b) => a == (decimal)b;

		internal static bool Run(double a, double b) => a == b;

		internal static bool Run(double a, decimal b)
		{
			if (a >= (double)decimal.MaxValue || a <= (double)decimal.MinValue)
				return a == (double)b;
			return (decimal)a == b;
		}
		internal static bool Run(decimal a, double b)
		{
			if (b >= (double)decimal.MaxValue || b <= (double)decimal.MinValue)
				return (double)a == b;
			return a == (decimal)b;
		}

		internal static bool Run(decimal a, decimal b) => a == b;

		internal static bool Run(string a, string b) => a == b;
		internal static bool Run(string a, char b) => a == b.ToString();
		internal static bool Run(char a, string b) => a.ToString() == b;

		internal static bool Run(object a, object b) => (dynamic)a == (dynamic)b;
	}
}
