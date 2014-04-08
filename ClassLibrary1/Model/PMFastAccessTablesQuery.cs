﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlTypes;
using System.Data.Linq;
using Microsoft.SqlServer.Types;
using GoogleGeocoder;
using System.Collections;
using Microsoft.WindowsAzure.ServiceRuntime;


namespace ClassLibrary1.Model
{
    public static class PMFastAccessTablesQuery
    {

        internal class DoQueryResults
        {
            public List<InfoForBodyRows> bodyRowList;
            public int? rowCount;

        }

        internal static List<string> GetListOfFastAccessVariablesToLoad(List<TblColumn> theTblColumns, bool includeRowHeader)
        {
            List<string> allColumns = new List<string>();
            allColumns.Add("ID");
            allColumns.Add("NME");
            allColumns.Add("DEL");
            allColumns.Add("CNNE");
            allColumns.Add("CUP");
            allColumns.Add("ELEV");
            allColumns.Add("HS");
            allColumns.Add("RC");
            if (includeRowHeader)
            {
                allColumns.Add("RH");
            }
            foreach (var col in theTblColumns)
            {
                string colString = col.TblColumnID.ToString();
                allColumns.Add("RS" + colString);
                //allColumns.Add("RV" + colString);
                allColumns.Add("R" + colString);
                allColumns.Add("RG" + colString);
                allColumns.Add("RC" + colString);
            }
            return allColumns;
        }

