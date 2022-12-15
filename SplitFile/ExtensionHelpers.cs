using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitFile
{
    public static class ExtensionHelpers
    {
        public static bool ContainsAny<T>(this IEnumerable<T> enumerable, params T[] values)
        {
            foreach (T item in enumerable)
                if (values.Any(a => item.Equals(a)))
                    return true;
            return false;
        }
        public static bool ContainsAny(this string[] value, params string[] values)
            => value.Select(s => s.ToLowerInvariant()).ContainsAny(values);

        public static bool TryParseToInt32(this IEnumerable<string> enumerable, out int[] result)
        {
            result = new int[enumerable.Count()];
            int index = 0;
            foreach (string item in enumerable)
                if (!int.TryParse(item, out result[index++]))
                    return false;
            return true;
        }

        public static bool ContainsMultiple<T>(this IEnumerable<T> enumerable, params T[] values)
        {
            int number = 0;
            foreach (T item in enumerable)
                if (values.Any(a => item.Equals(a)))
                    number += 1;
            return number > 1;
        }
        public static bool ContainsMultiple(this string[] value, params string[] values)
            => value.Select(s => s.ToLowerInvariant()).ContainsMultiple(values);

        public static int IndexOfAny<T>(this IEnumerable<T> enumerable, params T[] values)
        {
            int index = 0;
            foreach (T item in enumerable)
            {
                if (values.Any(a => item.Equals(a)))
                    return index;
                index += 1;
            }
            return -1;
        }
        public static IEnumerable<T> NotIn<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            List<T> list = new List<T>();
            foreach (T item in enumerable)
                if (predicate(item))
                    list.Add(item);
            return list;
        }

        public static bool ToEnum<T>(this string value, out T state)
            where T : struct
        {
            bool succes = Enum.TryParse(value, true, out T mstate);
            state = mstate;
            return succes;
        }

        public static T Previous<T>(this T src)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(typeof(T));
            int j = Array.IndexOf<T>(Arr, src) - 1;
            if (j < 0)
                throw new ArgumentException(String.Format("Argument {0} does not have a previous Enum", typeof(T).FullName));
            return Arr[j];
        }
    }
}
