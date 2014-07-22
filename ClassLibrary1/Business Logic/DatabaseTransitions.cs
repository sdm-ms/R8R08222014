using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;
using System.ComponentModel;

namespace ClassLibrary1.Model
{

    /// <summary>
    /// Summary description for DatabaseTransitions
    /// </summary>
    public partial class R8RDataManipulation
    {

        public void Transition1()
        {
            // We've added some new points tracking fields, so now we need to set them to the correct values.
            foreach (PointsTotal theTotal in DataContext.GetTable<PointsTotal>())
            {
                theTotal.NotYetPendingPoints = DataContext.GetTable<UserRating>().Where(p => p.UserID == theTotal.UserID && p.Rating.RatingGroup.TblRow.Tbl.PointsManagerID == theTotal.PointsManagerID).Sum(p => p.NotYetPendingPointsShortTerm + p.NotYetPendingPointsLongTerm);
            }
            foreach (PointsManager thePointsManager in DataContext.GetTable<PointsManager>())
            {
                var theUserRatings = DataContext.GetTable<UserRating>().Where(p => p.Rating.RatingGroup.TblRow.Tbl.PointsManagerID == thePointsManager.PointsManagerID);
                if (!theUserRatings.Any())
                    thePointsManager.CurrentUserPendingPoints = 0;
                else
                    thePointsManager.CurrentUserPendingPoints = theUserRatings.Sum(p => p.PendingPoints);
                var theUserRatings2 = DataContext.GetTable<UserRating>().Where(p => p.Rating.RatingGroup.TblRow.Tbl.PointsManagerID == thePointsManager.PointsManagerID);
                if (!theUserRatings2.Any())
                    thePointsManager.CurrentUserNotYetPendingPoints = 0;
                else
                    thePointsManager.CurrentUserNotYetPendingPoints = theUserRatings2.Sum(p => p.NotYetPendingPointsShortTerm + p.NotYetPendingPointsLongTerm);
            }
            DataContext.SubmitChanges();
        }

        public void DeleteAllTblRowStatusRecords()
        {
            bool moreWorkToDo = true;
            while (moreWorkToDo)
            {
                var theTblRowStatusRecords = DataContext.GetTable<TblRowStatusRecord>().Take(5000);
                moreWorkToDo = theTblRowStatusRecords.Any();
                DataContext.GetTable<TblRowStatusRecord>().DeleteAllOnSubmit<TblRowStatusRecord>(theTblRowStatusRecords);
                DataContext.SubmitChanges();
            }
        }

        public void AddMissingVolatilityTrackers()
        {
            bool moreToDo = true;
            int i = 0;
            while (moreToDo)
            {
                var ent = DataContext.GetTable<TblRow>().Where(x => !x.VolatilityTblRowTrackers.Any()).Take(200).Select(x => new { TblRow = x, RatingGroups = x.RatingGroups, VolatilityTblRowTrackers = x.VolatilityTblRowTrackers }).ToList();
                moreToDo = ent.Any();
                foreach (var e in ent)
                    VolatilityTracking.AddVolatilityTracking(this, e.TblRow);
                DataContext.SubmitChanges();
                i += 200;
                //Trace.TraceInformation("Adding volatility tracking " + i);
            }
        }

        public void UpdateLastTrustedValue()
        {
            var theRatings = DataContext.GetTable<Rating>();
            foreach (var rating in theRatings)
            {
                rating.LastTrustedValue = rating.CurrentValue;
            }
            DataContext.SubmitChanges();
        }

        public void DeleteAllOfType<K>(IQueryable<K> theItems) where K : class
        {
            int numTries = 0;

        tryStart:
            try
            {
                numTries++;
                DeleteAllOfTypeHelper(theItems);
            }
            catch (Exception ex)
            {
                Trace.Write("Inner exception: ");
                Trace.TraceError(ex.Message);
                System.Threading.Thread.Sleep(10000);
                if (numTries < 6)
                    goto tryStart;
                else throw;
            }
        }

        public void DeleteAllOfTypeHelper<K>(IQueryable<K> theItems) where K : class
        {
            const int numAtTime = 100;
            IQueryable<K> items = theItems.Take(numAtTime);
            IQueryable<K> remainingItems = theItems.Skip(numAtTime);
            while (items.Any())
            {
                DataContext.GetTable<K>().DeleteAllOnSubmit<K>(items);
                DataContext.SubmitChanges();
                items = remainingItems.Take(numAtTime);
            }
        }


        public void DeleteUserRatingDataInTable(Guid tblID)
        {
            int numTries = 0;

        tryStart:
            try
            {
                numTries++;
                DeleteUserRatingDataInTableHelper(tblID);
            }
            catch (Exception ex)
            {
                Trace.Write("Outer exception: ");
                Trace.TraceError(ex.Message);
                System.Threading.Thread.Sleep(10000);
                if (numTries < 6)
                    goto tryStart;
                else throw;
            }
        }

        public void DeleteAllDataInTable(Guid tblID)
        {
            int numTries = 0;

        tryStart:
            try
            {
                numTries++;
                DeleteAllDataInTableHelper(tblID);
            }
            catch (Exception ex)
            {
                Trace.Write("Outer exception: ");
                Trace.TraceError(ex.Message);
                System.Threading.Thread.Sleep(10000);
                if (numTries < 6)
                    goto tryStart;
                else throw;
            }
        }

