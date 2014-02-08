﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.ComponentModel;
using System.Linq.Expressions;
using ClassLibrary1.Model;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Collections;

namespace ClassLibrary1.Misc
{


    public interface IDataContext
    {
        IRepository<T> GetTable<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void SubmitChanges();
        void SubmitChanges(System.Data.Linq.ConflictMode conflictMode);
        void CompleteSubmitChanges(System.Data.Linq.ConflictMode conflictMode);
        void Reset();
        bool TooLateToSetPageLoadOptions { get; set; }
    }

    public interface IRepository<T> : IQueryable<T> where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        void InsertOnSubmit(T theObject);
        void InsertOnSubmitIfNotAlreadyInserted(T theObject);
        void DeleteOnSubmit(T theObject);

        // As far as I know, this method just appeared...should we delete it?
        //UserInteraction Single(Func<UserInteraction, bool> func);
    }

    public static class IRepositoryExtensions 
    {
        public static void DeleteAllOnSubmit<T>(this IRepository<T> iRepository, IEnumerable<T> entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            foreach (var entity in entities)
                iRepository.DeleteOnSubmit(entity);
        }


        public static void InsertAllOnSubmit<T>(this IRepository<T> iRepository, IEnumerable<T> entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            foreach (var entity in entities)
                iRepository.InsertOnSubmit(entity);
        }
    }





}
