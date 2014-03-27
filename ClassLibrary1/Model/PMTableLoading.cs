using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;


using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;


using ClassLibrary1.Model;
using ClassLibrary1.Misc;
using System.Diagnostics;

namespace ClassLibrary1.Model
{

    /// <summary>
    /// Summary description for PMTableLoading
    /// </summary>
    public static class PMTableLoading
    {
        public static TablePopulateResponse PopulateTableInitially(string theTableInfoString, int visibleRows, int additionalRows, int rowsToSkip, bool includeHeaderRow)
        {
            return PopulateTable(theTableInfoString, rowsToSkip + 1, visibleRows + additionalRows, true, includeHeaderRow, null);
        }

        public static TablePopulateResponse PopulateTableSpecificRows(string theTableInfoString, int firstRowNum, int numRowsNeeded, int visibleRowsEstimate, int firstRowIfResetting)
        {
            return PopulateTable(theTableInfoString, firstRowNum, Math.Max(numRowsNeeded, visibleRowsEstimate), false, false, firstRowIfResetting);
        }

        public static TablePopulateResponse PopulateTable(string theTableInfoString, int firstRowNum, int numRows, bool populatingInitially, bool includeHeaderRow, int? firstRowIfResetting)
        {
            try
            {
                //ProfileSimple.Start("PopulateTable");
                string cacheString = MD5HashGenerator.GenerateKey(theTableInfoString + firstRowNum.ToString() + numRows.ToString() + includeHeaderRow.ToString());
                string[] myDependencies = { };
                if (populatingInitially)
                {
                    TablePopulateResponse cachedResponse = PMCacheManagement.GetItemFromCache(cacheString + "FullResponse") as TablePopulateResponse;
                    if (cachedResponse != null)
                        return cachedResponse;
                }

                RaterooDataAccess theDataAccess = new RaterooDataAccess();
                TableInfo theTableInfo = TableInfoToStringConversion.GetTableInfoFromString(theTableInfoString);
                string tableInfoForReset = "";
                CheckTableLoadingResetNeeded(theTableInfoString, ref firstRowNum, ref numRows, firstRowIfResetting, theDataAccess, theTableInfo, ref tableInfoForReset, populatingInitially);


                int? maxNumResults;
                int? rowCountOverride;
                TableSortRule theTableSortRule;
                TableLoadingCalculateMaxNumResults(firstRowNum, numRows, theDataAccess, theTableInfo, out maxNumResults, out rowCountOverride, out theTableSortRule);

                PointsManager thePointsManager = null;
                Tbl theTbl = null;
                TblTab theTblTab = null;
                GetTblAndPointsManagerForTblTab(theDataAccess, theTableInfo.TblTabID, out theTblTab, out theTbl, out thePointsManager);

                string theHeaderRow = "";
                if (includeHeaderRow)
                    theHeaderRow = LoadHeaderRowFromWebService(theDataAccess, theTableInfo.TblTabID, (theTableSortRule is TableSortRuleTblColumn ? (int?)((TableSortRuleTblColumn)theTableSortRule).TblColumnToSortID : null), theTableSortRule is TableSortRuleEntityName, theTableSortRule.Ascending, theTbl.TblID);

                //ProfileSimple.Start("GetTablePopulateResponse");
                TablePopulateResponse theResponse = GetTablePopulateResponse(firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, thePointsManager, theTbl, theHeaderRow, rowCountOverride);
                //ProfileSimple.End("GetTablePopulateResponse");

                if (firstRowNum == 1)
                    PMCacheManagement.AddItemToCache(cacheString + "FullResponse", myDependencies, theResponse, new TimeSpan(0, 0, 15));
                //ProfileSimple.End("PopulateTable");
                return theResponse;
            }
            catch (Exception ex)
            {
                string fullError = "";
                Exception ex2 = ex;
                int i = 0;
                while (ex2 != null && i < 10)
                {
                    fullError += ex2.Message;
                    ex2 = ex2.InnerException;
                }
                fullError += "Original request " + theTableInfoString;
                var serializer = new JavaScriptSerializer();
                string serializedErr = serializer.Serialize(fullError);

                TablePopulateResponse theResponse = new TablePopulateResponse
                {
                    mainRows = "",
                    success = false,
                    err = serializedErr,
                    rowCount = 0,
                    tableInfoForReset = ""
                };
                return theResponse;
            }

        }


