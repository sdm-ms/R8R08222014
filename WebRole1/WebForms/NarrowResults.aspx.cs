using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Diagnostics;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class NarrowResults : System.Web.UI.Page
{
    FilterRules theFilterRules = null;
    Guid TblID;
    Guid TblTabID;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            IncrementCounter();
            TblID = new Guid(Request.QueryString["TableId"]);
            TblTabID = new Guid(Request.QueryString["SubtableId"]);
            LoadFilterRules();
            FieldsBox.SetupStandalonePage(TblID, TblTabID);
        }
        catch
        {
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetFilterRulesHiddenText();
    }

    public void IncrementCounter()
    {
        string currentValue = narrowResultsCount.Value;
        if (currentValue == "")
            narrowResultsCount.Value = "1";
        else
            narrowResultsCount.Value = (Convert.ToInt32(currentValue) + 1).ToString();
        Debug.WriteLine("narrowResultsCount " + narrowResultsCount.Value);
    }

    public void LoadFilterRules()
    {
        string filterStringQuery = Request.QueryString["FilterRules"];
        if (filterStringQuery == null || filterStringQuery == "")
            theFilterRules = new FilterRules(TblID, false, false);
        else
            theFilterRules = FilterRulesSerializer.GetFilterRulesFromUrlEncodedString(filterStringQuery);
    }

    public void SetFilterRulesHiddenText()
    {
        theFilterRules = FieldsBox.GetFilterRulesWithDeletedBasedOnUserChoice();
        LiteralFilterRules.Text = FilterRulesSerializer.GetStringFromFilterRules(theFilterRules);

        Debug.WriteLine("LoadFilterRulesText " + LiteralFilterRules.Text);
    }
}
