using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using System.Data.Linq;
using System.Linq.Expressions;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{

    public static class GetIR8RDataContext
    {
        //private static R8RDataContext _UnderlyingSQLDataContextForInMemoryContext = 
        //    new R8RDataContext("no real connection"); // R8RSQLDataContext(true,true).GetRealDatabaseIfExists();

        public static bool UseRealDatabase = true; // Do not change this code. This may be overriden in tests when Test_UseRealDatabase.UseReal() returns false.

        public static IR8RDataContext New(bool doAllowChangeData, bool enableObjectTracking)
        {
            if (UseRealDatabase)
                return new R8RSQLDataContext(doAllowChangeData, enableObjectTracking);
            else
                return new R8RInMemoryDataContext();
        }
    }

    public interface IR8RDataContext : IDataContextExtended
    {
        bool IsRealDatabase();

        IR8RDataContext GetRealDatabaseIfExists();

        System.IO.TextWriter Log { get; set; }

        System.Data.Common.DbCommand GetCommand(IQueryable query);

        bool ResolveConflictsIfPossible();

        void LoadStatsWithTrustTrackersAndUserInteractions();

        void SetUserRatingAddingLoadOptions();
    }


    public static class R8RDataContextExtensions
    {

        public static void PreSubmittingChangesTasks(this IR8RDataContext theDataContext)
        {
            DatabaseAndAzureRoleStatus.CheckPreventChanges(theDataContext);
            FastAccessTablesMaintenance.PushRowsRequiringUpdateToAzureQueue(theDataContext);
            StatusRecords.RecordRememberedStatusRecordChanges(theDataContext);
            theDataContext.RegisteredToBeInserted = new List<object>();
        }


        public static void RepeatedlyAttemptToSubmitChanges(this IR8RDataContext theDataContext, System.Data.Linq.ConflictMode failureMode)
        {
            PreSubmittingChangesTasks(theDataContext);
            int numTries = 0;
        TRYSTART:
            try
            {
                numTries++;
                theDataContext.CompleteSubmitChanges(failureMode);
            }
            catch (ChangeConflictException)
            {
                if (numTries <= 3)
                {
                    bool resolvable = theDataContext.ResolveConflictsIfPossible();
                    if (resolvable)
                        goto TRYSTART;
                    else
                        throw;
                }
                else
                    throw;
            }
        }
    }

}
