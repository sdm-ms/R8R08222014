using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;

using System.Diagnostics;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;


namespace ClassLibrary1.Model
{

    public static class PickTblRows
    {

        public static List<TblRow> GetMostActiveTblRowsFromVariousTbls(IRaterooDataContext theDataContext, VolatilityDuration theTimeFrame,  RoutingInfoMainContent theLocation, int numTbls, int numToConsiderPerTbl, int numToReturnPerTbl)
        {
            try
            {
                bool disableCaching = false;
                string cacheString = "MostActiveTblRowsVariousTbls" + theLocation.GetHashString() + theTimeFrame.ToString();
                List<TblRow> queryToReturn = CacheManagement.GetItemFromCache(cacheString) as List<TblRow>;
                if (queryToReturn != null && !disableCaching)
                    return queryToReturn;

                Expression<Func<Tbl, bool>> thePredicate = PredicateBuilder.True<Tbl>();
                thePredicate = thePredicate.And<Tbl>(x => x.Status == (int)StatusOfObject.Active && x.Name != "Changes" && x.AllowOverrideOfRatingGroupCharacterstics == false && x.OneRatingPerRatingGroup);
                List<Tbl> randomizedList = GetRandomizedListOfTbls(theDataContext, numTbls, thePredicate, (theLocation == null) ? null : theLocation.lastItemInHierarchy);

                IEnumerable<List<TblRow>> theListsToCombine = randomizedList.Select(x => GetMostActiveTblRows(theDataContext, x, theTimeFrame, numToConsiderPerTbl, numToReturnPerTbl));

                List<TblRow> finalList = MergeLists(theListsToCombine);

                TimeSpan timeToKeep;
                if (theTimeFrame == VolatilityDuration.oneHour)
                    timeToKeep = new TimeSpan(0, 10, 0);
                else
                    timeToKeep = new TimeSpan(1, 0, 0);
                CacheManagement.AddItemToCache(cacheString, new string[] { }, finalList, timeToKeep);
                return finalList;
            }
            catch
            {
                return new List<TblRow>();
            }
        }

        public static List<TblRow> GetMostHighlyRatedTblRowsFromVariousTbls(IRaterooDataContext theDataContext, RoutingInfoMainContent theLocation, int numTbls, int numToConsiderPerTbl, int numToReturnPerTbl)
        {
            try
            {
                bool disableCaching = false;
                string cacheString = "MostHighlyRatedTblRowsVariousTbls" + theLocation.GetHashString();
                List<TblRow> queryToReturn = CacheManagement.GetItemFromCache(cacheString) as List<TblRow>;
                if (queryToReturn != null && !disableCaching)
                    return queryToReturn;

                Expression<Func<Tbl, bool>> thePredicate = PredicateBuilder.True<Tbl>();
                thePredicate = thePredicate.And<Tbl>(x =>
                    x.Status == (int)StatusOfObject.Active &&
                    x.Name != "Changes" &&
                    x.TblRows.Any() &&
                    x.TblTabs.Any() &&
                    x.TblTabs.First().TblColumns
                        .Where(cd => cd.RatingGroupAttribute.RatingCharacteristic.Name.StartsWith("Rating"))
                        .Count() >= 3
                    );
                List<Tbl> randomizedList = GetRandomizedListOfTbls(theDataContext, numTbls, thePredicate, (theLocation == null) ? null : theLocation.lastItemInHierarchy);

                IEnumerable<List<TblRow>> theListsToCombine = randomizedList.Select(x => GetMostHighlyRatedTblRows(theDataContext, x, numToConsiderPerTbl, numToReturnPerTbl));

                List<TblRow> finalList = MergeLists(theListsToCombine);

                TimeSpan timeToKeep = new TimeSpan(1, 0, 0);
                CacheManagement.AddItemToCache(cacheString, new string[] { }, finalList, timeToKeep);
                return finalList;
            }
            catch
            {
                return new List<TblRow>();
            }

        }

        public static List<T> MergeLists<T>(IEnumerable<List<T>> theListsToCombine)
        {
            List<T> finalList = new List<T>();
            bool moreToDo = true;
            int j = -1;
            while (moreToDo)
            { // mix up the order, so we don't have all from one to combine in a row
                moreToDo = false;
                j++;
                foreach (List<T> aList in theListsToCombine)
                {
                    if (aList.Count() > j)
                    {
                        finalList.Add(aList[j]);
                        moreToDo = true;
                    }
                }
            }
            return finalList;
        }

