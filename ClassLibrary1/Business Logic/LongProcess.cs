using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using System.Transactions;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.EFModel;

/// <summary>
/// Summary description for LongProess
/// </summary>
namespace ClassLibrary1.Model
{


    //[Serializable]
    //public class UpdateUserRatingsProcessInfo
    //{

    //    int? numberToDoEachTime { get; set; }
    //    Guid startingAtUserRatingID { get; set; }
    //    decimal? referenceUserRating { get; set; }
    //    DateTime? referenceTime { get; set; }
    //    bool updateUserRatingGroup { get; set; }


    //    public UpdateUserRatingsProcessInfo(int? numberToDo, Guid startingAtUserRatingID, decimal? referenceUserRating, DateTime? referenceTime, bool updateUserRatingGroup)
    //    {
    //        this.numberToDoEachTime = numberToDo;
    //        this.startingAtUserRatingID = startingAtUserRatingID;
    //        this.referenceUserRating = referenceUserRating;
    //        this.referenceTime = referenceTime;
    //        this.updateUserRatingGroup = updateUserRatingGroup;
    //    }

    //    public int? Continue(Guid? currentUserRatingID, Guid ratingID, Guid ratingPhaseStatusID, UserRatingUpdatingReason theResolutionType)
    //    {
    //        if (currentUserRatingID == null)
    //            currentUserRatingID = startingAtUserRatingID;
    //        R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
    //        Rating theRating = theDataAccessModule.R8RDB.GetTable<Rating>().Single(m => m.RatingID == ratingID);
    //        RatingGroupPhaseStatus ratingPhaseStatus = theDataAccessModule.R8RDB.GetTable<RatingGroupPhaseStatus>().Single(mps => mps.RatingGroupPhaseStatusID == ratingPhaseStatusID);
    //        return theDataAccessModule.UpdatePointsForUserRatings(theRating, ratingPhaseStatus, referenceUserRating, referenceTime, theResolutionType, updateUserRatingGroup);

    //    }
    //}

    

    [Serializable]
    public class UploadTblRowsInfo
    {
        Guid TblID { get; set; }
        int startingAtNumber { get; set; }
        int totalNumberToDo { get; set; }
        int numberToDoEachTime { get; set; }
        string uploadFileLocation { get; set; }
        string logFileLocation { get; set; }
        string tblRowType { get; set; }
        Guid userID { get; set; }
        bool copyValuesIntoTbl { get; set; }

        public UploadTblRowsInfo(Guid TblID, int startingAtNumber, int totalNumberToDo, int numberToDo, string uploadFileLoc, string logFileLoc, string tblRowType, Guid userID, bool copyValuesIntoTbl)
        {
            this.TblID = TblID;
            this.startingAtNumber = startingAtNumber;
            this.totalNumberToDo = totalNumberToDo;
            this.numberToDoEachTime = numberToDo;
            this.uploadFileLocation = uploadFileLoc;
            this.logFileLocation = logFileLoc;
            this.tblRowType = tblRowType;
            this.userID = userID;
            this.copyValuesIntoTbl = copyValuesIntoTbl;
        }

        public int? Continue(int? elementNumber)
        {
            R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
            if (elementNumber == null)
                elementNumber = startingAtNumber;
            ImportExport myImporter = new ImportExport(theDataAccessModule.DataContext.GetTable<Tbl>().Single(x => x.TblID == TblID));
            int lastRecord = startingAtNumber + totalNumberToDo - 1;
            int lastRecordToDo = (int)elementNumber + numberToDoEachTime - 1;
            if (lastRecordToDo > lastRecord)
                lastRecordToDo = lastRecord;
            myImporter.PerformImportHelper(uploadFileLocation, logFileLocation, tblRowType, userID, (int)elementNumber, lastRecordToDo, copyValuesIntoTbl);
            if (lastRecordToDo == lastRecord)
            {
                File.Delete(uploadFileLocation);
                return null;
            }
            else
                return lastRecordToDo + 1; // where to continue from next time
        }
    }

    public partial class R8RDataManipulation
    {


        public enum LongProcessTypes
        {
            createMissingRatings,
            uploadTblRows
            
        }

        public static int GetBasePriorityLevelForLongProcess(LongProcessTypes theProcessType)
        {
            switch (theProcessType)
            {
                case LongProcessTypes.createMissingRatings:
                    return 40;
                case LongProcessTypes.uploadTblRows:
                    return 35;
                default:
                    throw new Exception("Internal error: not a prediction updating long process.");
            }
        }

        public UserRatingUpdatingReason GetUserRatingUpdatingReasonForLongProcess(LongProcessTypes theProcessType)
        {
            switch (theProcessType)
            {
                default:
                    throw new Exception("Internal error: not a prediction updating long process.");
            }
        }

        public LongProcessTypes GetLongProcessTypeForUserRatingUpdatingReason(UserRatingUpdatingReason theReason)
        {
            switch (theReason)
            {
                default:
                    throw new Exception("Internal error: not a known prediction updating reason.");
            }
        }



        public Guid AddOrResetLongProcess(LongProcessTypes typeOfProcess, int? delayBeforeReset, Guid? object1ID, Guid? object2ID, int priority, byte[] additionalInfo)
        {

            LongProcess theLongProcess = DataContext.GetTable<LongProcess>().SingleOrDefault(
                lp => !lp.Complete 
                    && lp.TypeOfProcess == (int)typeOfProcess
                    && lp.Object1ID == object1ID
                    && lp.Object2ID == object2ID);
            if (theLongProcess == null)
            {
                // Trace.TraceInformation("Adding new long process for object1ID " + object1ID + " object2ID " + object2ID + " and type of process " + typeOfProcess.ToString());
                return AddLongProcess(typeOfProcess, delayBeforeReset, object1ID, object2ID, priority, additionalInfo);
            }
            else if (!theLongProcess.ResetWhenComplete)
            {
                // Trace.TraceInformation("Set long process to reset for objectID " + object1ID + " and type of process " + typeOfProcess.ToString());
                theLongProcess.ResetWhenComplete = true;
                DataContext.SubmitChanges();
            }
            else
            {
                // Trace.TraceInformation("Ignoring add long process for objectID for objectID " + object1ID + " and type of process " + typeOfProcess.ToString());
            }
            return theLongProcess.LongProcessID;
        }

