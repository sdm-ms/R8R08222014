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
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using ClassLibrary1.Model;


public partial class Main_Table_FieldsDisplay : System.Web.UI.UserControl
{
    public FieldDisplayHtml Setup(IR8RDataContext theDataContextToUse, TblDimension theTblDimension, FieldsLocation theLocation, int entityID, bool includeEntityName)
    {
        FieldsDisplayCreator theCreator = new FieldsDisplayCreator();
        //ProfileSimple.Start("FieldDisplayHtml");

        FieldDisplayHtml myFieldDisplayHtml = theCreator.GetFieldDisplayHtml(theDataContextToUse, entityID, theLocation);

        //ProfileSimple.End("FieldDisplayHtml");
        //Trace.TraceInformation(myFieldDisplayHtml.entityName);
        DisplayList.Text = myFieldDisplayHtml.theMainHtml;
        return myFieldDisplayHtml;
    }
    
}
