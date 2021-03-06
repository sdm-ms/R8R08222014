﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!(HttpContext.Current.Profile != null && (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != null))
            Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
    }
}
