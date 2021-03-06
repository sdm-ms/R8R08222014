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

public partial class CommonControl_RoundedCornerPageTop : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //String csname1 = "AutocompleteScript";
        //// We must (1) use ScriptManager (rather than client side call) for this to work after partial page update; and
        //// (2) use window setTimeout trick to overcome bug with dropdownlists in Firefox 3.0.
        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), csname1, "window.setTimeout(\"treeSetup()\",0);", true);

        ImageButtonAnchor.Attributes.Add("href", Routing.Outgoing(new RoutingInfo(RouteID.HomePage)));
    }
    protected void DoSearch_Click(object sender, EventArgs e)
    {
        if (SearchBox.Text != "")
        {
            Routing.Redirect(Response, new RoutingInfoSearchResults(SearchBox.Text));
        }
    }
}
