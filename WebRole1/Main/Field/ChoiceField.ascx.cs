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

using System.Collections.Generic;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;



public partial class ChoiceFieldFilter : System.Web.UI.UserControl, IFilterField
{
    public FieldsBoxMode Mode { get; set; }
    public Guid? TblRowID { get; set; }
    public Guid FieldDefinitionOrTblColumnID { get; set; }
    public R8RDataAccess DataAccess { get; set; }
    public bool AllowMultipleSelections;
    List<ChoiceMenuItem> theChoices = null;
    public Guid ChoiceGroupID { get; set; }
    public Guid FieldDefinitionID { get; set; }
    private ChoiceFieldFilter _DependsOn;
    ChoiceFieldFilter DependsOn
    {
        get { return _DependsOn; }
        set
        {
            _DependsOn = value;
            if (value == null)  
                filterDepend.Value = "";
            else
                filterDepend.Value = value.FieldDefinitionOrTblColumnID.ToString();
        } }
    public List<ChoiceFieldFilter> Dependees = new List<ChoiceFieldFilter>();


    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void Setup()
    {
        if (theChoices == null)
            theChoices = new List<ChoiceMenuItem>();
        ChoiceGroupFieldDefinition theCGFD = DataAccess.R8RDB.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.FieldDefinitionID == FieldDefinitionOrTblColumnID);
        ChoiceGroup theChoiceGroup = theCGFD.ChoiceGroup;
        ChoiceGroupID = theChoiceGroup.ChoiceGroupID;
        AllowMultipleSelections = theChoiceGroup.AllowMultipleSelections;

