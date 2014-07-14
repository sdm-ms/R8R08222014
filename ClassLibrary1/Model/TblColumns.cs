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
        public static List<TblColumn> GetTblColumnsForTab(IR8RDataContext dataContext, int tblID, int tabID)
        {
            string cacheKey = "TblColumnsForTab" + tabID.ToString();
            List<TblColumn> theList = CacheManagement.GetItemFromCache(cacheKey) as List<TblColumn>;

            if (theList == null)
            {
                theList = dataContext.GetTable<TblColumn>().Where(x => x.TblTabID == tabID && x.Status == (int)StatusOfObject.Active).OrderBy(x => x.CategoryNum).ThenBy(x => x.TblColumnID).ToList();  // OK to order by ID to get consistent ordering
                CacheManagement.AddItemToCache(cacheKey, new string[] { "ColumnsForTblID" + tblID.ToString() }, theList, new TimeSpan(1, 0, 0));
            }

            return theList;
        }
    }

    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
    {
        //  Methods related to columns

        

        /// <summary>
        /// Returns the number of active columns in a particular table column group
        /// </summary>
        /// <param name="TblTabID">The table column group</param>
        /// <returns>The number of columns within that table column group</returns>
        public int CountTblColumnsForTblTab(int TblTabID)
        {
            return DataContext.GetTable<TblColumn>().Where(c => c.TblTabID == TblTabID && c.Status == (Byte)StatusOfObject.Active).Count();
        }

        /// <summary>
        /// Returns the number of active columns for all groups in a Tbl.
        /// </summary>
        /// <param name="TblID">The Tbl</param>
        /// <returns>The number of columns in the Tbl</returns>
        public int CountColumnsForTbl(int TblID)
        {
            var theTblTabs = DataContext.GetTable<TblTab>().Where(cg => cg.TblID == TblID && cg.Status == (Byte)StatusOfObject.Active);
            int total = 0;
            foreach (TblTab theGroup in theTblTabs)
                total += CountTblColumnsForTblTab(theGroup.TblTabID);
            return total;
        }

        /// <summary>
        /// Deletes a table column and all associated ratings.
        /// </summary>
        /// <param name="TblColumnID">The table column to delete</param>
        /// <returns></returns>
        //public void DeleteTblColumn(int TblColumnID)
        //{
           
        //    TblColumn theTblColumn = R8RDB.GetTable<TblColumn>().Single(x=>x.TblColumnID==TblColumnID);
        //    TblTab theTblTab = theTblColumn.TblTab;
        //    if (theTblTab.DefaultSortTblColumnID == TblColumnID)
        //    {
        //        theTblTab.DefaultSortTblColumnID = null;
        //    }
        //    var referringTblRows = R8RDB.GetTable<TblRow>().Where(e => e.Status == (Byte)StatusOfObject.Active && e.TblID == theTblColumn.TblTab.TblID);
        //    foreach (var referringTblRow in referringTblRows)
        //    {
        //        int? ratingGroupID = GetTopRatingGroupForTblRowAndColumn(referringTblRow.TblRowID, TblColumnID);
        //        if (ratingGroupID != null)
        //            EndRatingGroupAndSubgroupsAtCurrentValues((int)ratingGroupID);
        //    }
        //    R8RDB.GetTable<TblColumn>().DeleteOnSubmit(theTblColumn);
        //    R8RDB.SubmitChanges();
        //}

        /// <summary>
        /// Deletes a table column group (including all table columns and ratings within)
        /// </summary>
        /// <param name="TblTabID"></param>
        //public void DeleteTblTab(int TblTabID)
        //{
            
        //    TblTab theGroup = R8RDB.GetTable<TblTab>().Single(x=>x.TblTabID==TblTabID);
        //    var referringColumns = R8RDB.GetTable<TblColumn>().Where(cd => cd.Status == (Byte)StatusOfObject.Active && cd.TblTabID == TblTabID);
        //    foreach (var referringColumn in referringColumns)
        //        DeleteTblColumn(referringColumn.TblColumnID);
        //    R8RDB.GetTable<TblTab>().DeleteOnSubmit(theGroup);
        //    R8RDB.SubmitChanges();
        //}

        /// <summary>
        /// Change default sort table column of table column group
        /// </summary>
        /// <param name="TblTabID">The table column group Id to change</param>
        /// <param name="defaultSortTblColumnID">The table column id to set as default sort</param>
        public void ChangeTblTabDefaultSort(int TblTabID,int? defaultSortTblColumnID)
        {
            TblTab theGroup = DataContext.GetTable<TblTab>().Single(x => x.TblTabID == TblTabID);
            theGroup.DefaultSortTblColumnID = defaultSortTblColumnID;
            DataContext.SubmitChanges();

        }

        public void ChangeTblColumnSortOptions(int TblColumnID,bool useAsFilter,bool sortable,bool defaultSortOrderAscending)
        {
            TblColumn tblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == TblColumnID);
            tblColumn.UseAsFilter = useAsFilter;
            tblColumn.Sortable = sortable;
            tblColumn.DefaultSortOrderAscending = defaultSortOrderAscending;
            DataContext.SubmitChanges();

        }
    }
}