        public static List<Tbl> GetRandomizedListOfTbls(IRaterooDataContext theDataContext, int numTbls, Expression<Func<Tbl, bool>> thePredicate, HierarchyItem fromBelowThisItem = null)
        {
            try
            {
                // Create a randomized list of Tbls
                var tablesQuery = HierarchyItems.GetTblsUnderneathHierarchyItem(theDataContext, fromBelowThisItem).AsQueryable();
                var tablesQueryNarrowed = tablesQuery.Where(thePredicate).Where(x => x.PointsManager.UsersRights.Any(y => y.Status == (int)StatusOfObject.Active && y.User == null && y.MayView && y.MayPredict)).Take(Math.Max(100, numTbls));
                List<Tbl> bigList = tablesQueryNarrowed.ToList();
                Tbl[] bigListCopy = bigList.ToArray();
                foreach (Tbl theTbl in bigListCopy)
                {
                    DateTime aDayAgo = TestableDateTime.Now - new TimeSpan(7, 0, 0, 0);
                    // Not clear why this is running fast while the Any() approach keeps hitting a timeout.
                    //var anyURG = theTbl.TblRows.SelectMany(x => x.RatingGroups).SelectMany(x => x.UserRatingGroups).Where(x => x.WhenMade > aDayAgo).Any();
                    //var anyURG = theTbl.TblRows.Any(x => x.RatingGroups.Any(y => y.UserRatingGroups.Any(z => z.WhenMade > aDayAgo)));
                    var anyGroups = theDataContext.GetTable<UserRatingGroup>().Where(pg => pg.RatingGroup.TblRow.Tbl == theTbl && pg.WhenMade > aDayAgo).Count() > 0;
                    if (!anyGroups)
                        bigList.Remove(theTbl);
                }
                //if (!bigList.Any())
                //    bigList = theDataContext.GetTable<Tbl>().Where(thePredicate).Take(Math.Max(100, numTbls)).ToList();

                List<Tbl> randomizedList = GetRandomMembersOfList(numTbls, bigList);
                return randomizedList;
            }
            catch
            {
                try
                {
                    return theDataContext.GetTable<Tbl>().Where(thePredicate).Take(Math.Max(100, numTbls)).ToList();
                }
                catch
                {
                    return new List<Tbl>();
                }
            }
        }

        private static List<T> GetRandomMembersOfList<T>(int numToGet, List<T> bigList)
        {
            List<T> randomizedList = new List<T>();
            for (int i = 1; i <= numToGet; i++)
            {
                if (bigList.Any())
                {
                    int theNum = RandomGenerator.GetRandom(0, bigList.Count() - 1);
                    randomizedList.Add(bigList[theNum]);
                    bigList.RemoveAt(theNum);
                }
            }
            return randomizedList;
        }

        public static List<TblRow> GetMostActiveTblRows(IRaterooDataContext theDataContext, Tbl theTbl, VolatilityDuration theTimeFrame, int numToConsider, int numToReturn)
        {
            bool disableCaching = false; 
            string cacheString = "MostActiveTblRows" + theTbl.GetHashString() + theTimeFrame.ToString();
            List<TblRow> queryForTblRowsToConsider = CacheManagement.GetItemFromCache(cacheString) as List<TblRow>;
            if (queryForTblRowsToConsider != null && !disableCaching)
                return queryForTblRowsToConsider;

            IQueryable<VolatilityTblRowTracker> theQuery = null;

            theQuery = theDataContext.GetTable<VolatilityTblRowTracker>().Where(x => x.TblRow.Tbl == theTbl);
            theQuery = theQuery.Where(t => t.DurationType == (int)theTimeFrame).OrderByDescending(t => t.Pushback);
            theQuery = theQuery.Take((int) numToConsider); 
            IQueryable<TblRow> theQueryAsTblRows = theQuery.Select(t => t.TblRow);
            theQueryAsTblRows = theQueryAsTblRows.Distinct().Take((int)numToConsider);
            queryForTblRowsToConsider = theQueryAsTblRows.ToList();
            List<TblRow> queryToReturn = GetRandomMembersOfList(numToReturn, queryForTblRowsToConsider);
            TimeSpan timeToKeep;
            if (theTimeFrame == VolatilityDuration.oneHour)
                timeToKeep = new TimeSpan(0, 10, 0);
            else
                timeToKeep = new TimeSpan(1, 0, 0);
            CacheManagement.AddItemToCache(cacheString, new string[] { }, queryToReturn, timeToKeep);
            return queryToReturn;
        }

        public static List<TblRow> GetMostHighlyRatedTblRows(IRaterooDataContext theDataContext, Tbl theTbl, int numToConsider, int numToReturn)
        {
            bool disableCaching = false; 
            string cacheString = "MostHighlyRatedTblRows" + theTbl.TblID.ToString();
            List<TblRow> queryForTblRowsToConsider = CacheManagement.GetItemFromCache(cacheString) as List<TblRow>;
            if (queryForTblRowsToConsider != null && !disableCaching)
                return queryForTblRowsToConsider;

            IQueryable<RatingGroup> theQuery = null;

            theQuery = theDataContext.GetTable<RatingGroup>().Where(mg => mg.TblRow.Tbl == theTbl && mg.TblColumn == theTbl.TblTabs.First().TblColumns.First(cd => cd.RatingGroupAttribute.Name.StartsWith("Rating"))).OrderByDescending(mg => mg.CurrentValueOfFirstRating);
            IQueryable<TblRow> theQueryAsTblRows = theQuery.Take(numToReturn * 2).Select(mg => mg.TblRow);
            theQueryAsTblRows = theQueryAsTblRows.Distinct().Take((int)numToReturn);
            queryForTblRowsToConsider = theQueryAsTblRows.ToList();
            List<TblRow> queryToReturn = GetRandomMembersOfList(numToReturn, queryForTblRowsToConsider);
            TimeSpan timeToKeep = new TimeSpan(3, 0, 0);
            CacheManagement.AddItemToCache(cacheString, new string[] { }, queryToReturn, timeToKeep);
            return queryToReturn;
        }
    }
}