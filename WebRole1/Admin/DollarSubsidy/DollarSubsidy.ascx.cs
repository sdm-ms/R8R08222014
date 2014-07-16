using System;
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

public partial class Admin_DollarSubsidy_DollarSubsidy : System.Web.UI.UserControl
{
    Admin_DollarSubsidy_SetDollarSubsidy dollarsubs;
    public Guid? SubtopicId = null;

    public void Setup(Guid subtopicId)
    {
        SubtopicId = subtopicId;
        dollarsubs = (Admin_DollarSubsidy_SetDollarSubsidy)LoadControl("~/Admin/DollarSubsidy/SetDollarSubsidy.ascx");
        dollarsubs.SetupDollarSubsidy(SubtopicId);
        DollsubsPopupHolder.Controls.Add(dollarsubs);
    }
    protected void LinkDollar_Click(object sender, EventArgs e)
    {
        dollarsubs.ShowUserControl();

    }
}
