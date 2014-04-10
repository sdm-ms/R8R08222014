
using System;
using System.Linq;
using ClassLibrary1.Model;
using System.Collections.Generic;
using ClassLibrary1.Misc;
namespace ClassLibrary1.Model
{
    public static class StatusRecords
    {
        internal class TblRowUpdateRecentStatusTracker
        {
#pragma warning disable 0649
            public TblRowStatusRecord entityStatusRecord;
            public bool anyToKeepForThisTblRow;
#pragma warning restore 0649
        }

        internal static void RememberChangeForLater(IRaterooDataContext RaterooDB, string changeType, int changeNum)
        {
            List<int> theList = RaterooDB.TempCacheGet(changeType) as List<int>;
            if (theList == null)
                theList = new List<int>();
            if (!theList.Contains(changeNum))
            {
                theList.Add(changeNum);
                RaterooDB.TempCacheAdd(changeType, theList);
            }
        }

        internal static void RecordRememberedChangesOfType(IRaterooDataContext RaterooDB, string changeType, Action<int> theAction)
        {
            List<int> theList = RaterooDB.TempCacheGet(changeType) as List<int>;
            if (theList != null)
                foreach (var item in theList)
                    theAction(item);
        }

        public static void RecordRememberedStatusRecordChanges(IRaterooDataContext RaterooDB)
        {
            RecordRememberedChangesOfType(RaterooDB, "TblStatusChangeList", RecordRowStatusChange);
            RecordRememberedChangesOfType(RaterooDB, "LastTblColumnRatingChange", RecordRatingChange);
        }

        public static void PrepareToRecordRowStatusChange(IRaterooDataContext RaterooDB, int tblID)
        {
            RememberChangeForLater(RaterooDB, "TblStatusChangeList", tblID);
        }

        public static void PrepareToRecordRatingChange(IRaterooDataContext RaterooDB, int tblColumnID)
        {
            RememberChangeForLater(RaterooDB, "LastTblColumnRatingChange", tblColumnID);
        }

        internal static void RecordRowStatusChange(int tblID)
        {
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

        internal static void RecordRatingChange( int tblColumnID)
        {
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

        public static bool RowStatusHasChangedSince(DateTime earliestTimeToConsider, int tblID)
        {
            TableServiceContextAccess<DataTableServiceEntity<DateTime>> context = null;
            DateTime? theLastDateTime = AzureTable<DateTime>.LoadDataOnlyByPartitionKey(tblID.ToString(), ref context, "LastTblStatusChange").FirstOrDefault();
            if (theLastDateTime == null)
                return false;
            else
                return (theLastDateTime > earliestTimeToConsider);
        }

        public static bool RatingHasBeenRecordedSince(DateTime earliestTimeToConsider, int tblColumnID)
        {
            TableServiceContextAccess<DataTableServiceEntity<DateTime>> context = null;
            DateTime? theLastDateTime = AzureTable<DateTime>.LoadDataOnlyByPartitionKey(tblColumnID.ToString(), ref context, "LastTblColumnRatingChange").FirstOrDefault();
            if (theLastDateTime == null)
                return false;
            else
                return (theLastDateTime > earliestTimeToConsider);
        }

        public static bool TblRowStatusRecordsExistSince(IRaterooDataContext rdc, DateTime asOfDateTime)
        {
            return rdc.GetTable<TblRowStatusRecord>().Any(x => x.TimeChanged > asOfDateTime);
        }


        public static bool RatingGroupStatusRecordsExistSince(IRaterooDataContext rdc, DateTime asOfDateTime)
        {
            return rdc.GetTable<RatingGroupStatusRecord>().Any(x => x.NewValueTime > asOfDateTime);
        }

        public static bool DeleteOldStatusRecords(IRaterooDataContext theDataContext)
        {
            return DeleteOldTblRowStatusRecords(theDataContext) || DeleteOldRatingGroupStatusRecords(theDataContext);
        }

        public static bool DeleteOldTblRowStatusRecords(IRaterooDataContext theDataContext)
        {
            // We delete the old entity status records that we no longer need
            // to track in an effort to keep search results current. We must
            // update the StatusRecentlyChanged field for entities with no
            // remaining TblRowStatusRecords after the deletion.
            DateTime oldestToKeep = TestableDateTime.Now - new TimeSpan(0,30,0);
            var entityStatusRecordsToDelete = theDataContext.GetTable<TblRowStatusRecord>()
                .Where(x => x.TimeChanged < oldestToKeep)
                .Select(x => new TblRowUpdateRecentStatusTracker
                    {
                        entityStatusRecord = x,
                        anyToKeepForThisTblRow = x.TblRow.TblRowStatusRecords.Any(y => y.TimeChanged >= oldestToKeep)
                    }).ToList();
            foreach (var theOneToDelete in entityStatusRecordsToDelete)
            {
                theDataContext.GetTable<TblRowStatusRecord>().DeleteOnSubmit(theOneToDelete.entityStatusRecord);
                if (!theOneToDelete.anyToKeepForThisTblRow)
                    theOneToDelete.entityStatusRecord.TblRow.StatusRecentlyChanged = false;
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

        public static bool DeleteOldRatingGroupStatusRecords(IRaterooDataContext theDataContext)
        {
            if (!FastAccessTablesMaintenance.RecordRecentChangesInStatusRecords)
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

                    var farui = new FastAccessRecentlyChangedInfo()
                    {
                        TblColumnID = theOneToDelete.tblCol.TblColumnID,
                        RecentlyChanged = false,
                    };
                    farui.AddToTblRow(theOneToDelete.tblRow);
                }
                FastAccessTablesMaintenance.IdentifyRowRequiringUpdate(theDataContext, theOneToDelete.tbl, theOneToDelete.tblRow, true, false);
            }
            return false; // no more work to do for a while.

        }
    }
}