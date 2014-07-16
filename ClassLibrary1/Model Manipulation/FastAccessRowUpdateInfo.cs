using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using System.Data;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    [Serializable]
    public abstract class FastAccessRowUpdateInfo
    {
        public abstract List<SQLCellInfo> GetSQLParameterInfo();

        public virtual SQLUpdateInfoTableSpecification GetTableSpecification(TblRow tblRow)
        {
            return GetMainSqlUpdateInfoTableSpecification(tblRow.TblID);
        }

        public virtual void AddToTblRow(TblRow tblRow, SQLUpdateInfoTableSpecification tableSpec = null)
        {
            bool dataIsAlreadyInDatabase = false;
            if (tableSpec == null)
            {
                tableSpec = GetTableSpecification(tblRow);
                if (!tblRow.NotYetAddedToDatabase)
                    dataIsAlreadyInDatabase = true;
            }
            
            SQLInfoForCellsInRow_MainAndSecondaryTables sqlUpdateInfos = null;
            sqlUpdateInfos = GetFastAccessRowUpdateInfoList(tblRow);
            List<SQLCellInfo> suis = GetSQLParameterInfo();
            foreach (SQLCellInfo sui in suis)
            {
                sui.DataIsAlreadyInDatabase = dataIsAlreadyInDatabase;
                sqlUpdateInfos.Add(sui, tableSpec);
            }
            //for (int spi_index = 0; spi_index < suis.Count(); spi_index++)
            //{
            //    SQLCellInfo sui = suis[spi_index];
            //    var match = sqlUpdateInfos.SQLUpdateInfos.Select((item, index) => new { Item = item, Index = index}).FirstOrDefault(x => x.Item.Paramname == newparamname);
            //    int? matchIndex = null;
            //    if (match != null)
            //        matchIndex = match.Index;
            //    if (match == null)
            //        sqlUpdateInfos.Add(sui);
            //    else
            //        sqlUpdateInfos[(int)matchIndex] = sui;
            //}
            tblRow.FastAccessUpdated = BinarySerializer.Serialize<SQLInfoForCellsInRow_MainAndSecondaryTables>(sqlUpdateInfos);
            tblRow.FastAccessUpdateSpecified = true;
        }

        public SQLUpdateInfoTableSpecification GetMainSqlUpdateInfoTableSpecification(Guid tblID)
        {
            return FastAccessTableInfo.GetSpecification(tblID);
        }

        public SQLUpdateInfoTableSpecification GetMultipleChoicesTableSpecification(Guid fieldDefinitionID, Guid tblID)
        {
            return FastAccessMultipleChoiceFieldTableInfo.GetSpecification(fieldDefinitionID, tblID);
        }

        public static SQLInfoForCellsInRow_MainAndSecondaryTables GetFastAccessRowUpdateInfoList(TblRow tblRow)
        {
            SQLInfoForCellsInRow_MainAndSecondaryTables updates = null;
            if (tblRow.FastAccessUpdated == null)
                updates = new SQLInfoForCellsInRow_MainAndSecondaryTables();
            else
                updates = BinarySerializer.Deserialize<SQLInfoForCellsInRow_MainAndSecondaryTables>(tblRow.FastAccessUpdated.ToArray());
            if (!tblRow.NotYetAddedToDatabase) 
                updates.SetMainTablePrimaryKey(tblRow.TblRowID, true);
            return updates;
        }

    }

    [Serializable]
    public class FastAccessHighStakesKnownUpdateInfo : FastAccessRowUpdateInfo
    {
        public int HighStakesKnownChange;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "HS", Value = HighStakesKnownChange, DBtype = SqlDbType.Int, DefaultToUseIfMissing = "0", ValueIsRelative = true }
            };
        }
    }

    [Serializable]
    public class FastAccessDeletedUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool Deleted;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "DEL", Value = Deleted, DBtype = SqlDbType.Bit, DefaultToUseIfMissing = "0" }
            };
        }
    }

    [Serializable]
    public class FastAccessElevateOnMostNeedsRatingUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool ElevateOnMostNeedsRating;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "ELEV", Value = ElevateOnMostNeedsRating, DBtype = SqlDbType.Bit, DefaultToUseIfMissing = "0" }
            };
        }
    }

    [Serializable]
    public class FastAccessCountNonNullEntriesUpdateInfo : FastAccessRowUpdateInfo
    {
        public int CountNonNullEntries;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "CNNE", Value = CountNonNullEntries, DBtype = SqlDbType.Int, DefaultToUseIfMissing = "0" }
            };
        }
    }

    [Serializable]
    public class FastAccessCountUserPointsUpdateInfo : FastAccessRowUpdateInfo
    {
        public decimal CountUserPoints;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "CUP", Value = CountUserPoints, DBtype = SqlDbType.Decimal, DefaultToUseIfMissing = "0" }
            };
        }
    }

    [Serializable]
    public class FastAccessRowHeadingUpdateInfo : FastAccessRowUpdateInfo
    {
        public string RowHeading;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "RH", Value = RowHeading, DBtype = SqlDbType.NVarChar }
            };
        }
    }

    [Serializable]
    public class FastAccessTblRowNameUpdateInfo : FastAccessRowUpdateInfo
    {
        public string Name;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "NME", Value = Name, DBtype = SqlDbType.NVarChar }
            };
        }
    }

    [Serializable]
    public class FastAccessVolatilityUpdateInfo : FastAccessRowUpdateInfo
    {
        public decimal Value;
        public VolatilityDuration TimeFrame;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = FastAccessTableInfo.GetVolatilityColumnNameForDuration(TimeFrame), Value = Value, DBtype = SqlDbType.Decimal, DefaultToUseIfMissing = "0" }
            };
        }
    }

    [Serializable]
    public abstract class FastAccessFieldUpdateInfo : FastAccessRowUpdateInfo
    {
        public Guid FieldDefinitionID;
    }

    [Serializable]
    public class FastAccessTextFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public string Text;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "F" + FieldDefinitionID.ToString(), Value = Text, DBtype = SqlDbType.NVarChar }
            };
        }
    }

    [Serializable]
    public class FastAccessNumberFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public decimal? Number;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "F" + FieldDefinitionID.ToString(), Value = Number, DBtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessDateTimeFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public DateTime? DateTimeInfo;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "F" + FieldDefinitionID.ToString(), Value = DateTimeInfo, DBtype = SqlDbType.DateTime }
            };
        }
    }

    [Serializable]
    public class FastAccessChoiceFieldSingleSelectionUpdateInfo : FastAccessFieldUpdateInfo
    {
        public Guid? ChoiceInGroupID;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "F" + FieldDefinitionID.ToString(), Value = ChoiceInGroupID, DBtype = SqlDbType.Int }
            };
        }
    }

    public class FastAccessChoiceFieldMultipleSelectionUpdateInfo : FastAccessFieldUpdateInfo
    {
        public Guid TblRowID;
        public Guid ChoiceInGroupID;
        public bool Delete;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            Guid g = Guid.NewGuid(); // will be used to group the items. We can't just use ChoiceInFieldID, because we don't have it until submission to the database, and we can't use TblRowID, because there could be multiple choices for this field for a TblRowID, and we can't use ChoiceInGroupID, because we need to group within the particular TblRow (otherwise we would be grouping items from different table rows)
            string groupingKey = g.ToString();
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "CHO", Value = ChoiceInGroupID, DBtype = SqlDbType.Int, Rownum = null, Delete = Delete, GroupingKey = groupingKey, DataIsAlreadyInDatabase = Delete, SetValueToPrimaryKeyIDOfMainTableOnceLoaded = false },
                new SQLCellInfo() { Fieldname = "TRID", Value = TblRowID, SetValueToPrimaryKeyIDOfMainTableOnceLoaded = true /* in case TblRowID is null or 0 */, DBtype = SqlDbType.Int, Rownum = null, Delete = Delete, GroupingKey = groupingKey, DataIsAlreadyInDatabase = Delete, groupedWithNPreviousItems = 1 }
            };
        }

        public override SQLUpdateInfoTableSpecification GetTableSpecification(TblRow tblRow)
        {
            return GetMultipleChoicesTableSpecification(FieldDefinitionID, tblRow.TblID);
        }
    }

    [Serializable]
    public class FastAccessAddressFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public SQLGeographyInfo GeoInfo;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "F" + FieldDefinitionID.ToString(), Value = GeoInfo, DBtype = SqlDbType.Udt }
            };
        }
    }

    [Serializable]
    public abstract class FastAccessCellUpdateInfo : FastAccessRowUpdateInfo
    {
        public Guid TblColumnID;
    }

    [Serializable]
    public class FastAccessRecentlyChangedInfo : FastAccessCellUpdateInfo
    {
        public bool RecentlyChanged;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "RC", Value = RecentlyChanged, DBtype = SqlDbType.Bit }
            };
        }
    }


    [Serializable]
    public class FastAccessRatingUpdatingInfo : FastAccessCellUpdateInfo
    {
        public decimal? NewValue;
        public string StringRepresentation;
        public int CountNonNullEntries;
        public decimal CountUserPoints;
        public bool RecentlyChanged; // we include this rather than rely solely on FastAccessRecentlyChangedInfo since we will generally want to change them together
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "RS" + TblColumnID.ToString(), Value = StringRepresentation, DBtype = SqlDbType.NVarChar },
                new SQLCellInfo() { Fieldname = "RV" + TblColumnID.ToString(), Value = NewValue, DBtype = SqlDbType.Decimal },
                new SQLCellInfo() { Fieldname = "RC" + TblColumnID.ToString(), Value = RecentlyChanged, DBtype = SqlDbType.Bit },
                // The following are not necessary because we will automatically add these in FastAccessRowUpdatePartialClasses (so adding them again will just lead to redundancy and a need to eliminate them later)
                //new SQLParameterInfo() { fieldname = "CNNE", value = CountNonNullEntries, dbtype = SqlDbType.Int },
                //new SQLParameterInfo() { fieldname = "CUP", value = CountUserPoints, dbtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessRatingIDUpdatingInfo : FastAccessCellUpdateInfo
    {
        public Guid RatingID;
        public Guid RatingGroupID;
        public override List<SQLCellInfo> GetSQLParameterInfo()
        {
            return new List<SQLCellInfo>()
            {
                new SQLCellInfo() { Fieldname = "R" + TblColumnID.ToString(), Value = RatingID, DBtype = SqlDbType.Int },
                new SQLCellInfo() { Fieldname = "RG" + TblColumnID.ToString(), Value = RatingGroupID, DBtype = SqlDbType.Int }
            };
        }
    }

}
