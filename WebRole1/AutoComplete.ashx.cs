using System;
using System.Web;
using System.Linq;

using System.Text;
using System.Collections.Generic;
using ClassLibrary1.Model;

namespace WebRole1
{

    public class AutoComplete : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
            string thePhrase = HttpUtility.HtmlDecode(context.Request.QueryString["q"] as string);
            List<AutoCompleteData> theData = RaterooDataManipulation.GetAutoCompleteData(theDataAccessModule.DataContext, thePhrase).Take(15).ToList();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var theOutput = serializer.Serialize(theData);
            context.Response.Write(theOutput);
            //List<string> theSearchWords = theDataAccessModule.GetItemPathStringsFollowedByPathToItemsForPhrase(thePhrase, context.Request.Url.Host).Take(15).ToList();
            //StringBuilder SB = new StringBuilder();
            //for (int i = 0; i < theSearchWords.Count; i++)
            //{
            //    SB.Append(theSearchWords[i]);
            //    if (i != theSearchWords.Count - 1)
            //        SB.Append("\n");
            //}
            //context.Response.Write(SB.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return true; // This should be thread safe.
            }
        }

    }
}