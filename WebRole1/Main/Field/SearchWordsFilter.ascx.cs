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
using ClassLibrary1.EFModel;




public partial class SearchWordsFilter : System.Web.UI.UserControl, IFilterField
{
    public Guid TblID {get; set;}
    // These aren't really relevant here, but we need them to implement IFilterField
    public FieldsBoxMode Mode { get; set; }
    public Guid? TblRowID { get; set; }
    public Guid FieldDefinitionOrTblColumnID { get; set; }
    public R8RDataAccess DataAccess { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public bool FilterOn()
    {
        return (TxtMain.Text.Trim() != "");
    }

    //Creating filter query
    public FilterRule GetFilterRule()
    {
        string MainText = TxtMain.Text.Trim();
        if (MainText == "")
            return null;
        else
            return new SearchWordsFilterRule(TblID, MainText);
    }

    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        return null;
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        return true;
    }

}
