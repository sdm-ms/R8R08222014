using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data;

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


    public static class SQLDirectManipulate
    {
        internal static void ExecuteSQL(this DataContext linqSqlDataContext, string command)
        {
            linqSqlDataContext.ExecuteCommand(command);
        }

        public static void DropTable(DataContext linqSqlDataContext, string tableName)
        {
            string dropCommand = String.Format("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U')) \n DROP TABLE [dbo].[{0}] \n ", tableName);
            linqSqlDataContext.ExecuteCommand(dropCommand);
        }

        public static void AddTable(DataContext linqSqlDataContext, SQLTableDescription table)
        {
            DropTable(linqSqlDataContext, table.Name);
            string colNames = "";
            foreach (var col in table.Columns)
                colNames += col.ToSpecificationString() + ", \n ";
            string primaryKey = table.Columns.Single(x => x.PrimaryKey).Name;
            string addCommand = String.Format("CREATE TABLE [dbo].[{0}]( \n {1} CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED \n ( \n [{2}] ASC \n )WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF \n ) ON [PRIMARY] \n ) ON [PRIMARY]", table.Name, colNames, primaryKey);
            linqSqlDataContext.ExecuteCommand(addCommand);
        }

        public static void AddIndicesForSpecifiedColumns(DataContext linqSqlDataContext, SQLTableDescription table)
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
                linqSqlDataContext.ExecuteCommand(cmd);
            }
        }

    }
}
