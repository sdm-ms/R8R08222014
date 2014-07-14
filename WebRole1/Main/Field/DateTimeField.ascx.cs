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



public partial class DateTimeFieldFilter : System.Web.UI.UserControl, IFilterField
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
                DateTimeField theDateTimeField = DataAccess.R8RDB.GetTable<DateTimeField>().SingleOrDefault(a =>
                            a.Field.FieldDefinitionID == FieldDefinitionOrTblColumnID
                            && a.Field.TblRowID == TblRowID && a.Status == (Byte)StatusOfObject.Active);
                if (theDateTimeField != null)
                    ToDate.theDate = theDateTimeField.DateTime;
            }
        }
    }

    //Creating the filter query
    public FilterRule GetFilterRule()
    {
        DateTime? MinValue;
        DateTime? MaxValue;
        MinValue = FromDate.theDate;
        MaxValue = ToDate.theDate;
        if (MinValue == null && MaxValue == null)
            return null;
        else
            return new DateTimeFilterRule(FieldDefinitionOrTblColumnID, MinValue, MaxValue);
    }

    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        if (ToDate.theDate == null)
            return null;
        else
        {
            FieldDefinition theFieldDefinition = DataAccess.R8RDB.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == (int)FieldDefinitionOrTblColumnID);
            return new DateTimeFieldDataInfo(theFieldDefinition, (DateTime)ToDate.theDate, theGroup, DataAccess);
        }
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        return FromDate.ValidatesOK(true, null, null, ref errorMessage) && ToDate.ValidatesOK(true, null, null, ref errorMessage);
    }
}
