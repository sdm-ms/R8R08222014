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


using MoreStrings;
using ClassLibrary1.Model;


public partial class TextFieldFilter : System.Web.UI.UserControl, IFilterField
{
    public FieldsBoxMode Mode { get; set; }
    public int? TblRowID { get; set; }
    public int FieldDefinitionOrTblColumnID {get; set;}
    public RaterooDataAccess DataAccess { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Mode == FieldsBoxMode.addTblRow || Mode == FieldsBoxMode.modifyFields)
        {
            if (Mode == FieldsBoxMode.modifyFields)
            {
                TextField theTextField = DataAccess.RaterooDB.GetTable<TextField>().SingleOrDefault(a =>
                            a.Field.FieldDefinitionID == FieldDefinitionOrTblColumnID
                            && a.Field.TblRowID == TblRowID && a.Status == (Byte)StatusOfObject.Active);
                if (theTextField != null)
                {
                    TxtMain.Text = theTextField.Text;
                    LinkTextBox.Text = theTextField.Link;
                }
            }
            TxtMain.CssClass = "setFieldsTextBox";
            LinkTextBox.CssClass = "setFieldsTextBox";
            TextFieldDefinition theTextFieldDefinition = DataAccess.RaterooDB.GetTable<TextFieldDefinition>().Single(t => t.FieldDefinitionID == FieldDefinitionOrTblColumnID);
            if (!theTextFieldDefinition.IncludeText)
            {
                TextRowOpening.AddAttribute("style", "display:none;");
                TextRowClosing.AddAttribute("style", "display:none;");
            }
            if (!theTextFieldDefinition.IncludeLink)
            {
                TextLabelOpening.AddAttribute("style", "display:none;");
                TextLabelClosing.AddAttribute("style", "display:none;");
                LinkRowOpening.AddAttribute("style", "display:none;");
                LinkRowClosing.AddAttribute("style", "display:none;");
            }
        }
        else
        {
                TextLabelOpening.AddAttribute("style", "display:none;");
                TextLabelClosing.AddAttribute("style", "display:none;");
                TxtMain.CssClass = "filterTextBoxFullLength";
                LinkRowOpening.AddAttribute("style", "display:none;");
                LinkRowClosing.AddAttribute("style", "display:none;");
        }
    }
    //protected void RBtnExact_CheckedChanged(object sender, EventArgs e)
    //{
    //    if (RBtnExact.Checked == true)
    //    {

    //        TxtFrom.Text = "";
    //        TxtTo.Text = "";
    //    }

    //}
    //protected void RBtnRange_CheckedChanged(object sender, EventArgs e)
    //{
    //    if (RBtnRange.Checked == true)
    //    {

    //        TxtExact.Text = "";
    //    }
    //}

    protected void BtnClear_Click(object sender, EventArgs e)
    {
        // TxtExact.Text = "";
        TxtMain.Text = "";
    }
    //Checking the filter
    public bool FilterOn()
    {
        return (TxtMain.Text != "");
        //if ((TxtFrom.Text == "" || TxtTo.Text == "") && TxtExact.Text == "")
        //{
        //    return false;
        //}
        //else
        //{
        //    return true;
        //}
    }
    //Creating filter query
    public FilterRule GetFilterRule()
    {
        //string ExactText = TxtExact.Text.Trim();

        //string FromText = TxtFrom.Text.Trim();
        string MainText = TxtMain.Text.Trim();
        //if (RBtnExact.Checked == true)
        //{
        //    if (ExactText == "")
        //        return null;
        //    else
        //        return new TextFilterRule(FieldDefinitionID, ExactText, "", "");
        //}
        //else
        {
            if (MainText == "")
                return null;
            else
                return new TextFilterRule(FieldDefinitionOrTblColumnID, MainText);
        }

    }

    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        string MainText = MoreStringManip.StripHtml(TxtMain.Text.Trim());
        string LinkText = MoreStringManip.StripHtml(LinkTextBox.Text.Trim());
        if (MainText == "" && LinkText == "")
            return null;
        else
        {
            FieldDefinition theFieldDefinition = DataAccess.RaterooDB.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == (int)FieldDefinitionOrTblColumnID);
            return new TextFieldDataInfo(theFieldDefinition, MainText, LinkText, theGroup, DataAccess);
        }
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        return true;
    }

}
