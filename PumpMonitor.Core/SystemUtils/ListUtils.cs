using System;
using System.Collections.Generic;

namespace PumpMonitor.Core.SystemUtils
{
    public static class ListUtils
    {
        private static int BinarySearch<T>(IList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            
            var comp = Comparer<T>.Default;
            
            int lo = 0, hi = list.Count - 1;
            
            while (lo < hi) {
                int m = (hi + lo) / 2;
                if (comp.Compare(list[m], value) < 0) lo = m + 1;
                else hi = m - 1;
            }
            
            if (comp.Compare(list[lo], value) < 0) lo++;
            
            return lo;
        }

        public static int FindFirstIndexGreaterThanOrEqualTo<T,TU>(this SortedList<T,TU> sortedList, T key)
        {
            return BinarySearch(sortedList.Keys, key);
        }
    }
}