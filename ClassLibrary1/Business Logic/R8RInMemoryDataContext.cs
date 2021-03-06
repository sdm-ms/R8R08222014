﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using System.Data.Linq;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Business_Logic
{

    public class R8RInMemoryDataContext : InMemoryContext, IR8RDataContext
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

        public IR8RDataContext GetRealDatabaseIfExists()
        {
            return null;
        }

        internal DictionaryByType _RegisteredToBeInserted = new DictionaryByType();
        public DictionaryByType RegisteredToBeInserted
        {
            get
            {
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

        public override void BeforeSubmitChanges()
        {
            base.BeforeSubmitChanges();
        }

        // When converting to Entity Framework, we will need to use the R8RContext instead.
        ClassLibrary1.OldModel.OldDataContext underlyingR8RDataContext;
        public R8RInMemoryDataContext(InMemoryDatabase simulatedPermanentStorage)
            : base(new ClassLibrary1.OldModel.OldDataContext("no real connection" /* AzureSetup.GetConfigurationSetting("R8RConnectionString") */), simulatedPermanentStorage)
        {
            underlyingR8RDataContext = (ClassLibrary1.OldModel.OldDataContext)UnderlyingDataContext;
        }

        // NOTE: Keep this commented out code for now -- if we want to implement support for nearest neighbors on the normalized database, we will need it. We will have to change int? to Guid? for tblID
        //public IQueryable<TblRow> UDFNearestNeighborsForTbl(float? latitude, float? longitude, int? maxNumResults, int? tblID)
        //{
        //    return (
        //        from tblrow in GetTable<TblRow>().AsQueryable().Where(x => x.TblID == tblID)
        //        let addressField = tblrow.Fields.SelectMany(y => y.AddressFields).FirstOrDefault()
        //        let hasAddressField = addressField != null && !(addressField.Latitude == null) && !(addressField.Longitude == null) && !(addressField.Latitude == 0 && addressField.Longitude == 0)
        //        let distance = GeoCodeCalc.CalcDistance((double)(latitude ?? 0), (double)(longitude ?? 0), (double)(addressField.Latitude ?? 0), (double)(addressField.Longitude ?? 0))
        //        orderby hasAddressField descending, distance
        //        select tblrow
        //        )
        //        .Take(maxNumResults ?? 9999999)
        //        .AsQueryable<TblRow>();
        //}

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
