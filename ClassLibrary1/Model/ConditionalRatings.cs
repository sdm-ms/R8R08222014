using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;



namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RatingId"></param>
        //public void CheckConditionalRating(int RatingId)
        //{
            
        //    int? RatingConditionId = ObjDataAccess.GetRating(RatingId).RatingGroup.RatingGroupAttribute.RatingConditionID;
        //    if (RatingConditionId != null)
        //    {
        //        UpdateUserRatingStatusForRating(RatingId, true, true);
        //    }

        //}

        /// <summary>
        /// Checks for a rating and its corresponding rating plan whether a rating conditions object needs to be updated.
        /// </summary>
        /// <param name="ratingID">The rating (typically newly created)</param>
        /// <param name="ratingPlanID">The rating plan for the rating</param>
        //public void RegisterRatingConditions(int ratingID, int ratingPlanID, bool updateConditionalRating)
        //{



        //    var registrationsNeeded = R8RDB.GetTable<RatingCondition>().Where(mc => mc.Status == (Byte)StatusOfObject.Active
        //                                        && (mc.ConditionalRatingPlanID == ratingPlanID || mc.ConditionRatingPlanID == ratingPlanID));
        //    foreach (RatingCondition theCondition in registrationsNeeded)
        //    {
        //        if (theCondition.ConditionalRatingPlanID == ratingPlanID)
        //        {
        //            theCondition.ConditionalRatingID = ratingID;
        //            if (updateConditionalRating)
        //                UpdateUserRatingStatusForRating(ratingID, true, true);
        //        }
        //        if (theCondition.ConditionRatingPlanID == ratingPlanID)
        //            theCondition.ConditionRatingID = ratingID;
        //        if (theCondition.ConditionRatingID == theCondition.ConditionalRatingID)
        //        {
        //            theCondition.ConditionRatingID = theCondition.ConditionalRatingID = null;
        //            throw new Exception("A rating cannot be conditional on its own value.");
        //        }

        //    }

        //    R8RDB.SubmitChanges();
        //}

        /// <summary>
        /// Returns true if and only if any rating conditions for a rating are met. If not, predictions should 
        /// be resolved at 0.
        /// </summary>
        /// <param name="ratingID">The rating being checked</param>
        /// <returns>True if all conditions are met.</returns>
        public bool RatingConditionsAreMet(Rating theRating, DateTime referenceTime)
        {
            
            bool returnVal = true;
            RatingCondition MCondition = theRating.RatingGroup.RatingGroupAttribute.RatingCondition;
            // var conditionsToCheck = R8RDB.GetTable<RatingCondition>().Where(mc => mc.Status == (Byte)StatusOfObject.Active
            //                                     && mc.ConditionalRatingID == ratingID && mc.ConditionRatingID != null);

            if (MCondition != null)
            {

                decimal? referenceValue;
                UserRating theReferenceUserRating = DataContext.GetTable<UserRating>().Where(
                        p => p.RatingID == MCondition.ConditionRatingID
                            && p.UserRatingGroup.WhenMade < referenceTime)
                            .OrderByDescending(p => p.UserRatingGroup.WhenMade)
                            .FirstOrDefault();
                if (theReferenceUserRating == null)
                    referenceValue = null;
                else
                    referenceValue = theReferenceUserRating.NewUserRating;

                if (referenceValue == null)
                    returnVal = false;
                else
                {
                    if (MCondition.GreaterThan != null && referenceValue <= MCondition.GreaterThan)
                        returnVal = false;
                    else if (MCondition.LessThan != null && referenceValue >= MCondition.LessThan)
                        returnVal = false;
                }

            }
            //foreach (RatingCondition theCondition in conditionsToCheck)
            //{
            //    decimal? currentValue = R8RDB.NewOrSingle<Rating>(m => m.RatingID == theCondition.ConditionRatingID).RatingStatu.CurrentUserRatingOrFinalValue;
            //    if (currentValue == null)
            //        returnVal = false;
            //    else
            //    {
            //        if (theCondition.GreaterThan != null && currentValue <= theCondition.GreaterThan)
            //            returnVal = false;
            //        else if (theCondition.LessThan != null && currentValue >= theCondition.LessThan)
            //            returnVal = false;
            //    }
            //}
            return returnVal;
        }

        public void AddConditionalRatingForTbl(int TblColumnID,int? conditionTblColumnID,decimal? greaterThan,decimal? lessThan)
        {
           
            TblColumn theTblColumn = DataContext.GetTable<TblColumn>().Single(x=>x.TblColumnID==TblColumnID);
            theTblColumn.ConditionGreaterThan = greaterThan;
            theTblColumn.ConditionLessThan = lessThan;
            theTblColumn.ConditionTblColumnID = conditionTblColumnID;
            DataContext.SubmitChanges();

        }


    }
}
