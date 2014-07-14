using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1.EFModel;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity;
using System.Collections;

namespace ClassLibrary1.Misc
{

    public class EFRepository<T> : IRepository<T> where T : class
    {
        ObjectQuery<T> UnderlyingTable;

        public EFRepository(ObjectQuery<T> underlyingTable)
        {
            UnderlyingTable = underlyingTable;
        }

        public Type ElementType { get { return UnderlyingTable.AsQueryable().ElementType; } }
        public Expression Expression { get { return UnderlyingTable.AsQueryable().Expression; } }
        public IQueryProvider Provider { get { return UnderlyingTable.AsQueryable().Provider; } }

        public IEnumerator<T> GetEnumerator()
        {
            return UnderlyingTable.AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return UnderlyingTable.AsQueryable().GetEnumerator();
        }

        private void SetEntityKey<TEntity>(ObjectContext context, TEntity entity) where TEntity : EntityObject, new()
        {
            entity.EntityKey = context.CreateEntityKey(GetEntitySetName(context, entity.GetType()), entity);
        }

        private string GetEntitySetName(ObjectContext context, Type entityType)
        {
            return EntityHelper.GetEntitySetName(entityType, context);
        }

        public void InsertOnSubmit(T theObject)
        {
            UnderlyingTable.Context.AddObject(GetEntitySetName(UnderlyingTable.Context, theObject.GetType()), theObject);
        }

        public void DeleteOnSubmit(T theObject)
        {
            UnderlyingTable.Context.DeleteObject(theObject);
        }
    }


    public class EFDataContext : IDataContext
    {
        public DbContext UnderlyingDataContext { get; set; }

        public EFDataContext(DbContext underlyingDataContext)
        {
            UnderlyingDataContext = underlyingDataContext;
        }

        bool _tooLate = false;
        public bool TooLateToSetPageLoadOptions
        {
            get { return _tooLate; }
            set { _tooLate = value; }
        }

        public void Reset()
        {
            if (UnderlyingDataContext.Connection != null)
            {
                if (UnderlyingDataContext.Connection.State != System.Data.ConnectionState.Closed)
                {
                    UnderlyingDataContext.Connection.Close();
                }
            }
            UnderlyingDataContext.Dispose();
            TooLateToSetPageLoadOptions = false;
        }

        public IRepository<T> GetTable<T>() where T : class
        {
            TooLateToSetPageLoadOptions = true; // can't do it after a query
            var theTable = UnderlyingDataContext.GetTable<T>();
            return new EFRepository<T>(theTable);
        }

        public virtual void SubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            SubmitChanges();
        }

        public virtual void SubmitChanges()
        {
            BeforeSubmitChanges();
            CompleteSubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
            TooLateToSetPageLoadOptions = true;
        }

        public virtual void BeforeSubmitChanges()
        {
        }

        public virtual void CompleteSubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            UnderlyingDataContext.SubmitChanges();
        }
    }
}
