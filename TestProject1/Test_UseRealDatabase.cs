using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
#else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif

using FluentAssertions;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.ServiceHosting.Tools.DevelopmentStorage;
using Microsoft.ServiceHosting.Tools.DevelopmentFabric;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace TestProject1
{
    public static class Test_UseRealDatabase
    {
        public static bool UseReal()
        {
            // Use true when you want all tests to use a SQL Server database
            // Use false when you want all tests to use an in-memory database
            return true;
        }
    }

    [TestClass]
    public class TestRealDatabase
    {

        [TestMethod]
        public void ResetAndCreateStandard()
        {
            if (!Test_UseRealDatabase.UseReal())
                return;
            RaterooBuilder theBuilder = new RaterooBuilder();
            theBuilder.DeleteAndRebuild();
            theBuilder.CreateStandard();
        }

        [TestMethod]
        public void TestMemoryLeaks()
        {
            if (!Test_UseRealDatabase.UseReal())
                return; // if we're not using the real database, then memory will certainly rise since we'll be storing more objects

            // before switching to NUnit, this test was failing, with dramatic increases in memory, even though we were not getting the same result when running the same test through a console application. It's not clear why using MSTest should make a difference, particularly since we are using the same vstest execution engine with NUnit, but it appears to make a difference.

            TestHelper _testHelper;
            RaterooDataManipulation _dataManipulation;

            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // go to next day
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            CacheManagement.DisableCaching = true; 

            _testHelper = new TestHelper(true);
            _dataManipulation = new RaterooDataManipulation();
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(20); 

            // When we had a memory leak:
            // Creation of extra users is SUFFICIENT to create memory leaks. 
            // What if we reset data contexts before creating the users (creating more than last time)? No difference
            // What if we create a random user using a separate data context? That IS enough to trigger a memory leak. So, it's nothing in our data context or custom logic,
            // except possibly in SubmitChanges. 
            // If we manually add a user to the database, that does NOT trigger the memory leak. 
            AddUser(); // again, this was enough to trigger memory leak.

            // WeakReferenceTracker.Track = true; // If there is a memory leak, we can use WeakReferenceTracker to hold references to objects to see if they are disposed. It appears that a few objects are held (perhaps because of connection), but number does not go up.
            int initialRepetitions = 20;

            for (int r = 0; r < initialRepetitions; r++)
                TestMemoryLeaks_Helper(_testHelper, _dataManipulation, r == initialRepetitions - 1 || r % 5 == 0);
            int repetitions = 100;
            double avgMemory = 0;
            GC.Collect();
            long initMemory = GC.GetTotalMemory(false), lastMemory = initMemory;
            double timesMemoryWentUp = 0.0;
            double timesMemoryWentDown = 0.0;
            for (int r = 0; r < repetitions; r++)
            {
                long newMemory = TestMemoryLeaks_Helper(_testHelper, _dataManipulation, r == repetitions - 1 || r % 5 == 0);
                if (newMemory > lastMemory)
                    timesMemoryWentUp += 1.0;
                else
                    timesMemoryWentDown += 1.0;
                double memoryIncreasesProportion = (timesMemoryWentUp/(timesMemoryWentUp + timesMemoryWentDown)); // this is usually about 0.7, nothing to worry about
                avgMemory = (newMemory - initMemory) / (double)(r + 1);
            }
            avgMemory.Should().BeLessThan(10000.0); // it's hard to settle on a value here, since memory goes up and down even when using GC.Collect. With many repetitions, we can use a lower number.
        }

        private static long TestMemoryLeaks_Helper(TestHelper _testHelper, RaterooDataManipulation _dataManipulation, bool waitIdleTasks = false)
        {
            UserRatingResponse theResponse = new UserRatingResponse();
            _testHelper.ActionProcessor.UserRatingAdd(1, 5.0M, 5, ref theResponse);
            CacheManagement.ClearCache();
            _testHelper.FinishUserRatingAdd(_dataManipulation);
            if (waitIdleTasks)
                _testHelper.WaitIdleTasks();
            _testHelper.ActionProcessor.DataContext.SubmitChanges();
            _testHelper.ActionProcessor.ResetDataContexts();

            GC.Collect();
            WeakReferenceTracker.CheckUncollected();
            long newMemory = GC.GetTotalMemory(false);
            return newMemory;
        }

        private static void AddUser()
        {
            RaterooDataContext myDataContext = new RaterooDataContext(AzureSetup.GetConfigurationSetting("RaterooConnectionString"));

            User newUser =  new User {
                Username = "ause" + new Random((int) DateTime.Now.Ticks).Next(0, 1000000).ToString(),
                SuperUser = false,
                Status = (Byte)StatusOfObject.Active
            };
            myDataContext.GetTable<User>().InsertOnSubmit(newUser);
            myDataContext.SubmitChanges();
            myDataContext.ClearContextCache();
        }


        [TestMethod]
        public void TestConnectionString()
        {
            if (!Test_UseRealDatabase.UseReal())
                return;

            string connectionString = AzureSetup.GetConfigurationSetting("RaterooConnectionString"); //  "Data Source=PC2012;Initial Catalog=Rateroo10;Integrated Security=true";

            // Provide the query string with a parameter placeholder. 
            string queryString =
                "SELECT TblTabWord from dbo.Tbls "
                    + "WHERE Status = @myParam;";

            // Specify the parameter value. 
            int paramValue = 1;

            // Create and open the connection in a using block. This 
            // ensures that all resources will be closed and disposed 
            // when the code exits. 
            using (System.Data.SqlClient.SqlConnection connection =
                new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@myParam", paramValue);

                // Open the connection in a try/catch block.  
                // Create and execute the DataReader, writing the result 
                // set to the console window. 
                try
                {
                    connection.Open();
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}",
                            reader[0]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
