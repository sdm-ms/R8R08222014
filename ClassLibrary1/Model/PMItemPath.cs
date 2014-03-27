using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;


namespace ClassLibrary1.Model
{
    [Serializable]
    public class AutoCompleteData
    {
        public string ItemPath;
        public string Item;
        public string RouteToPage;
    }

    public static class PMItemPathWrapper
    {
        public static AutoCompleteData GetAutoCompleteData(object theObject)
        {
            return GetAutoCompleteData(theObject as HierarchyItem, theObject as TblRow, theObject as Rating);
        }

        public static AutoCompleteData GetAutoCompleteData(HierarchyItem theHierarchyItem, TblRow theTblRow, Rating theRating)
        {
            PMItemPath theItemPath = new PMItemPath();
            if (theHierarchyItem != null)
                theItemPath.Setup(theHierarchyItem);
            else if (theTblRow != null)
                theItemPath.Setup(theTblRow.Tbl, theTblRow, theRating);
            else if (theRating != null)
                theItemPath.Setup(theRating.RatingGroup.TblRow.Tbl, theRating.RatingGroup.TblRow, theRating);
            return theItemPath.GetAutoCompleteData();
        }

        public static string GetItemPath(object theObject, bool withHtml)
        {
            return GetItemPath(theObject as HierarchyItem, theObject as Tbl, theObject as TblRow, theObject as Rating, withHtml);
        }

        public static string GetItemPath(HierarchyItem theHierarchyItem, Tbl theTbl, TblRow theTblRow, Rating theRating, bool withHtml)
        {
            PMItemPath theItemPath = new PMItemPath();
            if (theHierarchyItem != null)
                theItemPath.Setup(theHierarchyItem);
            else
                theItemPath.Setup(theTbl, theTblRow, theRating);
            return theItemPath.GetItemPath(withHtml);
        }

        public static string GetPageForItem(object theObject)
        {
            return GetPageForItem(theObject as Tbl, theObject as TblRow, theObject as Rating);
        }

        public static string GetPageForItem(Tbl theTbl, TblRow theTblRow, Rating theRating)
        {
            PMItemPath theItemPath = new PMItemPath();
            theItemPath.Setup(theTbl, theTblRow, theRating);
            return theItemPath.GetPageForItem();
        }
    }

    /// <summary>
    /// Summary description for PMItemPath
    /// </summary>
    public class PMItemPath
    {
        protected class PMItemPathInfo
        {
            public PMRoutingInfoMainContent theLocation;
            public Rating theSpecificRating;

            public PMItemPathInfo(HierarchyItem theItem)
            {
                theLocation = PMRoutingInfoMainContentFactory.GetRoutingInfo(theItem);
            }

            public PMItemPathInfo(Tbl theTbl, TblRow theTblRow, Rating theRating, TblColumn theTblColumn)
            {
                theLocation = PMRoutingInfoMainContentFactory.GetRoutingInfo(theTbl, theTblRow, theTblColumn);
                if (theRating != null)
                {
                    int[] ratingGroupsNeedingSpecificationOfSpecificRating = { (int)RatingGroupTypes.hierarchyNumbersBelow, (int)RatingGroupTypes.hierarchyNumbersTop, (int)RatingGroupTypes.probabilityHierarchyBelow, (int)RatingGroupTypes.probabilityHierarchyTop, (int)RatingGroupTypes.probabilityMultipleOutcomes, (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy };
                    if (ratingGroupsNeedingSpecificationOfSpecificRating.Contains(theRating.RatingGroup.TypeOfRatingGroup))
                        theSpecificRating = theRating;
                }
            }

            public PMRoutingInfoMainContent GetLocationInfo()
            {
                return theLocation;
            }
        }


        protected PMItemPathInfo theItemPathInfo;
        public bool SuppressTable = false;
        public bool SuppressRow = false;

        public void Setup(HierarchyItem theHierarchyItem)
        {
            theItemPathInfo = new PMItemPathInfo(theHierarchyItem);
        }

        public void Setup(Tbl theTbl, TblRow theTblRow, Rating theRating)
        {
            Setup(theTbl, theTblRow, theRating, null);
        }

        public void Setup(Tbl theTbl, TblRow theTblRow, Rating theRating, TblColumn theTblColumn)
        {
            if (theRating != null && theTblRow == null)
                theTblRow = theRating.RatingGroup.TblRow;
            if (theTblRow != null && theTbl == null)
                theTbl = theTblRow.Tbl;

            theItemPathInfo = new PMItemPathInfo(theTbl, theTblRow, theRating, (theTblColumn != null ? theTblColumn : ((theRating == null) ? null : theRating.RatingGroup.TblColumn)));
        }