        public static TblColumn GetTblColumn(RaterooDataAccess dataAccess, int TblColumnID, int domainID)
        {
            TblColumn theCD = PMCacheManagement.GetItemFromCache("TblColumnID" + TblColumnID) as TblColumn;
            if (theCD == null)
            {
                theCD = dataAccess.RaterooDB.GetTable<TblColumn>().Single(cd => cd.TblColumnID == TblColumnID);
                PMCacheManagement.AddItemToCache("TblColumnID" + TblColumnID, new string[] { "DomainID" + domainID, "TblID" + theCD.TblTab.TblID }, theCD);
            }
            return theCD;
        }

        public static void GetTblAndPointsManagerForTblTab(RaterooDataAccess dataAccess, int TblTabID, out TblTab theTblTab, out Tbl theTbl, out PointsManager thePointsManager)
        {
            theTblTab = PMCacheManagement.GetItemFromCache("TblTab" + TblTabID) as TblTab;
            bool addTblTabToCache = false;
            if (theTblTab == null)
            {
                theTblTab = dataAccess.RaterooDB.GetTable<TblTab>().SingleOrDefault(x => x.TblTabID == TblTabID);
                addTblTabToCache = true;
            }
            theTbl = PMCacheManagement.GetItemFromCache("TblForTblTab" + TblTabID) as Tbl;
            bool addTblToCache = false;
            if (theTbl == null)
            {
                theTbl = theTblTab.Tbl;
                if (theTbl == null)
                    throw new Exception("Sorry, an internal error loading information on this table occurred.");
                addTblToCache = true;
            }
            thePointsManager = PMCacheManagement.GetItemFromCache("PointsManagerForTbl" + theTbl.TblID) as PointsManager;
            if (thePointsManager == null)
            {
                thePointsManager = theTbl.PointsManager;
                PMCacheManagement.AddItemToCache("PointsManagerForTbl" + theTbl.TblID, new string[] { "DomainID" + thePointsManager.DomainID, "TblID" + theTbl.TblID }, theTbl);
            }
            if (addTblTabToCache)
                PMCacheManagement.AddItemToCache("TblTab" + TblTabID, new string[] { "DomainID" + thePointsManager.DomainID, "TblID" + theTbl.TblID }, theTblTab);
            if (addTblToCache)
                PMCacheManagement.AddItemToCache("TblForTblTab" + TblTabID, new string[] { "DomainID" + thePointsManager.DomainID, "TblID" + theTbl.TblID }, theTbl);
        }



        private static TablePopulateResponse GetTablePopulateResponse(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, RaterooDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCountOverride)
        {
            TablePopulateResponse theResponse = null;
            try
            {
                if (theTbl.FastTableSyncStatus != (int)FastAccessTableStatus.fastAccessNotCreated && PMFastAccessTablesQuery.FastAccessTablesEnabled() && !theTableInfo.Filters.theFilterRules.Any(x => x is SearchWordsFilterRule)) // we don't do fast queries if it is disabled, or (for now) if we have a search words query, which is too cumbersome to implement.
                {
                    //ProfileSimple.Start("GetTablePopulateResponseWithFastQueries");
                    DenormalizedTableAccess dta = new DenormalizedTableAccess(1);
                    theResponse = GetTablePopulateResponseWithFastQueries(dta, firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, thePointsManager, theTbl, headerRow, rowCountOverride);
                    //ProfileSimple.End("GetTablePopulateResponseWithFastQueries");
                }
                else
                    theResponse = GetTablePopulateResponseWithNormalizedQueries(firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, thePointsManager, theTbl, headerRow, rowCountOverride);
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetTablePopulateResponse produced error " + ex.Message);
            }
            return theResponse;
        }

