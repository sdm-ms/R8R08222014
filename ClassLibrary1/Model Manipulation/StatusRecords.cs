
using System;
using System.Linq;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using System.Collections.Generic;
using ClassLibrary1.Misc;
namespace ClassLibrary1.Model
{
    public static class StatusRecords
    {
        internal class TblRowUpdateRecentStatusTracker
        {
#pragma warning disable 0649
            public TblRowStatusRecord tblRowStatusRecord;
            public bool anyToKeepForThisTblRow;
#pragma warning restore 0649
        }

        public static bool RecordRecentChangesInStatusRecords = false; // We are disabling this feature. If enabling it, we would need to copy the TblRowStatusRecords to the denormalized database, so that we could then filter queries to figure out state of tables at particular time in the past.

        internal static void RememberChangeForLater(IR8RDataContext R8RDB, string changeType, Guid changeNum)
        {
            List<Guid> theList = R8RDB.TempCacheGet(changeType) as List<Guid>;
            if (theList == null)
                theList = new List<Guid>();
            if (!theList.Contains(changeNum))
            {
                theList.Add(changeNum);
                R8RDB.TempCacheAdd(changeType, theList);
            }
        }

        internal static void RecordRememberedChangesOfType(IR8RDataContext R8RDB, string changeType, Action<Guid> theAction)
        {
            List<Guid> theList = R8RDB.TempCacheGet(changeType) as List<Guid>;
            if (theList != null)
                foreach (var item in theList)
                    theAction(item);
        }

        public static void RecordRememberedStatusRecordChanges(IR8RDataContext R8RDB)
        {
            RecordRememberedChangesOfType(R8RDB, "TblStatusChangeList", RecordRowStatusChange);
            RecordRememberedChangesOfType(R8RDB, "LastTblColumnRatingChange", RecordRatingChange);
        }

        public static void PrepareToRecordRowStatusChange(IR8RDataContext R8RDB, Guid tblID)
        {
            RememberChangeForLater(R8RDB, "TblStatusChangeList", tblID);
        }

        public static void PrepareToRecordRatingChange(IR8RDataContext R8RDB, Guid tblColumnID)
        {
            RememberChangeForLater(R8RDB, "LastTblColumnRatingChange", tblColumnID);
        }

        internal static void RecordRowStatusChange(Guid tblID)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                return;
            DateTime asOfDateTime = TestableDateTime.Now;
            TableServiceContextAccess<DataTableServiceEntity<DateTime>> context = null;
            DataTableServiceEntity<DateTime> theLastDateTime = AzureTable<DateTime>.LoadFirstOrDefaultByPartitionKey(tblID.ToString(), ref context, "LastTblStatusChange");
            if (theLastDateTime == null)
                AzureTable<DateTime>.Add(asOfDateTime,tblID.ToString(), "LastTblStatusChange");
            else
            {
                theLastDateTime.SetData(asOfDateTime);
                AzureTable<DateTime>.Update(theLastDateTime, context, "LastTblStatusChange");
            }
        }

