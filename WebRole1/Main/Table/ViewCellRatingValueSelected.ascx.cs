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
using ClassLibrary1.Model;


public partial class Main_Table_ViewCellRatingValueSelected : System.Web.UI.UserControl
{
    int RatingID;
    decimal? Value;
    int DecimalPlaces;
    decimal? MinVal;
    decimal? MaxVal;
    string Description;

    public void Setup(RaterooDataAccess dataAccess, int ratingID, decimal? value, int decimalPlaces, decimal? minVal, decimal? maxVal, string description, string suppStyle)
    {
        RatingID = ratingID;
        Value = value;
        DecimalPlaces = decimalPlaces;
        MinVal = minVal;
        MaxVal = maxVal;
        Description = description;

        TheValue.Attributes["Class"] = "numberInCellNoJS numberInCellEditableNoJS " + suppStyle;
    }

    public void GetRatingAndProposedValue(ref int ratingID, ref decimal? value)
    {
        ratingID = RatingID;
        value = GetValue();
    }

    public decimal? GetValue()
    {
        try
        {

            if (TheValue.Text == "")
                return null;
            decimal theValue = Convert.ToDecimal(TheValue.Text);
            theValue = (decimal) Math.Round((double) theValue, DecimalPlaces);
            if ((theValue >= MinVal || MinVal == null) && (theValue <= MaxVal || MaxVal == null))
                return (decimal)theValue;
            if ((MinVal != null && theValue < MinVal) || (MaxVal != null && theValue > MaxVal))
                return null;
            return (decimal) theValue;
        
        }
        catch
        {
            return null;
        }
        finally
        {
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

}