        private static TablePopulateResponse GetTablePopulateResponseWithFastQueries(DenormalizedTableAccess dta, int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, RaterooDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCountOverride)
        {
            //ProfileSimple.Start("FastQueries intro");
            List<InfoForBodyRows> theInfoForBodyRows;
            int? rowCountFromQuery = null;
            PMFastAccessTablesQuery.DoQuery(dta, firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess.RaterooDB, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, theTbl, out theInfoForBodyRows, out rowCountFromQuery);

            int? rowCount = rowCountOverride ?? rowCountFromQuery;

            //ProfileSimple.End("FastQueries intro");
            return CompleteGetTablePopulateResponse(theDataAccess, theTableInfo, tableInfoForReset, thePointsManager, theTbl, headerRow, rowCount, firstRowNum, null, theInfoForBodyRows);
        }

        private static TablePopulateResponse GetTablePopulateResponseWithNormalizedQueries(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, RaterooDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCountOverride)
        {
            //ProfileSimple.Start("NormalizedQuery");
            TblRowsToPopulatePage theInfoToPopulatePage = GetTblRowsToPopulatePage(firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule);
            int? rowCount = rowCountOverride ?? theInfoToPopulatePage.rowCount;

            TablePopulateResponse theResponse = CompleteGetTablePopulateResponse(theDataAccess, theTableInfo, tableInfoForReset, thePointsManager, theTbl, headerRow, rowCount, theInfoToPopulatePage.numRowOfFirstTblRow, theInfoToPopulatePage);
            //ProfileSimple.End("NormalizedQuery");
            return theResponse;
        }

        private static TablePopulateResponse CompleteGetTablePopulateResponse(RaterooDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCount, int numRowOfFirstTblRow, TblRowsToPopulatePage theInfoToPopulatePage = null, List<InfoForBodyRows> bodyRowInfoList = null)
        {
            //ProfileSimple.Start("CompleteTablePopulateResponse");
            if (theInfoToPopulatePage == null && bodyRowInfoList == null)
                throw new Exception("Internal error: one of theInfoToPopulatePage and bodyRowInfoList should be null, not both.");

            //ProfileSimple.Start("GetTableDimension");
            TblDimension theTblDimension = GetTblDimension(theDataAccess, theTbl);
            //ProfileSimple.End("GetTableDimension");
            //ProfileSimple.Start("GetMainRowsString");
            string mainRowsString = GetMainRowsString(theDataAccess, theTableInfo, thePointsManager, theTbl, theTblDimension, numRowOfFirstTblRow, theInfoToPopulatePage, bodyRowInfoList);
            //ProfileSimple.End("GetMainRowsString");

            TablePopulateResponse theResponse = new TablePopulateResponse
            {
                mainRows = mainRowsString,
                headerRow = headerRow,
                success = true,
                err = "",
                rowCount = rowCount,
                tableInfoForReset = tableInfoForReset
            };

            //ProfileSimple.End("CompleteTablePopulateResponse");
            return theResponse;
        }

        private static TblDimension GetTblDimension(RaterooDataAccess theDataAccess, Tbl theTbl)
        {
            TblDimension theTblDimension = null;
            TblDimensionAccess theCssAccess = new TblDimensionAccess(theDataAccess);
            theTblDimension = theCssAccess.GetTblDimensionsForRegularTbl(theTbl.TblID);
            return theTblDimension;
        }

