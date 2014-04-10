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

    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        DataContextManagement _dataContextManagement = null;
        RaterooDataAccess _dataAccess = null;
        

        // Misc. methods (random number generation, dealing with data contexts)

        public RaterooDataManipulation()
        {
            _dataContextManagement = new DataContextManagement();
        }

        

        // The following routines each return the same data context -- initially,
        // the plan was to have separate datacontexts for reading and writing, but
        // this proved problematic. because this file is accessed only by parts of the
        // code that have a right to make changes, we will only use the read/write datacontext.


        public IRaterooDataContext DataContext
        {
            get
            {
                return _dataContextManagement.GetDataContext(true, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RaterooDataAccess ObjDataAccess
        {
            get
            {
                if (null == _dataAccess)
                {
                    _dataAccess = new RaterooDataAccess();
                }
                return _dataAccess;
            }
        }

        /// <summary>
        ///  Sets the DataContexts to null so that they will be created from scratch. The
        ///  DataContexts are repeatedly recreated on the fly. Note that we now only use one
        ///  DataContext at a time (so the name probably should be changed).
        /// </summary>
        public void ResetDataContexts()
        {
            _dataContextManagement.ResetMyDataContext();
        }

    }
}
