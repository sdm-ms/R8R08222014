using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;

using ClassLibrary1.Model;


namespace ClassLibrary1.Model
{

    public static class TblColumnsForTabCache
    {
        public static List<TblColumn> GetTblColumnsForTab(IRaterooDataContext dataContext, int tblID, int tabID)
        {
            string cacheKey = "TblColumnsForTab" + tabID.ToString();
            List<TblColumn> theList = PMCacheManagement.GetItemFromCache(cacheKey) as List<TblColumn>;

            if (theList == null)
            {
                theList = dataContext.GetTable<TblColumn>().Where(x => x.TblTabID == tabID && x.Status == (int)StatusOfObject.Active).OrderBy(x => x.CategoryNum).ThenBy(x => x.TblColumnID).ToList();
                PMCacheManagement.AddItemToCache(cacheKey, new string[] { "CategoriesForTblID" + tblID.ToString() }, theList, new TimeSpan(1, 0, 0));
            }

            return theList;
        }
    }

    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        //  Methods related to categories

        

        /// <summary>
        /// Returns the number of active categories in a particular category group
        /// </summary>
        /// <param name="TblTabID">The category group</param>
        /// <returns>The number of categories within that category group</returns>
        public int CountCategoriesForTblTab(int TblTabID)
        {
            return DataContext.GetTable<TblColumn>().Where(c => c.TblTabID == TblTabID && c.Status == (Byte)StatusOfObject.Active).Count();
        }

        /// <summary>
        /// Returns the number of active categories for all groups in a Tbl.
        /// </summary>
        /// <param name="TblID">The Tbl</param>
        /// <returns>The number of categories in the Tbl</returns>
        public int CountCategoriesForTbl(int TblID)
        {
            var theTblTabs = DataContext.GetTable<TblTab>().Where(cg => cg.TblID == TblID && cg.Status == (Byte)StatusOfObject.Active);
            int total = 0;
            foreach (TblTab theGroup in theTblTabs)
                total += CountCategoriesForTblTab(theGroup.TblTabID);
            return total;
        }

        /// <summary>
        /// Deletes a category descriptor and all associated ratings.
        /// </summary>
        /// <param name="TblColumnID">The category descriptor to delete</param>
        /// <returns></returns>
        //public void DeleteTblColumn(int TblColumnID)
        //{
           
        //    TblColumn theDescriptor = RaterooDB.GetTable<TblColumn>().Single(x=>x.TblColumnID==TblColumnID);
        //    TblTab theTblTab = theDescriptor.TblTab;
        //    if (theTblTab.DefaultSortTblColumnID == TblColumnID)
        //    {
        //        theTblTab.DefaultSortTblColumnID = null;
        //    }
        //    var referringTblRows = RaterooDB.GetTable<TblRow>().Where(e => e.Status == (Byte)StatusOfObject.Active && e.TblID == theDescriptor.TblTab.TblID);
        //    foreach (var referringTblRow in referringTblRows)
        //    {
        //        int? ratingGroupID = GetTopRatingGroupForTblRowCategory(referringTblRow.TblRowID, TblColumnID);
        //        if (ratingGroupID != null)
        //            EndRatingGroupAndSubgroupsAtCurrentValues((int)ratingGroupID);
        //    }
        //    RaterooDB.GetTable<TblColumn>().DeleteOnSubmit(theDescriptor);
        //    RaterooDB.SubmitChanges();
        //}

        /// <summary>
        /// Deletes a category group (including all category descriptors and ratings within)
        /// </summary>
        /// <param name="TblTabID"></param>
        //public void DeleteTblTab(int TblTabID)
        //{
            
        //    TblTab theGroup = RaterooDB.GetTable<TblTab>().Single(x=>x.TblTabID==TblTabID);
        //    var referringDescriptors = RaterooDB.GetTable<TblColumn>().Where(cd => cd.Status == (Byte)StatusOfObject.Active && cd.TblTabID == TblTabID);
        //    foreach (var referringDescriptor in referringDescriptors)
        //        DeleteTblColumn(referringDescriptor.TblColumnID);
        //    RaterooDB.GetTable<TblTab>().DeleteOnSubmit(theGroup);
        //    RaterooDB.SubmitChanges();
        //}

        /// <summary>
        /// Change default sort category descriptor of category group
        /// </summary>
        /// <param name="TblTabID">The category group Id to change</param>
        /// <param name="defaultSortTblColumnID">The category descriptor id to set as default sort</param>
        public void ChangeTblTabDefaultSort(int TblTabID,int? defaultSortTblColumnID)
        {
            TblTab theGroup = DataContext.GetTable<TblTab>().Single(x => x.TblTabID == TblTabID);
            theGroup.DefaultSortTblColumnID = defaultSortTblColumnID;
            DataContext.SubmitChanges();

        }

        public void ChangeTblColumnSortOptions(int TblColumnID,bool useAsFilter,bool sortable,bool defaultSortOrderAscending)
        {
            TblColumn theDescriptor = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == TblColumnID);
            theDescriptor.UseAsFilter = useAsFilter;
            theDescriptor.Sortable = sortable;
            theDescriptor.DefaultSortOrderAscending = defaultSortOrderAscending;
            DataContext.SubmitChanges();

        }
    }
}