        private static string GetMainRowsString(RaterooDataAccess theDataAccess, TableInfo theTableInfo, PointsManager thePointsManager, Tbl theTbl, TblDimension theTblDimension, int numRowOfFirstTblRow, TblRowsToPopulatePage theInfoToPopulatePage = null, List<InfoForBodyRows> bodyRowInfoList = null)
        {
            //ProfileSimple.Start("GetMainRowsString beginning");
            StringBuilder theMainRows = new StringBuilder();
            int rowCount = 0;
            List<int> theTblRowIDs = null;
            List<bool> tblRowDeleted = null;
            if (theInfoToPopulatePage == null)
            {
                if (!bodyRowInfoList.Any())
                    return "";
                int aColumn = bodyRowInfoList.First().TblColumnID;
                theTblRowIDs = bodyRowInfoList.Where(x => x.TblColumnID == aColumn).Select(x => x.TblRowID).ToList();
                tblRowDeleted = bodyRowInfoList.Where(x => x.TblColumnID == aColumn).Select(x => x.Deleted).ToList();
                rowCount = theTblRowIDs.Count();
            }
            else if (bodyRowInfoList == null)
            {
                theTblRowIDs = theInfoToPopulatePage.theTblRows.Select(x => x.TblRowID).ToList();
                bodyRowInfoList = GetBodyRowsInfoListFromDatabase(theDataAccess, theTableInfo.TblTabID, theTblRowIDs);
                tblRowDeleted = theInfoToPopulatePage.theTblRows.Select(x => x.Status != (int)StatusOfObject.Active).ToList();
                rowCount = theInfoToPopulatePage.theTblRows.Count;
            }
            //ProfileSimple.End("GetMainRowsString beginning");
            //ProfileSimple.Start("GetBodyRowString loop");
            for (int i = 0; i < rowCount; i++)
            {
                string theRow = GetBodyRowString(theDataAccess, numRowOfFirstTblRow + i, theTblRowIDs[i], tblRowDeleted[i], theTbl.TblID, thePointsManager, theTableInfo.TblTabID, theTableInfo.SuppStyle, theTblDimension, bodyRowInfoList);
                theMainRows.Append(theRow);
            }
            //ProfileSimple.End("GetBodyRowString loop");
            string mainRowsString = theMainRows.ToString();
            return mainRowsString;
        }

        private static TblRowsToPopulatePage GetTblRowsToPopulatePage(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, RaterooDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule)
        {
            TblRowsToPopulatePage theInfoToPopulatePage = null;

            if (populatingInitially)
                theInfoToPopulatePage = (TblRowsToPopulatePage)PMCacheManagement.GetItemFromCache(cacheString + "TblRowsOnly");

            if (theInfoToPopulatePage == null)
            {
                bool sortByNameAfterTakingTop = false;
                bool nameAscending = true;
                if (populatingInitially)
                {
                    theInfoToPopulatePage = theTableInfo.Filters.GetQueryToPopulatePageInitially(theDataAccess.RaterooDB, maxNumResults,
                theTableSortRule, sortByNameAfterTakingTop, nameAscending, numRows, firstRowNum - 1);
                }
                else
                {
                    theInfoToPopulatePage = theTableInfo.Filters.GetQueryForSpecificRows(theDataAccess.RaterooDB, maxNumResults, theTableSortRule, sortByNameAfterTakingTop, nameAscending, (int)firstRowNum, (int)numRows, tableInfoForReset != "");
                }
                if (firstRowNum == 1)
                    PMCacheManagement.AddItemToCache(cacheString + "TblRowsOnly", myDependencies, theInfoToPopulatePage, new TimeSpan(0, 1, 0));
            }
            return theInfoToPopulatePage;
        }

        private static void TableLoadingCalculateMaxNumResults(int firstRowNum, int numRows, RaterooDataAccess theDataAccess, TableInfo theTableInfo, out int? maxNumResults, out int? rowCountOverride, out TableSortRule theTableSortRule)
        {
            maxNumResults = null;
            rowCountOverride = null;
            theTableSortRule = TableSortRuleGenerator.GetTableSortRuleFromStringRepresentation(theTableInfo.SortInstruction);
            if (theTableSortRule is TableSortRuleEntityName)
                maxNumResults = null;
            else if (theTableSortRule is TableSortRuleDistance)
            {
                if (theTableInfo.Filters.theFilterRules.Count() == 0)
                { // we want to limit the maxNumResults to the highest row number we're seeking, but we need to remember how many results there really are, so user can scroll
                    if (firstRowNum > 1)
                        maxNumResults = firstRowNum + (int)numRows - 1;
                    else
                        maxNumResults = numRows;
                    rowCountOverride = theDataAccess.RaterooDB.GetTable<TblRow>().Where(t => t.InitialFieldsDisplaySet && t.TblID == theTableInfo.TblID).Count();
                    if (rowCountOverride > 1000)
                        rowCountOverride = 1000;
                }
                else
                { // we're filtering, but we'll start (arbitrarily) with maxNumResults = 2000 before filtering, because we can't run our nearest-neighbors algorithm to take into account the other filters
                    maxNumResults = 2000;
                }
            }
            else
                maxNumResults = 5000;
        }

