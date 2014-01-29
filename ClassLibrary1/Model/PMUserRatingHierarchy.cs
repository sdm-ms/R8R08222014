using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Transactions;

using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public class UserRatingDataException : Exception
    {
        public UserRatingDataException(string message)
            : base(message)
        {
        }
    }

    public class RatingHierarchyEntry
    {
        public string RatingName { get; set; }
        public int? RatingID { get; set; }
        public decimal? Value { get; set; }
        public int HierarchyLevel { get; set; }
        public int DecimalPlaces { get; set; }
        public decimal? MinVal { get; set; }
        public decimal? MaxVal { get; set; }
        public bool IsDate { get; set; }
        public int EntryNum { get; set; }
        public int? Superior { get; set; }
        public string Description { get; set; }
    }

    public class RatingHierarchyData
    {
        public List<RatingHierarchyEntry> RatingHierarchyEntries;

        public RatingHierarchyData()
        {
            RatingHierarchyEntries = new List<RatingHierarchyEntry>();
        }

        public void SetDefaultValuesForPlannedRatingGroupHelper(IEnumerable<RatingHierarchyEntry> theDefaultsThisLevel, decimal theSum)
        {
            int count = theDefaultsThisLevel.Count();
            decimal eachDefaultUserRating = Math.Round(theSum / count, 4);
            decimal lastDefaultUserRating = theSum - (eachDefaultUserRating) * (count - 1);

            foreach (var theDefaultUserRating in theDefaultsThisLevel)
                theDefaultUserRating.Value = eachDefaultUserRating;
            theDefaultsThisLevel.Last().Value = lastDefaultUserRating;
        }

        public void SetDefaultValuesForPlannedRatingGroup(decimal? constrainedSum, decimal? firstLevelValue)
        {
            var levelOneUserRatings = RatingHierarchyEntries.Where(d => d.HierarchyLevel == 1);
            SetDefaultValuesForPlannedRatingGroupHelper(levelOneUserRatings, (constrainedSum != null) ? (decimal)constrainedSum : (decimal)firstLevelValue * levelOneUserRatings.Count());
            int finishingHierarchyLevel = RatingHierarchyEntries.Max(d => d.HierarchyLevel);
            for (int hierarchyLevel = 1; hierarchyLevel <= finishingHierarchyLevel; hierarchyLevel++)
            {
                var theUserRatingsLevelAbove = RatingHierarchyEntries.Where(d => d.HierarchyLevel == hierarchyLevel - 1);
                foreach (var theUserRatingLevelAbove in theUserRatingsLevelAbove)
                {
                    var theUserRatingsThisLevel = RatingHierarchyEntries.Where(d => d.Superior == theUserRatingLevelAbove.EntryNum);
                    if (theUserRatingsThisLevel.Any())
                        SetDefaultValuesForPlannedRatingGroupHelper(theUserRatingsThisLevel, (decimal)theUserRatingLevelAbove.Value);
                }
            }
        }

        public void CalculateDefaultValuesForPlannedHierarchy(decimal? constrainedSum, decimal? minValEachEntry, decimal? maxValEachEntry)
        {
            if (constrainedSum != null && minValEachEntry != null && minValEachEntry != 0)
                throw new UserRatingDataException("If there is a constrained sum, the minimum value for each must be 0.");
            var nonNullEntries = RatingHierarchyEntries.Where(x => x.Value != null);
            if (minValEachEntry != null)
            {
                if (nonNullEntries.Any(x => (decimal)x.Value < (decimal)minValEachEntry))
                    throw new UserRatingDataException("No entry can be less than the minimum value specified.");
            }
            if (maxValEachEntry != null)
            {
                if (nonNullEntries.Any(x => (decimal)x.Value > (decimal)maxValEachEntry))
                    throw new UserRatingDataException("No entry can be less than the minimum value specified.");
            }

            bool userHasOverridenDefaultDefaults = nonNullEntries.Any();
            RatingHierarchyData cloneData = null;
            if (userHasOverridenDefaultDefaults)
            { // remember the values that have been set before we load the defaults
                cloneData = new RatingHierarchyData();
                foreach (var entry in RatingHierarchyEntries)
                    cloneData.Add("", entry.Value, entry.HierarchyLevel, "");
            }

            // Now load the default defaults.
            if (constrainedSum != null)
                SetDefaultValuesForPlannedRatingGroup(constrainedSum, null);
            else
            {
                decimal midPoint = 0;
                if (minValEachEntry != null && maxValEachEntry != null)
                    midPoint = ((decimal)minValEachEntry + (decimal)maxValEachEntry) / 2;
                SetDefaultValuesForPlannedRatingGroup(null, midPoint);
            }

            // Now adjust based on the manually entered defaults.
            if (userHasOverridenDefaultDefaults)
                AdjustDefaultValuesForPlannedHierarchy(constrainedSum, minValEachEntry, maxValEachEntry, cloneData);

        }

        // This tests a hierarchy to be used to define a RatingGroupAttributes,
        // to ensure that its entries are valid
        public void AdjustDefaultValuesForPlannedHierarchy(decimal? constrainedSum, decimal? minValEachEntry, decimal? maxValEachEntry, RatingHierarchyData manuallySetValues)
        {

            // We'll do this by using the routines we have created for the UserRatingHierarchy.
            // We'll use artificial numbers for ratingGroupID.
            UserRatingHierarchyData myUserRatingData = new UserRatingHierarchyData();
            int lastHierarchy = -1;
            int numRatingGroupsCreated = 0;
            int ratingGroupID = 0;
            int entryNum = -1;
            foreach (var entry in RatingHierarchyEntries)
            {
                entryNum++;
                if (entry.HierarchyLevel > lastHierarchy)
                {
                    numRatingGroupsCreated++; // This will set only first in each rating group correctly.
                    ratingGroupID = numRatingGroupsCreated;
                }
                else // Set rating group to that of the most recent entry on this hierarchy level.
                    ratingGroupID = (int)myUserRatingData.UserRatingHierarchyEntries.Last(x => x.HierarchyLevel == entry.HierarchyLevel).RatingGroupId;
                lastHierarchy = entry.HierarchyLevel;
                decimal? manualValue = manuallySetValues.RatingHierarchyEntries[entryNum].Value;
                myUserRatingData.Add(entry.RatingID, ratingGroupID, entry.Value, entry.Value, manualValue ?? 0, manualValue, manualValue != null, manualValue != null, entry.HierarchyLevel);
            }

            myUserRatingData.AddDerivativeUserRatingsToUserRatingHierarchy(constrainedSum, (decimal)minValEachEntry, (decimal)maxValEachEntry);

            for (entryNum = 0; entryNum < RatingHierarchyEntries.Count; entryNum++)
            {
                RatingHierarchyEntries[entryNum].Value = myUserRatingData.UserRatingHierarchyEntries[entryNum].EnteredValueOrCalculatedValue;
            }
        }

        // Add without a specified ratingID or decimal places (useful before the creation of the rating)
        public void Add(String name, decimal? theValue, int hierarchyLevel, string description)
        {
            Add(name, null, theValue, hierarchyLevel, 0, null, null, false, description);
        }

        public void Add(string ratingName, int? ratingID, decimal? theValue, int hierarchyLevel, int decimalPlaces, decimal? minVal, decimal? maxVal, bool isDate, string description)
        {
            if (hierarchyLevel <= 0)
                throw new UserRatingDataException("Invalid hierarchy.");
            int numEntries = RatingHierarchyEntries.Count;
            if (numEntries == 0)
            {
                if (hierarchyLevel != 1)
                    throw new UserRatingDataException("Invalid hierarchy.");
            }
            else
            {
                if (hierarchyLevel > RatingHierarchyEntries[numEntries - 1].HierarchyLevel + 1)
                    throw new UserRatingDataException("Invalid hierarchy.");
            }

            int? superior = null;
            if (hierarchyLevel > 1)
                superior = RatingHierarchyEntries.Last(d => d.HierarchyLevel == hierarchyLevel - 1).EntryNum;

            
        //public string RatingName { get; set; }
        //public int? RatingID { get; set; }
        //public decimal? Value { get; set; }
        //public int HierarchyLevel { get; set; }
        //public int DecimalPlaces { get; set; }
        //public decimal? MinVal { get; set; }
        //public decimal? MaxVal { get; set; }
        //public bool IsDate { get; set; }
        //public int EntryNum { get; set; }
        //public int? Superior { get; set; }
        //public string Description { get; set; }

            RatingHierarchyEntry ratingHierarchyEntry = new RatingHierarchyEntry  {
                RatingName  = ratingName,
                RatingID = ratingID, 
                Value = theValue,
                HierarchyLevel = hierarchyLevel, 
                DecimalPlaces = decimalPlaces, 
                MinVal = minVal,
                MaxVal =  maxVal, 
                IsDate = isDate,
                EntryNum = numEntries + 1, 
                Superior = superior, 
                Description = description
            };
            RatingHierarchyEntries.Add(ratingHierarchyEntry);

        }
    }

    /// <summary>
    /// Another view of the hierarchy, this one excluding the names of the ratings, but including
    /// the old and new values, pre- and post-prediction, as well as an indication of which rating
    /// or ratings have new values as a result of being directly made by the user, and which cannot
    /// be changed (e.g., because the ratings have ended).
    /// </summary>
    [Serializable()]
    public class UserRatingHierarchyEntry
    {
        public int? RatingId;
        public int? RatingGroupId;
        public decimal? OriginalTrustedValue;
        public decimal? OriginalDisplayedRating;
        public decimal OriginalTrustedValueOrBasisForCalculatingPoints;
        public decimal? EnteredValueOrCalculatedValue;
        public decimal NewValueAfterAdjustment;
        public bool MadeDirectly;
        public bool CannotBeChanged;
        public decimal? MinimumValue;
        public decimal? MaximumValue;
        public decimal? PreferredValue;
        public int HierarchyLevel;
        public int EntryNum;
        public int? Superior;

        public decimal ValueStatus()
        {
            if (OriginalTrustedValue == null && EnteredValueOrCalculatedValue == null)
                throw new Exception("Cannot get value status when original prediction is null.");
            return EnteredValueOrCalculatedValue ?? (decimal)OriginalTrustedValue;
        }

        public UserRatingHierarchyEntry(int? theRatingId, int? ratingGroupId, decimal? theOriginalTrustedValue, decimal? theOriginalDisplayedRating, decimal theOriginalDisplayedRatingOrBasisOfCalc, decimal? theNewValue, bool theDirectlyMade, bool theCannotBeChanged, int theHierarchyLevel, int theEntryNum, int? theSuperior)
        {
            RatingId = theRatingId;
            RatingGroupId = ratingGroupId;
            OriginalTrustedValue = theOriginalTrustedValue;
            OriginalDisplayedRating = theOriginalDisplayedRating;
            OriginalTrustedValueOrBasisForCalculatingPoints = theOriginalDisplayedRatingOrBasisOfCalc;
            EnteredValueOrCalculatedValue = theNewValue;
            MadeDirectly = theDirectlyMade;
            CannotBeChanged = theCannotBeChanged;
            HierarchyLevel = theHierarchyLevel;
            EntryNum = theEntryNum;
            Superior = theSuperior;
            NewValueAfterAdjustment = default(decimal);
            MinimumValue = default(decimal?);
            MaximumValue = default(decimal?);
            PreferredValue = default(decimal?);
        }

        public void DebugOutput()
        {
            for (int i = 1; i < HierarchyLevel; i++)
                Debug.Write("     ");
            Trace.TraceInformation(HierarchyLevel + " " + RatingId + " originalValue: " + OriginalTrustedValue + " newValue: " + EnteredValueOrCalculatedValue + " min: " + MinimumValue + " max: " + MaximumValue + " pref: " + PreferredValue);
        }
    }


    [Serializable()]
    public class UserRatingHierarchyAdditionalInfo
    {
        public float AdjustPct;
        public float OverallTrustLevel;
        public bool IsTrusted;
        public float PercentPreviousRatings;
        public decimal OneHourVolatility;
        public decimal OneDayVolatility;
        public List<TrustTrackerChoiceSummary> TrustTrackerChoiceSummaries;
        public List<int> ChoiceInGroupIDsNotTrackedYet;

        public UserRatingHierarchyAdditionalInfo(
             float adjustPct,
             float overallTrustLevel,
             bool isTrusted,
             float percentPreviousRatings,
             decimal oneHourVolatility,
             decimal oneDayVolatility,
             List<TrustTrackerChoiceSummary> theTrustTrackerChoiceSummaries,
             List<int> choiceInFieldIDsNotTrackedYet
        )
        {
            AdjustPct = adjustPct;
            OverallTrustLevel = overallTrustLevel;
            IsTrusted = isTrusted;
            PercentPreviousRatings = percentPreviousRatings;
            OneHourVolatility = oneHourVolatility;
            OneDayVolatility = oneDayVolatility;
            TrustTrackerChoiceSummaries = theTrustTrackerChoiceSummaries;
            ChoiceInGroupIDsNotTrackedYet = choiceInFieldIDsNotTrackedYet;
        }
    }

    [Serializable()]
    public class UserRatingHierarchyData
    {
        public List<UserRatingHierarchyEntry> UserRatingHierarchyEntries;
        public UserRatingHierarchyAdditionalInfo AdditionalInfo;
        public UserRatingHierarchyData(UserRatingHierarchyAdditionalInfo additionalInfo)
        {
            AdditionalInfo = additionalInfo;
            UserRatingHierarchyEntries = new List<UserRatingHierarchyEntry>();
        }

        public UserRatingHierarchyData()
        {
            UserRatingHierarchyEntries = new List<UserRatingHierarchyEntry>();
        }

        internal class RatingAndGroup
        {
            public Rating Rating { get; set; }
            public RatingGroup RatingGroup { get; set; }
        }

        public void GetRatingsAndGroups(IRaterooDataContext theDataContext, UserRatingsToAdd theUserRatings, out List<Rating> theRatings, out List<RatingGroup> theRatingGroups, out RatingGroup topRatingGroup)
        {

            List<RatingAndGroup> loadedData;
            if (UserRatingHierarchyEntries.Count == 1)
                loadedData = new List<RatingAndGroup> { new RatingAndGroup { Rating = theUserRatings.RatingGroup.Ratings.First(), RatingGroup = theUserRatings.RatingGroup } };
            else
                loadedData = theDataContext.GetTable<Rating>().Where(x => UserRatingHierarchyEntries.Select(y => y.RatingId).Contains(x.RatingID)).Select(x => new RatingAndGroup { Rating = x, RatingGroup = x.RatingGroup }).ToList();
            theRatings = UserRatingHierarchyEntries.Select(x => loadedData.First(y => y.Rating.RatingID == x.RatingId).Rating).ToList();
            theRatingGroups = UserRatingHierarchyEntries.Select(x => loadedData.First(y => y.RatingGroup.RatingGroupID == x.RatingGroupId).RatingGroup).Distinct().ToList();
            topRatingGroup = theRatingGroups.Single(x => x.RatingGroupID == UserRatingHierarchyEntries.First(y => y.HierarchyLevel == 1).RatingGroupId);
        }

        public void DebugOutput()
        {
            foreach (var x in UserRatingHierarchyEntries)
                x.DebugOutput();
        }

        public bool CheckIntegrity(decimal? constrainedSum)
        {
            foreach (var entry in UserRatingHierarchyEntries)
            {
                var belowEntries = UserRatingHierarchyEntries.Where(d => d.Superior == entry.EntryNum);
                if (belowEntries.Count() > 0)
                {
                    decimal sumBelow = belowEntries.Sum(d => d.ValueStatus());
                    if (Math.Abs(entry.ValueStatus() - sumBelow) > (decimal)0.01)
                        return false;
                }
            }
            if (constrainedSum != null)
            {
                var levelOneEntries = UserRatingHierarchyEntries.Where(d => d.HierarchyLevel == 1);
                decimal sumLevelOne = levelOneEntries.Sum(d => d.ValueStatus());
                if (Math.Abs((decimal)constrainedSum - sumLevelOne) > (decimal)0.01)
                    return false;
            }
            return true;
        }

        public bool EntryIsWithinRatingGroup(int entryNum, int theRatingGroupID)
        {
            if (UserRatingHierarchyEntries[entryNum - 1].RatingGroupId == theRatingGroupID)
                return true;
            if (UserRatingHierarchyEntries[entryNum - 1].Superior == null)
                return false;
            if (UserRatingHierarchyEntries[entryNum - 1].Superior == entryNum)
                throw new UserRatingDataException("Invalid prediction hierarchy. Item cannot be its own superior.");
            return EntryIsWithinRatingGroup((int)UserRatingHierarchyEntries[entryNum - 1].Superior, theRatingGroupID);
        }

        public IEnumerable<UserRatingHierarchyEntry> GetTargetUserRatings(int hierarchyLevel, int ratingGroupID)
        {
            return UserRatingHierarchyEntries.Where(d => d.HierarchyLevel == hierarchyLevel && EntryIsWithinRatingGroup(d.EntryNum, (int)ratingGroupID));
        }

        public void Add(int? ratingID, int? ratingGroupID, decimal? theOriginalTrustedValue, decimal? theOriginalDisplayedRating, decimal theOriginalDisplayedRatingOrBasisOfCalc, decimal? theNewValue, bool theDirectlyMade, bool theCannotBeChanged, int hierarchyLevel)
        {
            if (hierarchyLevel <= 0)
                throw new UserRatingDataException("Invalid hierarchy.");
            int numEntries = UserRatingHierarchyEntries.Count;
            if (numEntries == 0)
            {
                if (hierarchyLevel != 1)
                    throw new UserRatingDataException("Invalid hierarchy.");
            }
            else
            {
                if (hierarchyLevel > UserRatingHierarchyEntries[numEntries - 1].HierarchyLevel + 1)
                    throw new UserRatingDataException("Invalid hierarchy.");
            }

            int? superior = null;
            if (hierarchyLevel > 1)
                superior = UserRatingHierarchyEntries.Last(d => d.HierarchyLevel == hierarchyLevel - 1).EntryNum;

            UserRatingHierarchyEntries.Add(new UserRatingHierarchyEntry(ratingID, ratingGroupID, theOriginalTrustedValue, theOriginalDisplayedRating, theOriginalDisplayedRatingOrBasisOfCalc, theNewValue, theDirectlyMade, theCannotBeChanged, hierarchyLevel, numEntries + 1, superior));
        }

        // Move up the prediction hierarchy, calculating the value of predictions that can be set definitively on the 
        // basis of their descendants, and for others, calculating minimum, maximum, and preferred values.
        // Find inconsistent prediction hierarchies.
        public void AdjustUserRatingsUpUserRatingHierarchy(decimal? constrainedSum, decimal minValEachEntry, decimal maxValEachEntry)
        {
            int startingHierarchyLevel = UserRatingHierarchyEntries.Max(d => d.HierarchyLevel);
            int finishingHierarchyLevel = 1;
            for (int hierarchyLevel = startingHierarchyLevel; hierarchyLevel >= finishingHierarchyLevel; hierarchyLevel--)
            {
                var theUserRatingsThisLevel = UserRatingHierarchyEntries.Where(d => d.HierarchyLevel == hierarchyLevel);
                foreach (var theUserRatingThisLevel in theUserRatingsThisLevel)
                {
                    var belowHierarchyEntries = UserRatingHierarchyEntries.Where(d => d.Superior == theUserRatingThisLevel.EntryNum);
                    if (hierarchyLevel == startingHierarchyLevel || (!belowHierarchyEntries.Any()))
                    { // we're on the lowest level
                        theUserRatingThisLevel.PreferredValue = theUserRatingThisLevel.ValueStatus(); // latest prediction
                        if (theUserRatingThisLevel.MadeDirectly || theUserRatingThisLevel.CannotBeChanged)
                            theUserRatingThisLevel.MinimumValue = theUserRatingThisLevel.MaximumValue = theUserRatingThisLevel.EnteredValueOrCalculatedValue = theUserRatingThisLevel.PreferredValue;
                        else
                        {
                            theUserRatingThisLevel.MinimumValue = minValEachEntry;
                            theUserRatingThisLevel.MaximumValue = maxValEachEntry;
                        }
                    }
                    else
                    { // there is a lower level
                        var belowHierarchyEntriesFixed = belowHierarchyEntries.Where(d => d.MadeDirectly || d.CannotBeChanged);
                        var belowHierarchyEntriesOther = belowHierarchyEntries.Where(d => !d.MadeDirectly && !d.CannotBeChanged);
                        if ((constrainedSum == null && !theUserRatingThisLevel.CannotBeChanged) || !belowHierarchyEntriesOther.Any())
                        { // all descendants are fixed (or, if no constrained sum, will be treated as such)
                            decimal belowFixedSum = belowHierarchyEntriesFixed.Sum(d => d.ValueStatus());
                            decimal belowOtherSum = belowHierarchyEntriesOther.Sum(d => d.ValueStatus());
                            decimal sumToSetTo = belowFixedSum + belowOtherSum;
                            if ((theUserRatingThisLevel.MadeDirectly || theUserRatingThisLevel.CannotBeChanged)
                                && (theUserRatingThisLevel.ValueStatus() != sumToSetTo))
                                throw new UserRatingDataException("Invalid or incomplete ratings entered. Please try again.");
                            else if (sumToSetTo > maxValEachEntry || sumToSetTo < minValEachEntry)
                                throw new UserRatingDataException("Invalid or incomplete ratings entered. Please try again.");
                            else
                            { // set this to sum of descendants.
                                theUserRatingThisLevel.CannotBeChanged = true;
                                theUserRatingThisLevel.MinimumValue = theUserRatingThisLevel.MaximumValue = theUserRatingThisLevel.EnteredValueOrCalculatedValue = sumToSetTo;
                            }
                        }
                        else
                        { // at least one descendant is not fixed
                            decimal sumFixedDescendants = belowHierarchyEntriesFixed.Sum(d => d.ValueStatus());
                            decimal sumOtherDescendants = belowHierarchyEntriesOther.Sum(d => d.ValueStatus());
                            decimal minimumOtherDescendants = belowHierarchyEntriesOther.Sum(d => (decimal)d.MinimumValue);
                            decimal maximumOtherDescendants = belowHierarchyEntriesOther.Sum(d => (decimal)d.MaximumValue);
                            if (theUserRatingThisLevel.MadeDirectly || theUserRatingThisLevel.CannotBeChanged)
                            {
                                if (sumFixedDescendants + minimumOtherDescendants > theUserRatingThisLevel.ValueStatus()
                                    || sumFixedDescendants + maximumOtherDescendants < theUserRatingThisLevel.ValueStatus())
                                    throw new UserRatingDataException("Invalid predictions entered. Please try again.");
                                theUserRatingThisLevel.PreferredValue = theUserRatingThisLevel.MinimumValue = theUserRatingThisLevel.MaximumValue = theUserRatingThisLevel.EnteredValueOrCalculatedValue = theUserRatingThisLevel.ValueStatus();
                            }
                            else
                            {
                                theUserRatingThisLevel.MinimumValue = Math.Max(minValEachEntry, minimumOtherDescendants + sumFixedDescendants);
                                theUserRatingThisLevel.MaximumValue = Math.Min(maxValEachEntry, maximumOtherDescendants + sumFixedDescendants);
                                if (theUserRatingThisLevel.MinimumValue == theUserRatingThisLevel.MaximumValue)
                                {
                                    theUserRatingThisLevel.CannotBeChanged = true;
                                    theUserRatingThisLevel.EnteredValueOrCalculatedValue = theUserRatingThisLevel.MinimumValue;
                                }
                                else if (theUserRatingThisLevel.MinimumValue > theUserRatingThisLevel.MaximumValue)
                                    throw new UserRatingDataException("Invalid predictions entered. Please try again.");
                                else
                                {
                                    theUserRatingThisLevel.PreferredValue = Math.Min(maxValEachEntry, Math.Max(minValEachEntry, sumFixedDescendants + minimumOtherDescendants));
                                    if (constrainedSum == null)
                                        theUserRatingThisLevel.EnteredValueOrCalculatedValue = theUserRatingThisLevel.PreferredValue;
                                }
                            }
                        }
                    }
                }
            }
            // Now, check top level.
            if (constrainedSum != null)
            {
                var level1HierarchyEntries = UserRatingHierarchyEntries.Where(d => d.HierarchyLevel == 1);
                var level1HierarchyEntriesFixed = level1HierarchyEntries.Where(d => d.MadeDirectly || d.CannotBeChanged);
                var level1HierarchyEntriesOther = level1HierarchyEntries.Where(d => !d.MadeDirectly && !d.CannotBeChanged);
                if (!level1HierarchyEntriesOther.Any())
                { // all descendants are fixed
                    decimal sumLevel1 = level1HierarchyEntriesFixed.Sum(d => d.ValueStatus());
                    if ((decimal)constrainedSum != sumLevel1)
                        throw new UserRatingDataException("The numbers that you entered are inconsistent. Please try again.");
                }
                else
                { // at least one descendant is not fixed
                    decimal sumFixedLevel1 = level1HierarchyEntriesFixed.Sum(d => d.ValueStatus());
                    decimal sumOtherLevel1 = level1HierarchyEntriesOther.Sum(d => d.ValueStatus());
                    decimal minimumOtherLevel1 = level1HierarchyEntriesOther.Sum(d => (decimal)d.MinimumValue);
                    decimal maximumOtherLevel1 = level1HierarchyEntriesOther.Sum(d => (decimal)d.MaximumValue);
                    decimal level1minimumValue = minimumOtherLevel1 + sumFixedLevel1;
                    decimal level1maximumValue = Math.Min((decimal)constrainedSum, maximumOtherLevel1 + sumFixedLevel1);
                    if (level1minimumValue > level1maximumValue)
                        throw new UserRatingDataException("The numbers that you entered are inconsistent. Please try again.");
                }
            }
        }

        /// <summary>
        /// Adjust a set of predictions so that the new values sum up to a specified value.
        /// We assume that we have already confirmed that this is possible by calling AdjustUserRatingsUpHierarchy
        /// on the prediction hierarchy.
        /// </summary>
        /// <param name="theUserRatingsThisLevel">The target predictions for this level of the hierarchy</param>
        /// <param name="theSum">What these now need to sum to</param>
        /// <returns>True if this is success</returns>
        public void AdjustUserRatingsToSum(IEnumerable<UserRatingHierarchyEntry> theUserRatingsThisLevel, decimal theSum)
        {
            bool doAgain = true;
            while (doAgain)
            {
                var theUnchangeableUserRatingsThisLevel = theUserRatingsThisLevel.Where(d => d.CannotBeChanged || d.MadeDirectly);
                decimal unchangeableSum = theUnchangeableUserRatingsThisLevel.Sum(d => d.ValueStatus());
                var theChangeableUserRatingsThisLevel = theUserRatingsThisLevel.Where(d => !d.CannotBeChanged && !d.MadeDirectly);
                if (!theChangeableUserRatingsThisLevel.Any())
                {
                    if (theUserRatingsThisLevel.Sum(d => d.EnteredValueOrCalculatedValue) != theSum)
                        throw new UserRatingDataException("Internal error: UserRatings do not add up correctly.");
                    return;
                }
                decimal newChangeableSum = theSum - unchangeableSum;
                decimal portionNeededToMeetMinima = theChangeableUserRatingsThisLevel.Sum(d => (decimal)d.MinimumValue);
                decimal remainingAmountToDistribute = newChangeableSum - portionNeededToMeetMinima;
                if (remainingAmountToDistribute < 0)
                    throw new UserRatingDataException("Internal error: UserRatings cannot be adjusted");
                decimal sumWeightsForDistribution = theChangeableUserRatingsThisLevel.Sum(d => (decimal)d.PreferredValue - (decimal)d.MinimumValue);

                doAgain = false;
                // decimal sumPortionDistributedSoFar = 0;
                int lastEntryNum = theChangeableUserRatingsThisLevel.Last().EntryNum;
                foreach (var theChangeableUserRating in theChangeableUserRatingsThisLevel)
                {
                    decimal portionOfAmountToDistribute = 0;
                    if (sumWeightsForDistribution > 0)
                        portionOfAmountToDistribute = ((decimal)theChangeableUserRating.PreferredValue - (decimal)theChangeableUserRating.MinimumValue) / sumWeightsForDistribution;
                    else
                        portionOfAmountToDistribute = ((decimal)1 / (decimal)theChangeableUserRatingsThisLevel.Count());
                    if (theChangeableUserRating.EntryNum == lastEntryNum)
                    { // Make sure everything adds up despite rounding errors.
                        theChangeableUserRating.EnteredValueOrCalculatedValue = theSum - (theChangeableUserRatingsThisLevel.Where(d => d.EntryNum != lastEntryNum).Sum(d => (decimal)d.EnteredValueOrCalculatedValue) + theUnchangeableUserRatingsThisLevel.Sum(d => (decimal)d.EnteredValueOrCalculatedValue));
                        if (theChangeableUserRating.EnteredValueOrCalculatedValue < theChangeableUserRating.MinimumValue)
                            throw new UserRatingDataException("Internal error: UserRatings cannot be adjusted.");
                    }
                    else
                        theChangeableUserRating.EnteredValueOrCalculatedValue = Math.Round((decimal)theChangeableUserRating.MinimumValue + portionOfAmountToDistribute * remainingAmountToDistribute, 4);
                    if ((decimal)theChangeableUserRating.EnteredValueOrCalculatedValue > (decimal)theChangeableUserRating.MaximumValue)
                    {
                        doAgain = true; // We need to redistribute the amounts elsewhere, so let's do it again.
                        theChangeableUserRating.EnteredValueOrCalculatedValue = theChangeableUserRating.MaximumValue;
                        theChangeableUserRating.CannotBeChanged = true;
                    }
                }
            }
        }

        public void AdjustUserRatingsDownUserRatingHierarchyHelper(IEnumerable<UserRatingHierarchyEntry> predictionsToAdjust, decimal? sumToAdjustTo)
        {
            if (sumToAdjustTo != null)
                AdjustUserRatingsToSum(predictionsToAdjust, (decimal)sumToAdjustTo);
            foreach (var theUserRatingThisLevel in predictionsToAdjust)
            {
                var immediateDescendants = UserRatingHierarchyEntries.Where(d => d.Superior == theUserRatingThisLevel.EntryNum);
                if (immediateDescendants.Any())
                    AdjustUserRatingsDownUserRatingHierarchyHelper(immediateDescendants, (decimal)theUserRatingThisLevel.EnteredValueOrCalculatedValue);
            }
        }

        public void AdjustUserRatingsDownUserRatingHierarchy(decimal? constrainedSum)
        {
            var theUserRatingsThisLevel = UserRatingHierarchyEntries.Where(d => d.HierarchyLevel == 1);
            AdjustUserRatingsDownUserRatingHierarchyHelper(theUserRatingsThisLevel, constrainedSum);
        }


        public void ApplyAdjustmentPercentageDownUserRatingHierarchy(
            float adjustPct, 
            decimal? constrainedSum)
        {
            var theUserRatingsThisLevel = UserRatingHierarchyEntries.Where(d => d.HierarchyLevel == 1);
            ApplyAdjustmentPercentageDownUserRatingHierarchyHelper(theUserRatingsThisLevel, adjustPct, constrainedSum);
        }


        public void ApplyAdjustmentPercentageDownUserRatingHierarchyHelper(IEnumerable<UserRatingHierarchyEntry> predictionsToAdjust, 
            float adjustmentFactor, decimal? sumToAdjustTo)
        {
            List<UserRatingHierarchyEntry> changedPredictions = predictionsToAdjust.Where(x => /* x.enteredValueOrCalculatedValue != x.originalTrustedValue && */x.EnteredValueOrCalculatedValue != null && x.OriginalTrustedValue != null).ToList();
            int numPredictionsToAdjust = changedPredictions.Count();
            int i = 0;
            decimal sumOriginalValues = 0;
            decimal sumNewValuesAdjusted = 0;
            foreach (var predictionToAdjust in changedPredictions)
            {
                sumOriginalValues += (decimal)predictionToAdjust.OriginalTrustedValue;
                // last one
                if (i == numPredictionsToAdjust - 1 && sumToAdjustTo != null)
                    // Make the sum of all of the new values equal to teh sum of the old values.
                    predictionToAdjust.NewValueAfterAdjustment = sumOriginalValues - sumNewValuesAdjusted;
                else
                    predictionToAdjust.NewValueAfterAdjustment = predictionToAdjust.OriginalTrustedValue.Value + (decimal)adjustmentFactor *
                        (predictionToAdjust.EnteredValueOrCalculatedValue.Value - predictionToAdjust.OriginalTrustedValue.Value);
                sumNewValuesAdjusted += (decimal)predictionToAdjust.NewValueAfterAdjustment;
                i++;
            }
            foreach (var theUserRatingThisLevel in predictionsToAdjust)
            {
                if (theUserRatingThisLevel.EnteredValueOrCalculatedValue != null && theUserRatingThisLevel.OriginalTrustedValue == null)
                    theUserRatingThisLevel.NewValueAfterAdjustment = (decimal)theUserRatingThisLevel.EnteredValueOrCalculatedValue;
                var immediateDescendants = this.UserRatingHierarchyEntries.Where(d => d.Superior == theUserRatingThisLevel.EntryNum);
                if (immediateDescendants.Any())
                    ApplyAdjustmentPercentageDownUserRatingHierarchyHelper(immediateDescendants, adjustmentFactor, (decimal)theUserRatingThisLevel.EnteredValueOrCalculatedValue);
            }
        }


        public void AddDerivativeUserRatingsToUserRatingHierarchy(decimal? constrainedSum, decimal minValEachEntry, decimal maxValEachEntry)
        {
            // Trace.TraceInformation("BEFORE UP");
            // DebugOutput();
            AdjustUserRatingsUpUserRatingHierarchy(constrainedSum, minValEachEntry, maxValEachEntry);
            // Trace.TraceInformation("AFTER UP");
            // DebugOutput();
            AdjustUserRatingsDownUserRatingHierarchy(constrainedSum);
            if (constrainedSum != null && constrainedSum != UserRatingHierarchyEntries.Where(x => x.HierarchyLevel == 1).Sum(x => x.ValueStatus()))
                throw new UserRatingDataException("Invalid or incomplete ratings entered. Please try again.");
            ConfirmRatingConsistency();
        }

        public void ConfirmRatingConsistency()
        {
            var superiors = UserRatingHierarchyEntries.Where(y => UserRatingHierarchyEntries.Select(x => x.Superior).Distinct().Contains(y.EntryNum));
            bool failure = superiors.Any(x => UserRatingHierarchyEntries.Where(y => y.Superior == x.EntryNum).Sum(z => z.ValueStatus()) != x.ValueStatus());
            if (failure)
                throw new UserRatingDataException("Invalid or incomplete ratings entered. Please try again.");
        }

    }



    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        // Methods related to making predictions in prediction groups

        public void AddRatingGroupStatusRecord(RatingGroup theRatingGroup, decimal? oldValueOfFirstRating)
        {
            RatingGroupStatusRecord theRecord = new RatingGroupStatusRecord
            {
                RatingGroup = theRatingGroup,
                NewValueTime = TestableDateTime.Now,
                OldValueOfFirstRating = oldValueOfFirstRating
            };
            DataContext.GetTable<RatingGroupStatusRecord>().InsertOnSubmit(theRecord);
            DataContext.RegisterObjectToBeInserted(theRecord);
        }

        /// <summary>
        /// Adds a rating group and hierarchically below groups to the prediction hierarchy. A helper routine.
        /// </summary>
        /// <param name="ratingGroupID">The rating group.</param>
        /// <param name="theData">The data</param>
        /// <param name="hierarchyLevel">The hierarchy at which addition occurs</param>
        public void AddRatingGroupToUserRatingHierarchy(RatingGroup theRatingGroup, List<Rating> theRatings, List<RatingGroup> theRatingGroups, ref UserRatingHierarchyData theData, int hierarchyLevel)
        {
            // The below data is for the end of the routine, but we load it now to minimize the chance that it will have been processed in the interim.
            UserRatingsToAdd newUserRatings = null;
            if (hierarchyLevel == 1)
            {
                const int maxTries = 10;
                int numTries = 0;
            tryAgain:
                try
                {
                    //bool loadRatingsFromAzureTable = false; // This feature not yet implemented. Will have to change since we will need to use a serializable data structure instead of UserRatingsToAdd.
                    //TableServiceContextAccess<DataTableServiceEntity<UserRatingsToAdd>> context = null;
                    //if (loadRatingsFromAzureTable)
                    // newUserRatings = AzureTable<UserRatingsToAdd>.LoadDataTableServiceEntityByPartitionKey(theRatingGroup.RatingGroupID.ToString(), ref context, "UserRatingsToAdd").ToList().OrderByDescending(x => x.Timestamp).Select(x => x.GetData()).FirstOrDefault();
                    // else
                    newUserRatings = DataContext.GetTable<UserRatingsToAdd>().Where(x => x.TopRatingGroupID == theRatingGroup.RatingGroupID).ToList().OrderByDescending(x => x.UserRatingsToAddID).FirstOrDefault();
                }
                catch
                {
                    numTries++;
                    if (numTries > maxTries)
                        throw new Exception("Cannot add rating because a database problem occurred.");
                    Thread.Sleep(1000);
                    goto tryAgain;
                }
            }

            // Create the prediction hierarchy based on ratings that have already been processed, if any.
            var theRatings2 = theRatings.Where(m => m.RatingGroupID == theRatingGroup.RatingGroupID).Select(m => new
            {
                RatingID = m.RatingID,
                Rating = m,
                RatingGroupID = m.RatingGroupID,
                CurrentUserRatingOrFinalValue = m.CurrentValue,
                LastTrustedValue = m.LastTrustedValue,
                OwnedRatingGroupID = m.OwnedRatingGroupID
            }
                );
            foreach (var theRating in theRatings2)
            {
                decimal lastTrustedValueOrBasisOfCalc = theRating.LastTrustedValue ?? RaterooDataManipulation.GetAlternativeBasisForCalcIfNoPreviousUserRating(DataContext, theRating.Rating, theRatingGroup.RatingGroupAttribute);
                theData.Add(theRating.RatingID, theRating.RatingGroupID, theRating.LastTrustedValue, theRating.CurrentUserRatingOrFinalValue, lastTrustedValueOrBasisOfCalc, null, false, !theRatingGroup.RatingGroupAttribute.RatingsCanBeAutocalculated, hierarchyLevel);
                if (theRating.OwnedRatingGroupID != null)
                    AddRatingGroupToUserRatingHierarchy(theRatingGroups.Single(mg => mg.RatingGroupID == (int)theRating.OwnedRatingGroupID), theRatings, theRatingGroups, ref theData, hierarchyLevel + 1);
            }
            // Check whether there is going to be a new prediction hierarchy shortly because of very recently added ratings.
            if (hierarchyLevel == 1)
            {
                if (newUserRatings != null)
                {
                    UserRatingHierarchyData replacementData = BinarySerializer.Deserialize<UserRatingHierarchyData>(newUserRatings.UserRatingHierarchy.ToArray());
                    var replaceEntries = replacementData.UserRatingHierarchyEntries.Where(x => x.EnteredValueOrCalculatedValue != null || x.OriginalTrustedValue != null);
                    foreach (var replaceEntry in replaceEntries)
                    {
                        var entryToChange = theData.UserRatingHierarchyEntries.Single(x => x.EntryNum == replaceEntry.EntryNum);
                        var realCurrentValue = replaceEntry.EnteredValueOrCalculatedValue ?? replaceEntry.OriginalTrustedValue; // could be multiple predictions
                        if (entryToChange.OriginalTrustedValue != realCurrentValue)
                        {
                            //Trace.TraceInformation("Replacing in " + entryToChange.entryNum + " " + entryToChange.originalValue + " with " + realCurrentValue);
                            entryToChange.OriginalTrustedValue = realCurrentValue;
                        }
                    }
                }
                //else
                //    Trace.TraceInformation("No newer ratings for replacement.");
            }
        }


        /// <summary>
        /// Get a prediction hierarchy that does not yet contain predictions. 
        /// </summary>
        /// <param name="ratingGroupID"></param>
        /// <param name="theData"></param>
        public void GetUserRatingHierarchyWithoutUserRatings(RatingGroup theRatingGroup, List<Rating> theRatings, List<RatingGroup> theRatingGroups, ref UserRatingHierarchyData theData)
        {
            AddRatingGroupToUserRatingHierarchy(theRatingGroup, theRatings, theRatingGroups, ref theData, 1);
        }

        /// <summary>
        /// Add a prediction to a prediction hierarchy.
        /// </summary>
        /// <param name="ratingID">The rating in which the prediction is made</param>
        /// <param name="theUserRating">The prediction made</param>
        /// <param name="theData">The prediction hierarchy</param>
        public void AddUserRatingToUserRatingHierarchy(int ratingID, decimal theUserRating, ref UserRatingHierarchyData theData)
        {
            UserRatingHierarchyEntry theEntry = theData.UserRatingHierarchyEntries.SingleOrDefault(d => d.RatingId == ratingID);
            if (theEntry != null)
            {
                theEntry.EnteredValueOrCalculatedValue = theUserRating;
                theEntry.MadeDirectly = true;
            }
        }

        /// <summary>
        /// Get prediction hierarchy data, based on a particular rating and prediction.
        /// </summary>
        /// <param name="ratingID">The rating</param>
        /// <param name="theUserRating">The prediction</param>
        /// <param name="theData">The prediction hierarchy data</param>
        public void GetUserRatingHierarchyBasedOnUserRating(int ratingID, decimal? constrainedSum, int ratingGroupID, List<Rating> theRatings, List<RatingGroup> theRatingGroups, decimal theUserRatingEntered, float adjustPct, ref UserRatingHierarchyData theData)
        {
            GetUserRatingHierarchyBasedOnUserRatings(new List<RatingIdAndUserRatingValue> { new RatingIdAndUserRatingValue { RatingID = ratingID, UserRatingValue = theUserRatingEntered } }, theRatings, theRatingGroups, constrainedSum, adjustPct, ref theData);
        }

        public void GetUserRatingHierarchyBasedOnUserRatings(
            List<RatingIdAndUserRatingValue> theUserRatings, 
            List<Rating> theRatings, 
            List<RatingGroup> theRatingGroups, 
            decimal? constrainedSum, 
            float adjustPct, 
            ref UserRatingHierarchyData userRatingHierarchyData)
        {
            RatingGroup topRatingGroup = theRatingGroups.Single(x => x.RatingGroupID == theRatings.FirstOrDefault().TopmostRatingGroupID);
            GetUserRatingHierarchyWithoutUserRatings(topRatingGroup, theRatings, theRatingGroups, ref userRatingHierarchyData);
            CheckIncompleteInitialEntryOfRatings(theUserRatings, userRatingHierarchyData);

            for (int i = 0; i < theUserRatings.Count(); i++)
            {
                AddUserRatingToUserRatingHierarchy(theUserRatings[i].RatingID, theUserRatings[i].UserRatingValue, ref userRatingHierarchyData);
            }

            RatingCharacteristic theRatingChars = topRatingGroup.RatingGroupAttribute.RatingCharacteristic;
            userRatingHierarchyData.AddDerivativeUserRatingsToUserRatingHierarchy(constrainedSum, theRatingChars.MinimumUserRating, theRatingChars.MaximumUserRating);
            userRatingHierarchyData.ApplyAdjustmentPercentageDownUserRatingHierarchy(adjustPct, constrainedSum);
        }

        private static void CheckIncompleteInitialEntryOfRatings(List<RatingIdAndUserRatingValue> theUserRatings, UserRatingHierarchyData theData)
        {
            if (theUserRatings.Count != theData.UserRatingHierarchyEntries.Count)
            {
                bool dataIncludesNullLeafValues = false;
                var theNullVals = theData.UserRatingHierarchyEntries.Where(x => x.OriginalTrustedValue == null && !theUserRatings.Any(y => y.RatingID == x.RatingId));
                foreach (var theNullVal in theNullVals)
                {
                    if (!theData.UserRatingHierarchyEntries.Any(y => y.Superior == theNullVal.EntryNum))
                    {
                        dataIncludesNullLeafValues = true;
                        break;
                    }
                }
                if (dataIncludesNullLeafValues)
                    throw new UserRatingDataException("The ratings you have entered are incomplete. Please rate each item, or wait until Rateroo has completely processed a complete list of ratings.");
            }
        }

        public void AddUserRatingsBasedOnOneOrMore(List<RatingIdAndUserRatingValue> ratingIdAndUserRatingValues, List<Rating> theRatings, List<RatingGroup> theRatingGroups, User theUser, ref UserRatingResponse theResponse)
        {
            int numTries = 0;
        TryLabel:
            try
            {
                numTries++;
                // we're only doing this now on second try BackgroundThread.Instance.RequestPauseAndWait();
                //Trace.TraceInformation("Starting to add predictions.");
                //RaterooDB.GetTable<SetUserRatingAddOption>(); doesn't seem to work
                Rating firstRating = theRatings.Single(m => m.RatingID == ratingIdAndUserRatingValues[0].RatingID);
                RatingGroup topRatingGroup = firstRating.RatingGroup2;
                decimal? constrainedSum = topRatingGroup.RatingGroupAttribute.ConstrainedSum;

                RatingIdAndUserRatingValue firstUserRating = ratingIdAndUserRatingValues.First();
                UserRatingHierarchyAdditionalInfo additionalInfo;
                List<int> choiceInGroupIDs = topRatingGroup.TblRow.Fields
                    .SelectMany(y => y.ChoiceFields)
                    .Where(y => y.Field.FieldDefinition.ChoiceGroupFieldDefinitions
                        .Any(z => z.TrackTrustBasedOnChoices)) // there really should be no more than 1
                    .SelectMany(y => y.ChoiceInFields)
                    .Select(y => y.ChoiceInGroupID)
                    .ToList();
                var choiceTrustTrackers = topRatingGroup.TblRow.Tbl.TrustTrackerForChoiceInGroups
                        .Where(u => u.User == theUser && choiceInGroupIDs.Any(y => y == u.ChoiceInGroupID))
                        .Select(x => new TrustTrackerChoiceSummary
                        {
                            ChoiceInGroupID = x.ChoiceInGroupID,
                            SumAdjustmentPctTimesRatingMagnitude = x.SumAdjustmentPctTimesRatingMagnitude,
                            SumRatingMagnitudes = x.SumRatingMagnitudes,
                            TrustValueForChoice = x.TrustLevelForChoice
                        }).ToList();
                var otherChoiceInFieldIDs = choiceInGroupIDs.Where(x => !choiceTrustTrackers.Any(y => y.ChoiceInGroupID == x)).ToList();
                TrustTrackerStatManager theManager = new TrustTrackerStatManager(
                    DataContext, 
                    theRatings.Single(x => x.RatingID == firstUserRating.RatingID), 
                    theUser, 
                    firstUserRating.UserRatingValue, 
                    choiceTrustTrackers, 
                    otherChoiceInFieldIDs,
                    out additionalInfo
                    );

                UserRatingHierarchyData theData = new UserRatingHierarchyData(additionalInfo);
                GetUserRatingHierarchyBasedOnUserRatings(ratingIdAndUserRatingValues, theRatings, theRatingGroups, constrainedSum, 
                    theManager.AdjustmentFactor, ref theData);

                if (RatingGroupIsResolved(topRatingGroup))
                    throw new UserRatingDataException("You cannot enter a rating, because this table cell is completed.");

                //PointsManager thePointsManager = topRatingGroup.TblRow.Tbl.PointsManager;
                //bool userIsTrusted = UserIsTrusted(thePointsManager, theUser);

                // We will not yet add the predictions to the relevant tables, but instead will store data in the database,
                // so that the predictions to add can be completed later. This reduces the danger of transaction deadlock.
                AddUserRatingsToAdd(theUser, theData);


                TblColumnFormatting theFormatting = PMNumberandTableFormatter.GetFormattingForTblColumn(topRatingGroup.TblColumnID);
                // Set the currentValues based on what we anticipate these will be once the predictions are added.
                theResponse.currentValues = theData.UserRatingHierarchyEntries
                        .Select(m => new RatingAndUserRatingString
                        {
                            ratingID = m.RatingId.ToString(),
                            theUserRating = PMNumberandTableFormatter.FormatAsSpecified(m.NewValueAfterAdjustment, theRatings.Single(y => y.RatingID == m.RatingId).RatingCharacteristic.DecimalPlaces, theFormatting) + (additionalInfo.IsTrusted ? "" : "*")
                        }).ToList();
            }
            catch (System.Data.Linq.ChangeConflictException)
            {
                if (numTries < 3 && !CheckJavaScriptHelper.IsJavascriptEnabled)
                {
                    BackgroundThread.Instance.RequestBriefPauseAndWaitForPauseToBegin();
                    goto TryLabel;
                }
                else throw new Exception("Sorry, Rateroo is busy processing other ratings. Try again soon.");
            }
            catch (TransactionException)
            {
                if (numTries < 3 && !CheckJavaScriptHelper.IsJavascriptEnabled)
                {
                    BackgroundThread.Instance.RequestBriefPauseAndWaitForPauseToBegin();
                    goto TryLabel;
                }
                else throw new Exception("Sorry, Rateroo is busy processing other ratings. Try again soon.");
            }
        }

        static bool loadUserRatingsToAddFromAzure = false; // feature not yet implemented...
        // The other challenges for implementing this are around AddUserRatingsToAdd and AddRatingGroupToUserRatingHierarchy.

        public bool CompleteMultipleAddUserRatings()
        {
            DataContext.SetUserRatingAddingLoadOptions();
            int numAtATime = 100;
            /* We manually load the first rating in rating group to speed processing of common scenario. The load options should bring everything else in. */
            List<UserRatingsToAdd> userRatingsToAddList = null;
            //IQueryable<DataTableServiceEntity<UserRatingsToAdd>> userRatingsToAddDataServiceEntities = null;
            if (loadUserRatingsToAddFromAzure)
            {
                throw new NotImplementedException();
                //List<string> partitionKeys = AzureQueue.Pop("UserRatingsToAddQueue", numAtATime).ConvertAll<string>(x => (string)x);
                //theUserRatingsInfos = new List<UserRatingsToAdd>();
                //foreach (string partitionKey in partitionKeys)
                //{
                //    TableServiceContextAccess<DataTableServiceEntity<UserRatingsToAdd>> context = null;
                //    userRatingsToAddDataServiceEntities = AzureTable<UserRatingsToAdd>.LoadDataTableServiceEntityByPartitionKey(partitionKey, ref context, "UserRatingsToAdd");
                //    theUserRatingsInfos = theUserRatingsInfos.Concat<UserRatingsToAdd>(userRatingsToAddDataServiceEntities.Select(x => x.GetData()).AsEnumerable()).ToList();
                //}
            }
            else
                userRatingsToAddList = DataContext.GetTable<UserRatingsToAdd>()
                        .Take(numAtATime)
                        .ToList();

            //var DEBUGx = theUserRatingsInfos.FirstOrDefault();
            //if (DEBUGx != null)
            //{
            //    var myTest = DEBUGx.RatingGroup;
            //    var myTest2 = DEBUGx.RatingGroup.Ratings;
            //    var firstRating = DEBUGx.RatingGroup.Ratings.First();
            //    var pointsTotal = DEBUGx.User.PointsTotals.SingleOrDefault(pt => pt.User == DEBUGx.User && pt.PointsManager == DEBUGx.RatingGroup.TblRow.Tbl.PointsManager);
            //    Debug.WriteLine("CompleteMultiple firstRatingID " + firstRating.RatingID + " pointsTotal is null? " + (pointsTotal == null).ToString());
            //}

            var theUserRatingsInfoList = userRatingsToAddList
                        .Select(urta => new
                        {
                            UserRatingsToAdd = urta,
                            //FirstRating = urta.RatingGroup.Ratings.First(),
                            PointsTotal = urta.User.PointsTotals.SingleOrDefault(pt =>
                                pt.User.UserID == urta.User.UserID &&
                                pt.PointsManager == urta.RatingGroup.TblRow.Tbl.PointsManager)
                        })
                        .ToList();
            foreach (var theUserRatingsInfo in theUserRatingsInfoList)
            {
                try
                {
                    CompleteAddUserRatings(theUserRatingsInfo.UserRatingsToAdd, theUserRatingsInfo.PointsTotal);
                }
                catch
                {
                }
                finally
                { // if there is an error, we just disregard the prediction.
                    //if (loadUserRatingsToAddFromAzure)
                    //    userRatingsToAddDataServiceEntities.ToList().ForEach(x => AzureTable<UserRatingsToAdd>.Delete(x, context, "UserRatingsToAdd"));
                    //else
                    DataContext.GetTable<UserRatingsToAdd>().DeleteOnSubmit(theUserRatingsInfo.UserRatingsToAdd);
                }
            }

            return theUserRatingsInfoList.Count() == numAtATime; // If we had this many, we still may have more work to do.
        }

        public void CompleteAddUserRatings(UserRatingsToAdd userRatingsToAdd, PointsTotal theUserPointsTotal)
        {
            UserRatingHierarchyData userRatingHierarchyData =
                BinarySerializer.Deserialize<UserRatingHierarchyData>(userRatingsToAdd.UserRatingHierarchy.ToArray());

            List<Rating> theRatings;
            List<RatingGroup> theRatingGroups;
            RatingGroup topRatingGroup;
            userRatingHierarchyData.GetRatingsAndGroups(DataContext, userRatingsToAdd, out theRatings, out theRatingGroups, out topRatingGroup);

            UserRatingGroup theUserRatingGroup = AddUserRatingGroup(topRatingGroup);

            foreach (UserRatingHierarchyEntry theEntry in userRatingHierarchyData.UserRatingHierarchyEntries)
            {
                Rating theRating = theRatings.Single(x => x.RatingID == theEntry.RatingId);
                if (theEntry.MadeDirectly && theEntry.EnteredValueOrCalculatedValue != null)
                {
                    RatingPhaseStatus theRatingPhaseStatus = GetRatingPhaseStatus(theRating);
                    AddUserRating(userRatingsToAdd.User, theRating, theUserRatingGroup,
                                    theUserPointsTotal, theRatingPhaseStatus, theRatingGroups,
                                    (decimal) theEntry.EnteredValueOrCalculatedValue,
                                    theEntry.NewValueAfterAdjustment,
                                    theEntry.OriginalTrustedValueOrBasisForCalculatingPoints,
                                    theEntry.MadeDirectly, userRatingHierarchyData.AdditionalInfo);
                }
            }
        }


        public bool CheckWhetherRatingCanBeSetToValue(int ratingID, List<Rating> theRatings, List<RatingGroup> theRatingGroups, decimal theValue)
        {
            RatingGroup theRatingGroup = theRatingGroups.Single(mg => mg.RatingGroupID == theRatings.Single(m => m.RatingID == ratingID).RatingGroupID);
            int ratingGroupID = theRatingGroup.RatingGroupID;
            decimal? constrainedSum = theRatingGroups.Single(mg => mg.RatingGroupID == theRatings.First().TopmostRatingGroupID).RatingGroupAttribute.ConstrainedSum;
            UserRatingHierarchyData theData = new UserRatingHierarchyData();
            try
            {
                GetUserRatingHierarchyBasedOnUserRating(ratingID, constrainedSum, (int)ratingGroupID, theRatings, theRatingGroups, theValue, 1F /* we're not actually setting to this value, just checking */, ref theData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckWhetherRatingCanBeSetWinner(int ratingID, List<Rating> theRatings, List<RatingGroup> theRatingGroups)
        {
            Rating theRating = theRatings.Single(m => m.RatingID == ratingID);
            int ratingGroupID = theRating.RatingGroupID;
            decimal? constrainedSum = theRatingGroups.Single(mg => mg.RatingGroupID == theRating.TopmostRatingGroupID).RatingGroupAttribute.ConstrainedSum;
            if (constrainedSum == null)
                return false;
            else
                return CheckWhetherRatingCanBeSetToValue(ratingID, theRatings, theRatingGroups, (decimal)constrainedSum);
        }
    }
}