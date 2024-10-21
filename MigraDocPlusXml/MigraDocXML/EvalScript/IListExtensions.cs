using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript
{
    public static class IListExtensions
    {
        /// <summary>
        /// Get the index of the first item which meets the search criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list of items to search through</param>
        /// <param name="func">The search criteria</param>
        /// <param name="startIndex">Which index to start searching from</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> func, int startIndex = 0)
        {
            for (int i = startIndex; i < list.Count; i++)
            {
                if (func(list[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Get the index of the last item which meets the search criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list of items to search through</param>
        /// <param name="func">The search criteria</param>
        /// <param name="startIndex">Which index to start searching from, if non-zero value, then the search will work backwards from that value</param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this IList<T> list, Func<T, bool> func, int startIndex = -1)
        {
            if (startIndex < 0)
                startIndex = list.Count - 1;
            for(int i = startIndex; i >= 0; i--)
            {
                if (func(list[i]))
                    return i;
            }
            return -1;
        }
    }
}