        private static void CheckTableLoadingResetNeeded(string theTableInfoString, ref int firstRowNum, ref int numRows, int? firstRowIfResetting, RaterooDataAccess theDataAccess, TableInfo theTableInfo, ref string tableInfoForReset, bool populatingInitially)
        {
            bool sortingAnew = (populatingInitially && TestableDateTime.Now - theTableInfo.Filters.AsOfDateTime < new TimeSpan(0, 0, 10));
            if (sortingAnew || theTableInfo.Filters.GetNewDateTime == true || theTableInfo.Filters.AsOfDateTime < TestableDateTime.Now - new TimeSpan(0, 29, 30))
            { // We're going to reset the request
                theTableInfo.Filters.AsOfDateTime = DateTimeManip.RoundDateTimeSeconds(TestableDateTime.Now, 15, DateTimeManip.eRoundingDirection.down); // Round down to nearest 15 seconds, so that we can cache effectively.
                theTableInfo.Filters.GetNewDateTime = false;
                if (firstRowIfResetting != null)
                {
                    numRows += (int)firstRowNum - (int)firstRowIfResetting;
                    firstRowNum = (int)firstRowIfResetting;
                }
            }
            bool trustedUser = theTableInfo.SortMenu.Any(x => x.I == "R" /* TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleNeedsRating()) */);
            var theSortMenu = SortMenuGenerator.GetSortMenuForTblTab(theDataAccess.RaterooDB, theTableInfo.TblTabID, trustedUser);
            theTableInfo.SortMenu = theSortMenu;
            string potentialTableInfoForReset = TableInfoToStringConversion.GetStringFromTableInfo(theTableInfo);
            if (potentialTableInfoForReset != theTableInfoString)
                tableInfoForReset = potentialTableInfoForReset;
        }



        public static string GetBodyRowString(RaterooDataAccess theDataAccess, int rowNumber, int theTblRowID, bool tblRowDeleted, int theTblID, PointsManager thePointsManager, int theTblTabID, string suppStyle, TblDimension theTblDimension, List<InfoForBodyRows> bodyRowInfoList)
        {
            int thePointsManagerID = thePointsManager.PointsManagerID;

            string entireRow = "<tr id=\"maintr{0}\" class=\"somerow prow {1}\"><td class=\"nmcl\">{2}</td><td><div class=\"rowHead\">{3}</div></td>{4}</tr>";
            string extraCSSClass, theRowHeading, theBodyOfRow;


            extraCSSClass = tblRowDeleted ? "deletedTblRow " : "";
            if (rowNumber % 2 == 0)
                extraCSSClass += "altrow";
            else
                extraCSSClass += "row";

            InfoForBodyRows theInfo = bodyRowInfoList.FirstOrDefault(x => x.TblRowID == theTblRowID);
            if (theInfo.RowHeadingWithPopup != null && theInfo.RowHeadingWithPopup != "")
                theRowHeading = theInfo.RowHeadingWithPopup;
            else
                theRowHeading = GetBodyRowHeading(theDataAccess, theTblDimension, theTblRowID, theTblID, thePointsManagerID);

            theBodyOfRow = GetColumnsOfBodyRowString(theTblTabID, suppStyle, theTblRowID, theTblID, bodyRowInfoList);

            entireRow = String.Format(entireRow, theTblRowID.ToString(), extraCSSClass, rowNumber.ToString(), theRowHeading, theBodyOfRow);
            return entireRow;
        }

