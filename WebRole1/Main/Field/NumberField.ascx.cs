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




public partial class NumberFieldFilter : System.Web.UI.UserControl, IFilterField
{
    public FieldsBoxMode Mode { get; set; }
    public int? TblRowID { get; set; }
    public int FieldDefinitionOrTblColumnID {get; set;}
    public R8RDataAccess DataAccess { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Mode == FieldsBoxMode.addTblRow || Mode == FieldsBoxMode.modifyFields)
        {

            TD1a.AddAttribute("style", "display:none;");
            TD1b.AddAttribute("style", "display:none;");
            TD2a.AddAttribute("style", "display:none;");
            TD2b.AddAttribute("style", "display:none;");
            if (Mode == FieldsBoxMode.modifyFields)
            {
                NumberField theNumberField = DataAccess.R8RDB.GetTable<NumberField>().SingleOrDefault(a =>
                            a.Field.FieldDefinitionID == FieldDefinitionOrTblColumnID
                            && a.Field.TblRowID == TblRowID && a.Status == (Byte)StatusOfObject.Active);
                
                if (theNumberField != null)
                    TxtTo.Text = (theNumberField.Number == null) ? "" : MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(theNumberField.Number, theNumberField.Field.FieldDefinition.NumberFieldDefinitions.FirstOrDefault().DecimalPlaces);

            } 
        }
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
            return new NumberFilterRule(FieldDefinitionOrTblColumnID, MinValue, MaxValue);
    }

    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        string ToText = TxtTo.Text.Trim();
        if (ToText == "")
            return null;
        else
        {
            decimal theNumber = Convert.ToDecimal(TxtTo.Text);
            FieldDefinition theFieldDefinition = DataAccess.R8RDB.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == (int)FieldDefinitionOrTblColumnID);
            return new NumericFieldDataInfo(theFieldDefinition, theNumber, theGroup, DataAccess);
        }
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        NumberFieldDefinition theNFD = DataAccess.R8RDB.GetTable<NumberFieldDefinition>().Single(nfd => nfd.FieldDefinitionID == FieldDefinitionOrTblColumnID);
        return MoreStrings.MoreStringManip.ValidateNumberString(TxtFrom.Text, true, theNFD.Minimum, theNFD.Maximum, ref errorMessage) && MoreStrings.MoreStringManip.ValidateNumberString(TxtTo.Text, true, theNFD.Minimum, theNFD.Maximum, ref errorMessage);
    }
}