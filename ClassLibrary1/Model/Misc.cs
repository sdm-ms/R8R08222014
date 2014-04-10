using System;
using System.Data;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

//namespace PredRatings
//{

namespace ClassLibrary1.Model
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


    public static class RandomGenerator
    {
        static Random r = null;
        public static int? _seedOverride = null;
        public static int? SeedOverride { get { return _seedOverride; } set { r = null; _seedOverride = value; } }

        /// <summary> 
        /// Get a Random object which is cached between requests. 
        /// The advantage over creating the object locally is that the .Next 
        /// call will always return a new value. If creating several times locally 
        /// with a generated seed (like millisecond ticks), the same number can be 
        /// returned. 
        /// </summary> 
        /// <returns>A Random object which is cached between calls.</returns> 
        internal static Random GetRandomObject(int seed)
        {
            if (HttpContext.Current != null)
                r = (Random)HttpContext.Current.Cache.Get("RandomNumber");
            if (r == null)
            {
                if (seed == 0)
                    r = new Random();
                else
                    r = new Random(seed);
                if (HttpContext.Current != null)
                    HttpContext.Current.Cache.Insert("RandomNumber", r);
            }
            return r;
        }

        /// <summary> 
        /// GetRandom with no parameters. 
        /// </summary> 
        /// <returns>A Random object which is cached between calls.</returns> 
        internal static Random GetRandomObject()
        {
            return GetRandomObject(SeedOverride ?? (int)DateTime.Now.Ticks);
        }

        /// <summary> 
        /// Returns a double between 0 inclusive and 1 non-inclusive
        /// </summary> 
        /// <returns>A double between 0 and 1.</returns>
        public static double GetRandom()
        {
            Random r = GetRandomObject();
            lock (r)
            {
                return r.NextDouble();
            }
        }


        public static double GetRandom(double low, double high)
        {
            return low + GetRandom() * (high - low);
        }


        public static float GetRandom(float low, float high)
        {
            return low + ((float) GetRandom()) * (high - low);
        }

        public static decimal GetRandom(decimal low, decimal high)
        {
            return (decimal)GetRandom((double)low, (double)high);
        }

        public static DateTime GetRandom(DateTime first, DateTime last)
        {
            int seconds = (int)((last - first).TotalSeconds);
            int randSeconds = GetRandom(0, seconds);
            return first + new TimeSpan(0, 0, randSeconds);
        }

        /// <summary> 
        /// GetRandom with two integer parameters. 
        /// </summary> 
        /// <returns>A double between 0 and 1.</returns>
        public static int GetRandom(int low, int high)
        {
            Random r = GetRandomObject();
            lock (r)
            {
                return r.Next(low, high + 1);
            }
        }
    }

    public static class ShuffleExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = RandomGenerator.GetRandom(0, n);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    //}
}