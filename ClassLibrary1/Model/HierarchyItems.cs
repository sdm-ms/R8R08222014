using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Diagnostics;
using MoreStrings;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{

    public static class HierarchyItems
    {
        public static HierarchyItem GetHierarchyFromStrings(string[] theStrings, out string[] remainderOfHierarchy)
        {
            remainderOfHierarchy = null;
            if (theStrings.Count() == 1 && theStrings[0] == "")
                return null;
            R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
            int i = 0;
            int length = theStrings.Count();
            HierarchyItem higherItem = null;
            while (i <= length - 1)
            {
                remainderOfHierarchy = theStrings.Skip(i).ToArray();
                HierarchyItem nextItem;
                /* NOTE: It would seem that the special case for higherItem == null would not be needed. But Linq to SQL produces the same SQL code regardless of whether higherItem == null, */
                /* trying to reference the HigherHierarchyItemForRoutingID field on a null object. */
                /* The try/catch loop also ought not be necessary, but we are getting sporadic unexplained errors (invalid cast exception) here. */
                /* This also explains why we are copying the id into a local variable. */
                try
                {
                    if (higherItem == null)
                        nextItem = theDataAccessModule.DataContext.NewOrFirstOrDefault<HierarchyItem>(x => x.HierarchyItemName == theStrings[i] && (x.HigherHierarchyItemID == null));
                    else
                    {
                        //if (higherItem.HierarchyItemID == null)
                        //    throw new Exception("GetHierarchyFromStrings internal error: higherItem.HierarchyItemID was null. "); /* Could occur if we add a new hierarchy item and immediately search for it, but this does not seem likely to explain error received (?) */
                        int higherItemHierarchyID = (int) higherItem.HierarchyItemID;
                        nextItem = theDataAccessModule.DataContext.NewOrFirstOrDefault<HierarchyItem>(x => x.HierarchyItemName == theStrings[i] && (x.HigherHierarchyItemID == higherItemHierarchyID));
                    }
                }
                catch
                {
                    return higherItem;
                }
                if (nextItem == null)
                    return higherItem;
                higherItem = nextItem;
                i++;
            } 
            remainderOfHierarchy = theStrings.Skip(i).ToArray();
            return higherItem;
        }

        public static void SetFullHierarchy(ref HierarchyItem theItem)
        {
            ItemPath theItemPath = new ItemPath();
            theItemPath.Setup(theItem);
            // The full hierarchy is what is used in the topics menu.
            theItem.FullHierarchyNoHtml = theItemPath.BuildItemPath(false);
            theItem.FullHierarchyWithHtml = theItemPath.BuildItemPath(true);
            // The route is what is used in the URL and may be shorter than the full hierarchy, because a single word may designate what the item is relatively accurately.
            theItem.RouteToHere = BuildRouteToItem(theItem);
        }

        public static string BuildRouteToItem(HierarchyItem theItem)
        {
            return Routing.Outgoing(new RoutingInfoMainContent(theItem));
        }

        public static HierarchyItem GetItemParent(HierarchyItem theItem)
        {
            /* Note: Because of extensive problems, we're not using automatic association properties on this table. */
            int targetHierarchyItemID;
            if (theItem.HigherHierarchyItemID == null)
                return null;
            targetHierarchyItemID = (int)theItem.HigherHierarchyItemID;
            R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
            return theDataAccessModule.DataContext.GetTable<HierarchyItem>().Single(x => x.HierarchyItemID == targetHierarchyItemID);
        }


        public static IEnumerable<HierarchyItem> GetItemChildren(HierarchyItem theItem)
        {
            /* Note: Because of extensive problems, we're not using automatic association properties on this table. */

            R8RDataManipulation theDataAccessModule = new R8RDataManipulation(); 
            return theDataAccessModule.DataContext.GetTable<HierarchyItem>().Where(x => x.HigherHierarchyItemID == theItem.HierarchyItemID);
            
        }

        public static List<HierarchyItem> GetHierarchyAsList(HierarchyItem theItem, bool includeOnlyItemsInMenu = false)
        {
            List<HierarchyItem> theList = new List<HierarchyItem>();
            if (includeOnlyItemsInMenu && !theItem.IncludeInMenu)
                return theList;
            theList.Add(theItem);
            HierarchyItem higherItem = GetItemParent(theItem);
            while (higherItem != null)
            {
                theList.Add(higherItem);
                higherItem = GetItemParent(higherItem);
            }
            theList.Reverse();
            return theList;
        }


        public static List<HierarchyItem> GetHierarchyAsListForTbl(Tbl theTbl)
        {
            HierarchyItem theItem = GetHierarchyItemForTbl(theTbl);
            return GetHierarchyAsList(theItem);
        }

        public static HierarchyItem GetHierarchyItemForTbl(Tbl theTbl)
        {
            string cacheKey = "HierarchyItemsForTbl" + (theTbl == null ? "null" : theTbl.TblID.ToString());
            HierarchyItem theItem = (theTbl == null ? null : theTbl.HierarchyItems.SingleOrDefault());
            return theItem;
        }

        public static List<Tbl> GetTblsUnderneathHierarchyItem(IR8RDataContext theDataContext, HierarchyItem theItem)
        {
            return GetHierarchyUnderneathHierarchyItem(theDataContext, theItem).AsQueryable().Where(x => x.Tbl != null).Select(x => x.Tbl).ToList();
        }

        public static List<HierarchyItem> GetHierarchyUnderneathHierarchyItem(IR8RDataContext theDataContext, HierarchyItem theItem)
        { 
            string cacheKey = "HierarchyUnderneath" + theItem.GetHashString();
            List<HierarchyItem> theList = CacheManagement.GetItemFromCache(cacheKey) as List<HierarchyItem>;
            if (theList != null)
                return theList;
            theList = new List<HierarchyItem>();
            IQueryable<HierarchyItem> itemsBelow;
            if (theItem == null)
                itemsBelow = theDataContext.GetTable<HierarchyItem>().Where(x => x.HigherHierarchyItemID == null && x.IncludeInMenu);
            else
                itemsBelow = theDataContext.GetTable<HierarchyItem>().Where(x => x.HigherHierarchyItemID == theItem.HierarchyItemID && x.IncludeInMenu);
            foreach (var itemBelow in itemsBelow)
            {
                theList.Add(itemBelow);
                theList.AddRange(GetHierarchyUnderneathHierarchyItem(theDataContext, itemBelow));
            }
            CacheManagement.AddItemToCache(cacheKey, new string[] { }, theList);
            return theList;
        }

        private class PrizeAndDateInfo
        {
            public decimal thePrize;
            public DateTime? theDate;
        }

        public static void GetPrizeInfoForHierarchyItem(HierarchyItem theItem, out decimal totalPrize, out DateTime? concludingDate)
        {
            if (theItem == null)
            {
                totalPrize = 0;
                concludingDate = null;
                return;
            }

            string cacheKey = "PrizeAndDateInfo" + theItem.GetHashCode();
            PrizeAndDateInfo theInfo = CacheManagement.GetItemFromCache(cacheKey) as PrizeAndDateInfo;

            if (theInfo != null)
            {
                totalPrize = theInfo.thePrize;
                concludingDate = theInfo.theDate;
                return;
            }

            if (theItem.Tbl == null)
            {
                totalPrize = 0;
                foreach (var item in GetItemChildren(theItem))
                {
                    decimal addAmount;
                    DateTime? lastDate;
                    GetPrizeInfoForHierarchyItem(item, out addAmount, out lastDate);
                    totalPrize += addAmount;
                }
                concludingDate = null;
            }
            else
            {
                totalPrize = theItem.Tbl.PointsManager.CurrentPeriodDollarSubsidy;
                concludingDate = theItem.Tbl.PointsManager.EndOfDollarSubsidyPeriod;
            }
            theInfo = new PrizeAndDateInfo();
            theInfo.thePrize = totalPrize;
            theInfo.theDate = concludingDate;
            CacheManagement.AddItemToCache(cacheKey, new string[] { }, theInfo, new TimeSpan(0, 10, 0));
        }

    }
}