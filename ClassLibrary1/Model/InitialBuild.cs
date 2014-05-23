using System;
using System.Data;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


using StringEnumSupport;

using System.IO;
using Microsoft.WindowsAzure.ServiceRuntime;

using ClassLibrary1.Misc;
using System.Diagnostics;
using ClassLibrary1.Model;
using System.Text.RegularExpressions;
using System.Threading; 
//namespace PredRatings
//{

namespace ClassLibrary1.Model
{
    public class RaterooBuilder
    {
        private RaterooDataManipulation theDataAccessModule = null;
        public RaterooDataManipulation Supporter
        {
            get
            {
                if (null == theDataAccessModule)
                {
                    theDataAccessModule = new RaterooDataManipulation();
                }

                return theDataAccessModule;
            }
        }

        DataContextManagement myDataContextManagement = null;

        public RaterooBuilder()
        {
            myDataContextManagement = new DataContextManagement();
        }


        /// <summary>
        /// Returns the DataContext used to access objects, creating it if necessary.
        /// </summary>
        private IRaterooDataContext RaterooDB
        {
            get
            {
                return myDataContextManagement.GetDataContext(true, true);
            }
        }

        public void ResetDataContexts()
        {
            myDataContextManagement.ResetMyDataContext();
        }

        [DebuggerHidden]
        internal void ExecuteCommand(string commandText)
        {
            SqlConnection myConnection = new SqlConnection();
            try
            {
                SqlCommand myCommand = new SqlCommand();
                myCommand.CommandText = commandText;
                String strConnection = AzureSetup.GetConfigurationSetting("RaterooConnectionString"); // ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
                myConnection.ConnectionString = strConnection;
                myConnection.Open();
                myCommand.Connection = myConnection;
                int numRowsAffected = myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Dispose();
            }
        }

        [DebuggerHidden]
        public void DeleteAllRecords(string tableName)
        {
            ExecuteCommand("DELETE FROM " + tableName);
            string tablenameWithoutDBO = GetTableNameWithoutDBOPrefix(tableName);
            try
            {
                ExecuteCommand("DBCC CHECKIDENT (" + tablenameWithoutDBO + ", RESEED, 0)");
            }
            catch
            { // ignore failure -- the command is unnecessary if table has no identity column
            }


        }

       internal string GetTableNameWithoutDBOPrefix(string tableName)
       {
           string pattern = @"dbo\.";
           string replacement = "";
           Regex rgx = new Regex(pattern);
           string tablenameWithoutDBO = rgx.Replace(tableName, replacement);
           return tablenameWithoutDBO;
       }

