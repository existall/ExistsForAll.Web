using System;
using System.Collections.Generic;

namespace ExistsForAll.Web.Extensions
{
    internal static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> target, Action<T> actionToPerform)
        {
            foreach (var item in target)
            {
                actionToPerform(item);
            }
        }

        public static T[] AsArray<T>(this T target)
        {
            return new T[] {target};
        }
    }
}
