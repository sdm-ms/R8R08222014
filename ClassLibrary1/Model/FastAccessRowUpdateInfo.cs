using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1.Misc;
using System.Data;

namespace ClassLibrary1.Model
{
    [Serializable]
    public abstract class FastAccessRowUpdateInfo
    {
        public abstract List<SQLParameterInfo> GetSQLParameterInfo();
        public void AddToTblRow(TblRow tblRow)
        {
            List<SQLParameterInfo> sqlParamInfos = null;
            if (tblRow.FastAccessUpdated != null)
                sqlParamInfos = GetFastAccessRowUpdateInfoList(tblRow);
            if (sqlParamInfos == null)
            {
                sqlParamInfos = new List<SQLParameterInfo>()
                {
                    new SQLParameterInfo() { fieldname = "ID", rownum = tblRow.TblRowID, tablename = "V" + tblRow.TblID.ToString(), value = tblRow.TblRowID, dbtype = SqlDbType.Int } // we only need to add this once per table row
                };
            }
            List<SQLParameterInfo> spis = GetSQLParameterInfo();
            foreach (var spi in spis)
            {
                spi.rownum = tblRow.TblRowID;
                spi.tablename = "V" + tblRow.TblID.ToString();
            }
            sqlParamInfos.AddRange(spis);
            tblRow.FastAccessUpdated = BinarySerializer.Serialize<List<SQLParameterInfo>>(sqlParamInfos);
            tblRow.FastAccessUpdateSpecified = true;
        }

        public static List<SQLParameterInfo> GetFastAccessRowUpdateInfoList(TblRow tblRow)
        {
            return BinarySerializer.Deserialize<List<SQLParameterInfo>>(tblRow.FastAccessUpdated.ToArray());
        }
    }

    [Serializable]
    public class FastAccessHighStakesKnownUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool HighStakesKnown;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "HS", value = HighStakesKnown, dbtype = SqlDbType.Bit }
            };
        }
    }

    [Serializable]
    public class FastAccessElevateOnMostNeedsRatingUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool ElevateOnMostNeedsRating;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "ELEV", value = ElevateOnMostNeedsRating, dbtype = SqlDbType.Bit }
            };
        }
    }

    [Serializable]
    public class FastAccessCountNonNullEntriesUpdateInfo : FastAccessRowUpdateInfo
    {
        public int CountNonNullEntries;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "CNNE", value = CountNonNullEntries, dbtype = SqlDbType.Int }
            };
        }
    }

    [Serializable]
    public abstract class FastAccessFieldUpdateInfo : FastAccessRowUpdateInfo
    {
        public int FieldDefinitionID;
    }

    [Serializable]
    public class FastAccessTextFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public string Text;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = Text, dbtype = SqlDbType.NVarChar }
            };
        }
    }

    [Serializable]
    public class FastAccessNumberFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public decimal? Number;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = Number, dbtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessDateTimeFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public DateTime DateTimeInfo;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = DateTimeInfo, dbtype = SqlDbType.DateTime }
            };
        }
    }

    [Serializable]
    public class FastAccessChoiceFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public int ChoiceInGroupID;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = ChoiceInGroupID, dbtype = SqlDbType.Int }
            };
        }
    }

    [Serializable]
    public class FastAccessAddressFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public SQLGeographyInfo GeoInfo;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = GeoInfo, dbtype = SqlDbType.Udt }
            };
        }
    }

    [Serializable]
    public abstract class FastAccessCellUpdateInfo : FastAccessRowUpdateInfo
    {
        public int TblColumnID;
    }

    [Serializable]
    public class FastAccessRecentlyChangedInfo : FastAccessCellUpdateInfo
    {
        public bool RecentlyChanged;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "CNNE", value = RecentlyChanged, dbtype = SqlDbType.Bit }
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
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "RS" + TblColumnID.ToString(), value = StringRepresentation, dbtype = SqlDbType.NVarChar },
                new SQLParameterInfo() { fieldname = "RV" + TblColumnID.ToString(), value = NewValue, dbtype = SqlDbType.Decimal },
                new SQLParameterInfo() { fieldname = "RC" + TblColumnID.ToString(), value = RecentlyChanged, dbtype = SqlDbType.Bit },
                new SQLParameterInfo() { fieldname = "CNNE", value = CountNonNullEntries, dbtype = SqlDbType.Int },
                new SQLParameterInfo() { fieldname = "CUP", value = CountUserPoints, dbtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessRatingIDUpdatingInfo : FastAccessCellUpdateInfo
    {
        public int RatingID;
        public int RatingGroupID;
        public override List<SQLParameterInfo> GetSQLParameterInfo()
        {
            return new List<SQLParameterInfo>()
            {
                new SQLParameterInfo() { fieldname = "R" + TblColumnID.ToString(), value = RatingID, dbtype = SqlDbType.Int },
                new SQLParameterInfo() { fieldname = "RG" + TblColumnID.ToString(), value = RatingGroupID, dbtype = SqlDbType.Int }
            };
        }
    }

}
