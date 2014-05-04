﻿using System;
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
        public abstract List<SQLUpdateInfo> GetSQLParameterInfo();
        public void AddToTblRow(TblRow tblRow)
        {
            List<SQLUpdateInfo> sqlParamInfos = null;
            sqlParamInfos = GetFastAccessRowUpdateInfoList(tblRow);
            bool rowNotYetInDatabase;
            if (sqlParamInfos != null && sqlParamInfos.Any())
                rowNotYetInDatabase = sqlParamInfos.First().rowNotYetInDatabase;
            else
                rowNotYetInDatabase = false; // we assume that the row IS in the database, until we explicitly specify that the row isn't (see GetFastAccessRowUpdateInfoList)
            List<SQLUpdateInfo> spis = GetSQLParameterInfo();
            for (int spi_index = 0; spi_index < spis.Count(); spi_index++)
            {
                SQLUpdateInfo spi = spis[spi_index];
                spi.rownum = tblRow.TblRowID;
                spi.tablename = "V" + tblRow.TblID.ToString();
                spi.rowNotYetInDatabase = rowNotYetInDatabase;
                string newparamname = spi.paramname;
                var match = spis.Select((item, index) => new { Item = item, Index = index}).FirstOrDefault(x => x.Item.paramname == newparamname);
                int? matchIndex = null;
                if (match != null)
                    matchIndex = match.Index;
                if (match == null)
                    sqlParamInfos.Add(spi);
                else
                    spis[(int)matchIndex] = spi;
            }
            sqlParamInfos.AddRange(spis);
            tblRow.FastAccessUpdated = BinarySerializer.Serialize<List<SQLUpdateInfo>>(sqlParamInfos);
            tblRow.FastAccessUpdateSpecified = true;
        }

        public static List<SQLUpdateInfo> GetFastAccessRowUpdateInfoList(TblRow tblRow)
        {
            List<SQLUpdateInfo> updates;
            if (tblRow.FastAccessUpdated == null)
                updates = new List<SQLUpdateInfo>();
            else
                updates = BinarySerializer.Deserialize<List<SQLUpdateInfo>>(tblRow.FastAccessUpdated.ToArray());
            bool idAlreadyExists = updates.Any(x => x.fieldname == "ID");
            if (!idAlreadyExists && tblRow.TblRowID != 0) // once we execute this once, we have an ID field with a non-zero ID, so we don't have to do it again. Until TblRowID is non-zero, we won't execute it.
            {

                updates.Add(new SQLUpdateInfo() { fieldname = "ID", rownum = tblRow.TblRowID, tablename = "V" + tblRow.TblID.ToString(), value = tblRow.TblRowID, dbtype = SqlDbType.Int, rowNotYetInDatabase = false });

                if (tblRow.FastAccessInitialCopy)
                {
                    // this has just been added to the database, so we previously did not have a TblRowID and so the rownum is wrong, and we must mark this as data that should be inserted
                    // update all the previous items
                    foreach (SQLUpdateInfo update in updates)
                    {
                        update.rownum = tblRow.TblRowID;
                        update.rowNotYetInDatabase = true;
                    }
                }
            }
            return updates;
        }
    }

    [Serializable]
    public class FastAccessHighStakesKnownUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool HighStakesKnown;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "HS", value = HighStakesKnown, dbtype = SqlDbType.Bit }
            };
        }
    }

    [Serializable]
    public class FastAccessDeletedUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool Deleted;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "DEL", value = Deleted, dbtype = SqlDbType.Bit }
            };
        }
    }

    [Serializable]
    public class FastAccessElevateOnMostNeedsRatingUpdateInfo : FastAccessRowUpdateInfo
    {
        public bool ElevateOnMostNeedsRating;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "ELEV", value = ElevateOnMostNeedsRating, dbtype = SqlDbType.Bit }
            };
        }
    }

    [Serializable]
    public class FastAccessCountNonNullEntriesUpdateInfo : FastAccessRowUpdateInfo
    {
        public int CountNonNullEntries;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "CNNE", value = CountNonNullEntries, dbtype = SqlDbType.Int }
            };
        }
    }

    [Serializable]
    public class FastAccessCountUserPointsUpdateInfo : FastAccessRowUpdateInfo
    {
        public decimal CountUserPoints;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "CUP", value = CountUserPoints, dbtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessRowHeadingUpdateInfo : FastAccessRowUpdateInfo
    {
        public string RowHeading;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "RH", value = RowHeading, dbtype = SqlDbType.NVarChar }
            };
        }
    }

    [Serializable]
    public class FastAccessTblRowNameUpdateInfo : FastAccessRowUpdateInfo
    {
        public string Name;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "NME", value = Name, dbtype = SqlDbType.NVarChar }
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
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = Text, dbtype = SqlDbType.NVarChar }
            };
        }
    }

    [Serializable]
    public class FastAccessNumberFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public decimal? Number;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = Number, dbtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessDateTimeFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public DateTime? DateTimeInfo;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = DateTimeInfo, dbtype = SqlDbType.DateTime }
            };
        }
    }

    [Serializable]
    public class FastAccessChoiceFieldSingleSelectionUpdateInfo : FastAccessFieldUpdateInfo
    {
        public int? ChoiceInGroupID;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = ChoiceInGroupID, dbtype = SqlDbType.Int }
            };
        }
    }

    [Serializable]
    public class FastAccessAddressFieldUpdateInfo : FastAccessFieldUpdateInfo
    {
        public SQLGeographyInfo GeoInfo;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "F" + FieldDefinitionID.ToString(), value = GeoInfo, dbtype = SqlDbType.Udt }
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
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "RC", value = RecentlyChanged, dbtype = SqlDbType.Bit }
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
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "RS" + TblColumnID.ToString(), value = StringRepresentation, dbtype = SqlDbType.NVarChar },
                new SQLUpdateInfo() { fieldname = "RV" + TblColumnID.ToString(), value = NewValue, dbtype = SqlDbType.Decimal },
                new SQLUpdateInfo() { fieldname = "RC" + TblColumnID.ToString(), value = RecentlyChanged, dbtype = SqlDbType.Bit },
                // The following are not necessary because we will automatically add these in FastAccessRowUpdatePartialClasses (so adding them again will just lead to redundancy and a need to eliminate them later)
                //new SQLParameterInfo() { fieldname = "CNNE", value = CountNonNullEntries, dbtype = SqlDbType.Int },
                //new SQLParameterInfo() { fieldname = "CUP", value = CountUserPoints, dbtype = SqlDbType.Decimal }
            };
        }
    }

    [Serializable]
    public class FastAccessRatingIDUpdatingInfo : FastAccessCellUpdateInfo
    {
        public int RatingID;
        public int RatingGroupID;
        public override List<SQLUpdateInfo> GetSQLParameterInfo()
        {
            return new List<SQLUpdateInfo>()
            {
                new SQLUpdateInfo() { fieldname = "R" + TblColumnID.ToString(), value = RatingID, dbtype = SqlDbType.Int },
                new SQLUpdateInfo() { fieldname = "RG" + TblColumnID.ToString(), value = RatingGroupID, dbtype = SqlDbType.Int }
            };
        }
    }

}
