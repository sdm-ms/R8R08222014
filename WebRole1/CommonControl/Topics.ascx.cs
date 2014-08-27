using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CommonControl_Topics : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Page_Init(object sender, EventArgs e)
    {
        if (CheckJavaScriptHelper.IsJavascriptEnabled)
            TopicsCell.Attributes.Add("class", "topicsInitialDisplay");
        else
            TopicsCell.Attributes.Add("class", "topicsInitialDisplayNoJS");
        Control menu = LoadControl("~/CommonControl/Menu.ascx");
        Place1.Controls.Add(menu);

    }
}
