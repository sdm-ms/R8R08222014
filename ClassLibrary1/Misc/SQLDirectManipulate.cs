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

    [Serializable]
    public class SQLParameterInfo
    {
        public string fieldname { get; set; }
        public int? rownum { get; set; }
        public string tablename { get; set; }
        public string paramname { get { if (rownum == null) return fieldname; if (tablename == null) return fieldname + rownum.ToString(); return fieldname + rownum.ToString() + "X" + tablename; } }
        public object value { get; set; }
        public SqlDbType dbtype { get; set; }
    }

    public static class SQLParameterInfoDistinct
    {
        public static void EliminateOverridenUpdates(this List<SQLParameterInfo> updates)
        {
            updates.Reverse(); // we reverse so that the last one will be included, but earlier ones won't
            Dictionary<string, bool> itemsProcessed = new Dictionary<string, bool>();
            List<int> itemsToRemove = new List<int>();
            int totalUpdates = updates.Count();
            for (int itemNum = 0; itemNum < totalUpdates; itemNum++)
            {
                SQLParameterInfo update = updates[itemNum];
                if (!itemsProcessed.ContainsKey(update.paramname))
                    itemsProcessed.Add(update.paramname, true);
                else
                    itemsToRemove.Add(itemNum);
            }
            updates.Reverse(); // reverse again
            // because we've reversed again, the itemsToRemove refer to items near the back of the list first, so we remove those first (thus not affecting the formula for removing items earlier in the list). The index of each item has to be reversed.
            foreach (int itemToRemove in itemsToRemove)
                updates.RemoveAt(totalUpdates - 1 - itemToRemove); 
        }
    }

    // For geography objects, place a SQLGeographyInfo in value of SQLParameterInfo
    [Serializable]
    public class SQLGeographyInfo
    {
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }

    public class SQLUpdateRowInfo
    {
        public List<SQLParameterInfo> Parameters { get; set; }
        int Rownum;
        string Tablename;

        public SQLUpdateRowInfo(int rownum, string tablename)
        {
            Rownum = rownum;
            Tablename = tablename;
            Parameters = new List<SQLParameterInfo>();
        }

        public void Add(string varname, object value, SqlDbType dbtype)
        {
            Parameters.Add(new SQLParameterInfo { fieldname = varname, rownum = Rownum, tablename = Tablename, value = value, dbtype = dbtype });
        }

        public void GetUpdateCommand(string tableName, out string updateCommand, out List<SQLParameterInfo> parameters)
        {
            // step 1: formulate the SQL statement
            if (!Parameters.Any())
            {
                updateCommand = null;
                parameters = null;
                return;
            }

            Parameters.EliminateOverridenUpdates();

            List<string> updateStringComponents = new List<string>();

            updateStringComponents.AddRange(new string[] { "UPDATE ", tableName, " ", "SET " });
            bool isFirst = true;
            string idparamname = "";
            foreach (var variable in Parameters)
            {
                if (variable.fieldname != "ID")
                {
                    if (!isFirst)
                        updateStringComponents.Add(", ");
                    isFirst = false;
                    updateStringComponents.Add(variable.fieldname);
                    if (variable.value == null)
                        updateStringComponents.Add(" = NULL");
                    else
                    {
                        if (variable.value is SQLGeographyInfo)
                        {
                            SQLGeographyInfo fastUpdateGeo = ((SQLGeographyInfo)variable.value);
                            // we must construct the two variable parameter names that we will get once we convert this (see below)
                            updateStringComponents.AddRange(new string[] { " = geography::STPointFromText('POINT(' + CAST(@", variable.fieldname, "LONG", variable.rownum.ToString(), " AS VARCHAR(20)) + ' ' + CAST(@", variable.fieldname, "LAT", variable.rownum.ToString(), " AS VARCHAR(20)) + ')', 4326)" });
                        }
                        else
                        {
                            updateStringComponents.AddRange(new string[] { " = @", variable.paramname });
                        }
                    }
                }
                else
                    idparamname = variable.paramname;
            }
            updateStringComponents.AddRange(new string[] { " WHERE ID = @", idparamname, "; " });
            StringBuilder sb = new StringBuilder();
            foreach (string component in updateStringComponents)
                sb.Append(component);
            updateCommand = sb.ToString();

            foreach (var p in Parameters)
                if (p.value == null)
                    p.value = DBNull.Value;

            List<SQLParameterInfo> sqlParameters = Parameters.OrderBy(x => x.fieldname == "ID").ToList(); // put ID field last
            parameters = ConvertGeographyParametersIntoSeparateLongitudeAndLatitudeParameters(sqlParameters);
        }

        private static List<SQLParameterInfo> ConvertGeographyParametersIntoSeparateLongitudeAndLatitudeParameters(List<SQLParameterInfo> parameters)
        {
            if (parameters.Any(x => x.value is SQLGeographyInfo))
            {
                List<SQLParameterInfo> parameters2 = new List<SQLParameterInfo>();
                foreach (SQLParameterInfo p in parameters)
                {
                    if (p.value is SQLGeographyInfo)
                    {
                        SQLGeographyInfo fug = ((SQLGeographyInfo)(p.value));
                        SQLParameterInfo longParam = new SQLParameterInfo() { dbtype = SqlDbType.Decimal, value = fug.Longitude, fieldname = p.fieldname + "LONG", rownum = p.rownum };
                        parameters2.Add(longParam);
                        SQLParameterInfo latParam = new SQLParameterInfo() { dbtype = SqlDbType.Decimal, value = fug.Latitude, fieldname = p.fieldname + "LAT", rownum = p.rownum };
                        parameters2.Add(latParam);
                    }
                    else
                        parameters2.Add(p);
                }
                return parameters2;
            }
            else
                return parameters;
        }
    }

    public class SQLUpdateRowsInfo
    {
        public string TableName { get; set; }
        public List<SQLUpdateRowInfo> Rows = new List<SQLUpdateRowInfo>();

        public void DoUpdate(ISQLDirectConnectionManager dta)
        {
            StringBuilder sb;
            List<SQLParameterInfo> parameters;
            GetUpdateCommands(out sb, out parameters);
            SQLDirectManipulate.ExecuteSQLNonQuery(dta, sb.ToString(), parameters);
        }

        public void GetUpdateCommands(out StringBuilder sb, out List<SQLParameterInfo> parameters)
        {
            sb = new StringBuilder();
            parameters = new List<SQLParameterInfo>();
            foreach (var row in Rows)
            {
                string updateCommand;
                List<SQLParameterInfo> partialParameters;
                row.GetUpdateCommand(TableName, out updateCommand, out partialParameters);
                sb.Append(updateCommand);
                parameters.AddRange(partialParameters);
            }
        }
    }

    public class SQLUpdateTablesInfo
    {
        public List<SQLUpdateRowsInfo> UpdatesForSingleTable = new List<SQLUpdateRowsInfo>();

        public void DoUpdate(ISQLDirectConnectionManager dta)
        {
            StringBuilder sb;
            List<SQLParameterInfo> parameters;
            GetUpdateCommands(out sb, out parameters);
            SQLDirectManipulate.ExecuteSQLNonQuery(dta, sb.ToString(), parameters);
        }

        public void GetUpdateCommands(out StringBuilder sb, out List<SQLParameterInfo> parameters)
        {
            sb = new StringBuilder();
            parameters = new List<SQLParameterInfo>();
            foreach (var table in UpdatesForSingleTable)
            {
                StringBuilder sb2;
                List<SQLParameterInfo> partialParameters;
                table.GetUpdateCommands(out sb2, out partialParameters);
                sb.Append(sb2);
                parameters.AddRange(partialParameters);
            }
        }
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
                    AddSQLParametersToSqlCommand(parameters, sqlCommand);
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void AddSQLParametersToSqlCommand(List<SQLParameterInfo> parameters, SqlCommand sqlCommand)
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
                        string fullname = "@" + p.fieldname;
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
