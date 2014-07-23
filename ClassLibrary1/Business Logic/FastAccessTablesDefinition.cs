using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.EFModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlTypes;
using System.Data.Linq;
using Microsoft.SqlServer.Types;
using GoogleGeocoder;
using System.Collections;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ClassLibrary1.Model
{
    public class DenormalizedTableAccess : ISQLDirectConnectionManager
    {
        int DenormalizedTableGroupNumber;
        public DenormalizedTableAccess(int denormalizedTableGroupNumber)
        {
            DenormalizedTableGroupNumber = denormalizedTableGroupNumber;
        }

        public string GetConnectionString()
        {
            return AzureSetup.GetConfigurationSetting(String.Format("Denorm{0}ConnectionString", DenormalizedTableGroupNumber.ToString("D4")));
        }
    }

    public class FastAccessColumnInfo
    {
        TblColumn TheTblColumn;
        public SQLTableColumnDescription ratingStringColumn { get; set; }
        public SQLTableColumnDescription ratingValueColumn { get; set; }
        public SQLTableColumnDescription ratingIDColumn { get; set; }
        public SQLTableColumnDescription ratingGroupIDColumn { get; set; }
        public SQLTableColumnDescription ratingRecentlyChangedColumn { get; set; }

        public FastAccessColumnInfo(TblColumn tblColumn)
        {
            TheTblColumn = tblColumn;
            Guid tblColumnID = tblColumn.TblColumnID;
            ratingStringColumn = new SQLTableColumnDescription() { Name = "RS" + tblColumnID.ToString("N").ToUpper(), ColType = SQLColumnType.typeStringUnlimited, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            ratingValueColumn = new SQLTableColumnDescription() { Name = "RV" + tblColumnID.ToString("N").ToUpper(), ColType = SQLColumnType.typeDecimal, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = false };
            ratingIDColumn = new SQLTableColumnDescription() { Name = "R" + tblColumnID.ToString("N").ToUpper(), ColType = SQLColumnType.typeGuid, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            ratingGroupIDColumn = new SQLTableColumnDescription() { Name = "RG" + tblColumnID.ToString("N").ToUpper(), ColType = SQLColumnType.typeGuid, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            ratingRecentlyChangedColumn = new SQLTableColumnDescription() { Name = "RC" + tblColumnID.ToString("N").ToUpper(), ColType = SQLColumnType.typeBit, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
        }

        public List<SQLTableColumnDescription> GetColumns()
        {
            return new List<SQLTableColumnDescription> { ratingStringColumn, ratingValueColumn, ratingIDColumn, ratingGroupIDColumn, ratingRecentlyChangedColumn };
        }
    }

    public class FastAccessFieldDefinition
    {
        FieldDefinition TheFieldDefinition;
        public SQLTableColumnDescription fieldColumn { get; set; }

        public FastAccessFieldDefinition(FieldDefinition theFieldDefinition)
        {
            TheFieldDefinition = theFieldDefinition;
            SQLColumnType colType = SQLColumnType.typeBit; // must set a default value
            bool index = false;
            switch (theFieldDefinition.FieldType)
            {
                case (int) FieldTypes.AddressField:
                    colType = SQLColumnType.typeGeography;
                    index = true; // spatial index
                    break;
                case (int) FieldTypes.ChoiceField:
                    colType = SQLColumnType.typeGuid;
                    index = true; // index of integers -- note that we never call this constructor if we are dealing with a choice group with AllowMultipleSelectios = TRUE (in which case we would not have a column here, but instead another table)
                    break;
                case (int)FieldTypes.DateTimeField:
                    colType = SQLColumnType.typeDateTime;
                    index = true;
                    break;
                case (int)FieldTypes.NumberField:
                    colType = SQLColumnType.typeDecimal;
                    index = true;
                    break;
                case (int)FieldTypes.TextField:
                    colType = SQLColumnType.typeStringUnlimited;
                    index = false; // can't index a text field that is a string of unlimited length -- so let's not filter or sort by text fields.
                    break;
                default:
                    break;
            }
            fieldColumn = new SQLTableColumnDescription() { Name = "F" + theFieldDefinition.FieldDefinitionID.ToString("N").ToUpper(), ColType = colType, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = index, Ascending = true };
        }

        public SQLTableColumnDescription GetColumn()
        {
            return fieldColumn;
        }
    }

    // For multiple choice fields, we have a separate table that includes all the cohices. 
    public class FastAccessMultipleChoiceFieldTableInfo
    {
        public FieldDefinition TheFieldDefinition { get; set; }
        public ChoiceGroup TheChoiceGroup { get; set; }
        public SQLTableColumnDescription idColumn { get; set; }
        public SQLTableColumnDescription tblRowIDColumn { get; set; } // == idColumn on SQLFastAccessTableInfo
        public SQLTableColumnDescription choiceColumn { get; set; }

        public FastAccessMultipleChoiceFieldTableInfo(IR8RDataContext iDataContext, FieldDefinition theFieldDefinition)
        {
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            TheFieldDefinition = theFieldDefinition;
            TheChoiceGroup = TheFieldDefinition.ChoiceGroupFieldDefinitions.FirstOrDefault().ChoiceGroup;

            idColumn = new SQLTableColumnDescription() { Name = "MCID" /* distinctive name avoids complication of ambiguous column names when querying */, ColType = SQLColumnType.typeInt, Nullable = false, PrimaryKey = true, AutoIncrement = true, NonclusteredIndex = false, ClusteredIndex = false }; // this column's ID will be created by the database, since we don't care about it and it doesn't come from elsewhere
            tblRowIDColumn = new SQLTableColumnDescription() { Name = "TRID", ColType = SQLColumnType.typeGuid, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            choiceColumn = new SQLTableColumnDescription() { Name = "CHO", ColType = SQLColumnType.typeGuid, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false, ClusteredIndex = true, Ascending = true };
        }

        public SQLTableDescription GetSQLTableDescription()
        {
            return new SQLTableDescription() { Name = "VFMC" + TheFieldDefinition.FieldDefinitionID.ToString("N").ToUpper(), Columns = new List<SQLTableColumnDescription>() { idColumn, tblRowIDColumn, choiceColumn } };
        }

        public static SQLUpdateInfoTableSpecification GetSpecification(Guid fieldDefinitionID, Guid tblID)
        {
            var mainTableSpecification = FastAccessTableInfo.GetSpecification(tblID);
            return new SQLUpdateInfoTableSpecification() { PrimaryKeyFieldName = "MCID", IDIsAutogenerated = true, MainTable = mainTableSpecification, Tablename = "VFMC" + fieldDefinitionID.ToString("N").ToUpper(), HasSameIDAsMainTable = false };
        }
    }

    public class FastAccessTableInfo
    {
        Tbl TheTbl;
        public SQLTableColumnDescription idColumn { get; set; } // == tblRowID
        public SQLTableColumnDescription nameColumn { get; set; }
        public SQLTableColumnDescription nameShortenedForIndexingColumn { get; set; }
        public SQLTableColumnDescription rowFieldDisplay { get; set; }
        //public SQLTableColumnDescription tblRowPageFieldDisplay { get; set; }
        public SQLTableColumnDescription deleted { get; set; }
        public SQLTableColumnDescription countNullEntries { get; set; }
        public SQLTableColumnDescription countUserPoints { get; set; }
        public SQLTableColumnDescription elevateOnMostNeedsRating { get; set; }
        public SQLTableColumnDescription currentlyHighStakes { get; set; }
        public SQLTableColumnDescription recentlyChanged { get; set; }
        public SQLTableColumnDescription volatilityHour { get; set; }
        public SQLTableColumnDescription volatilityDay { get; set; }
        public SQLTableColumnDescription volatilityWeek { get; set; }
        public SQLTableColumnDescription volatilityYear { get; set; }
        public List<FastAccessColumnInfo> ratingInfo { get; set; }
        public List<FastAccessFieldDefinition> fields { get; set; }
        public List<FastAccessMultipleChoiceFieldTableInfo> multipleChoiceFields { get; set; }

        public FastAccessTableInfo(IR8RDataContext iDataContext, Tbl theTbl)
        {
            TheTbl = theTbl;
            idColumn = new SQLTableColumnDescription() { Name = "ID", ColType = SQLColumnType.typeGuid, Nullable = false, PrimaryKey = true, AutoIncrement = false, NonclusteredIndex = false, ClusteredIndex = true /* NOTE: Because we use a spatial index, the clustered index MUST be for the primary key */ };
            nameColumn = new SQLTableColumnDescription() { Name = "NME", ColType = SQLColumnType.typeStringUnlimited, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false, Ascending = true };
            nameShortenedForIndexingColumn = new SQLTableColumnDescription() { Name = "NS", ColType = SQLColumnType.typeString4Chars, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false, Ascending = true, ComputedColumnSpecification = " AS CAST(LEFT(NME,4) AS NCHAR(4)) PERSISTED " };
            rowFieldDisplay = new SQLTableColumnDescription() { Name = "RH", ColType = SQLColumnType.typeStringUnlimited, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            deleted = new SQLTableColumnDescription() { Name = "DEL", ColType = SQLColumnType.typeBit, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            countNullEntries = new SQLTableColumnDescription() { Name = "CNNE", ColType = SQLColumnType.typeInt, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            countUserPoints = new SQLTableColumnDescription() { Name = "CUP", ColType = SQLColumnType.typeDecimal, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            elevateOnMostNeedsRating = new SQLTableColumnDescription() { Name = "ELEV", ColType = SQLColumnType.typeBit, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            currentlyHighStakes = new SQLTableColumnDescription() { Name = "HS", ColType = SQLColumnType.typeInt, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            recentlyChanged = new SQLTableColumnDescription() { Name = "RC", ColType = SQLColumnType.typeBit, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            volatilityHour = new SQLTableColumnDescription() { Name = GetVolatilityColumnNameForDuration(VolatilityDuration.oneHour), ColType = SQLColumnType.typeDecimal, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = false };
            volatilityDay = new SQLTableColumnDescription() { Name = GetVolatilityColumnNameForDuration(VolatilityDuration.oneDay), ColType = SQLColumnType.typeDecimal, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = false };
            volatilityWeek = new SQLTableColumnDescription() { Name = GetVolatilityColumnNameForDuration(VolatilityDuration.oneWeek), ColType = SQLColumnType.typeDecimal, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = false };
            volatilityYear = new SQLTableColumnDescription() { Name = GetVolatilityColumnNameForDuration(VolatilityDuration.oneYear), ColType = SQLColumnType.typeDecimal, Nullable = false, DefaultValue = "0", PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = false };
            

            List<TblColumn> tblColumns = iDataContext.GetTable<TblColumn>().Where(x => x.TblTab.TblID == TheTbl.TblID).ToList();
            ratingInfo = new List<FastAccessColumnInfo>();
            foreach (var col in tblColumns)
                ratingInfo.Add(new FastAccessColumnInfo(col));

            List<FieldDefinition> fieldDefinitions = iDataContext.GetTable<FieldDefinition>().Where(x => x.TblID == TheTbl.TblID && x.Status == (int)StatusOfObject.Active && (x.FieldType != (int) FieldTypes.ChoiceField || (x.ChoiceGroupFieldDefinitions.Any() && !x.ChoiceGroupFieldDefinitions.FirstOrDefault().ChoiceGroup.AllowMultipleSelections))).ToList();
            fields = new List<FastAccessFieldDefinition>();
            foreach (var field in fieldDefinitions)
                fields.Add(new FastAccessFieldDefinition(field));

            List<FieldDefinition> fieldDefinitionsMultipleChoice = iDataContext.GetTable<FieldDefinition>().Where(x => x.TblID == TheTbl.TblID && x.Status == (int)StatusOfObject.Active &&  x.FieldType == (int)FieldTypes.ChoiceField && x.ChoiceGroupFieldDefinitions.Any() && x.ChoiceGroupFieldDefinitions.FirstOrDefault().ChoiceGroup.AllowMultipleSelections).ToList();
            multipleChoiceFields = new List<FastAccessMultipleChoiceFieldTableInfo>();
            foreach (var fieldMC in fieldDefinitionsMultipleChoice)
                multipleChoiceFields.Add(new FastAccessMultipleChoiceFieldTableInfo(iDataContext, fieldMC));
        }

        public static SQLUpdateInfoTableSpecification GetSpecification(Guid tblID)
        {
            return new SQLUpdateInfoTableSpecification() { IDIsAutogenerated = false, PrimaryKeyFieldName = "ID", Tablename = "V" + tblID.ToString("N").ToUpper() };
        }


        public static string GetVolatilityColumnNameForDuration(VolatilityDuration timeFrame)
        {
            string trackerString = null;
            switch (timeFrame)
            {
                case VolatilityDuration.oneHour:
                    trackerString = "VTH";
                    break;
                case VolatilityDuration.oneDay:
                    trackerString = "VTD";
                    break;
                case VolatilityDuration.oneWeek:
                    trackerString = "VTW";
                    break;
                case VolatilityDuration.oneYear:
                    trackerString = "VTY";
                    break;
            }
            return trackerString;
        }

        public List<SQLTableColumnDescription> GetColumns()
        {
            List<SQLTableColumnDescription> theList = new List<SQLTableColumnDescription>() { idColumn, nameColumn, nameShortenedForIndexingColumn, rowFieldDisplay, deleted, countNullEntries, countUserPoints, elevateOnMostNeedsRating, currentlyHighStakes, recentlyChanged, volatilityHour, volatilityDay, volatilityWeek, volatilityYear };
            foreach (var rating in ratingInfo)
                theList.AddRange(rating.GetColumns());
            foreach (var field in fields)
                theList.Add(field.GetColumn());
            return theList;
        }

        internal string GetSqlCommandToDropFunction(string name)
        {
            return String.Format("IF EXISTS (SELECT  * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT')) DROP FUNCTION [dbo].[{0}] ", name);
        }

        internal string GetSqlCommandToDropFunctionForTblRowStatusRecords(Guid tblID)
        {
            return GetSqlCommandToDropFunction("WasActiveTblRowV" + tblID.ToString("N").ToUpper());
        }

        internal string GetSqlCommandToAddFunctionForTblRowStatusRecords(Guid tblID)
        {
            
            return String.Format("CREATE FUNCTION dbo.WasActiveTblRowV{0} ( @p1 DateTime ) RETURNS @TblRowIdsToReturn TABLE ( ID2 INT ) AS BEGIN INSERT INTO @TblRowIdsToReturn SELECT [ID] AS [ID2] FROM dbo.V{0} AS [t0] WHERE (((NOT ([t0].[RC] = 1)) AND ([t0].[DEL] = 0)) OR (([t0].[RC] = 1) AND ([t0].[DEL] = 0) AND (NOT (EXISTS( SELECT NULL AS [EMPTY] FROM [dbo].[TblRowStatusRecord] AS [t1] WHERE ([t1].[TimeChanged] > @p1) AND ([t1].[TblRowID] = [t0].[ID]))))) OR (((SELECT [t3].[Deleting] FROM ( SELECT TOP (1) [t2].[Deleting] FROM [dbo].[TblRowStatusRecord] AS [t2] WHERE ([t2].[TimeChanged] > @p1) AND ([t2].[TblRowID] = [t0].[ID]) ORDER BY [t2].[TimeChanged]) AS [t3])) = 1)) RETURN END", tblID);
        }


        // Note: We have shifted for now to using the built-in nearest neighbor and filtering functions in SQL Server 2012. We may be able to delete these once testing on those is complete. We have commented out the code calling this, but it could be uncommented.
        internal string GetSqlCommandToDropFunctionForNearestNeighbors(string geoColName)
        {
            return GetSqlCommandToDropFunction("UDFNearestNeighborsFor" + geoColName);
        }

        internal string GetSqlCommandToAddFunctionForNearestNeighbors(Guid tblID, string geoColName)
        {
            return String.Format(@"
CREATE FUNCTION [dbo].[UDFNearestNeighborsFor{1}] 
    (@lat as real,@long as real, @neighbors as int)
    RETURNS @TblRowIdsToReturn TABLE 
        (
             TblRowID INT
        )
    AS
    BEGIN
        DECLARE @edi geography = geography::STPointFromText('POINT(' + CAST(@Long AS VARCHAR(20)) + ' ' + 
                                    CAST(@Lat AS VARCHAR(20)) + ')', 4326)
        DECLARE @start FLOAT = 1000;
        WITH NearestPoints AS
        (
          SELECT TOP(@neighbors) WITH TIES n, V{0}.ID,  V{0}.{1}.STDistance(@edi) AS dist
          FROM Numbers JOIN V{0} WITH(INDEX(SIndx_V{0}_{1})) 
          ON V{0}.{1}.STDistance(@edi) < @start*POWER(2,Numbers.n)
          ORDER BY n
        )
        
            
        INSERT INTO @TblRowIdsToReturn
        
        SELECT TOP(@neighbors)
             ID AS TblRowID
        FROM NearestPoints
        ORDER BY n DESC, dist
        
        RETURN 
        
    END
", tblID, geoColName);
        }

        public void AddTable(DenormalizedTableAccess dta)
        {
            var columns = GetColumns();
            SQLTableDescription theTable = new SQLTableDescription() { Name = "V" + TheTbl.TblID.ToString("N").ToUpper(), Columns = columns };
            SQLDirectManipulate.AddTable(dta, theTable);
            SQLDirectManipulate.AddIndicesForSpecifiedColumns(dta, theTable);

            /* We are not joining TblRowStatusRecords for now, but can uncomment this if we go back to that approach.
            SQLDirectManipulate.ExecuteSQLNonQuery(dta, GetSqlCommandToDropFunctionForTblRowStatusRecords(TheTbl.TblID));
            SQLDirectManipulate.ExecuteSQLNonQuery(dta, GetSqlCommandToAddFunctionForTblRowStatusRecords(TheTbl.TblID));
             */

            /* The following is commented out pending more complete testing of the SQL Server 2012 functionality that accomplishes the same thing.
            var geocolumns = columns.Where(x => x.ColType == SQLColumnType.typeGeography);
            foreach (var geocol in geocolumns)
            {
                SQLDirectManipulate.ExecuteSQLNonQuery(dta, GetSqlCommandToDropFunctionForNearestNeighbors(geocol.Name));
                SQLDirectManipulate.ExecuteSQLNonQuery(dta, GetSqlCommandToAddFunctionForNearestNeighbors(TheTbl.TblID, geocol.Name));
            }
             */

            foreach (var fieldMC in multipleChoiceFields)
            {
                SQLTableDescription subTable = fieldMC.GetSQLTableDescription();
                SQLDirectManipulate.AddTable(dta, subTable);
                SQLDirectManipulate.AddIndicesForSpecifiedColumns(dta, subTable);
            }
        }

        public void DropTable(DenormalizedTableAccess dta)
        {
            SQLDirectManipulate.DropTable(dta, "V" + TheTbl.TblID);
            foreach (var fieldMC in multipleChoiceFields)
            {
                SQLTableDescription subTable = fieldMC.GetSQLTableDescription();
                SQLDirectManipulate.DropTable(dta, subTable.Name);
            }
        }

        internal class FastRatingsInfo
        {
#pragma warning disable 0649
            public Guid RowID;
            public Guid ColumnID;
            public decimal? Value;
            public decimal? LastTrustedValue;
            public bool SingleNumber;
            public int DecimalPlaces;
            public Guid? RatingGroupID;
            public Guid? RatingID;
            public bool? RecentlyChanged;
#pragma warning restore 0649
        }

        internal class FastAddressFieldsInfo
        {
#pragma warning disable 0649
            public Guid FieldDefinitionID;
            public SqlGeography Geo;
            public decimal? Longitude;
            public decimal? Latitude;
            //public Guid FieldID; // rather than copy the Geo field, we will join AddressFields table, because of challenges of SQLGeography in Linq to SQL
#pragma warning restore 0649
        }

        internal class FastChoiceFieldsInfo
        {
#pragma warning disable 0649
            public Guid FieldDefinitionID;
            public bool MultipleChoices;
            public List<ChoiceInField> Choices;
#pragma warning restore 0649
        }

        internal class FastDateTimeFieldsInfo
        {
#pragma warning disable 0649
            public Guid FieldDefinitionID;
            public DateTime? DateTime;
#pragma warning restore 0649
        }

        internal class FastNumberFieldsInfo
        {
#pragma warning disable 0649
            public Guid FieldDefinitionID;
            public decimal? Number;
#pragma warning restore 0649
        }

        internal class FastTextFieldsInfo
        {
#pragma warning disable 0649
            public Guid FieldDefinitionID;
            public string Text;
#pragma warning restore 0649
        }

        internal class FastAccessRowsInfo
        {
            public TblRow TblRow;

#pragma warning disable 0649
            public Guid ID;
            public string NME;
            internal string RF;
            internal string PF;
            public string RowHeading { get { return GetCombinedRowHeadingWithPopup(RF, PF, NME, ID); }  }
            public bool DEL;
            public int CNNE;
            public decimal CUP;
            public bool ELEV;
            public int HS;
            public bool RC;
            public IEnumerable<FastRatingsInfo> Ratings;
            public IEnumerable<FastAddressFieldsInfo> AddressFields;
            public IEnumerable<FastChoiceFieldsInfo> ChoiceFields;
            public IEnumerable<FastDateTimeFieldsInfo> DateTimeFields;
            public IEnumerable<FastNumberFieldsInfo> NumberFields;
            public IEnumerable<FastTextFieldsInfo> TextFields;
#pragma warning restore 0649

            internal static string GetCombinedRowHeadingWithPopup(string rowHeading, string popup, string name, Guid tblRowID)
            {
                if (rowHeading == null)
                    throw new Exception("Internal error. Should not copy row until initial fields display set.");
                bool popupIsEmpty = popup == null || popup.Contains("borderless nopop");
                string relAttr = popupIsEmpty ? "" : "rel=\"#FPU" + tblRowID.ToString("N").ToUpper() + "\"";
                string idAttr = popupIsEmpty ? "" : "id=\"FPU" + tblRowID.ToString("N").ToUpper() + "\"";
                string mainAreaOpening = String.Format("<div class=\"loadrowpopup\" title=\"{0}\" {1}>", name ?? "", relAttr);
                string closeDiv = "</div>"; 
                string popupOpening = String.Format("<div style=\"display:none;\" {0}>", idAttr);
                return mainAreaOpening + rowHeading + closeDiv + popupOpening + (popup ?? "") + closeDiv;
            }


            public void AddDataRow(DataTable dataTable)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["ID"] = ID;
                dataRow["NME"] = NME;
                dataRow["RH"] = RowHeading;
                dataRow["DEL"] = DEL;
                dataRow["CNNE"] = CNNE;
                dataRow["CUP"] = CUP;
                dataRow["ELEV"] = ELEV;
                dataRow["HS"] = HS;
                dataRow["RC"] = RC;
                foreach (var rating in Ratings)
                {
                    string theRatingString = "";
                    if (rating.SingleNumber)
                        theRatingString = NumberandTableFormatter.FormatAsSpecified(rating.Value, rating.DecimalPlaces, rating.ColumnID);
                    else
                        theRatingString = TableLoading.GetHtmlStringForCell((Guid)rating.RatingGroupID, rating.Value, rating.DecimalPlaces, rating.RowID, rating.ColumnID, rating.RatingID, null, rating.LastTrustedValue == rating.Value, rating.SingleNumber, false);
                    dataRow["RS" + rating.ColumnID.ToString("N").ToUpper()] = theRatingString;
                    dataRow["RV" + rating.ColumnID.ToString("N").ToUpper()] = (object)rating.Value ?? (object)System.DBNull.Value;
                    dataRow["R" + rating.ColumnID.ToString("N").ToUpper()] = (object)rating.RatingID ?? (object)System.DBNull.Value;
                    dataRow["RG" + rating.ColumnID.ToString("N").ToUpper()] = (object)rating.RatingGroupID ?? (object)System.DBNull.Value;
                    dataRow["RC" + rating.ColumnID.ToString("N").ToUpper()] = (object)rating.RecentlyChanged ?? (object)System.DBNull.Value;
                }
                foreach (var addressField in AddressFields)
                {
                    dataRow["F" + addressField.FieldDefinitionID.ToString("N").ToUpper()] = (object) addressField.Geo;
                }
                foreach (var choiceField in ChoiceFields)
                {
                    if (!choiceField.MultipleChoices)
                    {
                        dataRow["F" + choiceField.FieldDefinitionID.ToString("N").ToUpper()] = choiceField.Choices.FirstOrDefault() == null ? (object)System.DBNull.Value : (object)choiceField.Choices.FirstOrDefault().ChoiceInGroupID;
                    }
                }
                foreach (var dateTimeField in DateTimeFields)
                {
                    dataRow["F" + dateTimeField.FieldDefinitionID.ToString("N").ToUpper()] = dateTimeField.DateTime ?? (object)System.DBNull.Value;
                }
                foreach (var numberField in NumberFields)
                {
                    dataRow["F" + numberField.FieldDefinitionID.ToString("N").ToUpper()] = numberField.Number ?? (object)System.DBNull.Value;
                }
                foreach (var textField in TextFields)
                {
                    dataRow["F" + textField.FieldDefinitionID.ToString("N").ToUpper()] = textField.Text ?? (object)System.DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }

            public void AddDataRowsForMultipleChoiceField(FieldDefinition theFieldDefinition, DataTable dataTable)
            {
                var theChoices = ChoiceFields.Where(x => x.FieldDefinitionID == theFieldDefinition.FieldDefinitionID).SelectMany(y => y.Choices);
                foreach (var choice in theChoices)
                {
                    var dataRow = dataTable.NewRow();
                    dataRow["ID"] = choice.ChoiceInFieldID;
                    dataRow["CHO"] = choice.ChoiceInGroupID;
                    dataRow["TRID"] = ID; // the table row id
                    dataTable.Rows.Add(dataRow);
                };
            }

        }

        internal List<FastAccessRowsInfo> storedRows;

        internal void StoreRowsInfo(IEnumerable<TblRow> tblRows, bool omitRatings = false, bool omitFields = false)
        {
            if (omitRatings && omitFields)
            {
                storedRows = tblRows.Select(x => new FastAccessRowsInfo
                {
                    TblRow = x,
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name,199),
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNNE = x.CountNonnullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow,
                    RC = x.StatusRecentlyChanged
                }).ToList();
            }
            else if (!omitRatings && !omitFields)
            {
                storedRows = tblRows.Select(x => new FastAccessRowsInfo
                {
                    TblRow = x,
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name, 199),
                    RF = x.TblRowFieldDisplay.Row,
                    PF = x.TblRowFieldDisplay.PopUp,
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNNE = x.CountNonnullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow,
                    RC = x.StatusRecentlyChanged,
                    Ratings =   from y in x.RatingGroups
                                let firstRating = y.Ratings.OrderBy(z => z.NumInGroup).FirstOrDefault()
                                let singleNumber = y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber
                                where singleNumber || (y.RatingGroupAttribute.TypeOfRatingGroup != (int) RatingGroupTypes.hierarchyNumbersBelow && y.RatingGroupAttribute.TypeOfRatingGroup != (int) RatingGroupTypes.probabilityHierarchyBelow && y.RatingGroupAttribute.TypeOfRatingGroup != (int) RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy)
                                select new FastRatingsInfo
                                {
                                    RowID = y.TblRowID,
                                    ColumnID = y.TblColumnID,
                                    Value = firstRating == null ? null : firstRating.CurrentValue,
                                    LastTrustedValue = firstRating == null ? null : firstRating.LastTrustedValue,
                                    SingleNumber = (bool) singleNumber,
                                    RatingGroupID = y.RatingGroupID,
                                    DecimalPlaces = firstRating == null ? 0 : y.Ratings.FirstOrDefault().RatingCharacteristic.DecimalPlaces,
                                    RatingID = firstRating == null || !singleNumber ? null : (Guid?)y.Ratings.FirstOrDefault().RatingID, 
                                    RecentlyChanged = y.ValueRecentlyChanged
                                },
                    AddressFields = x.Fields.SelectMany(z => z.AddressFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastAddressFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Longitude = z.Longitude,
                            Latitude = z.Latitude,
                            Geo = (z.Latitude != null && z.Longitude != null && !(z.Latitude == 0 && z.Longitude == 0)) ? SqlGeography.Point((double) z.Latitude, (double) z.Longitude, 4326) : null
                            //FieldID = z.FieldID
                        }),
                    ChoiceFields = x.Fields.SelectMany(z => z.ChoiceFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastChoiceFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            MultipleChoices = z.Field.FieldDefinition.ChoiceGroupFieldDefinitions.FirstOrDefault().ChoiceGroup.AllowMultipleSelections,
                            Choices = z.ChoiceInFields.Where(w => true).ToList()
                        }),
                    DateTimeFields = x.Fields.SelectMany(z => z.DateTimeFields).Where(w => w.Status == (int) StatusOfObject.Active).Select(
                        z => new FastDateTimeFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            DateTime = z.DateTime
                        }),
                    NumberFields = x.Fields.SelectMany(z => z.NumberFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastNumberFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Number = z.Number
                        }),
                    TextFields = x.Fields.SelectMany(z => z.TextFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastTextFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Text = z.Text
                        })
                }
                ).ToList();
            }
            else if (omitFields)
            {
                storedRows = tblRows.Select(x => new FastAccessRowsInfo
                {
                    TblRow = x,
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name,199),
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNNE = x.CountNonnullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow,
                    RC = x.StatusRecentlyChanged,
                    Ratings = from y in x.RatingGroups
                              let firstRating = y.Ratings.FirstOrDefault()
                              let singleNumber = y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber
                              where singleNumber || (y.RatingGroupAttribute.TypeOfRatingGroup != (int)RatingGroupTypes.hierarchyNumbersBelow && y.RatingGroupAttribute.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityHierarchyBelow && y.RatingGroupAttribute.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy)
                              select new FastRatingsInfo
                              {
                                  ColumnID = y.TblColumnID,
                                  Value = firstRating == null ? null : firstRating.CurrentValue,
                                  LastTrustedValue = firstRating == null ? null : firstRating.LastTrustedValue,
                                  SingleNumber = (bool)singleNumber,
                                  RatingGroupID = y.RatingGroupID,
                                  DecimalPlaces = firstRating == null ? 0 : y.Ratings.FirstOrDefault().RatingCharacteristic.DecimalPlaces,
                                  RatingID = firstRating == null || !singleNumber ? null : (Guid?)y.Ratings.FirstOrDefault().RatingID,
                                  RecentlyChanged = y.ValueRecentlyChanged
                              }
                }).ToList();
            }
            else if (omitRatings)
            {
                storedRows = tblRows.Select(x => new FastAccessRowsInfo
                {
                    TblRow = x,
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name,199),
                    RF = x.TblRowFieldDisplay.Row,
                    PF = x.TblRowFieldDisplay.PopUp, 
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNNE = x.CountNonnullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow,
                    RC = x.StatusRecentlyChanged,
                    AddressFields = x.Fields.SelectMany(z => z.AddressFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastAddressFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Longitude = z.Longitude,
                            Latitude = z.Latitude,
                            Geo = (z.Latitude != null && z.Longitude != null && !(z.Latitude == 0 && z.Longitude == 0)) ? SqlGeography.Point((double) z.Latitude, (double) z.Longitude, 4326) : null
                            //FieldID = z.FieldID
                        }),
                    ChoiceFields = x.Fields.SelectMany(z => z.ChoiceFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastChoiceFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            MultipleChoices = z.Field.FieldDefinition.ChoiceGroupFieldDefinitions.FirstOrDefault().ChoiceGroup.AllowMultipleSelections,
                            Choices = z.ChoiceInFields.Where(w => true).ToList()
                        }),
                    DateTimeFields = x.Fields.SelectMany(z => z.DateTimeFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastDateTimeFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            DateTime = z.DateTime
                        }),
                    NumberFields = x.Fields.SelectMany(z => z.NumberFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastNumberFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Number = z.Number
                        }),
                    TextFields = x.Fields.SelectMany(z => z.TextFields).Where(w => w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastTextFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Text = z.Text
                        })
                }
                ).ToList();
            }
        }

        internal DataTable CreateDataTable(IR8RDataContext iDataContext, List<TblRow> tblRows)
        {
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            // assumes that row info has already been stored. 

            // Create a new DataTable.
            System.Data.DataTable dataTable = new DataTable("ParentTable");

            SQLTableDescription theTable = new SQLTableDescription() { Name = "V" + TheTbl.TblID.ToString("N").ToUpper(), Columns = GetColumns() };
            List<DataColumn> dataColumns = theTable.GetDataColumns();
            foreach (var column in dataColumns)
                dataTable.Columns.Add(column);

            // Make the ID column the primary key column.
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = dataTable.Columns["ID"];
            dataTable.PrimaryKey = PrimaryKeyColumns;

            storedRows.ForEach(x => x.AddDataRow(dataTable));

            return dataTable;
        }

        internal DataTable CreateDataTableForMultipleChoiceField(IR8RDataContext iDataContext, FastAccessMultipleChoiceFieldTableInfo subtableInfo)
        {
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;

            SQLTableDescription subTable = subtableInfo.GetSQLTableDescription(); 

            // Create a new DataTable.
            System.Data.DataTable dataTable = new DataTable("SubTable" + subTable.Name);

            List<DataColumn> dataColumns = subTable.GetDataColumns(false);
            foreach (var column in dataColumns)
                dataTable.Columns.Add(column);

            storedRows.ForEach(x => x.AddDataRowsForMultipleChoiceField(subtableInfo.TheFieldDefinition, dataTable));

            return dataTable;
        }

        public int BulkCopyRows(IR8RDataContext iDataContext, DenormalizedTableAccess dta, List<TblRow> tblRows, List<TblColumn> tblColumns)
        {
            IR8RDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return 0;

            StoreRowsInfo(tblRows, false, false);

            DataTable theDataTable = CreateDataTable(iDataContext, tblRows);

            string denormalizedConnection = dta.GetConnectionString();
            
            SqlBulkCopy bulk = new SqlBulkCopy(denormalizedConnection, SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.UseInternalTransaction);
            bulk.DestinationTableName = "dbo.V" + TheTbl.TblID.ToString("N").ToUpper();
            bulk.WriteToServer(theDataTable);

            int numRecords = theDataTable.Rows.Count;

            // now, bulk copy data to the separate table created for each multiple choice field (if any)
            foreach (var fieldMC in multipleChoiceFields)
            {
                theDataTable = CreateDataTableForMultipleChoiceField(iDataContext, fieldMC);
                SqlBulkCopy bulk2 = new SqlBulkCopy(denormalizedConnection);
                bulk2.ColumnMappings.Add(new SqlBulkCopyColumnMapping("TRID", "TRID"));
                bulk2.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CHO", "CHO"));
                bulk2.DestinationTableName = "dbo.VFMC" + fieldMC.TheFieldDefinition.FieldDefinitionID.ToString("N").ToUpper();
                bulk2.WriteToServer(theDataTable);
            }

            return numRecords;
            
        }

        public int UpdateRows(DenormalizedTableAccess dta, IQueryable<TblRow> tblRows, bool updateRatings = true, bool updateFields = true)
        {
            StoreRowsInfo(tblRows, !updateRatings, !updateFields);
            SQLDatabaseChangeInfo allRows = GenerateSQLDatabaseChangeInfoForMainTable(updateRatings, updateFields);
            if (updateFields)
                GenerateChangesForMCTables(dta);
            allRows.ExecuteAllCommands(dta);
            return allRows.Rows.Count();
        }

        internal void GenerateChangesForMCTables(DenormalizedTableAccess dta)
        {
            foreach (var storedRow in storedRows)
            {
                foreach (var choiceField in storedRow.ChoiceFields)
                {
                    if (choiceField.MultipleChoices)
                    {
                        DeleteExistingChoicesInMCTable(dta, choiceField.FieldDefinitionID, storedRow.ID);
                        AddChoicesToMCTable(dta, choiceField.FieldDefinitionID, storedRow.ID, choiceField.Choices.Select(x => x.ChoiceInGroupID).ToList());
                    }
                }
            }
        }

        internal void DeleteExistingChoicesInMCTable(DenormalizedTableAccess dta, Guid fieldDefinitionID, Guid tblRowID)
        {
            string tableName = "dbo.VFMC" + fieldDefinitionID.ToString("N").ToUpper();
            string deleteString = "DELETE FROM " + tableName + " WHERE TRID = @trid ";
            SQLDirectManipulate.ExecuteSQLNonQuery(dta, deleteString, new List<SQLCellInfo>() { new SQLCellInfo() { DBtype = SqlDbType.UniqueIdentifier, Value = tblRowID, Fieldname = "trid" } });
        }

        internal void AddChoicesToMCTable(DenormalizedTableAccess dta, Guid fieldDefinitionID, Guid tblRowID, List<Guid> choiceInGroupIDs)
        {
            string tableName = "dbo.VFMC" + fieldDefinitionID.ToString("N").ToUpper();
            foreach (Guid choiceInGroupID in choiceInGroupIDs)
            {
                string insertString = "INSERT INTO " + tableName + " (TRID, CHO) VALUES (@trid, @cigid) ";
                SQLDirectManipulate.ExecuteSQLNonQuery(dta, insertString, new List<SQLCellInfo>() { new SQLCellInfo() { DBtype = SqlDbType.UniqueIdentifier, Value = tblRowID, Fieldname = "trid" }, new SQLCellInfo() { DBtype = SqlDbType.UniqueIdentifier, Value = choiceInGroupID, Fieldname = "cigid" } });
            }
        }

        public void ResetBulkRatingsFlags(TblRow tblRow)
        {
            tblRow.FastAccessUpdateFields = false;
            tblRow.FastAccessUpdateRatings = false;
        }

        internal SQLDatabaseChangeInfo GenerateSQLDatabaseChangeInfoForMainTable(bool updateRatings, bool updateFields)
        {
            SQLDatabaseChangeInfo dci = new SQLDatabaseChangeInfo();

            // Note: right now, this code should not get called. If we change that temporarily, set one or more of the following to false so that we can do bulk updating or adding.
            bool DisableUpdateRowInfo = true; 
            bool DisableUpdateFields = true; 
            bool DisableUpdateRatings = true; 
            foreach (var storedRow in storedRows)
            {
                SQLInfoForCellsInRow_MainAndSecondaryTables row = new SQLInfoForCellsInRow_MainAndSecondaryTables();
                row.DefaultTableSpec = GetSpecification(TheTbl.TblID);
                row.Add("ID", storedRow.ID, SqlDbType.UniqueIdentifier);
                if (!DisableUpdateRowInfo)
                {
                    row.Add("NME", storedRow.NME, SqlDbType.NVarChar); 
                    row.Add("DEL", storedRow.DEL.ToString(), SqlDbType.Bit);
                    row.Add("CNNE", storedRow.CNNE.ToString(), SqlDbType.Int);
                    row.Add("CUP", storedRow.CUP.ToString(), SqlDbType.Decimal);
                    row.Add("ELEV", storedRow.ELEV.ToString(), SqlDbType.Bit);
                    row.Add("HS", storedRow.HS.ToString(), SqlDbType.Bit);
                    row.Add("RC", storedRow.RC.ToString(), SqlDbType.Bit);
                }
                if (updateFields && !DisableUpdateFields)
                {
                    row.Add("RH", storedRow.RowHeading, SqlDbType.NVarChar);
                    foreach (var field in storedRow.AddressFields)
                    {
                        row.Add("F" + field.FieldDefinitionID.ToString("N").ToUpper(), (field.Latitude == null || field.Longitude == null || (field.Longitude == 0 && field.Latitude == 0)) ? null : new SQLGeographyInfo { Longitude = (decimal)field.Longitude, Latitude = (decimal)field.Latitude }, SqlDbType.Udt);
                    }
                    foreach (var field in storedRow.ChoiceFields)
                    {
                        if (!field.MultipleChoices)
                        {
                            row.Add("F" + field.FieldDefinitionID.ToString("N").ToUpper(), field.Choices.FirstOrDefault().ChoiceInGroupID, SqlDbType.UniqueIdentifier); // Just adding the first ChoiceInGroupID here, even if there are multiple rows
                        }
                    }
                    foreach (var field in storedRow.DateTimeFields)
                    {
                        row.Add("F" + field.FieldDefinitionID.ToString("N").ToUpper(), field.DateTime, SqlDbType.DateTime);
                    }
                    foreach (var field in storedRow.NumberFields)
                    {
                        row.Add("F" + field.FieldDefinitionID.ToString("N").ToUpper(), field.Number, SqlDbType.Decimal);
                    }
                    foreach (var field in storedRow.TextFields)
                    {
                        row.Add("F" + field.FieldDefinitionID.ToString("N").ToUpper(), field.Text, SqlDbType.NVarChar);
                    }
                }
                if (updateRatings && !DisableUpdateRatings)
                {
                    foreach (var rating in storedRow.Ratings)
                    {
                        if (rating.SingleNumber)
                            row.Add("RS" + rating.ColumnID.ToString("N").ToUpper(), NumberandTableFormatter.FormatAsSpecified(rating.Value, rating.DecimalPlaces, rating.ColumnID) + (rating.LastTrustedValue == rating.Value ? "" : "*"), SqlDbType.NVarChar);
                        else
                            row.Add("RS" + rating.ColumnID.ToString("N").ToUpper(), TableLoading.GetHtmlStringForCell((Guid)rating.RatingGroupID, rating.Value, rating.DecimalPlaces, rating.RowID, rating.ColumnID, rating.RatingID, null, rating.LastTrustedValue == rating.Value, rating.SingleNumber, false), SqlDbType.NVarChar);
                        row.Add("RV" + rating.ColumnID.ToString("N").ToUpper(), rating.Value, SqlDbType.Decimal);
                        row.Add("RC" + rating.ColumnID.ToString("N").ToUpper(), rating.RecentlyChanged, SqlDbType.Bit);
                        //rowInfo.Add("R" + rating.ColumnID.ToString("N").ToUpper(), rating.RatingID); doesn't change -- no need to update
                        //rowInfo.Add("RG" + rating.ColumnID.ToString("N").ToUpper(), rating.RatingGroupID); doesn't change -- no need to update
                    }
                }

                dci.Rows.Add(row);
            }
            return dci;
        }
    }

    public enum FastAccessTableStatus
    {
        fastAccessNotCreated,
        newRowsMustBeCopied,
        apparentlySynchronized
    }
}
