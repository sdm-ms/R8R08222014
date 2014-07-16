using System.Text;
using ClassLibrary1.Misc;
using System.Data.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.EFModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using ClassLibrary1.Nonmodel_Code;
using System;

namespace ClassLibrary1.Model
{
    public class R8REFDataContext : EFDataContext, IR8RDataContext
    {
        R8RContext _underlyingR8RContext;

        public R8REFDataContext() : base(new R8RContext(ConnectionString.GetR8RNormalizedDatabaseConnectionString()))
        {
            _underlyingR8RContext = (R8RContext)UnderlyingDbContext;
        }

        public bool IsRealDatabase()
        {
            return true;
        }

        public IR8RDataContext GetRealDatabaseIfExists()
        {
            return this;
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

        public override void BeforeSubmitChanges()
        {
            foreach (ObjectStateEntry entry in
                GetObjectContext().ObjectStateManager.GetObjectStateEntries(
                EntityState.Added)) // | EntityState.Modified))
            {
                if (!entry.IsRelationship && entry.Entity.GetType() == typeof(TblRow))
                {
                    TblRow tblRow = entry.Entity as TblRow;
                    tblRow.NotYetAddedToDatabase = false;
                }
                else if (!entry.IsRelationship && entry.Entity.GetType() == typeof(TblColumn))
                {
                    TblColumn tblColumn = entry.Entity as TblColumn;
                    tblColumn.NotYetAddedToDatabase = false;
                }
                else if (!entry.IsRelationship && entry.Entity.GetType() == typeof(Field))
                {
                    Field field = entry.Entity as Field;
                    field.NotYetAddedToDatabase = false;
                }
            }

            base.BeforeSubmitChanges();

        }

        public System.Linq.IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance)
        {
            throw new System.NotImplementedException();
        }
    }
}