        public void ResetDatabaseAlt()
        {
            RaterooDataContext dataContext = RaterooDB.GetRealDatabaseIfExists();
            if (dataContext != null)
            {
                dataContext.ExecuteCommand("EXEC [sp_MSforeachtable] 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
                //RaterooDB.ExecuteCommand("exec [sp_MSforeachtable] 'truncate table ?'");
                //RaterooDB.ExecuteCommand("EXEC [sp_MSforeachtable] @command1='DBCC CHECKIDENT (?, RESEED, 100)'");
                //RaterooDB.ExecuteCommand("EXEC [sp_MSforeachtable] @command1='DBCC DBREINDEX('?')'");
                dataContext.ExecuteCommand("EXEC [sp_MSforeachtable] 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            }
        }

        //public void DeleteEverythingSQL()
        //{

        //    // We must delete everything in an order that will not cause table errors. So something can
        //    // be deleted only after a table with a key pointing to it is deleted.

        //    DeleteAllRecords("LongProcesses"); // start with this, so we don't get an error in a long process
        //    DeleteAllRecords("UserRatings");
        //    DeleteAllRecords("UserRatingsToAdd");
        //    DeleteAllRecords("UserRatingGroups");
        //    DeleteAllRecords("PointsAdjustments");
        //    DeleteAllRecords("PointsTotals");
        //    DeleteAllRecords("RatingConditions");
        //    DeleteAllRecords("RatingPhaseStatus");
        //    DeleteAllRecords("Ratings");
        //    DeleteAllRecords("SubsidyAdjustments");
        //    DeleteAllRecords("RatingGroupPhaseStatus");
        //    DeleteAllRecords("RatingPhases");

        //    DeleteAllRecords("RatingGroupAttributes");
        //    DeleteAllRecords("RatingCharacteristics");
        //    DeleteAllRecords("RatingPhaseGroups"); 
        //    DeleteAllRecords("VolatilityTrackers");
        //    DeleteAllRecords("RatingGroupStatusRecords");
        //    DeleteAllRecords("VolatilityTblRowTrackers");
        //    DeleteAllRecords("RatingGroups");

        //    // Note: We avoided the following by setting UPDATE AND INSERT RULE for the properties connecting TblTab and TblColumn
        //    // to Cascade Delete. We  do the same for Rating and UserRating. And also for UserRating to UserRating.
        //    // Note that we only set ONE of these rules in SQL Server Management Studio, as it must avoid multiple cascade
        //    // paths, so we set the rule governing the item that seems most clearly to create the circularity (e.g.,
        //    // the MostRecentUserRating in Rating.
        //    //// we need to avoid a circular reference problem -- so set to null all DefaultSortTblColumnID's.
        //    ////String strConnection = ConfigurationManager.ConnectionStrings["RaterooConnectionString"].ConnectionString;
        //    //ResetDataContexts();
        //    //var someTblTabs = RaterooDB.GetTable<TblTab>().Where(cg => cg.DefaultSortTblColumnID != null);
        //    //foreach (var theTblTab in someTblTabs)
        //    //    theTblTab.DefaultSortTblColumnID = null;
        //    //RaterooDB.SubmitChanges();
        //    //ResetDataContexts();

        //    DeleteAllRecords("OverrideCharacteristics");
        //    DeleteAllRecords("TblColumnFormatting");

        //    DeleteAllRecords("UserInteractionStats");
        //    DeleteAllRecords("UserInteractions");
        //    DeleteAllRecords("TrustTrackerStats");
        //    DeleteAllRecords("TrustTrackers");
        //    DeleteAllRecords("TrustTrackerUnits");

        //    DeleteAllRecords("TblColumns");
        //    DeleteAllRecords("TblTabs");

        //    DeleteAllRecords("SearchWordChoices");
        //    DeleteAllRecords("SearchWordHierarchyItems");
        //    DeleteAllRecords("SearchWordTblRowNames");
        //    DeleteAllRecords("SearchWordTextFields");
        //    DeleteAllRecords("SearchWords");

        //    DeleteAllRecords("HierarchyItems");
        //    DeleteAllRecords("NumberFields");
        //    DeleteAllRecords("TextFields");
        //    DeleteAllRecords("ChoiceInFields");
        //    DeleteAllRecords("ChoiceFields");
        //    DeleteAllRecords("AddressFields");
        //    DeleteAllRecords("DateTimeFields");
        //    DeleteAllRecords("Fields");
        //    DeleteAllRecords("ChoiceInGroups");
        //    DeleteAllRecords("ChoiceGroupFieldDefinitions");
        //    DeleteAllRecords("NumberFieldDefinitions");
        //    DeleteAllRecords("DateTimeFieldDefinitions");
        //    DeleteAllRecords("TextFieldDefinitions");
        //    DeleteAllRecords("FieldDefinitions");
        //    DeleteAllRecords("ChoiceGroups");
        //    DeleteAllRecords("Comments");
        //    DeleteAllRecords("RewardPendingPointsTrackers");
        //    DeleteAllRecords("TblRowStatusRecord");
        //    DeleteAllRecords("TblRowFieldDisplays");
        //    DeleteAllRecords("TblRows");
        //    DeleteAllRecords("ProposalSettings");
        //    DeleteAllRecords("InsertableContents");
        //    DeleteAllRecords("ChangesStatusOfObject");
        //    DeleteAllRecords("ChangesGroup");
        //    DeleteAllRecords("Tbls");
        //    DeleteAllRecords("UsersRights");
        //    DeleteAllRecords("AdministrationRights");
        //    DeleteAllRecords("AdministrationRightsGroups");
        //    DeleteAllRecords("UsersAdministrationRightsGroups");
        //    DeleteAllRecords("ProposalEvaluationRatingSettings");
        //    DeleteAllRecords("RewardRatingSettings");

        //    DeleteAllRecords("PointsManagers");
        //    DeleteAllRecords("Domains");
        //    DeleteAllRecords("TblDimensions");
        //    DeleteAllRecords("RatingPlans");
        //    DeleteAllRecords("PointsTrustRules");
        //    DeleteAllRecords("RatingPhaseGroups");
        //    DeleteAllRecords("SubsidyDensityRanges");
        //    DeleteAllRecords("SubsidyDensityRangeGroups");
        //    DeleteAllRecords("UserActions");
        //    DeleteAllRecords("UserInfo");
        //    DeleteAllRecords("Users");
        //    DeleteAllRecords("InvitedUser");
        //    DeleteAllRecords("DatabaseStatus");
        //    DeleteAllRecords("RoleStatus");


        //    ResetDataContexts();
        //}


        public void DeleteTempImportExportFiles()
        {
            RaterooBlobAccess.DeleteAllBlobs();
                //string tempFileLoc = Server.MapPath("~/ImportExportTemp");
                //string[] filePaths = Directory.GetFiles(tempFileLoc);
                //foreach (string filePath in filePaths)
                //    File.Delete(filePath);
        }

        public void CreateStandardRatingPhaseGroups()
        {
            ScoringRules standardScoringRules = ScoringRules.Quadratic;
            int singlePhaseGroupNum = Supporter.AddRatingPhaseGroup("Indefinite duration", null);
            SetStatusOfObjectInitialBuild(singlePhaseGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int singlePhase = Supporter.AddRatingPhase(singlePhaseGroupNum, 1000, standardScoringRules, false, false, null, null, 86400, true, null);
            SetStatusOfObjectInitialBuild(singlePhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int oneYearPhasesGroupNum = Supporter.AddRatingPhaseGroup("One year phases", null);
            SetStatusOfObjectInitialBuild(oneYearPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int oneYearPhase = Supporter.AddRatingPhase(oneYearPhasesGroupNum, 1000, standardScoringRules, true, false, null, 31536000, 604800, true, null);
            SetStatusOfObjectInitialBuild(oneYearPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int thirtyDayPhasesGroupNum = Supporter.AddRatingPhaseGroup("Thirty day phases", null);
            SetStatusOfObjectInitialBuild(thirtyDayPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int thirtyDayPhase = Supporter.AddRatingPhase(thirtyDayPhasesGroupNum, 1000, standardScoringRules, true, false, null, 2592000, 604800, true, null);
            SetStatusOfObjectInitialBuild(thirtyDayPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int oneWeekPhasesGroupNum = Supporter.AddRatingPhaseGroup("One week phases", null);
            SetStatusOfObjectInitialBuild(oneWeekPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int oneWeekPhase = Supporter.AddRatingPhase(oneWeekPhasesGroupNum, 1000, standardScoringRules, true, false, null, 604800, 86400, true, null);
            SetStatusOfObjectInitialBuild(oneWeekPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int oneDayPhasesGroupNum = Supporter.AddRatingPhaseGroup("One day phases", null);
            SetStatusOfObjectInitialBuild(oneDayPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int oneDayPhase = Supporter.AddRatingPhase(oneDayPhasesGroupNum, 1000, standardScoringRules, true, false, null, 86400, 86400, true, null);
            SetStatusOfObjectInitialBuild(oneDayPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int oneHourPhasesGroupNum = Supporter.AddRatingPhaseGroup("One hour phases", null);
            SetStatusOfObjectInitialBuild(oneHourPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int oneHourPhase = Supporter.AddRatingPhase(oneHourPhasesGroupNum, 1000, standardScoringRules, true, false, null, 3600, 3600, true, null);
            SetStatusOfObjectInitialBuild(oneHourPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int fifteenMinutePhasesGroupNum = Supporter.AddRatingPhaseGroup("Fifteen minute phases", null);
            SetStatusOfObjectInitialBuild(fifteenMinutePhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int fifteenMinutePhase = Supporter.AddRatingPhase(fifteenMinutePhasesGroupNum, 1000, standardScoringRules, true, false, null, 900, 900, true, null);
            SetStatusOfObjectInitialBuild(fifteenMinutePhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int oneMinutePhasesGroupNum = Supporter.AddRatingPhaseGroup("One minute phases", null);
            SetStatusOfObjectInitialBuild(oneMinutePhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int oneMinutePhase = Supporter.AddRatingPhase(oneMinutePhasesGroupNum, 1000, standardScoringRules, true, false, null, 60, 60, true, null);
            SetStatusOfObjectInitialBuild(oneMinutePhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int tenSecondPhasesGroupNum = Supporter.AddRatingPhaseGroup("Ten second phases", null);
            SetStatusOfObjectInitialBuild(tenSecondPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int tenSecondPhase = Supporter.AddRatingPhase(tenSecondPhasesGroupNum, 1000, standardScoringRules, true, false, null, 10, 10, true, null);
            SetStatusOfObjectInitialBuild(tenSecondPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int oneSecondPhasesGroupNum = Supporter.AddRatingPhaseGroup("One second phases", null);
            SetStatusOfObjectInitialBuild(oneSecondPhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int oneSecondPhase = Supporter.AddRatingPhase(oneSecondPhasesGroupNum, 1000, standardScoringRules, true, false, null, 1, 1, true, null);
            SetStatusOfObjectInitialBuild(oneSecondPhase, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int multiplePhasesGroupNum = Supporter.AddRatingPhaseGroup("Short initial phases, followed by one week phases", null);
            SetStatusOfObjectInitialBuild(multiplePhasesGroupNum, TypeOfObject.RatingPhaseGroup, StatusOfObject.Active);
            int phase1 = Supporter.AddRatingPhase(multiplePhasesGroupNum, 50, standardScoringRules, true, false, null, 3600, 3600, false, 3);
            SetStatusOfObjectInitialBuild(phase1, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int phase2 = Supporter.AddRatingPhase(multiplePhasesGroupNum, 100, standardScoringRules, true, false, null, 86400, 86400, false, 1);
            SetStatusOfObjectInitialBuild(phase2, TypeOfObject.RatingPhase, StatusOfObject.Active);
            int phase3 = Supporter.AddRatingPhase(multiplePhasesGroupNum, 1000, standardScoringRules, true, false, null, 604800, 86400, true, null);
            SetStatusOfObjectInitialBuild(phase3, TypeOfObject.RatingPhase, StatusOfObject.Active);

        }

        public void CreateStandardSubsidyDensityRangeGroups()
        {
            int testGroupNum = Supporter.AddSubsidyDensityRangeGroup("Extra subsidy near middle of spectrum", null);
            SetStatusOfObjectInitialBuild(testGroupNum, TypeOfObject.SubsidyDensityRangeGroup, StatusOfObject.Active);
            int rangeNum = Supporter.AddSubsidyDensityRange(testGroupNum, (decimal)0.3, (decimal)0.7, (decimal)1.5, true);
            SetStatusOfObjectInitialBuild(rangeNum, TypeOfObject.SubsidyDensityRange, StatusOfObject.Active);
            rangeNum = Supporter.AddSubsidyDensityRange(testGroupNum, (decimal)0.4, (decimal)0.6, (decimal)2.0, true);
            SetStatusOfObjectInitialBuild(rangeNum, TypeOfObject.SubsidyDensityRange, StatusOfObject.Active);
            testGroupNum = Supporter.AddSubsidyDensityRangeGroup("Extra subsidy near ends of spectrum", null);
            SetStatusOfObjectInitialBuild(testGroupNum, TypeOfObject.SubsidyDensityRangeGroup, StatusOfObject.Active);
            rangeNum = Supporter.AddSubsidyDensityRange(testGroupNum, (decimal)0.0, (decimal)0.1, (decimal)2.0, true);
            SetStatusOfObjectInitialBuild(rangeNum, TypeOfObject.SubsidyDensityRange, StatusOfObject.Active);
            rangeNum = Supporter.AddSubsidyDensityRange(testGroupNum, (decimal)0.9, (decimal)1.0, (decimal)2.0, true);
            SetStatusOfObjectInitialBuild(rangeNum, TypeOfObject.SubsidyDensityRange, StatusOfObject.Active);

        }

        // NOTE: This seems not to be used any more. Maybe it can be deleted? 4/3/14
        public void CreateStandardDefaultRatingCharacteristics()
        {
            CreateStandardRatingPhaseGroups();
            RatingPhaseGroup theRatingPhaseGroup = RaterooDB.GetTable<RatingPhaseGroup>().Single(g => g.Name == "Thirty day phases");
            CreateStandardSubsidyDensityRangeGroups();
            int? theSubsidyDensityRangeGroupID = null;
            SubsidyDensityRangeGroup theSubsidyDensityRangeGroup = null;
            // Use the following to use varied subsidy density: 
            //RaterooDB.GetTable<SubsidyDensityRangeGroup>().Single(g => g.Name == "Extra subsidy near middle of spectrum");
            if (theSubsidyDensityRangeGroup != null)
                theSubsidyDensityRangeGroupID = theSubsidyDensityRangeGroup.SubsidyDensityRangeGroupID;
            int defaultRatingCharacteristicsID = Supporter.AddRatingCharacteristics(theRatingPhaseGroup.RatingPhaseGroupID, theSubsidyDensityRangeGroupID, 0, 100, 1, "Thirty-day phase 0 to 100 rating", null);
            SetStatusOfObjectInitialBuild(defaultRatingCharacteristicsID, TypeOfObject.RatingCharacteristics, StatusOfObject.Active);
            theRatingPhaseGroup = RaterooDB.GetTable<RatingPhaseGroup>().Single(g => g.Name == "One minute phases");
            defaultRatingCharacteristicsID = Supporter.AddRatingCharacteristics(theRatingPhaseGroup.RatingPhaseGroupID, theSubsidyDensityRangeGroupID, 0, 100, 1, "One-minute phase 0 to 100 rating", null);
            SetStatusOfObjectInitialBuild(defaultRatingCharacteristicsID, TypeOfObject.RatingCharacteristics, StatusOfObject.Active);
            theRatingPhaseGroup = RaterooDB.GetTable<RatingPhaseGroup>().Single(g => g.Name == "One hour phases");
            defaultRatingCharacteristicsID = Supporter.AddRatingCharacteristics(theRatingPhaseGroup.RatingPhaseGroupID, theSubsidyDensityRangeGroupID, 0, 100, 1, "One-hour phase 0 to 100 rating", null);
            SetStatusOfObjectInitialBuild(defaultRatingCharacteristicsID, TypeOfObject.RatingCharacteristics, StatusOfObject.Active);
            theRatingPhaseGroup = RaterooDB.GetTable<RatingPhaseGroup>().Single(g => g.Name == "One day phases");
            defaultRatingCharacteristicsID = Supporter.AddRatingCharacteristics(theRatingPhaseGroup.RatingPhaseGroupID, theSubsidyDensityRangeGroupID, 0, 100, 1, "One-day phase 0 to 100 rating", null);
            SetStatusOfObjectInitialBuild(defaultRatingCharacteristicsID, TypeOfObject.RatingCharacteristics, StatusOfObject.Active);
            theRatingPhaseGroup = RaterooDB.GetTable<RatingPhaseGroup>().Single(g => g.Name == "Indefinite duration");
            defaultRatingCharacteristicsID = Supporter.AddRatingCharacteristics(theRatingPhaseGroup.RatingPhaseGroupID, theSubsidyDensityRangeGroupID, 0, 100, 1, "Indefinite duration 0 to 100 rating", null);
            SetStatusOfObjectInitialBuild(defaultRatingCharacteristicsID, TypeOfObject.RatingCharacteristics, StatusOfObject.Active);
            theRatingPhaseGroup = RaterooDB.GetTable<RatingPhaseGroup>().Single(g => g.Name == "Short initial phases, followed by one week phases");
            defaultRatingCharacteristicsID = Supporter.AddRatingCharacteristics(theRatingPhaseGroup.RatingPhaseGroupID, theSubsidyDensityRangeGroupID, 0, 100, 1, "Default characteristics", null);
            SetStatusOfObjectInitialBuild(defaultRatingCharacteristicsID, TypeOfObject.RatingCharacteristics, StatusOfObject.Active);
        }

        public void CreateEstablishedUsers()
        {
            int theUserID1 = Supporter.AddUserReturnId("zeroaccount", true, "adfadsf@x.com", "dsfgsdfgsdfg", false); // this is just to make sure that legitimate accounts do not have userid 0
            int theUserID2 = Supporter.AddUserReturnId("admin", true, "mbabramo@yahoo.com", "finch2127248474", false);
            // theUserID = Supporter.AddUser("basic", false, "Anonymous", "User", "", "No address", "", "", "", "", "basic", "", "", "", "");
            //SetStatusOfObjectInitialBuild(theUserID, TypeOfObject.User, StatusOfObject.Active);

            // Supporter.SetUserVerificationStatus(theUserID, true);
        }

        public void CreateCssTemplates()
        {

            int theTblDimensionId1 = Supporter.AddTblDimensions(10, 150, 250, 100, 175, 275); /* note: last number is currently ignored; we would need to put it in the html and have it read by jquery */


        }
        

        public void CreateStandardStuff()
        {
            Supporter.AddDatabaseStatus();
            CreateEstablishedUsers();
            CreateCssTemplates();
        }


        public void DeleteAndRebuild()
        {
            DeleteDatabase();
            ResetDataContexts();
            CreateStandardStuff();
            ResetDataContexts();
        }

        List<MetaTable> successfulOrder = new List<MetaTable>();

        public void DeleteAllTablesSql(IRaterooDataContext dataContext)
        {
            if (!dataContext.IsRealDatabase())
                return;

            int successfulCount = successfulOrder.Count();
            if (successfulCount != 0) // we've done this successfully before, so let's try in the same order.
            {
                ExecuteCommand("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");
                List<MetaTable> successfulOrderCopy = successfulOrder;
                successfulOrder = new List<MetaTable>();
                DeleteAllTablesSQLHelper(successfulOrderCopy);
                ExecuteCommand("exec sp_msforeachtable @command1=\"print '?'\", @command2=\"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
                if (successfulOrder.Count() == successfulCount) // it worked -- otherwise we'll keep going with tables that didn't work
                    return;
            }

            List<MetaTable> theTables = dataContext.GetRealDatabaseIfExists().Mapping.GetTables().ToList().Where(x => !x.TableName.Contains("aspnet") && !x.TableName.Contains("Forum") && !successfulOrder.Any(y => y.TableName == x.TableName)).ToList();
            while (theTables.Any())
            {
                ExecuteCommand("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");
                DeleteAllTablesSQLHelper(theTables);
                ExecuteCommand("exec sp_msforeachtable @command1=\"print '?'\", @command2=\"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
            }
        }

        [DebuggerHidden]
        public void DeleteAllTablesSQLHelper(List<MetaTable> remainingTables)
        {
            List<MetaTable> tablesToRemove = new List<MetaTable>();
            foreach (var tbl in remainingTables)
            {
                try
                {
                    DeleteAllRecords(tbl.TableName);
                    tablesToRemove.Add(tbl);
                    successfulOrder.Add(tbl);
                    Trace.WriteLine("Successfully deleted all data from " + tbl.TableName);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Not yet able to delete all data from " + tbl.TableName + " " + ex.Message);
                }
            }
            remainingTables.RemoveAll(x => tablesToRemove.Contains(x));
        }

        public void DeleteDatabase()
        {
            //int numTries = 0;

            RaterooDataManipulation manipulator = new RaterooDataManipulation();
            if (manipulator.DataContext.IsRealDatabase())
            {
                //manipulator.DropFastAccessTables();
                DeleteAllTablesSql(manipulator.DataContext);

                //List<IUserProfileInfo> profiles = UserProfileCollection.GetAllUsers();
                //foreach (var profile in profiles)
                //    profile.DeleteUser(true);

                //UserProfileCollection.DeleteInactiveProfiles();

                //if (RoleEnvironment.IsAvailable)
                //    DeleteTempImportExportFiles();

                manipulator.ResetDataContexts();
            }
            else
                SimulatedPermanentStorage.Reset();

            UserProfileCollection.DeleteAllUsers();

            // clear the cache
            if (HttpContext.Current != null)
                foreach (DictionaryEntry x in HttpContext.Current.Cache)
                    HttpContext.Current.Cache.Remove((string)x.Key);

        }

        public void SetStatusOfObjectInitialBuild(int objectID, TypeOfObject theObjectType, StatusOfObject theStatus)
        {

            Byte newValue = (Byte)theStatus;
            switch (theObjectType)
            {
                case TypeOfObject.AddressField:
                    RaterooDB.GetTable<AddressField>().Single(x => x.AddressFieldID == objectID).Status = newValue;
                    break;
                case TypeOfObject.TblColumn:
                    TblColumn theTblColumn = RaterooDB.GetTable<TblColumn>().Single(x => x.TblColumnID == objectID);
                    //if (theStatus == StatusOfObject.Active && theTblColumn.Status != (Byte)StatusOfObject.Active)
                    //{
                    //    bool keepLooking = true;

                    //    do
                    //    {
                    //        TblColumn match = RaterooDB.GetTable<TblColumn>().SingleOrDefault(cd => cd.Status == (Byte)StatusOfObject.Active
                    //                                       && cd.TblTabID == theTblColumn.TblTabID
                    //                                       && cd.CategoryNum == theTblColumn.CategoryNum);
                    //        if (match == null)
                    //            keepLooking = false;
                    //        else
                    //            theTblColumn.CategoryNum++;
                    //    } while (keepLooking);

                    //}
                    theTblColumn.Status = newValue;
                    break;
                case TypeOfObject.TblTab:
                    TblTab theTblTab = RaterooDB.GetTable<TblTab>().Single(x => x.TblTabID == objectID);
                    if (theStatus == StatusOfObject.Active && theTblTab.Status != (Byte)StatusOfObject.Active)
                        //{
                        //    bool keepLooking = true;

                        //    do
                        //    {
                        //        TblTab match = RaterooDB.GetTable<TblTab>().SingleOrDefault(cg => cg.Status == (Byte)StatusOfObject.Active
                        //                                       && cg.TblID == theTblTab.TblID
                        //                                       && cg.NumInTbl == theTblTab.NumInTbl);
                        //        if (match == null)
                        //            keepLooking = false;
                        //        else
                        //            theTblTab.NumInTbl++;
                        //    } while (keepLooking);

                        //}
                        theTblTab.Status = newValue;
                    break;
                case TypeOfObject.ChoiceField:
                    RaterooDB.GetTable<ChoiceField>().Single(x => x.ChoiceFieldID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ChoiceGroup:
                    RaterooDB.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ChoiceGroupFieldDefinition:
                    RaterooDB.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.ChoiceGroupFieldDefinitionID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ChoiceInField:
                    RaterooDB.GetTable<ChoiceInField>().Single(x => x.ChoiceInFieldID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ChoiceInGroup:
                    RaterooDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceInGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.Tbl:
                    RaterooDB.GetTable<Tbl>().Single(x => x.TblID == objectID).Status = newValue;
                    break;
                case TypeOfObject.DateTimeField:
                    RaterooDB.GetTable<DateTimeField>().Single(x => x.DateTimeFieldID == objectID).Status = newValue;
                    break;
                case TypeOfObject.DateTimeFieldDefinition:
                    RaterooDB.GetTable<DateTimeFieldDefinition>().Single(x => x.DateTimeFieldDefinitionID == objectID).Status = newValue;
                    break;
                case TypeOfObject.Domain:
                    RaterooDB.GetTable<Domain>().Single(x => x.DomainID == objectID).Status = newValue;
                    break;
                case TypeOfObject.TblRow:
                    TblRow tblRow = RaterooDB.GetTable<TblRow>().Single(x => x.TblRowID == objectID);
                    var oldValue = tblRow.Status;
                    tblRow.Status = newValue;
                    Tbl tbl = tblRow.Tbl;
                    if (oldValue != newValue)
                    {
                        if (newValue == (byte)StatusOfObject.Active)
                        {
                            tbl.NumTblRowsActive++;
                            tbl.NumTblRowsDeleted--;
                        }
                        else
                        {
                            tbl.NumTblRowsDeleted++;
                            tbl.NumTblRowsActive--;
                        }
                    }
                    break;
                case TypeOfObject.Field:
                    RaterooDB.GetTable<Field>().Single(x => x.FieldID == objectID).Status = newValue;
                    //Field theField = RaterooDB.GetTable<Field>().Single(x => x.FieldID == objectID);
                    //if (theStatus == StatusOfObject.Active && theField.Status != (Byte)StatusOfObject.Active)
                    //{
                    //    Field match = RaterooDB.GetTable<Field>().SingleOrDefault(f => f.Status == (Byte)StatusOfObject.Active
                    //                                                    && f.FieldDefinitionID == theField.FieldDefinitionID
                    //                                                    && f.TblRowID == theField.TblRowID);
                    //    if (match != null)
                    //        Supporter.DeleteField(match);
                    //}
                    //theField.Status = newValue;
                    break;
                case TypeOfObject.FieldDefinition:
                    FieldDefinition theFieldDefinition = RaterooDB.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == objectID);
                    //if (theStatus == StatusOfObject.Active && theFieldDefinition.Status != (Byte)StatusOfObject.Active)
                    //{
                    //    bool keepLooking = true;

                    //    do
                    //    {
                    //        FieldDefinition match = RaterooDB.GetTable<FieldDefinition>().SingleOrDefault(fd => fd.Status == (Byte)StatusOfObject.Active
                    //                                        && fd.TblID == theFieldDefinition.TblID
                    //                                        && fd.FieldNum == theFieldDefinition.FieldNum);
                    //        if (match == null)
                    //            keepLooking = false;
                    //        else
                    //            theFieldDefinition.FieldNum++;
                    //    } while (keepLooking);
                    //}
                    theFieldDefinition.Status = newValue;
                    break;
                case TypeOfObject.InsertableContent:
                    RaterooDB.GetTable<InsertableContent>().Single(x => x.InsertableContentID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    RaterooDB.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingCondition:
                    RaterooDB.GetTable<RatingCondition>().Single(x => x.RatingConditionID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingGroup:
                    RaterooDB.GetTable<RatingGroup>().Single(x => x.RatingGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    RaterooDB.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingPhase:
                    RaterooDB.GetTable<RatingPhase>().Single(x => x.RatingPhaseID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    RaterooDB.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingPlan:
                    RaterooDB.GetTable<RatingPlan>().Single(x => x.RatingPlansID == objectID).Status = newValue;
                    break;
                case TypeOfObject.NumberField:
                    RaterooDB.GetTable<NumberField>().Single(x => x.NumberFieldID == objectID).Status = newValue;
                    break;
                case TypeOfObject.NumberFieldDefinition:
                    RaterooDB.GetTable<NumberFieldDefinition>().Single(x => x.NumberFieldDefinitionID == objectID).Status = newValue;
                    break;
                case TypeOfObject.OverrideCharacteristics:
                    RaterooDB.GetTable<OverrideCharacteristic>().Single(x => x.OverrideCharacteristicsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.PointsAdjustment:
                    RaterooDB.GetTable<PointsAdjustment>().Single(x => x.PointsAdjustmentID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ProposalSettings:
                    ProposalSetting theSettings = RaterooDB.GetTable<ProposalSetting>().Single(x => x.ProposalSettingsID == objectID);
                    ProposalSetting oldSettings = RaterooDB.GetTable<ProposalSetting>().SingleOrDefault(ps => ps.Status == (Byte)StatusOfObject.Active
                                                                                          && ps.PointsManagerID == theSettings.PointsManagerID
                                                                                          && ps.TblID == theSettings.TblID);
                    if (oldSettings != null)
                        RaterooDB.GetTable<ProposalSetting>().DeleteOnSubmit(oldSettings);
                    RaterooDB.GetTable<ProposalSetting>().Single(x => x.ProposalSettingsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.SubsidyAdjustment:
                    RaterooDB.GetTable<SubsidyAdjustment>().Single(x => x.SubsidyAdjustmentID == objectID).Status = newValue;
                    break;
                case TypeOfObject.SubsidyDensityRange:
                    RaterooDB.GetTable<SubsidyDensityRange>().Single(x => x.SubsidyDensityRangeID == objectID).Status = newValue;
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    RaterooDB.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.TextField:
                    RaterooDB.GetTable<TextField>().Single(x => x.TextFieldID == objectID).Status = newValue; break;

                case TypeOfObject.TextFieldDefinition:
                    RaterooDB.GetTable<TextFieldDefinition>().Single(x => x.TextFieldDefinitionID == objectID).Status = newValue;
                    break;
                case TypeOfObject.PointsManager:
                    RaterooDB.GetTable<PointsManager>().Single(x => x.PointsManagerID == objectID).Status = newValue;
                    break;
                case TypeOfObject.User:
                    RaterooDB.GetTable<User>().Single(x => x.UserID == objectID).Status = newValue;
                    break;
                case TypeOfObject.AdministrationRightsGroup:
                    RaterooDB.GetTable<AdministrationRightsGroup>().Single(x => x.AdministrationRightsGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.AdministrationRight:
                    RaterooDB.GetTable<AdministrationRight>().Single(x => x.AdministrationRightID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RewardRatingSettings:
                    RaterooDB.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ProposalEvaluationRatingSettings:
                    RaterooDB.GetTable<ProposalEvaluationRatingSetting>().Single(x => x.ProposalEvaluationRatingSettingsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.UsersAdministrationRightsGroup:
                    RaterooDB.GetTable<UsersAdministrationRightsGroup>().Single(x => x.UsersAdministrationRightsGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.UsersActions:
                    break;
                case TypeOfObject.UsersRights:
                    UsersRight theRights = RaterooDB.GetTable<UsersRight>().Single(x => x.UsersRightsID == objectID);
                    UsersRight oldRights = RaterooDB.GetTable<UsersRight>().SingleOrDefault(ur => ur.Status == (Byte)StatusOfObject.Active
                                                                              && ur.PointsManagerID == theRights.PointsManagerID
                                                                              && ur.UserID == theRights.UserID);
                    if (oldRights != null)
                        RaterooDB.GetTable<UsersRight>().DeleteOnSubmit(oldRights);
                    theRights.Status = newValue;
                    break;
                default:
                    throw new Exception("Internal error -- trying to activate or deactivate unknown object type");
            }

            RaterooDB.SubmitChanges();
        }

        public void CreateStandard()
        {
            RaterooDataAccess.AllowNullOrUserID0UserForTestingAndInitialBuild = true;
            BackgroundThread.BriefPauseRequestNumberSeconds = 30000;
            BackgroundThread.BriefPauseRequested = true;
            Thread.Sleep(5000);

            
            /*Government government = new Government();
            government.Create();

            Baseball baseball = new Baseball();
            baseball.Create();

            Entertainment entertainment = new Entertainment();
            entertainment.Create();

            Hockey hockey = new Hockey();
            hockey.Create();

            RealEstate realEstate = new RealEstate();
            realEstate.Create(); */

            // We've amended the Restaurants table so that it has various types of columns and fields.
            Restaurants restaurants = new Restaurants();
            restaurants.Create();

            /*  
            Blogs blogs = new Blogs();
            blogs.Create();

            
            News news = new News();
            news.Create();

            PrivatePages privatePages = new PrivatePages();
            privatePages.Create();
            */

            BackgroundThread.BriefPauseRequestNumberSeconds = 0;
        }

    }
    //}
}