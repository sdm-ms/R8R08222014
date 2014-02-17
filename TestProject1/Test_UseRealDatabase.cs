using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void TestMemoryLeaksSetupDEBUG()
        {
            TestHelper _testHelper;
            RaterooDataManipulation _dataManipulation;

            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // go to next day
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            _testHelper = new TestHelper();
            _dataManipulation = new RaterooDataManipulation();

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(20);
        }

        [TestMethod]
        public void TestMemoryLeaks_TemporaryAddSingleUser()
        {
            TestHelper _testHelper;
            RaterooDataManipulation _dataManipulation;

            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // go to next day
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            _testHelper = new TestHelper(false); // DEBUG -- does this make a difference? doesn't seem to (but if we rebuild, must recreat users)
            _dataManipulation = new RaterooDataManipulation();

            Helper3_DEBUG(); // this is enough to trigger memory leak below. Will it be enough if this is run separately?
        }

        [TestMethod]
        public void TestMemoryLeaks()
        {
            TestHelper _testHelper;
            RaterooDataManipulation _dataManipulation;

            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // go to next day
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            _testHelper = new TestHelper(true); // DEBUG -- does this make a difference? doesn't seem to (but if we rebuild, must recreat users)
            _dataManipulation = new RaterooDataManipulation();

            _testHelper.CreateSimpleTestTable(true); // DEBUG -- does this make a difference?
            _testHelper.CreateUsers(31); // DEBUG -- does this make a difference? 
            // TestHelper(false), no create table, no create users ==> no leak
            // TestHelper(false), no create table BUT create users ==> leak
            // DEBUG -- now that we fixed it so that it doesn't create the same username twice, creating users seems to make no difference. WHY SHOULD THAT MATTER?
            // Something to do with the fake user profiles? Creating a single user seems to make the difference, whether or not that user is used to add user ratings.
            // NO -- taking out that code doesn't seem to make a difference.
            // Creation of extra users is SUFFICIENT to create memory leaks. It's possible that rebuilding database could also be sufficient; we can't tell, because we have to create users.
            // What if we reset data contexts before creating the users (creating more than last time)? No difference
            // What if we create a random user using a separate data context? That IS enough to trigger a memory leak. So, it's nothing in our data context or custom logic,
            // except possibly in SubmitChanges. 
            // (We could try making a SEPARATE .dbml file and see if adding something to the database makes a difference.)
            // If we manually add a user to the database, that does NOT trigger the memory leak. 
            Helper3_DEBUG(); // again, this is enough to trigger memory leak.

            WeakReferenceTracker.Track = true;
            int initialRepetitions = 5;

            for (int r = 0; r < initialRepetitions; r++)
                TestMemoryLeaks_Helper(_testHelper, _dataManipulation);
            int repetitions = 2000;
            double avgMemory = 0;
            GC.Collect();
            long initMemory = GC.GetTotalMemory(false);
            for (int r = 0; r < repetitions; r++)
            {
                long newMemory = TestMemoryLeaks_Helper(_testHelper, _dataManipulation);
                avgMemory = (newMemory - initMemory) / (double)(r + 1);
            }
            avgMemory.Should().BeLessThan(1000);
        }

        private static long TestMemoryLeaks_Helper(TestHelper _testHelper, RaterooDataManipulation _dataManipulation)
        {
            UserRatingResponse theResponse = new UserRatingResponse();
            //for (int q = 0; q < 1000; q++)
            //    //Helper2();
            //    _testHelper.ActionProcessor.ChangesGroupCreate(null, null, null, null, null, null, null);
            _testHelper.ActionProcessor.UserRatingAdd(1, 5.0M, 5, ref theResponse);
            PMCacheManagement.ClearCache();
            //ZDEBUG _testHelper.FinishUserRatingAdd(_dataManipulation);
            //DEBUG _testHelper.WaitIdleTasks();
            _testHelper.ActionProcessor.DataContext.SubmitChanges();
            _testHelper.ActionProcessor.ResetDataContexts();

            GC.Collect();
            WeakReferenceTracker.CheckUncollected();
            long newMemory = GC.GetTotalMemory(false);
            return newMemory;
        }

        private static void Helper2()
        {
            RaterooDataContext myDataContext = new RaterooDataContext(AzureSetup.GetConfigurationSetting("RaterooConnectionString"));
           
            ChangesGroup theChangesGroup = new ChangesGroup();
            myDataContext.GetTable<ChangesGroup>().InsertOnSubmit(theChangesGroup);
            myDataContext.SubmitChanges();
        }

        private static void Helper3_DEBUG()
        {
            RaterooDataContext myDataContext = new RaterooDataContext("Data Source=PC2012;Initial Catalog=Rateroo7;Integrated Security=True;Connect Timeout=300"); // AzureSetup.GetConfigurationSetting("RaterooConnectionString"));

            User newUser =  new User {
                Username = "ause" + new Random((int) DateTime.Now.Ticks).Next(0, 1000000).ToString(),
                SuperUser = false,
                Status = (Byte)StatusOfObject.Active
            };
            myDataContext.GetTable<User>().InsertOnSubmit(newUser);
            myDataContext.SubmitChanges();

            myDataContext.ClearContextCache();
            System.Data.SqlClient.SqlConnection.ClearAllPools(); // DEBUG
        }


        [TestMethod]
        public void TestConnectionString()
        {
            if (!Test_UseRealDatabase.UseReal())
                return;

            string connectionString = AzureSetup.GetConfigurationSetting("RaterooConnectionString"); //  "Data Source=PC2012;Initial Catalog=Rateroo7;Integrated Security=true";

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
                Console.ReadLine();
            }
        }
    }
}