        internal static void GetOrderBy(IRaterooDataContext iDataContext, TableSortRule tableSortRule, int tblID, int originalTblIndex, ref int sqlTblIndex, DateTime asOfDateTime, ref int paramNumber, List<SqlParameter> parameters, out string orderByString, out string joinString)
        {
            joinString = "";
            orderByString = "";
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            joinString = " ";
            string ascOrDescString = (tableSortRule.Ascending) ? "ASC" : "DESC";
            if (tableSortRule is TableSortRuleEntityName)
                orderByString = "ORDER BY NME " + ascOrDescString;
            else if (tableSortRule is TableSortRuleNewestInDatabase)
                orderByString = "ORDER BY ID " + ascOrDescString;
            else if (tableSortRule is TableSortRuleNeedsRating)
                orderByString = String.Format("ORDER BY [t{0}].[DEL], [t{0}].[ELEV] DESC, [t{0}].[CNNE] DESC, [t{0}].[CUP]", sqlTblIndex);
            else if (tableSortRule is TableSortRuleNeedsRatingUntrustedUser)
                orderByString = String.Format("ORDER BY [t{0}].[DEL], [t{0}].[CNNE] DESC, [t{0}].[CUP]", sqlTblIndex);
            else if (tableSortRule is TableSortRuleTblColumn)
            {
                TableSortRuleTblColumn tableSortRuleTblColumn = (TableSortRuleTblColumn)tableSortRule;
                if (!StatusRecords.RatingGroupStatusRecordsExistSince(iDataContext, asOfDateTime))
                { // no need to bother correcting for changes in sorting
                    orderByString = String.Format("ORDER BY [t{0}].[RV{1}] {2}", sqlTblIndex, tableSortRuleTblColumn.TblColumnToSortID, ascOrDescString);
                }
                else
                { // must correct for changes in sorting, so that we recover the order as of the time specified. that way, when scrolling, we don't get repeats or skips.
                    sqlTblIndex++;
                    int subtableIndex1 = sqlTblIndex;
                    sqlTblIndex++;
                    int subtableIndex2 = sqlTblIndex;
                    sqlTblIndex++;
                    int subtableIndex3 = sqlTblIndex;
                    int asOfDateTimeParamNum = paramNumber;
                    parameters.Add(new SqlParameter("P" + asOfDateTimeParamNum.ToString(), asOfDateTime));
                    paramNumber++;
                    orderByString = String.Format(@"ORDER BY 
        (CASE 
            WHEN NOT ([t{0}].[RC{4}] = 1) THEN [t{0}].[RV{4}]
            WHEN EXISTS(
                SELECT NULL AS [EMPTY]
                FROM [RatingGroupStatusRecords] AS [t{1}]
                WHERE (([t{1}].[RatingGroupID]) = [t{0}].[RG{4}]) AND ([t{1}].[NewValueTime] > @p{6})
                ) THEN (
                SELECT [t{3}].[OldValueOfFirstRating]
                FROM (
                    SELECT TOP (1) [t{2}].[OldValueOfFirstRating]
                    FROM [RatingGroupStatusRecords] AS [t{2}]
                    WHERE (([t{2}].[RatingGroupID]) = [t{0}].[RG{4}]) AND ([t{2}].[NewValueTime] > @p{6})
                    ) AS [t{3}]
                )
            ELSE [t{0}].[RV{4}]
         END) {5}, [t{0}].[NME]", originalTblIndex, subtableIndex1, subtableIndex2, subtableIndex3, tableSortRuleTblColumn.TblColumnToSortID, ascOrDescString, asOfDateTimeParamNum);
                };
            }
            else if (tableSortRule is TableSortRuleActivityLevel)
            {
                TableSortRuleActivityLevel activityLevelSortRule = (TableSortRuleActivityLevel)tableSortRule;
                sqlTblIndex++;
                int volatilityIndex = sqlTblIndex;
                orderByString = String.Format(" ORDER BY (SELECT [t{1}].[Volatility] FROM [dbo].[VolatilityTblRowTrackers] AS [t{1}] WHERE ([t{1}].[DurationType] = {2}) AND ([t{1}].[TblRowID] = [t{0}].[ID])) {3} ", originalTblIndex, volatilityIndex, (int)activityLevelSortRule.TimeFrame, ascOrDescString);
            }
            else if (tableSortRule is TableSortRuleDistance)
            {
                TableSortRuleDistance distanceSortRule = (TableSortRuleDistance)tableSortRule;
                sqlTblIndex++;
                int nearestNeighborsIndex = sqlTblIndex;
                // DEBUG: We should not need to use UDFNearestNeighbors here. Search for nearest neighbors SQL Server 2012 for some simpler code examples. We do need it in the normalized database, since Linq to SQL otherwise won't produce appropriate queries.
                joinString = String.Format(" INNER JOIN [dbo].[UDFNearestNeighborsFor{0}]({1}, {2}, {3}) AS [t{5}] ON ([t{4}].[ID]) = [t{5}].[TblRowID] ", "F" + distanceSortRule.FieldDefinitionID.ToString(), distanceSortRule.Latitude, distanceSortRule.Longitude, 1000, originalTblIndex, nearestNeighborsIndex);
                orderByString = String.Format(" ORDER BY [t{0}].TblRowID {1}", nearestNeighborsIndex, ascOrDescString);
            }
            else
                throw new NotImplementedException();
        }

        internal class ChoiceFullFieldDefinition
        {
            public FieldDefinition fd { get; set; }
            public ChoiceGroupFieldDefinition cgfd { get; set; }
            public ChoiceGroup cg { get; set; }
        }

        internal static List<ChoiceFullFieldDefinition> GetChoiceFullFieldDefinitions(IRaterooDataContext iDataContext, Tbl tbl)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "ChoiceFullFieldDefinition" + tbl.TblID.ToString();
            List<ChoiceFullFieldDefinition> fd = PMCacheManagement.GetItemFromCache(cacheString) as List<ChoiceFullFieldDefinition>;
            if (fd == null)
            {
                fd = iDataContext.GetTable<ChoiceGroupFieldDefinition>().Where(x => x.FieldDefinition.TblID == tbl.TblID && x.Status == (int)StatusOfObject.Active).Select(x => new ChoiceFullFieldDefinition { fd = x.FieldDefinition, cgfd = x, cg = x.ChoiceGroup }).ToList();
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }

        internal static List<FieldDefinition> GetAddressFieldDefinitions(IRaterooDataContext iDataContext, Tbl tbl)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "AddressFieldDefinition" + tbl.TblID.ToString();
            List<FieldDefinition> fd = PMCacheManagement.GetItemFromCache(cacheString) as List<FieldDefinition>;
            if (fd == null)
            {
                fd = iDataContext.GetTable<FieldDefinition>().Where(x => x.TblID == tbl.TblID && x.FieldType == (int)FieldTypes.AddressField && x.Status == (int)StatusOfObject.Active).ToList();
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }

        internal static FieldDefinition GetFieldDefinition(IRaterooDataContext iDataContext, int fieldDefinitionID)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "FieldDefinition" + fieldDefinitionID.ToString();
            FieldDefinition fd = PMCacheManagement.GetItemFromCache(cacheString) as FieldDefinition;
            if (fd == null)
            {
                fd = iDataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == fieldDefinitionID);
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + fd.Tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }


