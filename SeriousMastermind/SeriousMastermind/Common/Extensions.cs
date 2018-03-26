using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SeriousMastermind.Models;

namespace SeriousMastermind.Common
{
    public static class Extensions
    {
        public static string Remove(this string str, string substring)
        {
            return str.Replace(substring, "");
        }

        public static string Take(this string str, int n)
        {
            return new string(str.AsEnumerable().Take(n).ToArray());
        }

        public static string Skip(this string str, int n)
        {
            return new string(str.AsEnumerable().Skip(n).ToArray());
        }

        public static void RemoveBy(this DbStatistics dbSet, Func<Statistic, bool> selector)
        {
            var set = dbSet.ToArray();
            foreach (var entity in set)
            {
                if (selector(entity))
                    dbSet.Remove(entity);
            }
        }

        public static IDictionary<string, object> PropertiesDictionary(this object source)
        {
            return source.PropertiesDictionary<object>();
        }

        public static IDictionary<string, T> PropertiesDictionary<T>(this object source)
        {
            if (source == null)
                throw new NullReferenceException("Unable to convert anonymous object to a dictionary. The source anonymous object is null.");

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                var value = property.GetValue(source);
                if (IsOfType<T>(value))
                    dictionary.Add(property.Name, (T)value);
            }
            return dictionary;
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        public static bool HasSamePropertiesWithSameValues<T>(this T o1, Func<T, object> p, T o2)
        {
            var o1p = o1.PropertiesDictionary();
            var o2p = o2.PropertiesDictionary();
            var filter = p(o2).PropertiesDictionary();
            var sameKeys = o1p.Keys.ToArray().Intersect(o2p.Keys.ToArray()).Intersect(filter.Keys).ToArray();
            return sameKeys.All(k => o1p[k].Equals(o2p[k]));
        }

        public static bool Eq(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException(@"Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static string[] Split(this string str, string separator, bool includeSeparator = false)
        {
            var split = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            if (includeSeparator)
            {
                var splitWithSeparator = new string[split.Length + split.Length - 1];
                var j = 0;
                for (var i = 0; i < splitWithSeparator.Length; i++)
                {
                    if (i % 2 == 1)
                        splitWithSeparator[i] = separator;
                    else
                        splitWithSeparator[i] = split[j++];
                }
                split = splitWithSeparator;
            }
            return split;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> en, int partsN)
        {
            var arr = en.ToArray();
            var l = arr.Length;
            var partL = (int) Math.Ceiling((double) l / partsN);

            var parts = new T[partsN][];
            for (var i = 0; i < partsN; i++)
            {
                parts[i] = new T[partL];
                for (var j = 0; j < partL; j++)
                {
                    var listIdx = i * partL + j;
                    if (listIdx < arr.Length)
                        parts[i][j] = arr[listIdx];
                    else
                        break;
                }
            }
            var excess = partL * partsN - l;
            parts[partsN - 1] = parts[partsN - 1].Take(partL - excess).ToArray();

            return parts;
        }

        public static string ToStringDelimitedBy<T>(this IEnumerable<T> enumerable, string strBetween = "")
        {
            return string.Join(strBetween, enumerable);
        }

        public static string JoinAsString<T>(this IEnumerable<T> enumerable, string strBetween = "")
        {
            return enumerable.ToStringDelimitedBy(strBetween);
        }
    }
}
