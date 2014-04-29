﻿using System;
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

using GoogleGeocoder;
using System.Data.SqlClient;

using MoreStrings;
using ClassLibrary1.Model;

public partial class AddressFieldFilter : System.Web.UI.UserControl, IFilterField
{
    public FieldsBoxMode Mode { get; set; }
    public int? TblRowID { get; set; }
    public int FieldDefinitionOrTblColumnID {get; set;}
    public RaterooDataAccess DataAccess { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Mode == FieldsBoxMode.addTblRow || Mode == FieldsBoxMode.modifyFields)
        {
            WithinMile.AddAttribute("style", "display:none;");
            if (Mode == FieldsBoxMode.modifyFields)
            {
                AddressField theAddressField = DataAccess.RaterooDB.GetTable<AddressField>().SingleOrDefault(a =>
                            a.Field.FieldDefinitionID == FieldDefinitionOrTblColumnID
                            && a.Field.TblRowID == TblRowID && a.Status == (Byte) StatusOfObject.Active);
                if (theAddressField != null)
                    TxtAddress.Text = theAddressField.AddressString;
            }
        }
    }

    protected void BtnClear_Click(object sender, EventArgs e)
    {
        TxtAddress.Text = "";
        TxtMile.Text = "";
    }

    public FilterRule GetFilterRule()
    {
        string mileText = TxtMile.Text;
        if (TxtMile.Text == "")
            mileText = "0.1";
        if (TxtAddress.Text.Trim() == "")
            return null;
        else
            return new AddressFilterRule((int)FieldDefinitionOrTblColumnID, TxtAddress.Text.Trim(), Convert.ToDecimal(mileText));
    }

    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        string addressText = MoreStringManip.StripHtml(TxtAddress.Text.Trim());
        if (addressText == "")
            return null;
        else
        {
            FieldDefinition theFieldDefinition = DataAccess.RaterooDB.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == (int)FieldDefinitionOrTblColumnID);
            return new AddressFieldDataInfo(theFieldDefinition, addressText, theGroup, DataAccess);
        }
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        errorMessage = "";

        bool milesValidatesOK = MoreStringManip.ValidateNumberString(TxtMile.Text, true, 0, null, ref errorMessage);
        if (!milesValidatesOK)
        {
            errorMessage = "Miles -- " + errorMessage;
            return false;
        }

        string theAddress = TxtAddress.Text.Trim();
        if (theAddress == "")
            return true;
        Coordinate myCoordinates = Geocode.GetCoordinates(theAddress);
        errorMessage = "Google could not find the address you specified. Please delete or correct the address."; // in case of error
        return !(myCoordinates.Latitude == 0 && myCoordinates.Latitude == 0);
    }

    



}