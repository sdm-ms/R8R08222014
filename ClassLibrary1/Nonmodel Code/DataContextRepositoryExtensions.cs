using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Diagnostics;

namespace ClassLibrary1.Misc
{
    public interface IDataContextExtended : IDataContext
    {
        List<object> RegisteredToBeInserted { get; set; }
        /// <summary>
        /// The idea of the temporary cache is to cache information only for the (short) life of a DataContext. 
        /// I think I used this sparingly, but there were cases where it was much easier than trying to pass the 
        /// information around. We could encapsulate it as a general functionality that can be used in any data 
        /// context. But it still needs ultimately to be stored in the DataContext.
        /// </summary>
        Dictionary<string, object> TempCache { get; set; }
    }

    public static class DataContextRepositoryExtensions
    {

        public static object TempCacheGet(this IDataContextExtended theDataContext, string key)
        {
            if (theDataContext.TempCache.ContainsKey(key))
                return theDataContext.TempCache[key];
            return null;
        }

        public static void TempCacheAdd(this IDataContextExtended theDataContext, string key, object theObject)
        {
            if (theDataContext.TempCache.ContainsKey(key))
                theDataContext.TempCache[key] = theObject;
            else
                theDataContext.TempCache.Add(key, theObject);
        }

        public static void RegisterObjectToBeInserted(this IDataContextExtended theDataContext, object theObject)
        {
            if (theObject != null)
            {
                theDataContext.RegisteredToBeInserted.Add(theObject);
                WeakReferenceTracker.AddWeakReferenceTo(theObject);
            }
        }

        internal static void InsertOnSubmitObjectsToBeInserted<T>(this IDataContextExtended theDataContext) where T : class
        {
            var theDBTable = theDataContext.GetTable<T>();
            foreach (var theObject in theDataContext.RegisteredToBeInserted.OfType<T>())
                theDBTable.InsertOnSubmit(theObject);
                //theDBTable.InsertOnSubmitIfNotAlreadyInserted(theObject);
        }

        /// <summary>
        /// Returns objects that either have been registered to be inserted or are in the database.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="theDataContext"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereFromNewOrDatabase<TSource>(this IDataContextExtended theDataContext, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            IQueryable<TSource> newObjects = theDataContext.RegisteredToBeInserted.OfType<TSource>().AsQueryable().Where<TSource>(predicate);
            if (newObjects.Any())
                return newObjects;
            var theTable = theDataContext.GetTable<TSource>();
            return theTable.Where<TSource>(predicate);
        }

        public static IQueryable<TSource> WhereFromNewAndDatabase<TSource>(this IDataContextExtended theDataContext, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            List<TSource> newObjects = theDataContext.RegisteredToBeInserted.OfType<TSource>().AsQueryable().Where<TSource>(predicate).ToList();
            var theTable = theDataContext.GetTable<TSource>();
            List<TSource> databaseObjects = theTable.Where<TSource>(predicate).ToList();
            IQueryable<TSource> combined = newObjects.Union(databaseObjects).AsQueryable();
            return combined;
        }

        public static TSource NewOrFirst<TSource>(this IDataContextExtended theDataContext) where TSource : class
        {
            var thePredicate = PredicateBuilder.True<TSource>();
            return NewOrFirst(theDataContext, thePredicate);
        }

        public static TSource NewOrFirst<TSource>(this IDataContextExtended theDataContext, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            TSource newObject = theDataContext.RegisteredToBeInserted.OfType<TSource>().AsQueryable().FirstOrDefault(predicate);
            if (newObject != null)
                return newObject;
            var theTable = theDataContext.GetTable<TSource>();
            return theTable.First<TSource>(predicate);
        }

        public static TSource NewOrFirstOrDefault<TSource>(this IDataContextExtended theDataContext) where TSource : class
        {
            var thePredicate = PredicateBuilder.True<TSource>();
            return NewOrFirstOrDefault(theDataContext, thePredicate);
        }

        public static TSource NewOrFirstOrDefault<TSource>(this IDataContextExtended theDataContext, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            TSource newObject = theDataContext.RegisteredToBeInserted.OfType<TSource>().AsQueryable().FirstOrDefault(predicate);
            if (newObject != null)
                return newObject;
            var theTable = theDataContext.GetTable<TSource>();
            return theTable.FirstOrDefault<TSource>(predicate);
        }

        public static TSource NewOrSingle<TSource>(this IDataContextExtended theDataContext) where TSource : class
        {
            var thePredicate = PredicateBuilder.True<TSource>();
            return NewOrSingle(theDataContext, thePredicate);
        }

        public static TSource NewOrSingle<TSource>(this IDataContextExtended theDataContext, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            TSource newObject = theDataContext.RegisteredToBeInserted.OfType<TSource>().AsQueryable().SingleOrDefault(predicate);
            if (newObject != null)
                return newObject;
            var theTable = theDataContext.GetTable<TSource>();
            return theTable.Single<TSource>(predicate);
        }

        public static TSource NewOrSingleOrDefault<TSource>(this IDataContextExtended theDataContext) where TSource : class
        {
            var thePredicate = PredicateBuilder.True<TSource>();
            return NewOrSingleOrDefault(theDataContext, thePredicate);
        }

        public static TSource NewOrSingleOrDefault<TSource>(this IDataContextExtended theDataContext, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            TSource newObject = theDataContext.RegisteredToBeInserted.OfType<TSource>().AsQueryable().SingleOrDefault<TSource>(predicate);
            if (newObject != null)
                return newObject;
            var theTable = theDataContext.GetTable<TSource>();
            return theTable.SingleOrDefault<TSource>(predicate);
        }

    }
}