        public void Suppress(bool table, bool row)
        {
            SuppressTable = table;
            SuppressRow = row;
        }

        internal string AddColon(bool prefixColon, bool withHtml)
        {
            if (!prefixColon)
                return "";
            if (withHtml)
                return "&nbsp;:&nbsp;";
            return " : ";
        }

        internal void AppendPathPart(StringBuilder theStringBuilder, string innerText, string virtualPath, bool prefixColon, bool withHtml)
        {
            if (withHtml)
                theStringBuilder.Append(AddColon(prefixColon, withHtml) + "<a href=" + virtualPath + ">" + innerText + "</a>");
            else
                theStringBuilder.Append(AddColon(prefixColon, withHtml) + innerText);
        }

        public void AppendSpecificRatingFromGroupToPath(StringBuilder SB, string routeToTblRow, bool withHtml)
        {
            AppendPathPart(SB, theItemPathInfo.theSpecificRating.Name, routeToTblRow, true, withHtml);
        }

        public void AppendTblColumnToPath(StringBuilder SB, string routeToHere, bool withHtml)
        {
            if (theItemPathInfo.theLocation.theTblColumn != null)
                AppendPathPart(SB, theItemPathInfo.theLocation.theTblColumn.Name, routeToHere, !SuppressTable || !SuppressRow, withHtml);
        }

        public string GetItemPath(bool withHtml)
        {
            StringBuilder SB = new StringBuilder();
            PMRoutingInfoMainContent theLocation = theItemPathInfo.GetLocationInfo();
            if (!SuppressTable)
            {
                if (withHtml)
                    SB.Append(theLocation.lastItemInHierarchy.FullHierarchyWithHtml);
                else
                    SB.Append(theLocation.lastItemInHierarchy.FullHierarchyNoHtml);
            }
            string routeToTblRow = "";
            if (!SuppressRow)
            {
                if (theLocation.theTblRow != null)
                {
                    routeToTblRow = PMRouting.Outgoing(PMRoutingInfoMainContentFactory.GetRoutingInfo(theLocation.lastItemInHierarchy, theLocation.theTblRow));
                    AppendPathPart(SB, theLocation.theTblRow.Name, routeToTblRow, !SuppressTable, withHtml);
                }
            }
            if (theLocation.theTblColumn != null)
                AppendTblColumnToPath(SB, PMRouting.Outgoing(theItemPathInfo.theLocation), withHtml);
            if (theItemPathInfo.theSpecificRating != null)
                AppendSpecificRatingFromGroupToPath(SB, routeToTblRow, withHtml);
            return SB.ToString();
        }

        public string BuildItemPath(bool withHtml)
        {
            StringBuilder SB = new StringBuilder();
            PMRoutingInfoMainContent theLocation = theItemPathInfo.GetLocationInfo();
            List<string> names, routes;
            theLocation.GetInfoAllLevels(out names, out routes);
            int levels = names.Count();
            for (int level = 1; level <= levels; level++)
                AppendPathPart(SB, names[level - 1], routes[level - 1], level != 1, withHtml);
            if (theLocation.theTblColumn != null)
                AppendTblColumnToPath(SB, PMRouting.Outgoing(theItemPathInfo.theLocation), withHtml);
            if (theItemPathInfo.theSpecificRating != null)
                AppendSpecificRatingFromGroupToPath(SB, PMRouting.Outgoing(PMRoutingInfoMainContentFactory.GetRoutingInfo(theLocation.lastItemInHierarchy, theLocation.theTblRow)), withHtml);
            return SB.ToString();
        }

        public string GetItemNameOnly(bool withHtml)
        {
            PMRoutingInfoMainContent theLocation = theItemPathInfo.GetLocationInfo();
            return theLocation.GetName();
        }

        public string GetPageForItem()
        {
            PMRoutingInfoMainContent theLocation = theItemPathInfo.GetLocationInfo();
            return theLocation.GetOutgoingRoute();
        }

        public AutoCompleteData GetAutoCompleteData()
        {
            return new AutoCompleteData
            {
                Item = GetItemNameOnly(false),
                ItemPath = GetItemPath(false),
                RouteToPage = GetPageForItem()
            };
        }
    }
}