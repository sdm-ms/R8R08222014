using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Misc;



namespace WebRole1.WebForms
{
    public partial class AdminResetEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null))
                Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
            R8RDataAccess myDataAccess = new R8RDataAccess();
            User theUser = myDataAccess.R8RDB.GetTable<User>().Single(x => x.UserGuid == (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"));
            if (theUser.Username != "admin")
                Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
        }

        protected void SetEmailAddress_Click(object sender, EventArgs e)
        {
            IUserProfileInfo theUser = UserProfileCollection.LoadByUsername(UserNameToReset.Text);
            if (theUser == null)
            {
                ErrorMsg.Visible = true;
            }
            else
            {
                theUser.Email = NewEmailAddress.Text;
                Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
            }
        }

        protected void UnlockUser_Click(object sender, EventArgs e)
        {
            IUserProfileInfo theUser = UserProfileCollection.LoadByUsername(UserNameToReset.Text);
            if (theUser == null)
            {
                ErrorMsg.Visible = true;
            }
            else
            {
                bool success;
                try
                {
                    success = theUser.UnlockUser();
                }
                catch
                {
                    success = false;
                }

                if (success)
                    Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
                else
                {
                    ErrorMsg.Text = "Unlock User failed.";
                    ErrorMsg.Visible = true;
                }
            }
        }
    }
}