using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Misc
{
    public static class MedianExtensions
    {
        public static double? Median<TColl, TValue>(
            this IEnumerable<TColl> source,
            Func<TColl, TValue> selector)
        {
            double? returnVal = source.Select<TColl, TValue>(selector).Median();

            if (returnVal == null)
                throw new Exception("Internal error.");

            return returnVal;
        }

        public static double? Median<T>(
            this IEnumerable<T> source)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
                source = source.Where(x => x != null);

            int count = source.Count();
            if (count == 0)
                return null;

            source = source.OrderBy(n => n);

            double? returnVal;

            int midpoint = count / 2;
            if (count % 2 == 0)
                returnVal = (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
            else
                returnVal = Convert.ToDouble(source.ElementAt(midpoint));

            if (returnVal == null)
                throw new Exception("Internal error");

            return returnVal;
        }
    }
}
