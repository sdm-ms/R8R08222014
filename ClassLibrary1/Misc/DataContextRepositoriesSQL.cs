using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Linq;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

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

        public IRepository<T> GetTable<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            TooLateToSetPageLoadOptions = true; // can't do it after a query
            var theTable = UnderlyingDataContext.GetTable<T>();
            return new SQLRepository<T>(theTable);
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
