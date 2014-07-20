using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Diagnostics;
using Subgurim.Controles;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;



public partial class Main_Table_TableCellView : System.Web.UI.UserControl
{

    protected Guid RatingGroupID;
    protected Guid TblRowID;
    protected Guid TblColumnID;
    protected Guid TblID;
    protected Guid PointsManagerID;
    protected bool CanPredict;
    protected bool CanAdminister;
    protected bool CanEditFields;
    protected bool CanResolveRatings;
    protected bool CommentsEnabled;
    protected TblDimension TheTblDimensions;
    protected R8RDataAccess DataAccess;
    ActionProcessor Obj = new ActionProcessor();

    public void Setup(Guid tblRowID, Guid tblColumnID)
    {
        DataAccess = new R8RDataAccess();
        TblRowID = tblRowID;
        TblColumnID = tblColumnID;
        TblColumn theTblColumn = DataAccess.R8RDB.GetTable<TblColumn>().Single(cd => cd.TblColumnID == TblColumnID);
        RatingGroupID = (Guid)DataAccess.GetRatingGroupForTblRowAndColumn(tblRowID, TblColumnID);
        Guid topRatingGroupID = DataAccess.R8RDB.GetTable<Rating>().First(m => m.RatingGroupID == RatingGroupID).TopmostRatingGroupID;
        Tbl theTbl = DataAccess.R8RDB.GetTable<TblRow>().Single(x => x.TblRowID == TblRowID).Tbl;
        TblID = theTbl.TblID;
        PointsManagerID = theTbl.PointsManagerID;
        DetermineUserRights();

        TblDimensionAccess theCssAccess = new TblDimensionAccess(DataAccess);
        TheTblDimensions = theCssAccess.GetTblDimensionsForRegularTbl(TblID);

        RecentRatingsTable.PointsManagerID = PointsManagerID; 
        RecentRatingsTable.TopRatingGroupID = topRatingGroupID;

        FieldDisplayHtml mainFieldDisplayHtml = new FieldDisplayHtml();
        Main_Table_FieldsDisplay theMainFieldsDisplay = (Main_Table_FieldsDisplay)LoadControl("~/Main/Table/FieldsDisplay.ascx");
        mainFieldDisplayHtml = theMainFieldsDisplay.Setup(DataAccess.R8RDB, TheTblDimensions, FieldsLocation.TblRowPage, TblRowID, true);
        FieldsDisplayPlaceHolder.Controls.Add(theMainFieldsDisplay);

        RatingOverTimeGraph theRatingOverTimeGraph = (RatingOverTimeGraph)LoadControl("~/CommonControl/RatingOverTimeGraph.ascx");
        theRatingOverTimeGraph.Manual_Setup(RatingGroupID, null);
        ChartPlaceHolder.Controls.Add(theRatingOverTimeGraph);

        Main_Table_BodyRow MainTableBodyRow = (Main_Table_BodyRow)LoadControl("~/Main/Table/BodyRow.ascx");
        MainTableBodyRow.Setup(DataAccess, TblID, theTblColumn.TblTabID, TblColumnID, null, TblRowID, 1, CanPredict, CanAdminister, false, SelectionChanged, "");
        BodyRowPlaceHolder.Controls.Add(MainTableBodyRow);

        Main_Table_HeaderRowOnTblRowPage MainTableHeaderRow = (Main_Table_HeaderRowOnTblRowPage)LoadControl("~/Main/Table/HeaderRowOnTblRowPage.ascx");
        MainTableHeaderRow.Setup(DataAccess, TblRowID, theTblColumn.TblTabID, TblColumnID);
        HeaderRowPlaceHolder.Controls.Add(MainTableHeaderRow);

        Main_Table_RatingGroupResolution RatingGroupResolution = (Main_Table_RatingGroupResolution)LoadControl("~/Main/Table/RatingResolution.ascx");
        RatingGroupResolution.RatingGroupID = RatingGroupID;
        RatingGroupResolution.UserID = (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        RatingGroupResolution.CanResolve = CanResolveRatings;
        ResolveRatingsPlaceHolder.Controls.Add(RatingGroupResolution);
    
    }

    protected void DetermineUserRights()
    {

        CanPredict = false;
        CanAdminister = false;
        CanEditFields = false;
        CanResolveRatings = false;
        if ((int) ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != 0)
        {
            Guid UserId = (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            // Checking user rights to predict
            CanPredict = DataAccess.CheckUserRights(UserId, UserActionType.Predict, false, PointsManagerID, TblID);
            CanAdminister = DataAccess.CheckUserRights(UserId, UserActionType.ResolveRatings, false, PointsManagerID, TblID);
            CanEditFields = DataAccess.CheckUserRights(UserId, UserActionType.ChangeTblRows, false, PointsManagerID, TblID);
            CanResolveRatings = DataAccess.CheckUserRights(UserId, UserActionType.ResolveRatings, false, PointsManagerID, TblID);
        }

        CommentsEnabled = false; // TO DO: Add Comments capability

    }

    public void SelectionChanged(int? newRow, int? newColumn)
    { // Nothing to do.
    }

}