        if (Mode == FieldsBoxMode.addTblRow || Mode == FieldsBoxMode.modifyFields)
        {
            if (Mode == FieldsBoxMode.modifyFields)
            {
                ChoiceField theChoiceField = DataAccess.R8RDB.GetTable<ChoiceField>().SingleOrDefault(a =>
                            a.Field.FieldDefinitionID == FieldDefinitionOrTblColumnID
                            && a.Field.TblRowID == TblRowID && a.Status == (Byte)StatusOfObject.Active);
                if (theChoiceField != null)
                {
                    theChoices = DataAccess.R8RDB.GetTable<ChoiceInField>().Where(
                        a => a.ChoiceFieldID == theChoiceField.ChoiceFieldID && a.Status == (Byte)StatusOfObject.Active).Select(a => new ChoiceMenuItem { Value = a.ChoiceInGroupID.ToString(), Text = a.ChoiceInGroup.ChoiceText }).ToList(); 
                }
            }
        }
        Populate();
    }

    internal bool DependentOn(ChoiceFieldFilter theChoiceField)
    {
        if (theChoiceField == null)
            return false;
        else if (DependsOn == null)
            return false;
        else if (DependsOn == theChoiceField)
            return true;
        else return (DependentOn(theChoiceField.DependsOn));
    }

    public void AddDependee(ChoiceFieldFilter theDependee)
    {
        if (!DependentOn(theDependee))
        {
            if (!Dependees.Any())
            { // We need to hook up the selected index changed event.
                HookUpSelectedIndexChanged();
            }
            
            theDependee.DependsOn = this;
            Dependees.Add(theDependee);

            // Don't need the following: theDependee.UpdatePanelAroundChoiceGroup.Triggers.Add(new AsyncPostBackTrigger { ControlID = DdlChoice.ClientID.ToString(), EventName = "SelectedIndexChanged" });
        }
    }

    public List<ChoiceMenuItem> GetSelectedChoices()
    {
        MyLoadViewState();
        if (AllowMultipleSelections)
        {
            return theChoices;
        }
        else
        {
            List<ChoiceMenuItem> theList = new List<ChoiceMenuItem>();
            ChoiceMenuItem theChoice = GetSelectedChoiceMenuItem();
            if (theChoice != null)
                theList.Add(theChoice);
            return theList;
        }
    }

    //This function return the choice number of selected choice
    public Guid? GetValue()
    {
        Guid? ChoiceInGroupId = (DdlChoice.SelectedValue == "-1" || DdlChoice.SelectedValue == "") ? null : (int?)(Convert.ToInt32(DdlChoice.SelectedValue));
        return ChoiceInGroupId;
    }

    public ChoiceMenuItem GetSelectedChoiceMenuItem()
    {
        if (DdlChoice.SelectedValue == "-1" || DdlChoice.SelectedValue == "")
            return null;
        return new ChoiceMenuItem { Text = DdlChoice.SelectedItem.Text, Value = DdlChoice.SelectedItem.Value };
    }

    public void Populate()
    {
        List<ChoiceMenuItem> dropDownItems = null;
        if (DependsOn == null)
        {
            dropDownItems = ChoiceMenuAccess.GetChoiceMenuItemsForIndependentGroup(ChoiceGroupID);
        }
        else
        {
            List<ChoiceMenuItem> dependsOnChoices = DependsOn.GetSelectedChoices();
            if (dependsOnChoices == null || !dependsOnChoices.Any())
            {
                List<int> availableOptionsInDepended = null;
                if (DependsOn.DdlChoice.Items != null)
                {
                    availableOptionsInDepended = DependsOn.DdlChoice.Items.Cast<ListItem>()
                             .Where(x => x.Value != "-1")
                             .Select(x => Convert.ToInt32(x.Value))
                             .ToList();
                }
                dropDownItems = ChoiceMenuAccess.GetChoiceMenuItemsForDependentGroupWithNoDependentSelection(availableOptionsInDepended, ChoiceGroupID);
            }
            else
            {
                List<int> determiningGroupValues = dependsOnChoices.Select(x => Convert.ToInt32(x.Value)).ToList();
                dropDownItems = ChoiceMenuAccess.GetChoiceMenuItemsForDependentGroupWithDependentSelections(determiningGroupValues, ChoiceGroupID);
            }
        }

        UpdateChoicesBasedOnAvailability(dropDownItems);

        BuildChoicesFromMenu(dropDownItems);

        DdlChoice.ClearSelection();
        DdlChoice.DataSource = dropDownItems;
        DdlChoice.DataTextField = "Text";
        DdlChoice.DataValueField = "Value";
        DdlChoice.DataBind();

        ListChoices();
    }

    public void BuildChoicesFromMenu(List<ChoiceMenuItem> dropDownItems)
    {
        MyLoadViewState();
        if (!AllowMultipleSelections || Mode == FieldsBoxMode.filterWithButton || Mode == FieldsBoxMode.filterWithoutButton)
        {
            if (DdlChoice.SelectedItem != null && theChoices == null && dropDownItems != null
                && dropDownItems.Any(x => x.Value == DdlChoice.SelectedItem.Value))
            {

                theChoices = new List<ChoiceMenuItem>();
                ChoiceMenuItem theItem = new ChoiceMenuItem();
                theItem.Text = DdlChoice.SelectedItem.Text;
                theItem.Value = DdlChoice.SelectedItem.Value;
                theChoices.Add(theItem);
            }
        }
    }

    public void UpdateChoicesBasedOnAvailability(List<ChoiceMenuItem> dropDownItems)
    {
        if (theChoices != null)
        {
            List<ChoiceMenuItem> theNewChoices = new List<ChoiceMenuItem>();
            foreach (ChoiceMenuItem choice in theChoices)
            {
                ChoiceMenuItem theItem = dropDownItems.SingleOrDefault(d => d.Value == choice.Value);
                if (theItem != null)
                    theNewChoices.Add((ChoiceMenuItem) theItem);
            }
            theChoices = theNewChoices;

        }
    }

    public void RemoveChoice(string value)
    {
        ChoiceMenuItem theItem = theChoices.SingleOrDefault(c => c.Value == value);
        if (theItem != null)
            theChoices.Remove((ChoiceMenuItem) theItem);
    }

    public void SelectChoiceInMenu(int? theChoice)
    {
        string theValue = "-1";
        if (theChoice != null)
            theValue = theChoice.ToString();
        DdlChoice.SelectedValue = theValue;
    }

    protected void LinqDataSourceChoices_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        if (theChoices == null || !AllowMultipleSelections || Mode == FieldsBoxMode.filterWithButton || Mode == FieldsBoxMode.filterWithoutButton)
            e.Result = new List<ChoiceMenuItem>(); // empty list
        else
            e.Result = theChoices.OrderBy(c => c.Text);
    }

    public void ListChoices()
    {
        ListChoicesSingleSelection();
        ListChoicesMultipleSelection();
    }

    private void ListChoicesSingleSelection()
    {        
        if (!AllowMultipleSelections || Mode == FieldsBoxMode.filterWithButton || Mode == FieldsBoxMode.filterWithoutButton)
        {
            if (theChoices != null && theChoices.Any())
                SelectChoiceInMenu(Convert.ToInt32(theChoices.FirstOrDefault().Value));
        }
    }

    private void ListChoicesMultipleSelection()
    {
        if (AllowMultipleSelections && ( Mode == FieldsBoxMode.addTblRow || Mode == FieldsBoxMode.modifyFields))
        {
            ViewState["theChoices"] = theChoices;
            ViewState["AllowMultipleSelections"] = AllowMultipleSelections ? "1" : "0";
            ViewState["Mode"] = ((int)Mode).ToString();
            ListMultipleChoices.DataBind();
            SelectChoiceInMenu(null);
        }
    }

    protected void MyLoadViewState()
    {
        if (theChoices == null && ViewState["theChoices"] != null)
        {
            theChoices = ViewState["theChoices"] as List<ChoiceMenuItem>;
            AllowMultipleSelections = ViewState["AllowMultipleSelections"].ToString() == "1";
            Mode = (FieldsBoxMode)Convert.ToInt32(ViewState["Mode"]);
        }
    }

    // This event loads the choices to dropdowns
    protected void DdlChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        MyLoadViewState();
        ChoiceMenuItem theChoice = GetSelectedChoiceMenuItem();
        if (theChoices == null || !AllowMultipleSelections || Mode == FieldsBoxMode.filterWithButton || Mode == FieldsBoxMode.filterWithoutButton)
            theChoices = new List<ChoiceMenuItem>();
        if (theChoice != null)
        {
            bool alreadyExists = theChoices.Any(c => c.Value == theChoice.Value);
            if (!alreadyExists)
                theChoices.Add((ChoiceMenuItem)theChoice);
        }
        ListChoices();
        UpdatePanelAroundChoiceGroup.Update();
        UpdateDependees();
        //if (DependsOn != null || Dependees.Any())
        //    UpdateDependencyChainFromTop();
    }

    public void DeleteButton_Click(Object sender, CommandEventArgs e)
    {
        MyLoadViewState();
        RemoveChoice((string) e.CommandArgument);
        ListChoices();
        UpdatePanelAroundChoiceGroup.Update();
        UpdateDependees();
    }


    public void UpdateSelfAndDependees()
    {
        DoUpdating();
        UpdateDependees();
    }

    public void UpdateDependees()
    {
        foreach (var d in Dependees)
        {
            d.UpdateSelfAndDependees();
        }
    }

    public void DoUpdating()
    {
        Populate();
        UpdatePanelAroundChoiceGroup.Update();
    }

    public FilterRule GetFilterRule()
    {
        if (DdlChoice.SelectedIndex <= 0)
            return null;

        int ChoiceValue = int.Parse(DdlChoice.SelectedValue);
        return new ChoiceFilterRule(FieldDefinitionOrTblColumnID, ChoiceValue);
    }


    public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
    {
        MyLoadViewState();
        if (AllowMultipleSelections)
        {
            List<Guid> theChoiceInGroupIDs = new List<Guid>();
            if (theChoices != null && theChoices.Any())
            {
                theChoiceInGroupIDs = theChoices.Select(x => new Guid(x.Value)).ToList();
            }

            List<ChoiceInGroup> theChoiceInGroups = new List<ChoiceInGroup>();
            foreach (var choiceInGroupID in theChoiceInGroupIDs)
                theChoiceInGroups.Add(DataAccess.R8RDB.GetTable<ChoiceInGroup>().Single(cig => cig.ChoiceInGroupID == choiceInGroupID) );

            FieldDefinition theFieldDefinition = DataAccess.R8RDB.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == (Guid)FieldDefinitionOrTblColumnID);
            return new ChoiceFieldDataInfo(theFieldDefinition, theChoiceInGroups, theGroup, DataAccess);
        }
        else
        {
            if (DdlChoice.SelectedIndex <= 0)
                return null;
            else
            {
                FieldDefinition theFieldDefinition = DataAccess.R8RDB.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == (Guid)FieldDefinitionOrTblColumnID);
                ChoiceInGroup choiceInGroup = DataAccess.R8RDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceInGroupID == Convert.ToInt32(DdlChoice.SelectedValue));
                return new ChoiceFieldDataInfo(theFieldDefinition, choiceInGroup, theGroup, DataAccess);
            }
        }
    }

    public void HookUpSelectedIndexChanged()
    {
        // We need to hook up the selected index changed event manually for two reasons.
        // First, we want to generate a postback only if some other choice group depends on this one.
        // Second, even if this weren't the case, if we just set the event declaratively in
        // the page markup, the event won't fire, because the settings will not be set
        // at the time of event firing during postback, and so the event would not otherwise
        // fire.
        DdlChoice.AutoPostBack = true;
        DdlChoice.SelectedIndexChanged += new EventHandler(DdlChoice_SelectedIndexChanged);
    }

    public bool InputDataValidatesOK(ref string errorMessage)
    {
        return true;
    }

}
