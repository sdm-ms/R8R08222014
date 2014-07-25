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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using LinqKit;
using System.Diagnostics;

namespace ClassLibrary1.Nonmodel_Code
{

    public class EFRepository<T> : IRepository<T> where T : class
    {
        ObjectSet<T> UnderlyingTable;

        public EFRepository(ObjectSet<T> underlyingTable)
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

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            // return QueryableExtensions.Include<T, TProperty>(UnderlyingTable.AsQueryable(), path);
            return UnderlyingTable.Include<T, TProperty>(path);
        }

        private void SetEntityKey<TEntity>(ObjectContext context, TEntity entity) where TEntity : EntityObject, new()
        {
            entity.EntityKey = context.CreateEntityKey(GetEntitySetName(context, entity.GetType()), entity);
        }

        string _entitySetName = null;
        private string GetEntitySetName(ObjectContext context, Type entityType)
        {
            if (_entitySetName == null)
                _entitySetName = EntityHelper.GetEntitySetName(entityType, context);
            return _entitySetName;
        }
        

        public void InsertOnSubmit(T theObject)
        {
            UnderlyingTable.AddObject(theObject);
        }

        public void DeleteOnSubmit(T theObject)
        {
            UnderlyingTable.Context.DeleteObject(theObject);
        }
    }


    public class EFDataContext : IDataContext
    {
        public DbContext UnderlyingDbContext { get; set; }

        private Dictionary<Type, object> efRepositoriesForType = new Dictionary<Type, object>();

        public EFDataContext(DbContext underlyingDataContext)
        {
            UnderlyingDbContext = underlyingDataContext;
        }

        bool _tooLate = false;
        public bool TooLateToSetPageLoadOptions
        {
            get { return _tooLate; }
            set { _tooLate = value; }
        }

        public void Reset()
        {
            //if (UnderlyingDbContext.Connection != null)
            //{
            //    if (UnderlyingDbContext.Connection.State != System.Data.ConnectionState.Closed)
            //    {
            //        UnderlyingDbContext.Connection.Close();
            //    }
            //}
            UnderlyingDbContext.Dispose();
            TooLateToSetPageLoadOptions = false;
        }

        public IRepository<T> GetTable<T>() where T : class
        {
            Debug.WriteLine("DEBUG " + typeof(T) + " " + System.Environment.StackTrace);
            TooLateToSetPageLoadOptions = true; // can't do it after a query
            EFRepository<T> repo = null;
            if (!efRepositoriesForType.ContainsKey(typeof(T)))
            {
                ObjectSet<T> objectSet = null;
                ObjectContext objectContext = GetObjectContext();
                objectSet = objectContext.CreateObjectSet<T>();
                repo = new EFRepository<T>(objectSet);
                efRepositoriesForType[typeof(T)] = repo;
            }
            else
                repo = (EFRepository<T>) efRepositoriesForType[typeof(T)];
            return repo;
        }

        internal ObjectContext GetObjectContext()
        {
            return ((IObjectContextAdapter)UnderlyingDbContext).ObjectContext;
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
            try
            {
                UnderlyingDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
