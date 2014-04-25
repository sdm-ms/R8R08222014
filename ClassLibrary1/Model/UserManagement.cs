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

using StringEnumSupport;

using System.Diagnostics;
using System.Globalization;
using System.Web.Profile;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{


        /// <summary>
        /// Summary description for RaterooSupport
        /// </summary>
        public partial class RaterooDataManipulation
        {



            public static string GetRandomPasswordUsingGUID(int length)
            {
                // Get the GUID
                string guidResult = System.Guid.NewGuid().ToString();

                // Remove the hyphens
                guidResult = guidResult.Replace("-", string.Empty);

                // Make sure length is valid
                if (length <= 0 || length > guidResult.Length)
                    throw new ArgumentException("Length must be between 1 and " + guidResult.Length);

                // Return the first length bytes
                return guidResult.Substring(0, length);
            }

            public static UserAccessInfo GetUserAccessInfoForCurrentUser()
            {
                DateTime debugTiming = TestableDateTime.Now;

                IUserProfileInfo theUser = ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser();
                if (theUser == null)
                    return new UserAccessInfo { userName = "", passwordForWebService = "" };


                if ((string)theUser.GetProperty("LastPageLoadDateTime") == null || (string)theUser.GetProperty("LastPageLoadDateTime") == "")
                {
                    theUser.SetProperty("LastPageLoadDateTime", TestableDateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), false);
                    theUser.SetProperty("PasswordForWebService", GetRandomPasswordUsingGUID(12), false);
                    //Trace.WriteLine("Setting brand new PasswordForWebService " + existing_pc["PasswordForWebService"]);
                }
                DateTime theLastPageLoadDateTime;
                try
                {
                    theLastPageLoadDateTime = DateTime.ParseExact((string)theUser.GetProperty("LastPageLoadDateTime"), "yyyy-MM-dd HH:mm tt", CultureInfo.InvariantCulture);
                }
                catch
                {
                    theLastPageLoadDateTime = TestableDateTime.Now - new TimeSpan(24, 0, 0);
                }
                if (TestableDateTime.Now - theLastPageLoadDateTime > new TimeSpan(24, 0, 0))
                {
                    //Trace.WriteLine("Time lapsed -- LastPageLoadDateTime was " + theLastPageLoadDateTime.ToLongTimeString() + " is " + TestableDateTime.Now.ToLongTimeString() + " so Setting new PasswordForWebService " + existing_pc["PasswordForWebService"]);
                    theUser.SetProperty("PasswordForWebService", GetRandomPasswordUsingGUID(12), false);
                    theUser.SetProperty("LastPageLoadDateTime", TestableDateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), false);
                }
                else
                {
                    //Trace.TraceInformation("using passwordforwebservice " + existing_pc["PasswordForWebService"]);
                }

                theUser.SavePropertyChanges();

                Trace.TraceInformation("GetUserAccessInfo time elapsed " + (TestableDateTime.Now - debugTiming).ToString());

                return new UserAccessInfo { userName = theUser.Username, passwordForWebService = (string)theUser.GetProperty("PasswordForWebService") };
            }



            /// <summary>
            /// 
            /// </summary>
            /// <param name="userid"></param>
            /// <param name="isVerified"></param>
            public void SetUserVerificationStatus(int userid, bool isVerified)
            {
                UserInfo theUserInfo = DataContext.GetTable<UserInfo>().Single(user => user.UserID == userid);
                theUserInfo.IsVerified = isVerified;
                DataContext.SubmitChanges();
            }


        }
    }