        internal static ChoiceGroupFieldDefinition GetChoiceGroupFieldDefinition(IRaterooDataContext iDataContext, int fieldDefinitionID)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "FieldDefinition" + fieldDefinitionID.ToString();
            ChoiceGroupFieldDefinition fd = PMCacheManagement.GetItemFromCache(cacheString) as ChoiceGroupFieldDefinition;
            if (fd == null)
            {
                fd = iDataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == fieldDefinitionID);
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + fd.FieldDefinition.Tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }

        internal static void GetStatementsForFiltering(FilterRules filters, List<ChoiceFullFieldDefinition> choiceFDs, int originalSqlTableIndex, ref int sqlTableIndex, DateTime? asOfDateTime, int tblID, ref int paramNumber, ref List<SqlParameter> parameters, out string whereString, out string joinString)
        {
            joinString = " ";
            List<string> whereClauses = new List<string>();
            foreach (var filter in filters.theFilterRules.Where(x => x.FilterOn))
            {
                if (filter is DateTimeFilterRule)
                {
                    DateTimeFilterRule dateTimeFilter = ((DateTimeFilterRule)filter);
                    if (dateTimeFilter.MinValue != null)
                    {
                        whereClauses.Add("F" + dateTimeFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (DateTime)dateTimeFilter.MinValue));
                        paramNumber++;
                    }

                    if (dateTimeFilter.MaxValue != null)
                    {
                        whereClauses.Add("F" + dateTimeFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (DateTime)dateTimeFilter.MaxValue));
                        paramNumber++;
                    }
                }
                else if (filter is NumberFilterRule)
                {
                    NumberFilterRule NumberFilter = ((NumberFilterRule)filter);
                    if (NumberFilter.MinValue != null)
                    {
                        whereClauses.Add("F" + NumberFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)NumberFilter.MinValue));
                        paramNumber++;
                    }

                    if (NumberFilter.MaxValue != null)
                    {
                        whereClauses.Add("F" + NumberFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)NumberFilter.MaxValue));
                        paramNumber++;
                    }
                }

                else if (filter is TextFilterRule)
                {
                    TextFilterRule TextFilter = ((TextFilterRule)filter);
                    if (TextFilter.FromText != null && TextFilter.FromText != "")
                    {
                        whereClauses.Add("F" + TextFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), TextFilter.FromText));
                        paramNumber++;
                    }

                    if (TextFilter.ToText != null)
                    {
                        whereClauses.Add("F" + TextFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), TextFilter.ToText));
                        paramNumber++;
                    }
                }
                else if (filter is ChoiceFilterRule)
                {
                    ChoiceFilterRule ChoiceFilter = (ChoiceFilterRule)filter;
                    ChoiceFullFieldDefinition theFD = choiceFDs.Single(x => x.fd.FieldDefinitionID == ChoiceFilter.theID);
                    if (theFD.cg.AllowMultipleSelections)
                    {
                        string thisTbl = "T" + theFD.fd.TblID.ToString();
                        string mcTbl = "VFMC" + theFD.fd.FieldDefinitionID.ToString();
                        joinString += " INNER JOIN " + mcTbl + " ON ID=" + mcTbl + ".TRID " + " AND " + mcTbl + ".CHO=" + "@P" + paramNumber.ToString() + " ";
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), ChoiceFilter.ChoiceInGroupID));
                        paramNumber++;
                    }
                    else
                    {
                        whereClauses.Add("F" + ChoiceFilter.theID.ToString() + " = @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), ChoiceFilter.ChoiceInGroupID));
                        paramNumber++;
                    }
                }
                else if (filter is TblColumnFilterRule)
                {
                    TblColumnFilterRule ColumnFilter = ((TblColumnFilterRule)filter);
                    if (ColumnFilter.MinValue != null)
                    {
                        whereClauses.Add("RV" + ColumnFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)ColumnFilter.MinValue));
                        paramNumber++;
                    }

                    if (ColumnFilter.MaxValue != null)
                    {
                        whereClauses.Add("RV" + ColumnFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)ColumnFilter.MaxValue));
                        paramNumber++;
                    }
                }
                else if (filter is AddressFilterRule)
                {
                    AddressFilterRule AddressFilter = (AddressFilterRule)filter;
                    Coordinate ObjCod = new Coordinate();
                    ObjCod = Geocode.GetCoordinates(AddressFilter.Address);
                    float Latitude1 = (float)Math.Round(ObjCod.Latitude, 4);
                    float Longitude1 = (float)Math.Round(ObjCod.Longitude, 4);
                    int paramNum1 = 0, paramNum2 = 0, paramNum3 = 0;
                    if (!(Latitude1 == 0 && Longitude1 == 0))
                    {
                        paramNum1 = paramNumber;
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)Longitude1));
                        paramNumber++;
                        paramNum2 = paramNumber;
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)Latitude1));
                        paramNumber++;
                        paramNum3 = paramNumber;
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)AddressFilter.Mile * (decimal)1609.344)); // distance from miles to meters
                        paramNumber++;
                        whereClauses.Add(String.Format("F{0}.STDistance(geography::STPointFromText('POINT(' + CAST(@P{1} AS VARCHAR(20)) + ' ' + CAST(@P{2} AS VARCHAR(20)) + ')', 4326)) <= @P{3}", AddressFilter.theID, paramNum1, paramNum2, paramNum3));
                    }
                    // sqlTableIndex++;
                    // joinString += String.Format(" INNER JOIN [dbo].[UDFDistanceWithin](@p{0}, @p{1}, @p{2}) AS [t{4}] ON ([t{3}].[F{5}]) = [t{4}].[FieldID] ",paramNum1,paramNum2, paramNum3, originalSqlTableIndex,sqlTableIndex,AddressFilter.theID.ToString());
                }
                else if (filter is SearchWordsFilterRule)
                {
                    throw new Exception("Internal error: Fast queries are not implemented for search words query."); // This would be difficult, as we would need to convert the logic of GetTblRowsForPhrase, or we would need to convert the IQueryable<TblRow> into a SQL query and work with that. Overall, not worth it. Note that we should never hit this code as we check for whether there are any SearchWordsFilterRules before calling for a fast query.
                }
                else
                    throw new Exception("Internal error: Unknown filter rule type in GetStatementsForFiltering.");
            }
            if (filters.HighStakesOnly)
                whereClauses.Add("HS = 1");
            whereString = " ";
            if (whereClauses.Any() || asOfDateTime != null)
            {
                //whereString += "WHERE ";
                string mainFilter = GetSeparatedList(whereClauses, " AND ");
                if (FastAccessTablesMaintenance.RecordRecentChangesInStatusRecords && asOfDateTime != null && StatusRecords.RowStatusHasChangedSince((DateTime)asOfDateTime, tblID))
                    whereString = FilterBasedOnTblRowStatusRecords(mainFilter, originalSqlTableIndex, ref sqlTableIndex, ref paramNumber, parameters, (DateTime)asOfDateTime, tblID, !filters.ActiveOnly);
                else
                {
                    if (filters.ActiveOnly)
                    {
                        if (mainFilter.Trim() == "")
                            whereString = "WHERE (DEL = 0) ";
                        else
                            whereString = "WHERE (DEL = 0) AND (" + mainFilter + ") ";
                    }
                    else
                    {
                        if (mainFilter.Trim() != "")
                            whereString = "WHERE " + mainFilter;
                    }
                }
            }
        }

        //internal static string FilterBasedOnTblRowStatusRecordsWithStoredProcedure(string originalWhereStatement, ref int sqlTableIndex, ref int paramNumber, List<SqlParameter> parameters, DateTime asOfDateTime, int tblID)
        //{
        //    int sourceTableIndex = sqlTableIndex;
        //    sqlTableIndex++;
        //    int innerJoinTableIndex = sqlTableIndex;
        //    string innerJoinString = String.Format("INNER JOIN [dbo].[WasActiveTblRowV{2}](@p{3}) AS [t{1}] ON ([t{0}].[ID]) = [t{1}].[ID2]", sourceTableIndex, innerJoinTableIndex, tblID, paramNumber);
        //    parameters.Add(new SqlParameter("P" + paramNumber.ToString(), asOfDateTime));
        //    paramNumber++;
        //    if (originalWhereStatement.Trim() == "")
        //        return innerJoinString;
        //    else
        //        return "WHERE " + originalWhereStatement + " " + innerJoinString;
        //}

        internal static string FilterBasedOnTblRowStatusRecords(string originalWhereStatement, int originalSqlTableIndex, ref int sqlTableIndex, ref int paramNumber, List<SqlParameter> parameters, DateTime asOfDateTime, int tblID, bool showDeletedRows)
        {
            string broaderString;
            if (showDeletedRows)
                broaderString = "WHERE ((NOT ([t{0}].[RC] = 1)) OR (([t{0}].[RC] = 1) AND (NOT (EXISTS( SELECT NULL AS [EMPTY] FROM [dbo].[TblRowStatusRecord] AS [t{1}] WHERE ([t{1}].[TimeChanged] > @p{5}) AND ([t{1}].[TblRowId] = [t{0}].[ID]))))) OR (NOT (((SELECT [t{3}].[Adding] FROM (SELECT TOP (1) [t{2}].[Adding] FROM [dbo].[TblRowStatusRecord] AS [t{2}] WHERE ([t{2}].[TimeChanged] > @p{5}) AND ([t{2}].[TblRowId] = [t{0}].[ID]) ORDER BY [t{2}].[TimeChanged]) AS [t{3}])) = 1)))";
            else
                broaderString = "WHERE (((NOT ([t{0}].[RC] = 1)) AND ([t{0}].[DEL] = 0)) OR (([t{0}].[RC] = 1) AND ([t{0}].[DEL] = 0) AND (NOT (EXISTS( SELECT NULL AS [EMPTY] FROM [dbo].[TblRowStatusRecord] AS [t{1}] WHERE ([t{1}].[TimeChanged] > @p{5}) AND ([t{1}].[TblRowID] = [t{0}].[ID]))))) OR (((SELECT [t{3}].[Deleting] FROM ( SELECT TOP (1) [t{2}].[Deleting] FROM [dbo].[TblRowStatusRecord] AS [t{2}] WHERE ([t{2}].[TimeChanged] > @p{5}) AND ([t{2}].[TblRowID] = [t{0}].[ID]) ORDER BY [t{2}].[TimeChanged]) AS [t{3}])) = 1))";
            if (originalWhereStatement != "")
                broaderString += " AND ({4})";
            sqlTableIndex++;
            int tblRowStatusRecordTableNumber1 = sqlTableIndex;
            sqlTableIndex++;
            int tblRowStatusRecordTableNumber2 = sqlTableIndex;
            sqlTableIndex++;
            int innerQueryTableNumber = sqlTableIndex;
            string returnVal = String.Format(broaderString, originalSqlTableIndex, tblRowStatusRecordTableNumber1, tblRowStatusRecordTableNumber2, innerQueryTableNumber, originalWhereStatement, paramNumber);
            parameters.Add(new SqlParameter("P" + paramNumber.ToString(), asOfDateTime));
            paramNumber++;
            return returnVal;
        }

        internal static DataTable GetDataTable(IRaterooDataContext iDataContext, DenormalizedTableAccess dta, List<string> variableList, TableSortRule tableSortRule, FilterRules filters, List<ChoiceFullFieldDefinition> choiceFieldDefinitions, int tblID, int skip, int take)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            // SAMPLE -- this produces query results somewhat like this and adds the rowcount (indicating the total number of results without skip & take
            //SELECT *
            //, (
            //    SELECT COUNT(NME)
            //    FROM [Rateroo].dbo.V244
            //  ) AS row_count
            //FROM
            //(
            //SELECT ID, NME, DEL, RH, RS617, RV617, R617, RG617, RS618, RV618, R618, RG618, RS619, RV619, R619, RG619, RS620, RV620, R620, RG620, RS621, RV621, R621, RG621, RS622, RV622, R622, RG622, RS623, RV623, R623, RG623 
            //, ROW_NUMBER() OVER (ORDER BY R617 ASC) rownum
            //FROM [Rateroo].dbo.V244
            //) seq
            //where seq.rownum BETWEEN 6 and 10
            string tblName = "V" + tblID.ToString();
            string selectString = "SELECT ";

            int sqlTblIndex = 0;
            int originalSqlTblIndex = sqlTblIndex;
            string stringList = GetSeparatedList(variableList, ", ", "[t0].[", "]");
            selectString += stringList + " ";
            string indexHint = " ";
            if (tableSortRule is TableSortRuleDistance)
            {
                indexHint = String.Format(" WITH(INDEX(SIndx_V{0}_F{1}) ", tblID, ((TableSortRuleDistance)tableSortRule).FieldDefinitionID);
            }
            else
            {
                AddressFilterRule theFilterRule = (AddressFilterRule)filters.theFilterRules.FirstOrDefault(x => x is AddressFilterRule);
                if (theFilterRule != null)
                    indexHint = String.Format(" WITH(INDEX(SIndx_V{0}_F{1}) ", tblID, theFilterRule.theID);
            }
            string fromInstruction = "FROM dbo." + tblName + " AS [t" + originalSqlTblIndex.ToString() + "] ";
            string orderInstruction;
            string joinInstructionForOrdering;
            int paramNumber = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            GetOrderBy(iDataContext, tableSortRule, tblID, originalSqlTblIndex, ref sqlTblIndex, filters.AsOfDateTime, ref paramNumber, parameters, out orderInstruction, out joinInstructionForOrdering);
            string whereInstruction;
            string joinInstructionForFiltering;
            GetStatementsForFiltering(filters, choiceFieldDefinitions, originalSqlTblIndex, ref sqlTblIndex, filters.AsOfDateTime, tblID, ref paramNumber, ref parameters, out whereInstruction, out joinInstructionForFiltering);

            string orderAndRowNumberLine = String.Format(", ROW_NUMBER() OVER ({0}) rownum ", orderInstruction);
            string coreQuery = selectString + orderAndRowNumberLine + fromInstruction + joinInstructionForOrdering + joinInstructionForFiltering + whereInstruction;
            string rowCountInstruction = String.Format(", ( SELECT COUNT(ID) {0} {1} {2} {3} ) AS row_count ", fromInstruction, joinInstructionForOrdering, joinInstructionForFiltering, whereInstruction);
            string skipTakeInstruction = " seq where seq.rownum BETWEEN " + (skip + 1).ToString() + " and " + (skip + take).ToString();
            string entireQuery = String.Format("SELECT * {0} FROM ( {1} ) {2}", rowCountInstruction, coreQuery, skipTakeInstruction);
            SqlDataAdapter adapter = new SqlDataAdapter(entireQuery, dta.GetConnectionString());
            foreach (var whereParam in parameters)
                adapter.SelectCommand.Parameters.Add(whereParam);
            DataSet data = new DataSet();
            adapter.Fill(data, tblName);
            return data.Tables[tblName];
        }

        private static string GetSeparatedList(List<string> variableList, string separator = ", ", string prefix = "", string suffix = "")
        {
            string stringList = "";
            bool isFirst = true;
            foreach (var variable in variableList)
            {
                if (!isFirst)
                    stringList += separator;
                isFirst = false;
                stringList += prefix + variable + suffix;
            }
            return stringList;
        }

        public static void DoQuery(DenormalizedTableAccess dta, int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, IRaterooDataContext iDataContext, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, Tbl theTbl, out List<InfoForBodyRows> bodyRowList, out int? rowCount)
        {
            bodyRowList = null;
            rowCount = null;
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            System.Diagnostics.Trace.WriteLine("DoQuery cacheString " + cacheString + " firstRow: " + firstRowNum + " numRows: " + numRows);
            DoQueryResults theResults = PMCacheManagement.GetItemFromCache(cacheString + "FastQueryResults") as DoQueryResults;

            if (theResults == null)
            {
                //ProfileSimple.Start("DoQuery"); // QUERYTIMING
                //ProfileSimple.Start("TblColumnsForTab"); // QUERYTIMING
                List<TblColumn> tblColumns = TblColumnsForTabCache.GetTblColumnsForTab(iDataContext, theTbl.TblID, theTableInfo.TblTabID);
                //ProfileSimple.End("TblColumnsForTab"); // QUERYTIMING
                //ProfileSimple.Start("GetListOfFastAccessVariablesToLoad"); // QUERYTIMING
                List<string> theVariables = GetListOfFastAccessVariablesToLoad(tblColumns, true);
                //ProfileSimple.End("GetListOfFastAccessVariablesToLoad"); // QUERYTIMING
                //ProfileSimple.Start("GetChoiceFullFieldDefinitions"); // QUERYTIMING
                List<ChoiceFullFieldDefinition> choiceFieldDefinitions = GetChoiceFullFieldDefinitions(iDataContext, theTbl);
                //ProfileSimple.End("GetChoiceFullFieldDefinitions"); // QUERYTIMING
                //ProfileSimple.Start("GetDataTable"); // QUERYTIMING
                DataTable theDataTable = GetDataTable(iDataContext, dta, theVariables, theTableSortRule, theTableInfo.Filters, choiceFieldDefinitions, theTbl.TblID, firstRowNum - 1, numRows);
                //ProfileSimple.End("GetDataTable"); // QUERYTIMING
                //ProfileSimple.Start("ParseDataTable"); // QUERYTIMING
                theResults = ParseDataTable(tblColumns, theDataTable);
                //ProfileSimple.End("ParseDataTable"); // QUERYTIMING

                PMCacheManagement.AddItemToCache(cacheString + "FastQueryResults", myDependencies, theResults, new TimeSpan(0, 0, 15));
                //ProfileSimple.End("DoQuery"); // QUERYTIMING
            }

            bodyRowList = theResults.bodyRowList;
            rowCount = theResults.rowCount;
        }

        private static DoQueryResults ParseDataTable(List<TblColumn> tblColumns, DataTable theDataTable)
        {
            List<InfoForBodyRows> infoList = new List<InfoForBodyRows>();
            int rowCount = 0;
            for (int rowNum = 0; rowNum < theDataTable.Rows.Count; rowNum++)
            {
                var row = theDataTable.Rows[rowNum];
                foreach (var col in tblColumns)
                {
                    if (rowCount == 0)
                        rowCount = (int)row["row_count"];
                    string colS = col.TblColumnID.ToString();
                    InfoForBodyRows info = new InfoForBodyRows
                    {
                        RowHeadingWithPopup = row["RH"] as string,
                        TblRowID = row["ID"] is System.DBNull ? 0 : (int)row["ID"],
                        TblColumnID = col.TblColumnID,
                        TopRatingGroupID = row["RG" + colS] is System.DBNull ? 0 : (int)row["RG" + colS],
                        FirstRatingID = row["R" + colS] is System.DBNull ? 0 : (int)row["R" + colS],
                        DecPlaces = null, // ignored
                        ValueOfFirstRating = null, // ignored
                        SingleNumberOnly = !(row["RS" + colS] as string).Contains("<"),
                        Trusted = null, // ignored
                        Deleted = (bool)row["DEL"],
                        PreformattedString = row["RS" + colS] as string
                    };
                    infoList.Add(info);
                }
            }

            var theResults = new DoQueryResults { bodyRowList = infoList, rowCount = rowCount };
            return theResults;
        }

        public static bool FastAccessTablesEnabled()
        {
            return true;
            // Note: after turning on by changing to true, must drop tables on /admin/tester.aspx, and then when this is true they'll be added again.
        }
    }
}
