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
using ClassLibrary1.EFModel;


public partial class Main_Table_ViewCellRatingValue : System.Web.UI.UserControl
{
    Main_Table_ViewCellRatingValueSelected theSelectedRatingValue = null;
    // Main_Table_ViewCellRatingValueDateSelected theSelectedRatingValueDate;
    // Main_Table_ViewCellRatingValueDateUnselected theSelectedRatingValueDatUselected;

    public void Setup(R8RDataAccess dataAccess, Guid ratingID, decimal? value, int decimalPlaces, decimal? minVal, decimal? maxVal, bool isDate, string description, Guid TblColumnID, TradingStatus theTradingStatus, bool canPredict, bool selected, string suppStyle)
    {
        if (selected)
        {
            if (isDate)
            {
                throw new Exception("Date not yet implemented in user interface.");
               // theSelectedRatingValueDate=(Main_Table_ViewCellRatingValueDateSelected)LoadControl("~/Main/Table/ViewCellRatingValueDateSelected.ascx");
               // theSelectedRatingValueDate.Setup(dataAccess, ratingID, value, decimalPlaces, minVal, maxVal, description);
               // RatingValuePlaceHolder.Controls.Add(theSelectedRatingValueDate);
            }
            else
            {
                theSelectedRatingValue = (Main_Table_ViewCellRatingValueSelected)LoadControl("~/Main/Table/ViewCellRatingValueSelected.ascx");
                theSelectedRatingValue.Setup(dataAccess, ratingID, value, decimalPlaces, minVal, maxVal, description, suppStyle);
                RatingValuePlaceHolder.Controls.Add(theSelectedRatingValue);
            }
        }
        else
        {
            if (isDate)
            {
                // theSelectedRatingValueDatUselected = (Main_Table_ViewCellRatingValueDateUnselected)LoadControl("~/Main/Table/ViewCellRatingValueDateUnselected.ascx");
                // theSelectedRatingValueDatUselected.Setup(dataAccess, value, decimalPlaces, minVal, maxVal, description, theTradingStatus, canPredict);
                // RatingValuePlaceHolder.Controls.Add(theSelectedRatingValueDatUselected);
            }
            else
            {
                Main_Table_ViewCellRatingValueUnselected theRatingValue = (Main_Table_ViewCellRatingValueUnselected)LoadControl("~/Main/Table/ViewCellRatingValueUnselected.ascx");
                theRatingValue.Setup(dataAccess, ratingID, value, decimalPlaces, minVal, maxVal, description, TblColumnID, theTradingStatus, canPredict, suppStyle);
                RatingValuePlaceHolder.Controls.Add(theRatingValue);
            }
        }


    }

    public void GetRatingAndProposedValue(ref Guid ratingID, ref decimal? value)
    {
        if (theSelectedRatingValue != null)
            theSelectedRatingValue.GetRatingAndProposedValue(ref ratingID, ref value);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
