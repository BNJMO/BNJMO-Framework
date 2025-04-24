using System;
using System.Collections.Generic;

namespace BNJMO
{
    public static class Ext
    {
        private static Random random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool ContainedIn<T>(this T val, params T[] values) where T : struct
        {
            bool found = false;
            foreach (T t in values)
            {
                if (t.Equals(val))
                {
                    found = true;
                }
            }
            return found;
        }
    }

  
}