        public Guid AddOrResetLongProcess(LongProcessTypes typeOfProcess, int? delayBeforeReset, Guid? object1ID, Guid? object2ID, int priority, UploadTblRowsInfo myUploadInfo)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream mStream = new MemoryStream())
            {
                binFormat.Serialize(mStream, myUploadInfo);
                byte[] theData = mStream.GetBuffer();

                DataContext.SubmitChanges();
                Guid returnVal = AddOrResetLongProcess(typeOfProcess, delayBeforeReset, object1ID, object2ID, priority, theData);

                return returnVal;
            }
        }

        //public int AddOrResetLongProcess(LongProcessTypes typeOfProcess, int? delayBeforeReset, Guid? object1ID, Guid? object2ID, int priority, UpdateUserRatingsProcessInfo myUpdateInfo)
        //{
        //    BinaryFormatter binFormat = new BinaryFormatter();
        //    using (MemoryStream mStream = new MemoryStream())
        //    {
        //        binFormat.Serialize(mStream, myUpdateInfo);
        //        byte[] theData = mStream.GetBuffer();
                
        //        R8RDB.SubmitChanges();
        //        int returnVal = AddOrResetLongProcess(typeOfProcess,  delayBeforeReset, object1ID, object2ID, priority, theData);


        //        BackgroundThread.Instance.EnsureBackgroundTaskIsRunning(false);

        //        return returnVal;
        //    }
        //}


        public bool ContinueLongProcess()
        {
            DateTime currentTime = TestableDateTime.Now;
            LongProcess myLongProcess = DataContext.GetTable<LongProcess>().Where(
                    lp => (!lp.Complete || lp.ResetWhenComplete)
                        && (lp.EarliestRestart == null || lp.EarliestRestart < currentTime)
                    )
                .OrderByDescending(lp => lp.Priority)
                .FirstOrDefault();
            //var test = R8RDB.GetTable<LongProcesse>().Where(
            //        lp => (!lp.Complete || lp.ResetWhenComplete)
            //            && (lp.EarliestRestart == null || lp.EarliestRestart < currentTime)
            //        ).ToList();
            //var test2 = test.Where(lp => (!lp.Complete || lp.ResetWhenComplete)
            //            && (lp.EarliestRestart == null || lp.EarliestRestart < currentTime));
            //if (test.Count != test2.Count())
            //    Trace.TraceInformation("Anomaly.");
            if (myLongProcess == null)
            {
                // Trace.TraceInformation("No long process pending.");
                return false; // no more work to do
            }
            else
            {
                try
                {
                    if (!myLongProcess.Started)
                    {
                        myLongProcess.Started = true;
                        myLongProcess.Complete = false;
                        myLongProcess.ResetWhenComplete = false;
                    }


                    byte[] byteArray = myLongProcess.AdditionalInfo.ToArray();
                    MemoryStream mStream = new MemoryStream(byteArray);
                    BinaryFormatter binFormat = new BinaryFormatter();


                    if (myLongProcess.TypeOfProcess == (int)LongProcessTypes.createMissingRatings)
                    {
                        throw new NotImplementedException(); // this has been deleted
                    }

                    if (myLongProcess.TypeOfProcess == (int)LongProcessTypes.uploadTblRows)
                    {
                        Trace.TraceInformation("About to continue upload table rows long process.");
                        UploadTblRowsInfo myUploadInfo = null;
                        myUploadInfo = (UploadTblRowsInfo)binFormat.Deserialize(mStream);
                        if (myUploadInfo != null)
                            myLongProcess.ProgressInfo = myUploadInfo.Continue(myLongProcess.ProgressInfo);
                    }
                    if (myLongProcess.ProgressInfo == null)
                    {
                        if (myLongProcess.ResetWhenComplete)
                        {
                            // Trace.TraceInformation("Completing reset of long process for " + myLongProcess.ObjectID + " and type of process " + (int)myLongProcess.TypeOfProcess);

                            myLongProcess.Started = true;
                            myLongProcess.Complete = false;
                            myLongProcess.ResetWhenComplete = false;
                            myLongProcess.ProgressInfo = null;
                            myLongProcess.Priority--; // Lower the priority one level, since we just finished this.
                            if (myLongProcess.DelayBeforeRestart != null)
                                myLongProcess.EarliestRestart = TestableDateTime.Now + new TimeSpan(0, 0, (int)myLongProcess.DelayBeforeRestart);
                        }
                        else
                        {
                            // Trace.TraceInformation("Marking as complete long process for " + myLongProcess.ObjectID  + " and type of process " + (int) myLongProcess.TypeOfProcess);
                            myLongProcess.Complete = true;
                        }
                    }
                    return true; // this might be done, but there could be other long processes.
                }
                catch
                {
                    Guid myLongProcessID = myLongProcess.LongProcessID;
                    ResetDataContexts();
                    LongProcess longProcess2 = DataContext.GetTable<LongProcess>().Single(x => x.LongProcessID == myLongProcessID);
                    longProcess2.EarliestRestart = TestableDateTime.Now + new TimeSpan(0, 10, 0);
                    DataContext.SubmitChanges();
                    throw new Exception("Delaying long process after failure.");
                }


            }
        }
    }
}
