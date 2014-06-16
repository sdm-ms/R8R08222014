using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ClassLibrary1.Model
{

    [Serializable]
    public class SortMenuItem
    {
        public string M; /* MenuText -- short to save html */
        public string I; /* ItemInstruction */
    }

    public static class SortMenuGenerator
    {
        public static List<SortMenuItem> GetSortMenuForTblTab(IR8RDataContext theDataContext, int theTblTabID, bool trustedUser)
        {
            string cacheKey = "SortMenu" + theTblTabID + "," + trustedUser.ToString();
            List<SortMenuItem> theSortMenu = CacheManagement.GetItemFromCache(cacheKey) as List<SortMenuItem>;
            if (theSortMenu == null)
            {
                TblTab theCG = theDataContext.GetTable<TblTab>().Single(cg => cg.TblTabID == theTblTabID);
                theSortMenu = GetSortMenuForTblTab(theCG, trustedUser);
                CacheManagement.AddItemToCache(cacheKey, new string[] { "ColumnsForTblID" + theTblTabID }, theSortMenu, new TimeSpan(5, 0, 0));
            }
            return theSortMenu;
        }

        public static List<SortMenuItem> GetSortMenuForTblTab(TblTab theTblTab, bool trustedUser)
        {

            List<SortMenuItem> theSortMenu = new List<SortMenuItem>();

            var addressFieldDefinitions = theTblTab.Tbl.FieldDefinitions.Where(x => x.Status == (int)StatusOfObject.Active && x.FieldType == (int)FieldTypes.AddressField);
            bool hasAddress = addressFieldDefinitions.Any();
            if (hasAddress)
            {
                int fieldDefinitionID = addressFieldDefinitions.OrderBy(x => x.FieldNum).First().FieldDefinitionID;
                theSortMenu.Add(new SortMenuItem { M = "Distance From You", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleDistance(-1, -1, fieldDefinitionID, true)) });
                theSortMenu.Add(new SortMenuItem { M = "Distance From Center of Map", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleDistance(-2, -2, fieldDefinitionID, true)) });
                //theSortMenu.Add(new SortMenuItem { M = "Distance From Other Location...", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleDistance(0, 0, true)) });
            }

            theSortMenu.Add(new SortMenuItem { M = theTblTab.Tbl.TypeOfTblRow /* + " (A-Z)" */, I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleRowName(true)) });
            /* theSortMenu.Add(new SortMenuItem { M = theTblTab.Tbl.TypeOfTblRow + " (Z-A)", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleRowName(false)) }); */

            IQueryable<TblColumn> theCDs = theTblTab.TblColumns.AsQueryable().Where(x => x.Sortable == true && x.Status == (int)StatusOfObject.Active).OrderBy(x => x.CategoryNum).ThenBy(x => x.TblColumnID);
            foreach (var cd in theCDs)
                theSortMenu.Add(GetSortMenuItemForTblColumn(cd, false, false));
            /* theSortMenu.AddRange(GetSortMenuItemsForTblColumn(cd)); */
            /* uncomment to include A-Z and Z-A */

            theSortMenu.Add(new SortMenuItem { M = "Most Active (Last Hour)", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleActivityLevel(VolatilityDuration.oneHour, false)) });
            theSortMenu.Add(new SortMenuItem { M = "Most Active (Last Day)", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleActivityLevel(VolatilityDuration.oneDay, false)) });
            theSortMenu.Add(new SortMenuItem { M = "Most Active (Last Week)", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleActivityLevel(VolatilityDuration.oneWeek, false)) });
            theSortMenu.Add(new SortMenuItem { M = "Most Active (Last Year)", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleActivityLevel(VolatilityDuration.oneYear, false)) });

            theSortMenu.Add(new SortMenuItem { M = "Newest in R8R", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleNewestInDatabase(false)) });

            if (trustedUser)
                theSortMenu.Add(new SortMenuItem { M = "Most Needs Rating", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleNeedsRating()) });
            else
                theSortMenu.Add(new SortMenuItem { M = "Most Needs Rating", I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleNeedsRatingUntrustedUser()) });

            return theSortMenu;
        }

        public static List<SortMenuItem> GetSortMenuItemsForTblColumn(TblColumn theCD)
        {
            return new List<SortMenuItem> { GetSortMenuItemForTblColumn(theCD, false, true), GetSortMenuItemForTblColumn(theCD, true, true) };
        }

        internal static SortMenuItem GetSortMenuItemForTblColumn(TblColumn theCD, bool ascending, bool includeRange)
        {
            SortMenuItem theItem = new SortMenuItem();
            string min = (Math.Round(theCD.RatingGroupAttribute.RatingCharacteristic.MinimumUserRating, 0)).ToString();
            string max = (Math.Round(theCD.RatingGroupAttribute.RatingCharacteristic.MaximumUserRating, 0)).ToString();
            string range = " " + (ascending ? "(" + min + "-" + max + ")" : "(" + max + "-" + min + ")");
            theItem.M = theCD.Name;
            if (includeRange)
                theItem.M += range;
            theItem.I = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleTblColumn(theCD.TblColumnID, ascending));
            return theItem;
        }
    }

    public static class TableSortRuleGenerator
    {
        public static string GetStringRepresentationFromTableSortRule(TableSortRule theTableSortRule)
        {
            return theTableSortRule.GetStringRepresentation();
        }

        public static TableSortRule GetTableSortRuleFromStringRepresentation(string theString)
        {
            try
            {
                string[] split = theString.Split(',');
                switch (split[0])
                {
                    case "C":
                        return new TableSortRuleTblColumn(Convert.ToInt32(split[1]), Convert.ToBoolean(split[2]));

                    case "D":
                        return new TableSortRuleDistance((float)Convert.ToDecimal(split[1]), (float)Convert.ToDecimal(split[2]), Convert.ToInt32(split[3]), Convert.ToBoolean(split[4]));

                    case "E":
                        return new TableSortRuleRowName(Convert.ToBoolean(split[1]));

                    case "N":
                        return new TableSortRuleNewestInDatabase(Convert.ToBoolean(split[1]));

                    case "A":
                        return new TableSortRuleActivityLevel((VolatilityDuration)Convert.ToInt32(split[1]), Convert.ToBoolean(split[2]));

                    case "R":
                        return new TableSortRuleNeedsRating();

                    case "U":
                        return new TableSortRuleNeedsRatingUntrustedUser();
                }
                throw new Exception("Internal error: sort rule not defined.");
            }
            catch
            {
                throw new Exception("Internal error: sort rule not defined.");
            }

        }
    }

    public abstract class TableSortRule
    {
        public bool Ascending;
        public TableSortRule(bool ascending)
        {
            Ascending = ascending;
        }
        public abstract string GetStringRepresentation();
    }

    public class TableSortRuleTblColumn : TableSortRule
    {
        public int TblColumnToSortID;
        public TableSortRuleTblColumn(int tblColumnToSortID, bool ascending)
            : base(ascending)
        {
            TblColumnToSortID = tblColumnToSortID;
        }
        public override string GetStringRepresentation()
        {
            return "C" + "," + TblColumnToSortID + "," + Ascending.ToString();
        }
    }

    public class TableSortRuleDistance : TableSortRule
    {
        public float Latitude;
        public float Longitude;
        public int FieldDefinitionID;
        public TableSortRuleDistance(float latitude, float longitude, int fieldDefinitionID, bool ascending)
            : base(ascending)
        {
            Latitude = latitude;
            Longitude = longitude;
            FieldDefinitionID = fieldDefinitionID;
        }
        public override string GetStringRepresentation()
        {
            return "D" + "," + Latitude + "," + Longitude + "," + FieldDefinitionID + "," + Ascending.ToString();
        }
    }

    public class TableSortRuleRowName : TableSortRule
    {
        public TableSortRuleRowName(bool ascending)
            : base(ascending)
        {
        }
        public override string GetStringRepresentation()
        {
            return "E" + "," + Ascending.ToString();
        }
    }

    public class TableSortRuleNewestInDatabase : TableSortRule
    {
        public TableSortRuleNewestInDatabase(bool ascending)
            : base(ascending)
        {
        }
        public override string GetStringRepresentation()
        {
            return "N" + "," + Ascending.ToString();
        }
    }

    public class TableSortRuleActivityLevel : TableSortRule
    {
        public VolatilityDuration TimeFrame;
        public TableSortRuleActivityLevel(VolatilityDuration timeFrame, bool ascending)
            : base(ascending)
        {
            TimeFrame = timeFrame;
        }
        public override string GetStringRepresentation()
        {
            return "A" + "," + (int)TimeFrame + "," + Ascending.ToString();
        }
    }

    public class TableSortRuleNeedsRating : TableSortRule
    {
        public TableSortRuleNeedsRating()
            : base(false)
        {
        }
        public override string GetStringRepresentation()
        {
            return "R";
        }
    }

    public class TableSortRuleNeedsRatingUntrustedUser : TableSortRule
    {
        public TableSortRuleNeedsRatingUntrustedUser()
            : base(false)
        {
        }
        public override string GetStringRepresentation()
        {
            return "U";
        }
    }


}
