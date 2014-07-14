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
using System.Collections.Generic;


using System.Diagnostics;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class Main_Table_ViewCellMultipleOutcome : System.Web.UI.UserControl
{
    R8RDataAccess DataAccess;
    Action<int> SelectFn;
    int ColumnNumber;
    int TblColumnID;
    public bool rebinding = false;
    public int RatingGroupID;
    public bool CanPredict;
    public bool Selected;
    protected TradingStatus TheTradingStatus;
    protected List<Main_Table_ViewCellRatingValue> TheSelectedRatingValues;
    protected string SuppStyle;

    public void ActivateRebinding()
    {
        rebinding = true;
    }

    public void Setup(R8RDataAccess dataAccess, Action<int> selectFn, int columnNumber, int tblColumnID, int ratingGroupID, TradingStatus theTradingStatus, bool canPredict, bool selected, bool doRebind, string suppStyle)
    {
        SuppStyle = suppStyle;
        if (doRebind)
            ActivateRebinding();
        DataAccess = dataAccess;
        SelectFn = selectFn;
        ColumnNumber = columnNumber;
        TblColumnID = tblColumnID;
        RatingGroupID = ratingGroupID;
        TheTradingStatus = theTradingStatus;
        CanPredict = canPredict;
        Selected = selected;
        if (selected)
            TheSelectedRatingValues = new List<Main_Table_ViewCellRatingValue>();
    }

    public List<RatingIdAndUserRatingValue> GetRatingsAndUserRatings()
    {
        List<RatingIdAndUserRatingValue> theList = new List<RatingIdAndUserRatingValue>();
        foreach (var mv in TheSelectedRatingValues)
        {
            int thisRatingID = 0;
            decimal? thisUserRating = 0;
            mv.GetRatingAndProposedValue(ref thisRatingID, ref thisUserRating);
            if (thisUserRating != null)
                theList.Add(new RatingIdAndUserRatingValue
                    {
                        RatingID = thisRatingID,
                        UserRatingValue = (decimal)thisUserRating
                    }
                    );
                        
        }
        return theList;

    }

    protected void MultipleLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        RatingHierarchyData myRatingHierarchyData = DataAccess.GetRatingHierarchyDataForRatingGroup(RatingGroupID);
        e.Result = myRatingHierarchyData.RatingHierarchyEntries;
    }

    public void SetupChild(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            string ratingName = (string)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["ratingName"];
            int ratingID = (int)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["ratingID"];
            decimal? value = (decimal?)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["value"];
            int hierarchyLevel = (int)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["hierarchyLevel"];
            int decimalPlaces = (int)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["decimalPlaces"];
            decimal? minVal = (decimal?)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["minVal"];
            decimal? maxVal = (decimal?)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["maxVal"];
            string description = (string)MultipleListView.DataKeys[dataItem.DisplayIndex].Values["description"];
            
            PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("RatingValuePlaceHolder");
            Main_Table_ViewCellRatingValue theRatingValue = (Main_Table_ViewCellRatingValue)LoadControl("~/Main/Table/ViewCellRatingValue.ascx");
            theRatingValue.Setup(DataAccess,  ratingID, value, decimalPlaces, minVal, maxVal, false, description, TblColumnID, TheTradingStatus, CanPredict, Selected, SuppStyle);
            thePlaceHolder.Controls.Add(theRatingValue);
            if (Selected)
                TheSelectedRatingValues.Add(theRatingValue);

            Literal NameMkt = (Literal)e.Item.FindControl("NameMkt");
            NameMkt.Text = "<span>" + ratingName + "</span>";

            if (hierarchyLevel > 1)
            {
                Literal precedingSpace = (Literal)e.Item.FindControl("precedingSpace");
                Literal followingSpace = (Literal)e.Item.FindControl("followingSpace");
                string width = (30 * (hierarchyLevel - 1)).ToString();
                precedingSpace.Text = "<table><tr><td style=\"width:" + width + "px;\"></td><td>";
                followingSpace.Text = "</td></tr></table>";
            }
        }
    }

    protected void MultipleListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding && !CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            SetupChild(e);
        }

    }

    protected void MultipleListView_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (!Page.IsPostBack || rebinding)
        {
            SetupChild(e);
        }
    }

    public void ReBind()
    {
        rebinding = true;
        MultipleListView.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }



    protected void MultipleListView_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
    {
        SelectFn(ColumnNumber); // We've intercepted an event meant for the parent ListView. We'll have to call a function directly.
    }
}
