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
using System.Globalization;

public partial class TblRow_Date : System.Web.UI.UserControl
{
    public DateTime? theDate 
    {
        get
        {
            if (TxtDate.Text == "")
                return null;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime result;
            bool success = DateTime.TryParse(TxtDate.Text, culture, styles, out result);
            if (success)
                return result;
            return null;
        }
        set
        {
            if (value == null)
                TxtDate.Text = "";
            else
            {
                DateTime theDateToSet = (DateTime)value;
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                TxtDate.Text = theDateToSet.ToString("d", culture);
            }
        }
    }

    public bool ValidatesOK(bool emptyOK, DateTime? minDate, DateTime? maxDate, ref string errorMessage)
    {
        string dateText = TxtDate.Text.Trim();
        if (!emptyOK && dateText == "")
        {
            errorMessage = "Please enter a date.";
            return false;
        }
        if (dateText != "" && theDate == null)
        {
            errorMessage = "Please enter a date in MM/DD/YYYY format.";
            return false;
        }
        if (minDate != null && theDate != null && ((DateTime)theDate < (DateTime)minDate))
        {
            errorMessage = "The earliest date is " + ((DateTime)minDate).ToString("d") + ".";
            return false;
        }
        if (maxDate != null && theDate != null && ((DateTime)theDate > (DateTime)maxDate))
        {
            errorMessage = "The latest date is " + ((DateTime)maxDate).ToString("d") + ".";
            return false;
        }
        return true;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
  
   
    
}
