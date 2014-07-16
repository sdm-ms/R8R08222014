using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

using ClassLibrary1.EFModel;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for CssAccess
    /// </summary>
    public class TblDimensionAccess
    {
        protected R8RDataAccess DataAccess;

        public TblDimensionAccess(R8RDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        public Guid GetTblDimensionsIDForRegularTbl(Guid theTblID)
        {
            Guid? x = DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == theTblID).TblDimensionsID;
            if (x == null)
                x = DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == theTblID).PointsManager.Domain.TblDimensionsID;
            if (x == null)
                x = DataAccess.R8RDB.GetTable<TblDimension>().FirstOrDefault().TblDimensionsID;
            return (Guid)x;
        }

        public TblDimension GetTblDimensionsForRegularTbl(Guid theTblID)
        {
            TblDimension theTblDimension = CacheManagement.GetItemFromCache("TableDim" + theTblID) as TblDimension;
            if (theTblDimension != null)
                return theTblDimension;

            theTblDimension = DataAccess.R8RDB.GetTable<TblDimension>().Single(t => t.TblDimensionsID == GetTblDimensionsIDForRegularTbl(theTblID));
            string[] theDependency = { }; // no cache dependency needed since this is very short lived and not operation-critical.
            CacheManagement.AddItemToCache("TableDim" + theTblID, theDependency, theTblDimension, new TimeSpan(0, 5, 0));
            return theTblDimension;
        }
    }
}