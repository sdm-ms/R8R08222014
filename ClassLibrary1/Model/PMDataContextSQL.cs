using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Data.Linq;

namespace ClassLibrary1.Model
{
    public partial class RaterooDataContext
    {

        public new void Dispose()
        {
            var changes = this.GetChangeSet();
            if (changes.Deletes.Any() || changes.Inserts.Any() || changes.Updates.Any())
            {
                Trace.TraceError("Changes exist, but DataContext is being disposed.");
                //var theChanges = this.GetChangedItems();
                //foreach (var theChange in theChanges)
                //    theChange.DebugOutput();
            }
            if (this.Connection != null)
                if (this.Connection.State != System.Data.ConnectionState.Closed)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }
        }
    }

    public class RaterooSQLDataContext : SQLDataContext, IRaterooDataContext
    {
        public System.IO.TextWriter Log
        {
            get
            {
                return _underlyingRaterooDataContext.Log;
            }
            set
            {
                _underlyingRaterooDataContext.Log = value;
            }
        }

        public System.Data.Common.DbCommand GetCommand(IQueryable query)
        {
            return _underlyingRaterooDataContext.GetCommand(query);
        }

        public bool IsRealDatabase()
        {
            return true;
        }

        public RaterooDataContext GetRealDatabaseIfExists()
        {
            return _underlyingRaterooDataContext;
        }


        protected bool _allowChangeData;
        public bool AllowChangeData { get { return _allowChangeData; } }
        public static DataLoadOptions PageDataLoadOptions = null; // must save data load options in static variable for use with compiled queries

        internal List<object> _RegisteredToBeInserted;
        public List<object> RegisteredToBeInserted
        {
            get
            {
                if (_RegisteredToBeInserted == null)
                    _RegisteredToBeInserted = new List<object>();
                return _RegisteredToBeInserted;
            }
            set
            {
                _RegisteredToBeInserted = value;
            }
        }

        internal Dictionary<string, object> _TempCache;
        public Dictionary<string, object> TempCache
        {
            get
            {
                if (_TempCache == null)
                    _TempCache = new Dictionary<string, object>();
                return _TempCache;
            }
            set
            {
                _TempCache = value;
            }
        }

        public override void SubmitChanges(ConflictMode conflictMode)
        {
            BeforeSubmitChanges();
            this.RepeatedlyAttemptToSubmitChanges(ConflictMode.ContinueOnConflict);
        }

        public override void BeforeSubmitChanges()
        {
            this.InsertOnSubmitObjectsToBeInserted<SearchWord>();
        }

        RaterooDataContext _underlyingRaterooDataContext;

        public RaterooSQLDataContext(bool doAllowChangeData, bool enableObjectTracking) : 
            base( new RaterooDataContext(GetIRaterooDataContext.UseRealDatabase ? 
                AzureSetup.GetConfigurationSetting("RaterooConnectionString") : 
                "FakeConnectionStringForInMemoryTesting"))
        {
            _underlyingRaterooDataContext = (RaterooDataContext) UnderlyingDataContext;

            _allowChangeData = doAllowChangeData;
            if (!enableObjectTracking)
                _underlyingRaterooDataContext.ObjectTrackingEnabled = false;
        }



        public bool ResolveConflictsIfPossible()
        {
            if (!_underlyingRaterooDataContext.ChangeConflicts.Any())
                return false; // conflicts not listed here (probably won't happen)
            foreach (ObjectChangeConflict occ in _underlyingRaterooDataContext.ChangeConflicts)
            {
                if (!occ.MemberConflicts.Any())
                    return false; // member conflicts not listed here (probably won't happen)
                foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                {
                    bool resolvable = ResolveMemberChangeConflictIfPossible(mcc);
                    if (!resolvable)
                        return false;
                }
            }
            return true;
        }

        public bool ResolveMemberChangeConflictIfPossible(System.Data.Linq.MemberChangeConflict theConflict)
        {
            RefreshMode theMode;
            bool canResolve;
            HowToResolveMemberChangeConflict(theConflict, out canResolve, out theMode);
            if (canResolve)
                theConflict.Resolve(theMode);
            return canResolve;
        }

        public void HowToResolveMemberChangeConflict(System.Data.Linq.MemberChangeConflict theConflict, out bool canResolve, out RefreshMode theRefreshMode)
        {
            canResolve = false;
            theRefreshMode = RefreshMode.KeepChanges; // default value for out parameter.
            if (theConflict.Member.ReflectedType == typeof(DatabaseStatus))
                canResolve = true;
            else if (theConflict.Member.ReflectedType == typeof(PointsManager))
            {
                if (theConflict.CurrentValue == theConflict.OriginalValue)
                {
                    theRefreshMode = RefreshMode.OverwriteCurrentValues; // keep the other user's change
                    canResolve = true;
                }
            }
            else if (theConflict.Member.ReflectedType == typeof(PointsTotal))
            {
                if (theConflict.CurrentValue == theConflict.OriginalValue)
                {
                    theRefreshMode = RefreshMode.OverwriteCurrentValues; // keep the other user's change
                    canResolve = true;
                }
            }
            if (!canResolve)
            {
                Trace.TraceError("Could not resolve ChangeConflictException for " + theConflict.Member.ReflectedType.ToString() + " " + theConflict.Member.Name + " Current: " + theConflict.CurrentValue + " Original: " + theConflict.OriginalValue + " Database: " + theConflict.DatabaseValue);
            }
        }

        public void SetUserRatingAddOptions()
        {
            //Connection.Open();
            _underlyingRaterooDataContext.ExecuteCommand("SET DEADLOCK_PRIORITY HIGH;");
        }

        public void SetPageLoadOptions()
        {
            //Connection.Open();
            _underlyingRaterooDataContext.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            return;
            // Experimentation seems to indicate that using eager loading, even only a bit, slows things down on the main page a lot. This may be because I keep calling the database multiple times anyway, instead of using properties based on initial calls.
            //if (PageDataLoadOptions == null)
            //{
            //    DataLoadOptions dl = new DataLoadOptions();
            //    dl.LoadWith<TblRow>(x => x.Tbl);
            //    dl.LoadWith<Tbl>(x => x.PointsManager);
            //    dl.LoadWith<PointsManager>(x => x.Domain);
            //    dl.LoadWith<Tbl>(x => x.TblTabs);
            //    dl.LoadWith<TblTab>(x => x.TblColumns);
            //    dl.LoadWith<TblColumn>(x => x.TblColumnFormattings);
            //    dl.LoadWith<Tbl>(x => x.FieldDefinitions);
            //    dl.LoadWith<FieldDefinition>(x => x.ChoiceGroupFieldDefinitions);
            //    dl.LoadWith<FieldDefinition>(x => x.NumberFieldDefinitions);
            //    dl.LoadWith<FieldDefinition>(x => x.DateTimeFieldDefinitions);
            //    dl.LoadWith<FieldDefinition>(x => x.TextFieldDefinitions);
            //    dl.LoadWith<ChoiceGroupFieldDefinition>(x => x.ChoiceGroup);
            //    dl.LoadWith<ChoiceGroup>(x => x.ChoiceInGroups);
            //    PageDataLoadOptions = dl;
            //}
            //LoadOptions = PageDataLoadOptions;
        }

        public void LoadStatsWithTrustTrackersAndUserInteractions()
        {
            DataLoadOptions dl = new DataLoadOptions();
            dl.LoadWith<TrustTracker>(x => x.TrustTrackerStats);
            dl.LoadWith<UserInteraction>(x => x.UserInteractionStats);
            _underlyingRaterooDataContext.LoadOptions = dl;
        }

        public void SetUserRatingUpdatingLoadOptions()
        {
            DataLoadOptions dl = new DataLoadOptions();
            dl.LoadWith<RatingGroup>(x => x.RatingGroupAttribute);
            dl.LoadWith<RatingGroupAttribute>(x => x.RatingCharacteristic);
            dl.LoadWith<RatingCharacteristic>(x => x.RatingPhaseGroup);
            dl.LoadWith<RatingCharacteristic>(x => x.SubsidyDensityRangeGroup);
            dl.LoadWith<RatingPhaseGroup>(x => x.RatingPhases);
            dl.LoadWith<SubsidyDensityRangeGroup>(x => x.SubsidyDensityRanges);
            dl.LoadWith<RatingGroup>(x => x.TblRow);
            dl.LoadWith<TblRow>(x => x.Tbl);
            dl.LoadWith<Tbl>(x => x.PointsManager);

            _underlyingRaterooDataContext.LoadOptions = dl;
        }

        public void SetUserRatingAddingLoadOptions()
        {
            DataLoadOptions dl = new DataLoadOptions();
            dl.LoadWith<Rating>(x => x.RatingCharacteristic);
            dl.LoadWith<Rating>(x => x.RatingGroup);
            dl.LoadWith<Rating>(x => x.RatingGroup1);
            dl.LoadWith<Rating>(x => x.RatingGroup2);
            dl.LoadWith<RatingGroup>(x => x.TblRow);
            dl.LoadWith<RatingGroup>(x => x.TblColumn);
            dl.LoadWith<RatingGroup>(x => x.VolatilityTrackers);
            dl.LoadWith<VolatilityTracker>(x => x.VolatilityTblRowTracker);
            dl.LoadWith<TblRow>(x => x.RewardPendingPointsTrackers);
            dl.LoadWith<TblRow>(x => x.Tbl);
            dl.LoadWith<Tbl>(x => x.PointsManager);
            dl.LoadWith<PointsManager>(x => x.Domain);
            dl.LoadWith<RatingGroup>(x => x.RatingGroupAttribute);
            dl.LoadWith<RatingGroup>(x => x.RatingGroupPhaseStatus);
            dl.LoadWith<RatingGroupPhaseStatus>(x => x.SubsidyAdjustments);
            dl.LoadWith<RatingGroupPhaseStatus>(x => x.RatingPhase);
            dl.LoadWith<RatingGroupPhaseStatus>(x => x.RatingPhaseStatus);
            dl.LoadWith<RatingCharacteristic>(x => x.SubsidyDensityRangeGroup);
            dl.LoadWith<SubsidyDensityRangeGroup>(x => x.SubsidyDensityRanges);

            _underlyingRaterooDataContext.LoadOptions = dl;
        }

        public IQueryable<TblRow> UDFNearestNeighborsForTbl(float? latitude, float? longitude, int? maxNumResults, int? tblID)
        {
            return _underlyingRaterooDataContext.UDFNearestNeighborsForTbl(latitude, longitude, maxNumResults, tblID).Cast<TblRow>();
        }

        public IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance)
        {
            return _underlyingRaterooDataContext.UDFDistanceWithin(latitude, longitude, distance).Cast<AddressField>();
        }
    }
}
