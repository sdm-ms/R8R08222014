using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject1
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            return list.Select((item, index) => new {Index=index, Item=item})
                .GroupBy(itemAndIndex => itemAndIndex.Index % parts)
                .Select(group => group.Select(itemAndIndex => itemAndIndex.Item));
        }
    }
}
