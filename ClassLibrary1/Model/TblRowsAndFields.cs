using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;


using System.Text;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public enum FieldsBoxMode
    {
        filterWithButton,
        filterWithoutButton,
        modifyFields,
        addTblRow
    }
    [Serializable()]
    public class TblVal
    {
        public string TblCol;
        public int NumInGroup;
        public decimal CurrentValue;
    }

    public class FieldSetDataInfo
    {
        public TblRow theTblRow { get; set; }
        public Tbl theTbl { get; set; }
        public RaterooDataAccess DataAccess { get; set; }
        public string theEntityName { get; set; }
        public List<FieldDataInfo> theFieldDataInfos { get; set; }
        public List<FieldDefinition> theEmptyFieldDefinitions { get; set; }
        public List<TblVal> defaultTblVals { get; set; }
        internal bool isVerified = false;

        public FieldSetDataInfo(TblRow entity, Tbl Tbl, RaterooDataAccess dataAccess)
        {
            DataAccess = dataAccess;
            theTblRow = entity;
            theTbl = Tbl;
            if (theTbl == null && theTblRow == null)
                throw new Exception("FieldSetDataInfo: TblID or entityID must be specified.");
            if (theTbl == null)
            {
                theTbl = theTblRow.Tbl;
            }
            else
            {
                if (theTblRow != null && theTbl != theTblRow.Tbl && theTbl.TblID != theTblRow.Tbl.TblID)
                    throw new Exception("FieldSetDataInfo: TblID and entityID do not match.");
            }
            theFieldDataInfos = new List<FieldDataInfo>();
            theEmptyFieldDefinitions = new List<FieldDefinition>();
        }

        public void AddFieldDataInfo(FieldDataInfo theFieldData)
        {
            theFieldDataInfos.Add(theFieldData);
            isVerified = false;
        }

        public void LoadFromDatabase()
        {
            theEntityName = theTblRow.Name;
            List<FieldDefinition> FieldDefinitions = DataAccess.RaterooDB.GetTable<FieldDefinition>().Where(fd => fd.TblID == theTblRow.TblID).OrderBy(fd => fd.FieldNum).ThenBy(fd => fd.FieldName).ToList();
            foreach (var FieldDefinition in FieldDefinitions)
            {
                switch (FieldDefinition.FieldType)
                {
                    case (int) FieldTypes.AddressField:
                        AddressFieldDataInfo theInfo = new AddressFieldDataInfo(FieldDefinition, "", this, DataAccess);
                        if (theInfo.LoadFromDatabase())
                            AddFieldDataInfo(theInfo);
                        break;

                    case (int)FieldTypes.ChoiceField:
                        ChoiceFieldDataInfo theInfo2 = new ChoiceFieldDataInfo(FieldDefinition, new List<ChoiceInGroup>(), this, DataAccess);
                        if (theInfo2.LoadFromDatabase())
                            AddFieldDataInfo(theInfo2);
                        break;

                    case (int)FieldTypes.DateTimeField:
                        DateTimeFieldDataInfo theInfo3 = new DateTimeFieldDataInfo(FieldDefinition, TestableDateTime.Now, this, DataAccess);
                        if (theInfo3.LoadFromDatabase())
                            AddFieldDataInfo(theInfo3);
                        break;

                    case (int)FieldTypes.NumberField:
                        NumericFieldDataInfo theInfo4 = new NumericFieldDataInfo(FieldDefinition, 0, this, DataAccess);
                        if (theInfo4.LoadFromDatabase())
                            AddFieldDataInfo(theInfo4);
                        break;

                    case (int)FieldTypes.TextField:
                        TextFieldDataInfo theInfo5 = new TextFieldDataInfo(FieldDefinition, "", "", this, DataAccess);
                        if (theInfo5.LoadFromDatabase())
                            AddFieldDataInfo(theInfo5);
                        break;

                    default:
                        break;
                }
            }
        }

        public string GetComparison(FieldSetDataInfo oldSettings)
        {
            StringBuilder myBuilder = new StringBuilder();
            myBuilder.Append("Changes to " + oldSettings.theEntityName + "<br>");
            bool nameChanged = false;
            if (theEntityName != oldSettings.theEntityName)
            {
                nameChanged = true;
                myBuilder.Append("New name: " + theEntityName + "<br>");
            }
            var changedFields = theFieldDataInfos
                .Where(x =>
                        oldSettings.theFieldDataInfos.Any(
                            y => y.TheFieldDefinition == x.TheFieldDefinition
                            && y.GetDescription() != x.GetDescription()
                            && y.GetDescription() != ""
                            && x.GetDescription() != ""))
                .Select(x =>
                    new
                    {
                        FieldDefinition = x.TheFieldDefinition,
                        Description = "Changed " + x.TheFieldDefinition.FieldName + ":<br>Before -- " + oldSettings.theFieldDataInfos.Single(y => y.TheFieldDefinition == x.TheFieldDefinition).GetDescription() + "<br>After -- " + x.GetDescription()
                    });
            var addedFields = theFieldDataInfos
                .Where(x =>
                        !oldSettings.theFieldDataInfos.Any(
                            y => y.TheFieldDefinition == x.TheFieldDefinition)
                            && x.GetDescription() != ""
                            )
                .Select(x =>
                    new
                    {
                        FieldDefinition = x.TheFieldDefinition,
                        Description = "Added " + x.TheFieldDefinition.FieldName + ":<br>" + x.GetDescription()
                    });
            var deletedFields = oldSettings.theFieldDataInfos
                .Where(y =>
                        !theFieldDataInfos.Any(
                            x => x.TheFieldDefinition == y.TheFieldDefinition)
                            && y.GetDescription() != "")
                .Select(y =>
                    new
                    {
                        FieldDefinition = y.TheFieldDefinition,
                        Description = "Deleted " + y.TheFieldDefinition.FieldName + ":<br>" + y.GetDescription()
                    });
            var allChanges = changedFields
                .Concat(addedFields)
                .Concat(deletedFields)
                .OrderBy(x => x.FieldDefinition.FieldNum)
                .ThenBy(x => x.FieldDefinition.FieldName);

            if (!allChanges.Any() && !nameChanged)
                myBuilder.Append("No changes made.");
            else
            {
                foreach (var theChange in allChanges)
                {
                    myBuilder.Append(theChange.Description);
                    myBuilder.Append("<br>");
                }
            }
            return myBuilder.ToString();
        }

        public string GetDescription()
        {
            StringBuilder myBuilder = new StringBuilder();
            myBuilder.Append("Name: " + theEntityName);
            foreach (var fieldDataInfo in theFieldDataInfos)
            {
                string theString = fieldDataInfo.GetDescription();
                if (theString != "")
                {
                    myBuilder.Append("<br>");
                    myBuilder.Append(fieldDataInfo.TheFieldDefinition.FieldName);
                    myBuilder.Append(": ");
                    myBuilder.Append(theString);
                }
            }
            return myBuilder.ToString();
        }

        public void IdentifyEmptyFields(bool activeFieldDefinitionsOnly)
        {
            IQueryable<FieldDefinition> theFieldDefinitions = theTbl.FieldDefinitions.AsQueryable();
            if (activeFieldDefinitionsOnly)
                theFieldDefinitions = theFieldDefinitions.Where(x => x.Status == (int) StatusOfObject.Active);
            foreach (FieldDefinition theFieldDefinition in theFieldDefinitions)
            {
                int theCount = theFieldDataInfos.Where(fdi => fdi.TheFieldDefinition == theFieldDefinition).Count();
                if (theCount == 0)
                    theEmptyFieldDefinitions.Add(theFieldDefinition);
                if (theCount > 1)
                    throw new Exception("A field has been included in the FieldSetDataInfo more than once.");
            }
        }

        public void VerifyCanBeAdded()
        {
            if (!isVerified)
            {
                theFieldDataInfos.ForEach(x => x.VerifyCanBeAdded());
                isVerified = true;
            }
        }
    }

    public abstract class FieldDataInfo
    {
        public FieldDefinition TheFieldDefinition { get; set; }
        public FieldSetDataInfo TheGroup { get; protected set; }
        internal RaterooDataAccess DataAccess;
        public bool descriptionLoaded = false;
        public string theDescription = "";

        public FieldDataInfo(FieldDefinition theFieldDefinition, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
        {
            TheGroup = theGroup;
            DataAccess = theDataAccess;
            TheFieldDefinition = theFieldDefinition;
        }

        public virtual void VerifyCanBeAdded()
        {
            if (TheFieldDefinition == null)
                throw new Exception("FieldDefinition is invalid");
            if (TheFieldDefinition.Tbl.TblID != TheGroup.theTbl.TblID)
                throw new Exception("FieldDefinition is set to the wrong Tbl");
        }

        public abstract bool MatchesDatabase();

        public abstract bool LoadFromDatabase();

        public virtual string GetDescription()
        {
            return "";
        }
    }

    public class AddressFieldDataInfo : FieldDataInfo
    {
        public string TheAddress { get; set; }
        public AddressFieldDataInfo(FieldDefinition FieldDefinition, string theAddress, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            TheAddress = theAddress;
        }

        public override bool LoadFromDatabase()
        {
            AddressField theField = DataAccess.RaterooDB.GetTable<AddressField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
                return false;
            else
                TheAddress = theField.AddressString;
            return true;
        }

        public override bool MatchesDatabase()
        {
            AddressField theField = null;
            if (TheGroup.theTblRow.TblRowID != 0)
                theField = DataAccess.RaterooDB.GetTable<AddressField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int) StatusOfObject.Active);
            if (theField == null)
                return (TheAddress == "");
            else
                return (theField.AddressString == TheAddress);
        }

        public override string GetDescription()
        {
            if (descriptionLoaded)
                return theDescription;
            descriptionLoaded = true;
            theDescription = base.GetDescription() + TheAddress;
            return theDescription;
        }
    }

    public class DateTimeFieldDataInfo : FieldDataInfo
    {
        public DateTime TheDateTime { get; set; }
        public DateTimeFieldDataInfo(FieldDefinition FieldDefinition, DateTime theDateTime, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            TheDateTime = theDateTime;
        }

        public override bool LoadFromDatabase()
        {
            DateTimeField theField = DataAccess.RaterooDB.GetTable<DateTimeField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField != null && theField.DateTime != null)
            {
                TheDateTime = (DateTime)theField.DateTime;
                return true;
            }
            return false;
        }

        public override bool MatchesDatabase()
        {
            DateTimeField theField = null;
            if (TheGroup.theTblRow.TblRowID != 0)
                theField = DataAccess.RaterooDB.GetTable<DateTimeField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
                return (TheDateTime == null);
            else
                return (theField.DateTime == TheDateTime);
        }

        public override string GetDescription()
        {
            if (descriptionLoaded)
                return theDescription;
            descriptionLoaded = true;
            theDescription = base.GetDescription() + TheDateTime.ToShortDateString();
            return theDescription;
        }
    }
    
    public class TextFieldDataInfo : FieldDataInfo
    {
        public string TheText { get; set; }
        public string TheLink { get; set; }
        public TextFieldDataInfo(FieldDefinition FieldDefinition, string theText, string theLink, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            TheText = theText;
            TheLink = theLink;
        }

        public override bool LoadFromDatabase()
        {
            TextField theField = DataAccess.RaterooDB.GetTable<TextField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
            {
                return false;
            }
            else
            {
                TheText = theField.Text;
                TheLink = theField.Link;
                return true;
            }
        }

        public override bool MatchesDatabase()
        {
            TextField theField = null;
            if (TheGroup.theTblRow.TblRowID != 0)
                theField = DataAccess.RaterooDB.GetTable<TextField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
                return (TheText == "" && TheLink == "");
            else /* Note that if we already have html in the database and that html is not changed, then we want to leave the database alone. */
                return ((theField.Text == TheText && theField.Link == TheLink) || (MoreStringManip.StripHtml(theField.Text) == TheText && theField.Link == TheLink));
        }

        public override string GetDescription()
        {
            if (descriptionLoaded)
                return theDescription;
            descriptionLoaded = true;
            theDescription = base.GetDescription();
            if (TheText != "")
                theDescription += TheText + " ";
            if (TheLink != "")
                theDescription += "Link " + TheLink;
            return theDescription;
        }
    }

    public class NumericFieldDataInfo : FieldDataInfo
    {
        public decimal TheNumber { get; set; }
        public NumericFieldDataInfo(FieldDefinition FieldDefinition, decimal theNumber, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            TheNumber = theNumber;
        }

        public override void VerifyCanBeAdded()
        {
            base.VerifyCanBeAdded();
            string key = "NFD" + TheFieldDefinition.GetHashCode();
            NumberFieldDefinition theNFD = DataAccess.RaterooDB.TempCacheGet(key) as NumberFieldDefinition;
            if (theNFD == null)
            {
                theNFD = DataAccess.RaterooDB.GetTable<NumberFieldDefinition>().SingleOrDefault(nfd => nfd.FieldDefinition == TheFieldDefinition);
                DataAccess.RaterooDB.TempCacheAdd(key, theNFD);
            }
            if (theNFD == null)
                throw new Exception("The number field descriptor is missing.");
            if (theNFD.Minimum != null && TheNumber < theNFD.Minimum)
                throw new Exception("The number is set too low.");
            if (theNFD.Maximum != null && TheNumber > theNFD.Maximum)
                throw new Exception("The number is set too high.");
        }

        public override bool LoadFromDatabase()
        {
            NumberField theField = DataAccess.RaterooDB.GetTable<NumberField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null || theField.Number == null)
                return false;
            else
                TheNumber = (decimal) theField.Number;
            return true;
        }

        public override bool MatchesDatabase()
        {
            NumberField theField = null;
            if (TheGroup.theTblRow.TblRowID != 0)
                theField = DataAccess.RaterooDB.GetTable<NumberField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
                return (false);
            else
                return (theField.Number == TheNumber);
        }

        public override string GetDescription()
        {
            if (descriptionLoaded)
                return theDescription;
            descriptionLoaded = true;
            theDescription = base.GetDescription() + TheNumber.ToString();
            return theDescription;
        }
    }

    public class ChoiceFieldDataInfo : FieldDataInfo
    {
        public List<ChoiceInGroup> TheChoices { get; set; }
        public ChoiceGroupFieldDefinition theCGFD { get; set; }

        internal void Setup()
        {
            string key = "CGFD" + TheFieldDefinition.GetHashCode();
            theCGFD = DataAccess.RaterooDB.TempCacheGet(key) as ChoiceGroupFieldDefinition;
            if (theCGFD == null)
            {
                theCGFD = DataAccess.RaterooDB.GetTable<ChoiceGroupFieldDefinition>().SingleOrDefault(cgfd => cgfd.FieldDefinition == TheFieldDefinition);
                DataAccess.RaterooDB.TempCacheAdd(key, theCGFD);
            }
            if (theCGFD == null)
                throw new Exception("The choice group field descriptor is missing.");
        }

        public ChoiceInGroup GetChoiceInGroupIDForChoiceText(string theChoiceInGroupText)
        { // Note: Assumes Setup() has already been called.
            string key = "CIG" + theCGFD.ChoiceGroupID + theChoiceInGroupText;
            List<ChoiceInGroup> candidateChoiceInGroups = DataAccess.RaterooDB.TempCacheGet(key) as List<ChoiceInGroup>;
            if (candidateChoiceInGroups == null)
            {
                candidateChoiceInGroups = DataAccess.RaterooDB.GetTable<ChoiceInGroup>().Where(cig => cig.ChoiceGroupID == theCGFD.ChoiceGroupID && cig.ChoiceText == theChoiceInGroupText).ToList(); // must convert to list for code below to work
                DataAccess.RaterooDB.TempCacheAdd(key, candidateChoiceInGroups);
            }
            
            if (!candidateChoiceInGroups.Any())
                throw new Exception("No matches for " + theChoiceInGroupText);

            if (theCGFD.DependentOnChoiceGroupFieldDefinitionID == null)
            {
                if (candidateChoiceInGroups.Count() == 1)
                    return candidateChoiceInGroups.First();
                else
                    throw new Exception("There was more than one match for the specified text: " + theChoiceInGroupText);
            }
            else
            {
                ChoiceFieldDataInfo theChoiceFieldDataThisDependsOn = null;
                theChoiceFieldDataThisDependsOn = (ChoiceFieldDataInfo)TheGroup.theFieldDataInfos.SingleOrDefault(x => x is ChoiceFieldDataInfo && ((ChoiceFieldDataInfo)x).theCGFD.ChoiceGroupFieldDefinitionID == theCGFD.DependentOnChoiceGroupFieldDefinitionID);
                IEnumerable<ChoiceInGroup> remainingCandidates = null;
                if (theChoiceFieldDataThisDependsOn == null)
                { // No dependent choice group is listed, so we restrict ourselves to choices that are not dependent.
                    remainingCandidates = candidateChoiceInGroups.Where(c =>
                        c.ActiveOnDeterminingGroupChoiceInGroupID == null);
                }
                else
                {
                    // Count those candidates whose activeondetermininggroupchoiceingroupid is null or in theCHoiceFieldDataThisDependsOn
                    remainingCandidates = candidateChoiceInGroups.Where(c =>
                        c.ActiveOnDeterminingGroupChoiceInGroupID == null ||
                        theChoiceFieldDataThisDependsOn.TheChoices.Where(x => x.ChoiceInGroupID == c.ActiveOnDeterminingGroupChoiceInGroupID).Any());
                }
                if (remainingCandidates.Count() == 0)
                    throw new Exception("There was no suitable match for the text " + theChoiceInGroupText);
                else if (remainingCandidates.Count() > 1)
                    throw new Exception("There was more than one match for the specified text: " + theChoiceInGroupText);
                else
                    return remainingCandidates.First();
            }
        }

        public ChoiceFieldDataInfo(FieldDefinition FieldDefinition, List<ChoiceInGroup> theChoiceInGroups, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            Setup();
            TheChoices = theChoiceInGroups;
        }

        public ChoiceFieldDataInfo(FieldDefinition FieldDefinition, ChoiceInGroup theChoiceInGroup, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            Setup();
            TheChoices = new List<ChoiceInGroup>();
            TheChoices.Add(theChoiceInGroup);
        }

        public ChoiceFieldDataInfo(FieldDefinition FieldDefinition, List<string> theChoiceInGroupStrings, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            Setup();
            TheChoices = new List<ChoiceInGroup>();
            foreach (string theChoiceInGroupString in theChoiceInGroupStrings)
            {
                ChoiceInGroup theChoiceInGroup = GetChoiceInGroupIDForChoiceText(theChoiceInGroupString);
                TheChoices.Add(theChoiceInGroup);
            }
        }

        public ChoiceFieldDataInfo(FieldDefinition FieldDefinition, string theChoiceInGroupString, FieldSetDataInfo theGroup, RaterooDataAccess theDataAccess)
            : base(FieldDefinition, theGroup, theDataAccess)
        {
            Setup();
            TheChoices = new List<ChoiceInGroup>();
            TheChoices.Add(GetChoiceInGroupIDForChoiceText(theChoiceInGroupString));
        }

        public override void VerifyCanBeAdded()
        {
            base.VerifyCanBeAdded();
            ChoiceFieldDataInfo theChoiceFieldDataThisDependsOn = null;
            if (theCGFD.ChoiceGroupFieldDefinition1 != null)
            {
                theChoiceFieldDataThisDependsOn = (ChoiceFieldDataInfo) TheGroup.theFieldDataInfos.SingleOrDefault(x => x is ChoiceFieldDataInfo && ((ChoiceFieldDataInfo)x).theCGFD.ChoiceGroupFieldDefinitionID == theCGFD.ChoiceGroupFieldDefinition1.ChoiceGroupFieldDefinitionID);
                if (theChoiceFieldDataThisDependsOn == null)
                    throw new Exception("Dependent choice group not found."); 
                foreach (ChoiceInGroup theChoice in TheChoices)
                {
                    if (theChoice.ChoiceInGroup1 != null)
                    {
                        ChoiceInGroup theCorrespondingChoice = theChoiceFieldDataThisDependsOn.TheChoices.SingleOrDefault(c => c.ChoiceInGroupID == theChoice.ActiveOnDeterminingGroupChoiceInGroupID);
                        if (theCorrespondingChoice == null)
                            throw new Exception("A dependent choice group choice is inconsistent.");
                    }
                }  
            }
        }

        public override bool LoadFromDatabase()
        {
            ChoiceField theField = DataAccess.RaterooDB.GetTable<ChoiceField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
                return false;
            List<ChoiceInGroup> existingList = DataAccess.RaterooDB.GetTable<ChoiceInField>().Where(x => x.ChoiceFieldID == theField.ChoiceFieldID && x.Status == (int)StatusOfObject.Active).Select(x => x.ChoiceInGroup).OrderBy(x => x.ChoiceNum).ThenBy(x => x.ChoiceText).ToList();
            if (!existingList.Any())
                return false;
            TheChoices = existingList;
            return true;
        }

        public override bool MatchesDatabase()
        {
            List<ChoiceInGroup> thisListOrdered = TheChoices.OrderBy(x => x.ChoiceInGroupID).ToList();
            ChoiceField theField = null;
            if (TheGroup.theTblRow.TblRowID != 0)
                theField = DataAccess.RaterooDB.GetTable<ChoiceField>().SingleOrDefault(x => x.Field.TblRow == TheGroup.theTblRow && x.Field.FieldDefinition == TheFieldDefinition && x.Status == (int)StatusOfObject.Active);
            if (theField == null)
                return (thisListOrdered.Count() == 0);
            else
            {
                List<ChoiceInGroup> existingList = DataAccess.RaterooDB.GetTable<ChoiceInField>().Where(x => x.ChoiceFieldID == theField.ChoiceFieldID && x.Status == (int)StatusOfObject.Active).Select(x => x.ChoiceInGroup).OrderBy(x => x.ChoiceInGroupID).ToList();
                return (existingList.SequenceEqual(thisListOrdered));
            }
        }

        public override string GetDescription()
        {
            if (descriptionLoaded)
                return theDescription;
            descriptionLoaded = true;
            if (TheChoices.Count == 0)
                return "";
            theDescription = base.GetDescription();
            List<string> descriptions = DataAccess.RaterooDB.GetTable<ChoiceInGroup>().Where(x => TheChoices.Contains(x)).Select(x => x.ChoiceText).OrderBy(x => x).ToList();
            for (int i = 0; i < TheChoices.Count; i++)
            {
                theDescription += descriptions[i];
                if (i != TheChoices.Count - 1)
                    theDescription += ", ";
            }
            return theDescription;
        }
    }


    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        // Methods related to entities and fields.

        /// <summary>
        /// Returns a field descriptor for a specified entity and field number. 
        /// </summary>
        /// <param name="entityID">The entity</param>
        /// <param name="fieldNum">The field number</param>
        /// <returns>The field descriptor</returns>
        //public FieldDefinition GetFieldDefinitionForTblRow(int? entityID, int fieldNum)
        //{
        //    throw new Exception("This is no longer supported.");
        //    TblRow theTblRow = RaterooDB.GetTable<TblRow>().SingleOrDefault(e => e.TblRowID == entityID);
        //    if (theTblRow == null)
        //        return null;
        //    int TblID = theTblRow.TblID;
        //    FieldDefinition theFieldDefinition = RaterooDB.GetTable<FieldDefinition>().SingleOrDefault(fd => fd.TblID == TblID && fd.FieldNum == fieldNum);
        //    if (theFieldDefinition == null)
        //        return null;
        //    return theFieldDefinition;
        //}


        public TypeOfObject GetObjectTypeForFieldType(FieldTypes theFieldType)
        {
            switch (theFieldType)
            {
                case FieldTypes.AddressField:
                    return TypeOfObject.AddressField;
                case FieldTypes.ChoiceField:
                    return TypeOfObject.ChoiceField;
                case FieldTypes.DateTimeField:
                    return TypeOfObject.DateTimeField;
                case FieldTypes.NumberField:
                    return TypeOfObject.NumberField;
                case FieldTypes.TextField:
                    return TypeOfObject.TextField;
                default:
                    throw new Exception("Unknown field type to be converted to object type.");
            }
        }

        public Field GetFieldForTblRow(TblRow entity, FieldDefinition FieldDefinition)
        {
            return DataContext.GetTable<Field>().SingleOrDefault(f => f.TblRow == entity && f.FieldDefinition == FieldDefinition && f.Status == (byte)StatusOfObject.Active);
        }

        public Field GetFieldForTblRow(int? entityID, int? FieldDefinitionID)
        {
            return DataContext.GetTable<Field>().SingleOrDefault(f => f.TblRowID == entityID && f.FieldDefinitionID == FieldDefinitionID && f.Status == (byte)StatusOfObject.Active);
        }

        public void GetFieldForTblRow(int? entityID, int? FieldDefinitionID, ref int? theFieldID, ref int? theSubfieldID, ref FieldTypes theFieldType)
        {
            theFieldID = null;
            theSubfieldID = null;
            if (entityID == null || FieldDefinitionID == null)
                return;

            var theField = DataContext.GetTable<Field>().SingleOrDefault(f => f.TblRowID == entityID && f.FieldDefinitionID == FieldDefinitionID && f.Status == (byte)StatusOfObject.Active);
            if (theField != null)
                theFieldID = theField.FieldID;


            if (theFieldID != null)
            {
                theFieldType = (FieldTypes)DataContext.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == FieldDefinitionID).FieldType;
                int theFieldIDCopy = (int)theFieldID; // so we can use ref parameter in lambda expression

                if (theFieldType == FieldTypes.AddressField)
                {
                    AddressField theSubfield = DataContext.GetTable<AddressField>().SingleOrDefault(sf => sf.FieldID == theFieldIDCopy && (sf.Status == (int) StatusOfObject.Active));
                    if (theSubfield != null)
                        theSubfieldID = theSubfield.AddressFieldID;
                }
                else if (theFieldType == FieldTypes.ChoiceField)
                {
                    ChoiceField theSubfield = DataContext.GetTable<ChoiceField>().SingleOrDefault(sf => sf.FieldID == theFieldIDCopy && (sf.Status == (int)StatusOfObject.Active));
                    if (theSubfield != null)
                        theSubfieldID = theSubfield.ChoiceFieldID;
                }
                else if (theFieldType == FieldTypes.DateTimeField)
                {
                    DateTimeField theSubfield = DataContext.GetTable<DateTimeField>().SingleOrDefault(sf => sf.FieldID == theFieldIDCopy && (sf.Status == (int)StatusOfObject.Active));
                    if (theSubfield != null)
                        theSubfieldID = theSubfield.DateTimeFieldID;
                }
                else if (theFieldType == FieldTypes.NumberField)
                {
                    NumberField theSubfield = DataContext.GetTable<NumberField>().SingleOrDefault(sf => sf.FieldID == theFieldIDCopy && (sf.Status == (int)StatusOfObject.Active));
                    if (theSubfield != null)
                        theSubfieldID = theSubfield.NumberFieldID;
                }
                else if (theFieldType == FieldTypes.TextField)
                {
                    TextField theSubfield = DataContext.GetTable<TextField>().SingleOrDefault(sf => sf.FieldID == theFieldIDCopy && (sf.Status == (int)StatusOfObject.Active));
                    if (theSubfield != null)
                        theSubfieldID = theSubfield.TextFieldID;
                }
                else
                    throw new Exception("Unknown field type.");
            }
        }

        public void GetFieldForTblRow(TblRow entity, int? FieldDefinitionID, ref Field theField, ref object theSubfield, ref FieldTypes theFieldType)
        {
            theField = null;
            theSubfield = null;
            if (entity == null || FieldDefinitionID == null)
                return;

            theField = DataContext.NewOrSingleOrDefault<Field>(f => f.TblRow == entity && f.FieldDefinitionID == FieldDefinitionID && f.Status == (byte)StatusOfObject.Active);


            if (theField != null)
            {
                theFieldType = (FieldTypes)DataContext.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == FieldDefinitionID).FieldType;
                Field theFieldCopy = theField; // so we can use ref parameter in lambda expression

                if (theFieldType == FieldTypes.AddressField)
                {
                    theSubfield = DataContext.NewOrSingleOrDefault<AddressField>(sf => sf.Field == theFieldCopy && (sf.Status == (int)StatusOfObject.Active));
                }
                else if (theFieldType == FieldTypes.ChoiceField)
                {
                     theSubfield = DataContext.NewOrSingleOrDefault<ChoiceField>(sf => sf.Field == theFieldCopy && (sf.Status == (int)StatusOfObject.Active));
                }
                else if (theFieldType == FieldTypes.DateTimeField)
                {
                    theSubfield = DataContext.NewOrSingleOrDefault<DateTimeField>(sf => sf.Field == theFieldCopy && (sf.Status == (int)StatusOfObject.Active));
                }
                else if (theFieldType == FieldTypes.NumberField)
                {
                     theSubfield = DataContext.NewOrSingleOrDefault<NumberField>(sf => sf.Field == theFieldCopy && (sf.Status == (int)StatusOfObject.Active));
                }
                else if (theFieldType == FieldTypes.TextField)
                {
                     theSubfield = DataContext.NewOrSingleOrDefault<TextField>(sf => sf.Field == theFieldCopy && (sf.Status == (int)StatusOfObject.Active));
                }
                else
                    throw new Exception("Unknown field type.");
            }
        }

        public void ResetTblRowFieldDisplay(TblRow theTblRow)
        {
            theTblRow.TblRowFieldDisplay.ResetNeeded = true;
        }

        public void ResetTblRowFieldDisplay(Tbl theTbl)
        {
            string alreadyReset = DataContext.TempCacheGet("TblRowFieldDisplayResetForTbl" + theTbl.TblID) as string;
            if (!(alreadyReset == "Yes"))
            {
                int highestIDCompleted = 0;
                bool moreWorkToDo = true;
                while (moreWorkToDo)
                {
                    var theTblRowFieldDisplays = DataContext.GetTable<TblRowFieldDisplay>().OrderBy(x => x.TblRowFieldDisplayID).Where(e => e.TblRowFieldDisplayID > highestIDCompleted &&
 e.TblRows.First().Tbl == theTbl && !e.ResetNeeded).Take(5000);
                    moreWorkToDo = theTblRowFieldDisplays.Any();
                    if (moreWorkToDo)
                    {
                        foreach (var theTblRowFieldDisplay in theTblRowFieldDisplays)
                        {
                            theTblRowFieldDisplay.ResetNeeded = true;
                            if (theTblRowFieldDisplay.TblRowFieldDisplayID > highestIDCompleted)
                                highestIDCompleted = theTblRowFieldDisplay.TblRowFieldDisplayID;
                        }
                        DataContext.SubmitChanges();
                    }
                }
                DataContext.TempCacheAdd("TblRowFieldDisplayResetForTbl" + theTbl.TblID, "Yes");
            }
        }

        public bool RespondToResetTblRowFieldDisplays()
        {
            if (DataContext.GetTable<TblRowFieldDisplay>().Any(x => x.ResetNeeded))
            {
                FieldsDisplayCreator theCreator = new FieldsDisplayCreator();
                theCreator.SetFieldDisplayHtmlForSomeNeedingResetting();
                return true;
            }
            return false;
        }
      
    }
}
