using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

using GoogleGeocoder;

using System.Web.Script.Serialization;
using System.Data.Linq;
using System.Linq.Expressions;

using System.Diagnostics;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{

    public class TblRowsToPopulatePage
    {
        public List<TblRow> theTblRows;
        public int numRowOfFirstTblRow;
        public int? rowCount;
    }

    [Serializable]
    public abstract class FilterRule
    {
        public bool FilterOn { get; set; }
        public int theID { get; set; }

        public FilterRule() // parameterless constructor for deserialization
        {
        }

        public FilterRule(int relevantID) // could be TblColumnID, FieldDefinitionID, or TblID 
        {
            theID = relevantID;
            FilterOn = true;
        }

        public abstract IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate);
    }

    public static class FilterRulesSerializer
    {
        public static string GetStringFromFilterRules(FilterRules theFilterRules)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new FilterRuleConverter() });
            var theOutput = serializer.Serialize(theFilterRules);

            return theOutput;
        }

        public static FilterRules GetFilterRulesFromString(string theFilterRulesString)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new FilterRuleConverter() });
            FilterRules theOutput = (FilterRules)serializer.Deserialize<FilterRules>(theFilterRulesString);
            theOutput.AsOfDateTime = ((DateTime)theOutput.AsOfDateTime).ToLocalTime();
            return theOutput;
        }

        public static FilterRules GetFilterRulesFromUrlEncodedString(string theFilterRulesString)
        {
            return GetFilterRulesFromString(HttpUtility.UrlDecode(theFilterRulesString));
        }
    }

    /// <summary>
    /// Summary description for FilterRules
    /// </summary>
    [Serializable]
    public class FilterRules
    {
        public int TblID { get; set; }
        public bool ActiveOnly { get; set; } // true if we only want to return table rows where trading is active
        public bool HighStakesOnly { get; set; } // true if we only want to return table rows that include rating groups with the HighStakes flag set.
        public List<FilterRule> theFilterRules { get; set; }
        public bool GetNewDateTime { get; set; }
        public DateTime AsOfDateTime { get; set; }

        public FilterRules()
        {
            GetNewDateTime = false;
            AsOfDateTime = DateTimeManip.RoundDateTimeSeconds(TestableDateTime.Now, 15, DateTimeManip.eRoundingDirection.down); // Round down to nearest 15 seconds, so that we can cache effectively over short periods of time
        }

        public FilterRules(int tblID, bool activeOnly, bool highStakesOnly)
            : this()
        {
            TblID = tblID;
            ActiveOnly = activeOnly;
            HighStakesOnly = highStakesOnly;
            theFilterRules = new List<FilterRule>();
        }

        public void AddFilterRule(FilterRule theFilterRule)
        {
            theFilterRules.Add(theFilterRule);
        }

        public IQueryable<int> GetFilteredQuery(IR8RDataContext theDataContext, int? maxNumResults)
        {
            IQueryable<TblRow> theQuery = GetFilteredQueryAsTblRows(theDataContext, maxNumResults);
            var theQueryInt = theQuery.Select(x => x.TblRowID);
            return theQueryInt;
        }

        public class TblRowsToIncludeOrExclude
        {
            public TblRow tblRow;
            public bool? include;
        }

        public IQueryable<TblRow> GetFilteredQueryAsTblRows(IR8RDataContext theDataContext, int? maxNumResults)
        {
            IQueryable<TblRow> theQuery = null;

            theQuery = theDataContext.GetTable<TblRow>().Where(x => x.TblID == TblID && x.InitialFieldsDisplaySet);

            foreach (var r in theFilterRules)
            {
                if (r.FilterOn)
                {
                    theQuery = r.GetFilteredQuery(theDataContext, theQuery, null);
                }
            }

            theQuery = IdentifyTblRowsWithCorrectStatusAtTimeOfQuery(theQuery);

            if (HighStakesOnly)
                theQuery = theQuery.Where(x => x.RatingGroups.Any(y => y.HighStakesKnown));

            if (maxNumResults != null) // usually it will be null, since we're sorting later
                theQuery = theQuery.Take((int)maxNumResults);

            return theQuery;
        }

        private IQueryable<TblRow> IdentifyTblRowsWithCorrectStatusAtTimeOfQuery(IQueryable<TblRow> theQuery)
        {
            if (ActiveOnly)
            {
                // Organizing the search around StatusRecentlyChanged considerably speeds the query, because the TblRowStatusRecords only need to be examined for the small number of table rows that have been recently changed.
                theQuery = theQuery
                    .Where(x =>
                        (x.StatusRecentlyChanged == false && x.Status == (int)StatusOfObject.Active) /* active now and that's not a recent thing */
                        || /* there have been recent changes, but either ... */
                        x.StatusRecentlyChanged == true &&
                            /* none of these recent changes was after the relevant time and it's still active */
                            ((x.Status == (int)StatusOfObject.Active && !x.TblRowStatusRecords.Any(yield => yield.TimeChanged > AsOfDateTime))) ||
                            /* first change indicates that it was active as of the relevant time */
                            (x.TblRowStatusRecords.Where(y => y.TimeChanged > AsOfDateTime)
                                    .OrderBy(y => y.TimeChanged)
                                    .FirstOrDefault().Deleting == true)
                            );
            }
            else
            {
                theQuery = theQuery
                    .Where(x =>
                        (x.StatusRecentlyChanged == false) /* no recent status changes, so must have existed as of query time */
                        || /* there have been recent changes, but either ... */
                        x.StatusRecentlyChanged == true &&
                            /* none of these recent changes was after the relevant time */
                            ((!x.TblRowStatusRecords.Any(yield => yield.TimeChanged > AsOfDateTime))) ||
                            /* first change indicates that it was in existence as of the relevant time */
                            (x.TblRowStatusRecords.Where(y => y.TimeChanged > AsOfDateTime)
                                    .OrderBy(y => y.TimeChanged)
                                    .FirstOrDefault().Adding == false)
                            );
            }
            return theQuery;
        }

        public TblRowsToPopulatePage GetQueryForSpecificRows(IR8RDataContext theDataContext, int? maxNumResults, TableSortRule theSortRule, bool sortByNameAfterTakingTop, bool nameAscending, int firstRowNum, int numRows, bool returnRowNumForBaseQuery)
        {
            if (numRows > 30)
                numRows = 30;
            IQueryable<TblRow> theBaseQuery = GetFilteredAndSortedQuery(theDataContext, maxNumResults, theSortRule, sortByNameAfterTakingTop, nameAscending);
            //int theRowCount = theBaseQuery.Count();
            //var rowsToTake = theBaseQuery.Skip(firstRowNum - 1).Take(numRows).ToList();
            TblRowsToPopulatePage theTblRowsToReturn = new TblRowsToPopulatePage
            {
                theTblRows = theBaseQuery.Skip(firstRowNum - 1).Take(numRows).ToList(),
                numRowOfFirstTblRow = firstRowNum,
                rowCount = null
            };
            if (returnRowNumForBaseQuery)
                theTblRowsToReturn.rowCount = theBaseQuery.Count();
            return theTblRowsToReturn;
        }


        public TblRowsToPopulatePage GetQueryToPopulatePageInitially(IR8RDataContext theDataContext, int? maxNumResults, TableSortRule theSortRule, bool sortByNameAfterTakingTop, bool nameAscending, int numRows, int rowsToSkip)
        {
            int theRowCount = 0;
            if (numRows > 75)
                numRows = 30;
            IQueryable<TblRow> theBaseQuery = GetFilteredAndSortedQuery(theDataContext, maxNumResults, theSortRule, sortByNameAfterTakingTop, nameAscending);
            theRowCount = theBaseQuery.Count();
            var rowsToTake = theBaseQuery.Skip(rowsToSkip).Take(numRows).ToList();
            TblRowsToPopulatePage theTblRowsToReturn = new TblRowsToPopulatePage
                    {
                        theTblRows = rowsToTake,
                        numRowOfFirstTblRow = rowsToSkip + 1,
                        rowCount = theRowCount
                    };
            return theTblRowsToReturn;
        }

        public IQueryable<TblRow> GetFilteredAndSortedQuery(IR8RDataContext theDataContext, int? maxNumResults, TableSortRule theSortRule, bool sortByNameAfterTakingTop, bool nameAscending)
        {
            IQueryable<TblRow> theTblRowsWithRatings = null;
            if (theSortRule is TableSortRuleRowName)
                theTblRowsWithRatings = GetFilteredQuerySortedByName(theDataContext, maxNumResults, theSortRule.Ascending); // When sorting by name, we will show all entries, even though this takes longer, so users can use the scrollbar to find a particular one
            else
            {
                if (theSortRule is TableSortRuleTblColumn)
                    theTblRowsWithRatings = GetFilteredQuerySortedByColumn(theDataContext, maxNumResults, ((TableSortRuleTblColumn)theSortRule).TblColumnToSortID, theSortRule.Ascending);
                else if (theSortRule is TableSortRuleActivityLevel)
                    theTblRowsWithRatings = GetFilteredQuerySortedByActivityLevel(theDataContext, ((TableSortRuleActivityLevel)theSortRule).TimeFrame, maxNumResults, !theSortRule.Ascending);
                else if (theSortRule is TableSortRuleDistance)
                    theTblRowsWithRatings = GetFilteredQuerySortedByAddress(theDataContext, maxNumResults, ((TableSortRuleDistance)theSortRule).Latitude, ((TableSortRuleDistance)theSortRule).Longitude, this.TblID);
                else if (theSortRule is TableSortRuleNewestInDatabase)
                    theTblRowsWithRatings = GetFilteredQuerySortedByNewest(theDataContext, maxNumResults, theSortRule.Ascending);
                else if (theSortRule is TableSortRuleNeedsRating)
                    theTblRowsWithRatings = GetFilteredQuerySortedByNeedsRating(theDataContext, maxNumResults);
                else if (theSortRule is TableSortRuleNeedsRatingUntrustedUser)
                    theTblRowsWithRatings = GetFilteredQuerySortedByNeedsRatingUntrustedUser(theDataContext, maxNumResults);
                else
                    throw new Exception("Internal error: unknown sort rule.");

                if (sortByNameAfterTakingTop)
                    theTblRowsWithRatings = AddSortingByNameToFilteredQuery(null, nameAscending, theTblRowsWithRatings);
            }
            return theTblRowsWithRatings;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByColumn(IR8RDataContext theDataContext, int? maxNumResults, int TblColumnToSort, bool ascending)
        {


            IQueryable<TblRow> theFilteredQueryWithSortValues = null;

            if (ascending)
                theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                                 let theMG = e.RatingGroups.FirstOrDefault(x => x.TblColumnID == TblColumnToSort)
                                                 orderby ((theMG.ValueRecentlyChanged == false) ? theMG.CurrentValueOfFirstRating :
                                                     (theMG.RatingGroupStatusRecords.Any(sr => sr.NewValueTime > AsOfDateTime) ? theMG.RatingGroupStatusRecords.First(sr => sr.NewValueTime > AsOfDateTime).OldValueOfFirstRating : theMG.CurrentValueOfFirstRating)), e.Name
                                                 select e;
            else
                theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                                 let theMG = e.RatingGroups.FirstOrDefault(x => x.TblColumnID == TblColumnToSort)
                                                 orderby ((theMG.ValueRecentlyChanged == false) ? theMG.CurrentValueOfFirstRating :
                                                     (theMG.RatingGroupStatusRecords.Any(sr => sr.NewValueTime > AsOfDateTime) ? theMG.RatingGroupStatusRecords.First(sr => sr.NewValueTime > AsOfDateTime).OldValueOfFirstRating : theMG.CurrentValueOfFirstRating)) descending, e.Name
                                                 select e;

            //theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
            //                                 let theMG = e.RatingGroups.FirstOrDefault(x => x.TblColumnID == TblColumnToSort)
            //                                 orderby ((theMG.ValueRecentlyChanged == false) ? theMG.CurrentValueOfFirstRating : (theMG.RatingGroupStatusRecords.First(sr => sr.NewValueTime > AsOfDateTime).OldValueOfFirstRating ?? theMG.CurrentValueOfFirstRating)) descending, e.Name
            //                                 select e;

            if (maxNumResults != null)
                theFilteredQueryWithSortValues = theFilteredQueryWithSortValues.Take((int)maxNumResults);

            return theFilteredQueryWithSortValues;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByName(IR8RDataContext theDataContext, int? maxNumResults, bool ascending)
        {
            IQueryable<TblRow> theFilteredQuery = GetFilteredQueryAsTblRows(theDataContext, null);

            IQueryable<TblRow> theFilteredAndSortedQuery = AddSortingByNameToFilteredQuery(maxNumResults, ascending, theFilteredQuery);

            return theFilteredAndSortedQuery;
        }

        private static IQueryable<TblRow> AddSortingByNameToFilteredQuery(int? maxNumResults, bool ascending, IQueryable<TblRow> theFilteredQuery)
        {
            IQueryable<TblRow> theFilteredAndSortedQuery = null;

            if (ascending)
                theFilteredAndSortedQuery = theFilteredQuery
                    .OrderBy(x => x.Name).ThenBy(x => x.TblRowID); // OK to use ID as a secondary ordering
            else
                theFilteredAndSortedQuery = theFilteredQuery
                    .OrderByDescending(x => x.Name).ThenBy(x => x.TblRowID); // OK to use ID as a secondary ordering

            if (maxNumResults != null)
                theFilteredAndSortedQuery = theFilteredAndSortedQuery.Take((int)maxNumResults);
            return theFilteredAndSortedQuery;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByAddress(IR8RDataContext theDataContext, int? maxNumResults, float latitude, float longitude, int tblID)
        {
            IQueryable<TblRow> theFilteredQueryWithSortValues = null;

            if (maxNumResults == null)
                maxNumResults = 1000000000;

            throw new NotImplementedException(); // must copy stored procedure to Entity Framework context if we need to reimplement this

            //theFilteredQueryWithSortValues =
            //        from y in theDataContext.UDFNearestNeighborsForTbl(latitude, longitude, maxNumResults, tblID)
            //        join e in GetFilteredQueryAsTblRows(theDataContext, null)
            //        on y.TblRowID equals e.TblRowID
            //        select e;

            //return theFilteredQueryWithSortValues;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByNeedsRating(IR8RDataContext theDataContext, int? maxNumResults)
        {


            IQueryable<TblRow> theFilteredQueryWithSortValues = null;

            theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                             orderby e.Status, e.ElevateOnMostNeedsRating descending, e.CountNonnullEntries, e.CountUserPoints
                                             select e;

            if (maxNumResults != null)
                theFilteredQueryWithSortValues = theFilteredQueryWithSortValues.Take((int)maxNumResults);

            return theFilteredQueryWithSortValues;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByNeedsRatingUntrustedUser(IR8RDataContext theDataContext, int? maxNumResults)
        {


            IQueryable<TblRow> theFilteredQueryWithSortValues = null;

            theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                             orderby e.Status, e.CountNonnullEntries, e.CountUserPoints
                                             select e;

            if (maxNumResults != null)
                theFilteredQueryWithSortValues = theFilteredQueryWithSortValues.Take((int)maxNumResults);

            return theFilteredQueryWithSortValues;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByNewest(IR8RDataContext theDataContext, int? maxNumResults, bool ascending)
        {


            IQueryable<TblRow> theFilteredQueryWithSortValues = null;

            if (ascending)
                theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                                 orderby e.TblRowID
                                                 select e;
            else
                theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                                 orderby e.TblRowID descending
                                                 select e;

            if (maxNumResults != null)
                theFilteredQueryWithSortValues = theFilteredQueryWithSortValues.Take((int)maxNumResults);

            return theFilteredQueryWithSortValues;
        }

        public IQueryable<TblRow> GetFilteredQuerySortedByActivityLevel(IR8RDataContext theDataContext, VolatilityDuration theTimeFrame, int? maxNumResults, bool mostActiveFirst)
        {


            IQueryable<TblRow> theFilteredQueryWithSortValues = null;

            if (mostActiveFirst)
                theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                                 orderby e.VolatilityTblRowTrackers.Single(x => x.DurationType == (int)theTimeFrame).Pushback descending
                                                 select e;
            else
                theFilteredQueryWithSortValues = from e in GetFilteredQueryAsTblRows(theDataContext, null)
                                                 orderby e.VolatilityTblRowTrackers.Single(x => x.DurationType == (int)theTimeFrame).Pushback
                                                 select e;

            if (maxNumResults != null)
                theFilteredQueryWithSortValues = theFilteredQueryWithSortValues.Take((int)maxNumResults);

            return theFilteredQueryWithSortValues;
        }

    }

    [Serializable]
    public class AddressFilterRule : FilterRule
    {
        public string Address { get; set; }
        public decimal? Mile { get; set; }

        public AddressFilterRule(int FieldDefinitionID, string address, decimal? maximumMilesDistance)
            : base(FieldDefinitionID)
        {
            Address = address;
            if (maximumMilesDistance == 0)
                Mile = (decimal)0.05;
            else
                Mile = maximumMilesDistance;
        }

        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {

            Coordinate ObjCod = new Coordinate();
            ObjCod = Geocode.GetCoordinates(Address);
            float Latitude1 = (float)Math.Round(ObjCod.Latitude, 4);
            float Longitude1 = (float)Math.Round(ObjCod.Longitude, 4);
            if (Latitude1 == 0 && Longitude1 == 0)
                return new List<TblRow>().AsQueryable();
            float mile = (float)Mile;

            IQueryable<TblRow> theQuery =
                from x in querySoFar.SelectMany(x => x.Fields.Where(z => z.FieldDefinitionID == theID))
                join y in theDataContext.UDFDistanceWithin(Latitude1, Longitude1, mile) on x.FieldID equals y.FieldID
                select x.TblRow;

            if (predicate != null)
                theQuery = theQuery.Where(predicate);

            return theQuery;
        }
    }

    [Serializable]
    public class ChoiceFilterRule : FilterRule
    {
        public int ChoiceInGroupID { get; set; }

        public ChoiceFilterRule() // for deserialization
        {
        }

        public ChoiceFilterRule(int FieldDefinitionID, int choiceInGroupID)
            : base(FieldDefinitionID)
        {
            ChoiceInGroupID = choiceInGroupID;
        }


        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {

            IQueryable<TblRow> theQuery =
                querySoFar
                    .Where(y => y.Fields
                    .Any(z => z.FieldDefinitionID == theID && z.ChoiceFields
                    .Any(w => w.ChoiceInFields
                    .Any(x => x.ChoiceInGroupID == ChoiceInGroupID))));

            if (predicate != null)
                theQuery = theQuery.Where(predicate);
            return theQuery;
        }
    }

    [Serializable]
    public class DateTimeFilterRule : FilterRule
    {
        public DateTime? MinValue { get; set; }
        public DateTime? MaxValue { get; set; }

        public DateTimeFilterRule(int FieldDefinitionID, DateTime? minValue, DateTime? maxValue)
            : base(FieldDefinitionID)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {


            IQueryable<TblRow> theQuery =
                querySoFar
                    .Where(y => y.Fields
                    .Any(z => z.FieldDefinitionID == theID && z.DateTimeFields
                    .Any(w => (MinValue == null || w.DateTime >= MinValue) && (MaxValue == null || w.DateTime <= MaxValue) && w.DateTime != null)));

            if (predicate != null)
                theQuery = theQuery.Where(predicate);
            return theQuery;
        }
    }


    [Serializable]
    public class NumberFilterRule : FilterRule
    {
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public NumberFilterRule(int FieldDefinitionID, decimal? minValue, decimal? maxValue)
            : base(FieldDefinitionID)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {

            IQueryable<TblRow> theQuery =
                querySoFar
                    .Where(y => y.Fields
                    .Any(z => z.FieldDefinitionID == theID && z.NumberFields
                    .Any(w => (MinValue == null || w.Number >= MinValue) && (MaxValue == null || w.Number <= MaxValue) && w.Number != null)));

            if (predicate != null)
                theQuery = theQuery.Where(predicate);
            return theQuery;
        }
    }

    [Serializable]
    public class TextFilterRule : FilterRule
    {
        public string TextTags { get; set; }

        public TextFilterRule(int FieldDefinitionID, string textTags)
            : base(FieldDefinitionID)
        {
            TextTags = textTags;
        }

        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {

            IQueryable<TblRow> theQuery =
                querySoFar;

            if (TextTags != "")
            {
                theQuery = querySoFar
                    .Where(y => y.Fields
                    .Any(z => z.FieldDefinitionID == theID && z.TextFields
                    .Any(w => w.Text.StartsWith(TextTags)))); // this isn't really what we want, but we're disabling this functionality anyway
            }
            else
            {
                theQuery = querySoFar;
            }

            if (predicate != null)
                theQuery = theQuery.Where(predicate);

            return theQuery;

        }

    }

    [Serializable]
    public class TblColumnFilterRule : FilterRule
    {
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public TblColumnFilterRule(int tblColumnID, decimal? minValue, decimal? maxValue)
            : base(tblColumnID)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {

            IQueryable<TblRow> theQuery =
                querySoFar
                    .Where(y => y.RatingGroups
                    .Any(z => z.TblColumnID == theID && z.CurrentValueOfFirstRating != null && (MinValue == null || z.CurrentValueOfFirstRating > MinValue) && (MaxValue == null || z.CurrentValueOfFirstRating <= MaxValue)));

            if (predicate != null)
                theQuery = theQuery.Where(predicate);
            return theQuery;
        }

    }

    [Serializable]
    public class SearchWordsFilterRule : FilterRule
    {
        public string TheSearchWords { get; set; }

        public SearchWordsFilterRule(int TblID, string theSearchWords)
            : base(TblID)
        {
            TheSearchWords = theSearchWords;
        }

        public override IQueryable<TblRow> GetFilteredQuery(IR8RDataContext theDataContext, IQueryable<TblRow> querySoFar, System.Linq.Expressions.Expression<Func<TblRow, bool>> predicate)
        {
            throw new NotImplementedException("Searching by search word on the normalized database is no longer supported.");
            //if (TheSearchWords == null || TheSearchWords.Trim() == "")
            //    return querySoFar;
            //IQueryable<TblRow> theQuery = null;
            //IQueryable<TblRow> theSearchWordMatches = new List<TblRow>().AsQueryable(); // R8RDataManipulation.GetTblRowsForPhrase(TheSearchWords, theDataContext, theID);
            //if (theSearchWordMatches == null)
            //    return new List<TblRow>().AsQueryable();
            //if (predicate != null)
            //    theSearchWordMatches = theSearchWordMatches.Where(predicate);
            //theQuery = querySoFar.Intersect(theSearchWordMatches);

            //if (predicate != null)
            //    theQuery = theQuery.Where(predicate);

            //return theQuery;
        }
    }


    public class FilterRuleConverter : JavaScriptConverter
    {

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new List<Type>(new Type[] { 
            typeof(FilterRule),
            typeof(AddressFilterRule),
            typeof(ChoiceFilterRule),
            typeof(DateTimeFilterRule),
            typeof(NumberFilterRule),
            typeof(TextFilterRule),
            typeof(TblColumnFilterRule),
            typeof(SearchWordsFilterRule)
        });
            }
        }

        public override IDictionary<string, object> Serialize(object obj,
            JavaScriptSerializer serializer)
        {
            FilterRule theFilterRule = (FilterRule)obj;
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("id", theFilterRule.theID);
            if (theFilterRule is AddressFilterRule)
            {
                result.Add("type", "address");
                result.Add("address", ((AddressFilterRule)theFilterRule).Address);
                result.Add("miles", ((AddressFilterRule)theFilterRule).Mile);
            }
            if (theFilterRule is ChoiceFilterRule)
            {
                result.Add("type", "choice");
                result.Add("choice", ((ChoiceFilterRule)theFilterRule).ChoiceInGroupID);
            }
            if (theFilterRule is DateTimeFilterRule)
            {
                result.Add("type", "datetime");
                result.Add("min", ((DateTimeFilterRule)theFilterRule).MinValue);
                result.Add("max", ((DateTimeFilterRule)theFilterRule).MaxValue);
            }
            if (theFilterRule is NumberFilterRule)
            {
                result.Add("type", "number");
                result.Add("min", ((NumberFilterRule)theFilterRule).MinValue);
                result.Add("max", ((NumberFilterRule)theFilterRule).MaxValue);
            }
            if (theFilterRule is TextFilterRule)
            {
                result.Add("type", "text");
                result.Add("texttags", ((TextFilterRule)theFilterRule).TextTags);
            }
            if (theFilterRule is TblColumnFilterRule)
            {
                result.Add("type", "tblColumn");
                result.Add("min", ((TblColumnFilterRule)theFilterRule).MinValue);
                result.Add("max", ((TblColumnFilterRule)theFilterRule).MaxValue);
            }
            if (theFilterRule is SearchWordsFilterRule)
            {
                result.Add("type", "search");
                result.Add("search", ((SearchWordsFilterRule)theFilterRule).TheSearchWords);
            }
            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary,

            Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null || dictionary["type"] == null)
                return null;
            string theType = (string)dictionary["type"];
            if (theType == "address")
            {

                decimal? miles = null;
                if (dictionary["miles"] != null)
                    miles = Convert.ToDecimal(dictionary["miles"]);
                return new AddressFilterRule((int)dictionary["id"], (string)dictionary["address"], miles);
            }
            if (theType == "choice")
            {
                int choice = Convert.ToInt32(dictionary["choice"]);
                return new ChoiceFilterRule((int)dictionary["id"], choice);
            }
            if (theType == "datetime")
            {
                DateTime? min = null;
                DateTime? max = null;
                if (dictionary["min"] != null)
                    min = Convert.ToDateTime(dictionary["min"]);
                if (dictionary["max"] != null)
                    max = Convert.ToDateTime(dictionary["max"]);
                return new DateTimeFilterRule((int)dictionary["id"], min, max);
            }
            if (theType == "number")
            {
                decimal? min = null;
                decimal? max = null;
                if (dictionary["min"] != null)
                    min = Convert.ToDecimal(dictionary["min"]);
                if (dictionary["max"] != null)
                    max = Convert.ToDecimal(dictionary["max"]);
                return new NumberFilterRule((int)dictionary["id"], min, max);
            }
            if (theType == "text")
            {
                return new TextFilterRule((int)dictionary["id"], (string)dictionary["texttags"]);
            }
            if (theType == "tblColumn")
            {
                decimal? min = null;
                decimal? max = null;
                if (dictionary["min"] != null)
                    min = Convert.ToDecimal(dictionary["min"]);
                if (dictionary["max"] != null)
                    max = Convert.ToDecimal(dictionary["max"]);
                return new TblColumnFilterRule((int)dictionary["id"], min, max);
            }
            if (theType == "search")
            {
                return new SearchWordsFilterRule((int)dictionary["id"], (string)dictionary["search"]);
            }
            return null;
        }

    }
}