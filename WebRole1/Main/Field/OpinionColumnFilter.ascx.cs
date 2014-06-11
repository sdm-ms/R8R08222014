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
using ClassLibrary1.Model;



public partial class OpinionColumnFilter : System.Web.UI.UserControl, IFilterField
{
    // These aren't really relevant here, but we need them to implement IFilterField
    public FieldsBoxMode Mode { get; set; }
    public int? TblRowID { get; set; }
    public int FieldDefinitionOrTblColumnID {get; set;}
    public R8RDataAccess DataAccess { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void BtnClear_Click(object sender, EventArgs e)
    {
        TxtFrom.Text = "";
        TxtTo.Text = "";

    }

    //Creating the filter query
    public FilterRule GetFilterRule()
    {
        decimal? MinValue;
        decimal? MaxValue;
        if (TxtFrom.Text.Trim() == "")
            MinValue = null;
        else
            MinValue = Convert.ToDecimal(TxtFrom.Text);
        if (TxtTo.Text.Trim() == "")
            MaxValue = null;
        else
            MaxValue = Convert.ToDecimal(TxtTo.Text);
        if (MinValue == null && MaxValue == null)
            return null;
        else
            return new TblColumnFilterRule(FieldDefinitionOrTblColumnID, MinValue, MaxValue);
    }


    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        return null;
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        errorMessage = "";
        return MoreStrings.MoreStringManip.ValidateNumberString(TxtFrom.Text, true, null, null, ref errorMessage)
            && MoreStrings.MoreStringManip.ValidateNumberString(TxtTo.Text, true, null, null, ref errorMessage);
    }
}
