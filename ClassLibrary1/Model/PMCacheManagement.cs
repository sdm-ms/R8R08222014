using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Caching;
using System.Diagnostics;
using ClassLibrary1.Misc;
using Microsoft.WindowsAzure.ServiceRuntime;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for PMCacheManagement
    /// </summary>
    public static class PMCacheManagement
    {
        public class NullSubstituteForCache
        {
        }

        //Note that we have deleted this routine, because there is otherwise no
        //guarantee that the item will stay cached as of the next call. This is
        //especially an issue for short-lived items (and is more likely during
        //debugging stepping through the code).
        //public static bool ItemIsInCache(string cacheKey)
        //{
        //    bool cachingOn = true;
        //    if (!cachingOn)
        //        return false;
        //    return HttpRuntime.Cache[cacheKey] != null;
        //}

        public static void ClearCache()
        {
            // clear the cache
            foreach (System.Collections.DictionaryEntry x in HttpRuntime.Cache)
                HttpRuntime.Cache.Remove((string)x.Key);
            HttpResponse.RemoveOutputCacheItem("/WebForms/HomePage.aspx");
            HttpResponse.RemoveOutputCacheItem("/WebForms/Main.aspx");
        }

        public static object GetItemFromCache(string cacheKey, out bool isInCacheButNull)
        {
            if (DisableCaching)
            {
                isInCacheButNull = false;
                return null;
            }
            isInCacheButNull = false;
            var theResult = HttpRuntime.Cache[cacheKey];
            if (theResult as NullSubstituteForCache != null)
            {
                isInCacheButNull = true;
                return null;
            }
            return theResult;

        }

        public static object GetItemFromCache(string cacheKey)
        {
            var theResult = HttpRuntime.Cache[cacheKey];
            if (theResult == null || theResult as NullSubstituteForCache != null)
                return null;
            return theResult;
        }

        public static void AddItemToCache(string theCacheKey, string[] theDependencyStrings, object theObject)
        {
            AddItemToCache(theCacheKey, theDependencyStrings, theObject, new TimeSpan(0, 120, 0));
        }

        public static bool DisableCaching = false;

        public static void AddItemToCache(string theCacheKey, string[] theDependencyStrings, object theObject, TimeSpan keepFor)
        {
            if (DisableCaching)
                return;
            CacheDependency theDependencies = null;
            theDependencies = CreateCacheDependencyFromStrings(theDependencyStrings);
            if (theObject == null)
                theObject = new NullSubstituteForCache();
            HttpRuntime.Cache.Add(theCacheKey, theObject, theDependencies, TestableDateTime.Now + keepFor, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        // Disable caching by the browser. This leaves unaffected caching by the server.
        public static void DisablePageCaching()
        {

            //Used for disabling page caching
            HttpContext.Current.Response.Cache.SetExpires(TestableDateTime.Now.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
        }

        // We have defined the following cache dependency strings. 
        // RatingGroupID: cached information for individual table cell
        // RatingsForTblRowIDAndTblTabID: cached information for row of table cells
        // CategoriesForTblID: info on category descriptors and groups
        // FieldForTblRowID, FieldInfoForPointsManagerID: cached info on fields for particular entities and field descriptors for universe
        // TblRowForTblID: cached info on entities in Tbl
        // TblID: information on the table itself
        // TopicsMenu (with no ID): the topics menu
        // CommentForTblRowID : Comments for an entity
        // DomainID (for anything miscellaneous within a domain not in above)
        private static CacheDependency CreateCacheDependencyFromStrings(string[] theDependencyStrings)
        {
            CacheDependency theDependencies = null;
            if (theDependencyStrings != null)
            {
                foreach (string theDependencyString in theDependencyStrings)
                {
                    if (HttpRuntime.Cache[theDependencyString] == null)
                        HttpRuntime.Cache[theDependencyString] = "SomeValue";
                }
                theDependencies = new CacheDependency(null, theDependencyStrings);
            }
            return theDependencies;
        }

        public static void InvalidateCacheDependency(string dependencyString)
        {
            //Trace.TraceInformation("Worker role -- invalidating cache dependency " + dependencyString);
            InvalidateCacheDependencyOnThisMachine(dependencyString);
        }

        // This may be called within a web role either if the web role is also doing the background processing,
        // or because a notification has been received from a worker role that this cache dependency must be
        // invalidated. 
        public static void InvalidateCacheDependencyOnThisMachine(string dependencyString)
        {
            if (HttpRuntime.Cache[dependencyString] != null)
                HttpRuntime.Cache.Remove(dependencyString);
        }

    }

    public static class CacheInvalidityNotification
    {
        internal class CacheInvalidityNotificationProcessor
        {
            TimeSpan minTimeBetweenReadChecks = new TimeSpan(0, 0, 5);
            TimeSpan minTimeBetweenDeleteChecks = new TimeSpan(0, 2, 0);
            DateTime? lastReadTime = null;
            DateTime? lastDeleteCheck = null;

            public List<string> GetNewNotifications()
            {
                DateTime currentTime = TestableDateTime.Now;
                if (lastReadTime == null)
                {
                    PMCacheManagement.ClearCache(); // should be near empty anyway
                    lastReadTime = currentTime;
                    return new List<string>();
                }
                if (currentTime - (DateTime)lastReadTime > minTimeBetweenReadChecks)
                {
                    //Trace.TraceInformation("Reading all notifications from " + lastReadTime.ToString());
                    List<string> theNotifications = AzureNotificationProcessor.ReadNewNotifications(lastReadTime, "cache");
                    lastReadTime = currentTime;
                    return theNotifications;
                }
                else
                    return new List<string>();
            }

        }

    }
}
