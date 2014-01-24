using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Linq;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ClassLibrary1.Misc
{

    public class SQLRepository<T> : IRepository<T> where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        System.Data.Linq.Table<T> UnderlyingTable;

        public SQLRepository(Table<T> underlyingTable)
        {
            UnderlyingTable = underlyingTable;
        }

        public Type ElementType { get { return UnderlyingTable.AsQueryable().ElementType; } }
        public Expression Expression { get { return UnderlyingTable.AsQueryable().Expression; } }
        public IQueryProvider Provider { get { return UnderlyingTable.AsQueryable().Provider; } }

        public void InsertOnSubmit(T theObject)
        {
            UnderlyingTable.InsertOnSubmit(theObject);
        }

        public void InsertOnSubmitIfNotAlreadyInserted(T theObject)
        {
            if (UnderlyingTable.GetOriginalEntityState(theObject) == null)
                UnderlyingTable.InsertOnSubmit(theObject);
        }

        public void DeleteOnSubmit(T theObject)
        {
            UnderlyingTable.DeleteOnSubmit(theObject);
        }


        public T GetOriginalEntityState(T theObject)
        {
            return UnderlyingTable.GetOriginalEntityState(theObject);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return UnderlyingTable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return UnderlyingTable.GetEnumerator();
        }
    }


    public class SQLDataContext : IDataContext
    {
        public DataContext UnderlyingDataContext { get; set; }

        public SQLDataContext(DataContext underlyingDataContext)
        {
            UnderlyingDataContext = underlyingDataContext;
        }

        public void Reset()
        {
            UnderlyingDataContext.Dispose();
        }

        public IRepository<T> GetTable<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            return new SQLRepository<T>(UnderlyingDataContext.GetTable<T>());
        }

        public void SubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            UnderlyingDataContext.SubmitChanges(conflictMode);
        }

        public void SubmitChanges()
        {
            UnderlyingDataContext.SubmitChanges();
        }
    }
}
