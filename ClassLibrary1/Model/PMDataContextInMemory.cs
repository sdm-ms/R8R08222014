using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using System.Data.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ClassLibrary1.Model
{

    public class RaterooInMemoryDataContext : InMemoryContext, IRaterooDataContext
    {
        public System.IO.TextWriter Log
        {
            get
            {
                return null;
            }
            set
            {
                // Nada
            }
        }

        public System.Data.Common.DbCommand GetCommand(IQueryable query)
        {
            return null;
        }

        public bool IsRealDatabase()
        {
            return false;
        }

        public RaterooDataContext GetRealDatabaseIfExists()
        {
            return null;
        }


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


        public bool ResolveConflictsIfPossible()
        {
            return true;
        }

        public void BaseClassSubmitChanges(ConflictMode conflictMode)
        {
            this.InsertOnSubmitObjectsToBeInserted<SearchWord>();
            base.SubmitChanges(conflictMode);
        }


        public override void SubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            BaseClassSubmitChanges(conflictMode);
        }

        public override void SubmitChanges()
        {
            BaseClassSubmitChanges(ConflictMode.ContinueOnConflict);
        }

        RaterooDataContext underlyingRaterooDataContext;

        public RaterooInMemoryDataContext()
            : base(new RaterooDataContext("no real connection" /* AzureSetup.GetConfigurationSetting("RaterooConnectionString") */))
        {
            underlyingRaterooDataContext = (RaterooDataContext)UnderlyingDataContext;
        }


        public void SetUserRatingAddOptions()
        {
        }

        public void SetPageLoadOptions()
        {
        }

        public void LoadStatsWithTrustTrackersAndUserInteractions()
        {
        }
        public void SetUserRatingUpdatingLoadOptions()
        {
        }

        public void SetUserRatingAddingLoadOptions()
        {
        }

        public IQueryable<TblRow> UDFNearestNeighborsForTbl(float? latitude, float? longitude, int? maxNumResults, int? tblID)
        {
            return ( 
                from tblrow in GetTable<TblRow>().AsQueryable().Where(x => x.TblID == tblID)
                let addressField = tblrow.Fields.SelectMany(y => y.AddressFields).FirstOrDefault()
                let hasAddressField = addressField != null && !(addressField.Latitude == null) && !(addressField.Longitude == null) && !(addressField.Latitude == 0 && addressField.Longitude == 0)
                let distance = GeoCodeCalc.CalcDistance((double) (latitude ?? 0), (double) (longitude ?? 0), (double) (addressField.Latitude ?? 0), (double) (addressField.Longitude ?? 0))
                orderby hasAddressField descending, distance
                select tblrow
                )
                .Take(maxNumResults ?? 9999999)
                .AsQueryable<TblRow>();
        }

        public IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance)
        {
            return (
                   from addressField in GetTable<AddressField>().AsQueryable()
                   let hasAddressField = addressField != null && !(addressField.Latitude == null) && !(addressField.Longitude == null) && !(addressField.Latitude == 0 && addressField.Longitude == 0)
                   let theDistance = GeoCodeCalc.CalcDistance((double)(latitude ?? 0), (double)(longitude ?? 0), (double)(addressField.Latitude ?? 0), (double)(addressField.Longitude ?? 0))
                   where hasAddressField && theDistance < distance
                   select addressField
                   )
                   .AsQueryable<AddressField>();
        }
    }
}
