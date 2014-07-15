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
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    public static class FastAccessTablesMaintenance
    {

        public static bool DoBulkInserting = false; // We don't need this anymore, now that we've implemented individual updating, but we'll keep it in the code for the time being.
        public static bool DoBulkUpdating = false; // We don't need this either. Eventually, if we are sure that our current approach is satisfactory, we'll get rid of the underlying bulk code.

        public static bool ContinueFastAccessMaintenance(IR8RDataContext iDataContext, DenormalizedTableAccess dta)
        {

            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
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

        internal static bool ContinueCreatingFastAccessTables(IR8RDataContext iDataContext, DenormalizedTableAccess dta)
        {
            int numToProcess = 5;
            List<Tbl> tblsToBeCopied = iDataContext.GetTable<Tbl>().Where(x => x.FastTableSyncStatus == (int)FastAccessTableStatus.fastAccessNotCreated).Take(numToProcess).ToList();
            foreach (Tbl tbl in tblsToBeCopied)
            {
                new FastAccessTableInfo(iDataContext, tbl).AddTable(dta); // will drop first if it exists
                tbl.FastTableSyncStatus = (int)FastAccessTableStatus.newRowsMustBeCopied;
            }
            return tblsToBeCopied.Count() == numToProcess; // if so, there may be more work to do.
        }

        internal static bool ContinueAddingNewRows(IR8RDataContext iDataContext, DenormalizedTableAccess dta)
        {
            if (!DoBulkInserting)
                return false;

            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
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
            FastAccessTableInfo tinfo = (FastAccessTableInfo)CacheManagement.GetItemFromCache(cacheKey);
            if (tinfo == null)
            {
                tinfo = new FastAccessTableInfo(iDataContext, theTbl);
                CacheManagement.AddItemToCache(cacheKey, new string[] { "TblID" + theTbl.TblID }, tinfo);
            }

            List<TblRow> tblRowsToDeleteBecauseOfPastFailure = iDataContext
                .GetTable<TblRow>()
                .Where(x => x.Tbl == theTbl && x.InitialFieldsDisplaySet == true && x.FastAccessDeleteThenRecopy == true)
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
                .Where(x => x.Tbl == theTbl && x.InitialFieldsDisplaySet == true && x.FastAccessInitialCopy == true)
                .Take(numToTake)
                .ToList();

            int numRows = numToTake;
            try
            {
                numRows = tinfo.BulkCopyRows(iDataContext, dta, tblRowsToAdd, tblColumns);
                foreach (var tr in tblRowsToAdd)
                    tr.FastAccessInitialCopy = false;
            }
            catch
            { // this could be because row has already been copied, or for some other reason. either way, we'll back up and try to delete the row before continuing.
                foreach (var tr in tblRowsToAdd)
                    tr.FastAccessDeleteThenRecopy = true;
            }
            iDataContext.SubmitChanges(); // this will reduce the complication associated with handling change conflict exception if somewhere else we have set FastTableSyncStatus to indicate that new rows must be added -- it doesn't matter because we'll find the new rows eventually anyway

            if (numRows == 0 || numRows < numToTake)
                theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.apparentlySynchronized; // if another row has been added, then this will result in a change conflict exception that is then handled 

            return true;
        }

        [Serializable]
        internal class FastAccessRowRequiringUpdate : IComparer
        {
            public string TableName { get; set; }
            public Guid TblRowID { get; set; }
            public bool UpdateRatings { get; set; }
            public bool UpdateFields { get; set; }

            public FastAccessRowRequiringUpdate(string tableName, Guid tblRowID, bool updateRatings, bool updateFields)
            {
                TableName = tableName;
                TblRowID = tblRowID;
                UpdateRatings = updateRatings;
                UpdateFields = updateFields;
            }

            public int Compare(object x, object y)
            {
                FastAccessRowRequiringUpdate x1 = x as FastAccessRowRequiringUpdate;
                FastAccessRowRequiringUpdate y1 = y as FastAccessRowRequiringUpdate;
                return String.Compare(x1.TableName + x1.TblRowID.ToString() + x1.UpdateRatings.ToString() + x1.UpdateFields.ToString(), y1.TableName + y1.TblRowID.ToString() + y1.UpdateRatings.ToString() + y1.UpdateFields.ToString());
            }

        }

        [Serializable]
        internal class RowsRequiringUpdate
        {
            public List<FastAccessRowRequiringUpdate> Rows { get; set; }
        }

        static bool useAzureQueuesToDesignateRowsToUpdate = false; // for now, we are disabling the azure approach, because querying based on tbl row ids identified by number seems to tax sql server

        public static void IdentifyRowRequiringBulkUpdate(IR8RDataContext iDataContext, Tbl theTbl, TblRow theTblRow, bool updateRatings, bool updateFields)
        {
            if (useAzureQueuesToDesignateRowsToUpdate)
            {
                IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
                if (dataContext == null)
                    return;
                FastAccessRowRequiringUpdate row = new FastAccessRowRequiringUpdate("V" + theTbl.TblID.ToString(), theTblRow.TblRowID, updateRatings, updateFields);
                List<FastAccessRowRequiringUpdate> theRowsRequiringUpdates = iDataContext.TempCacheGet("fasttablerowupdate") as List<FastAccessRowRequiringUpdate>;
                if (theRowsRequiringUpdates == null)
                    theRowsRequiringUpdates = new List<FastAccessRowRequiringUpdate>();
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

        public static void PushRowsRequiringUpdateToAzureQueue(IR8RDataContext iDataContext)
        {
            if (!useAzureQueuesToDesignateRowsToUpdate)
                return;

            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            List<FastAccessRowRequiringUpdate> theRowsRequiringUpdates = iDataContext.TempCacheGet("fasttablerowupdate") as List<FastAccessRowRequiringUpdate>;
            if (theRowsRequiringUpdates != null)
            {
                RowsRequiringUpdate theRows = new RowsRequiringUpdate { Rows = theRowsRequiringUpdates.ToList() };

                AzureQueue.Push("fasttablerowupdate", theRows);
            }
            iDataContext.TempCacheAdd("fasttablerowupdate", null);
        }

        internal static bool ContinueUpdate_AzureVersion(IR8RDataContext iDataContext, DenormalizedTableAccess dta)
        {
            // We should probably not resurrect this, as the AzureQueueWithErrorRecovery simply drops the entire table and rebuilds if it runs into trouble.
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;
            const int numAtOnce = 25; // 100 produced an error for too many parameters, presumably because we enumerate the tbl rows we want and then query for the fields. 

            var queue = new AzureQueueWithErrorRecovery(10, null);
            List<FastAccessRowRequiringUpdate> theRows = new List<FastAccessRowRequiringUpdate>();
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
                Guid tblID = Convert.ToInt32(table.FirstOrDefault().Item.TableName.Substring(1));
                Tbl theTbl = iDataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);
                bool newRowsAdded = table.Any(x => x.Item.TblRowID == Guid.NewGuid()); // DEBUG -- test for whether it's new // we can't use update for this, because we don't know the TblRowID yet.
                List<Guid> tblRowIDs = table.Select(x => x.Item.TblRowID).OrderBy(x => x).Distinct().ToList();
                IQueryable<TblRow> tblRows = iDataContext.GetTable<TblRow>().Where(x => tblRowIDs.Contains(x.TblRowID));
                new FastAccessTableInfo(iDataContext, theTbl).UpdateRows(dta, tblRows, table.FirstOrDefault().Item.UpdateRatings, table.FirstOrDefault().Item.UpdateFields);
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

        internal static bool ContinueBulkUpdating(IR8RDataContext iDataContext, DenormalizedTableAccess dta)
        {
            if (!DoBulkUpdating && !DoBulkInserting)
                return false;

            if (useAzureQueuesToDesignateRowsToUpdate)
                return ContinueUpdate_AzureVersion(iDataContext, dta); // seems to create more problems but we'll keep the code around for now in case we change our mind
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;
            const int numAtOnce = 100;
            bool noMoreWork = true; // assume for now
            List<TblRow> theRows = iDataContext.GetTable<TblRow>().Where(x => x.InitialFieldsDisplaySet == true && (x.FastAccessUpdateFields || x.FastAccessUpdateRatings) && !x.FastAccessUpdateSpecified /* NOTE: The FastAccessUpdateSpecified is for individual updating. If that is true, we may be adding the row via individual updating. If we're using individual updating to add rows, as we now are, instead of bulk adding, then we must be sure for bulk updating of items waits until they're added, so wait for FastAccessUpdateSpecified to be complete.  */ ).Take(numAtOnce).ToList();
            if (theRows.Count() == numAtOnce)
                noMoreWork = false;
            var theRowsByTableAndUpdateInstruction = theRows.Select(x => new { Item = x, GroupByInstruct = x.TblID + x.FastAccessUpdateFields.ToString() + x.FastAccessUpdateRatings.ToString() }).GroupBy(x => x.GroupByInstruct);
            foreach (var group in theRowsByTableAndUpdateInstruction)
            {
                Tbl theTbl = group.FirstOrDefault().Item.Tbl;
                Guid tblID = theTbl.TblID;
                IQueryable<TblRow> tblRows = iDataContext.GetTable<TblRow>().Where(x => x.TblID == tblID && (x.FastAccessUpdateFields || x.FastAccessUpdateRatings)).Take(numAtOnce);
                int numRowsUpdated = new FastAccessTableInfo(iDataContext, theTbl).UpdateRows(dta, tblRows, group.FirstOrDefault().Item.FastAccessUpdateRatings, group.FirstOrDefault().Item.FastAccessUpdateFields);
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

        static bool DisableIndividualUpdating = false;
        internal static bool ContinueIndividualUpdating(IR8RDataContext iDataContext, DenormalizedTableAccess dta)
        {
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null || DisableIndividualUpdating)
                return false;
            const int numAtOnce = 2000;
            bool noMoreWork = true; // assume for now
            List<TblRow> theRows = iDataContext.GetTable<TblRow>().Where(x => x.InitialFieldsDisplaySet == true && x.FastAccessUpdateSpecified).Take(numAtOnce).ToList();
            int rowsCount = theRows.Count();
            if (rowsCount == 0)
                return false;
            if (rowsCount == numAtOnce)
                noMoreWork = false;

            // Changes from a single TblRow in a single Tbl can require changes (upserts, updates, deletes) to more than one destination table,
            // the primary table as well as any secondary tables containing data that we join with data from the primary table.
            // We thus go through each primary table in the outer loop, by TblRow in the next loop, by destination table in the loop within that,
            // and by destination row in the furthest inside loop. In that innermost loop, we can produce a SQLUpdateRowInfo for a row within that
            // destination table. That can then be added to a SQLUpdateRowsInfo for a destination table.
                

            var rowsGroupedBySourceTable = theRows.GroupBy(x => x.TblID);

            foreach (var groupOfUpdatesForSourceTable in rowsGroupedBySourceTable)
            {
                SQLDatabaseChangeInfo dci = new SQLDatabaseChangeInfo();
                foreach (TblRow tblRow in theRows)
                {
                    SQLInfoForCellsInRow_MainAndSecondaryTables toUpdateList = FastAccessRowUpdateInfo.GetFastAccessRowUpdateInfoList(tblRow);
                    dci.Rows.Add(toUpdateList);
                }
                dci.ExecuteAllCommands(dta);
            }

            foreach (TblRow row in theRows)
            {
                row.FastAccessUpdateSpecified = false;
                row.FastAccessUpdated = null;
            }

            return !noMoreWork;
        }

    }
}
