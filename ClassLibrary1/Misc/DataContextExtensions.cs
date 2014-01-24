using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Linq;
using System.Diagnostics;

namespace ClassLibrary1.Misc
{
    public static class DataContextExtensions
    {
        public static List<DataContextChangedItems<TItem>> GetChangedItems<TItem>
                      (this DataContext context)
        {
            // create a dictionary of type TItem for return to caller
            List<DataContextChangedItems<TItem>> changedItems = new List<DataContextChangedItems<TItem>>();

            System.Collections.IDictionary trackerItems = GetTrackerItems(context);

            // iterate through each item in context, adding
            // only those that are of type TItem to the changedItems dictionary
            foreach (System.Collections.DictionaryEntry entry in trackerItems)
            {
                object original = entry.Value.GetType().GetField("original",
                                  BindingFlags.NonPublic |
                                  BindingFlags.Instance |
                                  BindingFlags.GetField).GetValue(entry.Value);

                if (entry.Key is TItem && original is TItem)
                {
                    changedItems.Add(
                      new DataContextChangedItems<TItem>((TItem)entry.Key, (TItem)original)
                    );
                }
            }
            return changedItems;
        }

        public static List<DataContextChangedItems<object>> GetChangedItems
                    (this DataContext context)
        {
            // create a dictionary of type TItem for return to caller
            List<DataContextChangedItems<object>> changedItems = new List<DataContextChangedItems<object>>();

            System.Collections.IDictionary trackerItems = GetTrackerItems(context);

            // iterate through each item in context, adding
            // only those that are of type TItem to the changedItems dictionary
            foreach (System.Collections.DictionaryEntry entry in trackerItems)
            {
                object original = entry.Value.GetType().GetField("original",
                                  BindingFlags.NonPublic |
                                  BindingFlags.Instance |
                                  BindingFlags.GetField).GetValue(entry.Value);
                changedItems.Add(
                    new DataContextChangedItems<object>(entry.Key, original)
                  );
            }

            return changedItems;
        }

        private static System.Collections.IDictionary GetTrackerItems(this DataContext context)
        {
            // use reflection to get changed items from data context
            object services = context.GetType().BaseType.GetField("services",
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.GetField).GetValue(context);

            object tracker = services.GetType().GetField("tracker",
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.GetField).GetValue(services);

            System.Collections.IDictionary trackerItems =
              (System.Collections.IDictionary)tracker.GetType().GetField("items",
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.GetField).GetValue(tracker);
            return trackerItems;
        }

        public static bool ChangesExist
              (this DataContext context)
        {
            System.Collections.IDictionary trackerItems = GetTrackerItems(context);

            return trackerItems.Count > 0;
        }
    }

    public class DataContextChangedItems<TItem>
    {
        public DataContextChangedItems(TItem Current, TItem Original)
        {
            this.Current = Current;
            this.Original = Original;
        }
        public TItem Current { get; set; }
        public TItem Original { get; set; }
        public void DebugOutput()
        {
            Trace.TraceInformation("Original: " + ((Original == null) ? "null" : Original.ToString()) + " Current: " + ((Current == null) ? "null" : Current.ToString()));
        }
    }
}