        private static string GetColumnsOfBodyRowString(int theTblTabID, string suppStyle, int theTblRowID, int theTblID, List<InfoForBodyRows> bodyRowInfoQuery)
        {
            string theBodyOfRow;
            // currently disabling caching, because some of our dependencies are not working properly.
            //string myCacheKeyBodyRow = "MainTableBodyRow" + theTblRowID.ToString() + "," + theTblTabID.ToString() + "," + true.ToString() + "," + true.ToString();
            //theBodyOfRow = (string)PMCacheManagement.GetItemFromCache(myCacheKeyBodyRow);
            //if (theBodyOfRow == null)
            //{
            //if (simpleTblQuery != null)
            theBodyOfRow = GetUncachedColumnsOfBodyRowString(bodyRowInfoQuery, suppStyle, theTblRowID);
            //else
            //    theBodyOfRow = LoadBodyOfRowFromWebServiceForComplexTbl(theDataAccess, theTblTabID, suppStyle, theTblRowID, theTbl.TblID);
            //    string[] myDependencies = {
            //                            "RatingsForTblRowIDAndTblTabID" + theTblRowID.ToString() + "," + theTblTabID.ToString(),
            //                            "CategoriesForTblID" + theTblID.ToString()
            //                                              };
            //    PMCacheManagement.AddItemToCache(myCacheKeyBodyRow, myDependencies, theBodyOfRow);
            //}
            return theBodyOfRow;
        }

        private static List<InfoForBodyRows> GetBodyRowsInfoListFromDatabase(RaterooDataAccess theDataAccess, int theTblTabID, List<int> theTblRowIDs)
        {
            return theDataAccess.RaterooDB.GetTable<RatingGroup>()
                                                .Where(mg =>
                                                    theTblRowIDs.Contains(mg.TblRowID) &&
                                                    (mg.TypeOfRatingGroup != (int)RatingGroupTypes.hierarchyNumbersBelow && mg.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityHierarchyBelow) &&
                                                    mg.TblColumn.TblTabID == theTblTabID &&
                                                    mg.TblColumn.Status == (int)StatusOfObject.Active).
                                                OrderBy(mg => mg.TblColumn.CategoryNum).
                                                ThenBy(mg => mg.TblColumn.TblColumnID).
                                                Select(mg => new InfoForBodyRows
                                                {
                                                    TblRowFieldDisplay = mg.TblRow.TblRowFieldDisplay,
                                                    TblRowID = mg.TblRowID,
                                                    TblColumnID = mg.TblColumnID,
                                                    TopRatingGroupID = mg.RatingGroupID,
                                                    FirstRatingID = mg.Ratings.First().RatingID,
                                                    DecPlaces = mg.Ratings.First().RatingCharacteristic.DecimalPlaces,
                                                    ValueOfFirstRating = mg.CurrentValueOfFirstRating,
                                                    SingleNumberOnly = mg.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || mg.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || mg.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber,
                                                    Trusted = mg.Ratings.All(x => x.LastTrustedValue == x.CurrentValue),
                                                    Deleted = mg.TblRow.Status != (int)StatusOfObject.Active
                                                }).ToList();
        }

        private static string GetUncachedColumnsOfBodyRowString(List<InfoForBodyRows> bodyRowsList, string suppStyle, int theTblRowID)
        {
            StringBuilder build = new StringBuilder();
            var resultsForTblRow = bodyRowsList.Where(e => e.TblRowID == theTblRowID);
            foreach (var cell in resultsForTblRow)
            {
                BuildHtmlForCell(build, cell.TopRatingGroupID, cell.ValueOfFirstRating, cell.DecPlaces, cell.TblColumnID, cell.FirstRatingID, cell.PreformattedString, cell.Trusted, cell.SingleNumberOnly, true);
            }

            return build.ToString();

        }

        public static string GetHtmlStringForCell(int topRatingGroupID, decimal? valueOfFirstRating, int decPlaces, int tblColumnID, int? ratingIDIfSingleNumber, string preformattedContentsIfAvailable, bool trusted, bool singleNumberOnly, bool includeOuterTD)
        {
            StringBuilder builder = new StringBuilder();
            BuildHtmlForCell(builder, topRatingGroupID, valueOfFirstRating, decPlaces, tblColumnID, ratingIDIfSingleNumber, preformattedContentsIfAvailable, trusted, singleNumberOnly, includeOuterTD);
            return builder.ToString();
        }

