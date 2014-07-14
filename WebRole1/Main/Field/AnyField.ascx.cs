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
using ClassLibrary1.Model;



public partial class AnyFieldFilter : System.Web.UI.UserControl, IFilterField
{
    public FieldsBoxMode Mode { get; set; }
    public int? TblRowID { get; set; }
    public int FieldDefinitionOrTblColumnID {get; set;}
    public R8RDataAccess DataAccess { get; set; }
    public FieldDefinitionInfo FieldInfo { get; set; }
    public TblColumnInfo TblColInfo { get; set; }
    public System.Web.UI.UserControl theField;

    public AnyFieldFilter()
    {
        DataAccess = new R8RDataAccess();
    }

    public void AddSpecificFieldType(List<AnyFieldFilter> fieldsAlreadyAdded, bool isPostback)
    {
        // Note: We need to pass isPostback since this is called during itemCreated, before
        // the control is added to the parent. Thus, here, Page == null.

        // Note: This should be called during the load event but after databinding
        // (for example, in the ItemDataBound event).

        if (FieldInfo != null)
        {
            switch (FieldInfo.FieldType)
            {
                case FieldTypes.AddressField:
                    theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/AddressField.ascx");
                    break;
                case FieldTypes.ChoiceField:
                    theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/ChoiceField.ascx");
                    break;
                case FieldTypes.DateTimeField:
                    theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/DateTimeField.ascx");
                    break;
                case FieldTypes.NumberField:
                    theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/NumberField.ascx");
                    break;
                case FieldTypes.TextField:
                    theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/TextField.ascx");
                    break;
                default:
                    throw new Exception("Unknown field in AnyField");
            }
            ((IFilterField)theField).TblRowID = TblRowID;
            ((IFilterField)theField).Mode = Mode;
            ((IFilterField)theField).DataAccess = DataAccess;
            ((IFilterField)theField).FieldDefinitionOrTblColumnID = FieldInfo.FieldDefinitionID;

            PlaceHolder1.Controls.Add(theField);

            if (FieldInfo.FieldType == FieldTypes.ChoiceField)
            {
                SetChoiceFieldSettings(fieldsAlreadyAdded, isPostback);
            }

        }
        else if (TblColInfo != null)
        {
            if (Mode == FieldsBoxMode.addTblRow)
            {
                theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/RatingTypeSelector.ascx");
            }
            else
            {
                theField = (System.Web.UI.UserControl)LoadControl("~/Main/Field/OpinionColumnFilter.ascx");
            }
            ((IFilterField)theField).DataAccess = DataAccess;
            ((IFilterField)theField).FieldDefinitionOrTblColumnID = TblColInfo.TblColumnID;
            PlaceHolder1.Controls.Add(theField);
        }
    }

    public UserSelectedRatingInfo GetUserSelectedRatingInfo()
    {
        if (TblColInfo == null || Mode != FieldsBoxMode.addTblRow)
            return null;
        UserSelectedRatingInfo theRatingInfo = ((RatingTypeSelector)theField).GetUserSelectedRatingInfo();
        theRatingInfo.theColumnID = TblColInfo.TblColumnID;
        return theRatingInfo;
    }

    protected void SetChoiceFieldSettings(List<AnyFieldFilter> fieldsAlreadyAdded, bool isPostback)
    {
        ChoiceFieldFilter theChoiceField = (ChoiceFieldFilter)theField;
        theChoiceField.FieldDefinitionID = FieldInfo.FieldDefinitionID;
        R8RDataAccess Obj = new R8RDataAccess();
        ChoiceGroupFieldDefinition theFieldDefinition = Obj.R8RDB.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.FieldDefinitionID == FieldInfo.FieldDefinitionID);
        theChoiceField.ChoiceGroupID = theFieldDefinition.ChoiceGroupID;
        if (theFieldDefinition.DependentOnChoiceGroupFieldDefinitionID != null)
        {
            int dependentOnFieldDefinitionID = Obj.R8RDB.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.ChoiceGroupFieldDefinitionID == theFieldDefinition.DependentOnChoiceGroupFieldDefinitionID).FieldDefinitionID;
            var theDependerAlreadyAdded = fieldsAlreadyAdded.SingleOrDefault(f => f.FieldInfo.FieldDefinitionID == dependentOnFieldDefinitionID);
            if (theDependerAlreadyAdded != null)
            {
                ChoiceFieldFilter theDepender = (ChoiceFieldFilter)(theDependerAlreadyAdded.theField);
                theDepender.AddDependee(theChoiceField);
            }
        }
        if (!isPostback)
            theChoiceField.Setup();
    }

    public FilterRule GetFilterRule()
    {
        return ((IFilterField)theField).GetFilterRule();
    }

    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        return ((IFilterField)theField).GetFieldValue(theGroup);
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        return ((IFilterField)theField).InputDataValidatesOK(ref errorMessage);
    }

}