        public void DeleteAllDataInTableHelper(Guid tblID)
        {
            var tbl = DataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);
            var pmID = tbl.PointsManagerID;
            if (tbl.Name != "Changes")
            { // call recursively to Changes table if necessary
                var changesTbl = DataContext.GetTable<Tbl>().Single(x => x.Name == "Changes" && x.PointsManagerID == pmID);
                if (changesTbl != null)
                    DeleteAllDataInTable(changesTbl.TblID);
            }

            DeleteUserRatingDataInTableHelper(tblID);
            IQueryable<RatingCondition> items6 = DataContext.GetTable<RatingCondition>().Where(x => x.Rating.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingCondition>(items6);
            IQueryable<RatingPhaseStatus> items7 = DataContext.GetTable<RatingPhaseStatus>().Where(x => x.Rating.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingPhaseStatus>(items7);
            IQueryable<Rating> items8 = DataContext.GetTable<Rating>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<Rating>(items8);
            IQueryable<SubsidyAdjustment> items9 = DataContext.GetTable<SubsidyAdjustment>().Where(x => x.RatingGroupPhaseStatus.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<SubsidyAdjustment>(items9);
            IQueryable<RatingGroupPhaseStatus> items10 = DataContext.GetTable<RatingGroupPhaseStatus>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingGroupPhaseStatus>(items10);
            IQueryable<RatingPhase> items11 = DataContext.GetTable<RatingPhase>().Where(x => x.RatingPhaseGroup.RatingGroupPhaseStatuses.Any() && x.RatingPhaseGroup.RatingGroupPhaseStatuses.FirstOrDefault().RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingPhase>(items11);
            IQueryable<RatingPhaseGroup> items11a = DataContext.GetTable<RatingPhaseGroup>().Where(x => x.RatingGroupPhaseStatuses.Any() && x.RatingGroupPhaseStatuses.FirstOrDefault().RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingPhaseGroup>(items11a);
            IQueryable<RatingGroupResolution> items12 = DataContext.GetTable<RatingGroupResolution>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingGroupResolution>(items12);
            IQueryable<VolatilityTracker> items13 = DataContext.GetTable<VolatilityTracker>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<VolatilityTracker>(items13);
            IQueryable<RatingGroupStatusRecord> items14 = DataContext.GetTable<RatingGroupStatusRecord>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<RatingGroupStatusRecord>(items14);
            IQueryable<VolatilityTracker> items15a = DataContext.GetTable<VolatilityTracker>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<VolatilityTracker>(items15a);
            IQueryable<VolatilityTblRowTracker> items15 = DataContext.GetTable<VolatilityTblRowTracker>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<VolatilityTblRowTracker>(items15);
            IQueryable<RatingGroup> items16 = DataContext.GetTable<RatingGroup>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<RatingGroup>(items16);
            IQueryable<OverrideCharacteristic> items17 = DataContext.GetTable<OverrideCharacteristic>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<OverrideCharacteristic>(items17);
            IQueryable<TblColumnFormatting> items18 = DataContext.GetTable<TblColumnFormatting>().Where(x => x.TblColumn.TblTab.TblID == tblID);
            DeleteAllOfType<TblColumnFormatting>(items18);
            //IQueryable<TblColumn> items19 = R8RDB.GetTable<TblColumn>().Where(x => x.TblTab.TblID == tblID);
            //DeleteAllOfType<TblColumn>(items19);
            //IQueryable<TblTab> items20 = R8RDB.GetTable<TblTab>().Where(x => x.TblID == tblID);
            //DeleteAllOfType<TblTab>(items20);
            IQueryable<SearchWordChoice> items21 = DataContext.GetTable<SearchWordChoice>().Where(x => x.ChoiceInGroup.ChoiceGroup.PointsManagerID == pmID);
            DeleteAllOfType<SearchWordChoice>(items21);
            IQueryable<SearchWordTblRowName> items22 = DataContext.GetTable<SearchWordTblRowName>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<SearchWordTblRowName>(items22);
            IQueryable<SearchWordTextField> items23 = DataContext.GetTable<SearchWordTextField>().Where(x => x.TextField.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<SearchWordTextField>(items23);
            IQueryable<NumberField> items24 = DataContext.GetTable<NumberField>().Where(x => x.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<NumberField>(items24);
            IQueryable<TextField> items25 = DataContext.GetTable<TextField>().Where(x => x.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<TextField>(items25);
            IQueryable<AddressField> items25a = DataContext.GetTable<AddressField>().Where(x => x.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<AddressField>(items25a);
            IQueryable<ChoiceInField> items26 = DataContext.GetTable<ChoiceInField>().Where(x => x.ChoiceField.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<ChoiceInField>(items26);
            IQueryable<ChoiceField> items26b = DataContext.GetTable<ChoiceField>().Where(x => x.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<ChoiceField>(items26b);
            IQueryable<DateTimeField> items26c = DataContext.GetTable<DateTimeField>().Where(x => x.Field.FieldDefinition.TblID == tblID);
            DeleteAllOfType<DateTimeField>(items26c);
            IQueryable<Field> items27 = DataContext.GetTable<Field>().Where(x => x.FieldDefinition.TblID == tblID);
            DeleteAllOfType<Field>(items27);
            IQueryable<Comment> items28 = DataContext.GetTable<Comment>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<Comment>(items28);
            IQueryable<RewardPendingPointsTracker> items29 = DataContext.GetTable<RewardPendingPointsTracker>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<RewardPendingPointsTracker>(items29);
            IQueryable<TblRowStatusRecord> items30 = DataContext.GetTable<TblRowStatusRecord>().Where(x => x.TblRow.TblID == tblID);
            DeleteAllOfType<TblRowStatusRecord>(items30);
            IQueryable<TblRow> items31 = DataContext.GetTable<TblRow>().Where(x => x.TblID == tblID);
            DeleteAllOfType<TblRow>(items31);
            IQueryable<TblRowFieldDisplay> items32 = DataContext.GetTable<TblRow>().Where(x => x.TblID == tblID).Select(x => x.TblRowFieldDisplay);
            DeleteAllOfType<TblRowFieldDisplay>(items32);
        }

        private void DeleteUserRatingDataInTableHelper(Guid tblID)
        {
            var tbl = DataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);
            var pmID = tbl.PointsManagerID;
            if (tbl.Name != "Changes")
            { // call recursively to Changes table if necessary
                var changesTbl = DataContext.GetTable<Tbl>().Single(x => x.Name == "Changes" && x.PointsManagerID == pmID);
                if (changesTbl != null)
                    DeleteUserRatingDataInTable(changesTbl.TblID);
            }

            IQueryable<UserRating> items = DataContext.GetTable<UserRating>().Where(x => x.Rating.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<UserRating>(items);
            IQueryable<UserRatingsToAdd> items2 = DataContext.GetTable<UserRatingsToAdd>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<UserRatingsToAdd>(items2);
            IQueryable<UserRatingGroup> items3 = DataContext.GetTable<UserRatingGroup>().Where(x => x.RatingGroup.TblRow.TblID == tblID);
            DeleteAllOfType<UserRatingGroup>(items3);
            IQueryable<PointsAdjustment> items4 = DataContext.GetTable<PointsAdjustment>().Where(x => x.PointsManagerID == pmID);
            DeleteAllOfType<PointsAdjustment>(items4);
            IQueryable<PointsTotal> items5 = DataContext.GetTable<PointsTotal>().Where(x => x.PointsManagerID == pmID);
            DeleteAllOfType<PointsTotal>(items5);

            ResetRatingsToNullInTable(tblID);
        }

        private void ResetRatingsToNullInTable(Guid tblID)
        {
            const int numAtOnce = 100;
            bool keepGoing = true;
            while (keepGoing)
            {
                var ratingsToReset = DataContext.GetTable<Rating>().Where(x => (x.CurrentValue != null || x.LastTrustedValue != null) && x.RatingGroup.TblRow.TblID == tblID).Take(numAtOnce);
                keepGoing = ratingsToReset.Any();
                foreach (var rtng in ratingsToReset)
                {
                    rtng.CurrentValue = null;
                    rtng.LastTrustedValue = null;
                }
                DataContext.SubmitChanges();
                ResetDataContexts();
            }
            keepGoing = true;
            while (keepGoing)
            {
                var ratingsToReset = DataContext.GetTable<RatingGroup>().Where(x => x.CurrentValueOfFirstRating != null && x.TblRow.TblID == tblID).Take(numAtOnce);
                keepGoing = ratingsToReset.Any();
                foreach (var rtng in ratingsToReset)
                {
                    rtng.CurrentValueOfFirstRating = null;
                }
                DataContext.SubmitChanges();
                ResetDataContexts();
            }

        }

        public void SimplifyRestaurants()
        {
            ActionProcessor Action = new ActionProcessor();
            Tbl Restaurants = Action.DataContext.GetTable<Tbl>().Single(x => x.Name == "Restaurants");
            TblTab OnlyTab = Restaurants.TblTabs.First();
            TblColumn Overall = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Overall");
            TblColumn Taste = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Taste");
            TblColumn Creativity = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Creativity");
            TblColumn Service = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Service");
            TblColumn Atmosphere = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Atmosphere");
            TblColumn Price = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Price");
            TblColumn Speed = OnlyTab.TblColumns.Single(x => x.Abbreviation.Trim() == "Speed");
            Taste.Status = (int)StatusOfObject.Unavailable;
            Service.Status = (int)StatusOfObject.Unavailable;
            Creativity.Status = (int)StatusOfObject.Unavailable;
            Speed.Status = (int)StatusOfObject.Unavailable;
            Overall.Name = "Quality of food";
            Overall.Abbreviation = "Food";
            Overall.Explanation = "The taste, creativity, and plating of the food";
            Atmosphere.Abbreviation = "Experience";
            Atmosphere.Name = "Quality of experience";
            Atmosphere.Explanation = "The overall quality of the service, decor, and atmosphere";
            Price.WidthStyle = "wv10"; // from "wv15". Must change back if adding back more columns.
            Action.DataContext.SubmitChanges();
        }

        public void ChangeSupremeCourtMembership()
        {
            ActionProcessor Action = new ActionProcessor();
            Guid superUser = DataContext.GetTable<User>().Single(u => u.Username == "admin").UserID;
            Guid theRatingPhaseID = DataContext.GetTable<RatingPhaseGroup>().Single(x => x.Name == "Two week phases").RatingPhaseGroupID;
            Guid votesRating = Action.RatingCharacteristicsCreate(theRatingPhaseID, null, 0, 9, 1, "Votes", true, true, superUser, null);
            Guid indVoteRating = Action.RatingCharacteristicsCreate(theRatingPhaseID, null, 0, 1, 1, "Individual Vote", true, true, superUser, null);
            TblColumn supremeCourtVotesTableColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblTab.Tbl.Name == "Supreme Court" && x.Abbreviation == "Votes" && x.Name == "Number of expected votes");
            PointsManager supremeCourtPointsManager = supremeCourtVotesTableColumn.TblTab.Tbl.PointsManager;

            RatingHierarchyData theHierarchy = new RatingHierarchyData();
            theHierarchy.Add("Total votes", 9, 1, "Total votes");
            theHierarchy.Add("Yes votes", 4.5M, 2, "Total expected yes votes");
            theHierarchy.Add("Roberts", 0.5M, 3, "Probability Roberts votes yes");
            theHierarchy.Add("Scalia", 0.5M, 3, "Probability Scalia votes yes");
            theHierarchy.Add("Kennedy", 0.5M, 3, "Probability Kennedy votes yes");
            theHierarchy.Add("Thomas", 0.5M, 3, "Probability Thomas votes yes");
            theHierarchy.Add("Ginsburg", 0.5M, 3, "Probability Ginsburg votes yes");
            theHierarchy.Add("Breyer", 0.5M, 3, "Probability Breyer votes yes");
            theHierarchy.Add("Alito", 0.5M, 3, "Probability Alito votes yes");
            theHierarchy.Add("Sotomayor", 0.5M, 3, "Probability Sotomayor votes yes");
            theHierarchy.Add("Kagan", 0.5M, 3, "Probability Kagan votes yes");
            theHierarchy.Add("No votes", 4.5M, 2, "Total expected no votes");
            var yesVotesItem = theHierarchy.RatingHierarchyEntries.Single(x => x.RatingName == "Yes votes");

            var replacement = new List<ActionProcessor.RatingCharacteristicsHierarchyOverride> { new ActionProcessor.RatingCharacteristicsHierarchyOverride { theEntryForRatingGroupWhoseMembersWillHaveDifferentCharacteristics = yesVotesItem, theReplacementCharacteristicsID = indVoteRating } };
            var newSupremeCourtHierarchyRGA = Action.RatingGroupAttributesCreate(votesRating, replacement, null, null, theHierarchy, "Supreme Court vote totals", RatingGroupTypes.hierarchyNumbersTop, "Supreme Court vote totals", false, false, new List<RatingHierarchyEntry> { yesVotesItem }, (decimal)0.5, true, true, superUser, null, supremeCourtPointsManager.PointsManagerID);

            // reload to get new data context, and make change          
            ResetDataContexts();
            supremeCourtVotesTableColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblTab.Tbl.Name == "Supreme Court" && x.Abbreviation == "Votes" && x.Name == "Number of expected votes");
            supremeCourtVotesTableColumn.DefaultRatingGroupAttributesID = newSupremeCourtHierarchyRGA;
            DataContext.SubmitChanges();

        }

        public void AddFastAccessTables(DenormalizedTableAccess dta)
        {
            foreach (var tbl in DataContext.GetTable<Tbl>())
            {
                new FastAccessTableInfo(DataContext, tbl).AddTable(dta);
            }
        }

        public void DropFastAccessTables(DenormalizedTableAccess dta)
        {
            foreach (var tbl in DataContext.GetTable<Tbl>())
            {
                tbl.FastTableSyncStatus = (int)FastAccessTableStatus.fastAccessNotCreated;
                CacheManagement.InvalidateCacheDependency("TblID" + tbl.TblID);
                DataContext.SubmitChanges();
                new FastAccessTableInfo(DataContext, tbl).DropTable(dta);
            }
            DataContext.SubmitChanges();
        }

        public void DatabaseTransition20110324()
        {
            SetTimeTracking(); 
            FixRatingGroupAttributesForPriceOfMeal();
            RecalculateUserRatingLongTermScores();
            SetNewPointTotalsFields();
        }

        public void SetTimeTracking()
        {
            var data = DataContext.GetTable<PointsTotal>().Select(x => new { PT = x, NumRatings = x.User.UserRatings.Where(y => y.PointsHaveBecomePending).Select(y => y.UserRatingGroup).Distinct().Count() });
            foreach (var d in data)
            {
                if (d.PT.TotalTimeEver == 0)
                {
                    decimal totalTime = (decimal)RaterTime.GetTimeForUser(d.PT.UserID, new DateTime(2000, 1, 1), TestableDateTime.Now).TotalHours; // technically, this is all tables; but at time of this transition, it should be almost all restaurants table
                    if (totalTime == 0)
                    {
                        d.PT.TotalTimeEver = 0;
                        d.PT.PointsPerHour = 0;
                        d.PT.ProjectedPointsPerHour = 0;
                    }
                    else
                    {
                        d.PT.TotalTimeEver = totalTime;
                        d.PT.PointsPerHour = (d.PT.TotalPoints + d.PT.PendingPoints) / totalTime;
                        d.PT.ProjectedPointsPerHour = d.PT.PointsPerHour;
                    }
                }
                DataContext.SubmitChanges();
            }
        }

        public void FixRatingGroupAttributesForPriceOfMeal()
        {
            PointsManager restaurants = DataContext.GetTable<PointsManager>().Single(x => x.Tbls.Any(y => y.Name == "Restaurants"));
            RatingGroupAttribute rga = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.Description == "Price of a meal");
            rga.LongTermPointsWeight = 0;
            rga.RatingEndingTimeVaries = true;

            int totalComplete = 0;
            int numAtOnce = 500;
            bool done = false;
            while (!done)
            {
                try
                {
                    var urs = DataContext.GetTable<UserRating>().Where(x => x.Rating.RatingGroup.RatingGroupAttribute.Description == "Price of a meal").Skip(totalComplete).Take(numAtOnce).Select(x => new { UR = x, PT = x.User.PointsTotals.Single(y => y.PointsManager == restaurants) } );
                    foreach (var ur in urs)
                    {
                        // no longer necessary now that we have consolidated maxloss and potentialpoints
                        //ur.UR.MaxLossLongTerm = 0;
                        //ur.UR.MaxLossShortTerm *= 2;
                        //ur.UR.PotentialPointsLongTerm = 0;
                        //ur.UR.PotentialPointsShortTerm *= 2;
                    }
                    DataContext.SubmitChanges();
                    if (!urs.Any())
                        done = true;
                    totalComplete += numAtOnce;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void RecalculateUserRatingLongTermScores()
        {
            int totalRowsComplete = 0;
            int numAtOnce = 300;
            bool done = false;
            while (!done)
            {
                try
                {
                    var results = DataContext.GetTable<UserRating>().Where(x => true ).Skip(totalRowsComplete).Take(numAtOnce).Select(x => new { UR = x, R = x.Rating, RG = x.UserRatingGroup.RatingGroup, URG = x.UserRatingGroup, RGPS = x.Rating.RatingGroup.RatingGroupPhaseStatuses.FirstOrDefault(y => y.StartTime <= x.UserRatingGroup.WhenCreated && x.UserRatingGroup.WhenCreated <= y.ActualCompleteTime), LTPW = x.UserRatingGroup.RatingGroup.RatingGroupAttribute.LongTermPointsWeight });
                    foreach (var result in results)
                    {
                        decimal maxLoss, maxGain, profit, potentialPointsLongTermUnweighted;
                        // Note: To do short term scores, we would need to figure out what the rating was at the relevant time.
                        if (result.RGPS != null)
                        {
                            CalculatePointsInfo(result.R, result.RG, result.RGPS, result.URG.WhenCreated, result.UR.PreviousRatingOrVirtualRating, (decimal) result.UR.NewUserRating, result.R.CurrentValue, false, result.LTPW, result.UR.HighStakesMultiplierOverride, result.UR.PastPointsPumpingProportion, out maxLoss, out maxGain, out profit, out potentialPointsLongTermUnweighted);
                            result.UR.MaxLoss = result.UR.MaxLossShortTerm + result.UR.MaxLossLongTerm;
                            result.UR.PotentialPointsLongTerm = profit;
                            result.UR.PotentialPointsLongTermUnweighted = potentialPointsLongTermUnweighted;
                            result.UR.LongTermPointsWeight = result.LTPW;
                        }
                    }
                    DataContext.SubmitChanges();
                    if (!results.Any())
                        done = true;
                    totalRowsComplete += numAtOnce;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        
        public void SetNewPointTotalsFields()
        {
            var allPT = DataContext.GetTable<PointsTotal>().Where(x => true);
            foreach (var pt in allPT)
            {
                pt.TotalPointsOrPendingPointsLongTermUnweighted = 0;
                pt.PointsPerRatingLongTerm = 0;
            }
            int totalRowsComplete = 0;
            int numAtOnce = 300;
            bool done = false;
            while (!done)
            {
                try
                {
                    var results = DataContext.GetTable<UserRating>().Where(x => true).Skip(totalRowsComplete).Take(numAtOnce).Select(x => new { UR = x, PT = x.User.PointsTotals.SingleOrDefault(y => y.PointsManagerID == x.Rating.RatingGroup.TblRow.Tbl.PointsManagerID) });
                    foreach (var result in results)
                    {
                        if (result.PT != null)
                        {
                            result.PT.TotalPointsOrPendingPointsLongTermUnweighted += result.UR.PointsOrPendingPointsLongTermUnweighted;
                            if (result.PT.NumPendingOrFinalizedRatings > 0)
                                result.PT.PointsPerRatingLongTerm = result.PT.TotalPointsOrPendingPointsLongTermUnweighted / result.PT.NumPendingOrFinalizedRatings;
                        }
                    }
                    DataContext.SubmitChanges();
                    if (!results.Any())
                        done = true;
                    totalRowsComplete += numAtOnce;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void DatabaseTransition20110214()
        {
            SetPointsPerRating();
            SetNeedsRatingScores();
        }

        public void SetPointsPerRating()
        {
            var data = DataContext.GetTable<PointsTotal>().Select(x => new { PT = x, NumRatings = x.User.UserRatings.Where(y => y.PointsHaveBecomePending).Select(y => y.UserRatingGroup).Distinct().Count() });
            foreach (var d in data)
            {
                d.PT.NumPendingOrFinalizedRatings = d.NumRatings;
                if ((decimal)d.NumRatings == 0)
                    d.PT.PointsPerRating = 0;
                else
                    d.PT.PointsPerRating = (d.PT.TotalPoints + d.PT.PendingPoints) / (decimal) d.NumRatings;
            }
            DataContext.SubmitChanges();
        }

        public void SetNeedsRatingScores()
        {
            int totalRowsComplete = 0;
            int numAtOnce = 500;
            bool done = false;
            while (!done)
            {
                try
                {
                    var tblRows = DataContext.GetTable<TblRow>().Skip(totalRowsComplete).Take(numAtOnce);
                    NeedsRatingScore.SetNeedsRatingScoreFields(DataContext, tblRows);
                    DataContext.SubmitChanges();
                    if (!tblRows.Any())
                        done = true;
                    totalRowsComplete += numAtOnce;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void FixUnupdatedRatings(Guid? tblID = null)
        {
            if (tblID == null)
                return;
            bool done = false;
            while (!done)
            {
                try
                {
                    var unupdatedRatings = from x in DataContext.GetTable<RatingGroup>()
                                           where x.TblRow.TblID == tblID
                                           where x.Ratings.Any()
                                           let lastUserRating = x.Ratings.SelectMany(y => y.UserRatings).OrderByDescending(y => y.UserRatingGroup.WhenCreated).FirstOrDefault()
                                           where lastUserRating != null &&
                                             (lastUserRating.NewUserRating != x.CurrentValueOfFirstRating ||
                                             lastUserRating.NewUserRating == null && x.CurrentValueOfFirstRating != null ||
                                             lastUserRating.NewUserRating != null & x.CurrentValueOfFirstRating == null)
                                           select new { UserRating = lastUserRating, Rating = lastUserRating.Rating, RatingGroup = lastUserRating.Rating.RatingGroup }
                                ;
                    var someUnupdated = unupdatedRatings.Take(50).ToList();
                    foreach (var unupdated in someUnupdated)
                    {
                        unupdated.Rating.CurrentValue = unupdated.UserRating.NewUserRating;
                        unupdated.Rating.LastTrustedValue = unupdated.UserRating.NewUserRating;
                        unupdated.RatingGroup.CurrentValueOfFirstRating = unupdated.UserRating.NewUserRating;
                    }
                    if (!someUnupdated.Any())
                        done = true;
                    DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void SetLastTrustedValueToCurrentValue()
        {
            Guid tblID = new Guid("asdf"); // set this to applicable GUID
            bool done = false;
            while (!done)
            {
                try
                {
                    var needingUpdating = DataContext.GetTable<Rating>().Where(x => x.RatingGroup.TblRow.TblID == tblID && x.LastTrustedValue != x.CurrentValue);
                    var someUnupdated = needingUpdating.Take(50).ToList();
                    foreach (var unupdated in someUnupdated)
                    {
                        unupdated.LastTrustedValue = unupdated.CurrentValue;
                    }
                    if (!someUnupdated.Any())
                        done = true;
                    DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void ChangeShortTermResolveTime()
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    var affectedPhaseStatuses = DataContext.GetTable<RatingGroupPhaseStatus>().Where(x => x.RatingGroup.TblRow.Tbl.Name == "Restaurants" && x.ShortTermResolveTime > x.ActualCompleteTime).Take(50);
                    foreach (var status in affectedPhaseStatuses)
                        status.ShortTermResolveTime = status.ActualCompleteTime;
                    if (!affectedPhaseStatuses.Any())
                        done = true;
                    DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void MakePointsPendingImminently()
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    DateTime now = TestableDateTime.Now;
                    var userRatings = DataContext.GetTable<UserRating>().Where(x => x.WhenPointsBecomePending > now).Take(50).ToList();
                    foreach (var userRating in userRatings)
                    {
                        userRating.WhenPointsBecomePending = TestableDateTime.Now - new TimeSpan(0, 1, 0);
                    }
                    if (!userRatings.Any())
                        done = true;
                    DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void RatingGroupPhaseChange()
        {
            Guid ratingPhaseGroupID = new Guid("asdf");  // MUST BE a group with one repeating phase
            Guid tblID = new Guid("asdf"); // set this to applicable GUID table
            DateTime currentPhaseCompleteTime = TestableDateTime.Now + new TimeSpan(3,0,0);
            DateTime highStakesKnownTime = TestableDateTime.Now + new TimeSpan(0, 30, 0);
            RatingPhaseGroup theRatingPhaseGroup = DataContext.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == ratingPhaseGroupID); // one day with half life of two
            RatingPhase theRatingPhase = theRatingPhaseGroup.RatingPhases.Single(x => x.NumberInGroup == 1);

            Tbl theTbl = DataContext.GetTable<Tbl>().Single(x => x.TblID == tblID);

            RatingGroupAttribute theRGAForTbl = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == theTbl.DefaultRatingGroupAttributesID);
            theRGAForTbl.RatingCharacteristic.RatingPhaseGroup = theRatingPhaseGroup;
            var tblColumns = theTbl.TblTabs.SelectMany<TblTab, TblColumn>(x => x.TblColumns);
            foreach (var tblColumn in tblColumns)
            {
                RatingGroupAttribute theRGAForTblColumn = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == tblColumn.DefaultRatingGroupAttributesID);
                theRGAForTblColumn.RatingCharacteristic.RatingPhaseGroup = theRatingPhaseGroup;
            }
            DataContext.SubmitChanges();

            bool done = false;
            while (!done)
            {
                try
                {
                    ResetDataContexts();
                    var ratingGroups = DataContext.GetTable<RatingGroup>().Where(x => x.TblRow.TblID == tblID && x.RatingGroupPhaseStatuses.Any() && x.RatingGroupPhaseStatuses.OrderByDescending(y => y.EarliestCompleteTime).FirstOrDefault().ActualCompleteTime != currentPhaseCompleteTime /* RatingPhaseGroupID != theRatingPhaseGroup.RatingPhaseGroupID */).Take(20);
                    foreach (var ratingGroup in ratingGroups)
                    {
                        RatingGroupPhaseStatus theRGPS = ratingGroup.RatingGroupPhaseStatuses.OrderByDescending(x => x.EarliestCompleteTime).FirstOrDefault();
                        if (theRGPS != null)
                        {
                            theRGPS.RatingPhaseGroupID = theRatingPhaseGroup.RatingPhaseGroupID;
                            theRGPS.RatingPhaseID = theRatingPhase.RatingPhaseID;
                            theRGPS.RoundNum = 1;
                            theRGPS.RoundNumThisPhase = 1;
                            theRGPS.ActualCompleteTime = currentPhaseCompleteTime;
                            theRGPS.EarliestCompleteTime = currentPhaseCompleteTime;
                            theRGPS.ShortTermResolveTime = currentPhaseCompleteTime;
                            if (theRGPS.HighStakesKnown)
                                theRGPS.HighStakesBecomeKnown = highStakesKnownTime;
                        }
                    }
                    if (!ratingGroups.Any())
                        done = true;
                    DataContext.SubmitChanges();
                    System.Threading.Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void CorrectHighStakesKnown()
        {
            var incorrect = DataContext.GetTable<RatingGroup>().Where(x => x.HighStakesKnown != x.RatingGroupPhaseStatuses.OrderByDescending(y => y.ActualCompleteTime).FirstOrDefault().HighStakesKnown);
            foreach (var item in incorrect)
                item.HighStakesKnown = !item.HighStakesKnown;
            DataContext.SubmitChanges();
        }

        public void FinishKnownHighStakesSoon(Guid tblID)
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    DateTime completeTime = TestableDateTime.Now + new TimeSpan(0, 2, 0);
                    DateTime now = TestableDateTime.Now;
                    var rgps = DataContext.GetTable<RatingGroupPhaseStatus>().Where(x => x.RatingGroup.TblRow.TblID == tblID && x.HighStakesKnown && x.HighStakesBecomeKnown < now && x.ActualCompleteTime > completeTime).Take(50).ToList();
                    foreach (var rgp in rgps)
                    {
                        rgp.ActualCompleteTime = rgp.ShortTermResolveTime = rgp.EarliestCompleteTime = completeTime;
                    }
                    if (!rgps.Any())
                        done = true;
                    DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void SetCompleteTimeForTbl()
        {
            Guid tblID = new Guid("asdf"); // set this to applicable GUID
            DateTime endTimeEarliest = new DateTime(2011, 2, 13, 13, 0, 0);
            DateTime endTimeLatest = new DateTime(2011, 2, 13, 14, 0, 0);
            if (endTimeEarliest > endTimeLatest || endTimeEarliest < TestableDateTime.Now)
                throw new Exception("Internal error SetCompleteTimeForTbl");

            bool done = false;
            while (!done)
            {
                try
                {
                    var rgps = DataContext.GetTable<RatingGroupPhaseStatus>().Where(x => x.RatingGroup.TblRow.TblID == tblID && x.ActualCompleteTime > endTimeLatest).Take(50).ToList();
                    foreach (var rgp in rgps)
                    {
                        DateTime completeTime = RandomGenerator.GetRandom(endTimeEarliest, endTimeLatest);
                        rgp.ActualCompleteTime = rgp.ShortTermResolveTime = rgp.EarliestCompleteTime = completeTime;
                    }
                    if (!rgps.Any())
                        done = true;
                    DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void TurnOffHighStakesForParticularUser(Guid tblID, Guid targetSpecificUser)
        {
            DateTime willBecomeKnownAt = TestableDateTime.Now + new TimeSpan(0, 1, 0); // one minute from now
            DateTime endTime = TestableDateTime.Now + new TimeSpan(3, 0, 0); // three hours
            DateTime now = TestableDateTime.Now;
            var activePhaseStatus = DataContext.GetTable<RatingGroupPhaseStatus>()
                .Where(x => x.StartTime < now && x.ActualCompleteTime > now)
                .Where(x => x.RatingGroup.TblRow.TblID == tblID);
            var highStakesStatus = activePhaseStatus.Where(x => x.HighStakesSecret || x.HighStakesKnown);
            var notHighStakes = activePhaseStatus.Where(x => !x.HighStakesSecret && !x.HighStakesKnown);

            var highStakesAlreadyKnown = highStakesStatus.Where(x => x.HighStakesBecomeKnown < now);
            var highStakesToBecomeKnown = highStakesStatus.Where(x => x.HighStakesBecomeKnown > now);

            var highStakesToChange = highStakesAlreadyKnown
                .Where(x => x.RatingGroup.UserRatingGroups.Any())
                .Where(x => x.RatingGroup.UserRatingGroups.Any(y => y.UserRatings.Any(z => z.UserID == (Guid)targetSpecificUser)))
                .OrderByDescending(x => x.ShortTermResolveTime);
            foreach (var toChange in highStakesToChange)
            {
                toChange.HighStakesSecret = false;
                toChange.HighStakesKnown = false;
                toChange.HighStakesBecomeKnown = null;
                toChange.ActualCompleteTime = endTime;
                toChange.ShortTermResolveTime = endTime;
            }
            DataContext.SubmitChanges();
            var userR = highStakesToChange.Select(x => x.RatingGroup).SelectMany(x => x.UserRatingGroups).SelectMany(x => x.UserRatings);
            foreach (var ur in userR)
            {
                ur.Rating.RatingGroup.HighStakesKnown = false;
                ur.ForceRecalculate = true;
            }
            DataContext.SubmitChanges();
        }

        public void ChangeToHighStakes(Guid tblID, int forceHighStakesNum, Guid? targetSpecificUser)
        {
            DateTime willBecomeKnownAt = TestableDateTime.Now + new TimeSpan(0,1,0); // one minute from now
            DateTime endTime = TestableDateTime.Now + new TimeSpan(1,0,0); // one hour
            DateTime now = TestableDateTime.Now;

            var activePhaseStatus = DataContext.GetTable<RatingGroupPhaseStatus>()
	            .Where(x => x.StartTime < now && x.ActualCompleteTime > now)
	            .Where(x => x.RatingGroup.TblRow.TblID == tblID);
            var highStakesStatus = activePhaseStatus.Where(x => x.HighStakesSecret || x.HighStakesKnown);
            var notHighStakes = activePhaseStatus.Where(x => !x.HighStakesSecret && !x.HighStakesKnown);

            var highStakesAlreadyKnown = highStakesStatus.Where(x => x.HighStakesBecomeKnown < now);
            var highStakesToBecomeKnown = highStakesStatus.Where(x => x.HighStakesBecomeKnown > now);

            var highStakesAlreadyKnownTblRows = highStakesAlreadyKnown.Select(x => new { RowName = x.RatingGroup.TblRow.Name, ActualCompleteTime = x.ActualCompleteTime, ShortTermResolveTime = x.ShortTermResolveTime}).Distinct();
            var highStakesToBecomeKnownTblRows = highStakesToBecomeKnown.Select(x => new { RowName = x.RatingGroup.TblRow.Name, ActualCompleteTime = x.ActualCompleteTime, ShortTermResolveTime = x.ShortTermResolveTime}).Distinct();


            if (forceHighStakesNum > 0)
            {
	            var notHighStakesToChange = notHighStakes
		            .Where(x => x.RatingGroup.UserRatingGroups.Any())
		            .Where(x => x.ShortTermResolveTime > willBecomeKnownAt)
		            .OrderByDescending(x => x.ShortTermResolveTime);
                var notHighStakesCount = 0;
                if (targetSpecificUser == null)
                    notHighStakesCount = notHighStakesToChange.Count();
                else
                    notHighStakesCount = notHighStakesToChange.Where(x => x.RatingGroup.UserRatingGroups.Any(y => y.UserRatings.Any(z => z.UserID == (Guid)targetSpecificUser))).Count();
                var numToSkip = RandomGenerator.GetRandom(0, Math.Max(notHighStakesCount - forceHighStakesNum - 1, 1));
                if (targetSpecificUser == null)
                    notHighStakesToChange = notHighStakesToChange.Skip(numToSkip).Take(forceHighStakesNum)
                    .OrderByDescending(x => x.ShortTermResolveTime);
                else
                    notHighStakesToChange = notHighStakesToChange.Where(x => x.RatingGroup.UserRatingGroups.Any(y => y.UserRatings.Any(z => z.UserID == (Guid)targetSpecificUser)))
                    .Skip(numToSkip)
                    .Take(forceHighStakesNum)
                    .OrderByDescending(x => x.ShortTermResolveTime);
	            foreach (var toChange in notHighStakesToChange)
	            {
                    toChange.HighStakesSecret = true;
                    toChange.HighStakesKnown = true;
                    toChange.HighStakesBecomeKnown = willBecomeKnownAt;
		            toChange.ActualCompleteTime = endTime;
		            toChange.ShortTermResolveTime = endTime;
	            }
		        DataContext.SubmitChanges();
	            // Note: The idle task will automatically make the high stakes known soon, and change the HighStakesKnown field of the rating group.
            }
        }
    }


 
}