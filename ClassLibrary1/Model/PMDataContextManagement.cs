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
using System.Threading;
using System.Diagnostics;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

using System.Reflection;
using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.Model;

namespace ClassLibrary1.Model
{

    /// <summary>
    /// Summary description for DataContextManagement
    /// </summary>
    public class DataContextManagement
    {

        [ThreadStaticAttribute]
        static IRaterooDataContext _ThreadDataContext;

        internal void ResetMyDataContext(string key)
        {
            //Trace.TraceInformation("ResetDataContext " + key);
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[key] != null)
                {
                    IRaterooDataContext iDataContext = ((IRaterooDataContext)HttpContext.Current.Items[key]);
                    iDataContext.Reset();
                    HttpContext.Current.Items[key] = null;
                }
            }
            else
            {
                if (_ThreadDataContext != null)
                {
                    IRaterooDataContext iDataContext = ((IRaterooDataContext)_ThreadDataContext);
                    iDataContext.Reset();
                }
                _ThreadDataContext = null;
                //LocalDataStoreSlot threadData = Thread.GetNamedDataSlot(key);
                //if (threadData != null)
                //{
                //    ((IRaterooDataContext)Thread.GetData(threadData)).Dispose();
                //    Thread.FreeNamedDataSlot(key);
                //}
            }
            // Trace.TraceInformation("DataContext reset.");
        }

        public void ResetMyDataContext()
        {
            string key;
            if (HttpContext.Current != null)
            {
                key = "__WRSCDC_" + HttpContext.Current.GetHashCode().ToString("x") + Thread.CurrentContext.ContextID.ToString();
                ResetMyDataContext(key);
            }
            else
            {
                key = "__WRSCDC_" + Thread.CurrentContext.ContextID.ToString();
                ResetMyDataContext(key);
            }
        }

        internal IRaterooDataContext GetDataContext(string key, bool doAllowChangeData, bool enableObjectTracking)
        {
            IRaterooDataContext dataContext = null;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[key] != null)
                {
                    dataContext = (IRaterooDataContext)HttpContext.Current.Items[key];
                    if (dataContext != null && dataContext.IsRealDatabase())
                    {
                        RaterooDataContext realDatabase = dataContext.GetRealDatabaseIfExists();
                        if ( realDatabase != null && (((RaterooSQLDataContext)dataContext).AllowChangeData == false && doAllowChangeData == true)
                            || (realDatabase.ObjectTrackingEnabled != enableObjectTracking))
                        { // We only reset the data context if we want to write data and we can't do so.
                            ResetMyDataContext(key);
                            return GetDataContext(key, doAllowChangeData, enableObjectTracking);
                        }
                    }
                }
                else
                {
                    dataContext = GetIRaterooDataContext.New(doAllowChangeData, enableObjectTracking);
                    if (dataContext != null)
                        HttpContext.Current.Items[key] = dataContext;
                }
            }
            else
            {
                if (_ThreadDataContext == null)
                    _ThreadDataContext = GetIRaterooDataContext.New(doAllowChangeData, enableObjectTracking);
                dataContext = _ThreadDataContext;
                //LocalDataStoreSlot threadData = Thread.GetNamedDataSlot(key);
                //if (threadData != null)
                //{
                //    myDataContext = (IRaterooDataContext)Thread.GetData(threadData);
                //    if (myDataContext.AllowChangeData != doAllowChangeData || myDataContext.ObjectTrackingEnabled != enableObjectTracking)
                //    {
                //        ResetMyDataContext(key);
                //        return GetMyDataContext(key, doAllowChangeData, enableObjectTracking);
                //    }
                //}
                //else
                //{
                //    myDataContext = GetIRaterooDataContext.New(doAllowChangeData,enableObjectTracking);
                //    if (myDataContext != null)
                //    {
                //        threadData = Thread.AllocateNamedDataSlot(key);
                //        Thread.SetData(threadData, myDataContext);
                //    }
                //}
            }

            if (dataContext == null)
                throw new Exception("Internal error: Could not allocate datacontext.");

            return dataContext;
        }


        public IRaterooDataContext GetDataContext(bool doAllowChangeData, bool enableObjectTracking)
        {
            string key;
            if (HttpContext.Current != null)
            {
                key = "__WRSCDC_" + HttpContext.Current.GetHashCode().ToString("x") + Thread.CurrentContext.ContextID.ToString();
            }
            else
            {
                key = "__WRSCDC_" + Thread.CurrentContext.ContextID.ToString();
            }
            //Trace.TraceInformation("GetDataContext " + key);
            return GetDataContext(key, doAllowChangeData, enableObjectTracking);
        }
    }

}