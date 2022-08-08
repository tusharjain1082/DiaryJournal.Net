using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Evaluating.Functions
{
    public static class DateFunctions
    {
        public static object DateTime(object[] args)
        {
            if (args.Length < 1)
                throw new Exception("DateTime expects at least one argument for the year");
            if (args.Length > 7)
                throw new Exception("DateTime expects no more than 7 arguments for: year, month, day, hour, minute, second, millisecond");
            int year = Convert.ToInt32(args[0]);
            int month = (args.Length >= 2) ? Convert.ToInt32(args[1]) : 1;
            int day = (args.Length >= 3) ? Convert.ToInt32(args[2]) : 1;
            int hour = (args.Length >= 4) ? Convert.ToInt32(args[3]) : 0;
            int minute = (args.Length >= 5) ? Convert.ToInt32(args[4]) : 0;
            int second = (args.Length >= 6) ? Convert.ToInt32(args[5]) : 0;
            int millisecond = (args.Length >= 7) ? Convert.ToInt32(args[6]) : 0;
            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }


        public static object Now(object[] args)
        {
            if (args.Length > 0)
                throw new Exception("Now does not take any arguments");
            return System.DateTime.Now;
        }


        public static object AddYears(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddYears expects 2 arguments of types DateTime & int");
            return ((DateTime)args[0]).AddYears(Convert.ToInt32(args[1]));
        }


        public static object AddMonths(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddMonths expects 2 arguments of types DateTime & int");
            return ((DateTime)args[0]).AddMonths(Convert.ToInt32(args[1]));
        }


        public static object AddDays(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddDays expects 2 arguments of types DateTime & double");
            return ((DateTime)args[0]).AddDays(Convert.ToDouble(args[1]));
        }


        public static object AddHours(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddHours expects 2 arguments of types DateTime & double");
            return ((DateTime)args[0]).AddHours(Convert.ToDouble(args[1]));
        }


        public static object AddMinutes(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddMinutes expects 2 arguments of types DateTime & double");
            return ((DateTime)args[0]).AddMinutes(Convert.ToDouble(args[1]));
        }


        public static object AddSeconds(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddSeconds expects 2 arguments of types DateTime & double");
            return ((DateTime)args[0]).AddSeconds(Convert.ToDouble(args[1]));
        }


        public static object AddMilliseconds(object[] args)
        {
            if (args.Length != 2 || !(args[0] is DateTime))
                throw new Exception("AddMilliseconds expects 2 arguments of types DateTime & double");
            return ((DateTime)args[0]).AddMilliseconds(Convert.ToDouble(args[1]));
        }
    }
}
