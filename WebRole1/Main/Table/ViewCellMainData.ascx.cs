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
////using PredRatings;
using System.Collections.Generic;
using System.IO;
using ClassLibrary1.Model;



public partial class Main_Table_ViewCellMainData : System.Web.UI.UserControl
{
    bool MultipleOutcomes;
    Main_Table_ViewCellMultipleOutcome theMultipleOutcomeCellData;
    Main_Table_ViewCellSingleOutcome theSingleOutcomeCellData;

    Action<int> SelectFn;
    int ColumnNumber;
    int TblColumnID;

    public void Setup(R8RDataAccess dataAccess, Action<int> selectFn, int columnNumber, int tblColumnID, int? ratingGroupID, bool multipleOutcomes, TradingStatus theTradingStatus, bool canPredict, bool selected, bool doRebind, string suppStyle)
    {
        SelectFn = selectFn;
        ColumnNumber = columnNumber;
        TblColumnID = tblColumnID;
        MultipleOutcomes = multipleOutcomes;

        if (ratingGroupID == null)
        {
            Main_Table_ViewCellNoRatings theCellData = (Main_Table_ViewCellNoRatings)LoadControl("~/Main/Table/ViewCellNoRatings.ascx");
            MainDataPlaceHolder.Controls.Add(theCellData);
        }
        else
        {
            if (multipleOutcomes)
            {
                theMultipleOutcomeCellData = (Main_Table_ViewCellMultipleOutcome)LoadControl("~/Main/Table/ViewCellMultipleOutcome.ascx");
                theMultipleOutcomeCellData.Setup(dataAccess, selectFn, columnNumber, TblColumnID, (int)ratingGroupID, theTradingStatus, canPredict, selected, doRebind, suppStyle);
                MainDataPlaceHolder.Controls.Add(theMultipleOutcomeCellData);
            }
            else
            {
                Control theSingleOutcomeControl = LoadControl("~/Main/Table/ViewCellSingleOutcome.ascx");
                theSingleOutcomeCellData = (Main_Table_ViewCellSingleOutcome)theSingleOutcomeControl;
                theSingleOutcomeCellData.Setup(dataAccess, (int)ratingGroupID, theTradingStatus, canPredict, selected, suppStyle);
                MainDataPlaceHolder.Controls.Add(theSingleOutcomeCellData);
            }
        }

    }

    public List<RatingIdAndUserRatingValue> GetRatingsAndUserRatings()
    {
        if (MultipleOutcomes)
            return theMultipleOutcomeCellData.GetRatingsAndUserRatings();
        else
            return theSingleOutcomeCellData.GetRatingsAndUserRatings();

    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

}
