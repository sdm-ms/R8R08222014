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
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{

    /// <summary>
    /// Summary description for DataContextManagement
    /// </summary>
    public class DataContextManagement
    {

        [ThreadStaticAttribute]
        static IR8RDataContext _ThreadDataContext;

        internal void ResetMyDataContext(string key)
        {
            //Trace.TraceInformation("ResetDataContext " + key);
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[key] != null)
                {
                    IR8RDataContext iDataContext = ((IR8RDataContext)HttpContext.Current.Items[key]);
                    iDataContext.Reset();
                    HttpContext.Current.Items[key] = null;
                }
            }
            else
            {
                if (_ThreadDataContext != null)
                {
                    IR8RDataContext iDataContext = ((IR8RDataContext)_ThreadDataContext);
                    iDataContext.Reset();
                    _ThreadDataContext = null;
                }
                //LocalDataStoreSlot threadData = Thread.GetNamedDataSlot(key);
                //if (threadData != null)
                //{
                //    ((IR8RDataContext)Thread.GetData(threadData)).Dispose();
                //    Thread.FreeNamedDataSlot(key);
                //}
            }
            // Trace.TraceInformation("DataContext reset.");
        }

        private string GetThreadKey()
        {
            return "__WRSCDC_" + (HttpContext.Current == null ? "" : HttpContext.Current.GetHashCode().ToString("x")) + Thread.CurrentContext.ContextID.ToString();
        }

        public void ResetMyDataContext()
        {
            string key = GetThreadKey();
            ResetMyDataContext(key);
        }

        internal IR8RDataContext GetDataContext(string key, bool doAllowChangeData, bool enableObjectTracking)
        {
            IR8RDataContext dataContext = null;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[key] != null)
                {
                    dataContext = (IR8RDataContext)HttpContext.Current.Items[key];
                    if (dataContext != null && dataContext.IsRealDatabase())
                    {
                        IR8RDataContext realDatabase = dataContext.GetRealDatabaseIfExists();
                        if (realDatabase != null && realDatabase is ClassLibrary1.OldModel.R8RDataContext && (((ClassLibrary1.OldModel.R8RSQLDataContext)dataContext).AllowChangeData == false && doAllowChangeData == true)
                            || (((ClassLibrary1.OldModel.R8RDataContext)realDatabase).ObjectTrackingEnabled != enableObjectTracking))
                        { // We only reset the data context if we want to write data and we can't do so.
                            ResetMyDataContext(key);
                            return GetDataContext(key, doAllowChangeData, enableObjectTracking);
                        }
                    }
                }
                else
                {
                    dataContext = GetIR8RDataContext.New(doAllowChangeData, enableObjectTracking);
                    if (dataContext != null)
                        HttpContext.Current.Items[key] = dataContext;
                }
            }
            else
            {
                if (_ThreadDataContext == null)
                {
                    _ThreadDataContext = GetIR8RDataContext.New(doAllowChangeData, enableObjectTracking);
                }
                dataContext = _ThreadDataContext;
                //LocalDataStoreSlot threadData = Thread.GetNamedDataSlot(key);
                //if (threadData != null)
                //{
                //    myDataContext = (IR8RDataContext)Thread.GetData(threadData);
                //    if (myDataContext.AllowChangeData != doAllowChangeData || myDataContext.ObjectTrackingEnabled != enableObjectTracking)
                //    {
                //        ResetMyDataContext(key);
                //        return GetMyDataContext(key, doAllowChangeData, enableObjectTracking);
                //    }
                //}
                //else
                //{
                //    myDataContext = GetIR8RDataContext.New(doAllowChangeData,enableObjectTracking);
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


        public IR8RDataContext GetDataContext(bool doAllowChangeData, bool enableObjectTracking)
        {
            string key = GetThreadKey();
            //Trace.TraceInformation("GetDataContext " + key);
            return GetDataContext(key, doAllowChangeData, enableObjectTracking);
        }
    }

}