using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ClassLibrary1.EFModel;


namespace ClassLibrary1.Model
{
    public static class NumberandTableFormatter
    {
        public static TblColumnFormatting GetBlankCDFormatting()
        {
            TblColumnFormatting theFormatting = new TblColumnFormatting();
            theFormatting.Prefix = "";
            theFormatting.Suffix = "";
            return theFormatting;
        }

        public static TblColumnFormatting GetFormattingForTblColumn(int TblColumnID)
        {
            TblColumnFormatting theFormatting = null;
            string cacheKey = "CatDesFormat" + TblColumnID.ToString();
            bool isInCacheButNull;
            theFormatting = (TblColumnFormatting)CacheManagement.GetItemFromCache(cacheKey, out isInCacheButNull);
            if (theFormatting == null && !isInCacheButNull)
            {
                R8RDataAccess theDataAccessModule = new R8RDataAccess();
                theFormatting = theDataAccessModule.R8RDB.GetTable<TblColumnFormatting>().SingleOrDefault(cdf => cdf.TblColumnID == TblColumnID);
                int theTblID = theDataAccessModule.R8RDB.GetTable<TblColumn>().Single(cd => cd.TblColumnID == TblColumnID).TblTab.TblID;
                string[] myDependencies = {
                                    "ColumnsForTblID" + theTblID.ToString()
                                                      };
                CacheManagement.AddItemToCache(cacheKey, myDependencies, theFormatting);
            }
            return theFormatting;
        }

        public static string FormatAsSpecified(decimal? value, int decimalPlaces, int TblColumnID)
        {
            return FormatAsSpecified(value, decimalPlaces, GetFormattingForTblColumn(TblColumnID));
        }

        public static string GetSuppStyleMain(int? TblColumnID)
        {
            if (TblColumnID == null)
                return "";
            TblColumnFormatting theFormatting = GetFormattingForTblColumn((int)TblColumnID);
            if (theFormatting == null)
                return "";
            return theFormatting.SuppStylesMain;
        }

        public static string GetSuppStyleHeader(int? TblColumnID)
        {
            if (TblColumnID == null)
                return "";
            TblColumnFormatting theFormatting = GetFormattingForTblColumn((int)TblColumnID);
            if (theFormatting == null)
                return "";
            return theFormatting.SuppStylesHeader;
        }


        public static string FormatAsSpecified(decimal? value, int decimalPlaces, TblColumnFormatting theRules)
        {
            if (value == null)
                return "--";
            if (theRules == null)
                return MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(value, decimalPlaces);
            // add extra decimal places, but only if more precision is needed for the value. For example,
            // we don't want 100.000% if most numbers are rendered as 98.5%, but we might want 99.99%.
            if (theRules.ExtraDecimalPlaceAbove != null && 
                value > theRules.ExtraDecimalPlaceAbove && 
                ((double)(value * (10 ^ decimalPlaces)) != Math.Floor((double)(value * (10 ^ decimalPlaces)))))
                decimalPlaces++;
            if (theRules.ExtraDecimalPlace2Above != null && 
                value > theRules.ExtraDecimalPlace2Above && 
                ((double)(value * (10 ^ decimalPlaces)) != Math.Floor((double)(value * (10 ^ decimalPlaces)))))
                decimalPlaces++;
            if (theRules.ExtraDecimalPlace3Above != null && 
                value > theRules.ExtraDecimalPlace3Above && 
                ((double)(value * (10 ^ decimalPlaces)) != Math.Floor((double)(value * (10 ^ decimalPlaces)))))
                decimalPlaces++;
            string initialFormatting = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(value, decimalPlaces);
            if (value > 0 && value < 1 && theRules.OmitLeadingZero && initialFormatting[0] == '0')
                initialFormatting = MoreStrings.MoreStringManip.Right(initialFormatting, initialFormatting.Length - 1);
            return theRules.Prefix + initialFormatting + theRules.Suffix;
        }

        public static string RemovePrefixAndSuffix(string initialString, int TblColumnID)
        {
            return RemovePrefixAndSuffix(initialString, GetFormattingForTblColumn(TblColumnID));
        }

        public static string RemovePrefixAndSuffix(string initialString, TblColumnFormatting theRules)
        {
            if (theRules == null)
                return initialString;
            if (initialString.Contains(theRules.Prefix))
                initialString = initialString.Substring(theRules.Prefix.Length, initialString.Length - theRules.Prefix.Length);
            if (initialString.Contains(theRules.Suffix))
                initialString = initialString.Substring(0, initialString.Length - theRules.Suffix.Length);
            if (initialString.Contains(","))
            {
                string[] theStringComponents = initialString.Split(',');
                initialString = "";
                foreach (string theComponent in theStringComponents)
                    initialString += theComponent;
            }
            if (initialString.Contains("*"))
            {
                string[] theStringComponents = initialString.Split('*');
                initialString = "";
                foreach (string theComponent in theStringComponents)
                    initialString += theComponent;
            }
            return initialString;
        }

        public static bool UseVerticalColumns(R8RDataAccess dataAccess, int TblTabID, int? limitToThisTblColumnID, bool isTblRowPage)
        {
            var tblColumnNames = dataAccess.R8RDB.GetTable<TblColumn>()
               .Where(x => x.TblTabID == TblTabID
                       && (limitToThisTblColumnID == null || x.TblColumnID == limitToThisTblColumnID)
                       && x.Status == (byte)StatusOfObject.Active).Select(x => ((x.Abbreviation == "") ? x.Name : x.Abbreviation).Trim()).ToList();
            int approxTotalPixels = tblColumnNames.Count * 20 + tblColumnNames.Sum(x => x.Length) * 8;
            return approxTotalPixels > (isTblRowPage ? 500 : 300);
        }

    }
}