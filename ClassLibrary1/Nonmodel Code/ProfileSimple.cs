using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

/// <summary>
/// Summary description for ProfileSimple
/// </summary>
public static class ProfileSimple
{
    public static Dictionary<string, Stopwatch> activeProfiles = new Dictionary<string, Stopwatch>();
    public static TimeSpan[] elapsedSum = new TimeSpan[20];

    public static void Reset()
    {
        activeProfiles = new Dictionary<string, Stopwatch>();
    }

    public static void Start(string key)
    {
        if (!activeProfiles.ContainsKey(key))
            activeProfiles.Add(key, null);
        if (activeProfiles[key] != null)
            Trace.WriteLine("Profiling " + key + " was already started.");
        else
        {
            Stopwatch theStopwatch = new Stopwatch();
            activeProfiles[key] = theStopwatch;
            theStopwatch.Reset();
            theStopwatch.Start();
        }
    }

    public static void End(string key)
    {
        if (!activeProfiles.ContainsKey(key))
        {
            Trace.WriteLine("Profiling " + key + " not currently active.");
            return;
        }
        Stopwatch theStopWatch = activeProfiles[key];
        if (theStopWatch != null) // occasionally null even after ContainsKey check
        {
            theStopWatch.Stop();
            TimeSpan elapsedTime = theStopWatch.Elapsed;
            int activeCount = activeProfiles.Where(x => x.Value != null).Count();
            double pctAccountedFor = elapsedSum[activeCount + 1].TotalMilliseconds / elapsedTime.TotalMilliseconds;
            elapsedSum[activeCount] += elapsedTime;
            elapsedSum[activeCount + 1] = TimeSpan.FromSeconds(0);
            if (activeCount == 0)
                elapsedSum[activeCount] = TimeSpan.FromSeconds(0);
            for (int i = 0; i < activeCount; i++)
                Trace.Write("   ");
            Trace.WriteLine("Time elapsed for " + key + ": " + elapsedTime + " " + pctAccountedFor * 100 +"% accounted for");
        }
        activeProfiles[key] = null;

    }
}
