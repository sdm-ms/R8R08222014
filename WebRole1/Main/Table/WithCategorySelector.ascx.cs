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

using System.Text;

using ClassLibrary1.Model;
using ClassLibrary1.Misc;

public partial class Main_Table_WithCategorySelector : System.Web.UI.UserControl
{
    protected Func<int?, TableSortRule, bool, IQueryable<TblRow>> GetFilteredAndSortedQueryFn { get; set; }
    protected Func<bool, bool, FilterRules> GetFilterRulesFn { get; set; }
    protected int TblID { get; set; }
    protected RaterooDataAccess DataAccess { get; set; }
    public Main_Table_Table MainTable;
    public PMFieldsBox FieldsBox;
    string SuppStyle, SuppStyleHeader;

    public void SetupBeforeFieldsBox(Func<int?, TableSortRule, bool, IQueryable<TblRow>> getFilteredAndSortedQueryFn, Func<bool, bool, FilterRules> getFilterRulesFn, int theTblID, RaterooDataAccess dataAccess, string suppStyle, string suppStyleHeader)
    {
        GetFilteredAndSortedQueryFn = getFilteredAndSortedQueryFn;
        GetFilterRulesFn = getFilterRulesFn;
        TblID = theTblID;
        DataAccess = dataAccess;
        FillTblTabMenu();
        SuppStyle = suppStyle;
        SuppStyleHeader = suppStyleHeader;
    }

    public void SetupAfterFieldsBox(PMFieldsBox theFieldsBox)
    {
        FieldsBox = theFieldsBox;
        if (CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            LiteralControl myLiteral = new LiteralControl("<div class=\"outerDivAroundMainTable\"><div id=\"divAroundMainTable\" class=\"divAroundMainTable possibleBottom\"><div id=\"areaAroundTableHeader\"><table id=\"headert\" class=\"mainTable mainTablePositioning \"></table></div><div id=\"mainTableScrollArea\" class=\"mainTableScrollable\"><table id=\"maint\" class=\"mainTable " + SuppStyle + " " + SuppStyleHeader + " mainTablePositioning " + "\"></table></div></div><table><tr id=\"asteriskRow\" class=\"asteriskRowMain\"><td colspan=\"99\" class=\"asteriskRow\"><span></span></td></tr></table></div>");
            MainTablePlaceholder.Controls.Add(myLiteral); 
            int? TblTabID = GetTblTabID();
            if (TblTabID != null)
                InsertTableInfo((int) TblTabID);
        }
        else
            SetupMainTable();
    }

    protected void SetupMainTable()
    {
        //// The follow is a hack that makes it so that we'll redraw the box
        //// only if the postback came from within the listview or the btnfilter.
        //// There should be a better way of doing this, but the problem is that
        //// the controls are not yet loaded at this stage, which makes it impossible
        //// to locate the AsyncPostBackSourceElementID control on the page, and there
        //// is no working property on the UpdatePanel that will return this.
        //if (Page.IsPostBack)
        //{
        //    ScriptManager sm = ScriptManager.GetCurrent(Page);
        //    if (!(
        //        sm.AsyncPostBackSourceElementID.Contains("MainTableWithCategorySelector")
        //        || sm.AsyncPostBackSourceElementID.Contains("BtnFilter")))
        //        return;
        //}

        int? TblTabID = GetTblTabID();
        if (TblTabID == null)
        {
            MainTable = null;
        }
        else
        {
            MainTable = (Main_Table_Table)LoadControl("~/Main/Table/Table.ascx");
            MainTable.Setup(GetFilteredAndSortedQueryFn, GetFilterRulesFn, TblID, (int)TblTabID, SuppStyle, SuppStyleHeader);
            MainTablePlaceholder.Controls.Add(MainTable);
        }
    }

   
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// FillTblTabMenu
    /// Fills the menu.
    /// Hides the menu if there are 0 or 1 category group only.
    /// </summary>
    /// <returns></returns>
    protected void FillTblTabMenu()
    {        
        //Fetching category Group and binding it to drop down list
        var GetTblTab = DataAccess.RaterooDB.GetTable<TblTab>()
                               .Where(x => x.TblID == TblID && x.Status == Convert.ToByte(StatusOfObject.Active))
                               .OrderBy(x => x.NumInTbl)
                               .ThenBy(x => x.TblTabID)
                               .Select(x => new { Name = x.Name, TblTabID = x.TblTabID}).ToList();

        int categoryCount = GetTblTab.Count();

        if (categoryCount >= 1)
        {
            if (!Page.IsPostBack)
            {
                DdlCategory.ClearSelection();
                DdlCategory.DataSource = GetTblTab;
                DdlCategory.DataTextField = "Name";
                DdlCategory.DataValueField = "TblTabID";
                DdlCategory.DataBind();
            }
        }

        TableSelector.Visible = categoryCount > 1;
    }

