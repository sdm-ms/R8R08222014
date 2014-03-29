using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1.Misc
{
    public enum SQLColumnType
    {
        typeInt,
        typeString,
        typeDecimal,
        typeGeography,
        typeDateTime,
        typeBit
    }

    public class SQLTableColumnDescription
    {
        public string Name { get; set; }
        public SQLColumnType ColType { get; set; }
        public bool Nullable { get; set; }
        public bool PrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public bool NonclusteredIndex { get; set; }
        public bool Ascending { get; set; }

        public string ColTypeString()
        {
            switch (ColType)
            {
                case SQLColumnType.typeBit:
                    return "[bit]";
                case SQLColumnType.typeDateTime:
                    return "[datetime]";
                case SQLColumnType.typeDecimal:
                    return "[decimal](18,4)";
                case SQLColumnType.typeGeography:
                    return "[geography]";
                case SQLColumnType.typeInt:
                    return "[int]";
                case SQLColumnType.typeString:
                    if (NonclusteredIndex)
                        return "[nvarchar](200)"; // MAX can't be used on indexed columns, so strings must be truncated to fit this.
                    else
                        return "[nvarchar](max)";
                default:
                    throw new Exception("Unknown ColType");
            }
        }

        public Type GetColumnType()
        {
            switch (ColType)
            {
                case SQLColumnType.typeBit:
                    return System.Type.GetType("System.Boolean");
                case SQLColumnType.typeDateTime:
                    return System.Type.GetType("System.DateTime");
                case SQLColumnType.typeDecimal:
                    return System.Type.GetType("System.Decimal");
                case SQLColumnType.typeGeography:
                    return typeof(Microsoft.SqlServer.Types.SqlGeography);
                case SQLColumnType.typeInt:
                    return System.Type.GetType("System.Int32");
                case SQLColumnType.typeString:
                    return System.Type.GetType("System.String");
                default:
                    throw new Exception("Unknown ColType");
            }
        }

        internal string ToSpecificationString()
        {
            return String.Format("[{0}] {1} {2} {3}", Name, ColTypeString(), Nullable ? "NULL" : "NOT NULL", AutoIncrement ? "IDENTITY" : "");
        }

        public DataColumn GetDataColumn()
        {
            DataColumn column = new DataColumn();
            column.DataType = GetColumnType();
            column.ColumnName = Name;
            return column;
        }
    }

    public class SQLTableDescription
    {
        public string Name { get; set; }
        public List<SQLTableColumnDescription> Columns { get; set; }

        public List<DataColumn> GetDataColumns(bool includeIDColumn = true)
        {
            return Columns.Select(x => x.GetDataColumn()).Where(x => includeIDColumn || x.ColumnName != "ID").ToList();
        }
    }

    public interface ISQLDirectConnectionManager
    {
        string GetConnectionString();
    }

    public class SQLParameterInfo
    {
        public string paramname { get; set; }
        public object value { get; set; }
        public SqlDbType dbtype { get; set; }
    }

    public class SQLUpdateInfo
    {
        public string tableName;
        public string nameOfColumnToMatch;
        public string valueOfColumnToMatch;
        public string nameOfColumnToSet;
        public string valueOfColumnToSet;
    }

    public static class SQLDirectManipulate
    {
        internal static void ExecuteSQLNonQuery(ISQLDirectConnectionManager database, string command, List<SQLParameterInfo> parameters = null)
        {
            using (SqlConnection connection =
               new SqlConnection(database.GetConnectionString()))
            {
                // Create the Command and Parameter objects.
                SqlCommand sqlCommand = new SqlCommand(command, connection);
                if (parameters != null)
                {
                    foreach (SQLParameterInfo p in parameters)
                    {
                        string fullname = "@" + p.paramname;
                        sqlCommand.Parameters.Add(fullname, p.dbtype);
                        sqlCommand.Parameters[fullname].Value = p.value;
                        if (p.dbtype == SqlDbType.Udt)
                            sqlCommand.Parameters[fullname].UdtTypeName = "Geography";
                    }
                }
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }

        internal static object ExecuteSQLScalar(ISQLDirectConnectionManager database, string command, List<SQLParameterInfo> parameters = null)
        {
            using (SqlConnection connection =
               new SqlConnection(database.GetConnectionString()))
            {
                // Create the Command and Parameter objects.
                SqlCommand sqlCommand = new SqlCommand(command, connection);
                if (parameters != null)
                {
                    foreach (SQLParameterInfo p in parameters)
                    {
                        string fullname = "@" + p.paramname;
                        sqlCommand.Parameters.Add(fullname, p.dbtype);
                        sqlCommand.Parameters[fullname].Value = p.value;
                    }
                }
                sqlCommand.Connection.Open();
                return sqlCommand.ExecuteScalar();
            }
        }

        public static void DropTable(ISQLDirectConnectionManager database, string tableName)
        {
            string dropCommand = String.Format("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U')) \n DROP TABLE [dbo].[{0}] \n ", tableName);
            ExecuteSQLNonQuery(database, dropCommand);
        }

        public static void AddTable(ISQLDirectConnectionManager database, SQLTableDescription table)
        {
            DropTable(database, table.Name);
            string colNames = "";
            foreach (var col in table.Columns)
                colNames += col.ToSpecificationString() + ", \n ";
            string primaryKey = table.Columns.Single(x => x.PrimaryKey).Name;
            string addCommand = String.Format("CREATE TABLE [dbo].[{0}]( \n {1} CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED \n ( \n [{2}] ASC \n )WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF \n ) ON [PRIMARY] \n ) ON [PRIMARY]", table.Name, colNames, primaryKey);
            ExecuteSQLNonQuery(database, addCommand);
        }

        public static void DeleteMatchingItems(ISQLDirectConnectionManager database, string tableName, string fieldName, List<string> matchingValuesQuotedIfNecessary)
        {
            var individualMatches = matchingValuesQuotedIfNecessary.Select(x => fieldName + "=" + x);
            string deleteStatement = String.Join(" OR ", individualMatches);
            string deleteCommand = String.Format("DELETE FROM [dbo].[{0}] WHERE {1};", tableName, deleteStatement);
            ExecuteSQLNonQuery(database, deleteCommand);
        }


        private static string GetUpdateCommand(string tableName, string nameOfColumnToMatch, string valueOfColumnToMatch, List<string> namesOfColumnsToSet, List<string> valuesOfColumnsToSet)
        {
            int columnsCount = namesOfColumnsToSet.Count();
            if (columnsCount != valuesOfColumnsToSet.Count())
                throw new Exception("Number of columns to set must equal number of column values to set.");
            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE ");
            sb.Append(tableName);
            sb.Append(" SET ");
            bool isFirstEntry = true;
            for (int i = columnsCount - 1; i >= 0; i--) // go backward, so that last setting of column will control
            {
                bool alreadySet = false;
                for (int j = columnsCount - 1; j > i; j--)
                { // we can't set the column twice, so skip earlier settings of the columns
                    if (namesOfColumnsToSet[j] == namesOfColumnsToSet[i])
                    {
                        alreadySet = true;
                        break;
                    }
                }
                if (alreadySet)
                    continue;
                if (!isFirstEntry)
                    sb.Append(", "); // we add commas before the entry for all but the first (easier than tracking whether it's the last entry and adding comma afterward, because of the alreadySet logic).
                else
                    isFirstEntry = false;
                sb.Append(namesOfColumnsToSet[i]);
                sb.Append("=");
                sb.Append(valuesOfColumnsToSet[i]);
            }
            sb.Append(" WHERE ");
            sb.Append(nameOfColumnToMatch);
            sb.Append("=");
            sb.Append(valueOfColumnToMatch);
            sb.Append(";");
            return sb.ToString();
        }

        private static string GetUpdateCommands(List<SQLUpdateInfo> allUpdates)
        {
            StringBuilder sb = new StringBuilder();
            var theUpdates = allUpdates.GroupBy(x => x.tableName + "/" + x.nameOfColumnToMatch + "/" + x.valueOfColumnToMatch);
            foreach (var updateGroup in theUpdates)
            {
                List<SQLUpdateInfo> allInGroup = updateGroup.ToList();
                SQLUpdateInfo firstInGroup = allInGroup.First();
                sb.Append(GetUpdateCommand(firstInGroup.tableName, firstInGroup.nameOfColumnToMatch, firstInGroup.valueOfColumnToMatch, allInGroup.Select(x => x.nameOfColumnToSet).ToList(), allInGroup.Select(x => x.valueOfColumnToSet).ToList()));
            }
            return sb.ToString();
        }

        public static void ExecuteSpecifiedUpdates(ISQLDirectConnectionManager database, List<SQLUpdateInfo> allUpdates)
        {
            if (allUpdates == null || !allUpdates.Any())
                return;
            string updateCommands = GetUpdateCommands(allUpdates);
            ExecuteSQLNonQuery(database, updateCommands);
        }

        public static void AddIndicesForSpecifiedColumns(ISQLDirectConnectionManager database, SQLTableDescription table)
        {
            foreach (var col in table.Columns.Where(x => x.NonclusteredIndex))
            {
                string cmd = "";
                if (col.ColType == SQLColumnType.typeGeography)
                {
                    cmd = String.Format(@"
CREATE SPATIAL INDEX [SIndx_{0}_{1}] ON [dbo].[{0}] 
(
	[{1}]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
", table.Name, col.Name);
                }
                else
                {
                    cmd = "CREATE NONCLUSTERED INDEX IX_" + table.Name + "_" + col.Name + " ON " + table.Name + " (" + col.Name + (col.Ascending ? " ASC)" : " DESC)");
                }
                ExecuteSQLNonQuery(database, cmd);
            }
        }

    }
}
