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
using System.Diagnostics;
using ClassLibrary1.Model;



public partial class Main_Table_ViewCellColumnHeading : System.Web.UI.UserControl
{
    protected int? TblColumnID; 
    protected RaterooDataAccess DataAccess;
    protected string Abbreviation;
    protected string Name;
    protected string WidthStyle;
    protected bool SortableColumn;
    protected bool CurrentlySorting;
    protected bool DoSortOrderAscending;
    protected bool SubstituteRefreshButton;
    protected bool VerticalText;
    protected bool ChartButton;
    protected int? TblRowIDForChartButton;
    protected Action<int?, bool> SortFn;

    public void Setup(RaterooDataAccess dataAccess, Action<int?, bool> sortFn, int? tblColumnID, int? entityIDForChartButton, string abbreviation, string name, string widthStyle, bool sortableColumn, bool currentlySorting, bool doSortOrderAscending, bool substituteRefreshButton, bool verticalText)
    {
        DataAccess = dataAccess;
        SortFn = sortFn;
        TblColumnID = tblColumnID;
        Abbreviation = abbreviation;
        SortableColumn = sortableColumn;
        CurrentlySorting = currentlySorting;
        DoSortOrderAscending = doSortOrderAscending;
        SubstituteRefreshButton = substituteRefreshButton;
        Name = name;
        WidthStyle = widthStyle;
        VerticalText = verticalText;
        ChartButton = false;
        TblRowIDForChartButton = entityIDForChartButton;

        if (TblColumnID != null && entityIDForChartButton != null && TblColumnID != 0 && entityIDForChartButton != 0)
        {
            //if (substituteRefreshButton)
            //    ; // THIS FEATURE NOT CURRENTLY SUPPORTED ColumnPopUpMouseOverArea.AddAttribute("class", "refreshCell");
            //else
            {
                ChartButton = true;
                ColumnPopUpMouseOverArea.AddAttribute("class", VerticalText ? "chartcurveVertText" : "chartcurve");
            }
        }
        CompleteSetup();
    }

