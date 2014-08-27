using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Nonmodel_Code
{
    public static class DateTimeManip
    {
        public enum eRoundingDirection { up, down, nearest }

        public static DateTime RoundDateTime(DateTime dt, int minutes, eRoundingDirection direction)
        {
            TimeSpan t;

            switch (direction)
            {
                case eRoundingDirection.up:
                    t = (dt.Subtract(DateTime.MinValue)).Add(new TimeSpan(0, minutes, 0)); break;
                case eRoundingDirection.down:
                    t = (dt.Subtract(DateTime.MinValue)); break;
                default:
                    t = (dt.Subtract(DateTime.MinValue)).Add(new TimeSpan(0, minutes / 2, 0)); break;
            }

            return DateTime.MinValue.Add(new TimeSpan(0,
                   (((int)t.TotalMinutes) / minutes) * minutes, 0));
        }

        public static DateTime RoundDateTimeSeconds(DateTime dt, int seconds, eRoundingDirection direction)
        {
            TimeSpan t;

            switch (direction)
            {
                case eRoundingDirection.up:
                    t = (dt.Subtract(DateTime.MinValue)).Add(new TimeSpan(0, 0, seconds)); break;
                case eRoundingDirection.down:
                    t = (dt.Subtract(DateTime.MinValue)); break;
                default:
                    t = (dt.Subtract(DateTime.MinValue)).Add(new TimeSpan(0, 0, seconds / 2)); break;
            }

            return DateTime.MinValue.Add(TimeSpan.FromSeconds((double)(((long)t.TotalSeconds / seconds) * seconds)));
        }
    }
}