        private static void BuildHtmlForCell(StringBuilder builder, int topRatingGroupID, decimal? valueOfFirstRating, int? decPlaces, int tblColumnID, int? ratingIDIfSingleNumber, string preformattedContentsIfAvailable, bool? trusted, bool singleNumberOnly, bool includeOuterTD)
        {
            string nameAndTopRatingGroupID = "mg" + topRatingGroupID + "grpC";
            if (includeOuterTD)
                builder.Append("<td class=\"mainCellMarker \">");
            builder.Append("<input name=\"" + nameAndTopRatingGroupID + "\" type=\"hidden\" id=\"" + nameAndTopRatingGroupID + "\" class=\"mgID\" value=\"" + topRatingGroupID + "\" />");
            if (singleNumberOnly)
                builder.Append(GetStringForRatingInput(valueOfFirstRating, decPlaces, tblColumnID, (int)ratingIDIfSingleNumber, trusted, preformattedContentsIfAvailable, "rtgMult"));
            else
            {
                if (preformattedContentsIfAvailable != null && preformattedContentsIfAvailable != "")
                    builder.Append(preformattedContentsIfAvailable);
                else
                    BuildHtmlForComplexCellFromDatabase(builder, topRatingGroupID, tblColumnID, (bool)trusted);
            }
            if (includeOuterTD)
                builder.Append("</td>");
        }

        private static void BuildHtmlForComplexCellFromDatabase(StringBuilder builder, int topRatingGroupID, int tblColumnID, bool trusted)
        {
            RaterooDataAccess DataAccess = new RaterooDataAccess();
            RatingHierarchyData theData = DataAccess.GetRatingHierarchyDataForRatingGroup(topRatingGroupID);
            bool multipleLevels = theData.RatingHierarchyEntries.Any(x => x.HierarchyLevel == 2);
            if (multipleLevels)
                builder.Append("<div class=\"treenew tree no_dots\">");
            else
                builder.Append("<div class=\"multiplePossibilities\">");
            GetListForHierarchyLevel(builder, tblColumnID, theData, 1, null, trusted);
            //if (multipleLevels)
            builder.Append("</div>");
        }

        private static string GetStringForRatingInput(decimal? value, int? decPlaces, int TblColumnID, int ratingID, bool? trusted, string theValuePreformatted, string suppClass = "")
        {
            string theValue;
            if (theValuePreformatted == null || theValuePreformatted == "")
            {
                theValue = PMNumberandTableFormatter.FormatAsSpecified(value, (int)decPlaces, TblColumnID);
                if (!((bool)trusted) && value != null)
                    theValue = theValue + "*";
            }
            else
                theValue = theValuePreformatted;
            return "<input class=\"rtg " + suppClass + "\" name=\"mkt" + ratingID + "\" value=\"" + theValue + "\" readonly=\"true\">";
        }

        private static void GetListForHierarchyLevel(StringBuilder theStringBuilder, int TblColumnID, RatingHierarchyData theData, int hierarchyLevel, int? superior, bool trusted)
        {
            theStringBuilder.Append("<ul>");
            var theItems = theData.RatingHierarchyEntries.Where(x => x.HierarchyLevel == hierarchyLevel && (superior == null || x.Superior == superior));
            bool open = false;
            if (hierarchyLevel == 1)
            { // we keep top of hierarchy open if there aren't too many items in next level
                var itemsLevel2Count = theData.RatingHierarchyEntries.Where(x => x.HierarchyLevel == 2).Count();
                if (theItems.Count() + itemsLevel2Count <= 8)
                    open = true;
            }
            foreach (var item in theItems)
            {
                if (open)
                    theStringBuilder.Append("<li class=\"open\" >");
                else
                    theStringBuilder.Append("<li>");
                //theStringBuilder.Append(String.Format("<li id=\"{0}\">","li" + item.ratingID.ToString())); 
                theStringBuilder.Append("<a href=\"#\" class=\"tableCellTreeAnchor\" tabindex=\"-1\">"); // add <ins>&nbsp;</ins> afterward to get icon
                theStringBuilder.Append(item.RatingName);
                theStringBuilder.Append("</a>");

                theStringBuilder.Append(GetStringForRatingInput(item.Value, item.DecimalPlaces, TblColumnID, (int)item.RatingID, trusted, "", "rtgFixed"));
                if (theData.RatingHierarchyEntries.Any(x => x.Superior == item.EntryNum))
                    GetListForHierarchyLevel(theStringBuilder, TblColumnID, theData, hierarchyLevel + 1, item.EntryNum, trusted);
                theStringBuilder.Append("</li>");
            }
            theStringBuilder.Append("</ul>");
        }

