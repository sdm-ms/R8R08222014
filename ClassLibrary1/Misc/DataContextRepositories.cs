using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.ComponentModel;
using System.Linq.Expressions;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Collections;

namespace ClassLibrary1.Misc
{


    public interface IDataContext
    {
        IRepository<T> GetTable<T>() where T : class;
        void SubmitChanges();
        void SubmitChanges(System.Data.Linq.ConflictMode conflictMode);
        void CompleteSubmitChanges(System.Data.Linq.ConflictMode conflictMode);
        void Reset();
        bool TooLateToSetPageLoadOptions { get; set; }
    }

    public interface IRepository<T> : IQueryable<T> where T : class
    {
        void InsertOnSubmit(T theObject);
        //void InsertOnSubmitIfNotAlreadyInserted(T theObject);
        void DeleteOnSubmit(T theObject);
    }

    public static class IRepositoryExtensions 
    {
        public static void DeleteAllOnSubmit<T>(this IRepository<T> iRepository, IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
                iRepository.DeleteOnSubmit(entity);
        }


        public static void InsertAllOnSubmit<T>(this IRepository<T> iRepository, IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
                iRepository.InsertOnSubmit(entity);
        }
    }





}
