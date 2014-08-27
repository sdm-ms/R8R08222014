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

public partial class CommonControl_DateAndTime : System.Web.UI.UserControl
{
    public DateTime? theDateTime
    {
        get
        {
            if (TheDate.theDate == null)
                return null;
            return (TheDate.theDate + TheTime.timeSinceMidnight);
        }
        set
        {
            if (value == null)
            {
                TheDate.theDate = null;
                TheTime.timeSinceMidnight = new TimeSpan(0,0,0);
                return;
            }
            TheDate.theDate = ((DateTime)value).Date;
            TheTime.timeSinceMidnight = ((DateTime)value) - ((DateTime)value).Date;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