    protected void CompleteSetup()
    {

        string theText;
        if (Abbreviation == "")
            theText = Name;
        else
            theText = Abbreviation;

        if (SortableColumn == true)
        {
            string styleInfo = "mainTable " + PMNumberandTableFormatter.GetSuppStyleHeader(TblColumnID) + " " + WidthStyle + " ";

            if (CurrentlySorting)
                styleInfo += "sort";
            else
                styleInfo += "sortable";
            if (DoSortOrderAscending == true)
                styleInfo += "asc";
            else
                styleInfo += "desc";
            if (VerticalText)
                styleInfo += "VertText vertText"; // second one performs centering

            cellcolumnheading.Attributes["class"] = styleInfo;

            if (VerticalText)
            {
                if (CheckJavaScriptHelper.IsJavascriptEnabled)
                {
                    LiteralControl myLiteral = new LiteralControl("<img class=\"rotatedColHeader\" style=\"border-width: 0px;\" src=\"/RotatedTextHandler.ashx?TheFontSize=16&amp;TheText=" + theText + "&amp;TheAngle=90&amp;TheFontName=Trebuchet+MS&amp;TheHilite=" + (CurrentlySorting ? "1" : "0") + "\" id=\"ColImage" + TblColumnID.ToString() + "\"/>");
                    MyPlaceHolder.Controls.Add(myLiteral);
                }
                else
                {
                    AngledText theAngledText = (AngledText)LoadControl("~/CommonControl/AngledText.ascx");
                    theAngledText.TheText = theText;
                    theAngledText.TheFontSize = 16;
                    theAngledText.TheAngle = (float)90;
                    theAngledText.TheFontName = "Trebuchet MS";
                    MyPlaceHolder.Controls.Add(theAngledText);
                }
                if (CheckJavaScriptHelper.IsJavascriptEnabled)
                {
                    LiteralControl myLiteral = new LiteralControl("<a class=\"MainColumnDiv\" id=\"SortColumn" + TblColumnID.ToString() + "\" >&nbsp;</a>");
                    MyPlaceHolder.Controls.Add(myLiteral);
                }
                else
                {
                    LinkButton SortColumn = new LinkButton();
                    SortColumn.ID = "SortColumn";
                    SortColumn.Click += new EventHandler(ProcessSort);
                    SortColumn.Text = "&nbsp;";
                    MyPlaceHolder.Controls.Add(SortColumn);
                }
            }
            else
            {
                if (CheckJavaScriptHelper.IsJavascriptEnabled)
                {
                    LiteralControl myLiteral = new LiteralControl("<a class=\"MainColumnDiv\" id=\"SortColumn" + TblColumnID.ToString() + "\" >" + theText + "</a>");
                    MyPlaceHolder.Controls.Add(myLiteral);
                }
                else
                {
                    LinkButton SortColumn = new LinkButton();
                    SortColumn.ID = "SortColumn";
                    SortColumn.Click += new EventHandler(ProcessSort);
                    SortColumn.Text = theText;
                    MyPlaceHolder.Controls.Add(SortColumn);
                }
            }
        }
        else
        {
            string styleInfo = "mainTable " + PMNumberandTableFormatter.GetSuppStyleHeader(TblColumnID) + WidthStyle + " ";
            if (VerticalText)
                styleInfo += " vertText";

            cellcolumnheading.Attributes["class"] = styleInfo;

            if (VerticalText)
            {
                if (CheckJavaScriptHelper.IsJavascriptEnabled)
                {
                    LiteralControl myLiteral = new LiteralControl("<img style=\"border-width: 0px;\" src=\"/RotatedTextHandler.ashx?TheFontSize=16&amp;TheText=" + theText + "&amp;TheAngle=90&amp;TheFontName=Trebuchet+MS&amp;TheHilite=" + (CurrentlySorting ? "1" : "0") + "\" id=\"ColImage" + TblColumnID.ToString() + "\"/>");
                    MyPlaceHolder.Controls.Add(myLiteral);
                }
                else {
                    AngledText theAngledText = (AngledText)LoadControl("~/CommonControl/AngledText.ascx");
                    theAngledText.TheText = theText;
                    theAngledText.TheFontSize = 16;
                    theAngledText.TheAngle = (float)90;
                    theAngledText.TheFontName = "Trebuchet";
                    MyPlaceHolder.Controls.Add(theAngledText);
                }

            }
                                
            if (CheckJavaScriptHelper.IsJavascriptEnabled)
            {
                string href = "";
                if (ChartButton || SubstituteRefreshButton)
                {
                    int? RatingGroupID = DataAccess.GetRatingGroupForTblRowCategory((int)TblRowIDForChartButton, (int)TblColumnID);
                    if (RatingGroupID != null)
                    {
                        TblColumn theCD = DataAccess.RaterooDB.GetTable<TblColumn>().SingleOrDefault(cd => cd.TblColumnID == TblColumnID);
                        TblRow theTblRow = DataAccess.RaterooDB.GetTable<TblRow>().SingleOrDefault(e => e.TblRowID == TblRowIDForChartButton);
                        href = "href=\"" + PMRouting.Outgoing(new PMRoutingInfoMainContent(theTblRow.Tbl, theTblRow, theCD)) + "\"";
                    }
                }
                LiteralControl myLiteral = new LiteralControl("<a id=\"NoSortColumn" + TblColumnID.ToString() + "\" " + href + ">" + (VerticalText ? "&nbsp;" : theText) + "</a>");
                MyPlaceHolder.Controls.Add(myLiteral);
            }
            else
            {
                LinkButton NoSortColumn = new LinkButton();
                NoSortColumn.ID = "NoSortColumn";
                NoSortColumn.Text = "&nbsp;";
                MyPlaceHolder.Controls.Add(NoSortColumn);
            }
            if (!CheckJavaScriptHelper.IsJavascriptEnabled && !VerticalText)
            {
                Label NoSortColumn = new Label();
                NoSortColumn.Text = theText;
                MyPlaceHolder.Controls.Add(NoSortColumn);
            }

        }
        SetupCluetip();
    }

    protected void SetupCluetip()
    {
        if (CheckJavaScriptHelper.IsJavascriptEnabled && TblColumnID != null)
        {
            TblColumn theCD = DataAccess.RaterooDB.GetTable<TblColumn>().Single(cd => cd.TblColumnID == TblColumnID);
            string headingText = "";
            string bodyText = "";
            if ((theCD.Name == "" || theCD.Name == theCD.Abbreviation) && theCD.Explanation == "")
                return;
            if (theCD.Explanation == "")
            {
                headingText = "<span>" + theCD.Abbreviation + "</span>";
                bodyText = theCD.Name;
            }
            else
            {
                headingText = theCD.Name;
                bodyText = "<span>" + theCD.Explanation + "</span>";
            }
            ColumnPopUpLiteral.Text = bodyText;

            ColumnPopUpMouseOverArea.AddAttribute("title",headingText);
            ColumnPopUpMouseOverArea.AddAttribute("rel","#COLPU" + TblColumnID.ToString());
            ColumnPopUpContent.AddAttribute("id","COLPU" + TblColumnID.ToString());
        }
    }

    protected void ProcessSort(object sender, EventArgs e)
    {
        SortFn(TblColumnID, CurrentlySorting ? !DoSortOrderAscending : DoSortOrderAscending);
    }
}