        //private static string LoadBodyOfRowFromWebServiceForComplexTbl(UserRatingDataAccess theDataAccess, int theTblTabID, string suppStyle, int theTblRowID, int theTblID)
        //{ 
        //    // We could improve performance by taking approach similar to simple Tbl approach.

        //    // Create control from scratch.
        //    LoadBodyRowInfo theInfo = new LoadBodyRowInfo();
        //    theInfo.dataAccess = theDataAccess;
        //    theInfo.theTblID = theTblID;
        //    theInfo.theTblRowID = theTblRowID;
        //    theInfo.TblTabID = theTblTabID;
        //    theInfo.suppStyle = suppStyle;
        //    string theBodyOfRow = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/Main/Table/BodyRow.ascx", "theBodyRowInfo", theInfo);
        //    return theBodyOfRow;
        //}

        private static string GetBodyRowHeading(RaterooDataAccess theDataAccess, TblDimension theTblDimension, int theTblRowID, int theTblID, int thePointsManagerID)
        {
            string theRowHeading;
            string myCacheKeyRowHeading = "RowHeading" + theTblRowID.ToString();
            theRowHeading = PMCacheManagement.GetItemFromCache(myCacheKeyRowHeading) as string;
            if (theRowHeading == null)
            {
                // Create control from scratch.
                LoadRowHeadingInfo theInfo = new LoadRowHeadingInfo();
                theInfo.dataAccess = theDataAccess;
                theInfo.theTblID = theTblID;
                theInfo.thePointsManagerID = thePointsManagerID;
                theInfo.theTblRowID = theTblRowID;
                theInfo.theTblDimension = theTblDimension;
                theRowHeading = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/Main/Table/ViewCellRowHeading.ascx", "theRowHeadingInfo", theInfo);
                string[] myDependencies = {
                    "FieldForTblRowID" + theTblRowID.ToString(),
                    "FieldInfoForPointsManagerID" + thePointsManagerID.ToString()
                          };
                PMCacheManagement.AddItemToCache(myCacheKeyRowHeading, myDependencies, theRowHeading, new TimeSpan(7, 0, 0, 0));
            }
            return theRowHeading;
        }

        private static string LoadHeaderRowFromWebService(RaterooDataAccess theDataAccess, int TblTabID, int? TblColumnToSortID, bool sortByEntityName, bool ascending, int theTblID)
        {
            string theHeaderRow;
            string myCacheKeyHeaderRow = "HeaderRow" + TblTabID + TblColumnToSortID + ascending;
            theHeaderRow = PMCacheManagement.GetItemFromCache(myCacheKeyHeaderRow) as string;
            if (theHeaderRow == null)
            {
                LoadHeaderRowInfo theInfo = new LoadHeaderRowInfo();
                theInfo.dataAccess = theDataAccess;
                theInfo.TblTabID = TblTabID;
                theInfo.TblColumnToSortID = TblColumnToSortID;
                theInfo.SortByEntityName = sortByEntityName;
                theInfo.ascending = ascending;
                theHeaderRow = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/Main/Table/HeaderRow.ascx", "theHeaderRowInfo", theInfo);
                string[] myDependencies = {
                    "CategoriesForTblID" + theTblID.ToString()
                          };
                PMCacheManagement.AddItemToCache(myCacheKeyHeaderRow, myDependencies, theHeaderRow);
            }
            return theHeaderRow;
        }

    }
}