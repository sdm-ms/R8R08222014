using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Nonmodel_Code;
using System.Data.Linq;
using System.Linq.Expressions;
using ClassLibrary1.EFModel;
using Effort;
using ClassLibrary1.Business_Logic;

namespace ClassLibrary1.Model
{

    public static class GetIR8RDataContext
    {
        //private static R8RDataContext _UnderlyingSQLDataContextForInMemoryContext = 
        //    new R8RDataContext("no real connection"); // R8RSQLDataContext(true,true).GetRealDatabaseIfExists();

        public static bool UseRealDatabase = true; // DO NOT CHANGE THIS LINE OF CODE. This may be overriden in tests when Test_UseRealDatabase.UseReal() returns false. 

        public static Guid PersistentFakeDatabaseID = new Guid();

        public static IR8RDataContext New()
        {
            if (UseRealDatabase)
                return new R8REFDataContext();
            else
            {
                bool useEffort = false; // see effort.codeplex.com -- not currently working

                if (useEffort)
                {
                    var myTest = new R8RContext(Effort.DbConnectionFactory.CreateTransient());
                    myTest.SearchWords.Add(new SearchWord() { SearchWordID = Guid.NewGuid(), TheWord = "asdf" });
                    myTest.DatabaseStatus.Add(new DatabaseStatus() { DatabaseStatusID = Guid.NewGuid(), PreventChanges = true });
                    myTest.SaveChanges();

                    return new R8REFDataContext(Effort.DbConnectionFactory.CreatePersistent(PersistentFakeDatabaseID.ToString()));
                }
                else
                {
                    return new R8RInMemoryDataContext(InMemoryDatabaseFactory.GetDatabase(PersistentFakeDatabaseID.ToString(), new OldModel.OldDataContext()));
                }
            }
        }
    }

    public interface IR8RDataContext : IDataContextExtended
    {
        bool IsRealDatabase();

        IR8RDataContext GetRealDatabaseIfExists();

        bool ResolveConflictsIfPossible();

        IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance);
    }


    public static class R8RDataContextExtensions
    {

        public static void PreSubmittingChangesTasks(this IR8RDataContext theDataContext)
        {
            DatabaseAndAzureRoleStatus.CheckPreventChanges(theDataContext);
            FastAccessTablesMaintenance.PushRowsRequiringUpdateToAzureQueue(theDataContext);
            StatusRecords.RecordRememberedStatusRecordChanges(theDataContext);
            theDataContext.RegisteredToBeInserted = new DictionaryByType();
        }


    }

}
