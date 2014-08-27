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

public partial class CommonControl_Time : System.Web.UI.UserControl
{
    public TimeSpan timeSinceMidnight
    {
        get
        {
            return new TimeSpan(Convert.ToInt32(hour.SelectedValue) + Convert.ToInt32(timeOfDay.SelectedValue), Convert.ToInt32(minute.SelectedValue), 0);
        }
        set
        {
            if (value >= new TimeSpan(24, 0, 0))
                throw new Exception("Cannot set dropdown list to a time over 24 hours.");
            else
            {
                if (value.Hours < 12)
                    timeOfDay.SelectedIndex = 0;
                else
                    timeOfDay.SelectedIndex = 1;
                hour.SelectedIndex = (value.Hours % 12);
                minute.SelectedIndex = (int) (value.Minutes / 5);
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
