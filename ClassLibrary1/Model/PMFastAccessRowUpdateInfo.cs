using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Model
{
    [Serializable]
    public abstract class FastAccessRowUpdateInfo
    {
        public int TblColumnID;
        public abstract List<Tuple<string, string>> GetColumnNamesAndNewValues();
        public void AddToTblRow(TblRow tblRow)
        {
            List<FastAccessRowUpdateInfo> faruiList = null;
            if (tblRow.FastAccessUpdated != null)
                faruiList = GetFastAccessRowUpdateInfoList(tblRow);
            if (faruiList == null)
                faruiList = new List<FastAccessRowUpdateInfo>();
            faruiList.Add(this);
            tblRow.FastAccessUpdated = BinarySerializer.Serialize<List<FastAccessRowUpdateInfo>>(faruiList);
            tblRow.FastAccessUpdateSpecified = true;
        }

        public static List<FastAccessRowUpdateInfo> GetFastAccessRowUpdateInfoList(TblRow tblRow)
        {
            return BinarySerializer.Deserialize<List<FastAccessRowUpdateInfo>>(tblRow.FastAccessUpdated.ToArray());
        }
    }

    [Serializable]
    public class FastAccessRecentlyChangedInfo : FastAccessRowUpdateInfo
    {
        public bool RecentlyChanged;
        public override List<Tuple<string, string>> GetColumnNamesAndNewValues()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("RC" + TblColumnID.ToString(), RecentlyChanged ? "1" : "0")
            };
        }
    }

    [Serializable]
    public class FastAccessCountNonNullEntriesInfo : FastAccessRowUpdateInfo
    {
        public int CountNonNullEntries;
        public override List<Tuple<string, string>> GetColumnNamesAndNewValues()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("CNNE", CountNonNullEntries.ToString()),
            };
        }
    }

    [Serializable]
    public class FastAccessRatingUpdatingInfo : FastAccessRowUpdateInfo
    {
        public decimal? NewValue;
        public string StringRepresentation;
        public int CountNonNullEntries;
        public decimal CountUserPoints;
        public bool RecentlyChanged; // we include this rather than rely solely on FastAccessRecentlyChangedInfo since we will generally want to change them together
        public override List<Tuple<string, string>> GetColumnNamesAndNewValues()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("RS" + TblColumnID.ToString(), StringRepresentation),
                new Tuple<string,string>("RV" + TblColumnID.ToString(), NewValue == null ? "NULL" : NewValue.ToString()),
                new Tuple<string,string>("RC" + TblColumnID.ToString(), RecentlyChanged ? "1" : "0"),
                new Tuple<string,string>("CNNE", CountNonNullEntries.ToString()),
                new Tuple<string,string>("CUP", CountUserPoints.ToString()),
            };
        }
    }

    [Serializable]
    public class FastAccessRatingIDUpdatingInfo : FastAccessRowUpdateInfo
    {
        public int RatingID;
        public int RatingGroupID;
        public override List<Tuple<string, string>> GetColumnNamesAndNewValues()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("R" + TblColumnID.ToString(), RatingID.ToString()),
                new Tuple<string,string>("RG" + TblColumnID.ToString(), RatingGroupID.ToString())
            };
        }
    }
}