        internal static void RecordRatingChange( Guid tblColumnID)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                return;
            DateTime asOfDateTime = TestableDateTime.Now;
            TableServiceContextAccess<DataTableServiceEntity<DateTime>> context = null;
            var theLastDateTime = AzureTable<DateTime>.LoadFirstOrDefaultByPartitionKey(tblColumnID.ToString(), ref context, "LastTblColumnRatingChange");
            if (theLastDateTime == null)
                AzureTable<DateTime>.Add(asOfDateTime, tblColumnID.ToString(), "LastTblColumnRatingChange");
            else
            {
                theLastDateTime.SetData(asOfDateTime);
                AzureTable<DateTime>.Update(theLastDateTime, context, "LastTblColumnRatingChange");
            }
        }

        public static bool RowStatusHasChangedSince(DateTime earliestTimeToConsider, Guid tblID)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                throw new Exception("Cannot determine whether row status has changed.");
            TableServiceContextAccess<DataTableServiceEntity<DateTime>> context = null;
            DateTime? theLastDateTime = AzureTable<DateTime>.LoadDataOnlyByPartitionKey(tblID.ToString(), ref context, "LastTblStatusChange").FirstOrDefault();
            if (theLastDateTime == null)
                return false;
            else
                return (theLastDateTime > earliestTimeToConsider);
        }

        public static bool RatingHasBeenRecordedSince(DateTime earliestTimeToConsider, Guid tblColumnID)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                throw new Exception("Cannot determine whether rating has been changed recently.");
            TableServiceContextAccess<DataTableServiceEntity<DateTime>> context = null;
            DateTime? theLastDateTime = AzureTable<DateTime>.LoadDataOnlyByPartitionKey(tblColumnID.ToString(), ref context, "LastTblColumnRatingChange").FirstOrDefault();
            if (theLastDateTime == null)
                return false;
            else
                return (theLastDateTime > earliestTimeToConsider);
        }

        public static bool TblRowStatusRecordsExistSince(IR8RDataContext rdc, DateTime asOfDateTime)
        {
            return rdc.GetTable<TblRowStatusRecord>().Any(x => x.TimeChanged > asOfDateTime);
        }


        public static bool RatingGroupStatusRecordsExistSince(IR8RDataContext rdc, DateTime asOfDateTime)
        {
            return rdc.GetTable<RatingGroupStatusRecord>().Any(x => x.NewValueTime > asOfDateTime);
        }

        public static bool DeleteOldStatusRecords(IR8RDataContext theDataContext)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                return false;
            return DeleteOldTblRowStatusRecords(theDataContext) || DeleteOldRatingGroupStatusRecords(theDataContext);
        }

        public static bool DeleteOldTblRowStatusRecords(IR8RDataContext theDataContext)
        {
            // We delete the old table row status records that we no longer need
            // to track in an effort to keep search results current. We must
            // update the StatusRecentlyChanged field for table rows with no
            // remaining TblRowStatusRecords after the deletion.
            DateTime oldestToKeep = TestableDateTime.Now - new TimeSpan(0,30,0);
            var tblRowStatusRecordsToDelete = theDataContext.GetTable<TblRowStatusRecord>()
                .Where(x => x.TimeChanged < oldestToKeep)
                .Select(x => new TblRowUpdateRecentStatusTracker
                    {
                        tblRowStatusRecord = x,
                        anyToKeepForThisTblRow = x.TblRow.TblRowStatusRecords.Any(y => y.TimeChanged >= oldestToKeep)
                    }).ToList();
            foreach (var theOneToDelete in tblRowStatusRecordsToDelete)
            {
                theDataContext.GetTable<TblRowStatusRecord>().DeleteOnSubmit(theOneToDelete.tblRowStatusRecord);
                if (!theOneToDelete.anyToKeepForThisTblRow)
                    theOneToDelete.tblRowStatusRecord.TblRow.StatusRecentlyChanged = false;
            }
            return false; // no more work to do for a while.
                
        }


        internal class RatingGroupUpdateRecentStatusTracker
        {
#pragma warning disable 0649
            public RatingGroupStatusRecord ratingGroupStatusRecord;
            public bool anyToKeepForThisRatingGroup;
            public TblRow tblRow;
            public TblColumn tblCol;
            public Tbl tbl;
#pragma warning restore 0649
        }

        public static bool DeleteOldRatingGroupStatusRecords(IR8RDataContext theDataContext)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                return false;
            DateTime oldestToKeep = TestableDateTime.Now - new TimeSpan(0, 30, 0);
            var RatingGroupStatusRecordsToDelete = theDataContext.GetTable<RatingGroupStatusRecord>()
                .Where(x => x.NewValueTime < oldestToKeep)
                .Select(x => new RatingGroupUpdateRecentStatusTracker
                {
                    ratingGroupStatusRecord = x,
                    anyToKeepForThisRatingGroup = x.RatingGroup.RatingGroupStatusRecords.Any(y => y.NewValueTime >= oldestToKeep),
                    tblRow = x.RatingGroup.TblRow,
                    tblCol = x.RatingGroup.TblColumn,
                    tbl = x.RatingGroup.TblRow.Tbl
                }).ToList();
            foreach (var theOneToDelete in RatingGroupStatusRecordsToDelete)
            {
                theDataContext.GetTable<RatingGroupStatusRecord>().DeleteOnSubmit(theOneToDelete.ratingGroupStatusRecord);
                if (!theOneToDelete.anyToKeepForThisRatingGroup)
                {
                    theOneToDelete.ratingGroupStatusRecord.RatingGroup.ValueRecentlyChanged = false;

                }
            }
            return false; // no more work to do for a while.

        }
    }
}