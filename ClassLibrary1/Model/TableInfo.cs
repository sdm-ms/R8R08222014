using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Web.Script.Serialization;
using ClassLibrary1.Model;


namespace ClassLibrary1.Model
{

    [Serializable]
    public class TableInfo
    {
        public int TblID;
        public int TblTabID;
        public string SortInstruction;
        public List<SortMenuItem> SortMenu;
        public string SuppStyle;
        public FilterRules Filters;
    }

    public class LoadRowHeadingInfo
    {
        public R8RDataAccess dataAccess;
        public int theTblID;
        public int thePointsManagerID;
        public int theTblRowID;
        public TblDimension theTblDimension;
    }

    public class LoadBodyRowInfo
    {
        public R8RDataAccess dataAccess;
        public int theTblID;
        public int theTblRowID;
        public int TblTabID;
        public string suppStyle;
    }

    public class LoadHeaderRowInfo
    {
        public R8RDataAccess dataAccess;
        public int TblTabID;
        public int? TblColumnToSortID;
        public bool SortByEntityName; // only if there is no column to sort by
        public bool ascending;
    }

    public class InfoForBodyRows
    {
        public TblRowFieldDisplay TblRowFieldDisplay;
        public string RowHeadingWithPopup; // only if data is loaded through fast tables
        public int TblRowID;
        public int TblColumnID;
        public int? TopRatingGroupID;
        public int? FirstRatingID;
        public int? DecPlaces; // null if preformatted string exists
        public decimal? ValueOfFirstRating; // null if preformatted string exists
        public bool SingleNumberOnly;
        public bool? Trusted; // null if preformatted string exists
        public bool Deleted;
        public string PreformattedString; // null or "" if no preformatted string is available
    }

    public static class TableInfoToStringConversion
    {
        public static string GetStringFromTableInfo(TableInfo theTableInfo)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new FilterRuleConverter() });
            var theOutput = serializer.Serialize(theTableInfo);

            return theOutput;
        }

        public static TableInfo GetTableInfoFromString(string theTableInfoString)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new FilterRuleConverter() });
            TableInfo theOutput = (TableInfo)serializer.Deserialize<TableInfo>(theTableInfoString);
            theOutput.Filters.AsOfDateTime = ((DateTime)theOutput.Filters.AsOfDateTime).ToLocalTime();
            return theOutput;
        }
    }

}
