using System.Text;
using ClassLibrary1.Misc;
using System.Data.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.EFModel;
using System.Collections.Generic;

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
            base.BeforeSubmitChanges();
        }

        public System.Linq.IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance)
        {
            throw new System.NotImplementedException();
        }
    }
}
