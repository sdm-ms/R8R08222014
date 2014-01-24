﻿using System;
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

using System.Diagnostics;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        //  Methods related to user points and to counting rules

        /// <summary>
        /// Update the counting rules to reflect that a user has met the ultimate standard
        /// or fallen beneath the threshold. If we're below the threshold of the minimum
        /// number of users qualifying, we'll do a check to see what the points threshold
        /// for users' predictions to count should be.
        /// </summary>
        /// <param name="theRules">The counting rules object</param>
        /// <param name="userAdded">True if a user now meets the standard, false if a user who
        /// previously met the standard no longer does</param>
        /// <param name="pointsManagerID">The universe to which these rules apply</param>
        public void UpdatePointsTrustRules(PointsTrustRule theRules, PointsManager thePointsManager, bool newUserMeetsUltimateStandard, bool userFallsOutOfUltimateStandard, bool newUserMeetsCurrentStandard, bool userFallsOutOfCurrentStandard)
        {
          
            int? previousUsersMeetingUltimateStandard = thePointsManager.NumUsersMeetingUltimateStandard;
            int? previousUsersMeetingCurrentStandard = thePointsManager.NumUsersMeetingCurrentStandard;
            if (newUserMeetsUltimateStandard)
                thePointsManager.NumUsersMeetingUltimateStandard++;
            if (userFallsOutOfUltimateStandard)
                thePointsManager.NumUsersMeetingUltimateStandard--;

            if (thePointsManager.NumUsersMeetingUltimateStandard >= theRules.MinimumUsersCounting)
            { // Make sure our current standard is our ultimate standard
                if (thePointsManager.NumUsersMeetingCurrentStandard != thePointsManager.NumUsersMeetingUltimateStandard || thePointsManager.CurrentPointsToCount != theRules.UltimatePointsToCount)
                {
                    thePointsManager.NumUsersMeetingCurrentStandard = thePointsManager.NumUsersMeetingUltimateStandard;
                    thePointsManager.CurrentPointsToCount = theRules.UltimatePointsToCount;
                    DataContext.SubmitChanges();
                }
            }
            else
            { // See if we need to adjust our current standard because a user has crossed the boundary
                if (newUserMeetsCurrentStandard || userFallsOutOfCurrentStandard)
                    UpdatePointsTrustRules(theRules, thePointsManager);
            }
        }

        /// <summary>
        /// Update the counting rules by making sure that at least the specified number
        /// of users meets the standard if we're short of the ultimate standard.
        /// </summary>
        /// <param name="theRules">The counting rules</param>
        /// <param name="pointsManagerID">The universe to which these rules apply</param>
        public void UpdatePointsTrustRules(PointsTrustRule theRules, PointsManager thePointsManager)
        {
            var thePointsTotals = DataContext.WhereFromNewAndDatabase<PointsTotal>(pt => pt.PointsManager == thePointsManager).Select(pt => pt.TrustPoints); // pt.TotalPoints - theRules.CountPotentialMaxLossAgainstAt * pt.PotentialMaxLossOnNotYetPending + theRules.CountPendingAt * pt.PendingPoints);
            thePointsManager.NumUsersMeetingUltimateStandard = thePointsTotals.Count(thePointsTotal => thePointsTotal >= theRules.UltimatePointsToCount);
            if (thePointsManager.NumUsersMeetingUltimateStandard >= theRules.MinimumUsersCounting)
                thePointsManager.CurrentPointsToCount = theRules.UltimatePointsToCount;
            else
            { // We're not yet at the ultimate standard, so we need to set the current points to count so at least the minimum number of users are included
                var thePts = thePointsTotals.OrderByDescending(u => u);
                if (thePts.Any())
                {
                    int numUsersToCount = theRules.MinimumUsersCounting; // We need to set at the minimum number of users, unless there are fewer users than that
                    if (numUsersToCount > thePts.Count())
                        numUsersToCount = thePts.Count();
                    thePointsManager.NumUsersMeetingCurrentStandard = numUsersToCount;
                    int totalNumberUsers = thePts.Count();
                    if (totalNumberUsers <= theRules.MinimumUsersCounting)
                        thePointsManager.CurrentPointsToCount = -999999; // everyone counts for now
                    else
                        thePointsManager.CurrentPointsToCount = thePts.Skip(totalNumberUsers - 1).First(); // accept the lowest score
                }
                else
                    thePointsManager.CurrentPointsToCount = -999999; // everyone counts for now
            }
            //RaterooDB.SubmitChanges();
        }

        /// <summary>
        /// Updates a user's point total in a universe. This does not update a user's counting rules status,
        /// and so if that might be affected, then UpdateUserPointsAndStatus should be called instead.
        /// </summary>
        /// <param name="userID">The user</param>
        /// <param name="pointsManagerID">The universe</param>
        /// <param name="theReason">The reason for the update (enumerated above)</param>
        /// <param name="totalPointsAdjustment">Positive or negative change to total points</param>
        /// <param name="currentPointsAdjustment">Positive or negative change to current points</param>
        /// <param name="pendingPointsAdjustment">Positive or negative change to pending points</param>
        /// <param name="pendingMaxLossAdjustment">Positive or negative change to maximum loss</param>
        public void UpdateUserPoints(int userID, int pointsManagerID, PointsChangesReasons theReason, decimal totalPointsAdjustment, decimal currentPointsAdjustment, decimal pendingPointsAdjustment, decimal notYetPendingAdjustment, decimal pendingMaxLossAdjustment, decimal longTermUnweightedAdjustment, decimal? cashPaymentToMake, bool submitChanges)
        {
            PointsManager thePointsManager = DataContext.GetTable<PointsManager>().Single(x=>x.PointsManagerID ==pointsManagerID);
            User theUser = DataContext.GetTable<User>().Single(x => x.UserID == userID);
            UpdateUserPoints(theUser, thePointsManager, theReason, totalPointsAdjustment, currentPointsAdjustment, pendingPointsAdjustment, notYetPendingAdjustment, pendingMaxLossAdjustment, longTermUnweightedAdjustment, cashPaymentToMake, submitChanges);

        }
        
        public void UpdateUserPoints(User theUser, PointsManager thePointsManager, PointsChangesReasons theReason, decimal totalPointsAdjustment, decimal currentPointsAdjustment, decimal pendingPointsAdjustment, decimal notYetPendingAdjustment, decimal pendingMaxLossAdjustment, decimal longTermUnweightedAdjustment, decimal? cashPaymentToMake, bool submitChanges, PointsTotal theTotals = null)
        {
            if (totalPointsAdjustment != 0 || currentPointsAdjustment != 0) // Make a record of this adjustment
            {
                PointsAdjustment pointsAdjustment = AddPointsAdjustment(theUser, thePointsManager, theReason, totalPointsAdjustment, currentPointsAdjustment, cashPaymentToMake);
            }

            if (theTotals == null)
                theTotals = DataContext.NewOrSingleOrDefault<PointsTotal>(pt => pt.User == theUser && pt.PointsManagerID == thePointsManager.PointsManagerID);
            if (theTotals == null)
            {
                theTotals = AddPointsTotal(theUser, thePointsManager);
            }

            decimal originalTotalPoints = theTotals.TotalPoints;
            decimal originalCurrentPoints = theTotals.CurrentPoints;
            decimal originalPendingPoints = theTotals.PendingPoints;
            decimal originalNotYetPendingPoints = theTotals.NotYetPendingPoints;
            decimal originalLongTermUnweighted = theTotals.TotalPointsOrPendingPointsLongTermUnweighted;


            theTotals.TotalPoints += totalPointsAdjustment;
            theTotals.CurrentPoints += currentPointsAdjustment;
            theTotals.PotentialMaxLossOnNotYetPending += pendingMaxLossAdjustment;
            theTotals.PendingPoints += pendingPointsAdjustment;
            theTotals.NotYetPendingPoints += notYetPendingAdjustment;
            theTotals.TotalPointsOrPendingPointsLongTermUnweighted += longTermUnweightedAdjustment;
            //System.Diagnostics.Trace.TraceInformation("Pending points adjustment for user " + userID + ": " + pendingPointsAdjustment + " so pending points is now " + theTotals.PendingPoints);

            if (theReason == PointsChangesReasons.RatingsUpdate && Math.Abs(notYetPendingAdjustment) != 0 && (Math.Abs(totalPointsAdjustment) != 0 || Math.Abs(currentPointsAdjustment) != 0 || Math.Abs(pendingPointsAdjustment) != 0))
                theTotals.NumPendingOrFinalizedRatings++;
            if (theTotals.NumPendingOrFinalizedRatings == 0)
                theTotals.PointsPerRating = 0;
            else
            {
                theTotals.PointsPerRating = (theTotals.TotalPoints + theTotals.PendingPoints) / theTotals.NumPendingOrFinalizedRatings;
                theTotals.PointsPerRatingLongTerm = theTotals.TotalPointsOrPendingPointsLongTermUnweighted / theTotals.NumPendingOrFinalizedRatings;
            }
            if (theTotals.TotalTimeEver == 0)
                theTotals.PointsPerHour = 0;
            else
                theTotals.PointsPerHour = (theTotals.TotalPoints + theTotals.PendingPoints) / theTotals.TotalTimeEver;
            theTotals.ProjectedPointsPerHour = theTotals.PointsPerHour; /* someday, maybe we can come up with something better */

            //Trace.TraceInformation("UpdateUserPoints " + userID + " currentPoints: " + theTotals.CurrentPoints);

            // In calculating the universe points, we should ignore negative points.
            thePointsManager.TotalUserPoints += CalculateDeltaCountingNegativesAsZero(originalTotalPoints, totalPointsAdjustment);
            thePointsManager.CurrentUserPoints += CalculateDeltaCountingNegativesAsZero(originalCurrentPoints, currentPointsAdjustment);
            thePointsManager.CurrentUserPendingPoints += CalculateDeltaCountingNegativesAsZero(originalPendingPoints, pendingPointsAdjustment);
            thePointsManager.CurrentUserNotYetPendingPoints += CalculateDeltaCountingNegativesAsZero(originalNotYetPendingPoints, notYetPendingAdjustment);

            UpdateTrustPoints(ref theTotals, ref theUser, thePointsManager, thePointsManager.PointsTrustRule);

            if (submitChanges)
                DataContext.SubmitChanges();
        }

        public decimal CountNegativeAsZero(decimal theNum)
        {
            if (theNum < 0)
                return 0;
            return theNum;
        }

        public decimal CalculateDeltaCountingNegativesAsZero(decimal originalValue, decimal originalDeltaInValue)
        {
            decimal newValue = originalValue + originalDeltaInValue;
            decimal originalValuePositivesOnly = CountNegativeAsZero(originalValue);
            decimal newValuePositivesOnly = CountNegativeAsZero(newValue);
            decimal newAdjustment = newValuePositivesOnly - originalValuePositivesOnly;
            if (newAdjustment != 0)
                return newAdjustment;
            else
                return 0;
        }

        public void UpdateTrustPoints(ref PointsTotal thePointsTotal, ref User theUser, PointsManager thePointsManager, PointsTrustRule theRules)
        {
            decimal originalTrustPointsRatio = thePointsTotal.TrustPointsRatio;
            thePointsTotal.TrustPoints = (thePointsTotal.TotalPoints - theRules.CountPotentialMaxLossAgainstAt * thePointsTotal.PotentialMaxLossOnNotYetPending + theRules.CountPendingAt * thePointsTotal.PendingPoints + (decimal)0.0001) / Math.Max(theRules.UltimatePointsToCount, (decimal)0.1); // Note that not yet pending points are irrelevant.
            thePointsTotal.TrustPointsRatio = Math.Min(thePointsTotal.TrustPoints, (decimal)3); // We cap trust ratios at 3, so that no one universe can have too much influence on it
            theUser.TrustPointsRatioTotals += thePointsTotal.TrustPointsRatio - originalTrustPointsRatio;
        }

        /// <summary>
        /// Updates the number of points that a user has and then updates the status of
        /// whether the user's points count.
        /// </summary>
        /// <param name="userID">The user</param>
        /// <param name="pointsManagerID">The universe</param>
        /// <param name="theReason">The reason for the update (enumerated above)</param>
        /// <param name="totalPointsAdjustment">Positive or negative change to total points</param>
        /// <param name="currentPointsAdjustment">Positive or negative change to current points</param>
        /// <param name="pendingPointsAdjustment">Positive or negative change to pending points</param>
        /// <param name="pendingMaxLossAdjustment">Positive or negative change to maximum loss</param>
        public void UpdateUserPointsAndStatus(int userID, int pointsManagerID, PointsChangesReasons theReason, decimal totalPointsAdjustment, decimal currentPointsAdjustment, decimal pendingPointsAdjustment, decimal notYetPendingPointsAdjustment, decimal pendingMaxLossAdjustment, decimal longTermUnweightedAdjustment, bool submitChanges)
        {
            User theUser = ObjDataAccess.GetUser(userID);
            PointsManager thePointsManager = ObjDataAccess.GetPointsManager(pointsManagerID);
            PointsTotal thePointsTotal = DataContext.GetTable<PointsTotal>().SingleOrDefault(x => x.User == theUser && x.PointsManager == thePointsManager);
            UpdateUserPointsAndStatus(theUser, thePointsManager, theReason, totalPointsAdjustment, currentPointsAdjustment, pendingPointsAdjustment, notYetPendingPointsAdjustment, pendingMaxLossAdjustment, longTermUnweightedAdjustment, submitChanges, thePointsTotal);
        }

        public void UpdateUserPointsAndStatus(User theUser, PointsManager thePointsManager, PointsChangesReasons theReason, decimal totalPointsAdjustment, decimal currentPointsAdjustment, decimal pendingPointsAdjustment, decimal notYetPendingPointsAdjustment, decimal notYetPendingMaxLossAdjustment, decimal longTermUnweightedAdjustment, bool submitChanges, PointsTotal thePointsTotal)
        {
            if (totalPointsAdjustment == 0 && currentPointsAdjustment == 0 && pendingPointsAdjustment == 0 && notYetPendingPointsAdjustment == 0 && notYetPendingMaxLossAdjustment == 0)
                return;

            bool userPreviouslyMeetsUltimateStandard = UserMeetsUltimateCountingStandard(thePointsManager, theUser, thePointsTotal);
            bool userPreviouslyMeetsCurrentStandard = userPreviouslyMeetsUltimateStandard;
            if (!userPreviouslyMeetsCurrentStandard)
                userPreviouslyMeetsCurrentStandard = UserIsTrustedOldSystem(thePointsManager, theUser, thePointsTotal); // maybe the user meets the current standard

            UpdateUserPoints(theUser, thePointsManager, theReason, totalPointsAdjustment, currentPointsAdjustment, pendingPointsAdjustment, notYetPendingPointsAdjustment, notYetPendingMaxLossAdjustment, longTermUnweightedAdjustment, null, submitChanges, thePointsTotal);

            PMPaymentGuarantees.CalculateGuaranteedPaymentsEarnedThisRewardPeriod(thePointsTotal);

            bool userNowMeetsUltimateStandard = UserMeetsUltimateCountingStandard(thePointsManager, theUser, thePointsTotal);
            bool userNowMeetsCurrentStandard = userNowMeetsUltimateStandard;
            if (!userNowMeetsCurrentStandard)
                userNowMeetsCurrentStandard = UserIsTrustedOldSystem(thePointsManager, theUser, thePointsTotal); // maybe the user meets the current standard
            if (userPreviouslyMeetsCurrentStandard != userNowMeetsCurrentStandard || userNowMeetsUltimateStandard != userPreviouslyMeetsUltimateStandard)
            {
                PointsTrustRule theRules = thePointsManager.PointsTrustRule;
                bool newUserMeetsUltimateStandard = (!userPreviouslyMeetsUltimateStandard && userNowMeetsUltimateStandard);
                bool userFallsOutOfUltimateStandard = (userPreviouslyMeetsUltimateStandard && !userNowMeetsUltimateStandard);
                bool newUserMeetsCurrentStandard = (!userPreviouslyMeetsCurrentStandard && userNowMeetsCurrentStandard);
                bool userFallsOutOfCurrentStandard = (userPreviouslyMeetsCurrentStandard && !userNowMeetsCurrentStandard);
                UpdatePointsTrustRules(theRules, thePointsManager, newUserMeetsUltimateStandard, userFallsOutOfUltimateStandard, newUserMeetsCurrentStandard, userFallsOutOfCurrentStandard);
                if (submitChanges)
                    DataContext.SubmitChanges();
            }

        }

        /// <summary>
        /// Checks to see if there is a universe to cash out.
        /// </summary>
        public bool IdleTaskCheckPointsManagers()
        {
            DateTime now = TestableDateTime.Now;
            PointsManager thePointsManager = DataContext.GetTable<PointsManager>().FirstOrDefault(u => u.EndOfDollarSubsidyPeriod != null && u.EndOfDollarSubsidyPeriod < now);
            if (thePointsManager == null)
                return false; // no work done
            else
            {
                PointsManagerCashOut(thePointsManager);
                DataContext.SubmitChanges();
                return true; // did some work
            }
        }

        // The following are used in the next routine. We cannot use anonymous types, because those are immutable, and we want to make changes.
        class UserWithPointsType { public int User = 0; public decimal TotalPoints = 0; public decimal CurrentPoints = 0; public PointsTotal PT = null; public bool Include = true; };
        class UserWithPointsType2 { public int User = 0; public decimal PointsToCash = 0; public decimal CurrentPoints = 0; public decimal CashValue = 0; public PointsTotal PT = null; };
        class UserWithPointsType3 { public int User = 0; public decimal PointsToCash = 0; public decimal PerPrizeProbability = 0; public decimal ProbRangeBottom = 0; public decimal ProbRangeTop = 0; public decimal Prize = 0; public PointsTotal PT = null; };

        /// <summary>
        /// Convert current points to cash payments at the end of a dollar subsidy period.
        /// </summary>
        /// <param name="pointsManagerID">The universe to convert points for</param>

        public void PointsManagerCashOut(PointsManager thePointsManager)
        {
            DateTime currentTime = TestableDateTime.Now;
            if (thePointsManager.EndOfDollarSubsidyPeriod != null && thePointsManager.EndOfDollarSubsidyPeriod < currentTime)
            {
                decimal minimumPayment = 0;
                if (thePointsManager.MinimumPayment != null)
                    minimumPayment = (decimal)thePointsManager.MinimumPayment;
                decimal currentPointsToCash = 0;
                // get a list of users ordered by current points (with total points as a tiebreaker)

                List<UserWithPointsType>
                 usersWithPoints =
                                    DataContext.GetTable<PointsTotal>().Where(pt => pt.PointsManagerID == thePointsManager.PointsManagerID && pt.User.SuperUser == false && (pt.CurrentPoints > 0 || pt.GuaranteedPaymentsEarnedThisRewardPeriod > 0)).
                                                Select(pt => new UserWithPointsType { User = pt.UserID, TotalPoints = pt.TotalPoints, CurrentPoints = pt.CurrentPoints, PT = pt, Include = false }).
                                                OrderByDescending(urp => urp.CurrentPoints).ThenByDescending(urp => urp.TotalPoints).
                                                ToList();

                bool usePendingPointsOnly = false;
                if (!usersWithPoints.Any())
                {
                    usersWithPoints =
                                       DataContext.GetTable<PointsTotal>().Where(pt => pt.PointsManagerID == thePointsManager.PointsManagerID && pt.User.SuperUser == false && (pt.PendingPoints > 0 || pt.NotYetPendingPoints > 0 || pt.GuaranteedPaymentsEarnedThisRewardPeriod > 0)).
                                                   Select(pt => new UserWithPointsType { User = pt.UserID, TotalPoints = pt.TotalPoints, CurrentPoints = pt.PendingPoints + pt.NotYetPendingPoints, PT = pt, Include = false }).
                                                   OrderByDescending(urp => urp.CurrentPoints).ThenByDescending(urp => urp.TotalPoints).
                                                   ToList();
                    usePendingPointsOnly = true;
                }

                // go down the list to get as many users as we can, such that each would receive at least
                // the minimum payment. then exclude all the others
                bool includeBasedOnPerformance = true;
                foreach (var userWithPoints in usersWithPoints)
                {
                    if (includeBasedOnPerformance)
                    {
                        if (thePointsManager.NumPrizes > 0)
                        { // We'll include everyone with minimum points == MinPayment
                            if (userWithPoints.CurrentPoints >= thePointsManager.MinimumPayment)
                                currentPointsToCash += userWithPoints.CurrentPoints;
                            else
                                includeBasedOnPerformance = false;
                        }
                        else
                        { // We'll include only people who would make at least MinPayment in cash.
                            if (userWithPoints.CurrentPoints * thePointsManager.CurrentPeriodDollarSubsidy / (currentPointsToCash + userWithPoints.CurrentPoints) > minimumPayment)
                                currentPointsToCash += userWithPoints.CurrentPoints;
                            else
                                includeBasedOnPerformance = false;
                        }
                    }
                    userWithPoints.Include = includeBasedOnPerformance || userWithPoints.PT.GuaranteedPaymentsEarnedThisRewardPeriod > 0;
                }
                if (thePointsManager.NumPrizes == 0)
                {
                    // update the user's points (creating a PointsAdjustment object that can be used to 
                    // generate a report of whom to send the cash to)
                    List<UserWithPointsType2> usersToInclude = (List<UserWithPointsType2>)
                        usersWithPoints
                        .Where(uwp => uwp.Include == true)
                        .Select(uwp => new UserWithPointsType2 { User = uwp.User, PointsToCash = uwp.CurrentPoints, CashValue = (uwp.CurrentPoints / currentPointsToCash) * thePointsManager.CurrentPeriodDollarSubsidy, PT = uwp.PT })
                        .ToList();
                    foreach (UserWithPointsType2 user in usersToInclude)
                    {
                        if (user.CashValue < user.PT.GuaranteedPaymentsEarnedThisRewardPeriod)
                            user.CashValue = user.PT.GuaranteedPaymentsEarnedThisRewardPeriod;
                        decimal pointsToCash = usePendingPointsOnly ? 0 : 0 - user.PointsToCash;
                        UpdateUserPoints(user.User, thePointsManager.PointsManagerID, PointsChangesReasons.PointsCashed, 0, pointsToCash, 0, 0, 0, 0, user.CashValue, true);
                        PMPaymentGuarantees.EndOfRewardPeriodTasks(user.PT, user.CashValue);
                    }
                }
                else
                { // We'll award prizes by picking random numbers.
                    decimal probRange = 0;
                    List<UserWithPointsType3> usersToInclude = (List<UserWithPointsType3>)
                        usersWithPoints.Where(uwp => uwp.Include == true).Select(uwp => new UserWithPointsType3 { User = uwp.User, PointsToCash = uwp.CurrentPoints, PerPrizeProbability = (uwp.CurrentPoints / currentPointsToCash), ProbRangeBottom = 0M, ProbRangeTop = 0M, Prize = 0M, PT = uwp.PT }).ToList();
                    foreach (UserWithPointsType3 user in usersToInclude)
                    {
                        user.ProbRangeBottom = probRange;
                        probRange += user.PerPrizeProbability;
                        user.ProbRangeTop = probRange;
                    }
                    int prizesAwarded = 0;
                    while (prizesAwarded < usersToInclude.Count() && prizesAwarded < thePointsManager.NumPrizes)
                    {
                        decimal aRandom = (decimal)RandomGenerator.GetRandom();
                        var winningUser = usersToInclude.SingleOrDefault(uto => uto.ProbRangeBottom <= aRandom && uto.ProbRangeTop > aRandom);
                        if (winningUser != null)
                        {
                            if (winningUser.Prize == 0 || prizesAwarded > usersToInclude.Count())
                            {
                                winningUser.Prize += (decimal)thePointsManager.CurrentPeriodDollarSubsidy / (decimal)thePointsManager.NumPrizes;
                                prizesAwarded++;
                            }
                        }
                    }
                    foreach (var user in usersToInclude)
                    {
                        if (user.Prize < user.PT.GuaranteedPaymentsEarnedThisRewardPeriod)
                            user.Prize = user.PT.GuaranteedPaymentsEarnedThisRewardPeriod;
                        decimal pointsToCash = usePendingPointsOnly ? 0 : 0 - user.PointsToCash;
                        UpdateUserPoints(user.User, thePointsManager.PointsManagerID, PointsChangesReasons.PointsCashed, 0, pointsToCash, 0, 0, 0, 0, user.Prize, true);
                        PMPaymentGuarantees.EndOfRewardPeriodTasks(user.PT, user.Prize );
                    }
                }
                // Update the cash subsidy information
                if (thePointsManager.NextPeriodDollarSubsidy == 0 || thePointsManager.NextPeriodDollarSubsidy == null)
                {
                    thePointsManager.CurrentPeriodDollarSubsidy = 0;
                    thePointsManager.EndOfDollarSubsidyPeriod = null;
                }
                else
                {
                    thePointsManager.CurrentPeriodDollarSubsidy = thePointsManager.NextPeriodDollarSubsidy ?? 0;
                    int nextPeriodLength = (60 * 60 * 24 * 30); // assume one month
                    if (thePointsManager.NextPeriodLength != nextPeriodLength && thePointsManager.NextPeriodLength != null)
                        nextPeriodLength = (int)thePointsManager.NextPeriodLength;
                    thePointsManager.EndOfDollarSubsidyPeriod += new TimeSpan(0, 0, nextPeriodLength);
                }
                UpdateAutomaticInsertableContentForPointsManager(thePointsManager.PointsManagerID, thePointsManager.CurrentPeriodDollarSubsidy, thePointsManager.NumPrizes, thePointsManager.EndOfDollarSubsidyPeriod);
            }
        }

        public bool UserIsTrustedOldSystem(PointsManager thePointsManager, User theUser, PointsTotal thePointsTotal = null)
        {
            return ObjDataAccess.UserIsTrustedToMakeDatabaseChanges(thePointsManager, theUser, thePointsTotal);
        }

        /// <summary>
        /// Returns true if a user's predictions meet the ultimate standard for the universe (in which case, the user's 
        /// predictions will definitely count).
        /// </summary>
        /// <param name="pointsManagerID">The universe</param>
        /// <param name="userID">The user</param>
        /// <returns>True if and only if the user can change the predictions in the univese</returns>
        public bool UserMeetsUltimateCountingStandard(int pointsManagerID, int userID)
        {
            PointsManager thePointsManager = ObjDataAccess.GetPointsManager(pointsManagerID);
            User theUser = ObjDataAccess.GetUser(userID);
            return UserMeetsUltimateCountingStandard(thePointsManager, theUser);
        }
        public bool UserMeetsUltimateCountingStandard(PointsManager pointsManager, User user, PointsTotal thePointsTotal = null)
        {
            PointsTrustRule theRules = pointsManager.PointsTrustRule;
            if (thePointsTotal == null)
                thePointsTotal = DataContext.NewOrSingleOrDefault<PointsTotal>(pt => pt.PointsManager == pointsManager && pt.User == user);
            if (thePointsTotal == null)
            {
                thePointsTotal = AddPointsTotal(user, pointsManager);
            }

            bool userCounts = (thePointsTotal.TotalPoints - theRules.CountPotentialMaxLossAgainstAt * thePointsTotal.PotentialMaxLossOnNotYetPending + theRules.CountPendingAt * thePointsTotal.PendingPoints >= theRules.UltimatePointsToCount);
            return userCounts;
        }

        static int lastPointsManagerPointsUpdated = 0;
        static DateTime? stopUpdatingPointsToCountUntil = null;
        public bool UpdatePointsTrustRulesBackgroundTask()
        {
            if (stopUpdatingPointsToCountUntil != null && TestableDateTime.Now < (DateTime) stopUpdatingPointsToCountUntil)
                return false; // no more work to do.
            PointsManager thePointsManager = DataContext.GetTable<PointsManager>().OrderBy(x => x.PointsManagerID).FirstOrDefault(x => x.NumUsersMeetingUltimateStandard < x.PointsTrustRule.MinimumUsersCounting && x.PointsManagerID > lastPointsManagerPointsUpdated);
            if (thePointsManager == null)
            {
                lastPointsManagerPointsUpdated = 0;
                stopUpdatingPointsToCountUntil = TestableDateTime.Now + new TimeSpan(3, 0, 0); // check again in three hours
                return false;
            }
            UpdatePointsTrustRules(thePointsManager.PointsTrustRule, thePointsManager);
            lastPointsManagerPointsUpdated = thePointsManager.PointsManagerID;
            return true;
        }

    }

        
}