using System;
using System.Data.Entity;
using System.Linq;

namespace DataService.Common
{
    public static class Extensions
    {
        public static string Take(this string str, int n)
        {
            return new string(str.AsEnumerable().Take(n).ToArray());
        }

        public static string Skip(this string str, int n)
        {
            return new string(str.AsEnumerable().Skip(n).ToArray());
        }

        public static void RemoveBy<TSource>(this DbSet<TSource> dbSet, Func<TSource, bool> selector) where TSource : class
        {
            var set = dbSet.ToArray();
            foreach (var entity in set)
            {
                if (selector(entity))
                    dbSet.Remove(entity);
            }
        }
    }
}
