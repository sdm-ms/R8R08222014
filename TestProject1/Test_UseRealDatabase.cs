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

namespace TestProject1
{
    public static class Test_UseRealDatabase
    {
        public static bool UseReal()
        {
            // Use true when you want all tests to use a SQL Server database
            // Use false when you want all tests to use an in-memory database
            return false;
        }
    }

    [TestClass]
    public class TestRealDatabase
    {
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
