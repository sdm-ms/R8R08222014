using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ClassLibrary1.Model;

namespace WebApplication1
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            PMRouting.Redirect(Response, new PMRoutingInfo(PMRouteID.HomePage));
            return;
        }
    }
}
