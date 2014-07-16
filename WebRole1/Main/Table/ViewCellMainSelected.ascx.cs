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
using System.Collections.Generic;
using System.Diagnostics;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;




public partial class Main_Table_ViewCellMainSelected : System.Web.UI.UserControl
{
    Action DeselectFn;

    public void Setup(R8RDataAccess dataAccess, Action deselectFn, Action<int> selectFn, int columnNumber, Guid TblColumnID, Guid ratingGroupID, bool isInSortedColumn, bool multipleOutcomes, TradingStatus theTradingStatus, bool canPredict, bool canAdminister, bool doRebind, string suppStyle)
    {
        string theCSSClass = "mainCellMarker";
        if (theTradingStatus == TradingStatus.Ended)
            theCSSClass += " deletedTblRow ";
        theCSSClass += " " + NumberandTableFormatter.GetSuppStyleMain(TblColumnID);
        tdTag.AddAttribute("class", theCSSClass);
        tdTag.RenderNow();
        grpC.Value = ratingGroupID.ToString();
        DeselectFn = deselectFn;
        CellData.Setup(dataAccess, selectFn, columnNumber, TblColumnID, ratingGroupID, multipleOutcomes, theTradingStatus, canPredict, true, doRebind, suppStyle);
        if (canAdminister)
        {
            //Main_Table_ViewCellAdministrativeOptions theOptions = (Main_Table_ViewCellAdministrativeOptions) LoadControl("~/Main/Table/ViewCellAdministrativeOptions.ascx");
            //AdministrativeOptions.Controls.Add(theOptions);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void BtnCancel_Click(object sender, EventArgs e)
    {
        DeselectFn();
    }
    protected void BtnEnter_Click(object sender, EventArgs e)
    {
        List<RatingIdAndUserRatingValue> theList = CellData.GetRatingsAndUserRatings();
        ActionProcessor theActionProcessor = new ActionProcessor();
        UserEditResponse theResponse = new UserEditResponse();
        User theUser = theActionProcessor.DataContext.GetTable<User>().Single(u => u.UserID == (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"));
        theActionProcessor.UserRatingsAdd(theList, theUser, ref theResponse);
        DeselectFn();
    }
}
