using System;
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
    public static class FastAccessTablesMaintenance
    {

        public static bool RecordRecentChangesInStatusRecords = false; // We are disabling this feature. If enabling it, we would need to copy the TblRowStatusRecords to the denormalized database.

        public static bool DoBulkUpdating = true; // DEBUG -- once we get rid of automatically creating missing ratings, we can get rid of this

        public static int CountHighestRecord(DenormalizedTableAccess dta, string tableName)
        {
            // Note: We may not be able to rely on this if we change from numeric IDs.
            object result = SQLDirectManipulate.ExecuteSQLScalar(dta, String.Format("SELECT TOP 1 [ID] FROM [dbo].[{0}] ORDER BY [ID] DESC", tableName));
            if (result == null)
                return 0;
            else
                return (int)result;
        }


        public static bool ContinueFastAccessMaintenance(IRaterooDataContext iDataContext, DenormalizedTableAccess dta)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null || !RoleEnvironment.IsAvailable)
                return false;

            bool moreTableCreationToDo = false;
            bool moreBulkCopyingToDo = false;
            bool moreBulkUpdatingToDo = false;
            bool moreIndividualUpdatingToDo = false;
            moreTableCreationToDo = ContinueCreatingFastAccessTables(iDataContext, dta);
            moreBulkCopyingToDo = ContinueAddingNewRows(iDataContext, dta); // this will not conflict with above, because it will filter out cases where Tbl hasn't been created
            if (!moreTableCreationToDo && !moreBulkCopyingToDo)
                moreBulkUpdatingToDo = ContinueBulkUpdating(iDataContext, dta);
            moreIndividualUpdatingToDo = ContinueIndividualUpdating(iDataContext, dta); // shouldn't conflict, because something must be visible to users before it can be updated
            return moreTableCreationToDo || moreBulkCopyingToDo || moreBulkUpdatingToDo || moreIndividualUpdatingToDo;
        }

        internal static bool ContinueCreatingFastAccessTables(IRaterooDataContext iDataContext, DenormalizedTableAccess dta)
        {
            int numToProcess = 5;
            List<Tbl> tblsToBeCopied = iDataContext.GetTable<Tbl>().Where(x => x.FastTableSyncStatus == (int)FastAccessTableStatus.fastAccessNotCreated).Take(numToProcess).ToList();
            foreach (Tbl tbl in tblsToBeCopied)
            {
                new SQLFastAccessTableInfo(iDataContext, tbl).AddTable(dta); // will drop first if it exists
                tbl.FastTableSyncStatus = (int)FastAccessTableStatus.newRowsMustBeCopied;
            }
            return tblsToBeCopied.Count() == numToProcess; // if so, there may be more work to do.
        }

        internal static bool ContinueAddingNewRows(IRaterooDataContext iDataContext, DenormalizedTableAccess dta)
        {
            if (!DoBulkUpdating)
                return false;

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;

            if (!FastAccessTablesQuery.FastAccessTablesEnabled())
                return false; // not enabled, so no more work to do.

            Tbl theTbl = iDataContext.GetTable<Tbl>().FirstOrDefault(x => x.FastTableSyncStatus == (int)FastAccessTableStatus.newRowsMustBeCopied);
            if (theTbl == null)
            {
                theTbl = iDataContext.GetTable<Tbl>().FirstOrDefault(x => x.FastTableSyncStatus == (int)FastAccessTableStatus.apparentlySynchronized && x.TblRows.Any(y => y.FastAccessInitialCopy || y.FastAccessDeleteThenRecopy)); // apparently synchronized but not actually synchronized
                if (theTbl != null)
                    theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.newRowsMustBeCopied;
            }
            if (theTbl == null)
                return false;

            const int numToTake = 500;

            List<TblColumn> tblColumns = theTbl.TblTabs.SelectMany(x => x.TblColumns).ToList();

            string cacheKey = "fastaccesstableinfo" + theTbl.TblID;
            SQLFastAccessTableInfo tinfo = (SQLFastAccessTableInfo)CacheManagement.GetItemFromCache(cacheKey);
            if (tinfo == null)
            {
                tinfo = new SQLFastAccessTableInfo(iDataContext, theTbl);
                CacheManagement.AddItemToCache(cacheKey, new string[] { "TblID" + theTbl.TblID }, tinfo);
            }

            List<TblRow> tblRowsToDeleteBecauseOfPastFailure = iDataContext
                .GetTable<TblRow>()
                .Where(x => x.InitialFieldsDisplaySet == true && x.FastAccessDeleteThenRecopy == true)
                .Take(numToTake)
                .ToList();
            if (tblRowsToDeleteBecauseOfPastFailure.Any())
            {
                SQLDirectManipulate.DeleteMatchingItems(dta, "V" + theTbl.TblID, "ID", tblRowsToDeleteBecauseOfPastFailure.Select(x => x.TblRowID.ToString()).ToList());
                foreach (var tr in tblRowsToDeleteBecauseOfPastFailure)
                {
                    tr.FastAccessDeleteThenRecopy = false;
                    tr.FastAccessInitialCopy = true;
                }
            }

            List<TblRow> tblRowsToAdd = iDataContext
                .GetTable<TblRow>()
                .Where(x => x.InitialFieldsDisplaySet == true && x.FastAccessInitialCopy == true)
                .Take(numToTake)
                .ToList();

            int numRows = numToTake;
            bool adjustRowCount = true;
            try
            {
                numRows = tinfo.BulkCopyRows(iDataContext, dta, tblRowsToAdd, tblColumns);
                foreach (var tr in tblRowsToAdd)
                    tr.FastAccessInitialCopy = false;
            }
            catch
            { // this could be because row has already been copied, or for some other reason. either way, we'll back up and try to delete the row before continuing.
                adjustRowCount = false;
                foreach (var tr in tblRowsToAdd)
                    tr.FastAccessDeleteThenRecopy = true;
            }
            if (adjustRowCount)
                tinfo.AdjustRowCountForAddedRows(theTbl, tblColumns);
            iDataContext.SubmitChanges(); // this will reduce the complication associated with handling change conflict exception if somewhere else we have set FastTableSyncStatus to indicate that new rows must be added -- it doesn't matter because we'll find the new rows eventually anyway

            if (numRows == 0 || numRows < numToTake)
                theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.apparentlySynchronized; // if another row has been added, then this will result in a change conflict exception that is then handled 

            return true;
        }

        [Serializable]
        internal class RowRequiringUpdate : IComparer
        {
            public string TableName { get; set; }
            public int TblRowID { get; set; }
            public bool UpdateRatings { get; set; }
            public bool UpdateFields { get; set; }

            public RowRequiringUpdate(string tableName, int tblRowID, bool updateRatings, bool updateFields)
            {
                TableName = tableName;
                TblRowID = tblRowID;
                UpdateRatings = updateRatings;
                UpdateFields = updateFields;
            }

            public int Compare(object x, object y)
            {
                RowRequiringUpdate x1 = x as RowRequiringUpdate;
                RowRequiringUpdate y1 = y as RowRequiringUpdate;
                return String.Compare(x1.TableName + x1.TblRowID.ToString() + x1.UpdateRatings.ToString() + x1.UpdateFields.ToString(), y1.TableName + y1.TblRowID.ToString() + y1.UpdateRatings.ToString() + y1.UpdateFields.ToString());
            }

        }

        [Serializable]
        internal class RowsRequiringUpdate
        {
            public List<RowRequiringUpdate> Rows { get; set; }
        }

        static bool useAzureQueuesToDesignateRowsToUpdate = false; // for now, we are disabling the azure approach, because querying based on tbl row ids identified by number seems to tax sql server

        public static void IdentifyRowRequiringBulkUpdate(IRaterooDataContext iDataContext, Tbl theTbl, TblRow theTblRow, bool updateRatings, bool updateFields)
        {
            if (useAzureQueuesToDesignateRowsToUpdate)
            {
                RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
                if (dataContext == null)
                    return;
                RowRequiringUpdate row = new RowRequiringUpdate("V" + theTbl.TblID.ToString(), theTblRow.TblRowID, updateRatings, updateFields);
                List<RowRequiringUpdate> theRowsRequiringUpdates = iDataContext.TempCacheGet("fasttablerowupdate") as List<RowRequiringUpdate>;
                if (theRowsRequiringUpdates == null)
                    theRowsRequiringUpdates = new List<RowRequiringUpdate>();
                if (!theRowsRequiringUpdates.Any(x => x.TableName == "V" + theTbl.TblID.ToString() && x.TblRowID == theTblRow.TblRowID && x.UpdateFields == updateFields && x.UpdateRatings == updateRatings))
                    theRowsRequiringUpdates.Add(row);
                iDataContext.TempCacheAdd("fasttablerowupdate", theRowsRequiringUpdates);
            }
            else
            {
                // note that if one of these is false, we don't set it to false, since that might undo a separate update instruction
                if (updateRatings)
                    theTblRow.FastAccessUpdateRatings = true;
                if (updateFields)
                    theTblRow.FastAccessUpdateFields = true;
            }
        }

        public static void PushRowsRequiringUpdateToAzureQueue(IRaterooDataContext iDataContext)
        {
            if (!useAzureQueuesToDesignateRowsToUpdate)
                return;

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            List<RowRequiringUpdate> theRowsRequiringUpdates = iDataContext.TempCacheGet("fasttablerowupdate") as List<RowRequiringUpdate>;
            if (theRowsRequiringUpdates != null)
            {
                RowsRequiringUpdate theRows = new RowsRequiringUpdate { Rows = theRowsRequiringUpdates.ToList() };

                AzureQueue.Push("fasttablerowupdate", theRows);
            }
            iDataContext.TempCacheAdd("fasttablerowupdate", null);
        }

        internal static bool ContinueUpdate_AzureVersion(IRaterooDataContext iDataContext, DenormalizedTableAccess dta)
        {
            // We should probably not resurrect this, as the AzureQueueWithErrorRecovery simply drops the entire table and rebuilds if it runs into trouble.
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;
            const int numAtOnce = 25; // 100 produced an error for too many parameters, presumably because we enumerate the tbl rows we want and then query for the fields. 

            var queue = new AzureQueueWithErrorRecovery(10, null);
            List<RowRequiringUpdate> theRows = new List<RowRequiringUpdate>();
            var theMessages = queue.GetMessages("fasttablerowupdate", numAtOnce);
            foreach (var setOfRows in theMessages)
                theRows.AddRange(((RowsRequiringUpdate)setOfRows).Rows);
            if (!FastAccessTablesQuery.FastAccessTablesEnabled())
            {
                queue.ConfirmProperExecution("fasttablerowupdate");
                return theRows.Any();
            }
            var theRowsByTableAndUpdateInstruction = theRows.Select(x => new { Item = x, GroupByInstruct = x.TableName + x.UpdateFields.ToString() + x.UpdateRatings.ToString() }).GroupBy(x => x.GroupByInstruct);
            bool anyNewRowsAdded = false;
            foreach (var table in theRowsByTableAndUpdateInstruction)
            {
                int tblID = Convert.ToInt32(table.First().Item.TableName.Substring(1));
                Tbl theTbl = iDataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);
                bool newRowsAdded = table.Any(x => x.Item.TblRowID == 0); // we can't use update for this, because we don't know the TblRowID yet.
                List<int> tblRowIDs = table.Select(x => x.Item.TblRowID).OrderBy(x => x).Distinct().ToList();
                IQueryable<TblRow> tblRows = iDataContext.GetTable<TblRow>().Where(x => tblRowIDs.Contains(x.TblRowID));
                new SQLFastAccessTableInfo(iDataContext, theTbl).UpdateRows(dta, tblRows, table.First().Item.UpdateRatings, table.First().Item.UpdateFields);
                if (newRowsAdded && theTbl.FastTableSyncStatus == (int)FastAccessTableStatus.apparentlySynchronized)
                {
                    theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.newRowsMustBeCopied;
                    CacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
                    dataContext.SubmitChanges();
                    anyNewRowsAdded = true;
                }
            }
            queue.ConfirmProperExecution("fasttablerowupdate");
            if (anyNewRowsAdded)
                ContinueAddingNewRows(iDataContext, dta);
            return theRows.Count() == numAtOnce; // more work to do
        }


        internal static bool ContinueBulkUpdating(IRaterooDataContext iDataContext, DenormalizedTableAccess dta)
        {
            if (!DoBulkUpdating)
                return false;

            if (useAzureQueuesToDesignateRowsToUpdate)
                return ContinueUpdate_AzureVersion(iDataContext, dta); // seems to create more problems but we'll keep the code around for now in case we change our mind
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;
            const int numAtOnce = 100;
            bool noMoreWork = true; // assume for now
            List<TblRow> theRows = iDataContext.GetTable<TblRow>().Where(x => x.InitialFieldsDisplaySet == true && (x.FastAccessUpdateFields || x.FastAccessUpdateRatings)).Take(numAtOnce).ToList();
            if (theRows.Count() == numAtOnce)
                noMoreWork = false;
            var theRowsByTableAndUpdateInstruction = theRows.Select(x => new { Item = x, GroupByInstruct = x.TblID + x.FastAccessUpdateFields.ToString() + x.FastAccessUpdateRatings.ToString() }).GroupBy(x => x.GroupByInstruct);
            foreach (var group in theRowsByTableAndUpdateInstruction)
            {
                Tbl theTbl = group.First().Item.Tbl;
                int tblID = theTbl.TblID;
                IQueryable<TblRow> tblRows = iDataContext.GetTable<TblRow>().Where(x => x.TblID == tblID && (x.FastAccessUpdateFields || x.FastAccessUpdateRatings)).Take(numAtOnce);
                int numRowsUpdated = new SQLFastAccessTableInfo(iDataContext, theTbl).UpdateRows(dta, tblRows, group.First().Item.FastAccessUpdateRatings, group.First().Item.FastAccessUpdateFields);
                foreach (TblRow tblRow in group.Select(x => x.Item))
                {
                    tblRow.FastAccessUpdateFields = false;
                    tblRow.FastAccessUpdateRatings = false;
                }
                if (numRowsUpdated == numAtOnce)
                    noMoreWork = false;

                CacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
            }
            return !noMoreWork;
        }

        internal static void ResetIndividualUpdatingFlags(TblRow tblRow)
        {
            tblRow.FastAccessUpdateSpecified = false;
            tblRow.FastAccessUpdated = null;
        }

        internal static bool ContinueIndividualUpdating(IRaterooDataContext iDataContext, DenormalizedTableAccess dta)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;
            const int numAtOnce = 2000;
            bool noMoreWork = true; // assume for now
            List<TblRow> theRows = iDataContext.GetTable<TblRow>().Where(x => x.InitialFieldsDisplaySet == true && x.FastAccessUpdateSpecified).Take(numAtOnce).ToList();
            int rowsCount = theRows.Count();
            if (rowsCount == 0)
                return false;
            if (rowsCount == numAtOnce)
                noMoreWork = false;

            var groupedRows = theRows.GroupBy(x => x.TblID);

            SQLUpdateTablesInfo allTables = new SQLUpdateTablesInfo();

            foreach (var group in groupedRows)
            {
                string tableName = "V" + group.Key.ToString();
                SQLUpdateRowsInfo sqlUpdateRowsInfo = new SQLUpdateRowsInfo() { TableName = tableName };
                foreach (TblRow tblRow in theRows)
                {
                    List<SQLUpdateInfo> toUpdateList = FastAccessRowUpdateInfo.GetFastAccessRowUpdateInfoList(tblRow);
                    SQLUpdateRowInfo singleRow = new SQLUpdateRowInfo(tblRow.TblRowID, tableName, () => ResetIndividualUpdatingFlags(tblRow), () => tblRow.FastAccessUpdateSpecified == false) { SQLUpdateInfos = toUpdateList };
                    sqlUpdateRowsInfo.Rows.Add(singleRow);
                }
                allTables.TablesContainingInformationToUpdate.Add(sqlUpdateRowsInfo);
            }
            allTables.DoUpdate(dta);
            return !noMoreWork;
        }

    }
}
