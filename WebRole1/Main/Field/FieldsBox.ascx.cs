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


using MoreStrings;
using ClassLibrary1.Model;

public partial class FieldsBox : System.Web.UI.UserControl
{
    protected R8RDataAccess DataAccess;
    public FieldsBoxMode Mode { get; set; }
    protected bool rebinding = false;
    protected int TblID { get; set; }
    protected int? TblTabID { get; set; }
    protected TblRow TheTblRow { get; set; }
    internal SearchWordsFilter searchWordsControl;
    internal List<AnyFieldFilter> fieldsControls = new List<AnyFieldFilter>();
    internal List<FieldDefinitionInfo> fieldDefinitions;
    internal List<AnyFieldFilter> columnControls = new List<AnyFieldFilter>();
    internal List<TblColumnInfo> TblColumns;
    protected int index = 0; // keep track of number of items created/databound
    FieldSetDataInfo TheFieldSetDataInfo;
    Action<object, EventArgs> TheAction = null;
    internal bool SuppressHeading = false;
    internal bool ViewShowDeletedItems = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        TheFieldSetDataInfo = null;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ValidationErrorRow1.AddAttribute("style", "display:none;");
        HighStakesOnly.Visible = Mode != FieldsBoxMode.addTblRow && Mode != FieldsBoxMode.modifyFields;
    }

    public void SetupStandalonePage(int theTblID, int? theTblTabID)
    {
        SuppressHeading = true;
        ViewShowDeletedItems = true;
        ShowDeletedItems.Visible = true;
        BtnAction.Visible = false;
        Setup(theTblID, theTblTabID, FieldsBoxMode.filterWithButton);
    }

    public void Setup(int theTblID, int? theTblTabID, FieldsBoxMode mode)
    {
        Mode = mode;
        TblID = theTblID;
        TblTabID = theTblTabID;
        DataAccess = new R8RDataAccess();
        if (Mode == FieldsBoxMode.filterWithButton || Mode == FieldsBoxMode.filterWithoutButton)
        {
            EntityNameSelector.AddAttribute("style", "display:none;"); // We don't use the entity name selector when filtering; instead we use SearchWordsFilter
            searchWordsControl = (SearchWordsFilter)LoadControl("~/Main/Field/SearchWordsFilter.ascx");
            searchWordsControl.TblID = TblID;
            SearchWordsPlaceHolder.Controls.Add(searchWordsControl);
            if (Mode == FieldsBoxMode.filterWithButton)
            {
                if (!SuppressHeading)
                {
                    FieldsBoxHeading theHeading = (FieldsBoxHeading)LoadControl("~/Main/Field/FieldsBoxHeading.ascx");
                    HeadingPlaceHolder.Controls.Add(theHeading);
                }
                BtnAction.Text = "Narrow Results";
                BtnCancel.Visible = false;
            }
            else if (Mode == FieldsBoxMode.filterWithoutButton)
            {
                BtnCancel.Visible = false;
                BtnAction.Visible = false;
                Literal theDiv = new Literal();
                theDiv.Text = "<div>";
                HeadingPlaceHolder.Controls.Add(theDiv);
            }
        }
        else if (Mode == FieldsBoxMode.modifyFields || Mode == FieldsBoxMode.addTblRow)
        {
            string typeOfTblRow = DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == TblID).TypeOfTblRow;
            BtnCancel.Visible = true;
            BtnAction.Visible = true;
            Literal theDiv = new Literal();
            theDiv.Text = "<div>";
            HeadingPlaceHolder.Controls.Add(theDiv);
            if (Mode == FieldsBoxMode.modifyFields)
                BtnAction.Text = "Update Information";
            else
                BtnAction.Text = "Add This " + typeOfTblRow;
            EntityNameLabel.Text = typeOfTblRow;
            if (TheTblRow != null && !Page.IsPostBack)
                EntityName.Text = TheTblRow.Name;
            ViewShowDeletedItems = false;
            ShowDeletedItems.Visible = false;
            HighStakesOnly.Visible = false;
        }
        LoadFilters();
    }

    public void Setup(TblRow theTblRow, FieldsBoxMode mode, Action<object, EventArgs> theAction)
    {
        TheTblRow = theTblRow;
        Setup(TheTblRow.TblID, null, mode, theAction);
    }

    public void Setup(int theTblID, int? theTblTabID, FieldsBoxMode mode, Action<object, EventArgs> theAction)
    {
        Setup(theTblID, theTblTabID, mode);
        TheAction = theAction;
        BtnAction.Click += new EventHandler(theAction);
    }

    protected void LoadFilters()
    {
        if (Mode == FieldsBoxMode.filterWithButton || Mode == FieldsBoxMode.filterWithoutButton || Mode == FieldsBoxMode.addTblRow)
        {
            LoadTblColumns();
            LoadFieldDefinitions(Mode != FieldsBoxMode.addTblRow);
        }
        else
        {
            LoadFieldDefinitions(false);
        }
        if (!Page.IsPostBack)
        {
            // if there is no filter then making filter table invisible
            if ((Mode == FieldsBoxMode.filterWithButton || Mode== FieldsBoxMode.filterWithoutButton) && searchWordsControl == null && !fieldDefinitions.Any() && (TblColumns == null || !TblColumns.Any()))
            {
                PanelAroundFilterBox.Visible = false;
            }
        }
    }


    protected void LinqDataSourceFields_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        e.Result = fieldDefinitions;
    }

    public void SetupChildFields(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            FieldDefinitionInfo theFieldInfo = new FieldDefinitionInfo();
            theFieldInfo.FieldDefinitionID = (int)FieldsBoxListViewFields.DataKeys[dataItem.DisplayIndex].Values["FieldDefinitionID"];
            theFieldInfo.FieldName = (string)FieldsBoxListViewFields.DataKeys[dataItem.DisplayIndex].Values["FieldName"];
            theFieldInfo.FieldType = (FieldTypes)FieldsBoxListViewFields.DataKeys[dataItem.DisplayIndex].Values["FieldType"];
            theFieldInfo.FieldNum = (int)FieldsBoxListViewFields.DataKeys[dataItem.DisplayIndex].Values["FieldNum"];

            AnyFieldFilter theField = (AnyFieldFilter)e.Item.FindControl("FilterField");
            theField.Mode = Mode;
            if (TheTblRow != null)
                theField.TblRowID = TheTblRow.TblRowID;
            theField.FieldInfo = theFieldInfo;
            theField.AddSpecificFieldType(fieldsControls, Page.IsPostBack);
            fieldsControls.Add(theField);
        }
    }

    public void FieldsBoxListViewFields_DataBinding(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        { // usually, we don't databound on postback
            rebinding = true;
        }
    }

    protected void FieldsBoxListViewFields_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding)
        {
            //Trace.TraceInformation("FieldsBox itemcreated");
            SetupChildFields(e);
        }
    }

    protected void FieldsBoxListViewFields_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (!Page.IsPostBack || rebinding)
        {
            //Trace.TraceInformation("FieldsBox databound");
            SetupChildFields(e);
        }
    }

    protected void LinqDataSourceColumns_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        e.Result = (TblColumns == null) ? new List<TblColumnInfo>() : TblColumns;
    }

    public void SetupChildColumns(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            TblColumnInfo theColumnInfo = new TblColumnInfo();
            theColumnInfo.TblColumnID = (int)FieldsBoxListViewColumns.DataKeys[dataItem.DisplayIndex].Values["TblColumnID"];
            theColumnInfo.TblColumnName = (string)FieldsBoxListViewColumns.DataKeys[dataItem.DisplayIndex].Values["TblColumnName"];
            theColumnInfo.DefaultSortOrderAsc = (bool)FieldsBoxListViewColumns.DataKeys[dataItem.DisplayIndex].Values["DefaultSortOrderAsc"];
            theColumnInfo.Sortable = (bool)FieldsBoxListViewColumns.DataKeys[dataItem.DisplayIndex].Values["Sortable"];

            AnyFieldFilter theField = (AnyFieldFilter)e.Item.FindControl("FilterField");
            theField.Mode = Mode;
            theField.TblColInfo = theColumnInfo;
            theField.AddSpecificFieldType(columnControls, Page.IsPostBack);
            columnControls.Add(theField);
        }
    }

    public void FieldsBoxListViewColumns_DataBinding(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        { // usually, we don't databound on postback
            rebinding = true;
        }
    }

    protected void FieldsBoxListViewColumns_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding)
        {
            SetupChildColumns(e);
        }
    }

    protected void FieldsBoxListViewColumns_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (!Page.IsPostBack || rebinding)
        {
            //Trace.TraceInformation("Categories data binding.");
            SetupChildColumns(e);
        }
    }

    public void ReBind(bool resetFields, bool resetColumns, int? tblTabID)
    {
        rebinding = true;
        LoadFilters();
        if (resetFields)
            FieldsBoxListViewFields.DataBind();
        if (resetColumns)
        {
            TblTabID = tblTabID;
            LoadTblColumns();
            FieldsBoxListViewColumns.DataBind();
        }
        UpdatePanel2.Update();
    }

    protected void LoadFieldDefinitions(bool limitToUseAsFilters)
    {
        string cacheKey = "FieldsBoxFieldDefinitions" + TblID.ToString() + limitToUseAsFilters.ToString();
        fieldDefinitions = CacheManagement.GetItemFromCache(cacheKey) as List<FieldDefinitionInfo>;
        if (fieldDefinitions == null)
        {
            fieldDefinitions = DataAccess.R8RDB.GetTable<FieldDefinition>()
                             .Where(fd => fd.TblID == TblID && fd.Status == Convert.ToByte(StatusOfObject.Active)
                                            && (!limitToUseAsFilters || fd.UseAsFilter == true))
                             .OrderBy(fd => fd.FieldNum)
                             .ThenBy(fd => fd.FieldDefinitionID)
                            .Select(fd => new FieldDefinitionInfo
                            {
                                FieldDefinitionID = fd.FieldDefinitionID,
                                FieldName = fd.FieldName,
                                FieldType = (FieldTypes)fd.FieldType,
                                FieldNum = fd.FieldNum
                            }).ToList();
            string[] noDependency = { };
            CacheManagement.AddItemToCache(cacheKey, noDependency, fieldDefinitions, new TimeSpan(0,1,0));
        }
    }

    protected void LoadTblColumns()
    {
        string cacheKey = "FieldsBoxTblColumns" + TblID.ToString() + TblTabID.ToString() + Mode.ToString();
        TblColumns = CacheManagement.GetItemFromCache(cacheKey) as List<TblColumnInfo>;
        if (TblColumns == null)
        {
            if (Mode == FieldsBoxMode.addTblRow)
                TblColumns = DataAccess.R8RDB.GetTable<TblColumn>()
                             .Where(cd => cd.TblTab.TblID == TblID
                                 && cd.TblTab.Tbl.AllowOverrideOfRatingGroupCharacterstics)
                             .OrderBy(cd => cd.CategoryNum)
                             .ThenBy(cd => cd.TblColumnID)
                             .Select(cd => new TblColumnInfo
                             {
                                 TblColumnID = cd.TblColumnID,
                                 TblColumnName = cd.Name,
                                 DefaultSortOrderAsc = cd.DefaultSortOrderAscending,
                                 Sortable = cd.Sortable
                             }).ToList();
            else
                TblColumns = DataAccess.R8RDB.GetTable<TblColumn>()
                             .Where(cd => cd.TblTab.TblID == TblID
                                 && (cd.TblTabID == TblTabID || cd.TblTabID == null)
                                 && cd.Status == Convert.ToByte(StatusOfObject.Active)
                                 && cd.UseAsFilter == true)
                             .OrderBy(cd => cd.CategoryNum)
                             .ThenBy(cd => cd.TblColumnID)
                             .Select(cd => new TblColumnInfo
                             {
                                 TblColumnID = cd.TblColumnID,
                                 TblColumnName = cd.Name,
                                 DefaultSortOrderAsc = cd.DefaultSortOrderAscending,
                                 Sortable = cd.Sortable
                             }).ToList();
            string[] noDependency = { };
            CacheManagement.AddItemToCache(cacheKey, noDependency, TblColumns, new TimeSpan(0, 1, 0));
        }
    }

    public List<UserSelectedRatingInfo> GetUserSelectedRatingInfos()
    {
        return columnControls.Select(x => x.GetUserSelectedRatingInfo()).Where(x => x != null).ToList();
    }

    public FilterRules GetFilterRulesWithDeletedBasedOnUserChoice()
    {
        return GetFilterRules(!ShowDeletedItems.Checked, HighStakesOnly.Checked);
    }

    public FilterRules GetFilterRules(bool activeOnly, bool highStakesOnly)
    {
        try
        {
            FilterRules theRules = new FilterRules(TblID, activeOnly, highStakesOnly);
            FilterRule searchWordsFilterRule = searchWordsControl.GetFilterRule();
            if (searchWordsFilterRule != null)
                theRules.AddFilterRule(searchWordsFilterRule);
            foreach (var f in fieldsControls)
            {
                try
                {
                    FilterRule anotherRule = f.GetFilterRule();
                    if (anotherRule != null)
                        theRules.AddFilterRule(anotherRule);
                }
                catch
                {
                }
            }
          
            foreach (var c in columnControls)
            {
                try
                {
                    FilterRule catRule = c.GetFilterRule();
                    if (catRule != null)
                        theRules.AddFilterRule(catRule);
                }
                catch
                {
                }
            }

            return theRules;
        }
        catch
        {
            throw new Exception("Internal error attempting to process your search results.");
        }
    }

    internal IR8RDataContext lastDataContext = null;
    public void LoadFieldSetDataInfo()
    {
        if (TheFieldSetDataInfo == null || lastDataContext != DataAccess.R8RDB)
        {
            TheFieldSetDataInfo = new FieldSetDataInfo(TheTblRow == null ? null : DataAccess.R8RDB.GetTable<TblRow>().Single(x => x.TblRowID == TheTblRow.TblRowID), DataAccess.R8RDB.GetTable<Tbl>().Single(x => x.TblID==TblID), DataAccess); // Reload, since data context may have changed. We must use current data context so that any changes are persisted to the database.
            TheFieldSetDataInfo.theRowName = MoreStringManip.StripHtml(EntityName.Text);
            foreach (var f in fieldsControls)
            {
                FieldDataInfo theFieldData = f.GetFieldValue(TheFieldSetDataInfo);
                if (theFieldData != null)
                    TheFieldSetDataInfo.AddFieldDataInfo(theFieldData);
            }
            lastDataContext = DataAccess.R8RDB;
        }
    }

    public FieldSetDataInfo GetFieldSetDataInfo()
    {
        LoadFieldSetDataInfo();
        return TheFieldSetDataInfo;
    }

    public void ReportValidationError(string errorMessage)
    {
        FieldsValidator.ErrorMessage = errorMessage;
        ValidationErrorRow1.RemoveAttribute("style");
        if (TheAction != null)
            BtnAction.Click -= new EventHandler(TheAction);
        UpdatePanel2.Update();
    }

    public void ValidateFieldsInFilterMode(object sender, ServerValidateEventArgs args)
    {
        args.IsValid = true;
        string errorMessage = "" ;
        foreach (var theControl in fieldsControls)
        {
            if (!theControl.InputDataValidatesOK(ref errorMessage))
            {
                ReportValidationError(theControl.FieldInfo.FieldName + ": " + errorMessage);
                args.IsValid = false;
                break;
            }
        }
        if (args.IsValid)
        {
            foreach (var theControl in columnControls)
            {
                if (!theControl.InputDataValidatesOK(ref errorMessage))
                {
                    ReportValidationError(theControl.TblColInfo.TblColumnName + ": " + errorMessage);
                    args.IsValid = false;
                    break;
                }
            }
        }
        if (args.IsValid)
            FieldsValidator.ErrorMessage = "";
    }

    public void ValidateFieldsInFieldsEditMode(object sender, ServerValidateEventArgs args)
    {
        args.IsValid = true;
        ValidateFieldsInFilterMode(sender, args);
        if (args.IsValid)
        {
            LoadFieldSetDataInfo();
            if (TheFieldSetDataInfo.theRowName == "")
            {
                args.IsValid = false;
                ReportValidationError("You must provide a name in the first field below.");
                return;
            }
            foreach (var fieldDataInfo in TheFieldSetDataInfo.theFieldDataInfos)
            {
                try
                {
                    fieldDataInfo.VerifyCanBeAdded();
                }
                catch (Exception ex)
                {
                    args.IsValid = false;
                    ReportValidationError(fieldDataInfo.TheFieldDefinition.FieldName + ": " + ex.Message);
                }
            }

        }
    }

    public void ValidateFields(object sender, ServerValidateEventArgs args)
    {
        if (Mode == FieldsBoxMode.addTblRow || Mode == FieldsBoxMode.modifyFields)
            ValidateFieldsInFieldsEditMode(sender, args);
        else
            ValidateFieldsInFilterMode(sender, args);
    }

    public void BtnCancel_Click(object sender, EventArgs e)
    {
        Tbl theTbl = DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == TblID);
        Routing.Redirect(Response, new RoutingInfoMainContent( theTbl, null, null));
    }

}