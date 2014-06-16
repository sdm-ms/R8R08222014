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
    /// Summary description for TableLoading
    /// </summary>
    public static class TableLoading
    {
        public static TablePopulateResponse PopulateTableInitially(string theTableInfoString, int visibleRows, int additionalRows, int rowsToSkip, bool includeHeaderRow)
        {
            return PopulateTable(theTableInfoString, rowsToSkip + 1, visibleRows + additionalRows, true, includeHeaderRow, null);
        }

        public static TablePopulateResponse PopulateTableSpecificRows(string theTableInfoString, int firstRowNum, int numRowsNeeded, int visibleRowsEstimate, int firstRowIfResetting)
        {
            TablePopulateResponse theResponse = PopulateTable(theTableInfoString, firstRowNum, Math.Max(numRowsNeeded, visibleRowsEstimate), false, false, firstRowIfResetting);
            return theResponse;
        }

        public static TablePopulateResponse PopulateTable(string theTableInfoString, int firstRowNum, int numRows, bool populatingInitially, bool includeHeaderRow, int? firstRowIfResetting)
        {
            try
            {
                //ProfileSimple.Start("PopulateTable"); // QUERYTIMING
                string cacheString = MD5HashGenerator.GenerateKey(theTableInfoString + firstRowNum.ToString() + numRows.ToString() + includeHeaderRow.ToString());
                string[] myDependencies = { };
                if (populatingInitially)
                {
                    TablePopulateResponse cachedResponse = CacheManagement.GetItemFromCache(cacheString + "FullResponse") as TablePopulateResponse;
                    if (cachedResponse != null)
                        return cachedResponse;
                }

                R8RDataAccess theDataAccess = new R8RDataAccess();
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
                    theHeaderRow = LoadHeaderRowFromWebService(theDataAccess, theTableInfo.TblTabID, (theTableSortRule is TableSortRuleTblColumn ? (int?)((TableSortRuleTblColumn)theTableSortRule).TblColumnToSortID : null), theTableSortRule is TableSortRuleRowName, theTableSortRule.Ascending, theTbl.TblID);

                //ProfileSimple.Start("GetTablePopulateResponse"); // QUERYTIMING
                TablePopulateResponse theResponse = GetTablePopulateResponse(firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, thePointsManager, theTbl, theHeaderRow, rowCountOverride);
                //ProfileSimple.End("GetTablePopulateResponse"); // QUERYTIMING

                if (firstRowNum == 1)
                    CacheManagement.AddItemToCache(cacheString + "FullResponse", myDependencies, theResponse, new TimeSpan(0, 0, 15));
                //ProfileSimple.End("PopulateTable"); // QUERYTIMING
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


        public static TblColumn GetTblColumn(R8RDataAccess dataAccess, int TblColumnID, int domainID)
        {
            return dataAccess.GetTblColumn(TblColumnID);
        }

        public static void GetTblAndPointsManagerForTblTab(R8RDataAccess dataAccess, int TblTabID, out TblTab theTblTab, out Tbl theTbl, out PointsManager thePointsManager)
        {
            theTblTab = dataAccess.GetTblTab(TblTabID);
            theTbl = CacheManagement.GetItemFromCache("TblForTblTab" + TblTabID) as Tbl;
            bool addTblToCache = false;
            if (theTbl == null)
            {
                theTbl = theTblTab.Tbl;
                if (theTbl == null)
                    throw new Exception("Sorry, an internal error loading information on this table occurred.");
                addTblToCache = true;
            }
            thePointsManager = CacheManagement.GetItemFromCache("PointsManagerForTbl" + theTbl.TblID) as PointsManager;
            if (thePointsManager == null)
            {
                thePointsManager = theTbl.PointsManager;
                CacheManagement.AddItemToCache("PointsManagerForTbl" + theTbl.TblID, new string[] { "DomainID" + thePointsManager.DomainID, "TblID" + theTbl.TblID }, theTbl);
            }
            if (addTblToCache)
                CacheManagement.AddItemToCache("TblForTblTab" + TblTabID, new string[] { "DomainID" + thePointsManager.DomainID, "TblID" + theTbl.TblID }, theTbl);
        }



        private static TablePopulateResponse GetTablePopulateResponse(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, R8RDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCountOverride)
        {
            TablePopulateResponse theResponse = null;
            try
            {
                if (theTbl.FastTableSyncStatus != (int)FastAccessTableStatus.fastAccessNotCreated && FastAccessTablesQuery.FastAccessTablesEnabled() && !theTableInfo.Filters.theFilterRules.Any(x => x is SearchWordsFilterRule)) // we don't do fast queries if it is disabled, or (for now) if we have a search words query
                {
                    //ProfileSimple.Start("GetTablePopulateResponseWithFastQueries"); // QUERYTIMING
                    DenormalizedTableAccess dta = new DenormalizedTableAccess(1);
                    theResponse = GetTablePopulateResponseWithFastQueries(dta, firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, thePointsManager, theTbl, headerRow, rowCountOverride);
                    //ProfileSimple.End("GetTablePopulateResponseWithFastQueries"); // QUERYTIMING
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

        private static TablePopulateResponse GetTablePopulateResponseWithFastQueries(DenormalizedTableAccess dta, int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, R8RDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCountOverride)
        {
            //ProfileSimple.Start("FastQueries intro"); // QUERYTIMING
            List<InfoForBodyRows> theInfoForBodyRows;
            int? rowCountFromQuery = null;
            FastAccessTablesQuery.DoQuery(dta, firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess.R8RDB, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule, theTbl, out theInfoForBodyRows, out rowCountFromQuery);

            int? rowCount = rowCountOverride ?? rowCountFromQuery;

            //ProfileSimple.End("FastQueries intro"); // QUERYTIMING
            TablePopulateResponse theResponse = CompleteGetTablePopulateResponse(theDataAccess, theTableInfo, tableInfoForReset, thePointsManager, theTbl, headerRow, rowCount, firstRowNum, null, theInfoForBodyRows);
            return theResponse;
        }

        private static TablePopulateResponse GetTablePopulateResponseWithNormalizedQueries(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, R8RDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCountOverride)
        {
            //ProfileSimple.Start("NormalizedQuery");
            TblRowsToPopulatePage theInfoToPopulatePage = GetTblRowsToPopulatePage(firstRowNum, numRows, populatingInitially, cacheString, myDependencies, theDataAccess, theTableInfo, tableInfoForReset, maxNumResults, theTableSortRule);
            int? rowCount = rowCountOverride ?? theInfoToPopulatePage.rowCount;

            TablePopulateResponse theResponse = CompleteGetTablePopulateResponse(theDataAccess, theTableInfo, tableInfoForReset, thePointsManager, theTbl, headerRow, rowCount, theInfoToPopulatePage.numRowOfFirstTblRow, theInfoToPopulatePage);
            //ProfileSimple.End("NormalizedQuery");
            return theResponse;
        }

        private static TablePopulateResponse CompleteGetTablePopulateResponse(R8RDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, PointsManager thePointsManager, Tbl theTbl, string headerRow, int? rowCount, int numRowOfFirstTblRow, TblRowsToPopulatePage theInfoToPopulatePage = null, List<InfoForBodyRows> bodyRowInfoList = null)
        {
            //ProfileSimple.Start("CompleteTablePopulateResponse"); // QUERYTIMING
            if (theInfoToPopulatePage == null && bodyRowInfoList == null)
                throw new Exception("Internal error: one of theInfoToPopulatePage and bodyRowInfoList should be null, not both.");

            //ProfileSimple.Start("GetTableDimension"); // QUERYTIMING
            TblDimension theTblDimension = GetTblDimension(theDataAccess, theTbl);
            //ProfileSimple.End("GetTableDimension"); // QUERYTIMING
            //ProfileSimple.Start("GetMainRowsString"); // QUERYTIMING
            string mainRowsString = GetMainRowsString(theDataAccess, theTableInfo, thePointsManager, theTbl, theTblDimension, numRowOfFirstTblRow, theInfoToPopulatePage, bodyRowInfoList);
            //ProfileSimple.End("GetMainRowsString"); // QUERYTIMING

            TablePopulateResponse theResponse = new TablePopulateResponse
            {
                mainRows = mainRowsString,
                headerRow = headerRow,
                success = true,
                err = "",
                rowCount = rowCount,
                tableInfoForReset = tableInfoForReset
            };

            //ProfileSimple.End("CompleteTablePopulateResponse"); // QUERYTIMING
            return theResponse;
        }

        private static TblDimension GetTblDimension(R8RDataAccess theDataAccess, Tbl theTbl)
        {
            TblDimension theTblDimension = null;
            TblDimensionAccess theCssAccess = new TblDimensionAccess(theDataAccess);
            theTblDimension = theCssAccess.GetTblDimensionsForRegularTbl(theTbl.TblID);
            return theTblDimension;
        }

        private static string GetMainRowsString(R8RDataAccess theDataAccess, TableInfo theTableInfo, PointsManager thePointsManager, Tbl theTbl, TblDimension theTblDimension, int numRowOfFirstTblRow, TblRowsToPopulatePage theInfoToPopulatePage = null, List<InfoForBodyRows> bodyRowInfoList = null)
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

        private static TblRowsToPopulatePage GetTblRowsToPopulatePage(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, R8RDataAccess theDataAccess, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule)
        {
            TblRowsToPopulatePage theInfoToPopulatePage = null;

            if (populatingInitially)
                theInfoToPopulatePage = (TblRowsToPopulatePage)CacheManagement.GetItemFromCache(cacheString + "TblRowsOnly");

            if (theInfoToPopulatePage == null)
            {
                bool sortByNameAfterTakingTop = false;
                bool nameAscending = true;
                if (populatingInitially)
                {
                    theInfoToPopulatePage = theTableInfo.Filters.GetQueryToPopulatePageInitially(theDataAccess.R8RDB, maxNumResults,
                theTableSortRule, sortByNameAfterTakingTop, nameAscending, numRows, firstRowNum - 1);
                }
                else
                {
                    theInfoToPopulatePage = theTableInfo.Filters.GetQueryForSpecificRows(theDataAccess.R8RDB, maxNumResults, theTableSortRule, sortByNameAfterTakingTop, nameAscending, (int)firstRowNum, (int)numRows, tableInfoForReset != "");
                }
                if (firstRowNum == 1)
                    CacheManagement.AddItemToCache(cacheString + "TblRowsOnly", myDependencies, theInfoToPopulatePage, new TimeSpan(0, 1, 0));
            }
            return theInfoToPopulatePage;
        }

        private static void TableLoadingCalculateMaxNumResults(int firstRowNum, int numRows, R8RDataAccess theDataAccess, TableInfo theTableInfo, out int? maxNumResults, out int? rowCountOverride, out TableSortRule theTableSortRule)
        {
            maxNumResults = null;
            rowCountOverride = null;
            theTableSortRule = TableSortRuleGenerator.GetTableSortRuleFromStringRepresentation(theTableInfo.SortInstruction);
            if (theTableSortRule is TableSortRuleRowName)
                maxNumResults = null;
            else if (theTableSortRule is TableSortRuleDistance)
            {
                if (theTableInfo.Filters.theFilterRules.Count() == 0)
                { // we want to limit the maxNumResults to the highest row number we're seeking, but we need to remember how many results there really are, so user can scroll
                    if (firstRowNum > 1)
                        maxNumResults = firstRowNum + (int)numRows - 1;
                    else
                        maxNumResults = numRows;
                    rowCountOverride = theDataAccess.R8RDB.GetTable<TblRow>().Where(t => t.InitialFieldsDisplaySet && t.TblID == theTableInfo.TblID).Count();
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

        private static void CheckTableLoadingResetNeeded(string theTableInfoString, ref int firstRowNum, ref int numRows, int? firstRowIfResetting, R8RDataAccess theDataAccess, TableInfo theTableInfo, ref string tableInfoForReset, bool populatingInitially)
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
            var theSortMenu = SortMenuGenerator.GetSortMenuForTblTab(theDataAccess.R8RDB, theTableInfo.TblTabID, trustedUser);
            theTableInfo.SortMenu = theSortMenu;
            string potentialTableInfoForReset = TableInfoToStringConversion.GetStringFromTableInfo(theTableInfo);
            if (potentialTableInfoForReset != theTableInfoString)
                tableInfoForReset = potentialTableInfoForReset;
        }



        public static string GetBodyRowString(R8RDataAccess theDataAccess, int rowNumber, int theTblRowID, bool tblRowDeleted, int theTblID, PointsManager thePointsManager, int theTblTabID, string suppStyle, TblDimension theTblDimension, List<InfoForBodyRows> bodyRowInfoList)
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
            //theBodyOfRow = (string)CacheManagement.GetItemFromCache(myCacheKeyBodyRow);
            //if (theBodyOfRow == null)
            //{
            //if (simpleTblQuery != null)
            theBodyOfRow = GetUncachedColumnsOfBodyRowString(bodyRowInfoQuery, suppStyle, theTblRowID);
            //else
            //    theBodyOfRow = LoadBodyOfRowFromWebServiceForComplexTbl(theDataAccess, theTblTabID, suppStyle, theTblRowID, theTbl.TblID);
            //    string[] myDependencies = {
            //                            "RatingsForTblRowIDAndTblTabID" + theTblRowID.ToString() + "," + theTblTabID.ToString(),
            //                            "ColumnsForTblID" + theTblID.ToString()
            //                                              };
            //    CacheManagement.AddItemToCache(myCacheKeyBodyRow, myDependencies, theBodyOfRow);
            //}
            return theBodyOfRow;
        }

        private static List<InfoForBodyRows> GetBodyRowsInfoListFromDatabase(R8RDataAccess theDataAccess, int theTblTabID, List<int> theTblRowIDs)
        {
            return theDataAccess.R8RDB.GetTable<RatingGroup>()
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
                BuildHtmlForCell(build, cell.TblRowID, cell.TopRatingGroupID, cell.ValueOfFirstRating, cell.DecPlaces, cell.TblColumnID, cell.FirstRatingID, cell.PreformattedString, cell.Trusted, cell.SingleNumberOnly, true);
            }

            return build.ToString();

        }

        public static string GetHtmlStringForCell(int topRatingGroupID, decimal? valueOfFirstRating, int decPlaces, int tblRowID, int tblColumnID, int? ratingIDIfSingleNumber, string preformattedContentsIfAvailable, bool trusted, bool singleNumberOnly, bool includeOuterTD)
        {
            StringBuilder builder = new StringBuilder();
            BuildHtmlForCell(builder, tblRowID, topRatingGroupID, valueOfFirstRating, decPlaces, tblColumnID, ratingIDIfSingleNumber, preformattedContentsIfAvailable, trusted, singleNumberOnly, includeOuterTD);
            return builder.ToString();
        }

        private static void BuildHtmlForCell(StringBuilder builder, int tblRowID, int? topRatingGroupID, decimal? valueOfFirstRating, int? decPlaces, int tblColumnID, int? ratingIDIfSingleNumber, string preformattedContentsIfAvailable, bool? trusted, bool singleNumberOnly, bool includeOuterTD)
        {
            string topRatingIDString = topRatingGroupID == null ? tblRowID.ToString() + "/" + tblColumnID.ToString() : topRatingGroupID.ToString();
            string nameAndTopRatingGroupID = "mg" + topRatingIDString;
            if (includeOuterTD)
                builder.Append("<td class=\"mainCellMarker \" data-rg=\"" + topRatingIDString + " \">");
            // builder.Append("<input name=\"" + nameAndTopRatingGroupID + "\" type=\"hidden\" id=\"" + nameAndTopRatingGroupID + "\" class=\"mgID\" value=\"" + topRatingIDString + "\" />");
            if (singleNumberOnly)
            {
                builder.Append(GetStringForRatingInput(valueOfFirstRating, decPlaces, tblRowID, tblColumnID, ratingIDIfSingleNumber, preformattedContentsIfAvailable));
            }
            else
            {
                if (preformattedContentsIfAvailable != null && preformattedContentsIfAvailable != "")
                    builder.Append(preformattedContentsIfAvailable);
                else
                {
                    if (topRatingGroupID == null)
                        throw new NotImplementedException("Must deal with this when adding support for multiple ratings in a cell.");
                    BuildHtmlForComplexCellFromDatabase(builder, (int)topRatingGroupID, tblRowID, tblColumnID, (bool)trusted);
                }
            }
            if (includeOuterTD)
                builder.Append("</td>");
        }

        private static void BuildHtmlForComplexCellFromDatabase(StringBuilder builder, int topRatingGroupID, int tblRowID, int tblColumnID, bool trusted)
        {
            R8RDataAccess DataAccess = new R8RDataAccess();
            RatingHierarchyData theData = DataAccess.GetRatingHierarchyDataForRatingGroup(topRatingGroupID);
            bool multipleLevels = theData.RatingHierarchyEntries.Any(x => x.HierarchyLevel == 2);
            if (multipleLevels)
                builder.Append("<div class=\"treenew tree no_dots\">");
            else
                builder.Append("<div class=\"multiplePossibilities\">");
            GetListForHierarchyLevel(builder, tblRowID, tblColumnID, theData, 1, null, trusted);
            //if (multipleLevels)
            builder.Append("</div>");
        }

        private static string GetStringForRatingInput(decimal? value, int? decPlaces, int TblRowID, int TblColumnID, int? ratingID, string theValuePreformatted, string suppClass = "")
        {
            string ratingIDString = ratingID == null ? TblRowID.ToString() + "/" + TblColumnID.ToString() : ratingID.ToString();
            string theValue;
            if (theValuePreformatted == null || theValuePreformatted == "")
            {
                theValue = NumberandTableFormatter.FormatAsSpecified(value, (int)decPlaces, TblColumnID);
            }
            else
                theValue = theValuePreformatted;
            return "<input class=\"rtg " + suppClass + "\" name=\"mkt" + ratingIDString + "\" value=\"" + theValue + "\" readonly=\"true\">";
        }

        private static void GetListForHierarchyLevel(StringBuilder theStringBuilder, int TblRowID, int TblColumnID, RatingHierarchyData theData, int hierarchyLevel, int? superior, bool trusted)
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

                theStringBuilder.Append(GetStringForRatingInput(item.Value, item.DecimalPlaces, TblRowID, TblColumnID, (int)item.RatingID, "", "rtgFixed"));
                if (theData.RatingHierarchyEntries.Any(x => x.Superior == item.EntryNum))
                    GetListForHierarchyLevel(theStringBuilder, TblRowID, TblColumnID, theData, hierarchyLevel + 1, item.EntryNum, trusted);
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

        private static string GetBodyRowHeading(R8RDataAccess theDataAccess, TblDimension theTblDimension, int theTblRowID, int theTblID, int thePointsManagerID)
        {
            string theRowHeading;
            string myCacheKeyRowHeading = "RowHeading" + theTblRowID.ToString();
            theRowHeading = CacheManagement.GetItemFromCache(myCacheKeyRowHeading) as string;
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
                CacheManagement.AddItemToCache(myCacheKeyRowHeading, myDependencies, theRowHeading, new TimeSpan(7, 0, 0, 0));
            }
            return theRowHeading;
        }

        private static string LoadHeaderRowFromWebService(R8RDataAccess theDataAccess, int TblTabID, int? TblColumnToSortID, bool sortByTblRowName, bool ascending, int theTblID)
        {
            string theHeaderRow;
            string myCacheKeyHeaderRow = "HeaderRow" + TblTabID + TblColumnToSortID + ascending;
            theHeaderRow = CacheManagement.GetItemFromCache(myCacheKeyHeaderRow) as string;
            if (theHeaderRow == null)
            {
                LoadHeaderRowInfo theInfo = new LoadHeaderRowInfo();
                theInfo.dataAccess = theDataAccess;
                theInfo.TblTabID = TblTabID;
                theInfo.TblColumnToSortID = TblColumnToSortID;
                theInfo.SortByTblRowName = sortByTblRowName;
                theInfo.ascending = ascending;
                theHeaderRow = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/Main/Table/HeaderRow.ascx", "theHeaderRowInfo", theInfo);
                string[] myDependencies = {
                    "ColumnsForTblID" + theTblID.ToString()
                          };
                CacheManagement.AddItemToCache(myCacheKeyHeaderRow, myDependencies, theHeaderRow);
            }
            return theHeaderRow;
        }

    }
}