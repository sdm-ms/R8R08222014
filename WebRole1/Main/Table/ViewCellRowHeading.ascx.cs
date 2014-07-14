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


using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class Main_Table_ViewCellRowHeading : System.Web.UI.UserControl
{
    public LoadRowHeadingInfo theRowHeadingInfo {get; set;}


    protected void Page_Load(object sender, EventArgs e)
    {
        if (theRowHeadingInfo != null)
        {
            Setup(theRowHeadingInfo.dataAccess, theRowHeadingInfo.theTblDimension, theRowHeadingInfo.theTblID, theRowHeadingInfo.theTblRowID, false, true, false);
        }
    }

    public void Setup(R8RDataAccess theDataAccess, TblDimension theTblDimension, int theTblID, int theTblRowID, bool commentsEnabled, bool canEditFields, bool doRebind)
    {
        FieldDisplayHtml mainFieldDisplayHtml = new FieldDisplayHtml();
        Main_Table_FieldsDisplay theMainFieldsDisplay = (Main_Table_FieldsDisplay) LoadControl("~/Main/Table/FieldsDisplay.ascx");
        mainFieldDisplayHtml = theMainFieldsDisplay.Setup(theDataAccess.R8RDB, theTblDimension, FieldsLocation.RowHeading, theTblRowID, true);
        FieldsDisplayPlaceHolder.Controls.Add(theMainFieldsDisplay);

        if (CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            FieldDisplayHtml popupFieldDisplayHtml = new FieldDisplayHtml();
            Main_Table_FieldsDisplay thePopUpFieldsDisplay = (Main_Table_FieldsDisplay)LoadControl("~/Main/Table/FieldsDisplay.ascx");
            popupFieldDisplayHtml = thePopUpFieldsDisplay.Setup(theDataAccess.R8RDB, theTblDimension, FieldsLocation.RowPopup, theTblRowID, false);
            PopUpFieldsDisplayPlaceHolder.Controls.Add(thePopUpFieldsDisplay);

            FieldsDisplayDiv.AddAttribute("title", mainFieldDisplayHtml.rowName);
            if (popupFieldDisplayHtml.fieldDataExists)
            { // prepare info for our Clutip
                FieldsDisplayDiv.AddAttribute("rel", "#FPU" + theTblRowID.ToString());
                FieldsDisplayPopUp.AddAttribute("id", "FPU" + theTblRowID.ToString());
            }
        }

        SetupLinks(commentsEnabled, canEditFields, theTblRowID);
    }

    protected void SetupLinks(bool commentsEnabled, bool canEditFields, int entityID)
    {
        //if (commentsEnabled || canEditFields)
        //{

        //    if (commentsEnabled)
        //    {
        //        LinkButton CommentLink = new LinkButton();
        //        CommentLink.Text = "View Comments";
        //        CommentLink.PostBackUrl = "~/Main/TblRow/Comments.aspx?RowId=" + entityID;
        //        FieldsDisplayPlaceHolder.Controls.Add(CommentLink);
        //    }

        //    if (canEditFields)
        //    {
        //        LinkButton EditLink = new LinkButton();
        //        EditLink.Text = "Edit Fields";
        //        EditLink.PostBackUrl = "~/Fields/EditFieldValue.aspx?RowId=" + entityID;
        //        FieldsDisplayPlaceHolder.Controls.Add(EditLink);
        //    }

        //    //LinksRow.Cells.Add(theCell);
        //    //RowHeadingTable.Rows.Add(LinksRow);
        //}
    }




    protected void Page_Init(object sender, EventArgs e)
    {

    }


}
