using ClassLibrary1.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassLibrary1.Model
{

    public enum UserSelectableRatingTypes
    {
        standardRating,
        yesNo,
        multipleChoice,
        range
    }

    public class UserSelectedRatingInfo
    {
        public UserSelectableRatingTypes theRatingType;
        public decimal? fromRange, toRange;
        public List<string> multipleChoices;
        public Guid theColumnID;
    }

        /// <summary>
        /// Summary description for R8RSupport
        /// </summary>
        public partial class R8RDataManipulation
        {

            public OverrideCharacteristic AddOverrideCharacteristics(UserSelectedRatingInfo theUserSelectedRatingInfo, TblRow theTblRow)
            {
                TblColumn theTblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == theUserSelectedRatingInfo.theColumnID);
                RatingGroupAttribute existingRatingGroupAttribute = theTblColumn.RatingGroupAttribute;
                RatingCharacteristic existingRatingCharacteristic = existingRatingGroupAttribute.RatingCharacteristic;

                decimal minimumUserRating = (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.range) ? (decimal)theUserSelectedRatingInfo.fromRange : (decimal)0;
                decimal maximumUserRating = 100M;
                if (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.range)
                    maximumUserRating = (decimal)theUserSelectedRatingInfo.toRange;
                if (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.standardRating)
                    maximumUserRating = 10M;

                decimal? constrainedSum = null;
                if (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.multipleChoice)
                    constrainedSum = 100M;

                short decimalPlaces = 1;
                if (minimumUserRating >= -1 && maximumUserRating <= 1)
                    decimalPlaces = 3;

                bool ratingEndingTimeVaries = theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.standardRating;

                RatingGroupTypes ratingType = RatingGroupTypes.singleNumber;
                if (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.yesNo)
                    ratingType = RatingGroupTypes.probabilitySingleOutcome;
                else if (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.multipleChoice)
                    ratingType = RatingGroupTypes.probabilityMultipleOutcomes;

                int ratingCharacteristicsID = AddRatingCharacteristics(existingRatingCharacteristic.RatingPhaseGroupID, existingRatingCharacteristic.SubsidyDensityRangeGroupID, minimumUserRating, maximumUserRating, decimalPlaces, "Override Rating", null);
                int ratingGroupAttributesID = AddRatingGroupAttributes(ratingCharacteristicsID, null, constrainedSum, "OverrideRating", ratingType, "OverrideRating", null, theTblRow.Tbl.PointsManagerID, ratingEndingTimeVaries, true, 0.50M);
                if (theUserSelectedRatingInfo.theRatingType == UserSelectableRatingTypes.multipleChoice)
                {
                    int totalChoices = theUserSelectedRatingInfo.multipleChoices.Count();
                    decimal eachOne = Math.Round(100M / (decimal)totalChoices, decimalPlaces);
                    decimal lastOne = 100M - (eachOne * (totalChoices - 1));
                    int choiceNumber = 0;
                    foreach (string choice in theUserSelectedRatingInfo.multipleChoices)
                    {
                        choiceNumber++;
                        decimal thisValue = eachOne;
                        if (choiceNumber == totalChoices)
                            thisValue = lastOne;
                        AddRatingPlan(ratingGroupAttributesID, choiceNumber, thisValue, choice, choice, null);
                    }
                }
                else
                    AddRatingPlan(ratingGroupAttributesID, 1, Math.Round((minimumUserRating + maximumUserRating) / 2, decimalPlaces), "RatingOverridePlan", "RatingOverridePlan", null);

                return AddOverrideCharacteristics(DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == ratingGroupAttributesID), theTblRow, theTblColumn);
            }
        }

}