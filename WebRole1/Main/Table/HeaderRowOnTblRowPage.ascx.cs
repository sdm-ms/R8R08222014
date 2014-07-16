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
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

////using PredRatings;

public partial class Main_Table_HeaderRowOnTblRowPage : System.Web.UI.UserControl
{
    protected Guid TblTabID { get; set; }
    protected Guid? LimitToThisTblColumnID { get; set; }
    protected Guid TblRowID { get; set; }
    protected R8RDataAccess DataAccess { get; set; }
    protected bool rebinding = false;

    public void Setup(R8RDataAccess dataAccess, Guid entityID, Guid theTblTabID, Guid? limitToThisTblColumnID)
    {

        DataAccess = dataAccess;
        TblRowID = entityID;
        TblTabID = theTblTabID;
        LimitToThisTblColumnID = limitToThisTblColumnID;
        if (!Page.IsPostBack)
            HeaderLinqDataSource.Selecting += new EventHandler<LinqDataSourceSelectEventArgs>(HeaderLinqDataSource_Selecting);
    }

    class HeaderRowOnTblRowPageInfoType
    {
        public Guid? TblColumnID { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string WidthStyle { get; set; }
        public bool VerticalText { get; set; }
    }

    protected void HeaderLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        bool useVerticalColumns = NumberandTableFormatter.UseVerticalColumns(DataAccess, TblTabID, LimitToThisTblColumnID,true); 
        var theQuery = DataAccess.R8RDB.GetTable<TblColumn>()
                 .Where(x => x.TblTabID == TblTabID 
                     && (LimitToThisTblColumnID == null || LimitToThisTblColumnID == x.TblColumnID)
                     && x.Status == (byte)StatusOfObject.Active)
                 .Select(x => new HeaderRowOnTblRowPageInfoType
                 {
                     TblColumnID = x.TblColumnID,
                     Abbreviation = x.Abbreviation,
                     Name = x.Name,
                     WidthStyle = x.WidthStyle,
                     VerticalText = useVerticalColumns
                 });

        e.Result = theQuery;

    }

    public void SetupChild(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            Guid? TblColumnID = (Guid?)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["TblColumnID"];
            if (TblColumnID == new Guid()) // DEBUG
                TblColumnID = null;
            string theAbbreviation = (string)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["Abbreviation"];
            string theName = (string)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["Name"];
            string theWidthStyle = (string)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["WidthStyle"]; 
            bool verticalText = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["VerticalText"];

            Main_Table_ViewCellColumnHeading theColumnHeading = (Main_Table_ViewCellColumnHeading)e.Item.FindControl("Main_Table_ViewCellColumnHeading");
            theColumnHeading.Setup(DataAccess, null, TblColumnID, TblRowID, theAbbreviation, theName, theWidthStyle, false, false, false, LimitToThisTblColumnID != null, verticalText);

        }
    }

    protected void HeaderListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding && !CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            SetupChild(e);
        }

    }

    protected void HeaderListView_ItemDataBound(object sender, ListViewItemEventArgs e)
    {

        if (!Page.IsPostBack || rebinding)
        {
            SetupChild(e);
        }


    }

    public void ReBind(int? TblColumnToSortID, bool doSortOrderAsc)
    {
        rebinding = true;
        HeaderListView.DataBind();
    }
}
