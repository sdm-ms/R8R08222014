using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreStrings;

using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    public static class RatingAndUserRatingStringConverter
    {
        public static bool AddRatingIDsToList(List<RatingAndUserRatingString> theUserRatingsString, List<Guid> theRatingIDs)
        {
            bool ratingIDsProperlyFormatted = true;
            if (theUserRatingsString.Count() > 1)
                throw new Exception("Must revise this for multiple ratings in rating group, since after adding the rating group we'll need to get each individual rating.");
            foreach (RatingAndUserRatingString theRatingAndUserRatingString in theUserRatingsString)
            {
                Guid aRatingID = new Guid();
                //if (!MoreStringManip.IsInteger(theRatingAndUserRatingString.ratingID, ref aRatingID))
                //    ratingIDsProperlyFormatted = false;
                //else
               theRatingIDs.Add(aRatingID);
            }
            return ratingIDsProperlyFormatted;
        }
    }

    public static class RatingsAndRelatedInfoLoader
    {
        public static List<Rating> Load(IR8RDataContext theDataContext, List<Guid> ratingIDs, User theUser)
        {
            
            var results = (from x in theDataContext.GetTable<Rating>()
                          where ratingIDs.Contains(x.RatingID)
                          let ratingGroup = x.RatingGroup
                          let pointsManager = ratingGroup.TblRow.Tbl.PointsManager
                          let tblColumn = ratingGroup.TblColumn
                          let pmTrustTrackerUnit = pointsManager.TrustTrackerUnit
                          let tcTrustTrackerUnit = tblColumn.TrustTrackerUnit
                          let trustTrackerUnit = tcTrustTrackerUnit ?? pmTrustTrackerUnit // Note: Cannot access properties on this.
                          let trustTracker = (tcTrustTrackerUnit == null) ? pmTrustTrackerUnit.TrustTrackers.SingleOrDefault(y => y.User == theUser) : pmTrustTrackerUnit.TrustTrackers.SingleOrDefault(y => y.User == theUser)
                          let ratingCharacteristic = x.RatingCharacteristic
                          let tblRow = ratingGroup.TblRow
                           let tbl = tblRow.Tbl
                          let subsidyDensityRangeGroup = ratingCharacteristic.SubsidyDensityRangeGroup
                          //let choiceGroupFieldDefinitions = tbl.FieldDefinitions.Where(y => y.Status == (int) StatusOfObject.Active).SelectMany(y => y.ChoiceGroupFieldDefinitions).Where(y => y.TrackTrustBasedOnChoices).Select(y => y.ChoiceGroupFieldDefinitionID)
                          let choiceInGroupIDs = tblRow.Fields.SelectMany(y => y.ChoiceFields).Where(y => y.Field.FieldDefinition.ChoiceGroupFieldDefinitions.Any(z => z.TrackTrustBasedOnChoices)).SelectMany(y => y.ChoiceInFields).Select(z => z.ChoiceInGroup)
                           let trustTrackerForChoiceInGroups = ratingGroup.TblRow.Tbl.TrustTrackerForChoiceInGroups.Where(u => u.User == theUser && choiceInGroupIDs.Any(z => z.ChoiceInGroupID == u.ChoiceInGroupID))
                          select new
                          {
                              Rating = x,
                              RatingGroup = x.RatingGroup,
                              TopRatingGroup = x.TopRatingGroup,
                              RatingGroupResolutions = x.TopRatingGroup.RatingGroupResolutions,
                              RatingGroupAttribute = x.RatingGroup.RatingGroupAttribute,
                              TopRatingGroupAttribute = x.TopRatingGroup.RatingGroupAttribute,
                              RatingCharacteristic = ratingCharacteristic,
                              SubsidyDensityRangeGroup = subsidyDensityRangeGroup,
                              TblRow = tblRow,
                              Tbl = tbl,
                              PointsManager = pointsManager,
                              TblColumn = tblColumn,
                              TrustTrackerUnit = trustTrackerUnit,
                              TrustTracker = trustTracker,
                              TrustTrackerStats = trustTracker.TrustTrackerStats,
                              VolatilityTrackers = x.RatingGroup.VolatilityTrackers,
                              TrustTrackerForChoiceInFields = trustTrackerForChoiceInGroups
                          }).ToList();

            return results.Select(x => x.Rating).ToList();
        }
    }

    public class RatingAndUserRatingString
    {
        public string ratingID;
        public string theUserRating;
    }

    public class RatingIdAndUserRatingValue
    {
        public Guid RatingID;
        public decimal UserRatingValue;
    }

    public class RatingAndCurrentValue
    {
        public Guid ratingID;
        public decimal? theValue;
    }

    public class RatingCurrentValueAndDecimalPlaces
    {
        public Guid ratingID;
        public decimal? theValue;
        public int decimalPlaces;
    }

    public class UserRatingResult
    {
        public bool success;
        public string userMessage;

        public UserRatingResult()
        {
            success = true;
            userMessage = "";
        }

        public UserRatingResult(string theErrorMessage)
        {
            success = false;
            userMessage = theErrorMessage;
        }
    }

    public class UserEditResponse
    {
        public UserRatingResult result;
        public List<RatingAndUserRatingString> currentValues;
    }

    public class TablePopulateResponse
    {
        public bool success;
        public string err; // error tracking
        public int? rowCount; // null when not recalculating row count
        public string mainRows;
        public string headerRow;
        public string tableInfoForReset;
    }

    public class UserAccessInfo
    {
        public string userName;
        public string passwordForWebService;
    }
}
