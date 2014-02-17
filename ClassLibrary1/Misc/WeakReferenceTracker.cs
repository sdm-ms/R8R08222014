using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Misc
{
    
    public static class WeakReferenceTracker
    {
        static List<WeakReference> weakRefs = new List<WeakReference>();
        static object lockObj = new object();
        public static bool Track = false;

        public static void AddWeakReferenceTo(object obj)
        {
            if (Track && obj != null)
            {
                lock (lockObj)
                {
                    weakRefs.Add(new WeakReference(obj));
                }
            }
        }

        public static void CheckUncollected()
        {
            if (Track)
            {
                GC.Collect();
                var uncollected = weakRefs.Where(x => x.IsAlive).ToList();
                Debug.WriteLine("Uncollected: " + uncollected.Count().ToString() + " of " + weakRefs.Count().ToString());
                //weakRefs = new List<WeakReference>(); // uncomment this to reset
            }
        }
    }
}
