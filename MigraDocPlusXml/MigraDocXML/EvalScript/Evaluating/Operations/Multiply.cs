using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvalScript.Evaluating.Operations
{
    internal static class Multiply
    {
        internal static int Run(int a, int b) => a * b;
        
        internal static long Run(int a, long b) => a * b;
        internal static long Run(long a, int b) => a * b;
        
        internal static float Run(int a, float b) => a * b;
        internal static float Run(float a, int b) => a * b;
        
        internal static double Run(int a, double b) => a * b;
        internal static double Run(double a, int b) => a * b;
        
        internal static decimal Run(int a, decimal b) => a * b;
        internal static decimal Run(decimal a, int b) => a * b;
        
        internal static long Run(long a, long b) => a * b;
        
        internal static float Run(long a, float b) => a * b;
        internal static float Run(float a, long b) => a * b;
        
        internal static double Run(long a, double b) => a * b;
        internal static double Run(double a, long b) => a * b;
        
        internal static decimal Run(long a, decimal b) => a * b;
        internal static decimal Run(decimal a, long b) => a * b;
        
        internal static float Run(float a, float b) => a * b;
        
        internal static double Run(float a, double b) => a * b;
        internal static double Run(double a, float b) => a * b;
        
        internal static decimal Run(float a, decimal b) => (decimal)a * b;
        internal static decimal Run(decimal a, float b) => a * (decimal)b;
        
        internal static double Run(double a, double b) => a * b;
        
        internal static decimal Run(double a, decimal b) => (decimal)a * b;
        internal static decimal Run(decimal a, double b) => a * (decimal)b;
        
        internal static decimal Run(decimal a, decimal b) => a * b;

        internal static object Run(object a, object b) => (dynamic)a * (dynamic)b;
    }
}
