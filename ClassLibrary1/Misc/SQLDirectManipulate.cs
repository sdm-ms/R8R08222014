using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1.Misc
{
    public class SQLConstants
    {
        public const int SQLMaxParameters = 2100; // this is a SQL Server maximum that we will need to abide by -- we assume that we will have no more than 2100 per row
    }

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
    public class SQLUpdateInfo
    {
        public string fieldname { get; set; }
        public int? rownum { get; set; }
        public string tablename { get; set; }
        public string paramname { get { if (rownum == null) return fieldname; if (tablename == null) return fieldname + rownum.ToString(); return fieldname + rownum.ToString() + "X" + tablename; } }
        public object value { get; set; }
        public SqlDbType dbtype { get; set; }
        public bool rowNotYetInDatabase { get; set; }
    }

    public static class SQLParameterInfoDistinct
    {
        public static void EliminateOverridenUpdates(this List<SQLUpdateInfo> updates)
        {
            updates.Reverse(); // we reverse so that the last one will be included, but earlier ones won't
            Dictionary<string, bool> itemsProcessed = new Dictionary<string, bool>();
            List<int> itemsToRemove = new List<int>();
            int totalUpdates = updates.Count();
            for (int itemNum = 0; itemNum < totalUpdates; itemNum++)
            {
                SQLUpdateInfo update = updates[itemNum];
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
        public List<SQLUpdateInfo> SQLUpdateInfos { get; set; }
        int Rownum;
        string Tablename;
        private Action ActionToApplyAfterSuccessfulUpdating;
        public Func<bool> IsAlreadyUpdated;

        public SQLUpdateRowInfo(int rownum, string tablename, Action actionToApplyAfterSuccessfulUpdating, Func<bool> isAlreadyUpdated)
        {
            Rownum = rownum;
            Tablename = tablename;
            ActionToApplyAfterSuccessfulUpdating = actionToApplyAfterSuccessfulUpdating;
            IsAlreadyUpdated = isAlreadyUpdated;
            SQLUpdateInfos = new List<SQLUpdateInfo>();
        }

        public void ApplySuccessfulUpdateAction()
        {
            ActionToApplyAfterSuccessfulUpdating(); // NOTE: We might apply this before the query is complete, but it should be submitted only after the update query is complete.
        }

        public bool DataMayAlreadyBeInDatabase()
        {
            return SQLUpdateInfos.Any(x => !x.rowNotYetInDatabase);
        }

        public void Add(string varname, object value, SqlDbType dbtype)
        {
            SQLUpdateInfos.Add(new SQLUpdateInfo { fieldname = varname, rownum = Rownum, tablename = Tablename, value = value, dbtype = dbtype });
        }

        public List<string> GetFieldNames()
        {
            return SQLUpdateInfos.Select(x => x.fieldname).ToList();
        }

        public SQLUpdateInfo GetSQLUpdateInfoForFieldName(string fieldName)
        {
            return SQLUpdateInfos.LastOrDefault(x => x.fieldname == fieldName); // if there is more than one, we take the last
        }

        public string GetParameterizedValuesList(List<string> fieldNames)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            bool isFirst = true;
            foreach (string fieldName in fieldNames)
            {
                SQLUpdateInfo updateInfo = GetSQLUpdateInfoForFieldName(fieldName);
                if (!isFirst)
                    sb.Append(",");
                sb.Append("@");
                if (updateInfo == null)
                    sb.Append("DEFAULT");
                else
                    sb.Append(updateInfo.paramname);
                if (isFirst)
                    isFirst = false;
            }
            sb.Append(")");
            return sb.ToString();
        }

        public void GetUpdateCommand(string tableName, out string updateCommand, out List<SQLUpdateInfo> parameters)
        {
            // step 1: formulate the SQL statement
            if (!SQLUpdateInfos.Any())
            {
                updateCommand = null;
                parameters = null;
                return;
            }

            SQLUpdateInfos.EliminateOverridenUpdates();

            List<string> updateStringComponents = new List<string>();

            updateStringComponents.AddRange(new string[] { "UPDATE ", tableName, " ", "SET " });
            bool isFirst = true;
            string idparamname = "";
            foreach (var variable in SQLUpdateInfos)
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

            foreach (var p in SQLUpdateInfos)
                if (p.value == null)
                    p.value = DBNull.Value;

            List<SQLUpdateInfo> sqlParameters = SQLUpdateInfos.OrderBy(x => x.fieldname == "ID").ToList(); // put ID field last
            parameters = ConvertGeographyParametersIntoSeparateLongitudeAndLatitudeParameters(sqlParameters);
        }

        private static List<SQLUpdateInfo> ConvertGeographyParametersIntoSeparateLongitudeAndLatitudeParameters(List<SQLUpdateInfo> parameters)
        {
            if (parameters.Any(x => x.value is SQLGeographyInfo))
            {
                List<SQLUpdateInfo> parameters2 = new List<SQLUpdateInfo>();
                foreach (SQLUpdateInfo p in parameters)
                {
                    if (p.value is SQLGeographyInfo)
                    {
                        SQLGeographyInfo fug = ((SQLGeographyInfo)(p.value));
                        SQLUpdateInfo longParam = new SQLUpdateInfo() { dbtype = SqlDbType.Decimal, value = fug.Longitude, fieldname = p.fieldname + "LONG", rownum = p.rownum };
                        parameters2.Add(longParam);
                        SQLUpdateInfo latParam = new SQLUpdateInfo() { dbtype = SqlDbType.Decimal, value = fug.Latitude, fieldname = p.fieldname + "LAT", rownum = p.rownum };
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

        public void DoUpdate(int maxParameters, ISQLDirectConnectionManager dta)
        {
            StringBuilder sb;
            List<SQLUpdateInfo> parameters;
            bool moreToDo = true;
            while (moreToDo)
            {
                GetUpdateCommands(maxParameters, out sb, out parameters, out moreToDo);
                SQLDirectManipulate.ExecuteSQLNonQuery(dta, sb.ToString(), parameters);
            }
        }

        public void GetUpdateCommands(int maxParameters, out StringBuilder sb, out List<SQLUpdateInfo> parameters, out bool moreToDo)
        {
            sb = new StringBuilder();
            parameters = new List<SQLUpdateInfo>();
            int parametersUsed = 0;
            moreToDo = false;
            foreach (var row in Rows.Where(x => x.DataMayAlreadyBeInDatabase() && !x.IsAlreadyUpdated()))
            {
                string updateCommand;
                List<SQLUpdateInfo> partialParameters;
                row.GetUpdateCommand(TableName, out updateCommand, out partialParameters);
                parametersUsed += partialParameters.Count();
                if (parametersUsed > maxParameters)
                {
                    moreToDo = true;
                    break;
                }
                row.ApplySuccessfulUpdateAction();
                sb.Append(updateCommand);
                parameters.AddRange(partialParameters);
            }
        }

        public void GetUpsertCommands(int maxParameters, out StringBuilder sb, out List<SQLUpdateInfo> parameters, out bool moreToDo)
        {
            // The format we're looking for is like the below. So, we'll create a string and then fill in the changeable parts.
            // Note that we'll be using parameters instead of actual values in the Values clause.
            //Merge VTemp AS tbl
            //USING
            //(
            //    Select * FROM
            //    (
            //        Values
            //        (1,5,6),
            //        (2,6,7),
            //        (3,7,9),
            //        (4,8,9),
            //        (5,10,11)
            //    ) as upsert1 (ID, Data1, Data2)
            //) AS upsert2
            //ON tbl.ID = upsert2.ID
            //WHEN NOT MATCHED THEN
            //    INSERT(ID, Data1, Data2)
            //     VALUES(upsert2.ID, upsert2.Data1, upsert2.Data2)
            //WHEN MATCHED THEN
            //    UPDATE SET tbl.ID = upsert2.ID, tbl.Data1 = upsert2.Data1, tbl.Data2 = upsert2.Data2
            //;
            string upsertTemplate = @"
Merge {0} AS tbl 
USING 
( 
	Select * FROM 
	( 
		Values 
		{1} 
	) as upsert1 ({2}) 
) AS upsert2 
ON tbl.ID = upsert2.ID 
WHEN NOT MATCHED THEN 
	INSERT({2}) 
	 VALUES({3}) 
WHEN MATCHED THEN 
	UPDATE SET {4} 
; 
";

            sb = new StringBuilder();
            parameters = new List<SQLUpdateInfo>();
            moreToDo = false;
            var rowsStillToBeUpserted = Rows.Where(x => !x.DataMayAlreadyBeInDatabase() && !x.IsAlreadyUpdated());
            if (!rowsStillToBeUpserted.Any())
                return;
            List<string> fieldNames = GetAffectedFieldNames(rowsStillToBeUpserted);
            int maxRows = (maxParameters / fieldNames.Count());
            List<SQLUpdateRowInfo> rowsToProcess;
            if (maxRows < rowsStillToBeUpserted.Count())
            {
                moreToDo = true;
                rowsToProcess = rowsStillToBeUpserted.Take(maxRows).ToList();
            }
            else
                rowsToProcess = rowsStillToBeUpserted.ToList();
            foreach (var row in rowsToProcess)
            {
                foreach (string fieldName in fieldNames)
                {
                    SQLUpdateInfo updateInfo = row.GetSQLUpdateInfoForFieldName(fieldName);
                    if (updateInfo != null)
                        parameters.Add(updateInfo);
                }
                row.ApplySuccessfulUpdateAction();
            }
            string parameterizedValuesList = String.Join(",", rowsToProcess.Select(x => x.GetParameterizedValuesList(fieldNames)).ToArray()); // {1}
            string fieldNamesList = String.Join(",", fieldNames.ToArray()); // {2}
            string valuesStatement = String.Join(",", fieldNames.Select(x => "upsert2." + x).ToArray()); // {3}
            string updateStatement = String.Join(",", fieldNames.Select(x => "tbl." + x + " = upsert2." + x).ToArray()); // {4}
            sb.Append(String.Format(upsertTemplate, TableName, parameterizedValuesList, fieldNamesList, valuesStatement, updateStatement));
        }

        private static List<string> GetAffectedFieldNames(IEnumerable<SQLUpdateRowInfo> rows)
        {
            List<string> fieldNames = new List<string>();
            Dictionary<string, bool> included = new Dictionary<string, bool>(); // use a dictionary so we don't have to look at the list each time to see if it's already there
            foreach (var row in rows)
            {
                List<string> fieldNamesForRow = row.GetFieldNames();
                foreach (string fieldName in fieldNamesForRow)
                {
                    if (!included.ContainsKey(fieldName))
                    {
                        fieldNames.Add(fieldName);
                        included[fieldName] = true;
                    }
                }
            }
            return fieldNames;
        }
    }

    public class SQLUpdateTablesInfo
    {
        public List<SQLUpdateRowsInfo> TablesContainingInformationToUpdate = new List<SQLUpdateRowsInfo>();

        public void DoUpdate(ISQLDirectConnectionManager dta)
        {
            StringBuilder sb;
            List<SQLUpdateInfo> parameters;
            bool moreToDo = true;
            while (moreToDo)
            {
                GetUpdateCommands(SQLConstants.SQLMaxParameters, out sb, out parameters, out moreToDo);
                if (parameters.Any())
                    SQLDirectManipulate.ExecuteSQLNonQuery(dta, sb.ToString(), parameters);
            }
            moreToDo = true;
            while (moreToDo)
            {
                GetUpsertCommands(SQLConstants.SQLMaxParameters, out sb, out parameters, out moreToDo);
                if (parameters.Any())
                    SQLDirectManipulate.ExecuteSQLNonQuery(dta, sb.ToString(), parameters);
            }
        }

        public void GetUpdateCommands(int maxParameters, out StringBuilder sb, out List<SQLUpdateInfo> parameters, out bool moreToDo)
        {
            sb = new StringBuilder();
            parameters = new List<SQLUpdateInfo>();
            int parametersUsed = 0;
            moreToDo = false;
            foreach (var table in TablesContainingInformationToUpdate)
            {
                StringBuilder sb2;
                List<SQLUpdateInfo> partialParameters;
                table.GetUpdateCommands(maxParameters - parametersUsed, out sb2, out partialParameters, out moreToDo);
                parametersUsed += partialParameters.Count;
                if (parametersUsed > maxParameters)
                    throw new Exception("Internal exception. Must not exceed parameter limit.");
                sb.Append(sb2);
                parameters.AddRange(partialParameters);
                if (moreToDo)
                    break; // we're near the maximum number of parameters
            }
        }

        public void GetUpsertCommands(int maxParameters, out StringBuilder sb, out List<SQLUpdateInfo> parameters, out bool moreToDo)
        {
            sb = new StringBuilder();
            parameters = new List<SQLUpdateInfo>();
            int parametersUsed = 0;
            moreToDo = false;
            foreach (var table in TablesContainingInformationToUpdate)
            {
                StringBuilder sb2;
                List<SQLUpdateInfo> partialParameters;
                table.GetUpsertCommands(maxParameters - parametersUsed, out sb2, out partialParameters, out moreToDo);
                parametersUsed += partialParameters.Count;
                if (parametersUsed > maxParameters)
                    throw new Exception("Internal exception. Must not exceed parameter limit.");
                sb.Append(sb2);
                parameters.AddRange(partialParameters);
                if (moreToDo)
                    break; // we're near the maximum number of parameters
            }
        }
    }

    public static class SQLDirectManipulate
    {
        internal static void ExecuteSQLNonQuery(ISQLDirectConnectionManager database, string command, List<SQLUpdateInfo> parameters = null)
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

        private static void AddSQLParametersToSqlCommand(List<SQLUpdateInfo> parameters, SqlCommand sqlCommand)
        {
            foreach (SQLUpdateInfo p in parameters)
            {
                string fullname = "@" + p.paramname;
                sqlCommand.Parameters.Add(fullname, p.dbtype);
                sqlCommand.Parameters[fullname].Value = p.value;
                if (p.dbtype == SqlDbType.Udt)
                    sqlCommand.Parameters[fullname].UdtTypeName = "Geography";
            }
        }

        internal static object ExecuteSQLScalar(ISQLDirectConnectionManager database, string command, List<SQLUpdateInfo> parameters = null)
        {
            using (SqlConnection connection =
               new SqlConnection(database.GetConnectionString()))
            {
                // Create the Command and Parameter objects.
                SqlCommand sqlCommand = new SqlCommand(command, connection);
                if (parameters != null)
                {
                    foreach (SQLUpdateInfo p in parameters)
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
