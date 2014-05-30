using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

using System.Text;


namespace ClassLibrary1.Model
{
    public class ChoiceGroupSettingsMask
    {
        public static int AllowMultipleSelections = 1;
        public static int Alphabetize = 2;
        public static int InvisibleWhenEmpty = 4;
        public static int ShowTagCloud = 8;
        public static int PickViaAutoComplete = 16;
        public static int ShowAllPossibilitiesIfNoDependentChoice = 32;
        public static int AlphabetizeWhenShowingAllPossibilities = 64;
        public static int AllowAutoAddWhenAddingFields = 128;

        public static bool IsSet(int individualSetting, int settings)
        {
            return (settings & individualSetting) == individualSetting;
        }

        public static int GetStandardSetting()
        {
            return GetChoiceGroupSetting(false, false, false, false, false, false, true, false);
        }

        public static int GetChoiceGroupSetting(bool allowMultipleSelections, bool alphabetize,
            bool invisibleWhenEmpty, bool showTagCloud, bool pickViaAutoComplete,
            bool showAllPossibilitiesIfNoDependentChoice, bool alphabetizeWhenShowingAllPossibilities,
            bool allowAutoAddWhenAddingFields)
        {
            var myMask = new ChoiceGroupSettingsMask();
            int theValue = 0;
            if (allowMultipleSelections)
                theValue = theValue | AllowMultipleSelections;
            if (alphabetize)
                theValue = theValue | Alphabetize;
            if (invisibleWhenEmpty)
                theValue = theValue | InvisibleWhenEmpty;
            if (showTagCloud)
                theValue = theValue | ShowTagCloud;
            if (pickViaAutoComplete)
                theValue = theValue | PickViaAutoComplete;
            if (showAllPossibilitiesIfNoDependentChoice)
                theValue = theValue | ShowAllPossibilitiesIfNoDependentChoice;
            if (alphabetizeWhenShowingAllPossibilities)
                theValue = theValue | AlphabetizeWhenShowingAllPossibilities;
            if (allowAutoAddWhenAddingFields)
                theValue = theValue | AllowAutoAddWhenAddingFields;
            return theValue;
        }

    };

    // The following two classes allow for representations of hierarchical choice group data.
    public class DepItemsLevel
    {
        public string DependentOnString;
        public string[] NewItems;
        public DepItemsLevel(string dependentOnString, string[] newItems)
        {
            DependentOnString = dependentOnString;
            NewItems = newItems;
        }
    }

    public class DepItems
    {
        public string TheString;
        public DepItems[] TheDepItems;
        public DepItems(string theString, DepItems[] theDepItems)
        {
            TheString = theString;
            TheDepItems = theDepItems;
        }
        public DepItems(string theString, string[] itemsForLastLevel)
        {
            TheString = theString;
            DepItems[] emptyList = { };
            TheDepItems = itemsForLastLevel.ToList().AsQueryable().Select(x => new DepItems(x, emptyList)).ToArray();
        }
        public static DepItemsLevel[] GetLevel(DepItems[] theItems, int zeroBasedLevel)
        {
            if (zeroBasedLevel == 0)
                return theItems.Select(x => new DepItemsLevel(x.TheString, x.TheDepItems.Select(y => y.TheString).ToArray())).ToArray();
            else
            {
                List<DepItemsLevel> theLevelItems = new List<DepItemsLevel>();
                foreach (var item in theItems)
                {
                    DepItemsLevel[] itemsToAdd = GetLevel(item.TheDepItems, zeroBasedLevel - 1);
                    theLevelItems.AddRange(itemsToAdd);
                }
                return theLevelItems.ToArray();
            }
        }
    }

    /// <summary>
    /// A class representing a choice in a multiple-selection group list.
    /// </summary>
    public class ChoiceInGroupData
    {
        public String text; // Text for the item
        public int? determiningGroupValue; // The ChoiceInGroupID of a choice in the DependentOnChoiceGroupID that must be set for this to be available (or null if this is always avaiable)
        public bool isAvailable; // Is this generally available? False if this has been deactivated
        public int choiceNum; // The choice number -- the choices will be sorted by this number
        public int? choiceInGroupID; // Once this has been created in the database, this will be set to the object id for the ChoiceInGroup table

