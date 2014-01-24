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


public partial class _Default : System.Web.UI.Page
{
    // The following is necessary to get RenderControl to work properly for
    // controls not yet added to the web page.
    public override void VerifyRenderingInServerForm(Control control)
    {
        return;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
        PMRoutingInfo theRoutingInfo = PMRouting.Incoming(Page.RouteData, theDataAccessModule.DataContext);
        MainSlideshow.Setup(theRoutingInfo as PMRoutingInfoMainContent, theDataAccessModule.DataContext);
    }
}