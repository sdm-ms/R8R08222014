using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1.Misc
{
    public static class TimeSpanExtensions
    {
        public static long GetTotalWholeMilliseconds(this TimeSpan ts)
        {
            return (long)Math.Truncate(ts.TotalMilliseconds);
        }
    }
}
