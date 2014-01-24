using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;

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
    public class SQLTableColumnInfo
    {
        TblColumn TheTblColumn;
        public SQLTableColumnDescription ratingStringColumn { get; set; }
        public SQLTableColumnDescription ratingValueColumn { get; set; }
        public SQLTableColumnDescription ratingIDColumn { get; set; }
        public SQLTableColumnDescription ratingGroupIDColumn { get; set; }
        public SQLTableColumnDescription ratingRecentlyChangedColumn { get; set; }

        public SQLTableColumnInfo(TblColumn tblColumn)
        {
            TheTblColumn = tblColumn;
            int tblColumnID = tblColumn.TblColumnID;
            ratingStringColumn = new SQLTableColumnDescription() { Name = "RS" + tblColumnID.ToString(), ColType = SQLColumnType.typeString, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            ratingValueColumn = new SQLTableColumnDescription() { Name = "RV" + tblColumnID.ToString(), ColType = SQLColumnType.typeDecimal, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = false };
            ratingIDColumn = new SQLTableColumnDescription() { Name = "R" + tblColumnID.ToString(), ColType = SQLColumnType.typeInt, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            ratingGroupIDColumn = new SQLTableColumnDescription() { Name = "RG" + tblColumnID.ToString(), ColType = SQLColumnType.typeInt, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            ratingRecentlyChangedColumn = new SQLTableColumnDescription() { Name = "RC" + tblColumnID.ToString(), ColType = SQLColumnType.typeBit, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
        }

        public List<SQLTableColumnDescription> GetColumns()
        {
            return new List<SQLTableColumnDescription> { ratingStringColumn, ratingValueColumn, ratingIDColumn, ratingGroupIDColumn, ratingRecentlyChangedColumn };
        }
    }

    public class SQLTableFieldDefinition
    {
        FieldDefinition TheFieldDefinition;
        public SQLTableColumnDescription fieldColumn { get; set; }

        public SQLTableFieldDefinition(FieldDefinition theFieldDefinition)
        {
            TheFieldDefinition = theFieldDefinition;
            SQLColumnType colType = SQLColumnType.typeBit; // must set a default value
            switch (theFieldDefinition.FieldType)
            {
                case (int) FieldTypes.AddressField:
                    colType = SQLColumnType.typeGeography;
                    break;
                case (int) FieldTypes.ChoiceField:
                    colType = SQLColumnType.typeInt;
                    break;
                case (int)FieldTypes.DateTimeField:
                    colType = SQLColumnType.typeDateTime;
                    break;
                case (int)FieldTypes.NumberField:
                    colType = SQLColumnType.typeDecimal;
                    break;
                case (int)FieldTypes.TextField:
                    colType = SQLColumnType.typeString;
                    break;
                default:
                    break;
            }
            fieldColumn = new SQLTableColumnDescription() { Name = "F" + theFieldDefinition.FieldDefinitionID.ToString(), ColType = colType, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
        }

        public SQLTableColumnDescription GetColumn()
        {
            return fieldColumn;
        }
    }

    // For multiple choice fields, we have a separate table that includes all the cohices. 
    public class SQLMultipleChoiceFieldTableInfo
    {
        public FieldDefinition TheFieldDefinition { get; set; }
        public ChoiceGroup TheChoiceGroup { get; set; }
        public SQLTableColumnDescription idColumn { get; set; }
        public SQLTableColumnDescription tblRowIDColumn { get; set; } // == idColumn on SQLFastAccessTableInfo
        public SQLTableColumnDescription choiceColumn { get; set; }

        public SQLMultipleChoiceFieldTableInfo(IRaterooDataContext iDataContext, FieldDefinition theFieldDefinition)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            TheFieldDefinition = theFieldDefinition;
            TheChoiceGroup = TheFieldDefinition.ChoiceGroupFieldDefinitions.First().ChoiceGroup;

            idColumn = new SQLTableColumnDescription() { Name = "MCID", ColType = SQLColumnType.typeInt, Nullable = false, PrimaryKey = true, AutoIncrement = true, NonclusteredIndex = false };
            tblRowIDColumn = new SQLTableColumnDescription() { Name = "TRID", ColType = SQLColumnType.typeInt, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            choiceColumn = new SQLTableColumnDescription() { Name = "CHO", ColType = SQLColumnType.typeInt, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
        }

        public SQLTableDescription GetSQLTableDescription()
        {
            return new SQLTableDescription() { Name = "VFMC" + TheFieldDefinition.FieldDefinitionID.ToString(), Columns = new List<SQLTableColumnDescription>() { idColumn, tblRowIDColumn, choiceColumn } };
        }
    }

    public class SQLFastAccessTableInfo
    {
        Tbl TheTbl;
        public SQLTableColumnDescription idColumn { get; set; } // == tblRowID
        public SQLTableColumnDescription nameColumn { get; set; }
        public SQLTableColumnDescription rowFieldDisplay { get; set; }
        //public SQLTableColumnDescription tblRowPageFieldDisplay { get; set; }
        public SQLTableColumnDescription deleted { get; set; }
        public SQLTableColumnDescription countNullEntries { get; set; }
        public SQLTableColumnDescription countUserPoints { get; set; }
        public SQLTableColumnDescription elevateOnMostNeedsRating { get; set; }
        public SQLTableColumnDescription currentlyHighStakes { get; set; }
        public SQLTableColumnDescription recentlyChanged { get; set; }
        public List<SQLTableColumnInfo> ratingInfo { get; set; }
        public List<SQLTableFieldDefinition> filterableFields { get; set; }
        public List<SQLMultipleChoiceFieldTableInfo> filterableMultipleChoiceFields { get; set; }

        public SQLFastAccessTableInfo(IRaterooDataContext iDataContext, Tbl theTbl)
        {
            TheTbl = theTbl;
            idColumn = new SQLTableColumnDescription() { Name = "ID", ColType = SQLColumnType.typeInt, Nullable = false, PrimaryKey = true, AutoIncrement = false, NonclusteredIndex = false };
            nameColumn = new SQLTableColumnDescription() { Name = "NME", ColType = SQLColumnType.typeString, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            rowFieldDisplay = new SQLTableColumnDescription() { Name = "RH", ColType = SQLColumnType.typeString, Nullable = true, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = false };
            deleted = new SQLTableColumnDescription() { Name = "DEL", ColType = SQLColumnType.typeBit, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            countNullEntries = new SQLTableColumnDescription() { Name = "CNE", ColType = SQLColumnType.typeInt, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            countUserPoints = new SQLTableColumnDescription() { Name = "CUP", ColType = SQLColumnType.typeDecimal, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            elevateOnMostNeedsRating = new SQLTableColumnDescription() { Name = "ELEV", ColType = SQLColumnType.typeBit, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            currentlyHighStakes = new SQLTableColumnDescription() { Name = "HS", ColType = SQLColumnType.typeBit, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            recentlyChanged = new SQLTableColumnDescription() { Name = "RC", ColType = SQLColumnType.typeBit, Nullable = false, PrimaryKey = false, AutoIncrement = false, NonclusteredIndex = true, Ascending = true };
            

            List<TblColumn> tblColumns = iDataContext.GetTable<TblColumn>().Where(x => x.TblTab.TblID == TheTbl.TblID).ToList();
            ratingInfo = new List<SQLTableColumnInfo>();
            foreach (var col in tblColumns)
                ratingInfo.Add(new SQLTableColumnInfo(col));

            List<FieldDefinition> fieldDefinitions = iDataContext.GetTable<FieldDefinition>().Where(x => x.TblID == TheTbl.TblID && x.Status == (int)StatusOfObject.Active && x.UseAsFilter && (x.FieldType != (int) FieldTypes.ChoiceField || (x.ChoiceGroupFieldDefinitions.Any() && !x.ChoiceGroupFieldDefinitions.First().ChoiceGroup.AllowMultipleSelections))).ToList();
            filterableFields = new List<SQLTableFieldDefinition>();
            foreach (var field in fieldDefinitions)
                filterableFields.Add(new SQLTableFieldDefinition(field));

            List<FieldDefinition> fieldDefinitionsMultipleChoice = iDataContext.GetTable<FieldDefinition>().Where(x => x.TblID == TheTbl.TblID && x.Status == (int)StatusOfObject.Active && x.UseAsFilter && x.FieldType == (int)FieldTypes.ChoiceField && x.ChoiceGroupFieldDefinitions.Any() && x.ChoiceGroupFieldDefinitions.First().ChoiceGroup.AllowMultipleSelections).ToList();
            filterableMultipleChoiceFields = new List<SQLMultipleChoiceFieldTableInfo>();
            foreach (var fieldMC in fieldDefinitionsMultipleChoice)
                filterableMultipleChoiceFields.Add(new SQLMultipleChoiceFieldTableInfo(iDataContext, fieldMC));
        }

        public List<SQLTableColumnDescription> GetColumns()
        {
            List<SQLTableColumnDescription> theList = new List<SQLTableColumnDescription>() { idColumn, nameColumn, rowFieldDisplay, deleted, countNullEntries, countUserPoints, elevateOnMostNeedsRating, currentlyHighStakes, recentlyChanged };
            foreach (var rating in ratingInfo)
                theList.AddRange(rating.GetColumns());
            foreach (var field in filterableFields)
                theList.Add(field.GetColumn());
            return theList;
        }

        internal string GetSqlCommandToDropFunction(string name)
        {
            return String.Format("IF EXISTS (SELECT  * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT')) DROP FUNCTION [dbo].[{0}] ", name);
        }

        internal string GetSqlCommandToDropFunctionForTblRowStatusRecords(int tblID)
        {
            return GetSqlCommandToDropFunction("WasActiveTblRowV" + tblID.ToString());
        }

        internal string GetSqlCommandToAddFunctionForTblRowStatusRecords(int tblID)
        {
            
            return String.Format("CREATE FUNCTION dbo.WasActiveTblRowV{0} ( @p1 DateTime ) RETURNS @TblRowIdsToReturn TABLE ( ID2 INT ) AS BEGIN INSERT INTO @TblRowIdsToReturn SELECT [ID] AS [ID2] FROM dbo.V{0} AS [t0] WHERE (((NOT ([t0].[RC] = 1)) AND ([t0].[DEL] = 0)) OR (([t0].[RC] = 1) AND ([t0].[DEL] = 0) AND (NOT (EXISTS( SELECT NULL AS [EMPTY] FROM [dbo].[TblRowStatusRecord] AS [t1] WHERE ([t1].[TimeChanged] > @p1) AND ([t1].[TblRowID] = [t0].[ID]))))) OR (((SELECT [t3].[Deleting] FROM ( SELECT TOP (1) [t2].[Deleting] FROM [dbo].[TblRowStatusRecord] AS [t2] WHERE ([t2].[TimeChanged] > @p1) AND ([t2].[TblRowID] = [t0].[ID]) ORDER BY [t2].[TimeChanged]) AS [t3])) = 1)) RETURN END", tblID);
        }


        internal string GetSqlCommandToDropFunctionForNearestNeighbors(string geoColName)
        {
            return GetSqlCommandToDropFunction("UDFNearestNeighborsFor" + geoColName);
        }

        internal string GetSqlCommandToAddFunctionForNearestNeighbors(int tblID, string geoColName)
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

        public void AddTable(IRaterooDataContext iDataContext)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            var columns = GetColumns();
            SQLTableDescription theTable = new SQLTableDescription() { Name = "V" + TheTbl.TblID, Columns = columns };
            SQLDirectManipulate.AddTable(dataContext, theTable);
            SQLDirectManipulate.AddIndicesForSpecifiedColumns(dataContext, theTable);
            SQLDirectManipulate.ExecuteSQL(dataContext, GetSqlCommandToDropFunctionForTblRowStatusRecords(TheTbl.TblID));
            SQLDirectManipulate.ExecuteSQL(dataContext, GetSqlCommandToAddFunctionForTblRowStatusRecords(TheTbl.TblID));
            var geocolumns = columns.Where(x => x.ColType == SQLColumnType.typeGeography);
            foreach (var geocol in geocolumns)
            {
                SQLDirectManipulate.ExecuteSQL(dataContext, GetSqlCommandToDropFunctionForNearestNeighbors(geocol.Name));
                SQLDirectManipulate.ExecuteSQL(dataContext, GetSqlCommandToAddFunctionForNearestNeighbors(TheTbl.TblID, geocol.Name));
            }

            foreach (var fieldMC in filterableMultipleChoiceFields)
            {
                SQLTableDescription subTable = fieldMC.GetSQLTableDescription();
                SQLDirectManipulate.AddTable(dataContext, subTable);
                SQLDirectManipulate.AddIndicesForSpecifiedColumns(dataContext, subTable);
            }
        }

        public void DropTable(IRaterooDataContext iDataContext)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            SQLDirectManipulate.DropTable(dataContext, "V" + TheTbl.TblID);
            foreach (var fieldMC in filterableMultipleChoiceFields)
            {
                SQLTableDescription subTable = fieldMC.GetSQLTableDescription();
                SQLDirectManipulate.DropTable(dataContext, subTable.Name);
            }
        }

        internal class FastRatingsInfo
        {
#pragma warning disable 0649
            public int ColumnID;
            public decimal? Value;
            public decimal? LastTrustedValue;
            public bool SingleNumber;
            public int DecimalPlaces;
            public int? RatingGroupID;
            public int? RatingID;
            public bool? RecentlyChanged;
#pragma warning restore 0649
        }

        internal class FastAddressFieldsInfo
        {
#pragma warning disable 0649
            public int FieldDefinitionID;
            public SqlGeography Geo;
            public decimal? Longitude;
            public decimal? Latitude;
            //public int FieldID; // rather than copy the Geo field, we will join AddressFields table, because of challenges of SQLGeography in Linq to SQL
#pragma warning restore 0649
        }

        internal class FastChoiceFieldsInfo
        {
#pragma warning disable 0649
            public int FieldDefinitionID;
            public bool MultipleChoices;
            public List<ChoiceInField> Choices;
#pragma warning restore 0649
        }

        internal class FastDateTimeFieldsInfo
        {
#pragma warning disable 0649
            public int FieldDefinitionID;
            public DateTime? DateTime;
#pragma warning restore 0649
        }

        internal class FastNumberFieldsInfo
        {
#pragma warning disable 0649
            public int FieldDefinitionID;
            public decimal? Number;
#pragma warning restore 0649
        }

        internal class FastTextFieldsInfo
        {
#pragma warning disable 0649
            public int FieldDefinitionID;
            public string Text;
#pragma warning restore 0649
        }

        internal class FastTextRowsInfo
        {
#pragma warning disable 0649
            public int ID;
            public string NME;
            internal string RF;
            internal string PF;
            public string RowHeading { get { return GetCombinedRowHeadingWithPopup(RF, PF, NME, ID); }  }
            public bool DEL;
            public int CNE;
            public decimal CUP;
            public bool ELEV;
            public bool HS;
            public bool RC;
            public IEnumerable<FastRatingsInfo> Ratings;
            public IEnumerable<FastAddressFieldsInfo> AddressFields;
            public IEnumerable<FastChoiceFieldsInfo> ChoiceFields;
            public IEnumerable<FastDateTimeFieldsInfo> DateTimeFields;
            public IEnumerable<FastNumberFieldsInfo> NumberFields;
            public IEnumerable<FastTextFieldsInfo> TextFields;
#pragma warning restore 0649

            internal static string GetCombinedRowHeadingWithPopup(string rowHeading, string popup, string name, int tblRowID)
            {
                bool popupIsEmpty = popup == null || popup.Contains("borderless nopop");
                string relAttr = popupIsEmpty ? "" : "rel=\"#FPU" + tblRowID.ToString() + "\"";
                string idAttr = popupIsEmpty ? "" : "id=\"FPU" + tblRowID.ToString() + "\"";
                string mainAreaOpening = String.Format("<div class=\"loadrowpopup\" title=\"{0}\" {1}>", name ?? "", relAttr);
                string closeDiv = "</div>"; 
                string popupOpening = String.Format("<div style=\"display:none;\" {0}>", idAttr);
                return mainAreaOpening + (rowHeading ?? "") + closeDiv + popupOpening + (popup ?? "") + closeDiv;
            }


            public void AddDataRow(DataTable dataTable)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["ID"] = ID;
                dataRow["NME"] = NME;
                dataRow["RH"] = RowHeading;
                dataRow["DEL"] = DEL;
                dataRow["CNE"] = CNE;
                dataRow["CUP"] = CUP;
                dataRow["ELEV"] = ELEV;
                dataRow["HS"] = HS;
                dataRow["RC"] = RC;
                foreach (var rating in Ratings)
                {
                    string theRatingString = "";
                    if (rating.SingleNumber)
                        theRatingString = PMNumberandTableFormatter.FormatAsSpecified(rating.Value, rating.DecimalPlaces, rating.ColumnID) + (rating.LastTrustedValue == rating.Value ? "" : "*");
                    else
                        theRatingString = PMTableLoading.GetHtmlStringForCell((int)rating.RatingGroupID, rating.Value, rating.DecimalPlaces, rating.ColumnID, rating.RatingID, null, rating.LastTrustedValue == rating.Value, rating.SingleNumber, false);
                    dataRow["RS" + rating.ColumnID.ToString()] = theRatingString;
                    dataRow["RV" + rating.ColumnID.ToString()] = (object)rating.Value ?? (object)System.DBNull.Value;
                    dataRow["R" + rating.ColumnID.ToString()] = (object)rating.RatingID ?? (object)System.DBNull.Value;
                    dataRow["RG" + rating.ColumnID.ToString()] = (object)rating.RatingGroupID ?? (object)System.DBNull.Value;
                    dataRow["RC" + rating.ColumnID.ToString()] = (object)rating.RecentlyChanged ?? (object)System.DBNull.Value;
                }
                foreach (var addressField in AddressFields)
                {
                    dataRow["F" + addressField.FieldDefinitionID.ToString()] = (object) addressField.Geo;
                }
                foreach (var choiceField in ChoiceFields)
                {
                    if (!choiceField.MultipleChoices)
                    {
                        dataRow["F" + choiceField.FieldDefinitionID.ToString()] = choiceField.Choices.FirstOrDefault() == null ? (object)System.DBNull.Value : (object)choiceField.Choices.FirstOrDefault().ChoiceInGroupID;
                    }
                }
                foreach (var dateTimeField in DateTimeFields)
                {
                    dataRow["F" + dateTimeField.FieldDefinitionID.ToString()] = dateTimeField.DateTime ?? (object)System.DBNull.Value;
                }
                foreach (var numberField in NumberFields)
                {
                    dataRow["F" + numberField.FieldDefinitionID.ToString()] = numberField.Number ?? (object)System.DBNull.Value;
                }
                foreach (var textField in TextFields)
                {
                    dataRow["F" + textField.FieldDefinitionID.ToString()] = textField.Text ?? (object)System.DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }

            public void AddDataRowsForMultipleChoiceField(FieldDefinition theFieldDefinition, DataTable dataTable)
            {
                var theChoices = ChoiceFields.Where(x => x.FieldDefinitionID == theFieldDefinition.FieldDefinitionID).SelectMany(y => y.Choices);
                foreach (var choice in theChoices)
                {
                    var dataRow = dataTable.NewRow();
                    // dataRow["MCID"] should be set automatically.
                    dataRow["CHO"] = choice.ChoiceInGroupID;
                    dataRow["TRID"] = ID; // the table row id
                    dataTable.Rows.Add(dataRow);
                };
            }

        }



        internal List<FastTextRowsInfo> storedRows;

        internal void StoreRowsInfo(IRaterooDataContext iDataContext, IQueryable<TblRow> tblRows, bool omitRatings = false, bool omitFields = false)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            if (omitRatings && omitFields)
            {
                storedRows = tblRows.Select(x => new FastTextRowsInfo
                {
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name,199),
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNE = x.CountNullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow > 0,
                    RC = x.StatusRecentlyChanged
                }).ToList();
            }
            else if (!omitRatings && !omitFields)
            {
                storedRows = tblRows.Select(x => new FastTextRowsInfo
                {
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name, 199),
                    RF = x.TblRowFieldDisplay.Row,
                    PF = x.TblRowFieldDisplay.PopUp,
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNE = x.CountNullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow > 0,
                    RC = x.StatusRecentlyChanged,
                    Ratings =   from y in x.RatingGroups
                                let firstRating = y.Ratings.OrderBy(z => z.NumInGroup).FirstOrDefault()
                                let singleNumber = y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || y.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber
                                where singleNumber || (y.RatingGroupAttribute.TypeOfRatingGroup != (int) RatingGroupTypes.hierarchyNumbersBelow && y.RatingGroupAttribute.TypeOfRatingGroup != (int) RatingGroupTypes.probabilityHierarchyBelow && y.RatingGroupAttribute.TypeOfRatingGroup != (int) RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy)
                                select new FastRatingsInfo
                                {
                                    ColumnID = y.TblColumnID,
                                    Value = firstRating == null ? null : firstRating.CurrentValue,
                                    LastTrustedValue = firstRating == null ? null : firstRating.LastTrustedValue,
                                    SingleNumber = (bool) singleNumber,
                                    RatingGroupID = y.RatingGroupID,
                                    DecimalPlaces = firstRating == null ? 0 : y.Ratings.First().RatingCharacteristic.DecimalPlaces,
                                    RatingID = firstRating == null || !singleNumber ? null : (int?)y.Ratings.First().RatingID, 
                                    RecentlyChanged = y.ValueRecentlyChanged
                                },
                    AddressFields = x.Fields.SelectMany(z => z.AddressFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastAddressFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Longitude = z.Longitude,
                            Latitude = z.Latitude,
                            Geo = (z.Latitude != null && z.Longitude != null && !(z.Latitude == 0 && z.Longitude == 0)) ? SqlGeography.Point((double) z.Latitude, (double) z.Longitude, 4326) : null
                            //FieldID = z.FieldID
                        }),
                    ChoiceFields = x.Fields.SelectMany(z => z.ChoiceFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastChoiceFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            MultipleChoices = z.Field.FieldDefinition.ChoiceGroupFieldDefinitions.First().ChoiceGroup.AllowMultipleSelections,
                            Choices = z.ChoiceInFields.Where(w => w.ChoiceInFieldID != null).ToList()
                        }),
                    DateTimeFields = x.Fields.SelectMany(z => z.DateTimeFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int) StatusOfObject.Active).Select(
                        z => new FastDateTimeFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            DateTime = z.DateTime
                        }),
                    NumberFields = x.Fields.SelectMany(z => z.NumberFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastNumberFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Number = z.Number
                        }),
                    TextFields = x.Fields.SelectMany(z => z.TextFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
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
                storedRows = tblRows.Select(x => new FastTextRowsInfo
                {
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name,199),
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNE = x.CountNullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow > 0,
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
                                  DecimalPlaces = firstRating == null ? 0 : y.Ratings.First().RatingCharacteristic.DecimalPlaces,
                                  RatingID = firstRating == null || !singleNumber ? null : (int?)y.Ratings.First().RatingID,
                                  RecentlyChanged = y.ValueRecentlyChanged
                              }
                }).ToList();
            }
            else if (omitRatings)
            {
                storedRows = tblRows.Select(x => new FastTextRowsInfo
                {
                    ID = x.TblRowID,
                    NME = MoreStrings.MoreStringManip.TruncateString(x.Name,199),
                    RF = x.TblRowFieldDisplay.Row,
                    PF = x.TblRowFieldDisplay.PopUp, 
                    DEL = x.Status == (int)(StatusOfObject.Unavailable),
                    CNE = x.CountNullEntries,
                    CUP = x.CountUserPoints,
                    ELEV = x.ElevateOnMostNeedsRating,
                    HS = x.CountHighStakesNow > 0,
                    RC = x.StatusRecentlyChanged,
                    AddressFields = x.Fields.SelectMany(z => z.AddressFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastAddressFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Longitude = z.Longitude,
                            Latitude = z.Latitude,
                            Geo = (z.Latitude != null && z.Longitude != null && !(z.Latitude == 0 && z.Longitude == 0)) ? SqlGeography.Point((double) z.Latitude, (double) z.Longitude, 4326) : null
                            //FieldID = z.FieldID
                        }),
                    ChoiceFields = x.Fields.SelectMany(z => z.ChoiceFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastChoiceFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            MultipleChoices = z.Field.FieldDefinition.ChoiceGroupFieldDefinitions.First().ChoiceGroup.AllowMultipleSelections,
                            Choices = z.ChoiceInFields.Where(w => w.ChoiceInFieldID != null).ToList()
                        }),
                    DateTimeFields = x.Fields.SelectMany(z => z.DateTimeFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastDateTimeFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            DateTime = z.DateTime
                        }),
                    NumberFields = x.Fields.SelectMany(z => z.NumberFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastNumberFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Number = z.Number
                        }),
                    TextFields = x.Fields.SelectMany(z => z.TextFields).Where(w => w.Field.FieldDefinition.UseAsFilter && w.Status == (int)StatusOfObject.Active).Select(
                        z => new FastTextFieldsInfo
                        {
                            FieldDefinitionID = z.Field.FieldDefinitionID,
                            Text = z.Text
                        })
                }
                ).ToList();
            }
        }

        internal DataTable CreateDataTable(IRaterooDataContext iDataContext, IQueryable<TblRow> tblRows)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            // assumes that row info has already been stored. 

            // Create a new DataTable.
            System.Data.DataTable dataTable = new DataTable("ParentTable");

            SQLTableDescription theTable = new SQLTableDescription() { Name = "V" + TheTbl.TblID, Columns = GetColumns() };
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

        internal DataTable CreateDataTableForMultipleChoiceField(IRaterooDataContext iDataContext, SQLMultipleChoiceFieldTableInfo subtableInfo)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
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



        public int BulkCopyRows(IRaterooDataContext iDataContext, IQueryable<TblRow> tblRows)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return 0;

            StoreRowsInfo(iDataContext, tblRows, false, false);

            DataTable theDataTable = CreateDataTable(iDataContext, tblRows);
            SqlBulkCopy bulk = new SqlBulkCopy(AzureSetup.GetConfigurationSetting("RaterooConnectionString"), SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.UseInternalTransaction);
            bulk.DestinationTableName = "dbo.V" + TheTbl.TblID.ToString();
            bulk.WriteToServer(theDataTable);
            int numRecords = theDataTable.Rows.Count;

            // now, bulk copy data to the separate table created for each multiple choice field (if any)
            foreach (var fieldMC in filterableMultipleChoiceFields)
            {
                theDataTable = CreateDataTableForMultipleChoiceField(iDataContext, fieldMC);
                SqlBulkCopy bulk2 = new SqlBulkCopy(AzureSetup.GetConfigurationSetting("RaterooConnectionString"));
                bulk2.ColumnMappings.Add(new SqlBulkCopyColumnMapping("TRID", "TRID"));
                bulk2.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CHO", "CHO"));
                bulk2.DestinationTableName = "dbo.VFMC" + fieldMC.TheFieldDefinition.FieldDefinitionID.ToString();
                bulk2.WriteToServer(theDataTable);
            }

            return numRecords;
            
        }

        internal class FastUpdateGeography
        {
            public decimal Longitude { get; set; }
            public decimal Latitude { get; set; }
        }

        internal class FastUpdateVariableInfo
        {
            public string varname {get; set;}
            public object value {get; set;}
            public SqlDbType dbtype { get; set; }
        }

        internal class FastUpdateRowInfo
        {
            public List<FastUpdateVariableInfo> Variables {get; set;}

            public FastUpdateRowInfo()
            {
                Variables = new List<FastUpdateVariableInfo>();
            }

            public void Add(string varname, object value, SqlDbType dbtype)
            {
                Variables.Add(new FastUpdateVariableInfo { varname = varname, value = value, dbtype = dbtype });
            }

            public void DoUpdate(IRaterooDataContext iDataContext, string tableName)
            {

                RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
                if (dataContext == null)
                    return;

                // step 1: formulate the SQL statement
                if (!Variables.Any())
                    return;


                string updateString =
                  "UPDATE " + tableName + " " + "SET ";
                bool isFirst = true;
                int varNum = 0;
                foreach (var variable in Variables)
                {
                    if (variable.varname != "ID")
                    {
                        if (!isFirst)
                            updateString += ", ";
                        isFirst = false;
                        //updateString += variable.varname + " = @" + variable.varname;
                        updateString += variable.varname;
                        if (variable.value == null)
                            updateString += " = NULL";
                        else
                        {
                            if (variable.value is FastUpdateGeography)
                            {
                                FastUpdateGeography fastUpdateGeo = ((FastUpdateGeography)variable.value);
                                int longVarNum = varNum;
                                varNum++;
                                int latVarNum = varNum;
                                varNum++;
                                updateString += " = geography::STPointFromText('POINT(' + CAST({" + longVarNum + "} AS VARCHAR(20)) + ' ' + CAST({" + latVarNum + "} AS VARCHAR(20)) + ')', 4326)";
                            }
                            else
                            {
                                updateString += " = {" + varNum.ToString() + "}";
                                varNum++;
                            }
                        }
                    }
                }
                updateString += " WHERE ID = {" + varNum.ToString() + "}";

                Object[] parameters  = Variables.Where(x => x.value != null).OrderBy(x => x.varname == "ID").Select(x => x.value).ToArray(); // put ID field last, and exclude null fields, because ExecuteCommand cannot handle null parameters (despite documentation to the contrary)
                Object[] parameters2 = ConvertGeographyParametersIntoSeparateLongitudeAndLatitudeParameters(parameters);

                // execute the command
                dataContext.ExecuteCommand(updateString, parameters2);
            }

            private static Object[] ConvertGeographyParametersIntoSeparateLongitudeAndLatitudeParameters(Object[] parameters)
            {
                Object[] parameters2 = null;
                if (parameters.Any(x => x is FastUpdateGeography))
                {
                    int numGeo = parameters.Count(x => x is FastUpdateGeography);
                    parameters2 = new Object[parameters.Length + numGeo];
                    int indexInNewParameters = 0;
                    for (int indexInOldParameters = 0; indexInOldParameters < parameters.Length; indexInOldParameters++)
                    {
                        if (parameters[indexInOldParameters] is FastUpdateGeography)
                        {
                            parameters2[indexInNewParameters] = ((FastUpdateGeography)parameters[indexInOldParameters]).Longitude;
                            indexInNewParameters++;
                            parameters2[indexInNewParameters] = ((FastUpdateGeography)parameters[indexInOldParameters]).Latitude;
                            indexInNewParameters++;
                        }
                        else
                        {
                            parameters2[indexInNewParameters] = parameters[indexInOldParameters];
                            indexInNewParameters++;
                        }
                    }
                }
                else
                    parameters2 = parameters;
                return parameters2;
            }
        }

        internal class FastUpdateRowsInfo
        {
            public string TableName { get; set; }
            public List<FastUpdateRowInfo> Rows {get; set;}

            public void DoUpdate(IRaterooDataContext iDataContext)
            {

                RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
                if (dataContext == null)
                    return;

                foreach (var row in Rows)
                    row.DoUpdate(iDataContext, TableName);
            }
        }

        public void UpdateRows(IRaterooDataContext iDataContext, IQueryable<TblRow> tblRows, bool updateRatings = true, bool updateFields = true)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            StoreRowsInfo(iDataContext, tblRows, !updateRatings, !updateFields);
            FastUpdateRowsInfo allRows = GenerateFastUpdateRowsInfoForMainTable(updateRatings, updateFields);
            if (updateFields)
                GenerateChangesForMCTables(iDataContext);
            allRows.DoUpdate(iDataContext);
        }

        internal void GenerateChangesForMCTables(IRaterooDataContext iDataContext)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            foreach (var storedRow in storedRows)
            {
                foreach (var choiceField in storedRow.ChoiceFields)
                {
                    if (choiceField.MultipleChoices)
                    {
                        DeleteExistingChoicesInMCTable(iDataContext, choiceField.FieldDefinitionID, storedRow.ID);
                        AddChoicesToMCTable(iDataContext, choiceField.FieldDefinitionID, storedRow.ID, choiceField.Choices.Select(x => x.ChoiceInGroupID).ToList());
                    }
                }
            }

        }

        internal void DeleteExistingChoicesInMCTable(IRaterooDataContext iDataContext, int fieldDefinitionID, int tblRowID)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            string tableName = "dbo.VFMC" + fieldDefinitionID.ToString();
            string deleteString = "DELETE FROM " + tableName + " WHERE TRID = {0} ";
            dataContext.ExecuteCommand(deleteString, new object[] { tblRowID });
        }

        internal void AddChoicesToMCTable(IRaterooDataContext iDataContext, int fieldDefinitionID, int tblRowID, List<int> choiceInGroupIDs)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            string tableName = "dbo.VFMC" + fieldDefinitionID.ToString();
            foreach (int choiceInGroupID in choiceInGroupIDs)
            {
                string insertString = "INSERT INTO " + tableName + " (TRID, CHO) VALUES ({0}, {1}) ";
                dataContext.ExecuteCommand(insertString, new object[] { tblRowID, choiceInGroupID });
            }
        }

        internal FastUpdateRowsInfo GenerateFastUpdateRowsInfoForMainTable(bool updateRatings, bool updateFields)
        {
            List<FastUpdateRowInfo> rows = new List<FastUpdateRowInfo>();
            foreach (var storedRow in storedRows)
            {
                FastUpdateRowInfo rowInfo = new FastUpdateRowInfo();

                rowInfo.Add("ID", storedRow.ID, SqlDbType.Int);
                rowInfo.Add("NME", storedRow.NME, SqlDbType.NVarChar);
                rowInfo.Add("DEL", storedRow.DEL.ToString(), SqlDbType.Bit);
                rowInfo.Add("CNE", storedRow.CNE.ToString(), SqlDbType.Int);
                rowInfo.Add("CUP", storedRow.CUP.ToString(), SqlDbType.Decimal);
                rowInfo.Add("ELEV", storedRow.ELEV.ToString(), SqlDbType.Bit);
                rowInfo.Add("HS", storedRow.HS.ToString(), SqlDbType.Bit);
                rowInfo.Add("RC", storedRow.RC.ToString(), SqlDbType.Bit);
                if (updateFields)
                {
                    rowInfo.Add("RH", storedRow.RowHeading, SqlDbType.NVarChar);
                    foreach (var field in storedRow.AddressFields)
                    {
                        rowInfo.Add("F" + field.FieldDefinitionID.ToString(), (field.Latitude == null || field.Longitude == null || (field.Longitude == 0 && field.Latitude == 0)) ? null : new FastUpdateGeography { Longitude = (decimal) field.Longitude, Latitude = (decimal) field.Latitude }, SqlDbType.Udt);
                    }
                    foreach (var field in storedRow.ChoiceFields)
                    {
                        if (!field.MultipleChoices)
                        {
                            rowInfo.Add("F" + field.FieldDefinitionID.ToString(), field.Choices.First().ChoiceInGroupID, SqlDbType.Int);
                        }
                    }
                    foreach (var field in storedRow.DateTimeFields)
                    {
                        rowInfo.Add("F" + field.FieldDefinitionID.ToString(), field.DateTime, SqlDbType.DateTime);
                    }
                    foreach (var field in storedRow.NumberFields)
                    {
                        rowInfo.Add("F" + field.FieldDefinitionID.ToString(), field.Number, SqlDbType.Decimal);
                    }
                    foreach (var field in storedRow.TextFields)
                    {
                        rowInfo.Add("F" + field.FieldDefinitionID.ToString(), field.Text, SqlDbType.NVarChar);
                    }
                }
                if (updateRatings)
                {
                    foreach (var rating in storedRow.Ratings)
                    {
                        if (rating.SingleNumber)
                            rowInfo.Add("RS" + rating.ColumnID.ToString(), PMNumberandTableFormatter.FormatAsSpecified(rating.Value, rating.DecimalPlaces, rating.ColumnID) + (rating.LastTrustedValue == rating.Value ? "" : "*"), SqlDbType.NVarChar);
                        else
                            rowInfo.Add("RS" + rating.ColumnID.ToString(), PMTableLoading.GetHtmlStringForCell((int)rating.RatingGroupID, rating.Value, rating.DecimalPlaces, rating.ColumnID, rating.RatingID, null, rating.LastTrustedValue == rating.Value, rating.SingleNumber, false), SqlDbType.NVarChar);
                        rowInfo.Add("RV" + rating.ColumnID.ToString(), rating.Value, SqlDbType.Decimal);
                        rowInfo.Add("RC" + rating.ColumnID.ToString(), rating.RecentlyChanged, SqlDbType.Bit);
                        //rowInfo.Add("R" + rating.ColumnID.ToString(), rating.RatingID); doesn't change -- no need to update
                        //rowInfo.Add("RG" + rating.ColumnID.ToString(), rating.RatingGroupID); doesn't change -- no need to update
                    }
                }

                rows.Add(rowInfo);
            }
            FastUpdateRowsInfo allRows = new FastUpdateRowsInfo { TableName = "dbo.V" + TheTbl.TblID.ToString(), Rows = rows };
            return allRows;
        }
    }

    enum FastAccessTableStatus
    {
        bulkCopyNotStarted,
        bulkCopyInProgress,
        bulkCopyCompleted
    }

    public static class SQLFastAccess
    {
        public static int CountHighestRecord(IRaterooDataContext iDataContext, string tableName)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return 0;

            IEnumerable<int> result = dataContext.ExecuteQuery<int>(String.Format("SELECT TOP 1 [ID] FROM [dbo].[{0}] ORDER BY [ID] DESC", tableName)).ToList();
            if (result.Count() == 0)
                return 0;
            else
                return result.First();
        }

        public static bool ContinueFastAccessMaintenance(IRaterooDataContext iDataContext)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null || !RoleEnvironment.IsAvailable)
                return false;

            bool moreBulkCopyingToDo = ContinueBulkCopy(iDataContext);
            bool moreUpdatingToDo = true;
            if (!moreBulkCopyingToDo)
                moreUpdatingToDo = ContinueUpdate(iDataContext);

            return moreBulkCopyingToDo || moreUpdatingToDo;
        }

        internal static bool ContinueBulkCopy(IRaterooDataContext iDataContext)
        {

            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;

            if (!FastAccessTablesEnabled())
                return false; // not enabled, so no more work to do.
            Tbl theTbl = iDataContext.GetTable<Tbl>().FirstOrDefault(x => x.FastTableSyncStatus == (int) FastAccessTableStatus.bulkCopyNotStarted || x.FastTableSyncStatus == (int) FastAccessTableStatus.bulkCopyInProgress);
            if (theTbl == null)
                return false;

            if (theTbl.FastTableSyncStatus == (int) FastAccessTableStatus.bulkCopyNotStarted)
            {
                new SQLFastAccessTableInfo(iDataContext, theTbl).DropTable(iDataContext);
                new SQLFastAccessTableInfo(iDataContext, theTbl).AddTable(iDataContext);
                theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.bulkCopyInProgress;
                PMCacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
                dataContext.SubmitChanges();
            }

            const int numToTake = 500;
            int virtualTableHighestID = CountHighestRecord(iDataContext, "V" + theTbl.TblID.ToString());
            IQueryable<TblRow> tblRows = iDataContext.GetTable<TblRow>().Where(x => x.TblID == theTbl.TblID && x.TblRowID > virtualTableHighestID).OrderBy(x => x.TblRowID).Take(numToTake);
            TblRow lastTblRow = tblRows.OrderByDescending(x => x.TblRowID).FirstOrDefault();
            int lastTableRowID = 0;
            if (lastTblRow != null)
                lastTableRowID = lastTblRow.TblRowID;
            int numRecords = 0;
            if (lastTableRowID > 0)
                numRecords = new SQLFastAccessTableInfo(iDataContext, theTbl).BulkCopyRows(iDataContext, tblRows);
            System.Diagnostics.Trace.TraceInformation("Bulk copy from " + theTbl.Name + " to V" + theTbl.TblID.ToString() + " " + numRecords.ToString() + " records");

            if (numRecords == 0)
            {
                theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.bulkCopyCompleted;
                PMCacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
                dataContext.SubmitChanges();
                RaterooDataContext newDataContext = GetIRaterooDataContext.New(true, true).GetRealDatabaseIfExists();
                TblRow lastTblRow2 = newDataContext.GetTable<TblRow>().Where(x => x.TblID == theTbl.TblID && x.TblRowID > virtualTableHighestID).OrderByDescending(x => x.TblRowID).FirstOrDefault();
                int lastTableRowID2 = 0;
                if (lastTblRow2 != null)
                    lastTableRowID2 = lastTblRow2.TblRowID;
                if (lastTableRowID != lastTableRowID2)
                { // looks like a row was added while we ran this, so we must change the table status back
                    Tbl theTbl2 = newDataContext.GetTable<Tbl>().Single(t => t.TblID == theTbl.TblID);
                    theTbl2.FastTableSyncStatus = (int)FastAccessTableStatus.bulkCopyInProgress;
                    newDataContext.SubmitChanges();
                }
            }


            return true;
        }

        [Serializable]
        internal class RowRequiringUpdate : IComparer
        {
            public string TableName { get; set; }
            public int TblRowID { get; set; }
            public bool UpdateRatings { get; set; }
            public bool UpdateFields { get; set; }

            public RowRequiringUpdate(string tableName, int tblRowID, bool updateRatings, bool updateFields)
            {
                TableName = tableName;
                TblRowID = tblRowID;
                UpdateRatings = updateRatings;
                UpdateFields = updateFields;
            }

            public int Compare(object x, object y)
            {
                RowRequiringUpdate x1 = x as RowRequiringUpdate;
                RowRequiringUpdate y1 = y as RowRequiringUpdate;
                return String.Compare(x1.TableName + x1.TblRowID.ToString() + x1.UpdateRatings.ToString() + x1.UpdateFields.ToString(), y1.TableName + y1.TblRowID.ToString() + y1.UpdateRatings.ToString() + y1.UpdateFields.ToString());
            }

        }

        [Serializable]
        internal class RowsRequiringUpdate
        {
            public List<RowRequiringUpdate> Rows { get; set; }
        }

        public static void IdentifyRowRequiringUpdate(IRaterooDataContext iDataContext, Tbl theTbl, int tblRowID, bool updateRatings, bool updateFields)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            RowRequiringUpdate row = new RowRequiringUpdate("V" + theTbl.TblID.ToString(), tblRowID, updateRatings, updateFields);
            List<RowRequiringUpdate> theRowsRequiringUpdates = iDataContext.TempCacheGet("fasttablerowupdate") as List<RowRequiringUpdate>;
            if (theRowsRequiringUpdates == null)
                theRowsRequiringUpdates = new List<RowRequiringUpdate>();
            if (!theRowsRequiringUpdates.Any(x => x.TableName == "V" + theTbl.TblID.ToString() && x.TblRowID == tblRowID && x.UpdateFields == updateFields && x.UpdateRatings == updateRatings))
                theRowsRequiringUpdates.Add(row);
            iDataContext.TempCacheAdd("fasttablerowupdate", theRowsRequiringUpdates);
        }

        public static void PushRowsRequiringUpdateToAzureQueue(IRaterooDataContext iDataContext)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            List<RowRequiringUpdate> theRowsRequiringUpdates = iDataContext.TempCacheGet("fasttablerowupdate") as List<RowRequiringUpdate>;
            if (theRowsRequiringUpdates != null)
            {
                RowsRequiringUpdate theRows = new RowsRequiringUpdate { Rows = theRowsRequiringUpdates.ToList() };
                AzureQueue.Push("fasttablerowupdate", theRows );
            }
            iDataContext.TempCacheAdd("fasttablerowupdate", null);
        }

        internal static bool ContinueUpdate(IRaterooDataContext iDataContext)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return false;
            const int numAtOnce = 100;
            var queue = new AzureQueueWithErrorRecovery(10, RebuildTableBecauseOfFailedUpdateBasedOnCloudMessage);
            List<RowRequiringUpdate> theRows = new List<RowRequiringUpdate>();
            var theMessages = queue.GetMessages("fasttablerowupdate", numAtOnce);
            foreach (var setOfRows in theMessages)
                theRows.AddRange(((RowsRequiringUpdate)setOfRows).Rows);
            if (!FastAccessTablesEnabled())
            {
                queue.ConfirmProperExecution("fasttablerowupdate");
                return theRows.Any();
            }
            var theRowsByTableAndUpdateInstruction = theRows.Select(x => new {Item = x, GroupByInstruct = x.TableName + x.UpdateFields.ToString() + x.UpdateRatings.ToString()}).GroupBy(x => x.GroupByInstruct);
            bool anyNewRowsAdded = false;
            foreach (var table in theRowsByTableAndUpdateInstruction)
            {
                int tblID = Convert.ToInt32(table.First().Item.TableName.Substring(1));
                Tbl theTbl = iDataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);
                bool newRowsAdded = table.Any(x => x.Item.TblRowID == 0); // we can't use update for this, because we don't know the TblRowID yet.
                List<int> tblRowIDs = table.Select(x => x.Item.TblRowID).OrderBy(x => x).Distinct().ToList();
                IQueryable<TblRow> tblRows = iDataContext.GetTable<TblRow>().Where(x => tblRowIDs.Contains(x.TblRowID));
                new SQLFastAccessTableInfo(iDataContext, theTbl).UpdateRows(iDataContext, tblRows, table.First().Item.UpdateRatings, table.First().Item.UpdateFields);
                if (newRowsAdded && theTbl.FastTableSyncStatus == (int)FastAccessTableStatus.bulkCopyCompleted)
                {
                    theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.bulkCopyInProgress;
                    PMCacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
                    dataContext.SubmitChanges();
                    anyNewRowsAdded = true;
                }
            }
            queue.ConfirmProperExecution("fasttablerowupdate");
            if (anyNewRowsAdded)
                ContinueBulkCopy(iDataContext);
            return theRows.Count() == numAtOnce; // more work to do
        }

        internal static void RebuildTableBecauseOfFailedUpdateBasedOnCloudMessage(object rowRequiringUpdateObject)
        {
            RowRequiringUpdate row = (RowRequiringUpdate)rowRequiringUpdateObject;
            int tblID = Convert.ToInt32(row.TableName.Substring(1));
            IRaterooDataContext iDataContext = GetIRaterooDataContext.New(true, true);
            if (iDataContext.GetRealDatabaseIfExists() == null)
                return;
            Tbl theTbl = iDataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);
            RebuildTableBecauseOfFailedUpdate(iDataContext, theTbl);
        }

        public static void RebuildTableBecauseOfFailedUpdate(IRaterooDataContext iDataContext, Tbl theTbl)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.bulkCopyNotStarted;
            PMCacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
            iDataContext.SubmitChanges();
            new SQLFastAccessTableInfo(iDataContext, theTbl).DropTable(iDataContext);
        }

        public static void PlanDropTbl(IRaterooDataContext iDataContext, Tbl theTbl)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            theTbl.FastTableSyncStatus = (int)FastAccessTableStatus.bulkCopyNotStarted;
            PMCacheManagement.InvalidateCacheDependency("TblID" + theTbl.TblID);
            dataContext.SubmitChanges(); // fast access table will be dropped
        }

        public static void PlanDropTbls(IRaterooDataContext iDataContext, PointsManager pointsManager)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            foreach (var tbl in pointsManager.Tbls)
                PlanDropTbl(iDataContext, tbl);
        }

        internal class DoQueryResults
        {
            public List<InfoForBodyRows> bodyRowList; 
            public int? rowCount;

        }

        internal static List<string> GetListOfFastAccessVariablesToLoad(List<TblColumn> theTblColumns, bool includeRowHeader)
        {
            List<string> allColumns = new List<string>();
            allColumns.Add("ID"); 
            allColumns.Add("NME");
            allColumns.Add("DEL");
            allColumns.Add("CNE");
            allColumns.Add("CUP");
            allColumns.Add("ELEV");
            allColumns.Add("HS");
            allColumns.Add("RC");
            if (includeRowHeader)
            {
                allColumns.Add("RH");
            }
            foreach (var col in theTblColumns)
            {
                string colString = col.TblColumnID.ToString();
                allColumns.Add("RS" + colString);
                //allColumns.Add("RV" + colString);
                allColumns.Add("R" + colString);
                allColumns.Add("RG" + colString);
                allColumns.Add("RC" + colString);
            }
            return allColumns;
        }

        internal static void GetOrderBy(IRaterooDataContext iDataContext, TableSortRule tableSortRule, int tblID, int originalTblIndex, ref int sqlTblIndex, DateTime asOfDateTime, ref int paramNumber, List<SqlParameter> parameters, out string orderByString, out string joinString)
        {
            joinString = "";
            orderByString = "";
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;
            joinString = " ";
            string ascOrDescString = (tableSortRule.Ascending) ? "ASC" : "DESC";
            if (tableSortRule is TableSortRuleEntityName)
                orderByString = "ORDER BY NME " + ascOrDescString;
            else if (tableSortRule is TableSortRuleNewestInDatabase)
                orderByString = "ORDER BY ID " + ascOrDescString;
            else if (tableSortRule is TableSortRuleNeedsRating)
                orderByString = String.Format("ORDER BY [t{0}].[DEL], [t{0}].[ELEV] DESC, [t{0}].[CNE] DESC, [t{0}].[CUP]", sqlTblIndex); 
            else if (tableSortRule is TableSortRuleNeedsRatingUntrustedUser)
                orderByString = String.Format("ORDER BY [t{0}].[DEL], [t{0}].[CNE] DESC, [t{0}].[CUP]", sqlTblIndex);
            else if (tableSortRule is TableSortRuleTblColumn)
            {
                TableSortRuleTblColumn tableSortRuleTblColumn = (TableSortRuleTblColumn)tableSortRule;
                if (!StatusRecords.RatingGroupStatusRecordsExistSince(iDataContext, asOfDateTime))
                { // no need to bother correcting for changes in sorting
                    orderByString = String.Format("ORDER BY [t{0}].[RV{1}] {2}", sqlTblIndex, tableSortRuleTblColumn.TblColumnToSortID, ascOrDescString);
                }
                else
                { // must correct for changes in sorting, so that we recover the order as of the time specified. that way, when scrolling, we don't get repeats or skips.
                    sqlTblIndex++;
                    int subtableIndex1 = sqlTblIndex;
                    sqlTblIndex++;
                    int subtableIndex2 = sqlTblIndex;
                    sqlTblIndex++;
                    int subtableIndex3 = sqlTblIndex;
                    int asOfDateTimeParamNum = paramNumber;
                    parameters.Add(new SqlParameter("P" + asOfDateTimeParamNum.ToString(), asOfDateTime));
                    paramNumber++;
                    orderByString = String.Format(@"ORDER BY 
        (CASE 
            WHEN NOT ([t{0}].[RC{4}] = 1) THEN [t{0}].[RV{4}]
            WHEN EXISTS(
                SELECT NULL AS [EMPTY]
                FROM [RatingGroupStatusRecords] AS [t{1}]
                WHERE (([t{1}].[RatingGroupID]) = [t{0}].[RG{4}]) AND ([t{1}].[NewValueTime] > @p{6})
                ) THEN (
                SELECT [t{3}].[OldValueOfFirstRating]
                FROM (
                    SELECT TOP (1) [t{2}].[OldValueOfFirstRating]
                    FROM [RatingGroupStatusRecords] AS [t{2}]
                    WHERE (([t{2}].[RatingGroupID]) = [t{0}].[RG{4}]) AND ([t{2}].[NewValueTime] > @p{6})
                    ) AS [t{3}]
                )
            ELSE [t{0}].[RV{4}]
         END) {5}, [t{0}].[NME]", originalTblIndex, subtableIndex1, subtableIndex2, subtableIndex3, tableSortRuleTblColumn.TblColumnToSortID, ascOrDescString, asOfDateTimeParamNum);
                };
            }
            else if (tableSortRule is TableSortRuleActivityLevel)
            {
                TableSortRuleActivityLevel activityLevelSortRule = (TableSortRuleActivityLevel)tableSortRule;
                sqlTblIndex++;
                int volatilityIndex = sqlTblIndex;
                orderByString = String.Format(" ORDER BY (SELECT [t{1}].[Volatility] FROM [dbo].[VolatilityTblRowTrackers] AS [t{1}] WHERE ([t{1}].[DurationType] = {2}) AND ([t{1}].[TblRowID] = [t{0}].[ID])) {3} ", originalTblIndex, volatilityIndex, (int)activityLevelSortRule.TimeFrame, ascOrDescString);
            }
            else if (tableSortRule is TableSortRuleDistance)
            {
                TableSortRuleDistance distanceSortRule = (TableSortRuleDistance)tableSortRule;
                sqlTblIndex++;
                int nearestNeighborsIndex = sqlTblIndex;
                joinString = String.Format(" INNER JOIN [dbo].[UDFNearestNeighborsFor{0}]({1}, {2}, {3}) AS [t{5}] ON ([t{4}].[ID]) = [t{5}].[TblRowID] ", "F" + distanceSortRule.FieldDefinitionID.ToString(), distanceSortRule.Latitude, distanceSortRule.Longitude, 1000, originalTblIndex, nearestNeighborsIndex);
                orderByString = String.Format(" ORDER BY [t{0}].TblRowID {1}", nearestNeighborsIndex, ascOrDescString);
            }
            else
                throw new NotImplementedException();
        }

        internal class ChoiceFullFieldDefinition
        {
            public FieldDefinition fd { get; set; }
            public ChoiceGroupFieldDefinition cgfd { get; set; }
            public ChoiceGroup cg { get; set; }
        }

        internal static List<ChoiceFullFieldDefinition> GetChoiceFullFieldDefinitions(IRaterooDataContext iDataContext, Tbl tbl)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "ChoiceFullFieldDefinition" + tbl.TblID.ToString();
            List<ChoiceFullFieldDefinition> fd = PMCacheManagement.GetItemFromCache(cacheString) as List<ChoiceFullFieldDefinition>;
            if (fd == null)
            {
                fd = iDataContext.GetTable<ChoiceGroupFieldDefinition>().Where(x => x.FieldDefinition.TblID == tbl.TblID && x.Status == (int)StatusOfObject.Active).Select(x => new ChoiceFullFieldDefinition { fd = x.FieldDefinition, cgfd = x, cg = x.ChoiceGroup }).ToList();
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }

        internal static List<FieldDefinition> GetAddressFieldDefinitions(IRaterooDataContext iDataContext, Tbl tbl)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "AddressFieldDefinition" + tbl.TblID.ToString();
            List<FieldDefinition> fd = PMCacheManagement.GetItemFromCache(cacheString) as List<FieldDefinition>;
            if (fd == null)
            {
                fd = iDataContext.GetTable<FieldDefinition>().Where(x => x.TblID == tbl.TblID && x.FieldType == (int) FieldTypes.AddressField && x.Status == (int)StatusOfObject.Active).ToList();
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }

        internal static FieldDefinition GetFieldDefinition(IRaterooDataContext iDataContext, int fieldDefinitionID)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "FieldDefinition" + fieldDefinitionID.ToString();
            FieldDefinition fd = PMCacheManagement.GetItemFromCache(cacheString) as FieldDefinition;
            if (fd == null)
            {
                fd = iDataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == fieldDefinitionID);
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + fd.Tbl.PointsManagerID.ToString() } , fd, new TimeSpan(1,0,0) );
            }
            return fd;
        }


        internal static ChoiceGroupFieldDefinition GetChoiceGroupFieldDefinition(IRaterooDataContext iDataContext, int fieldDefinitionID)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            string cacheString = "FieldDefinition" + fieldDefinitionID.ToString();
            ChoiceGroupFieldDefinition fd = PMCacheManagement.GetItemFromCache(cacheString) as ChoiceGroupFieldDefinition;
            if (fd == null)
            {
                fd = iDataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == fieldDefinitionID);
                PMCacheManagement.AddItemToCache(cacheString, new string[] { "FieldInfoForPointsManagerID" + fd.FieldDefinition.Tbl.PointsManagerID.ToString() }, fd, new TimeSpan(1, 0, 0));
            }
            return fd;
        }

        internal static void GetStatementsForFiltering(FilterRules filters, List<ChoiceFullFieldDefinition> choiceFDs, int originalSqlTableIndex, ref int sqlTableIndex, DateTime? asOfDateTime, int tblID, ref int paramNumber, ref List<SqlParameter> parameters, out string whereString, out string joinString)
        {
            joinString = " ";
            List<string> whereClauses = new List<string>();
            foreach (var filter in filters.theFilterRules.Where(x => x.FilterOn))
            {
                if (filter is DateTimeFilterRule)
                {
                    DateTimeFilterRule dateTimeFilter = ((DateTimeFilterRule)filter);
                    if (dateTimeFilter.MinValue != null)
                    {
                        whereClauses.Add("F" + dateTimeFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (DateTime)dateTimeFilter.MinValue));
                        paramNumber++;
                    }

                    if (dateTimeFilter.MaxValue != null)
                    {
                        whereClauses.Add("F" + dateTimeFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (DateTime)dateTimeFilter.MaxValue));
                        paramNumber++;
                    }
                }
                else if (filter is NumberFilterRule)
                {
                    NumberFilterRule NumberFilter = ((NumberFilterRule)filter);
                    if (NumberFilter.MinValue != null)
                    {
                        whereClauses.Add("F" + NumberFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)NumberFilter.MinValue));
                        paramNumber++;
                    }

                    if (NumberFilter.MaxValue != null)
                    {
                        whereClauses.Add("F" + NumberFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)NumberFilter.MaxValue));
                        paramNumber++;
                    }
                }

                else if (filter is TextFilterRule)
                {
                    TextFilterRule TextFilter = ((TextFilterRule)filter);
                    if (TextFilter.FromText != null && TextFilter.FromText != "")
                    {
                        whereClauses.Add("F" + TextFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), TextFilter.FromText));
                        paramNumber++;
                    }

                    if (TextFilter.ToText != null)
                    {
                        whereClauses.Add("F" + TextFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), TextFilter.ToText));
                        paramNumber++;
                    }
                }
                else if (filter is ChoiceFilterRule)
                {
                    ChoiceFilterRule ChoiceFilter = (ChoiceFilterRule)filter;
                    ChoiceFullFieldDefinition theFD = choiceFDs.Single(x => x.fd.FieldDefinitionID == ChoiceFilter.theID);
                    if (theFD.cg.AllowMultipleSelections)
                    {
                        string thisTbl = "T" + theFD.fd.TblID.ToString();
                        string mcTbl = "VFMC" + theFD.fd.FieldDefinitionID.ToString();
                        joinString += " INNER JOIN " + mcTbl + " ON ID=" + mcTbl + ".TRID " + " AND " + mcTbl + ".CHO=" + "@P" + paramNumber.ToString() + " ";
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), ChoiceFilter.ChoiceInGroupID));
                        paramNumber++;
                    }
                    else
                    {
                        whereClauses.Add("F" + ChoiceFilter.theID.ToString() + " = @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), ChoiceFilter.ChoiceInGroupID));
                        paramNumber++;
                    }
                }
                else if (filter is TblColumnFilterRule)
                {
                    TblColumnFilterRule ColumnFilter = ((TblColumnFilterRule)filter);
                    if (ColumnFilter.MinValue != null)
                    {
                        whereClauses.Add("RV" + ColumnFilter.theID.ToString() + " >= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)ColumnFilter.MinValue));
                        paramNumber++;
                    }

                    if (ColumnFilter.MaxValue != null)
                    {
                        whereClauses.Add("RV" + ColumnFilter.theID.ToString() + " <= @P" + paramNumber.ToString());
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)ColumnFilter.MaxValue));
                        paramNumber++;
                    }
                }
                else if (filter is AddressFilterRule)
                {
                    AddressFilterRule AddressFilter = (AddressFilterRule)filter;
                    Coordinate ObjCod = new Coordinate();
                    ObjCod = Geocode.GetCoordinates(AddressFilter.Address);
                    float Latitude1 = (float)Math.Round(ObjCod.Latitude, 4);
                    float Longitude1 = (float)Math.Round(ObjCod.Longitude, 4);
                    int paramNum1 = 0, paramNum2 = 0, paramNum3 = 0;
                    if (!(Latitude1 == 0 && Longitude1 == 0))
                    {
                        paramNum1 = paramNumber;
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)Longitude1));
                        paramNumber++;
                        paramNum2 = paramNumber;
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)Latitude1));
                        paramNumber++;
                        paramNum3 = paramNumber;
                        parameters.Add(new SqlParameter("P" + paramNumber.ToString(), (decimal)AddressFilter.Mile * (decimal) 1609.344)); // distance from miles to meters
                        paramNumber++;
                        whereClauses.Add(String.Format("F{0}.STDistance(geography::STPointFromText('POINT(' + CAST(@P{1} AS VARCHAR(20)) + ' ' + CAST(@P{2} AS VARCHAR(20)) + ')', 4326)) <= @P{3}", AddressFilter.theID, paramNum1, paramNum2, paramNum3));
                    }
                    // sqlTableIndex++;
                    // joinString += String.Format(" INNER JOIN [dbo].[UDFDistanceWithin](@p{0}, @p{1}, @p{2}) AS [t{4}] ON ([t{3}].[F{5}]) = [t{4}].[FieldID] ",paramNum1,paramNum2, paramNum3, originalSqlTableIndex,sqlTableIndex,AddressFilter.theID.ToString());
                }
                else if (filter is SearchWordsFilterRule)
                {
                    throw new Exception("Internal error: Fast queries are not implemented for search words query."); // This would be difficult, as we would need to convert the logic of GetTblRowsForPhrase, or we would need to convert the IQueryable<TblRow> into a SQL query and work with that. Overall, not worth it. Note that we should never hit this code as we check for whether there are any SearchWordsFilterRules before calling for a fast query.
                }
                else
                    throw new Exception("Internal error: Unknown filter rule type in GetStatementsForFiltering.");
            }
            if (filters.HighStakesOnly)
                whereClauses.Add("HS = 1");
            whereString = " ";
            if (whereClauses.Any() || asOfDateTime != null) 
            {
                //whereString += "WHERE ";
                string mainFilter = GetSeparatedList(whereClauses, " AND ");
                if (asOfDateTime != null && StatusRecords.RowStatusHasChangedSince((DateTime)asOfDateTime, tblID))
                    whereString = FilterBasedOnTblRowStatusRecords(mainFilter, originalSqlTableIndex, ref sqlTableIndex, ref paramNumber, parameters, (DateTime)asOfDateTime, tblID, !filters.ActiveOnly);
                else
                {
                    if (filters.ActiveOnly)
                    {
                        if (mainFilter.Trim() == "")
                            whereString = "WHERE (DEL = 0) ";
                        else
                            whereString = "WHERE (DEL = 0) AND (" + mainFilter + ") ";
                    }
                    else
                    {
                        if (mainFilter.Trim() != "")
                            whereString = "WHERE " + mainFilter;
                    }
                }
            }
        }

        //internal static string FilterBasedOnTblRowStatusRecordsWithStoredProcedure(string originalWhereStatement, ref int sqlTableIndex, ref int paramNumber, List<SqlParameter> parameters, DateTime asOfDateTime, int tblID)
        //{
        //    int sourceTableIndex = sqlTableIndex;
        //    sqlTableIndex++;
        //    int innerJoinTableIndex = sqlTableIndex;
        //    string innerJoinString = String.Format("INNER JOIN [dbo].[WasActiveTblRowV{2}](@p{3}) AS [t{1}] ON ([t{0}].[ID]) = [t{1}].[ID2]", sourceTableIndex, innerJoinTableIndex, tblID, paramNumber);
        //    parameters.Add(new SqlParameter("P" + paramNumber.ToString(), asOfDateTime));
        //    paramNumber++;
        //    if (originalWhereStatement.Trim() == "")
        //        return innerJoinString;
        //    else
        //        return "WHERE " + originalWhereStatement + " " + innerJoinString;
        //}

        internal static string FilterBasedOnTblRowStatusRecords(string originalWhereStatement, int originalSqlTableIndex, ref int sqlTableIndex, ref int paramNumber, List<SqlParameter> parameters, DateTime asOfDateTime, int tblID, bool showDeletedRows)
        {
            string broaderString;
            if (showDeletedRows)
                broaderString = "WHERE ((NOT ([t{0}].[RC] = 1)) OR (([t{0}].[RC] = 1) AND (NOT (EXISTS( SELECT NULL AS [EMPTY] FROM [dbo].[TblRowStatusRecord] AS [t{1}] WHERE ([t{1}].[TimeChanged] > @p{5}) AND ([t{1}].[TblRowId] = [t{0}].[ID]))))) OR (NOT (((SELECT [t{3}].[Adding] FROM (SELECT TOP (1) [t{2}].[Adding] FROM [dbo].[TblRowStatusRecord] AS [t{2}] WHERE ([t{2}].[TimeChanged] > @p{5}) AND ([t{2}].[TblRowId] = [t{0}].[ID]) ORDER BY [t{2}].[TimeChanged]) AS [t{3}])) = 1)))";
            else
                broaderString = "WHERE (((NOT ([t{0}].[RC] = 1)) AND ([t{0}].[DEL] = 0)) OR (([t{0}].[RC] = 1) AND ([t{0}].[DEL] = 0) AND (NOT (EXISTS( SELECT NULL AS [EMPTY] FROM [dbo].[TblRowStatusRecord] AS [t{1}] WHERE ([t{1}].[TimeChanged] > @p{5}) AND ([t{1}].[TblRowID] = [t{0}].[ID]))))) OR (((SELECT [t{3}].[Deleting] FROM ( SELECT TOP (1) [t{2}].[Deleting] FROM [dbo].[TblRowStatusRecord] AS [t{2}] WHERE ([t{2}].[TimeChanged] > @p{5}) AND ([t{2}].[TblRowID] = [t{0}].[ID]) ORDER BY [t{2}].[TimeChanged]) AS [t{3}])) = 1))";
            if (originalWhereStatement != "")
                broaderString += " AND ({4})";
            sqlTableIndex++;
            int tblRowStatusRecordTableNumber1 = sqlTableIndex;
            sqlTableIndex++;
            int tblRowStatusRecordTableNumber2 = sqlTableIndex;
            sqlTableIndex++;
            int innerQueryTableNumber = sqlTableIndex;
            string returnVal = String.Format(broaderString, originalSqlTableIndex, tblRowStatusRecordTableNumber1, tblRowStatusRecordTableNumber2, innerQueryTableNumber, originalWhereStatement, paramNumber);
            parameters.Add(new SqlParameter("P" + paramNumber.ToString(), asOfDateTime));
            paramNumber++;
            return returnVal;
        }

        internal static DataTable GetDataTable(IRaterooDataContext iDataContext, List<string> variableList, TableSortRule tableSortRule, FilterRules filters, List<ChoiceFullFieldDefinition> choiceFieldDefinitions, int tblID, int skip, int take)
        {
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return null;
            // SAMPLE -- this produces query results somewhat like this and adds the rowcount (indicating the total number of results without skip & take
            //SELECT *
            //, (
            //    SELECT COUNT(NME)
            //    FROM [Rateroo].dbo.V244
            //  ) AS row_count
            //FROM
            //(
            //SELECT ID, NME, DEL, RH, RS617, RV617, R617, RG617, RS618, RV618, R618, RG618, RS619, RV619, R619, RG619, RS620, RV620, R620, RG620, RS621, RV621, R621, RG621, RS622, RV622, R622, RG622, RS623, RV623, R623, RG623 
            //, ROW_NUMBER() OVER (ORDER BY R617 ASC) rownum
            //FROM [Rateroo].dbo.V244
            //) seq
            //where seq.rownum BETWEEN 6 and 10
            string tblName = "V" + tblID.ToString();
            string selectString = "SELECT ";

            int sqlTblIndex = 0;
            int originalSqlTblIndex = sqlTblIndex;
            string stringList = GetSeparatedList(variableList, ", ", "[t0].[", "]");
            selectString += stringList + " ";
            string indexHint = " "; 
            if (tableSortRule is TableSortRuleDistance)
            {
                indexHint = String.Format(" WITH(INDEX(SIndx_V{0}_F{1}) ", tblID, ((TableSortRuleDistance)tableSortRule).FieldDefinitionID);
            }
            else
            {
                AddressFilterRule theFilterRule = (AddressFilterRule)filters.theFilterRules.FirstOrDefault(x => x is AddressFilterRule);
                if (theFilterRule != null)
                    indexHint = String.Format(" WITH(INDEX(SIndx_V{0}_F{1}) ", tblID, theFilterRule.theID);
            }
            string fromInstruction = "FROM dbo." + tblName + " AS [t" + originalSqlTblIndex.ToString() + "] ";
            string orderInstruction;
            string joinInstructionForOrdering;
            int paramNumber = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            GetOrderBy(iDataContext, tableSortRule, tblID, originalSqlTblIndex, ref sqlTblIndex, filters.AsOfDateTime, ref paramNumber, parameters, out orderInstruction, out joinInstructionForOrdering);
            string whereInstruction;
            string joinInstructionForFiltering;
            GetStatementsForFiltering(filters, choiceFieldDefinitions, originalSqlTblIndex, ref sqlTblIndex, filters.AsOfDateTime, tblID, ref paramNumber, ref parameters, out whereInstruction, out joinInstructionForFiltering);

            string orderAndRowNumberLine = String.Format(", ROW_NUMBER() OVER ({0}) rownum ", orderInstruction);
            string coreQuery = selectString + orderAndRowNumberLine + fromInstruction + joinInstructionForOrdering + joinInstructionForFiltering + whereInstruction;
            string rowCountInstruction = String.Format(", ( SELECT COUNT(ID) {0} {1} {2} {3} ) AS row_count ", fromInstruction, joinInstructionForOrdering, joinInstructionForFiltering, whereInstruction);
            string skipTakeInstruction = " seq where seq.rownum BETWEEN " + (skip + 1).ToString() + " and " + (skip + take).ToString();
            string entireQuery = String.Format("SELECT * {0} FROM ( {1} ) {2}", rowCountInstruction, coreQuery, skipTakeInstruction);
            SqlDataAdapter adapter = new SqlDataAdapter(entireQuery, AzureSetup.GetConfigurationSetting("RaterooConnectionString"));
            foreach (var whereParam in parameters)
                adapter.SelectCommand.Parameters.Add(whereParam);
            DataSet data = new DataSet();
            adapter.Fill(data, tblName);
            return data.Tables[tblName];
        }

        private static string GetSeparatedList(List<string> variableList, string separator=", ", string prefix="", string suffix="")
        {
            string stringList = "";
            bool isFirst = true;
            foreach (var variable in variableList)
            {
                if (!isFirst)
                    stringList += separator;
                isFirst = false;
                stringList += prefix + variable + suffix;
            }
            return stringList;
        }

        public static void DoQuery(int firstRowNum, int numRows, bool populatingInitially, string cacheString, string[] myDependencies, IRaterooDataContext iDataContext, TableInfo theTableInfo, string tableInfoForReset, int? maxNumResults, TableSortRule theTableSortRule, Tbl theTbl, out List<InfoForBodyRows> bodyRowList, out int? rowCount)
        {
            bodyRowList = null;
            rowCount = null;
            RaterooDataContext dataContext = iDataContext.GetRealDatabaseIfExists();
            if (dataContext == null)
                return;

            System.Diagnostics.Trace.WriteLine("DoQuery cacheString " + cacheString + " firstRow: " + firstRowNum + " numRows: " + numRows);
            DoQueryResults theResults = PMCacheManagement.GetItemFromCache(cacheString + "FastQueryResults") as DoQueryResults;

            if (theResults == null)
            {
                //ProfileSimple.Start("DoQuery");
                List<TblColumn> tblColumns = TblColumnsForTabCache.GetTblColumnsForTab(iDataContext, theTbl.TblID, theTableInfo.TblTabID);

                List<string> theVariables = GetListOfFastAccessVariablesToLoad(tblColumns, true);
                List<ChoiceFullFieldDefinition> choiceFieldDefinitions = GetChoiceFullFieldDefinitions(iDataContext, theTbl);
                //ProfileSimple.Start("GetDataTable");
                DataTable theDataTable = GetDataTable(iDataContext, theVariables, theTableSortRule, theTableInfo.Filters, choiceFieldDefinitions, theTbl.TblID, firstRowNum - 1, numRows);
                //ProfileSimple.End("GetDataTable");
                theResults = ParseDataTable(tblColumns, theDataTable);

                PMCacheManagement.AddItemToCache(cacheString + "FastQueryResults", myDependencies, theResults, new TimeSpan(0, 0, 15));
                //ProfileSimple.End("DoQuery");
            }

            bodyRowList = theResults.bodyRowList;
            rowCount = theResults.rowCount;
        }

        private static DoQueryResults ParseDataTable(List<TblColumn> tblColumns, DataTable theDataTable)
        {
            List<InfoForBodyRows> infoList = new List<InfoForBodyRows>();
            int rowCount = 0;
            for (int rowNum = 0; rowNum < theDataTable.Rows.Count; rowNum++)
            {
                var row = theDataTable.Rows[rowNum];
                foreach (var col in tblColumns)
                {
                    if (rowCount == 0)
                        rowCount = (int)row["row_count"];
                    string colS = col.TblColumnID.ToString();
                    InfoForBodyRows info = new InfoForBodyRows
                    {
                        RowHeadingWithPopup = row["RH"] as string,
                        TblRowID = row["ID"] is System.DBNull ? 0 : (int)row["ID"],
                        TblColumnID = col.TblColumnID,
                        TopRatingGroupID = row["RG" + colS] is System.DBNull ? 0 : (int)row["RG" + colS],
                        FirstRatingID = row["R" + colS] is System.DBNull ? 0 : (int)row["R" + colS],
                        DecPlaces = null, // ignored
                        ValueOfFirstRating = null, // ignored
                        SingleNumberOnly = !(row["RS" + colS] as string).Contains("<"),
                        Trusted = null, // ignored
                        Deleted = (bool)row["DEL"],
                        PreformattedString = row["RS" + colS] as string
                    };
                    infoList.Add(info);
                }
            }

            var theResults = new DoQueryResults { bodyRowList = infoList, rowCount = rowCount };
            return theResults;
        }

        public static bool FastAccessTablesEnabled()
        {
            return true;
            // Note: after turning on by changing to true, must drop tables on /admin/tester.aspx, and then when this is true they'll be added again.
        }
    }
}
