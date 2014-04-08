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

using MoreStrings;
using ClassLibrary1.Model;

////using PredRatings;

public partial class Main_Table_ViewCellRatingValueUnselected : System.Web.UI.UserControl
{
    decimal? Value;
    string Description;
    bool CanPredict;
    RaterooDataAccess DataAccess;

    public void Setup(RaterooDataAccess dataAccess, int ratingID, decimal? value, int decimalPlaces, decimal? minVal, decimal? maxVal, string description, int TblColumnID, TradingStatus theTradingStatus, bool canPredict, string suppStyle)
    {
        Value = value;
        Description = description;
        DataAccess = dataAccess;
        CanPredict = canPredict;

        string theText = NumberandTableFormatter.FormatAsSpecified(value, decimalPlaces, TblColumnID);
        TblColumnFormatting theFormatting = NumberandTableFormatter.GetFormattingForTblColumn(TblColumnID);
        if (theFormatting != null)
            suppStyle = suppStyle + " " + theFormatting.SuppStylesMain;

        // Determine whether we can allow predictions to be made here.
        if (theTradingStatus != TradingStatus.Active)
        {
            Literal MyLiteral = new Literal();
            MyLiteral.Text = String.Format("<span class=\"numberInCell {0}\">{1}</span>", suppStyle, theText);
            MyPlaceHolder.Controls.Add(MyLiteral);
        }
        else
        {
            if (CheckJavaScriptHelper.IsJavascriptEnabled)
            {
                Literal MyLiteral = new Literal();
                MyLiteral.Text = String.Format("<input class=\"rtg {0}\" name=\"mkt{1}\" value=\"{2}\" readonly=\"true\">", suppStyle, ratingID.ToString(), theText);
                MyPlaceHolder.Controls.Add(MyLiteral);
            }
            else
            {
                Button BtnRatingValue = new Button();
                BtnRatingValue.CommandName = "Select";
                BtnRatingValue.CssClass = "numberInCellNoJS " + suppStyle;
                BtnRatingValue.Text = theText;
                MyPlaceHolder.Controls.Add(BtnRatingValue);
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

}
