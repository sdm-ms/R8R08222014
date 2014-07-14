using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Data.Linq;
using System.Reflection;

namespace ClassLibrary1.OldModel
{
    public partial class R8RDataContext
    {
        //public new void Dispose() // Note: Because this hides Dispose() from the base class it won't get called when Dispose is called on IDataContext. Tried to implement standard Dispose(bool disposing) pattern, but it seems that the DataContext base class is not set up properly for that. So, we must make sure to close the connection state when resetting the data context, and we must remember to do that.
        //{
        //    var changes = this.GetChangeSet();
        //    if (changes.Deletes.Any() || changes.Inserts.Any() || changes.Updates.Any())
        //    {
        //        Trace.TraceError("Changes exist, but DataContext is being disposed.");
        //        //var theChanges = this.GetChangedItems();
        //        //foreach (var theChange in theChanges)
        //        //    theChange.DebugOutput();
        //    }
        //    if (this.Connection != null)
        //    {
        //        if (this.Connection.State != System.Data.ConnectionState.Closed)
        //            this.Connection.Close();
        //    }
        //}

        //partial void OnCreated()
        //{
        //}


        //public void ClearContextCache()
        //{
        //    const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
        //    var method = this.GetType().GetMethod("ClearCache", FLAGS);
        //    method.Invoke(this, null);
        //}

    }

    public class R8RSQLDataContext : SQLDataContext, ClassLibrary1.Model.IR8RDataContext
    {
        public System.IO.TextWriter Log
        {
            get
            {
                return _underlyingR8RDataContext.Log;
            }
            set
            {
                _underlyingR8RDataContext.Log = value;
            }
        }

        public System.Data.Common.DbCommand GetCommand(IQueryable query)
        {
            return _underlyingR8RDataContext.GetCommand(query);
        }

        public bool IsRealDatabase()
        {
            return true;
        }

        public ClassLibrary1.Model.IR8RDataContext GetRealDatabaseIfExists()
        {
            return _underlyingR8RDataContext;
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

        public override void SubmitChanges()
        {
            BeforeSubmitChanges();
            this.RepeatedlyAttemptToSubmitChanges(ConflictMode.ContinueOnConflict);
        }

        public override void SubmitChanges(ConflictMode conflictMode)
        {
            SubmitChanges(); // ignore conflictmode setting
        }

        public override void BeforeSubmitChanges()
        {
        }

        R8RDataContext _underlyingR8RDataContext;

        public R8RSQLDataContext(bool doAllowChangeData, bool enableObjectTracking) :
            base(new R8RDataContext(ClassLibrary1.Model.ConnectionString.GetR8RNormalizedDatabaseConnectionString()))
        {
            _underlyingR8RDataContext = (ClassLibrary1.Model.IR8RDataContext)UnderlyingDataContext;
            _allowChangeData = doAllowChangeData;
            if (!enableObjectTracking)
                _underlyingR8RDataContext.ObjectTrackingEnabled = false;
        }



        public bool ResolveConflictsIfPossible()
        {
            if (!_underlyingR8RDataContext.ChangeConflicts.Any())
                return false; // conflicts not listed here (probably won't happen)
            foreach (ObjectChangeConflict occ in _underlyingR8RDataContext.ChangeConflicts)
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


        public void LoadStatsWithTrustTrackersAndUserInteractions()
        {
            if (TooLateToSetPageLoadOptions)
                return;
            DataLoadOptions dl = new DataLoadOptions();
            dl.LoadWith<TrustTracker>(x => x.TrustTrackerStats);
            dl.LoadWith<UserInteraction>(x => x.UserInteractionStats);
            _underlyingR8RDataContext.LoadOptions = dl;
            TooLateToSetPageLoadOptions = true;
        }

        public void SetUserRatingAddingLoadOptions()
        {
            if (TooLateToSetPageLoadOptions)
                return;
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

            _underlyingR8RDataContext.LoadOptions = dl;
            TooLateToSetPageLoadOptions = true;
        }

        public IQueryable<TblRow> UDFNearestNeighborsForTbl(float? latitude, float? longitude, int? maxNumResults, int? tblID)
        {
            return _underlyingR8RDataContext.UDFNearestNeighborsForTbl(latitude, longitude, maxNumResults, tblID).Cast<TblRow>();
        }

        public IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance)
        {
            return _underlyingR8RDataContext.UDFDistanceWithin(latitude, longitude, distance).Cast<AddressField>();
        }
    }
}