    public int GetTblTabID()
    {
        var theString = DdlCategory.SelectedValue;
        if (theString == "")
            throw new Exception("Internal error: Category group not specified.");
        else
            return Convert.ToInt32(theString);
    }

    public void UpdateMainTable(bool resetTableToTop, bool resetFields, bool resetCategories, bool resetSort, bool reloadFields)
    {
        int TblTabID = GetTblTabID();
        if (MainTable != null)
        {
            MainTable.ReBind(TblTabID, resetTableToTop, resetSort, reloadFields);
        }
        UpdatePanelAroundMainTable.Update();
        if (FieldsBox != null)
            FieldsBox.ReBind(resetFields, resetCategories, TblTabID);
    }


    protected void DdlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateMainTable(false, false, true, true, false);
    }

    protected void InsertTableInfo(int TblTabID)
    {
        if (CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            int? TblColumnToSort = null;
            bool SortOrderAscending = true;
            DataAccess.GetDefaultSortForTblTab(TblTabID, ref TblColumnToSort, ref SortOrderAscending);
            TableInfo theInfo = new TableInfo();
            theInfo.TblID = TblID;
            theInfo.TblTabID = TblTabID;
            // theInfo.IPAddress = Request.UserHostAddress;

            bool userIsTrusted = false;
            IUserProfileInfo currentUser = ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser();
            int? userID = currentUser == null ? null : (int?) currentUser.GetProperty("UserID");
            if (userID != null && userID != 0) // logged in user ==> probably a rater
            {
                TblTab theTblTab;
                Tbl theTbl;
                PointsManager thePointsManager;
                PMTableLoading.GetTblAndPointsManagerForTblTab(DataAccess, TblTabID, out theTblTab, out theTbl, out thePointsManager);
                userIsTrusted = DataAccess.UserIsTrustedAtLeastSomewhatToEnterRatings(thePointsManager.PointsManagerID, (int) userID);
                if (userIsTrusted)
                    theInfo.SortInstruction = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule( new TableSortRuleNeedsRating());
                else
                    theInfo.SortInstruction = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleNeedsRatingUntrustedUser());
            }
            else if (TblColumnToSort == null)
                // Note: The client may change this to load by distance on initially loading the page.
                theInfo.SortInstruction = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleEntityName(true));
            else
                theInfo.SortInstruction = TableSortRuleGenerator.GetStringRepresentationFromTableSortRule(new TableSortRuleTblColumn((int)TblColumnToSort, SortOrderAscending));
            theInfo.SortMenu = SortMenuGenerator.GetSortMenuForTblTab(DataAccess.RaterooDB, TblTabID, userIsTrusted);
            theInfo.SuppStyle = SuppStyle;
            theInfo.Filters = GetFilterRulesFn(false, false);

            // Serialization
            string serialized = TableInfoToStringConversion.GetStringFromTableInfo(theInfo);

            StringBuilder theString = new StringBuilder();            
            theString.Append("<div id=\"tableInfo\" style=\"display:none;\">");
            theString.Append(serialized.ToString());
            theString.Append("</div>");
            tInfoLiteral.Text = theString.ToString();
        }
    }
}