        public ChoiceInGroupData(String theText, int? theDeterminingGroupValue, bool theIsAvailable, int theChoiceNum, int? theChoiceInGroupID)
        {
            text = theText;
            determiningGroupValue = theDeterminingGroupValue;
            isAvailable = theIsAvailable;
            choiceNum = theChoiceNum;
            choiceInGroupID = theChoiceInGroupID;
        }

        public void ChangeText(String theText)
        {
            text = theText;
        }

        public void ChangeDeterminingGroupValue(int? theDeterminingGroupValue)
        {
            determiningGroupValue = theDeterminingGroupValue;
        }

        public void ChangeAvailability(bool theIsAvailable)
        {
            isAvailable = theIsAvailable;
        }

        public void ChangeChoiceNum(int theChoiceNum)
        {
            choiceNum = theChoiceNum;
        }

    }

    /// <summary>
    /// ChoiceGroupData: A simple class to maintain data about choice groups outside the database.
    /// </summary>
    public class ChoiceGroupData
    {
        private List<ChoiceInGroupData> theData;

        public List<ChoiceInGroupData> TheData
        {
            get { return theData; }
            set { theData = TheData; }
        }

        public int Count
        {
            get { return theData.Count; }
        }

        public ChoiceGroupData()
        {
            theData = new List<ChoiceInGroupData>();
        }

        public void AddChoiceToGroup(String text)
        {
            AddChoiceToGroup(text, null, true, null);
        }

        public void AddChoiceToGroup(String text, int? determiningGroupValue)
        {
            AddChoiceToGroup(text, determiningGroupValue, true, null);
        }

        public void AddChoiceToGroup(String text, int? determiningGroupValue, bool isAvailable, int? choiceInGroupID)
        {
            int choiceNum = theData.Count + 1; // This is the default -- add it to the end of the group.
            theData.Add(new ChoiceInGroupData(text, determiningGroupValue, isAvailable, choiceNum, choiceInGroupID));
        }

        public void Sort()
        {
            theData.Sort(delegate(ChoiceInGroupData cig1, ChoiceInGroupData cig2) { return cig1.choiceNum.CompareTo(cig2.choiceNum); });
            // Now, make sure the choice numbers are numbered correctly.
            for (int i = 1; i < theData.Count; i++)
                theData[i - 1].choiceNum = i;
        }

        // Return false if and only if there are at least two choices in the group with the same text and determining group value,
        // or if there are two choices with the same text and one of them has a null determining group value.
        public bool ChoicesAreUnique()
        {
            // TO DO: If we start to get long choice groups and time becomes an issue, we could group the 
            // data by text and then look for
            // matches within each group with >= 2.

            foreach (ChoiceInGroupData theChoice in theData)
            {

                if (theChoice.determiningGroupValue == null)
                { // any text match results in returning false
                    if (theData.Any(d => d.choiceNum != theChoice.choiceNum && d.text == theChoice.text))
                        return false;
                }
                else
                { // only text matches where determininggroup matches also return false
                    if (theData.Any(d => d.choiceNum != theChoice.choiceNum && d.determiningGroupValue == theChoice.determiningGroupValue && d.text == theChoice.text))
                        return false;
                }

            }

            return true;
        }


        public void LoadFromChoiceGroupID(int choiceGroupID)
        {
            if (TheData.Count != 0)
                throw new Exception("To load from choice group id, start with an empty choice group.");
            ActionProcessor Obj = new ActionProcessor();
            //  R8RSupport dataAccessModule = new R8RSupport();
            var ChoicesInGroup = Obj.DataContext.GetTable<ChoiceInGroup>().Where(cig => cig.ChoiceGroupID == choiceGroupID && cig.Status != (Byte)StatusOfObject.Proposed).OrderBy(cig => cig.ChoiceNum);
            foreach (var theChoice in ChoicesInGroup)
            {
                AddChoiceToGroup(theChoice.ChoiceText, theChoice.ActiveOnDeterminingGroupChoiceInGroupID, theChoice.Status == (Byte)StatusOfObject.Active, theChoice.ChoiceInGroupID);
            }
        }

