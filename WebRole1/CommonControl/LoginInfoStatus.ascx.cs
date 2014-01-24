using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using ClassLibrary1.Model;




public partial class LoginInfoStatus : System.Web.UI.UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAccessInfo accessInfo = null;
        if (Request.IsAuthenticated)
        {
            LoginLink.Visible = false;
            CreateNewUserLink.Visible = false;
            loginSep2.Visible = false;
            accessInfo = RaterooDataManipulation.GetUserAccessInfoForCurrentUser();
            
        }
        else
        {
            RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
            LoginLink.NavigateUrl = PMRouting.Outgoing(new PMRoutingInfoLoginRedirect(PMRouting.OutgoingToCurrentRoute(Page.RouteData, theDataAccessModule.DataContext)));
            LogoutLink.Visible = false;
            CreateNewUserLink.Visible = true;
            loginSep2.Visible = true;
            accessInfo = new UserAccessInfo() { userName = "", passwordForWebService = "" };
        }

        TheUserAccessInfo.Text = String.Format("<div id=\"userAccessInfo\" style=\"display: none;\"><div id=\"userName\">{0}</div><div id=\"passwordForWebService\">{1}</div></div>", accessInfo.userName, accessInfo.passwordForWebService); 

    }
   
}
