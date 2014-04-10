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
using System.Diagnostics;
////using PredRatings;
using MoreStrings;

using ClassLibrary1.Model;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {

        // High stakes known means that at some point in this rating period (after HighStakesBecomeKnown), this row will show up as being known.
        // High stakes secret means that until HighStakesBecomeKnown, this is a super high stakes period, receiving the HighStakesSecretMultiplier, with the user's success depending on the period (within this phase status or the next one) when high stakes are known.
        // For ratings, the secret and known period are within the same phase period, since we're always projecting what people will think at the end of this period (rather than at the end of time). For events, we use different periods, since the score for ratings in the secret period will depend on the score for ratings in the period thereafter.

        public void HighStakesSettings(int pointsManagerID, decimal highStakesProbability, decimal highStakesMultiplierSecret, decimal highStakesMultiplierKnown, bool highStakesNoviceOn, int highStakesNoviceNumAutomatic, int highStakesNoviceNumOneThird, int highStakesNoviceNumOneTenth, int highStakesNoviceTargetNum, decimal databaseChangeSelectHighStakesNoviceNumPct)
        {
            PointsManager thePointsManager = DataContext.NewOrSingle<PointsManager>(x => x.PointsManagerID == pointsManagerID);
            thePointsManager.HighStakesProbability = highStakesProbability;
            thePointsManager.HighStakesSecretMultiplier = highStakesMultiplierSecret;
            thePointsManager.HighStakesKnownMultiplier = highStakesMultiplierKnown;
            thePointsManager.HighStakesNoviceOn = highStakesNoviceOn;
            thePointsManager.HighStakesNoviceNumAutomatic = highStakesNoviceNumAutomatic;
            thePointsManager.HighStakesNoviceNumOneThird = highStakesNoviceNumOneThird;
            thePointsManager.HighStakesNoviceNumOneTenth = highStakesNoviceNumOneTenth;
            thePointsManager.HighStakesNoviceTargetNum = highStakesNoviceTargetNum;
            thePointsManager.DatabaseChangeSelectHighStakesNoviceNumPct = databaseChangeSelectHighStakesNoviceNumPct;
        }

        public static void HighStakesImplementPlanForNewRatingGroupPhaseStatus(RatingGroup topRatingGroup, RatingGroupPhaseStatus newRatingGroupPhaseStatus, RatingGroupPhaseStatus oldRatingGroupPhaseStatus)
        {
            // We previously made a decision that the rating group in this new phase period would have high stakes known, if there were any ratings in the previous evaluation period. That is, this is for a table row that will pop up as high stakes. This will be called when creating a new rating group phase status, if the previous phase status was high stakes secret, with high stakes becoming known only at the end of the period (i.e., if we are dealing with an event rather than a rating).
            // We'll delay the resolve time of the previous phase until the end of this phase.
            if (oldRatingGroupPhaseStatus.UserRatingGroups.Any())
            {
                newRatingGroupPhaseStatus.HighStakesBecomeKnown = TestableDateTime.Now;
                newRatingGroupPhaseStatus.HighStakesSecret = false; // there is no secret here
                newRatingGroupPhaseStatus.HighStakesKnown = true; // it is now known that this is a high stakes row, thus disciplining the previous time period
                oldRatingGroupPhaseStatus.ShortTermResolveTime = newRatingGroupPhaseStatus.ActualCompleteTime;
            }
            else
            {
                // We do not want to do high stakes known if there were no ratings in the period to be
                // evaluated, since the purpose is to affect previous ratings.
                newRatingGroupPhaseStatus.HighStakesSecret = false;
                newRatingGroupPhaseStatus.HighStakesKnown = false;
            }
            newRatingGroupPhaseStatus.HighStakesNoviceUser = false;
            newRatingGroupPhaseStatus.HighStakesNoviceUserAfter = null;
        }

        internal static bool? highStakesRandomizeNext = null;
        // Call this to force a row to be randomized to high stakes. This is useful for testing.
        public static void HighStakesRandomizeSetNext(bool randomizeNext)
        {
            highStakesRandomizeNext = randomizeNext;
        }

        public static void HighStakesRandomize(RatingGroup topRatingGroup, RatingGroupPhaseStatus newRatingGroupPhaseStatus)
        {
            double theRandom = RandomGenerator.GetRandom();
            if (highStakesRandomizeNext == true || (highStakesRandomizeNext == null && theRandom < (double)topRatingGroup.TblRow.Tbl.PointsManager.HighStakesProbability))
                HighStakesPlan(topRatingGroup, newRatingGroupPhaseStatus);
            highStakesRandomizeNext = null;
        }

        private static void HighStakesPlan(RatingGroup topRatingGroup, RatingGroupPhaseStatus newRatingGroupPhaseStatus)
        {
            if (topRatingGroup.RatingGroupAttribute.RatingEndingTimeVaries)
            { // this is a rating rather than an event, and the evaluation point is at the ending time
                // ActualCompleteTime would be set to ShortTermResolveTime. We're going to expand the time.
                newRatingGroupPhaseStatus.HighStakesSecret = true; // for now, this is secret (so ratings now will get super high stakes), but...
                newRatingGroupPhaseStatus.HighStakesKnown = true; // later in the rating period, it will be known that this is high stakes
                newRatingGroupPhaseStatus.HighStakesBecomeKnown = newRatingGroupPhaseStatus.ShortTermResolveTime; // original short term resolve time (will change below)
                TimeSpan secretPlannedDuration = newRatingGroupPhaseStatus.ShortTermResolveTime - TestableDateTime.Now;
                TimeSpan knownPlannedDuration = TimeSpan.FromTicks((long)((double)secretPlannedDuration.Ticks * ((double)RandomGenerator.GetRandom(0.75M, 1.25M))));
                newRatingGroupPhaseStatus.ShortTermResolveTime = newRatingGroupPhaseStatus.ActualCompleteTime = ((DateTime)newRatingGroupPhaseStatus.HighStakesBecomeKnown) + knownPlannedDuration;
            }
            else
            { // this is an event, we don't change the complete times, since we will make this entire phase secret,
                // and the next one will be high stakes known up to a certain point.
                newRatingGroupPhaseStatus.HighStakesSecret = true; // for the entire rating period, this is secret (producing super high stakes)
                newRatingGroupPhaseStatus.HighStakesKnown = false; // this won't become known until the next rating period
                newRatingGroupPhaseStatus.HighStakesBecomeKnown = newRatingGroupPhaseStatus.ActualCompleteTime;
            }
        }

        public decimal HighStakesMultiplier(RatingGroupPhaseStatus theRatingGroupPhaseStatus, RatingGroup topmostRatingGroup, DateTime theTime)
        {
            // Note that this should not be called when there is a highStakesMultiplierOverride for the particular user rating.

            if (!theRatingGroupPhaseStatus.HighStakesSecret && !theRatingGroupPhaseStatus.HighStakesKnown)
                return (decimal)1;
            if (!theRatingGroupPhaseStatus.HighStakesReflected)
                return (decimal)1; /* we're pretending for now this isn't high stakes */
            if (theRatingGroupPhaseStatus.HighStakesSecret && theRatingGroupPhaseStatus.HighStakesKnown)
            { // part of this phase is secret and the next part is known.
                if (theTime < (DateTime)theRatingGroupPhaseStatus.HighStakesBecomeKnown)
                    return (decimal)topmostRatingGroup.TblRow.Tbl.PointsManager.HighStakesSecretMultiplier;
                else
                    return (decimal)(topmostRatingGroup.TblRow.Tbl.PointsManager.HighStakesKnownMultiplier ?? 10);
            }
            if (theRatingGroupPhaseStatus.HighStakesSecret)
                return (decimal)topmostRatingGroup.TblRow.Tbl.PointsManager.HighStakesSecretMultiplier;
            if (theRatingGroupPhaseStatus.HighStakesKnown)
                return (decimal)(topmostRatingGroup.TblRow.Tbl.PointsManager.HighStakesKnownMultiplier ?? 10);
            throw new Exception("This HighStakesMultiplier internal exception indicates an internal logic error. It should not ever be reached.");
        }

        public bool IdleTaskMakeHighStakesKnown()
        {
            DateTime now = TestableDateTime.Now;
            var pendingHighStakesBecomeKnown = DataContext.GetTable<RatingGroupPhaseStatus>().Where(x => ((x.HighStakesBecomeKnown != null && ((DateTime)x.HighStakesBecomeKnown) < now) || (x.HighStakesNoviceUser && ((DateTime)x.HighStakesNoviceUserAfter < now))) && !x.HighStakesReflected).Take(100).ToArray();
            foreach (var rgps in pendingHighStakesBecomeKnown)
            {
                if (!rgps.RatingGroup.HighStakesKnown)
                {
                    rgps.RatingGroup.TblRow.CountHighStakesNow++;
                    if (rgps.RatingGroup.TblRow.CountHighStakesNow == 1 && !rgps.RatingGroup.TblRow.ElevateOnMostNeedsRating)
                    {
                        rgps.RatingGroup.TblRow.Tbl.PointsManager.HighStakesNoviceNumActive++;
                        rgps.RatingGroup.TblRow.ElevateOnMostNeedsRating = true;
                    }
                    FastAccessTablesMaintenance.IdentifyRowRequiringUpdate(DataContext, rgps.RatingGroup.TblRow.Tbl, rgps.RatingGroup.TblRow, false, false);
                }
                rgps.RatingGroup.HighStakesKnown = true; // really should be called HighStakesReflected
                rgps.HighStakesReflected = true;
                foreach (var ur in rgps.UserRatingGroups.SelectMany(y => y.UserRatings))
                    ur.ForceRecalculate = true;
            }

            var currentHighStakesCompleteButNotResolved = (from x in DataContext.GetTable<RatingGroup>()
                                            where 
                x.HighStakesKnown &&
                x.RatingGroupPhaseStatus.OrderByDescending(y => y.RatingGroupPhaseStatusID).First().ActualCompleteTime < now &&
                x.TypeOfRatingGroup != (int)RatingGroupTypes.hierarchyNumbersBelow && x.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityHierarchyBelow && x.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy
                let resolution = x.RatingGroupResolutions.OrderByDescending(y => y.ExecutionTime).ThenByDescending(y => y.RatingGroupResolutionID).FirstOrDefault()
                where resolution == null || resolution.CancelPreviousResolutions
                select x)
                .Distinct().Take(100).ToArray();
            foreach (var item in currentHighStakesCompleteButNotResolved)
                AdvanceRatingGroupToNextRatingPhase(item);
            return pendingHighStakesBecomeKnown.Length == 100 || currentHighStakesCompleteButNotResolved.Length == 100; // more work to do if we just had 100 at once (unlikely)
        }

        static DateTime? waitUntilTime = null;
        static int numToSkipToConsiderDemoting = 0;
        static TimeSpan timeWithinWhichWontDemote = new TimeSpan(0, 1, 0);
        public bool IdleTaskConsiderDemotingHighStakesPrematurely()
        {
            if (waitUntilTime != null && waitUntilTime > TestableDateTime.Now)
                return false; // no work to do now.
            const int numAtOnce = 100;
            DateTime now = TestableDateTime.Now;
            var activeHighStakes = DataContext.GetTable<RatingGroupPhaseStatus>()
                .Where(x => x.HighStakesReflected && x.ActualCompleteTime > now + timeWithinWhichWontDemote)
                .Where(x => x.RatingGroup.Status == (int) (StatusOfObject.Active))
                .Select(x => new { 
                    RGPS = x, 
                    UserRatingCount = x.UserRatingGroups
                                        .Where(y => y.UserRatings.First().User.PointsTotals.SingleOrDefault(z => z.PointsManagerID == x.RatingGroup.TblRow.Tbl.PointsManagerID).TotalPoints > 0) // where the user rating group is made by a user with positive points
                                        .Select(y => y.UserRatings.First().User)
                                        .Distinct().Count()                
                })
                .Skip(numToSkipToConsiderDemoting)
                .Take(numAtOnce)
                .GroupBy(x => x.RGPS.RatingGroup.TblRow)
                .ToList();
            foreach (var a in activeHighStakes)
            {
                if (ShouldDemoteHighStakesInMostNeedsRankingSort(a.First().RGPS, (int) a.Average(x => (decimal) x.UserRatingCount)))
                    DemoteHighStakes(a.First().RGPS);
            }
            if (activeHighStakes.Any())
            {
                numToSkipToConsiderDemoting += numAtOnce;
                return true; // more work to do
            }
            else
            {
                numToSkipToConsiderDemoting = 0;
                waitUntilTime = TestableDateTime.Now + new TimeSpan(0, 5, 0);
                return false; // no more work to do
            }
        }

        internal bool ShouldDemoteHighStakesInMostNeedsRankingSort(RatingGroupPhaseStatus thePhaseStatus, int numLegitimateUsersSoFar)
        {
            const double probAnotherLegitUserNeeded = 0.7;
            int numLegitimateUsersNeeded = 1;
            // We want to get the same number for this each time we run it for a particular phase status, so we'll seed a random number generator.
            Random randomGenerator = new Random(thePhaseStatus.ActualCompleteTime.Second + thePhaseStatus.ActualCompleteTime.Minute);
            bool done = false;
            double random = 0;
            while (!done)
            {
                random = randomGenerator.NextDouble();
                if (random < probAnotherLegitUserNeeded)
                    numLegitimateUsersNeeded++;
                else
                    done = true;
            }
            //Trace.TraceInformation(String.Format("PS: {0}; Legitimate users needed: {1}; Legitimate users so far: {2}; Last random: {3}", thePhaseStatus.RatingGroupPhaseStatusID, numLegitimateUsersNeeded, numLegitimateUsersSoFar, random));
            return numLegitimateUsersSoFar >= numLegitimateUsersNeeded;
        }

        internal void DemoteHighStakes(RatingGroupPhaseStatus rgp)
        {
            if (rgp.RatingGroup.TblRow.ElevateOnMostNeedsRating)
            {
                rgp.RatingGroup.TblRow.ElevateOnMostNeedsRating = false;
                rgp.RatingGroup.TblRow.Tbl.PointsManager.HighStakesNoviceNumActive--;
            }
            FastAccessTablesMaintenance.IdentifyRowRequiringUpdate(DataContext, rgp.RatingGroup.TblRow.Tbl, rgp.RatingGroup.TblRow, false, false);
        }

        internal void TerminateHighStakesPrematurely(RatingGroupPhaseStatus rgp)
        {
            DateTime completeTime = TestableDateTime.Now + timeWithinWhichWontDemote;
            if (rgp.ActualCompleteTime == rgp.ShortTermResolveTime)
                rgp.ActualCompleteTime = rgp.ShortTermResolveTime = completeTime;
            else
                rgp.ActualCompleteTime = completeTime;
        }

        internal decimal GetNoviceHighStakesProbability(PointsManager thePointsManager, PointsTotal thePointsTotal)
        {
            decimal multiplyResultBy = 1M;
            if (thePointsManager.HighStakesNoviceTargetNum < thePointsManager.HighStakesNoviceNumActive)
                multiplyResultBy = (decimal) Math.Pow(0.97, (double)(thePointsManager.HighStakesNoviceNumActive - thePointsManager.HighStakesNoviceTargetNum));
            if (thePointsManager.HighStakesNoviceOn == false)
                return 0;
            if (thePointsTotal == null && thePointsManager.HighStakesProbability > 0)
                return multiplyResultBy; /* Brand new user -- maximum probability*/
            if (thePointsTotal.NumPendingOrFinalizedRatings <= 300)
            {
                decimal probNoviceHighStakes = 0;
                if (thePointsTotal.NumPendingOrFinalizedRatings <= thePointsManager.HighStakesNoviceNumAutomatic)
                    probNoviceHighStakes = 1;
                else if (thePointsTotal.NumPendingOrFinalizedRatings <= thePointsManager.HighStakesNoviceNumAutomatic + thePointsManager.HighStakesNoviceNumOneThird)
                    probNoviceHighStakes = 0.33M;
                else if (thePointsTotal.NumPendingOrFinalizedRatings <= thePointsManager.HighStakesNoviceNumAutomatic + thePointsManager.HighStakesNoviceNumOneThird + thePointsManager.HighStakesNoviceNumOneTenth)
                    probNoviceHighStakes = 0.1M;
                return probNoviceHighStakes * multiplyResultBy;
            }
            return 0;
        }

        public class NoviceHighStakesSettings
        {
            public bool UseNoviceHighStakes;
            public decimal? HighStakesMultiplierOverride;
        }

        internal static bool? noviceHighStakesNextAutomatic = null;
        // Call this with true to force a row to be randomized to novice high stakes. This is useful for testing.
        public static void NoviceHighStakesMakeNextAutomatic(bool? automaticSetting)
        {
            noviceHighStakesNextAutomatic = automaticSetting;
        }

        public NoviceHighStakesSettings UseNoviceHighStakes(PointsTotal thePointsTotal, PointsManager pointsManager, int tblRowID)
        {
            string cacheKey = "noviceHighStakes" + ((thePointsTotal == null) ? "NewUser" : thePointsTotal.UserID.ToString()) + "," + tblRowID.ToString();
            NoviceHighStakesSettings settings = CacheManagement.GetItemFromCache(cacheKey) as NoviceHighStakesSettings;
            if (settings != null)
                return settings;

            settings = new NoviceHighStakesSettings();
            decimal noviceHighStakesProbability = GetNoviceHighStakesProbability(pointsManager, thePointsTotal);
            settings.UseNoviceHighStakes = noviceHighStakesNextAutomatic == true || (noviceHighStakesNextAutomatic == null && RandomGenerator.GetRandom() < (double) noviceHighStakesProbability);
            noviceHighStakesNextAutomatic = null;
            if (settings.UseNoviceHighStakes)
            {
                // We want to make the multiplier equivalent in expected terms to what one would obtain if one were not a novice, placing aside known high stakes rows.
                // noviceHighStakesProbability*HighStakesMultiplierOverride + (1-noviceHighStakesProbability) * 1 = 
                // usualProbabilityOfSecretHighStakes*usualMultiplier + (1-usualProbabilityOfSecretHighStakes)* 1.
                // Thus, HighStakesMultiplierOverride = ((usualProbabilityOfSecretHighStakes*usualMultiplier + (1-usualProbabilityOfSecretHighStakes)* 1) - (1 - noviceHighStakesProbability)) / noviceHighStakesProbability.
                decimal usualProbabilityOfSecretHighStakes = pointsManager.HighStakesProbability;
                decimal usualMultiplier = pointsManager.HighStakesSecretMultiplier;
                settings.HighStakesMultiplierOverride = ((usualProbabilityOfSecretHighStakes * usualMultiplier + (1 - usualProbabilityOfSecretHighStakes) * 1) - (1 - noviceHighStakesProbability)) / noviceHighStakesProbability;
            }
            else if (noviceHighStakesProbability > 0)
                settings.HighStakesMultiplierOverride = 1; // no multiplier possible here

            CacheManagement.AddItemToCache(cacheKey, new string[] { }, settings, new TimeSpan(0,5,0));
            return settings;
        }
    }
}