        // Check to see if the changes list is valid.
        public bool ChangesAreValid(ChoiceGroupData choicesAsRevised)
        {
            // We need to make sure that every choice from this remains in choicesAsRevised, though the choices may 
            // not be available, and the ordering and/or the text may have changed.
            if (!choicesAsRevised.ChoicesAreUnique())
                return false;
            foreach (ChoiceInGroupData theOriginalChoice in this.TheData)
            {
                if (choicesAsRevised.TheData.Where(revised => revised.choiceInGroupID == theOriginalChoice.choiceInGroupID).Count() != 1)
                    return false;
            }
            // We also will double check that no new choiceInGroupID's have been added.
            if (choicesAsRevised.TheData.Where(d => d.choiceInGroupID != null).Count() != this.Count)
                return false;

            return true;
        }
    }

    [Serializable()]
    public class ChoiceMenuItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    };



    public static class ChoiceMenuAccess
    {

        public static List<ChoiceMenuItem> GetChoiceMenuItemsForIndependentGroup(int choiceGroupID)
        {
            List<int> determiningGroupValues = new List<int>();
            R8RDataAccess Obj = new R8RDataAccess();
            ChoiceGroup theChoiceGroup = Obj.R8RDB.GetTable<ChoiceGroup>().Single(cg => cg.ChoiceGroupID == choiceGroupID);
            bool orderAlphabetically = theChoiceGroup.Alphabetize;
            return GetChoiceMenuItemsHelper(determiningGroupValues, choiceGroupID, orderAlphabetically);
        }

        public static string GetChoiceMenuItemsForDependentGroupAsHtml(string valueSelectedInDepender, string valueSelectedInDependee, List<int> availableOptionsInDependee, int choiceGroupID)
        {
            List<ChoiceMenuItem> theList = null;
            if (valueSelectedInDependee == "-1") // no selection
                theList = GetChoiceMenuItemsForDependentGroupWithNoDependentSelection(availableOptionsInDependee, choiceGroupID);
            else
                theList = GetChoiceMenuItemsForDependentGroupWithDependentSelection(Convert.ToInt32(valueSelectedInDependee), choiceGroupID);
            StringBuilder sb = new StringBuilder();
            sb.Append("<select>");
            foreach (var option in theList)
            {
                sb.Append("<option value=\"");
                sb.Append(option.Value);
                if (valueSelectedInDepender == option.Value)
                    sb.Append("\" selected=\"yes\"");
                sb.Append("\">");
                sb.Append(option.Text);
                sb.Append("</option>");
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public static List<ChoiceMenuItem> GetChoiceMenuItemsForDependentGroupWithDependentSelection(int determiningGroupValue, int choiceGroupID)
        {
            return GetChoiceMenuItemsForDependentGroupWithDependentSelections(new List<int> { determiningGroupValue }, choiceGroupID);
        }

        public static List<ChoiceMenuItem> GetChoiceMenuItemsForDependentGroupWithDependentSelections(List<int> determiningGroupValues, int choiceGroupID)
        {
            R8RDataAccess Obj = new R8RDataAccess();
            ChoiceGroup theChoiceGroup = Obj.R8RDB.GetTable<ChoiceGroup>().Single(cg => cg.ChoiceGroupID == choiceGroupID);
            bool orderAlphabetically = theChoiceGroup.Alphabetize;
            return GetChoiceMenuItemsHelper(determiningGroupValues, choiceGroupID, orderAlphabetically);
        }

        public static List<ChoiceMenuItem> GetChoiceMenuItemsForDependentGroupWithNoDependentSelection(List<int> availableOptionsInDependee, int choiceGroupID)
        {
            R8RDataAccess Obj = new R8RDataAccess();
            ChoiceGroup theChoiceGroup = Obj.R8RDB.GetTable<ChoiceGroup>().Single(cg => cg.ChoiceGroupID == choiceGroupID);
            bool showAllPossibilities = theChoiceGroup.ShowAllPossibilitiesIfNoDependentChoice;
            if (showAllPossibilities && availableOptionsInDependee != null && availableOptionsInDependee.Count() > 100)
                showAllPossibilities = false;
            if (showAllPossibilities)
            {
                bool orderAlphabetically = theChoiceGroup.AlphabetizeWhenShowingAllPossibilities;
                if (availableOptionsInDependee == null)
                    availableOptionsInDependee = new List<int>();
                return GetChoiceMenuItemsHelper(availableOptionsInDependee, choiceGroupID, orderAlphabetically);
            }
            else
            {
                bool orderAlphabetically = theChoiceGroup.Alphabetize;
                return GetChoiceMenuItemsHelper(new List<int>(), choiceGroupID, orderAlphabetically);
            }
        }

        public static List<ChoiceMenuItem> GetChoiceMenuItemsHelper(List<int> determiningGroupValues, int choiceGroupID, bool orderAlphabetically)
        {
            if (determiningGroupValues == null)
                determiningGroupValues = new List<int>();
            R8RDataAccess Obj = new R8RDataAccess();
            var myPredicateAllowableDeterminingGroupChoices = PredicateBuilder.False<ChoiceInGroup>();

            var myPredicate = PredicateBuilder.True<ChoiceInGroup>();
            myPredicate = myPredicate.And(x => x.ChoiceGroupID == choiceGroupID && x.Status == (Byte)(StatusOfObject.Active));
            myPredicate = myPredicate.And(x =>
                                            x.ActiveOnDeterminingGroupChoiceInGroupID == null ||
                                            determiningGroupValues.Contains((int)x.ActiveOnDeterminingGroupChoiceInGroupID)
                                            );

            List<ChoiceMenuItem> noChoiceList = new List<ChoiceMenuItem> { new ChoiceMenuItem { Text = "", Value = "-1" } };
            List<ChoiceMenuItem> theChoices = new List<ChoiceMenuItem>();
            if (orderAlphabetically)
            {
                var temp = Obj.R8RDB.GetTable<ChoiceInGroup>()
                 .Where(myPredicate);
                if (temp.Any())
                    theChoices = temp
                         .OrderBy(x => x.ChoiceText)
                         .Select(x => new ChoiceMenuItem { Text = x.ChoiceText, Value = x.ChoiceInGroupID.ToString() })
                         .ToList();
            }
            else
            {
                var temp = Obj.R8RDB.GetTable<ChoiceInGroup>()
                 .Where(myPredicate);
                if (temp.Any())
                    theChoices = temp
                         .OrderBy(x => x.ChoiceNum)
                         .ThenBy(x => x.ChoiceText)
                         .Select(x => new ChoiceMenuItem { Text = x.ChoiceText, Value = x.ChoiceInGroupID.ToString() })
                         .ToList();
            }
            List<ChoiceMenuItem> dropDownItems;
            if (theChoices.Any())
                dropDownItems = (noChoiceList.Concat(theChoices)).ToList();
            else
                dropDownItems = (List<ChoiceMenuItem>)noChoiceList;
            return dropDownItems;
        }

    }

    //public static class ChoiceVerification
    //{
    //    public bool ChoiceVerifiesOK(ChoiceGroup theChoiceGroup, ChoiceGroup theChoiceGroupThisDependsOn, int theChoice)
    //    {

    //    }

    //    public bool ChoicesForFieldDefinitionVerifyOK(int entityID, int FieldDefinitionID, List<int> multipleChoices)
    //    {
    //        try
    //        {
    //            ChoiceGroupFieldDefinition theCGFD = R8RDB.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.FieldDefinitionID == FieldDefinitionID);
    //            ChoiceGroup theChoiceGroup = theCGFD.ChoiceGroup;
    //            ChoiceGroupFieldDefinition theCGFDThisDependsOn = null;
    //            ChoiceGroup theChoiceGroupThisDependsOn = null;
    //            if (theCGFD.DependentOnChoiceGroupFieldDefinitionID != null)
    //            {
    //                theCGFDThisDependsOn = R8RDB.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.FieldDefinitionID == theCGFD.DependentOnChoiceGroupFieldDefinitionID);
    //                theChoiceGroupThisDependsOn = theCGFDThisDependsOn.ChoiceGroup;
    //            }
    //            foreach (var choice in multipleChoices)
    //                if (!ChoiceVerifiesOK(theChoiceGroup, theChoiceGroupThisDependsOn, choice))
    //                    return false;
    //            return true;
    //        }
    //        catch
    //        {
    //            throw new Exception("Some selected choices are not available in the database.");
    //        }
    //    }

    //    public bool ChoicesForFieldNumVerifyOK(int entityID, int fieldNum, List<int> multipleChoices)
    //    {
    //        FieldDefinition theFieldDefinition = DataAccessModule.GetFieldDefinitionForTblRow(entityID, fieldNum);
    //        return ChoicesForFieldDefinitionVerifyOK(entityID, theFieldDefinition.FieldDefinitionID, multipleChoices);
    //    }
    //}
}