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
using System.Diagnostics;
////using PredRatings;

using System.Web.Profile;
using System.Text.RegularExpressions;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
    {
        // Methods for adding objects to database

        /// <summary>
        /// Adds an object representing a numeric field for a particular tblRow
        /// </summary>
        /// <param name="fieldID">The field (which corresponds in turn to a particular tblRow)</param>
        /// <param name="Address">The Address to which this field should be set</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of this object</returns>
        public AddressField AddAddressField(Field field, string addressString, decimal? latitude, decimal? longitude)
        {
            DateTime? lastGeocode = null;
            if (latitude == null)
                longitude = null;
            if (longitude == null)
                latitude = null;
            if (latitude == null && longitude == null && addressString.Contains("#LAT#") && addressString.Contains("#LNG#"))
            {
                string originalAddressString = addressString;
                addressString = Regex.Replace(addressString,",","QQQQQQ");
                addressString = Regex.Replace(addressString,"#LAT#",",");
                addressString = Regex.Replace(addressString,"#LNG#",",");
                string[] theComponents = addressString.Split(',');
                if (theComponents.Count() == 3)
                {
                    latitude = Convert.ToDecimal(theComponents[1]);
                    longitude = Convert.ToDecimal(theComponents[2]);
                    addressString = Regex.Replace(theComponents[0],"QQQQQQ",",");
                }
                else
                    addressString = originalAddressString;
            }
            if (latitude != null)
                lastGeocode = TestableDateTime.Now;
            AddressField theAddressField = new AddressField
            {
                AddressFieldID = Guid.NewGuid(),
                Field = field,
                AddressString = addressString,
                Latitude = latitude,
                Longitude = longitude,
                LastGeocode = lastGeocode,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<AddressField>().InsertOnSubmit(theAddressField);
            DataContext.RegisterObjectToBeInserted(theAddressField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + field.TblRowID);
            return theAddressField;
        }

        /// <summary>
        /// Add a table column to a table column group.
        /// </summary>
        /// <param name="TblTabID">table column group to add to</param>
        /// <param name="defaultRatingGroupAttributesID">The default rating group attributes for table rows in this column</param>
        /// <param name="columnNum">The order within the group (need not be unique)</param>
        /// <param name="abbreviation">An abbreviation for the column (used in tables)</param>
        /// <param name="name">The name of the column</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddTblColumn(Guid TblTabID, Guid defaultRatingGroupAttributesID, int columnNum, string abbreviation, string name, string widthStyle, string explanation, bool trackTrustWithinTableColumn)
        {
            TblColumn theTblColumn = new TblColumn
            {
                TblColumnID = Guid.NewGuid(),
                TblTabID = TblTabID,
                DefaultRatingGroupAttributesID = defaultRatingGroupAttributesID,
                TrustTrackerUnit = trackTrustWithinTableColumn ? AddTrustTrackerUnit() : null,
                CategoryNum = columnNum,
                Abbreviation = abbreviation,
                Name = name,
                Explanation = explanation,
                WidthStyle = widthStyle,
                WhenCreated = TestableDateTime.Now,
                NotYetAddedToDatabase = true,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<TblColumn>().InsertOnSubmit(theTblColumn);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("ColumnsForTblID" + DataContext.GetTable<TblTab>().Single(f => f.TblTabID == TblTabID).TblID);
            return theTblColumn.TblColumnID;
        }

        public Guid AddTblColumnFormatting(Guid TblColumnID, string prefix, string suffix, bool omitLeadingZero, decimal? extraDecimalPlaceAbove, decimal? extraDecimalPlace2Above, decimal? extraDecimalPlace3Above, string suppStylesHeader, string suppStylesMain)
        {
            TblColumnFormatting theTblColumnFormatting = new TblColumnFormatting
            {
                TblColumnFormattingID = Guid.NewGuid(),
                TblColumnID = TblColumnID,
                Prefix = prefix,
                Suffix = suffix,
                OmitLeadingZero = omitLeadingZero,
                ExtraDecimalPlaceAbove = extraDecimalPlaceAbove,
                ExtraDecimalPlace2Above = extraDecimalPlace2Above,
                ExtraDecimalPlace3Above = extraDecimalPlace3Above,
                SuppStylesHeader = suppStylesHeader ?? "",
                SuppStylesMain = suppStylesMain ?? "",
                Status = (Byte) StatusOfObject.Proposed
            };
            DataContext.GetTable<TblColumnFormatting>().InsertOnSubmit(theTblColumnFormatting);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("ColumnsForTblID" + DataContext.GetTable<TblColumn>().Single(f => f.TblColumnID == TblColumnID).TblTab.TblID);
            return theTblColumnFormatting.TblColumnFormattingID;
        }

        /// <summary>
        /// Adds a table column group. Create this, then call AddTblColumn.
        /// </summary>
        /// <param name="TblID">The Tbl to add to.</param>
        /// <param name="numInTbl">The number of the table column group (need not be unique).</param>
        /// <param name="name">The name of the table column group.</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddTblTab(Guid TblID, int numInTbl, string name)
        {
            TblTab theTblTab = new TblTab
            {
                TblTabID = Guid.NewGuid(),
                TblID = TblID,
                NumInTbl = numInTbl,
                Name = name,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<TblTab>().InsertOnSubmit(theTblTab);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("ColumnsForTblID" + TblID);
            return theTblTab.TblTabID;
        }

        /// <summary>
        /// Adds a changes group object
        /// </summary>
        /// <param name="pointsManagerID">The universe in which changes will occur (or the proposed new universe)</param>
        /// <param name="TblID">The already existing Tbl in which changes will occur (or null)</param>
        /// <param name="creator">The creator of the proposed changes</param>
        /// <param name="makeChangeRatingID">The rating, if any, that has the price determining whether changes will occur</param>
        /// <param name="rewardRatingID">The rating, if any, that determines the reward for the proposer</param>
        /// <param name="statusOfChanges">The status of the changes</param>
        /// <param name="scheduleApprovalOrRejection">If on track for approval or rejection, when</param>
        /// <param name="scheduleImplementation">If set for implementation, when</param>
        /// <returns></returns>
        public Guid AddChangesGroup(Guid? pointsManagerID, Guid? TblID, Guid? creator, Guid? makeChangeRatingID, Guid? rewardRatingID, StatusOfChanges statusOfChanges, DateTime? scheduleApprovalOrRejection, DateTime? scheduleImplementation)
        {
            DataContext.SubmitChanges();
           
            ChangesGroup theChangesGroup = new ChangesGroup
            {
                ChangesGroupID = Guid.NewGuid(),
                PointsManagerID = pointsManagerID,
                TblID = TblID,
                Creator = creator,
                MakeChangeRatingID = makeChangeRatingID,
                RewardRatingID = rewardRatingID,
                StatusOfChanges = (Byte)statusOfChanges,
                ScheduleApprovalOrRejection = scheduleApprovalOrRejection,
                ScheduleImplementation = scheduleImplementation
            };
            DataContext.GetTable<ChangesGroup>().InsertOnSubmit(theChangesGroup);
            DataContext.SubmitChanges();
            return theChangesGroup.ChangesGroupID;
        }

        /// <summary>
        /// Adds an object specifying a change to be made to the database
        /// </summary>
        /// <param name="changesGroupID">The group of changes of which this is a part</param>
        /// <param name="objectType">The type of object being affected</param>
        /// <param name="addObject">True to add the object</param>
        /// <param name="deleteObject">True to delete the object</param>
        /// <param name="replaceObject">True to replace the object</param>
        /// <param name="changeName">True to change the name of the object</param>
        /// <param name="changeOther">True to change something else about the object (such as a field)</param>
        /// <param name="newName">The new name or ""</param>
        /// <param name="newObject">The new object</param>
        /// <param name="existingObject">The existing object</param>
        /// <param name="newValueBoolean">The new Boolean value</param>
        /// <param name="newValueInteger">The new Integer value</param>
        /// <param name="newValueDecimal">The new Decimal value</param>
        /// <param name="newValueText">The new Text value</param>
        /// <returns></returns>
        /// 
        public Guid AddChangesStatusOfObject(Guid changesGroupID, TypeOfObject objectType, bool addObject, bool deleteObject, bool replaceObject, bool changeName, bool changeOther, bool changeSetting1, bool changeSetting2, bool mayAffectRunningRating, String newName, Guid? newObject, Guid? existingObject, bool? newValueBoolean, int? newValueInteger,  decimal? newValueDecimal, String newValueText, DateTime? newValueDateTime, Guid? newValueGuid = null)
        {
            return AddChangesStatusOfObject(changesGroupID, objectType, addObject, deleteObject, replaceObject, changeName, changeOther, changeSetting1, changeSetting2, mayAffectRunningRating, newName, newObject, existingObject, newValueBoolean, newValueInteger, newValueGuid, newValueDecimal, newValueText, newValueDateTime, "");
        }

        public Guid AddChangesStatusOfObject(Guid changesGroupID, TypeOfObject objectType, bool addObject, bool deleteObject, bool replaceObject, bool changeName, bool changeOther, bool changeSetting1, bool changeSetting2, bool mayAffectRunningRating, String newName, Guid? newObject, Guid? existingObject, bool? newValueBoolean, int? newValueInteger, Guid? newValueGuid, decimal? newValueDecimal, String newValueText, DateTime? newValueDateTime, string changeDescription)
        {
            ChangesStatusOfObject theChangesStatusOfObject = new ChangesStatusOfObject
            {
                ChangesStatusOfObjectID = Guid.NewGuid(),
                ChangesGroupID = changesGroupID,
                ObjectType = (Byte)objectType,
                AddObject = addObject,
                DeleteObject = deleteObject,
                ReplaceObject = replaceObject,
                ChangeName = changeName,
                ChangeOther = changeOther,
                MayAffectRunningRating = mayAffectRunningRating,
                ChangeSetting1 = changeSetting1,
                ChangeSetting2 = changeSetting2,
                NewName = newName,
                NewObject = newObject,
                ExistingObject = existingObject,
                NewValueBoolean = newValueBoolean,
                NewValueDecimal = newValueDecimal,
                NewValueInteger = newValueInteger,
                NewValueGuid = newValueGuid,
                NewValueText = newValueText,
                NewValueDateTime = newValueDateTime,
                ChangeDescription = changeDescription
            };
            DataContext.GetTable<ChangesStatusOfObject>().InsertOnSubmit(theChangesStatusOfObject);
            DataContext.SubmitChanges();
            return theChangesStatusOfObject.ChangesStatusOfObjectID;
        }

      


        /// <summary>
        /// Add a choice field. Note that the choice itself will be made with ChoiceInFields objects,
        /// which in turn point to ChoiceInGroup objects.
        /// </summary>
        /// <param name="fieldID">The field (for a particular tblRow) that is a choice field.</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public ChoiceField AddChoiceField(Field field)
        {
            ChoiceField theChoiceField = new ChoiceField
            {
                ChoiceFieldID = Guid.NewGuid(),
                Field = field,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<ChoiceField>().InsertOnSubmit(theChoiceField);
            DataContext.RegisterObjectToBeInserted(theChoiceField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + field.TblRowID);
            return theChoiceField;
        }

        /// <summary>
        /// Add a ChoiceGroupFieldDefinition, which contains more information to add to a FieldDefinition for a choice field.
        /// </summary>
        /// <param name="choiceGroupID">The choice group for this field.</param>
        /// <param name="FieldDefinitionID">The field definition to which information is being added.</param>
        /// <param name="DependentOnChoiceGroupFieldDefinitionID">Null, or the ID of another choice group that determines whether
        /// choices in this choice group are active</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddChoiceGroupFieldDefinition(Guid choiceGroupID, Guid FieldDefinitionID, Guid? DependentOnChoiceGroupFieldDefinitionID)
        {
            ChoiceGroupFieldDefinition theChoiceGroupFieldDefinition = new ChoiceGroupFieldDefinition
            {
                ChoiceGroupFieldDefinitionID = Guid.NewGuid(),
                ChoiceGroupID = choiceGroupID,
                FieldDefinitionID = FieldDefinitionID,
                DependentOnChoiceGroupFieldDefinitionID = DependentOnChoiceGroupFieldDefinitionID,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<ChoiceGroupFieldDefinition>().InsertOnSubmit(theChoiceGroupFieldDefinition);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + DataContext.GetTable<FieldDefinition>().Single(f => f.FieldDefinitionID == FieldDefinitionID).Tbl.PointsManagerID);
            return theChoiceGroupFieldDefinition.ChoiceGroupFieldDefinitionID;
        }

        public Guid AddChoiceGroup(Guid pointsManagerID, int choiceGroupSettings, Guid? DependentOnChoiceGroupID,
             String name)
        {
            return AddChoiceGroup(pointsManagerID,

                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.AllowMultipleSelections, choiceGroupSettings),
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.Alphabetize, choiceGroupSettings),
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.InvisibleWhenEmpty, choiceGroupSettings),
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.ShowTagCloud, choiceGroupSettings),
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.PickViaAutoComplete, choiceGroupSettings),
                DependentOnChoiceGroupID,
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.ShowAllPossibilitiesIfNoDependentChoice, choiceGroupSettings),
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.AlphabetizeWhenShowingAllPossibilities, choiceGroupSettings),
                ChoiceGroupSettingsMask.IsSet(ChoiceGroupSettingsMask.AllowAutoAddWhenAddingFields, choiceGroupSettings),
                name);
        }

        /// <summary>
        /// Add a choice group. After creating this, add ChoiceInGroups objects.
        /// </summary>
        /// <param name="allowMultipleSelections">True if multiple selections can be made.</param>
        /// <param name="name">The name of this choice group</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddChoiceGroup(Guid pointsManagerID, bool allowMultipleSelections, bool alphabetize, bool invisibleWhenEmpty,
            bool showTagCloud, bool pickViaAutoComplete, Guid? DependentOnChoiceGroupID, 
            bool showAllPossibilitiesIfNoDependentChoice, bool alphabetizeWhenShowingAllPossibilities, 
            bool allowAutoAddWhenAddingFields, String name)
        {
            ChoiceGroup theChoiceGroup = new ChoiceGroup
            {
                ChoiceGroupID = Guid.NewGuid(),
                PointsManagerID = pointsManagerID,
                AllowMultipleSelections = allowMultipleSelections,
                Alphabetize = alphabetize,
                InvisibleWhenEmpty = invisibleWhenEmpty,
                ShowTagCloud = showTagCloud,
                PickViaAutoComplete = pickViaAutoComplete,
                DependentOnChoiceGroupID = DependentOnChoiceGroupID,
                ShowAllPossibilitiesIfNoDependentChoice = showAllPossibilitiesIfNoDependentChoice,
                AlphabetizeWhenShowingAllPossibilities = alphabetizeWhenShowingAllPossibilities,
                AllowAutoAddWhenAddingFields = allowAutoAddWhenAddingFields,
                Name = name,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<ChoiceGroup>().InsertOnSubmit(theChoiceGroup);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
            return theChoiceGroup.ChoiceGroupID;
        }

        /// <summary>
        /// Add a ChoiceInField object (representing the identification of a choice made for a choice field for a particular tblRow).
        /// </summary>
        /// <param name="choiceFieldID">The choice field for which a choice is being made</param>
        /// <param name="choiceInGroupID">The selected choice from the choice group</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public ChoiceInField AddChoiceInField(ChoiceField choiceField, ChoiceInGroup choiceInGroup)
        {
            ChoiceInField theChoiceInField = new ChoiceInField
            {
                ChoiceInFieldID = Guid.NewGuid(),
                ChoiceField = choiceField,
                ChoiceInGroup = choiceInGroup,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<ChoiceInField>().InsertOnSubmit(theChoiceInField);
            DataContext.RegisterObjectToBeInserted(theChoiceInField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + choiceField.Field.TblRowID);
            return theChoiceInField;
        }

        /// <summary>
        /// Adds a choice to a group of choices that users may select for a field.
        /// </summary>
        /// <param name="choiceGroupID">The choice group to add to</param>
        /// <param name="choiceNum">The number of the choice (choices will be sorted by this)</param>
        /// <param name="choiceText">A textual description of the choice (e.g., "Maine" in a group of states)</param>
        /// <param name="activeInGeneral">True if this choice may be available for selection</param>
        /// <param name="ActiveOnDeterminingGroupChoiceInGroupID">If not null, then this choice will be active
        /// only if activeInGeneral and the value of the determining choice group equals this value.</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddChoiceInGroup(Guid choiceGroupID, int choiceNum, String choiceText, Guid? ActiveOnDeterminingGroupChoiceInGroupID)
        {
            ChoiceInGroup theChoiceInGroup = new ChoiceInGroup
            {
                ChoiceInGroupID = Guid.NewGuid(),
                ChoiceGroupID = choiceGroupID,
                ChoiceNum = choiceNum,
                ChoiceText = choiceText,
                ActiveOnDeterminingGroupChoiceInGroupID = ActiveOnDeterminingGroupChoiceInGroupID,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<ChoiceInGroup>().InsertOnSubmit(theChoiceInGroup);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + DataContext.GetTable<ChoiceGroup>().Single(f => f.ChoiceGroupID == choiceGroupID).PointsManagerID);
            return theChoiceInGroup.ChoiceInGroupID;
        }

        /// <summary>
        /// Adds a Tbl to a prediction rating universe
        /// </summary>
        /// <param name="pointsManagerID">The universe to add to</param>
        /// <param name="defaultRatingGroupAttributesID">The default attributes for rating groups in this Tbl</param>
        /// <param name="TblTabWord">A word to describe the table column groups (e.g., "Year")</param>
        /// <param name="name">The name of the Tbl</param>
        /// <param name="creator">The creator of the Tbl, or null if system-created</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddTbl(Guid pointsManagerID, Guid? defaultRatingGroupAttributesID, String TblTabWord, String name, Guid? creator, bool allowOverrideOfRatingCharacterstics, bool oneRatingPerRatingGroup, String typeOfTblRow, String rowAdditionCriteria, bool allowUsersToAddComments, bool limitCommentsToUsersWhoCanMakeUserRatings, string widthStyleNameCol, string widthStyleNumCol)
        {
            Tbl theTbl = new Tbl
            {
                TblID = Guid.NewGuid(),
                PointsManagerID = pointsManagerID,
                DefaultRatingGroupAttributesID = defaultRatingGroupAttributesID,
                WordToDescribeGroupOfColumnsInThisTbl = TblTabWord,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed,
                AllowOverrideOfRatingGroupCharacterstics = allowOverrideOfRatingCharacterstics,
                OneRatingPerRatingGroup = oneRatingPerRatingGroup,
                TypeOfTblRow = typeOfTblRow,
                AllowUsersToAddComments = allowUsersToAddComments,
                LimitCommentsToUsersWhoCanMakeUserRatings = limitCommentsToUsersWhoCanMakeUserRatings,
                TblRowAdditionCriteria = rowAdditionCriteria,
                SuppStylesHeader = "",
                SuppStylesMain = "",
                WidthStyleEntityCol = widthStyleNameCol,
                WidthStyleNumCol = widthStyleNumCol,
                FastTableSyncStatus = (int) FastAccessTableStatus.fastAccessNotCreated
            };
            string i = theTbl.Name;

            DataContext.GetTable<Tbl>().InsertOnSubmit(theTbl);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("DomainID" + DataContext.GetTable<PointsManager>().Single(u => u.PointsManagerID == pointsManagerID).DomainID);
            CacheManagement.InvalidateCacheDependency("TopicsMenu");
            return theTbl.TblID;
        }
        /// <summary>
        /// Adds a Comment object, allowing a user to add comments about a TblRow
        /// </summary>
        /// <param name="RowId">The Id of row</param>
        /// <param name="UserId">Id of user who add comment</param>
        /// <param name="CommentTitle">Title of comment</param>
        /// <param name="CommentText">Detailed description of comment text </param>
        /// <param name="Date">Date on which comment added</param>
        /// <returns></returns>
        public Guid AddComment(Guid tblRowID, Guid userId, string commentTitle, string commentText, DateTime date, StatusOfObject initialStatus)
        {
            Comment theComment = new Comment
            {
                CommentID = Guid.NewGuid(),
                TblRowID = tblRowID,
                UserID = userId,
                CommentTitle = commentTitle,
                CommentText = commentText,
                DateTime = date,
                LastDeletedDate = null,
                Status = (byte) initialStatus
            };
            DataContext.GetTable<Comment>().InsertOnSubmit(theComment);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("CommentForTblRowID" + tblRowID);
            return theComment.CommentID;

        }

        /// <summary>
        /// Adds an object representing a numeric field for a particular row
        /// </summary>
        /// <param name="fieldID">The field (which corresponds in turn to a particular row)</param>
        /// <param name="DateTime">The DateTime to which this field should be set</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of this object</returns>
        public DateTimeField AddDateTimeField(Field field, DateTime? theDateTime)
        {
            DateTimeField theDateTimeField = new DateTimeField
            {
                DateTimeFieldID = Guid.NewGuid(),
                Field = field,
                DateTime = theDateTime,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<DateTimeField>().InsertOnSubmit(theDateTimeField);
            DataContext.RegisterObjectToBeInserted(theDateTimeField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + field.TblRowID);
            return theDateTimeField;
        }

        public DatabaseStatus AddDatabaseStatus()
        {
            DatabaseStatus theDatabaseStatus = new DatabaseStatus { DatabaseStatusID = Guid.NewGuid(), PreventChanges = false };
            DataContext.GetTable<DatabaseStatus>().InsertOnSubmit(theDatabaseStatus);
            DataContext.RegisterObjectToBeInserted(theDatabaseStatus);
            return theDatabaseStatus;
        }

        /// <summary>
        /// Includes more information about a date time field to add information to a field definition
        /// </summary>
        /// <param name="FieldDefinitionID">The corresponding field definition</param>
        /// <param name="includeDate">Should this include a date?</param>
        /// <param name="includeTime">Should this include a time?</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Guid AddDateTimeFieldDefinition(Guid FieldDefinitionID, bool includeDate, bool includeTime)
        {
            DateTimeFieldDefinition theDateTimeFieldDefinition = new DateTimeFieldDefinition
            {
                DateTimeFieldDefinitionID = Guid.NewGuid(),
                FieldDefinitionID = FieldDefinitionID,
                IncludeDate = includeDate,
                IncludeTime = includeTime,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<DateTimeFieldDefinition>().InsertOnSubmit(theDateTimeFieldDefinition);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + DataContext.GetTable<FieldDefinition>().Single(f => f.FieldDefinitionID == FieldDefinitionID).Tbl.PointsManagerID);
            return theDateTimeFieldDefinition.DateTimeFieldDefinitionID;
        }

        /// <summary>
        /// Adds a new domain (the highest level container of prediction ratings).
        /// </summary>
        /// <param name="activeRatingWebsite">Should this be active in the general rating website?</param>
        /// <returns></returns>
        public Guid AddDomain(bool activeRatingWebsite, String name, Guid? creator)
        {

            Domain theDomain = new Domain
            {
                DomainID = Guid.NewGuid(),
                ActiveRatingWebsite = activeRatingWebsite,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<Domain>().InsertOnSubmit(theDomain);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("TopicsMenu");
            return theDomain.DomainID;
        }

        /// <summary>
        /// Adds a row to a Tbl. This does not automaticlaly add the associated ratings. See AddRatingsForTblRow.
        /// </summary>
        /// <param name="TblID">The Tbl to add to.</param>
        /// <param name="name">The name of the row to add.</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public TblRow AddTblRow(Tbl Tbl, string name, List<UserSelectedRatingInfo> theRatingTypeOverrides = null)
        {
            //ProfileSimple.Start("AddTblRow");
            //ProfileSimple.Start("AddTblRowFieldDisplay");
            TblRowFieldDisplay theFieldDisplay = AddTblRowFieldDisplay();
            //ProfileSimple.End("AddTblRowFieldDisplay");
            name = name.Trim();
            //ProfileSimple.Start("AddTblRow");
            TblRow theTblRow = new TblRow
            {
                TblRowID = Guid.NewGuid(),
                Tbl = Tbl,
                TblRowFieldDisplay = theFieldDisplay,
                Name = name,
                FastAccessInitialCopy = true, // must copy this to the denormalized fast access table
                InitialFieldsDisplaySet = false, // we can't set the fields display now, because we don't have an id
                NotYetAddedToDatabase = true, // we will change this just before SaveChanges
                WhenCreated = TestableDateTime.Now,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<TblRow>().InsertOnSubmit(theTblRow);
            DataContext.RegisterObjectToBeInserted(theTblRow);

            theTblRow.Tbl.NumTblRowsActive++; // increment number of table rows

            if (Tbl.FastTableSyncStatus == (int) (FastAccessTableStatus.apparentlySynchronized))
                Tbl.FastTableSyncStatus = (int)(FastAccessTableStatus.newRowsMustBeCopied);

            if (theRatingTypeOverrides != null)
            {
                foreach (UserSelectedRatingInfo theOverride in theRatingTypeOverrides)
                {
                    //ProfileSimple.Start("AddOverrideCharacteristics");
                    AddOverrideCharacteristics(theOverride, theTblRow);
                    //ProfileSimple.End("AddOverrideCharacteristics");
                }
            }

            //ProfileSimple.Start("SearchWords");
            //ProfileSimple.End("SearchWords");
            //ProfileSimple.Start("AddMissingRatings");
            //ProfileSimple.End("AddMissingRatings");
            //ProfileSimple.Start("AddVolatilityTracking");
            VolatilityTracking.AddVolatilityTracking(this, theTblRow); // this adds the TblRow-level trackers, not the RatingGroup trackers, which we now add as needed
            //ProfileSimple.End("AddVolatilityTracking");
            AddTblRowStatusRecord(theTblRow, TestableDateTime.Now, false, true);
            CacheManagement.InvalidateCacheDependency("TblRowForTblID" + theTblRow.TblID);
           // ProfileSimple.End("AddTblRow");
            return theTblRow;
        }

        public TblRowFieldDisplay AddTblRowFieldDisplay()
        {
            TblRowFieldDisplay fieldDisplay = new TblRowFieldDisplay
            { // these must be set later, after we get an ID
                TblRowFieldDisplayID = Guid.NewGuid(),
                Row = null, 
                PopUp = null,
                TblRowPage = null,
                ResetNeeded = false
            };
            DataContext.GetTable<TblRowFieldDisplay>().InsertOnSubmit(fieldDisplay);
            DataContext.RegisterObjectToBeInserted(fieldDisplay);
            return fieldDisplay;
        }

        /// <summary>
        /// Adds/updates a record about the addition of a row. 
        /// </summary>
        /// <param name="timeChanged">The time the row was added</param>
        /// <param name="status">A character indicating the status of this record</param>
        public TblRowStatusRecord AddTblRowStatusRecord(TblRow tblRow, DateTime timeChanged, bool deleting, bool adding)
        {
            if (!StatusRecords.RecordRecentChangesInStatusRecords)
                return null;
            StatusRecords.PrepareToRecordRowStatusChange(DataContext, tblRow.TblID);
            tblRow.StatusRecentlyChanged = true;
            TblRowStatusRecord record = new TblRowStatusRecord
            {
                TblRowStatusRecordID = Guid.NewGuid(),
                TblRow = tblRow,
                TimeChanged = timeChanged,
                Deleting = deleting,
                Adding = adding
            };
            DataContext.GetTable<TblRowStatusRecord>().InsertOnSubmit(record);
            DataContext.RegisterObjectToBeInserted(record);
            return record;
        }

        /// <summary>
        /// Adds a field to describe something about a row. Note that the actual information being
        /// added will be added separately in a ChoiceField, NumberField, or TextField.
        /// </summary>
        /// <param name="FieldDefinitionID">A description of the field being added.</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Field AddField(TblRow tblRow, FieldDefinition FieldDefinition)
        {
            Field theField = new Field
            {
                FieldID = Guid.NewGuid(),
                TblRow = tblRow,
                FieldDefinition = FieldDefinition,
                NotYetAddedToDatabase = true,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<Field>().InsertOnSubmit(theField);
            DataContext.RegisterObjectToBeInserted(theField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + tblRow.TblRowID);
            FieldDefinition.NumNonNull++;
            FieldDefinition.ProportionNonNull = (double)FieldDefinition.NumNonNull / ((double)tblRow.Tbl.NumTblRowsActive + (double)tblRow.Tbl.NumTblRowsDeleted);
            return theField;
        }

        /// <summary>
        /// Adds a description of a particular field for a Tbl. For example, it might specify that the first field is
        /// numeric.
        /// </summary>
        /// <param name="TblID">The Tbl to add to</param>
        /// <param name="fieldNum">The number of the field (from 1 to the maximum, but need not be unique, and some may be skipped)</param>
        /// <param name="fieldName">The name of the field</param>
        /// <param name="fieldType">The type of the field (e.g., number, choice) (see enumeration above)</param>
        /// <param name="useAsFilter">If true, then we will allow the user to search for table rows by specifying 
        /// information about this field</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddFieldDefinition(Guid TblID, int fieldNum, string fieldName, FieldTypes fieldType, bool useAsFilter)
        {
            FieldDefinition theFieldDefinition = new FieldDefinition
            {
                FieldDefinitionID = Guid.NewGuid(),
                TblID = TblID,
                FieldNum = fieldNum,
                FieldName = fieldName,
                FieldType = (int)fieldType,
                UseAsFilter = useAsFilter,
                DisplayInTableSettings = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false,false,false,false,false,false,false,false,false),
                DisplayInPopupSettings = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false,false,false,false,true,true,false,false,true),
                DisplayInTblRowPageSettings = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false,false,false,false,true,true,false,false,true),
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<FieldDefinition>().InsertOnSubmit(theFieldDefinition);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + DataContext.GetTable<Tbl>().Single(f => f.TblID == TblID).PointsManagerID);
            return theFieldDefinition.FieldDefinitionID;
        }

        public HierarchyItem AddHierarchyItem(HierarchyItem higherHierarchyItem, Tbl associatedTbl, bool includeInMenu, string name)
        {
            // The following is probably not relevant any more.
            // Note: Initially, the Linq to SQL data model had child and parent properties for routing and menu, but this seemed
            // to cause a number of problems in Linq to SQL (bad query translation and duplicated items). So, we are avoiding the 
            // properties for now, which means that we need to have the ID variable
            //if (higherHierarchyItem != null && higherHierarchyItem.HierarchyItemID == -1)
            //    throw new Exception("Internal error: Must submit changes on hierarchy item before using as basis for higher hierarchy item.");

            HierarchyItem theHierarchyItem = new HierarchyItem
            {
                HierarchyItemID = Guid.NewGuid(),
                ParentHierarchyItemID = higherHierarchyItem == null ? (Guid?)null : (Guid?)higherHierarchyItem.HierarchyItemID,
                Tbl = associatedTbl,
                IncludeInMenu = includeInMenu,
                HierarchyItemName = name
            };
            DataContext.GetTable<HierarchyItem>().InsertOnSubmit(theHierarchyItem);
            DataContext.RegisterObjectToBeInserted(theHierarchyItem);
            HierarchyItems.SetFullHierarchy(ref theHierarchyItem);
            return theHierarchyItem;
        }

        /// <summary>
        /// return an insertable content Id and add a Insertable content
        /// </summary>
        /// <param name="name">Name of the Insertable content object</param>
        /// <param name="domainId">Id of domain or nullable</param>
        /// <param name="universeId">Id of universe or nullable</param>
        /// <param name="TblId">Id of Tbl or nullable</param>
        /// <param name="contents">Content of the Insertable content object</param>
        /// <param name="isTextOnly">1, if content does not contain html else 0</param>
        /// <param name="overridable">1, if Object is overidable</param>
        /// <param name="loaction">loaction of the Insertable object</param>
        /// <returns></returns>
        public Guid AddInsertableContents(string name, Guid? domainId, Guid? universeId, Guid? TblId, string contents, bool isTextOnly, bool overridable, InsertableLocation location)
        {
            InsertableContent theInsertableContent = new InsertableContent
                                    {
                                        InsertableContentID = Guid.NewGuid(),
                                        Name=name,
                                        DomainID=domainId,
                                        PointsManagerID=universeId,
                                        TblID=TblId,
                                        Content=contents,
                                        IsTextOnly=isTextOnly,
                                        Overridable=overridable,
                                        Location=(short) location,
                                        Status=(Byte)StatusOfObject.Proposed

                                    };
            DataContext.GetTable<InsertableContent>().InsertOnSubmit(theInsertableContent);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("InsertableContent");
            return theInsertableContent.InsertableContentID;

        }
        

        /// <summary>
        /// return an unique activation number and add a Invited user

        /// </summary>
        /// <param name="EmaillId">Email Id of Invited User</param>
        /// <param name="mayView">Can view the ratings and predictions</param>
        /// <param name="mayPredict">Can make predictions</param>
        /// <param name="mayAddTbls">Can add Tbls</param>
        /// <param name="mayResolveRatings">Can resolve ratings and rating groups</param>
        /// <param name="mayChangeTblRows">Can change entities' fields, add table rows, etc.</param>
        /// <param name="mayChangeChoiceGroups">Can add to and inactivate choice group entries</param>
        /// <param name="mayChangeCharacteristics">Can change rating characteristics, group attributes</param>
        /// <param name="mayChangeCategories">Can add/delete table columns and groups</param>
        /// <param name="mayChangeUsersRights">Can add/delete users rights objects</param>
        /// <param name="mayAdjustPoints">Can make adjustments to users' points</param>
        /// <param name="mayChangeProposalSettings">Can change proposal settings</param>
        /// <returns></returns>

        public Guid AddInvitedUser(string emailId, bool mayView, bool mayPredict, bool mayAddTbls,
            bool mayResolveRatings, bool mayChangeTblRows, bool mayChangeChoiceGroups, bool mayChangeCharacteristics,
            bool mayChangeColumns, bool mayChangeUsersRights, bool mayAdjustPoints, bool mayChangeProposalSettings)
        {
            InvitedUser TheInvitedUser = new InvitedUser
            {
                EmailId = emailId,
                MayAddTbls = mayAddTbls,
                MayAdjustPoints = mayAdjustPoints,
                MayChangeColumns = mayChangeColumns,
                MayChangeCharacteristics = mayChangeCharacteristics,
                MayChangeChoiceGroups = mayChangeChoiceGroups,
                MayChangeTblRows = mayChangeTblRows,
                MayChangeProposalSettings = mayChangeProposalSettings,
                MayChangeUsersRights = mayChangeUsersRights,
                MayResolveRatings = mayResolveRatings,
                MayPredict = mayPredict,
                MayView = mayView,
                IsRegistered = false
            };
            DataContext.GetTable<InvitedUser>().InsertOnSubmit(TheInvitedUser);
            DataContext.SubmitChanges();
            return TheInvitedUser.ActivationNumber;
        }

        public Guid AddLongProcess(LongProcessTypes typeOfProcess, int? delayBeforeReset, Guid? object1ID, Guid? object2ID, int priority, byte[] additionalInfo)
        {
            LongProcess theLongProcess = new LongProcess
            {
                LongProcessID = Guid.NewGuid(),
                TypeOfProcess = (int)typeOfProcess,
                Object1ID = object1ID,
                Object2ID = object2ID,
                Priority = priority,
                AdditionalInfo = additionalInfo,
                Started = false,
                Complete = false,
                ResetWhenComplete = false,
                DelayBeforeRestart = delayBeforeReset,
                EarliestRestart = TestableDateTime.Now.AddSeconds(8)  // always wait a while so we don't try to initiate a long process while a prediction or rating resolution is still being processed
            };
            DataContext.GetTable<LongProcess>().InsertOnSubmit(theLongProcess);
            DataContext.SubmitChanges();
            return theLongProcess.LongProcessID;
        }



        /// <summary>
        /// Adds a rating to the database. The rating group and a rating plan for this rating should already be
        /// added. This routine creates other objects too (such as the rating status object), and it calculates
        /// the default prediction.
        /// 
        /// </summary>
        /// <param name="ratingGroupID">The rating group to add to</param>
        /// <param name="ratingPlanID">The plan for this rating</param>
        /// <param name="numRatingsBeingAdded">The total number of ratings being added to the group.</param>
        /// <returns>The id of the added rating object</returns>
        public Rating AddRating(
            RatingGroup ratingGroup, 
            RatingPlan ratingPlan, 
            RatingGroupPhaseStatus ratingGroupPhaseStatus, 
            RatingGroup topRatingGroup = null, 
            int numRatingsBeingAdded = 1)
        {
            RatingGroupAttribute theGroupAttributes = ratingPlan.RatingGroupAttribute;

            RatingCharacteristic RatingCharacteristics = theGroupAttributes.RatingCharacteristic;

            Rating theRating = new Rating
            {
                RatingID = Guid.NewGuid(),
                RatingGroup = ratingGroup,
                RatingCharacteristic = RatingCharacteristics,
                OwnedRatingGroup = null,
                TopRatingGroup = topRatingGroup,
                NumInGroup = ratingPlan.NumInGroup,
                Name = ratingPlan.Name,
                Creator = ratingPlan.Creator,
                CurrentValue = null,
                LastModifiedResolutionTimeOrCurrentValue = TestableDateTime.Now
            };


            DataContext.GetTable<Rating>().InsertOnSubmit(theRating);
            DataContext.RegisterObjectToBeInserted(theRating);

            //theRating = R8RDB.GetTable<Rating>().Where(m => m.RatingStatusID == theRatingStatusID).FirstOrDefault().RatingID;
            if (theRating.NumInGroup == 1)
            {
                RatingGroup theRatingGroup = theRating.RatingGroup;
                theRatingGroup.CurrentValueOfFirstRating = null; //  defaultUserRating;
            }

            AddRatingPhaseStatus(theRating, ratingGroupPhaseStatus);

            return theRating;
        }

        /// <summary>
        /// Adds an object representing the characteristics of a rating.
        /// </summary>
        /// <param name="RatingPhaseGroupID">The rating phase group for the rating</param>
        /// <param name="SubsidyDensityRangeGroupID">The subsidy density range group</param>
        /// <param name="PointsTrustRulesID">The counting rules</param>
        /// <param name="minimumUserRating">The minimum prediction for the rating</param>
        /// <param name="maximumUserRating">The maximum prediction for the rating</param>
        /// <param name="decimalPlaces">The number of decimal places t</param>
        /// <param name="name">The name of the rating</param>
        /// <param name="creator">The creator of the rating</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added rating characteristics object</returns>
        public Guid AddRatingCharacteristics(Guid RatingPhaseGroupID, Guid? SubsidyDensityRangeGroupID, decimal minimumUserRating, decimal maximumUserRating, short decimalPlaces, String name, Guid? creator)
        {
            RatingCharacteristic theCharacteristics = new RatingCharacteristic
            {
                RatingCharacteristicsID = Guid.NewGuid(),
                RatingPhaseGroupID = RatingPhaseGroupID,
                SubsidyDensityRangeGroupID = SubsidyDensityRangeGroupID,
                MinimumUserRating = minimumUserRating,
                MaximumUserRating = maximumUserRating,
                DecimalPlaces = (Byte)decimalPlaces,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<RatingCharacteristic>().InsertOnSubmit(theCharacteristics);
            DataContext.SubmitChanges();

            return theCharacteristics.RatingCharacteristicsID;
        }

        /// <summary>
        /// Add a rating condition.
        /// </summary>
        /// <param name="conditionalRatingPlanID">The id of a rating plan for the conditional rating, or null</param>
        /// <param name="conditionRatingPlanID">The id of a rating plan for the condition rating, or null</param>
        /// <param name="conditionalRatingID">The id of the conditional rating, or null</param>
        /// <param name="conditionRatingID">The id of the condition rating or null</param>
        /// <param name="greaterThan">UserRatings in conditional rating will resolve at 0 unless condition rating value is greater than this (unless null)</param>
        /// <param name="lessThan">UserRatings in conditional rating will resolve at 0 unless condition rating value is less than this (unless null)</param>
        /// <param name="status"></param>
        /// <returns>A RatingCondition object, specifying how a rating or planned rating should be
        /// conditional on another rating</returns>
        public RatingCondition AddRatingCondition(Rating conditionRating, decimal? greaterThan, decimal? lessThan)
        {

            RatingCondition theCondition = new RatingCondition
            {
                RatingConditionID = Guid.NewGuid(),
                Rating = conditionRating,
                GreaterThan = greaterThan,
                LessThan = lessThan,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<RatingCondition>().InsertOnSubmit(theCondition);
            DataContext.RegisterObjectToBeInserted(theCondition);

            return theCondition;
        }

        /// <summary>
        /// Adds a rating group for a row. Does not add the associated ratings (see AddRatingGroupAndRatings).
        /// </summary>
        /// <param name="TblColumnID">The column this rating group corresponds to</param>
        /// <param name="ratingGroupAttributesID">The attributes of the rating group</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added rating group</returns>
        public RatingGroup AddRatingGroup(TblRow tblRow, TblColumn tblColumn, RatingGroupAttribute ratingGroupAttributes, bool isTopGroup, ref RatingGroupPhaseStatus theRatingGroupPhaseStatus)
        {
            byte? MType = ratingGroupAttributes.TypeOfRatingGroup;
            if (tblRow.TblID != tblColumn.TblTab.TblID)
                throw new Exception("Internal exception. Row and column must be in same table.");
            RatingGroup theGroup = new RatingGroup
            {
                RatingGroupID = Guid.NewGuid(),
                TblRow = tblRow,
                TblColumn = tblColumn,
                RatingGroupAttribute = ratingGroupAttributes,
                CurrentValueOfFirstRating = null,
                ValueRecentlyChanged = false,
                HighStakesKnown = false, /* assume that rating group will not currently be high stakes */
                TypeOfRatingGroup = (Byte)MType,
                WhenCreated = TestableDateTime.Now,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<RatingGroup>().InsertOnSubmit(theGroup);
            DataContext.RegisterObjectToBeInserted(theGroup);

            if (isTopGroup)
            {
                theRatingGroupPhaseStatus = AddRatingGroupPhaseStatus(ratingGroupAttributes.RatingCharacteristic.RatingPhaseGroup, theGroup);
                VolatilityTracking.AddVolatilityTracking(this, theGroup);
            }

            return theGroup;
        }

        /// <summary>
        /// Add a rating group and the ratings associated with it. This is called by AddRatingsForTblRow.
        /// </summary>
        /// <param name="TblColumnID">The column this rating group corresponds to</param>
        /// <param name="ratingGroupAttributesID">The attributes of the rating group</param>
        /// <returns>The id for the rating group object</returns>
        /// 

        [Serializable]
        private class TblRowIDAndTblColumnIDUniquenessGenerator
        {
            public Guid TblRowID;
            public Guid TblColumnID;
            public string Purpose = "GenerateUniqueID";
        }

        public Tuple<Guid, Guid> AddMissingRatingGroupAndRatings(string tblRowIDAndColumnIDSeparatedBySlash)
        {
            string[] components = tblRowIDAndColumnIDSeparatedBySlash.Split('/');
            if (!(components.Length == 2))
                throw new Exception("Unexpected cell formatting error.");
            TblRow tblRow = null;
            TblColumn tblColumn = null;
            Guid tblRowID, tblColumnID;
            try
            {
                tblRowID = new Guid(components[0]);
                tblColumnID = new Guid(components[1]);
            }
            catch
            {
                throw new Exception("Improper cell formatting of row or column ID.");
            }
            try
            {
                tblRow = DataContext.GetTable<TblRow>().Single(x => x.TblRowID == tblRowID);
                tblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == tblColumnID);
                if (tblRow.TblID != tblColumn.TblTab.TblID)
                    throw new Exception("Row and column must be in same table.");
            }
            catch
            {
                throw new Exception("Database error. The rating information for that row and column could not be found.");
            }
            Rating rating = null;
            try
            {
                // see if it might have been added already -- this could happen if the page hasn't been updated in a while or if a user is trying to mess with us
                rating = DataContext.GetTable<Rating>().FirstOrDefault(x => x.RatingGroup.TblRowID == tblRowID && x.RatingGroup.TblColumnID == tblColumnID);
                if (rating == null)
                {
                    AddMissingRatingGroupAndRatings(tblRow, tblColumn);
                    DataContext.SubmitChanges();
                }
            }
            catch
            { // it may have failed because there was a simultaneous attempt to add the rating group and ratings (which we prevent), so let's see if it now exists in the database
                ResetDataContexts(); // we don't want to try adding this again because then we'd get another exception
            }
            rating = DataContext.GetTable<Rating>().FirstOrDefault(x => x.RatingGroup.TblRowID == tblRowID && x.RatingGroup.TblColumnID == tblColumnID);
            if (rating == null)
                throw new Exception("Database error. The rating could not be added to the database.");
            // update the fast-access tables; no tragedy if this fails (we'll just end up back here, because the fast-access tables will continue to use the row and column IDs instead of the rating ids) or if this happens more than once (because we simultaneously attempt to add rating in two different places), but once the rating is created it should be unusual for us to end up in this method. If we were to change things so that we regularly used this method (to refer to ratings by table row and column instead of rating id and rating group id), we might want to change this to decrease redundant writes.
            new FastAccessRatingIDUpdatingInfo() { RatingID = rating.RatingID, RatingGroupID = rating.RatingGroup.RatingGroupID, TblColumnID = tblColumn.TblColumnID }.AddToTblRow(tblRow);
            DataContext.SubmitChanges();
            ResetDataContexts();
            return new Tuple<Guid, Guid>(rating.RatingID, rating.RatingGroupID);
        }

        public RatingGroup AddRatingGroupAndRatings(TblRow tblRow, TblColumn tblColumn, RatingGroupAttribute ratingGroupAttributes)
        {
            if (!(tblRow.NotYetAddedToDatabase || tblColumn.NotYetAddedToDatabase)) // if it's a new row ==> no uniqueness constraint needed -- because we must be doing this from a background worker process that is executing in an orderly process. If it's an old row/column, then we are dynamically adding content to a particular cell, and there is a danger that we're doing this in two places at once
                AddUniquenessLock(new TblRowIDAndTblColumnIDUniquenessGenerator() { TblRowID = tblRow.TblRowID, TblColumnID = tblColumn.TblColumnID }, TestableDateTime.Now + TimeSpan.FromDays(1)); // will prevent anyone else from adding to the same cell simultaneously (note that 1 day is a bit arbitrary -- key is that we want to avoid scenario where we check for ratings and don't find any, and add just an instant later than another web role doing the same thing)
            RatingGroup theRatingGroup = AddRatingGroupAndRatings(tblRow, tblColumn, ratingGroupAttributes, null, null);
            AdvanceRatingGroupToNextRatingPhase(theRatingGroup);
            return theRatingGroup;
        }

        /// <summary>
        /// Add a rating group and ratings. This version has an extra parameter specifying whether it is being
        /// called recursively.
        /// </summary>
        /// <returns></returns>
        internal RatingGroup AddRatingGroupAndRatings(
            TblRow tblRow, 
            TblColumn TblColumn, 
            RatingGroupAttribute ratingGroupAttributes, 
            RatingGroupPhaseStatus theRatingGroupPhaseStatus, 
            RatingGroup topRatingGroup = null)
        {
            bool recursiveCall = (topRatingGroup != null);
            //Trace.TraceInformation("AddRatingGroupAndRatings " + tblRow.TblRowID + " " + TblColumnID + recursiveCall.ToString());
            if (!recursiveCall)
            { // We're at the top of the hierarchy. Let's make sure this rating group doesn't already exist.
                IQueryable<RatingGroup> existingRatingGroups = null;
                if (!tblRow.NotYetAddedToDatabase)
                {
                    existingRatingGroups = DataContext.WhereFromNewOrDatabase<RatingGroup>(mg => mg.TblRow.TblRowID == tblRow.TblRowID
                                                   && mg.TblColumn.TblColumnID == TblColumn.TblColumnID
                                                   && mg.Status == (Byte)StatusOfObject.Active)
                                                   ;
                    if (existingRatingGroups.Count() >= 1)
                        return existingRatingGroups.First();
                }
            }

            RatingGroupPhaseStatus theRatingGroupPhaseStatus2 = theRatingGroupPhaseStatus;
            //ProfileSimple.Start("AddRatingGroup");
            RatingGroup theGroup = AddRatingGroup(tblRow, TblColumn, ratingGroupAttributes, !recursiveCall, ref theRatingGroupPhaseStatus2);
            //ProfileSimple.End("AddRatingGroup");
            theRatingGroupPhaseStatus = theRatingGroupPhaseStatus2;
            if (topRatingGroup == null)
                topRatingGroup = theGroup;

            theGroup.Status = (int)StatusOfObject.Active;
            RatingGroupAttribute thisRatingGroupAttributes = theGroup.RatingGroupAttribute;
            decimal minimumUserRating = thisRatingGroupAttributes.RatingCharacteristic.MinimumUserRating;
            decimal? constrainedSum = thisRatingGroupAttributes.ConstrainedSum;

            string key = "RatingPlanFor" + ratingGroupAttributes.GetHashCode();
            List<RatingPlan> theRatingPlans = DataContext.TempCacheGet(key) as List<RatingPlan>;
            if (theRatingPlans == null)
            {
                theRatingPlans = DataContext.GetTable<RatingPlan>().Where(p => p.RatingGroupAttribute.RatingGroupAttributesID == ratingGroupAttributes.RatingGroupAttributesID && p.Status == (Byte)StatusOfObject.Active).ToList();
                DataContext.TempCacheAdd(key, theRatingPlans);
            }
            int numRatingsBeingAdded = theRatingPlans.Count();
            foreach (RatingPlan thePlan in theRatingPlans)
            {
                //ProfileSimple.Start("AddRating");
                Rating theRating = AddRating(theGroup, thePlan, theRatingGroupPhaseStatus, topRatingGroup, numRatingsBeingAdded);
                //ProfileSimple.End("AddRating");
                if (thePlan.OwnedRatingGroupAttributesID != null)
                { // add recursively
                    string key2 = "OwnedRatingGroupFor" + thePlan.GetHashCode();
                    RatingGroupAttribute ownedRatingGroupAttributes = DataContext.TempCacheGet(key2) as RatingGroupAttribute;
                    if (ownedRatingGroupAttributes == null)
                    {
                        bool inCacheAsNull = DataContext.TempCache.ContainsKey(key2);
                        if (!inCacheAsNull)
                        {
                            ownedRatingGroupAttributes = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == (Guid)thePlan.OwnedRatingGroupAttributesID);
                            DataContext.TempCacheAdd(key2, ownedRatingGroupAttributes);
                        }
                    }
                    if (ownedRatingGroupAttributes.ConstrainedSum == null && constrainedSum != null)
                    {
                        ownedRatingGroupAttributes.ConstrainedSum = minimumUserRating + constrainedSum / numRatingsBeingAdded;
                    }
                    //ProfileSimple.Start("AddRatingGroupAndRatings");
                    RatingGroup theSubgroup = AddRatingGroupAndRatings(tblRow, TblColumn, thePlan.RatingGroupAttribute1, theRatingGroupPhaseStatus, topRatingGroup);
                    //ProfileSimple.End("AddRatingGroupAndRatings");
                    RelateRatingAndGroup(theSubgroup, theRating);
                }
            }
            return theGroup;
        }

        /// <summary>
        /// Add the ratings for a row. Not currently in use.
        /// </summary>
        public void AddMissingRatingsForTblRow(TblRow theTblRow)
        {
            //ProfileSimple.Start("TblTabs");
            List<TblTab> theTblTabs = DataContext.TempCacheGet("TblTabs" + theTblRow.TblID) as List<TblTab>;
            if (theTblTabs == null)
            {
                theTblTabs = DataContext.GetTable<TblTab>().Where(cg => cg.TblID == theTblRow.TblID && cg.Status == (Byte)StatusOfObject.Active).ToList();
                DataContext.TempCacheAdd("TblTabs" + theTblRow.TblID, theTblTabs);
            }
            //ProfileSimple.End("TblTabs");
            foreach (TblTab theTblTab in theTblTabs)
            {
                List<TblColumn> tblColumns = DataContext.TempCacheGet("TblColumns" + theTblTab.TblTabID) as List<TblColumn>;
                if (tblColumns == null)
                {
                    tblColumns = DataContext.GetTable<TblColumn>().Where(c => c.TblTabID == theTblTab.TblTabID && c.Status == (Byte)StatusOfObject.Active).ToList();
                    DataContext.TempCacheAdd("TblColumns" + theTblTab.TblTabID, tblColumns);
                }
                foreach (TblColumn tblColumn in tblColumns)
                {
                    AddMissingRatingGroupAndRatings(theTblRow, tblColumn);
                }
            }
        }
        private void AddMissingRatingGroupAndRatings(TblRow theTblRow, TblColumn theColumn)
        {
            RatingGroupAttribute theGroupAttributes;
            OverrideCharacteristic overrideCharacteristics = null;
            if (!theTblRow.NotYetAddedToDatabase && theColumn.TblTab.Tbl.AllowOverrideOfRatingGroupCharacterstics)
                overrideCharacteristics = DataContext.GetTable<OverrideCharacteristic>().SingleOrDefault(oc => oc.TblRow.TblRowID == theTblRow.TblRowID && oc.TblColumnID == theColumn.TblColumnID && oc.Status == (Byte)StatusOfObject.Active);
            if (overrideCharacteristics == null)
                theGroupAttributes = theColumn.RatingGroupAttribute;
            else
                theGroupAttributes = overrideCharacteristics.RatingGroupAttribute;
            //ProfileSimple.Start("AddRatingGroupAndRatings");
            AddRatingGroupAndRatings(theTblRow, theColumn, theGroupAttributes);
            //ProfileSimple.End("AddRatingGroupAndRatings");
        }


        /// <summary>
        /// Adds attributes for a rating group.
        /// </summary>
        /// <param name="ratingCharacteristicsID">The rating characteristics for each rating in the group.</param>
        /// <param name="constrainedSum">The constrained sum (above the minimum) for all ratings (e.g., 100 for a rating 
        /// calculating probabilities)</param>
        /// <param name="name">The name of the rating group attributes</param>
        /// <param name="creator">The creator of the rating group attributes</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the attributes object</returns>
        public Guid AddRatingGroupAttributes(Guid ratingCharacteristicsID, Guid? ratingConditionID, decimal? constrainedSum, String name, RatingGroupTypes RatingType, String ratingGroupDescription, Guid? creator, Guid pointsManagerID, bool ratingEndingTimeVaries, bool ratingsCanBeAutoCalculated, decimal longTermPointsWeight)
        {
            RatingGroupAttribute theRatingGroupAttributes = new RatingGroupAttribute
            {
                RatingGroupAttributesID = Guid.NewGuid(),
               PointsManagerID=pointsManagerID,
                RatingCharacteristicsID = ratingCharacteristicsID,
                RatingConditionID = ratingConditionID,
                ConstrainedSum = constrainedSum,
                Name = name,
                TypeOfRatingGroup=(byte)RatingType,
                Description=ratingGroupDescription,
                RatingEndingTimeVaries=ratingEndingTimeVaries,
                RatingsCanBeAutocalculated = ratingsCanBeAutoCalculated,
                LongTermPointsWeight = longTermPointsWeight,
                MinimumDaysToTrackLongTerm = 365,
                Creator = creator,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<RatingGroupAttribute>().InsertOnSubmit(theRatingGroupAttributes);
            DataContext.SubmitChanges();
            return theRatingGroupAttributes.RatingGroupAttributesID;
        }

        /// <summary>
        /// Adds information about a rating phase to a rating phase group.
        /// </summary>
        /// <param name="RatingPhaseGroupID">The group to add to</param>
        /// <param name="subsidyLevel">The subsidy level during this phase (which can be modified by a subsidy adjustment factor)</param>
        /// <param name="scoringRule">The scoring rule to use for calculating points</param>
        /// <param name="timed">True if this is timed rather than indefinite</param>
        /// <param name="baseTimingOnSpecificTime">True if endTime rather than runTime should control</param>
        /// <param name="endTime">The earliest time at which this should end</param>
        /// <param name="runTime">The minimum time this should run</param>
        /// <param name="halfLifeForResolution">The half life for further running after the end</param>
        /// <param name="repeatIndefinitely">True if this phase should keep repeating</param>
        /// <param name="repeatNTimes">If not null, the number of times this phase should repeat</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the object added</returns>
        public Guid AddRatingPhase(Guid RatingPhaseGroupID, decimal subsidyLevel, ScoringRules scoringRule, bool timed, bool baseTimingOnSpecificTime, DateTime? endTime, int? runTime, int halfLifeForResolution, bool repeatIndefinitely, int? repeatNTimes)
        {
           
            RatingPhaseGroup theGroup =   DataContext.GetTable<RatingPhaseGroup>().Single(x=>x.RatingPhaseGroupID==RatingPhaseGroupID);
            theGroup.NumPhases++;
            DataContext.SubmitChanges();

            RatingPhase thePhase = new RatingPhase
            {
                RatingPhaseID = Guid.NewGuid(),
                RatingPhaseGroupID = RatingPhaseGroupID,
                NumberInGroup = theGroup.NumPhases,
                SubsidyLevel = subsidyLevel,
                ScoringRule = (short)scoringRule,
                Timed = timed,
                BaseTimingOnSpecificTime = baseTimingOnSpecificTime,
                EndTime = endTime,
                RunTime = runTime,
                HalfLifeForResolution = halfLifeForResolution,
                RepeatIndefinitely = repeatIndefinitely,
                RepeatNTimes = repeatNTimes,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<RatingPhase>().InsertOnSubmit(thePhase);
            DataContext.SubmitChanges();

            return thePhase.RatingPhaseID;

        }

        /// <summary>
        /// Add a group of rating phases. Call this, then AddRatingPhase for each phase.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="creator">The creator of the group, or null for system-created</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the object added</returns>
        public Guid AddRatingPhaseGroup(String name, Guid? creator)
        {
            RatingPhaseGroup theGroup = new RatingPhaseGroup
            {
                RatingPhaseGroupID = Guid.NewGuid(),
                NumPhases = 0,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };


            DataContext.GetTable<RatingPhaseGroup>().InsertOnSubmit(theGroup);
            DataContext.SubmitChanges();

            return theGroup.RatingPhaseGroupID;
        }

        /// <summary>
        /// Adds a rating phase status object. This should be created before creating the RatingStatus object.
        /// </summary>
        /// <param name="RatingPhaseGroupID">The group for which a status object is being created.</param>
        /// <returns>The id of this object</returns>
        public RatingGroupPhaseStatus AddRatingGroupPhaseStatus(RatingPhaseGroup ratingPhaseGroup, RatingGroup topRatingGroup)
        {
            // RatingPhaseGroup theGroup = ObjDataAccess.GetRatingPhaseGroup(RatingPhaseGroupID);
            string key = "RatingPhaseFirst" + ratingPhaseGroup.GetHashCode();
            RatingPhase thePhase = DataContext.TempCacheGet(key) as RatingPhase;
            if (thePhase == null)
            {
                thePhase = DataContext.GetTable<RatingPhase>().Single(p => p.RatingPhaseGroupID == ratingPhaseGroup.RatingPhaseGroupID && p.NumberInGroup == 1);
                DataContext.TempCacheAdd(key, thePhase);
            }

            DateTime currentTime = TestableDateTime.Now;

            RatingGroupPhaseStatus theStatus = new RatingGroupPhaseStatus
            {
                RatingGroupPhaseStatusID = Guid.NewGuid(),
                RatingPhaseGroup = ratingPhaseGroup,
                RatingPhase = thePhase,
                RatingGroup = topRatingGroup,
                RoundNum = 0,
                RoundNumThisPhase = 0,
                StartTime = currentTime,
                ActualCompleteTime = currentTime,
                EarliestCompleteTime = currentTime,
                ShortTermResolveTime = currentTime,
                WhenCreated = TestableDateTime.Now,
            };
            DataContext.GetTable<RatingGroupPhaseStatus>().InsertOnSubmit(theStatus);
            DataContext.RegisterObjectToBeInserted(theStatus);

            if (topRatingGroup.HighStakesKnown)
            {
                topRatingGroup.TblRow.CountHighStakesNow--;
                if (topRatingGroup.TblRow.CountHighStakesNow == 0 && topRatingGroup.TblRow.ElevateOnMostNeedsRating)
                {
                    topRatingGroup.TblRow.ElevateOnMostNeedsRating = false;
                    topRatingGroup.TblRow.Tbl.PointsManager.HighStakesNoviceNumActive--;
                }
            }
            topRatingGroup.HighStakesKnown = false; /* may be changed later by background process */

            return theStatus;
        }

        public RatingPhaseStatus AddRatingPhaseStatus(Rating theRating, RatingGroupPhaseStatus theRatingGroupPhaseStatus)
        {
            RatingPhaseStatus theStatus = new RatingPhaseStatus
            {
                RatingPhaseStatusID = Guid.NewGuid(),
                Rating = theRating,
                RatingGroupPhaseStatus = theRatingGroupPhaseStatus,
                ShortTermResolutionValue = null,
                TriggerUserRatingsUpdate = false
            };
            DataContext.GetTable<RatingPhaseStatus>().InsertOnSubmit(theStatus);
            DataContext.RegisterObjectToBeInserted(theStatus);
            return theStatus;
        }

        /// <summary>
        /// Returns an object specifying a plan for creating a rating. Note that the OwnedRatingGroupAttributesID is initially set to null.
        /// Call RelateRatingPlanAndGroupAttributes after creating a hierarchically lower rating group.
        /// </summary>
        /// <param name="ratingGroupAttributesID">The rating group attributes for the group to contain this rating</param>
        /// <param name="numInGroup">The number of this rating in its group</param>
        /// <param name="defaultUserRating">The default prediction for this rating</param>
        /// <param name="name">The name to be given to this rating</param>
        /// <param name="creator">The creator of this rating, or null for system-created</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the rating plan</returns>
        public Guid AddRatingPlan(Guid ratingGroupAttributesID, int numInGroup, decimal? defaultUserRating, string name, string ratingDescription, Guid? creator)
        {
            RatingPlan thePlan = new RatingPlan
            {
                RatingPlanID = Guid.NewGuid(),
                RatingGroupAttributesID = ratingGroupAttributesID,
                OwnedRatingGroupAttributesID = null,
                NumInGroup = numInGroup,
                DefaultUserRating = defaultUserRating,
                Name = name,
                Description=ratingDescription,
                Creator = creator,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<RatingPlan>().InsertOnSubmit(thePlan);
            DataContext.SubmitChanges();
            return thePlan.RatingPlanID;
        }

        /// <summary>
        /// AddRatingGroupResolution
        /// Adds an object indicating that a rating group should be resolved.
        /// </summary>
        /// <param name="ratingGroupID">The topmost rating group -- all ratings will be resolved</param>
        /// <param name="effectiveTime">The effective time of the resolution (can be before the current time but not later)</param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public RatingGroupResolution AddRatingGroupResolution(RatingGroup ratingGroup, bool cancelPreviousResolutions, bool resolveByUnwinding, DateTime? effectiveTime, Guid? creator)
        {

            DateTime currentTime = TestableDateTime.Now;
            if (effectiveTime == null) 
                effectiveTime = currentTime;
            else if (effectiveTime > currentTime)
                throw new Exception("Effective time cannot be after the current time.");

            RatingGroupResolution theResolution = new RatingGroupResolution
            {
                RatingGroupResolutionID = Guid.NewGuid(),
                RatingGroup = ratingGroup,
                CancelPreviousResolutions = cancelPreviousResolutions,
                ResolveByUnwinding = resolveByUnwinding,
                EffectiveTime = (DateTime) effectiveTime,
                ExecutionTime = currentTime, // note: not necessarily unique
                WhenCreated = TestableDateTime.Now,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed // will be changed to active after the resolution is accomplished.
            };
            DataContext.GetTable<RatingGroupResolution>().InsertOnSubmit(theResolution);



            DataContext.SubmitChanges(); // must submit to get proper ordering by RatingGroupResolutionID

            return theResolution;
        }

        /// <summary>
        /// Adds an object representing a numeric field for a particular tblRow
        /// </summary>
        /// <param name="fieldID">The field (which corresponds in turn to a particular tblRow)</param>
        /// <param name="number">The number to which this field should be set</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of this object</returns>
        public NumberField AddNumberField(Field field, decimal? number)
        {
            NumberField theNumberField = new NumberField
            {
                NumberFieldID = Guid.NewGuid(),
                Field = field,
                Number = number,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<NumberField>().InsertOnSubmit(theNumberField);
            DataContext.RegisterObjectToBeInserted(theNumberField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + field.TblRowID);
            return theNumberField;
        }

        /// <summary>
        /// Adds a NumberFieldDefinition, which contains more information to add to a number field for a number.
        /// </summary>
        /// <param name="FieldDefinitionID">The id of this object</param>
        /// <param name="minimum">The minimum value for this number field</param>
        /// <param name="maximum">The maximum value for this number field</param>
        /// <param name="decimalPlaces">The number of decimal places to display</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddNumberFieldDefinition(Guid FieldDefinitionID, decimal? minimum, decimal? maximum, short decimalPlaces)
        {
            NumberFieldDefinition theNumberFieldDefinition = new NumberFieldDefinition
            {
                NumberFieldDefinitionID = Guid.NewGuid(),
                FieldDefinitionID = FieldDefinitionID,
                Minimum = minimum,
                Maximum = maximum,
                DecimalPlaces = (Byte)decimalPlaces,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<NumberFieldDefinition>().InsertOnSubmit(theNumberFieldDefinition);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + DataContext.GetTable<FieldDefinition>().Single(f => f.FieldDefinitionID == FieldDefinitionID).Tbl.PointsManagerID);
            return theNumberFieldDefinition.NumberFieldDefinitionID;
        }

        /// <summary>
        /// Add an OverrideCharacteristics object, which specifies the rating group attributes for 
        /// a particular column of a particular row. It doesn't actually implement any changes, but
        /// the AddRatingsForTblRow function looks for these objects. This won't affect running ratings.
        /// </summary>
        /// <param name="ratingGroupAttributesID">The rating group attributes to use</param>
        /// <param name="TblColumnID">The column for which ratings are being added</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the object </returns>
        public OverrideCharacteristic AddOverrideCharacteristics(RatingGroupAttribute theRatingGroupAttributes, TblRow theTblRow, TblColumn theTblColumn)
        {
            OverrideCharacteristic theOverrideCharacteristics = new OverrideCharacteristic
            {
                OverrideCharacteristicsID = Guid.NewGuid(),
                RatingGroupAttribute = theRatingGroupAttributes,
                TblRow = theTblRow,
                TblColumn = theTblColumn,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<OverrideCharacteristic>().InsertOnSubmit(theOverrideCharacteristics);
            DataContext.SubmitChanges();
            return theOverrideCharacteristics;
        }

        /// <summary>
        /// Adds a points adjustment object. Note that this does not actually add the points (see, e.g., UpdateUserPoints). 
        /// It just maintains a record of the addition.
        /// </summary>
        /// <param name="userID">The user id of the user whose points are affected</param>
        /// <param name="pointsManagerID">The universe for which the user's points are affected</param>
        /// <param name="theReason">The reason for the change (see above enumeration)</param>
        /// <param name="totalAdjustment">The adjustment to the user's total points</param>
        /// <param name="currentAdjustment">The adjustment to the user's current points</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the object</returns>
        public PointsAdjustment AddPointsAdjustment(User theUser, PointsManager thePointsManager, PointsAdjustmentReason theReason, decimal totalAdjustment, decimal currentAdjustment, decimal? cashValue)
        {
            PointsAdjustment theAdjustment = new PointsAdjustment
            {
                PointsAdjustmentID = Guid.NewGuid(),
                User = theUser,
                PointsManager = thePointsManager,
                Reason = (int)theReason,
                TotalAdjustment = totalAdjustment,
                CurrentAdjustment = currentAdjustment,
                CashValue = cashValue,
                WhenMade = TestableDateTime.Now,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<PointsAdjustment>().InsertOnSubmit(theAdjustment);
            DataContext.RegisterObjectToBeInserted(theAdjustment);
            return theAdjustment;
        }

        /// <summary>
        /// Adds a PointsTotal object, allowing a user to accumulate points in a new universe. Points are
        /// initially set to zero.
        /// </summary>
        /// <param name="userID">The user</param>
        /// <param name="pointsManagerID">The universe</param>
        /// <returns>The id of the added object</returns>
        public PointsTotal AddPointsTotal(User user, PointsManager pointsManager)
        {
            var existingTotal = DataContext.NewOrFirstOrDefault<PointsTotal>(x => x.User.UserID == user.UserID && x.PointsManager.PointsManagerID == pointsManager.PointsManagerID); // could have just been added
            if (existingTotal != null)
                return existingTotal;
            PointsTotal theTotal = new PointsTotal
            {
                PointsTotalID = Guid.NewGuid(),
                User = user,
                PointsManagerID = pointsManager.PointsManagerID, // note that pointsManager may be from different data context, if loaded from cache by routing info
                CurrentPoints = 0,
                TotalPoints = 0,
                PotentialMaxLossOnNotYetPending = 0,
                PendingPoints = 0,
                PointsPumpingProportionAvg_Numer = 0,
                PointsPumpingProportionAvg_Denom = 0,
                PointsPumpingProportionAvg = 0.0F
            };
            DataContext.GetTable<PointsTotal>().InsertOnSubmit(theTotal);
            DataContext.RegisterObjectToBeInserted(theTotal);
            return theTotal;
        }

        

        public UserCheckIn AddUserCheckIn(User user, DateTime checkInTime, PointsTotal thePointsTotal)
        {
            UserCheckIn theCheckIn = new UserCheckIn
            {
                UserCheckInID = Guid.NewGuid(),
                User = user,
                CheckInTime = checkInTime
            };
            DataContext.GetTable<UserCheckIn>().InsertOnSubmit(theCheckIn);
            DataContext.RegisterObjectToBeInserted(theCheckIn);

            RaterTime.UpdateTimeForUser(thePointsTotal, TestableDateTime.Now);

            return theCheckIn;
        }



        /// <summary>
        /// Adds a prediction group. THis is called before any predictions are added.
        /// </summary>
        /// <param name="ratingGroupID">The rating group (top of the hierarchy) for which predictions are being made</param>
        /// <returns>The id of the prediction group</returns>
        public UserRatingGroup AddUserRatingGroup(RatingGroup topRatingGroup)
        {
            DateTime currentTime = TestableDateTime.Now;

            bool didAdvance;
            bool ratingExpired;
            RatingGroupPhaseStatus currentRatingGroupPhaseStatus;
            AdvanceRatingGroupIfNeeded(topRatingGroup, out didAdvance, out ratingExpired, out currentRatingGroupPhaseStatus);
            if (ratingExpired || currentRatingGroupPhaseStatus == null)
                throw new UserRatingDataException("You cannot enter a rating, because this table cell is completed."); // Note that AddUserRatings routines depend on this exact language
            if (didAdvance)
                currentTime = TestableDateTime.Now; // Get a time that will be after creation of the new rating phase status, but only if we advance the rating group, because we don't want to end up with a time just after the end of a valid rating phase status

            UserRatingGroup theGroup = new UserRatingGroup
            {
                UserRatingGroupID = Guid.NewGuid(),
                RatingGroup = topRatingGroup,
                RatingGroupPhaseStatus = currentRatingGroupPhaseStatus,
                WhenCreated = TestableDateTime.Now
            };
            DataContext.GetTable<UserRatingGroup>().InsertOnSubmit(theGroup);
            DataContext.RegisterObjectToBeInserted(theGroup);

            return theGroup;
        }

        /// <summary>
        /// Adds a prediction to a prediction group. Points information (e.g., max loss from prediction) is calculated,
        /// the rating status is updated, and the prediction status is updated.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rating"></param>
        /// <param name="userRatingGroup"></param>
        /// <param name="pointsTotal"></param>
        /// <param name="ratingPhaseStatus"></param>
        /// <param name="ratingGroups"></param>
        /// <param name="enteredUserRatingValue"></param>
        /// <param name="newUserRatingValue"></param>
        /// <param name="lastTrustedRatingOrBasisOfCalc"></param>
        /// <param name="madeDirectly"></param>
        /// <param name="additionalInfo"></param>
        /// <returns></returns>
        internal UserRating AddUserRating(User user, Rating rating, UserRatingGroup userRatingGroup, PointsTotal pointsTotal, 
            RatingPhaseStatus ratingPhaseStatus, List<RatingGroup> ratingGroups, 
            decimal enteredUserRatingValue, decimal newUserRatingValue, decimal lastTrustedRatingOrBasisOfCalc, 
            bool madeDirectly, UserRatingHierarchyAdditionalInfo additionalInfo)
        {
            //Trace.TraceInformation("AddUserRating " + enteredUserRating);

            bool userRatingIsFromSuperUser = user.SuperUser;

            TblRow tblRow = rating.RatingGroup.TblRow;
            TblColumn tblCol = rating.RatingGroup.TblColumn;
            Tbl tbl = tblRow.Tbl;
            RatingGroup topmostRatingGroup = ratingGroups.Single(m => m.RatingGroupID == rating.TopmostRatingGroupID);

            RewardPendingPointsTracker theRewardPendingPointsTracker = null;
            if (!userRatingIsFromSuperUser)
                theRewardPendingPointsTracker = tblRow.RewardPendingPointsTrackers.SingleOrDefault();
            PointsManager pointsManager = tbl.PointsManager;
            TrustTrackerUnit trustTrackerUnit = tblCol.TrustTrackerUnit ?? pointsManager.TrustTrackerUnit;
            //decimal? origValue = theRating.CurrentValue;

            if (enteredUserRatingValue < rating.RatingCharacteristic.MinimumUserRating)
                throw new UserRatingDataException("The rating must be at least " + rating.RatingCharacteristic.MinimumUserRating);
            if (enteredUserRatingValue > rating.RatingCharacteristic.MaximumUserRating)
                throw new UserRatingDataException("The rating must be no more than " + rating.RatingCharacteristic.MaximumUserRating);

            DateTime whenPending = TestableDateTime.Now.AddSeconds(10); // we want to generate a recalculation even if it's supposed to be pending immediately
            int extraTime = 0;
            RatingGroupPhaseStatus ratingGroupPhaseStatus = ratingPhaseStatus.RatingGroupPhaseStatus;
            double halfLife = (double)ratingGroupPhaseStatus.RatingPhase.HalfLifeForResolution;
            if (halfLife > 0)
            { // We will approximately use the half life for resolution, which ordinarily is shorter than the minimum time for resolution, as the time before which ratings become pending. More specifically, we will use half of the half life plus adding a random amount of time based on a half life of a quarter of the half life.
                extraTime = (int)((0 - 0.25 * halfLife) * Math.Log(RandomGenerator.GetRandom()));
                int totalTime = extraTime + (int) (0.5 * (double)halfLife);
                whenPending += new TimeSpan(0, 0, totalTime);
            }

            NoviceHighStakesSettings noviceHighStakesSettings = null;
            if (!userRatingIsFromSuperUser)
            {
                noviceHighStakesSettings = UseNoviceHighStakes(pointsTotal, pointsManager, tblRow.TblRowID);
                if (noviceHighStakesSettings.UseNoviceHighStakes)
                {
                    ratingGroupPhaseStatus.HighStakesNoviceUser = true;
                    ratingGroupPhaseStatus.HighStakesNoviceUserAfter = TestableDateTime.Now + new TimeSpan(0, 10, 0);
                }
            }
            else
                noviceHighStakesSettings = new NoviceHighStakesSettings() { UseNoviceHighStakes = false, HighStakesMultiplierOverride = 1.0M };

            decimal? previousUserRating = lastTrustedRatingOrBasisOfCalc; // this should equal rating.CurrentValue if rating.CurrentValue != null, since we made sure of that above
            if (rating.CurrentValue != null && rating.CurrentValue != lastTrustedRatingOrBasisOfCalc)
                throw new Exception("Internal error: Rating current value should equal last trusted value.");

            decimal maxLossLongTerm, maxGainLongTerm, profitLongTerm, maxLossShortTerm, maxGainShortTerm, profitShortTerm;

            DateTime currentTime = userRatingGroup.WhenCreated;

            decimal? oldLastTrustedUserRating = rating.LastTrustedValue;
            decimal? newLastTrustedUserRating;
            //if (additionalInfo.IsTrusted)
                newLastTrustedUserRating = newUserRatingValue;
            //else
            //    newLastTrustedUserRating = oldLastTrustedUserRating;

            decimal forecastFutureRating = newUserRatingValue;
            //if (!additionalInfo.IsTrusted && newLastTrustedUserRating != null)
            //    forecastFutureRating = (decimal) newLastTrustedUserRating;

            decimal profitShortTermUnweighted, profitLongTermUnweighted;

            // We will multiply points by the past points pumping proportion. We record this so that
            // the multiplier will stay the same for this UserRating, though later UserRatings may have
            // a different multiplier, if there is a danger of a points-pumping scheme.
            decimal pastPointsPumpingProportion;
            if (pointsTotal == null)
                pastPointsPumpingProportion = 0.0M;
            else
                pastPointsPumpingProportion = (decimal) pointsTotal.PointsPumpingProportionAvg;

            CalculatePointsInfo(rating, topmostRatingGroup, ratingPhaseStatus.RatingGroupPhaseStatus, currentTime, lastTrustedRatingOrBasisOfCalc, newUserRatingValue, forecastFutureRating, false, rating.RatingGroup.RatingGroupAttribute.LongTermPointsWeight, noviceHighStakesSettings.HighStakesMultiplierOverride, pastPointsPumpingProportion, out maxLossLongTerm, out maxGainLongTerm, out profitLongTerm, out profitLongTermUnweighted); 
            CalculatePointsInfo(rating, topmostRatingGroup, ratingPhaseStatus.RatingGroupPhaseStatus, currentTime, lastTrustedRatingOrBasisOfCalc, newUserRatingValue, forecastFutureRating, true, rating.RatingGroup.RatingGroupAttribute.LongTermPointsWeight, noviceHighStakesSettings.HighStakesMultiplierOverride, pastPointsPumpingProportion, out maxLossShortTerm, out maxGainShortTerm, out profitShortTerm, out profitShortTermUnweighted);

            // We shouldn't get to these two, but it's another check.
            if (newUserRatingValue < rating.RatingCharacteristic.MinimumUserRating)
                newUserRatingValue = rating.RatingCharacteristic.MinimumUserRating;
            if (newUserRatingValue > rating.RatingCharacteristic.MaximumUserRating)
                newUserRatingValue = rating.RatingCharacteristic.MaximumUserRating;

            UpdateUserPointsAndStatus(user, tbl.PointsManager, PointsAdjustmentReason.RatingsUpdate, 0, 0, 0, profitShortTerm + profitLongTerm, maxLossLongTerm + maxLossShortTerm, profitLongTermUnweighted, false, pointsTotal);
            if (pointsTotal == null)
                pointsTotal = AddPointsTotal(user, tbl.PointsManager);
            if (!userRatingIsFromSuperUser)
                RaterTime.UpdateTimeForUser(pointsTotal, currentTime);
            if (pointsTotal.FirstUserRating == null)
                pointsTotal.FirstUserRating = currentTime;


            User previousUser = (rating.UserRating == null) ? null : rating.UserRating.User;
            //UserRating thePreviousUserRating = theRating.UserRatings.OrderByDescending(x => x.UserRatingGroup.WhenMade).FirstOrDefault();
            //if (thePreviousUserRating != null)
            //    previousUser = thePreviousUserRating.User;

            UserRating theUserRating = new UserRating
            {
                UserRatingID = Guid.NewGuid(),
                UserRatingGroup = userRatingGroup,
                Rating = rating,
                RatingPhaseStatus = ratingPhaseStatus,
                User = user,
                TrustTrackerUnit = trustTrackerUnit,
                RewardPendingPointsTrackerID = (theRewardPendingPointsTracker == null) ? (Guid?)null : theRewardPendingPointsTracker.RewardPendingPointsTrackerID,
                UserRating1 = null, /* most recent user rating -- does not include this one */
                PreviousRatingOrVirtualRating = previousUserRating ?? lastTrustedRatingOrBasisOfCalc, // Formerly: lastTrustedRatingOrBasisOfCalc, BUT: we have eliminated the concept of trusted vs. nontrusted ratings, so now we can always use the previous user rating if there is one, resorting to the basis of the calculation if the previous user rating was null.
                PreviousDisplayedRating = previousUserRating,
                EnteredUserRating = enteredUserRatingValue,
                NewUserRating = newUserRatingValue,
                MaxGain = profitShortTerm + profitLongTerm,
                MaxLoss = maxLossShortTerm + maxLossLongTerm,
                PotentialPointsShortTerm = profitShortTerm,
                PotentialPointsLongTerm = profitLongTerm,
                PotentialPointsLongTermUnweighted = profitLongTermUnweighted,
                LongTermPointsWeight = rating.RatingGroup.RatingGroupAttribute.LongTermPointsWeight,
                PointsPumpingProportion = null,
                PastPointsPumpingProportion = pastPointsPumpingProportion,
                OriginalAdjustmentPct = (decimal) additionalInfo.AdjustPct,
                OriginalTrustLevel = (decimal) additionalInfo.OverallTrustLevel,
                MadeDirectly = madeDirectly,
                IsTrusted = true, // all ratings are trusted now
                LongTermResolutionReflected = false,
                ShortTermResolutionReflected = false,
                PointsHaveBecomePending = false,
                ForceRecalculate = false,
                HighStakesKnown = false, // may change below
                HighStakesPreviouslySecret = false, // may change below
                PreviouslyRated = rating.TotalUserRatings > 0,
                SubsequentlyRated = false,
                LogarithmicBase = (rating.RatingGroup.RatingGroupAttribute.RatingCharacteristic.SubsidyDensityRangeGroup == null) ? null : rating.RatingGroup.RatingGroupAttribute.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase,
                HighStakesMultiplierOverride = noviceHighStakesSettings.HighStakesMultiplierOverride,
                WhenPointsBecomePending = whenPending,
                LastModifiedTime = currentTime,
                VolatilityTrackingNextTimeFrameToRemove = (byte) VolatilityDuration.oneHour,
                IsMostRecent10Pct = true, // these will be updated by the recency update background task
                IsMostRecent30Pct = true,
                IsMostRecent70Pct = true,
                IsMostRecent90Pct = true,
                IsUsersFirstWeek = pointsTotal.FirstUserRating == null || pointsTotal.FirstUserRating > currentTime - TimeSpan.FromDays(7),
                LastWeekDistanceFromStart = (decimal) additionalInfo.LastWeekDistanceFromStart,
                LastWeekPushback = (decimal) additionalInfo.LastWeekPushback,
                LastYearPushback = (decimal) additionalInfo.LastYearPushback,
                UserRatingNumberForUser = pointsTotal.NumUserRatings + 1,
                NextRecencyUpdateAtUserRatingNum = RecencyUpdates.GetNextRecencyUpdate(0, pointsTotal.NumUserRatings + 1).TotalUserRatingsAtNextUpdate
            };

            if (theUserRating.NewUserRating - theUserRating.EnteredUserRating <= 0 != theUserRating.EnteredUserRating - theUserRating.PreviousDisplayedRating >= 0 && theUserRating.PreviousDisplayedRating != null && theUserRating.NewUserRating != theUserRating.EnteredUserRating && theUserRating.EnteredUserRating != theUserRating.PreviousDisplayedRating && theUserRating.NewUserRating != theUserRating.PreviousDisplayedRating)
                throw new Exception("User rating moved in wrong direction. Internal error.");

            UpdateUserRatingHighStakesKnownFields(theUserRating, ratingGroupPhaseStatus, userRatingGroup.WhenCreated);

            DataContext.GetTable<UserRating>().InsertOnSubmit(theUserRating);
            DataContext.RegisterObjectToBeInserted(theUserRating);

            List<Guid> allChoiceInGroupIDsToBeTrustTracked = additionalInfo.ChoiceInGroupIDsNotTrackedYet.Union(additionalInfo.TrustTrackerChoiceSummaries.Select(x => x.ChoiceInGroupID)).ToList();
            List<TrustTrackerForChoiceInGroup> choiceInGroupsInDatabaseAlready = DataContext.GetTable<TrustTrackerForChoiceInGroup>().Where(x => allChoiceInGroupIDsToBeTrustTracked.Contains(x.ChoiceInGroupID) && x.User.UserID == user.UserID).ToList();
            List<Guid> notYetInsertedInt = allChoiceInGroupIDsToBeTrustTracked.Except(choiceInGroupsInDatabaseAlready.Select(x => x.ChoiceInGroupID)).ToList();
            List<ChoiceInGroup> notYetInserted = DataContext.GetTable<ChoiceInGroup>().Where(x => additionalInfo.ChoiceInGroupIDsNotTrackedYet.Contains(x.ChoiceInGroupID)).ToList();
            List<TrustTrackerForChoiceInGroup> allTrustTrackersForChoiceInGroups = new List<TrustTrackerForChoiceInGroup>();
            allTrustTrackersForChoiceInGroups.AddRange(choiceInGroupsInDatabaseAlready);
            foreach (var mustInsertChoiceInGroup in notYetInserted)
            {
                TrustTrackerForChoiceInGroup theTrustTrackerForChoiceInGroup = AddTrustTrackerForChoiceInGroup(user, mustInsertChoiceInGroup, tbl);
                allTrustTrackersForChoiceInGroups.Add(theTrustTrackerForChoiceInGroup);
            }
            foreach (var trustTrackerForChoiceInGroup in allTrustTrackersForChoiceInGroups)
                AddTrustTrackerForChoiceInGroupsUserRatingLink(theUserRating, trustTrackerForChoiceInGroup);


            
            bool isHierarchyRatingType = RatingGroupTypesList.hierarchyRatingGroupTypes.Contains(topmostRatingGroup.TypeOfRatingGroup);
            if (!isHierarchyRatingType)
                VolatilityTracking.AddVolatilityForUserRating(theUserRating);
            bool simpleRatingType = !isHierarchyRatingType && RatingGroupTypesList.singleItemNotDate.Contains(topmostRatingGroup.TypeOfRatingGroup);

            // Invalidate the cache for the individual table cell and for the row of table cells.
            CacheManagement.InvalidateCacheDependency("RatingGroupID" + rating.TopmostRatingGroupID.ToString());
            CacheManagement.InvalidateCacheDependency("RatingsForTblRowIDAndTblTabID" + rating.RatingGroup.TblRowID.ToString() + "," + tblCol.TblTabID.ToString());

            rating.TotalUserRatings++;
            //Trace.TraceInformation("2Setting current value to " + newUserRating);
            decimal? previousValue = rating.CurrentValue;
            rating.CurrentValue = newUserRatingValue;
            if (previousValue == null && rating.CurrentValue != null)
            {
                tblCol.NumNonNull++;
                tblCol.ProportionNonNull = (double)tblCol.NumNonNull / ((double)tbl.NumTblRowsActive + (double)tbl.NumTblRowsDeleted);
            }
            rating.LastTrustedValue = newLastTrustedUserRating;
            if (rating.LastTrustedValue != rating.CurrentValue && rating.CurrentValue != null)
                throw new Exception("Internal error: Trusted value should equal current value, since trust concept is eliminated.");
            rating.LastModifiedResolutionTimeOrCurrentValue = currentTime;
            ratingPhaseStatus.NumUserRatingsMadeDuringPhase++;
            pointsTotal.NumUserRatings++;
            if (ratingPhaseStatus.NumUserRatingsMadeDuringPhase > 1) // i.e., there are earlier UserRatings in this phase
            {
                ratingPhaseStatus.TriggerUserRatingsUpdate = true;
            }
            else
            {
                ratingPhaseStatus.TriggerUserRatingsUpdate = true; // we still need to do this so that we adjust user interaction; alternatively, could find a way to just do that
                theUserRating.PointsPumpingProportion = 1.0M; // 100% of points that can be earned are real, because there are no prior users who might not have earned their points from whom these points will come
                pointsTotal.PointsPumpingProportionAvg_Numer += (float)(theUserRating.MaxGain * 1.0M);
                pointsTotal.PointsPumpingProportionAvg_Denom += (float)theUserRating.MaxGain;
                pointsTotal.PointsPumpingProportionAvg = R8RDataManipulation.CalculatePointsPumpingProportionAvg(pointsTotal.PointsPumpingProportionAvg_Numer, pointsTotal.PointsPumpingProportionAvg_Denom, pointsTotal.NumUserRatings);
            }
            if (rating.UserRating != null)
                rating.UserRating.SubsequentlyRated = true; // previous most recent user rating
            rating.UserRating = theUserRating; // now the new one is the most recent user rating
            if (rating.OwnedRatingGroupID != null)
            { // We must change the constrained sum of the owned rating group, if the sum is constrained
                RatingGroup ownedRatingGroup = ratingGroups.First(x => x.RatingGroupID == (Guid)rating.OwnedRatingGroupID);
                if (ownedRatingGroup.RatingGroupAttribute.ConstrainedSum != null)
                    ownedRatingGroup.RatingGroupAttribute.ConstrainedSum = newUserRatingValue;
            }

            if (rating.NumInGroup == 1 && rating.RatingGroupID == rating.TopmostRatingGroupID)
            {
                AddRatingGroupStatusRecord(rating.RatingGroup, rating.RatingGroup.CurrentValueOfFirstRating);
                NeedsRatingScore.SetCountUserPoints(DataContext, tblRow, previousUser, user, !simpleRatingType);
                if (rating.RatingGroup.CurrentValueOfFirstRating == null)
                    rating.RatingGroup.TblRow.CountNonnullEntries++;
                rating.RatingGroup.CurrentValueOfFirstRating = newUserRatingValue;

                rating.RatingGroup.ValueRecentlyChanged = true; 
                StatusRecords.PrepareToRecordRatingChange(DataContext, rating.RatingGroup.TblColumnID);
            }
            if (simpleRatingType && newUserRatingValue != previousValue)
            {
                var farui = new FastAccessRatingUpdatingInfo()
                {
                    TblColumnID = tblCol.TblColumnID,
                    NewValue = newUserRatingValue,
                    StringRepresentation = NumberandTableFormatter.FormatAsSpecified(newUserRatingValue, rating.RatingCharacteristic.DecimalPlaces, tblCol.TblColumnID),
                    RecentlyChanged = true,
                    CountNonNullEntries = tblRow.CountNonnullEntries,
                    CountUserPoints = tblRow.CountUserPoints
                };
                farui.AddToTblRow(tblRow);
            }

            //FindOffsettingUserRatings(theUserRating);

            CheckChangeGroupsLinkedToRating(rating); // See if there are any changes groups linked to this rating

            if (!simpleRatingType) // for multiple choice ratings, we can't use the simplified updating procedure
                throw new NotImplementedException(); // when we add mult choice ratings, we must implement copying to fast access

            //Trace.TraceInformation("Added prediction " + theUserRating.UserRatingID + " at " + theUserRating.UserRatingGroup.WhenMade);
            return theUserRating;
        }

        public UserRatingsToAdd AddUserRatingsToAdd(User theUser, UserRatingHierarchyData predictionHierarchyData)
        {
            //Trace.TraceInformation("AddUserRatingsToAdd entered " + predictionHierarchyData.TheData[0].enteredValueOrCalculatedValue + " afteradjustment:" + predictionHierarchyData.TheData[0].newValueAfterAdjustment);

            UserRatingsToAdd theUserRatingsData = new UserRatingsToAdd
            {
                UserRatingsToAddID = Guid.NewGuid(),
                User = theUser,
                TopRatingGroupID = (Guid)predictionHierarchyData.UserRatingHierarchyEntries.FirstOrDefault().RatingGroupId,
                UserRatingHierarchy = BinarySerializer.Serialize<UserRatingHierarchyData>(predictionHierarchyData),
                WhenCreated = TestableDateTime.Now
            };

            // At some point we might decide not to use SQL Server to store these.
            // Instead, we'd use Azure Tables to store the detailed information, and Azure Queue to keep track
            // of the TopRatingGroupID in order. 
            // When we implement this, we will need to switch the commenting out below, and also do more work -- create
            // a new data structure that we can serialize of the UserRatingsToAdd, and then when loading them, load all of
            // the other relevant objects (e.g., user).
            // The other challenges for implementing this are in CompleteMultipleAddUserRatings and AddRatingGroupToUserRatingHierarchy.
            DataContext.GetTable<UserRatingsToAdd>().InsertOnSubmit(theUserRatingsData);
            DataContext.RegisterObjectToBeInserted(theUserRatingsData);
            //AzureTable<UserRatingsToAdd>.Add(theUserRatingsData, theUserRatingsData.TopRatingGroupID.ToString(), "UserRatingsToAdd");
            //AzureQueue.Push("UserRatingsToAddQueue", theUserRatingsData.TopRatingGroupID.ToString());


            return theUserRatingsData;
        }

        /// <summary>
        /// Adds a ProposalSettings object.
        /// </summary>
        /// <param name="pointsManagerID">The id of the prediction ratings universe.</param>
        /// <param name="TblID">The id of the Tbl. If null, then these are the default settings
        /// for the universe as a whole.</param>
        /// <param name="usersMayProposeAddingTbls">If 1, users can propose entire new Tbls (including associated rating group attributes)</param>
        /// <param name="usersMayProposeResolvingRatings">If 1, users, can propose to resolve ratings and rating groups.</param>
        /// <param name="usersMayProposeChangingTblRows">If 1, users may add, change, and delete table rows.</param>
        /// <param name="usersMayProposeChangingChoiceGroups">If 1, users may inactivate choices in choice groups or add new choices</param>
        /// <param name="usersMayProposeChangingCharacteristics">If 1, users may change rating group attributes, characteristics, subisdy
        /// density ranges, etc. Note that these changes would be effected by terminating and restarting all ratings.</param>
        /// <param name="usersMayProposeChangingColumns">If 1, users may propose changing table column groups and descriptors.</param>
        /// <param name="usersMayProposeChangingUsersRights">If 1, users may propose changing rights of particular or default users</param>
        /// <param name="usersMayProposeAdjustingPoints">If 1, users may propose adjusting particular users' points.</param>
        /// <param name="usersMayProposeChangingProposalSettings">If 1, users may propose changing settings such as these.</param>
        /// <param name="minValueToApprove">The minimum value that must be met before proposed changes can be approved (e.g., 90)</param>
        /// <param name="maxValueToReject">The maximum value that can be met before proposed changes can be rejected.</param>
        /// <param name="minTimePastThreshold">For a change to be accepted or rejected, the threshold must be exceeded for at 
        /// least this many seconds </param>
        /// <param name="minProportionOfThisTime">The threshold will be considered exceeded over a span of time if it is exceeded
        /// for at least this proportion of the time.</param>
        /// <param name="bonusForProposal">How many points a user whose proposal is accepted receives. </param>
        /// <param name="penaltyForRejection">How many points a user whose proposal is rejected loses. A user must have at least
        /// this many points to make a proposal.</param>
        /// <param name="subsidyForApprovalRating">The subsidy level for the rating determining whether a proposal 
        /// is accepted.</param>
        /// <param name="halfLifeForResolvingAtFinalValue">If changes are made to rating characteristics, or if proposals
        /// are made to resolve ratings at their final value (rather than at a particular value), then this
        /// specifies a half life before the changes will go into effect (to prevent last minute user manipulation).</param>
        /// <param name="name">The name of this user settings object</param>
        /// <param name="creator">The creator of this user settings object (or null for system-created)</param>
        /// <param name="status">The status of this object</param>
        /// <returns></returns>
        public Guid AddProposalSettings(Guid? pointsManagerID, Guid? TblID, bool usersMayProposeAddingTbls, bool usersMayProposeResolvingRatings, bool usersMayProposeChangingTblRows,
            bool usersMayProposeChangingChoiceGroups, bool usersMayProposeChangingCharacteristics, bool usersMayProposeChangingColumns, bool usersMayProposeChangingUsersRights, bool usersMayProposeAdjustingPoints,
            bool usersMayProposeChangingProposalSettings, decimal minValueToApprove, decimal maxValueToReject, int minTimePastThreshold, decimal minProportionOfThisTime, int minAdditionalTimeForRewardRating,
            int halfLifeForRewardRating, decimal maxBonusForProposal, decimal maxPenaltyForRejection, decimal subsidyForApprovalRating, decimal subsidyForRewardRating,
            int halfLifeForResolvingAtFinalValue, decimal requiredPointsToMakeProposal,
            String name, Guid? creator)
        {
            ProposalSetting theProposalSettings = new ProposalSetting
            {
                ProposalSettingsID = Guid.NewGuid(),
                PointsManagerID = pointsManagerID,
                TblID = TblID,
                UsersMayProposeAddingTbls = usersMayProposeAddingTbls,
                UsersMayProposeResolvingRatings = usersMayProposeResolvingRatings,
                UsersMayProposeChangingTblRows = usersMayProposeChangingTblRows,
                UsersMayProposeChangingChoiceGroups = usersMayProposeChangingChoiceGroups,
                UsersMayProposeChangingCharacteristics = usersMayProposeChangingCharacteristics,
                UsersMayProposeChangingColumns = usersMayProposeChangingColumns,
                UsersMayProposeChangingUsersRights = usersMayProposeChangingUsersRights,
                UsersMayProposeAdjustingPoints = usersMayProposeAdjustingPoints,
                UsersMayProposeChangingProposalSettings = usersMayProposeChangingProposalSettings,
                MinValueToApprove = minValueToApprove,
                MaxValueToReject = maxValueToReject,
                MinTimePastThreshold = minTimePastThreshold,
                MinProportionOfThisTime = minProportionOfThisTime,
                MinAdditionalTimeForRewardRating = minAdditionalTimeForRewardRating,
                HalfLifeForRewardRating = halfLifeForRewardRating,
                MaxBonusForProposal = maxBonusForProposal,
                MaxPenaltyForRejection = maxPenaltyForRejection,
                SubsidyForApprovalRating = subsidyForApprovalRating,
                SubsidyForRewardRating = subsidyForRewardRating,
                HalfLifeForResolvingAtFinalValue = halfLifeForResolvingAtFinalValue,
                RequiredPointsToMakeProposal = requiredPointsToMakeProposal,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<ProposalSetting>().InsertOnSubmit(theProposalSettings);
            DataContext.SubmitChanges();
            return theProposalSettings.ProposalSettingsID;
        }



        /// <summary>
        /// Adds a subsidy adjustment object
        /// </summary>
        /// <param name="ratingPhaseStatusID">The rating phase status object that this modifies</param>
        /// <param name="subsidyAdjustmentFactor">The subsidy adjustment factor (multiply the subsidy by this)</param>
        /// <param name="endingTime">The ending time of this adjustment, or null for none</param>


        /// <returns></returns>
        public Guid AddSubsidyAdjustment(Guid ratingPhaseStatusID, decimal subsidyAdjustmentFactor, DateTime? endingTime, int? endingTimeHalfLife)
        {

            SubsidyAdjustment theAdjustment = new SubsidyAdjustment
            {
                SubsidyAdjustmentID = Guid.NewGuid(),
                RatingGroupPhaseStatusID = ratingPhaseStatusID,
                SubsidyAdjustmentFactor = subsidyAdjustmentFactor,
                EffectiveTime = TestableDateTime.Now,
                EndingTime = endingTime,
                EndingTimeHalfLife = endingTimeHalfLife
            };

            DataContext.GetTable<SubsidyAdjustment>().InsertOnSubmit(theAdjustment);
            DataContext.SubmitChanges();

            return theAdjustment.SubsidyAdjustmentID;
        }

        public Guid AddRewardPendingPointsTracker(TblRow rewardTblRow, Guid userWhoMadeChangeID)
        {
            RewardPendingPointsTracker theRewardPendingPointsTracker = new RewardPendingPointsTracker
            {
                RewardPendingPointsTrackerID = Guid.NewGuid(),
                PendingRating = null,
                TimeOfPendingRating = null,
                TblRow = rewardTblRow,
                UserID = userWhoMadeChangeID
            };

            DataContext.GetTable<RewardPendingPointsTracker>().InsertOnSubmit(theRewardPendingPointsTracker);
            DataContext.SubmitChanges();

            return theRewardPendingPointsTracker.RewardPendingPointsTrackerID;
        }

        public Guid AddRewardRatingSettings(Guid pointsManagerID, Guid? userActionID, Guid ratingGroupAttributesID, decimal probOfRewardEvaluation, decimal? multiplier, string name, Guid? creator)
        {

            RewardRatingSetting theRewardRatingSetting = new RewardRatingSetting
            {
                RewardRatingSettingsID = Guid.NewGuid(),
                PointsManagerID = pointsManagerID,
                UserActionID = userActionID,
                RatingGroupAttributesID = ratingGroupAttributesID,
                ProbOfRewardEvaluation = probOfRewardEvaluation,
                Multiplier = multiplier ?? (1 / probOfRewardEvaluation),
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };

            DataContext.GetTable<RewardRatingSetting>().InsertOnSubmit(theRewardRatingSetting);
            DataContext.SubmitChanges();

            return theRewardRatingSetting.RewardRatingSettingsID;
        }

        /// <summary>
        /// Adds a subsidy density range object to the database (to set the range parameters, use the other version of the same function).
        /// </summary>
        /// <param name="theSubsidyDensityRangeGroupID">The group to which this range should be added.</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddSubsidyDensityRange(Guid theSubsidyDensityRangeGroupID)
        {

            SubsidyDensityRange theRange = new SubsidyDensityRange
            {
                SubsidyDensityRangeID = Guid.NewGuid(),
                SubsidyDensityRangeGroupID = theSubsidyDensityRangeGroupID,
                Status = (Byte)StatusOfObject.Proposed
            };

            DataContext.GetTable<SubsidyDensityRange>().InsertOnSubmit(theRange);
            DataContext.SubmitChanges();

            return theRange.SubsidyDensityRangeID;
        }

        /// <summary>
        /// Add a subsidy density range with specified attributes. It can automatically
        /// calculate cumulative densities and make other adjustments to ensure that the
        /// entire spectrum from 0 to 1 is set to some value.
        /// </summary>
        /// <param name="theSubsidyDensityRangeGroupID">The group to add the range to</param>
        /// <param name="theRangeBottom">The bottom of the range (must be 0 to 1)</param>
        /// <param name="theRangeTop">The top of the range (must be 0 to 1)</param>
        /// <param name="theLiquidityFactor">The relative amount of liquidity for this portion of the range</param>
        /// <param name="fixGroup">True means that we will ensure that each point from 0 to 1 is assigned
        /// to a unique subsidy density range (for example, by eliminating or narrowing overlapping ranges).</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The added object</returns>
        public Guid AddSubsidyDensityRange(Guid theSubsidyDensityRangeGroupID, Decimal theRangeBottom, Decimal theRangeTop, Decimal theLiquidityFactor, bool fixGroup)
        {
            
            if (theRangeBottom < 0 || theRangeTop > 1 || theRangeBottom > theRangeTop)
                throw new Exception("Invalid liquidity range to create");

            Guid subsidyDensityRangeID = AddSubsidyDensityRange(theSubsidyDensityRangeGroupID);

            SubsidyDensityRange theRange =DataContext.GetTable<SubsidyDensityRange>().Single(x=>x.SubsidyDensityRangeID==subsidyDensityRangeID);
            theRange.RangeBottom = theRangeBottom;
            theRange.RangeTop = theRangeTop;
            theRange.LiquidityFactor = theLiquidityFactor;
            theRange.CumDensityTop = 0;
            DataContext.SubmitChanges();


            if (fixGroup)
            {
                Func<SubsidyDensityRange, bool> otherRangesInGroup = range => range.SubsidyDensityRangeGroupID == theSubsidyDensityRangeGroupID && range.SubsidyDensityRangeID != subsidyDensityRangeID;

                // Let's check to see if this is the first range added. If so, then we should make a surrounding range
                // (which the subsequent code will break up).
                int previousRanges = DataContext.GetTable<SubsidyDensityRange>().
                                        Where(otherRangesInGroup).
                                        Count();
                if (previousRanges == 0)
                {
                    Guid newRangeID = AddSubsidyDensityRange(theSubsidyDensityRangeGroupID);
                    SubsidyDensityRange newRange = DataContext.GetTable<SubsidyDensityRange>().Single(x => x.SubsidyDensityRangeID==newRangeID);
                    newRange.RangeBottom = 0;
                    newRange.RangeTop = 1;
                    newRange.LiquidityFactor = 1;
                    DataContext.SubmitChanges();
                }

                // We have to make sure we have no overlapping ranges, and that the cumulative densities are set appropriately.

                // First, let's eliminate any liquidity ranges that are entirely inbetween theRangeBottom
                // and theRangeTop.
                var rangesToEliminate = DataContext.GetTable<SubsidyDensityRange>().
                                        Where(otherRangesInGroup).
                                        Where(r => r.RangeBottom >= theRangeBottom && r.RangeTop <= theRangeTop);
                foreach (SubsidyDensityRange r in rangesToEliminate)
                {
                    DataContext.GetTable<SubsidyDensityRange>().DeleteOnSubmit(r);
                    DataContext.SubmitChanges();
                }

                // Second, let's find any liquidity ranges that surround the range. We need to turn this into two ranges, one below and one
                // above the new range.
                var rangesToDuplicate = DataContext.GetTable<SubsidyDensityRange>().
                                        Where(otherRangesInGroup).
                                        Where(r => r.RangeBottom < theRangeBottom && r.RangeTop > theRangeTop).
                                        ToList();
                foreach (SubsidyDensityRange r in rangesToDuplicate) // should be only one
                {
                    Guid newRangeID = AddSubsidyDensityRange(theSubsidyDensityRangeGroupID);
                    SubsidyDensityRange newRange = DataContext.GetTable<SubsidyDensityRange>().Single(x => x.SubsidyDensityRangeID==newRangeID);
                    newRange.RangeBottom = theRangeTop;
                    newRange.RangeTop = r.RangeTop;
                    newRange.LiquidityFactor = r.LiquidityFactor;
                    r.RangeTop = theRangeBottom;
                    DataContext.SubmitChanges();
                }

                // Third, let's find any liquidity range whose top goes into the range.
                var rangesToShrink = DataContext.GetTable<SubsidyDensityRange>().
                                        Where(otherRangesInGroup).
                                        Where(r => r.RangeBottom < theRangeBottom && r.RangeTop > theRangeBottom);
                foreach (SubsidyDensityRange r in rangesToShrink) // should be only one
                {
                    r.RangeTop = theRangeBottom;
                    DataContext.SubmitChanges();
                };


                // Fourth, let's find any liquidity range whose bottom goes into the range.
                var rangesToShrink2 = DataContext.GetTable<SubsidyDensityRange>().
                                        Where(otherRangesInGroup).
                                        Where(r => r.RangeBottom < theRangeTop && r.RangeTop > theRangeTop);
                foreach (SubsidyDensityRange r in rangesToShrink2) // should be only one
                {
                    r.RangeBottom = theRangeTop;
                    DataContext.SubmitChanges();
                };

                var theRemainingSubsidyDensityRanges = DataContext.GetTable<SubsidyDensityRange>().
                                                    Where(r => r.SubsidyDensityRangeGroupID == theSubsidyDensityRangeGroupID).
                                                    OrderBy(r => r.RangeBottom);
                Decimal cumDensity = 0;
                foreach (SubsidyDensityRange r in theRemainingSubsidyDensityRanges)
                {
                    r.CumDensityBottom = cumDensity;
                    cumDensity += (r.RangeTop - r.RangeBottom) * r.LiquidityFactor;
                    r.CumDensityTop = cumDensity;
                }
                DataContext.SubmitChanges();

                // Set group's characteristics
                SubsidyDensityRangeGroup theGroup = DataContext.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID==theSubsidyDensityRangeGroupID);
                theGroup.CumDensityTotal = cumDensity;
                DataContext.SubmitChanges();

            }

            return subsidyDensityRangeID;
        }

        /// <summary>
        /// Add a subsidy density range group. Do this before adding subsidy density ranges.
        /// </summary>
        /// <param name="name">The name of the group</param>
        /// <param name="creator">The creator of the group</param>
        /// <param name="theBase">If a simple logarithmic subsidy density range is used, the base of the range</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>

        public Guid AddSubsidyDensityRangeGroup(string name, Guid? creator, decimal? theBase)
        {
            SubsidyDensityRangeGroup theGroup = new SubsidyDensityRangeGroup
            {
                SubsidyDensityRangeGroupID = Guid.NewGuid(),
                UseLogarithmBase = theBase,
                CumDensityTotal = 1,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };

            DataContext.GetTable<SubsidyDensityRangeGroup>().InsertOnSubmit(theGroup);
            DataContext.SubmitChanges();

            return theGroup.SubsidyDensityRangeGroupID;
        }

        public Guid AddSubsidyDensityRangeGroup(string name, Guid? creator)
        {
            return AddSubsidyDensityRangeGroup(name, creator, null);
        }

        public Guid AddTblDimensions(int maxWidthOfImageInRowHeaderCell, int maxHeightOfImageInRowHeaderCell, int maxWidthOfImageInTblRowPopUpWindow, int maxHeightOfImageInTblRowPopUpWindow, int widthOfTblRowPopUpWindow)
        {
            TblDimension theTblDimension = new TblDimension
            {
                TblDimensionsID = Guid.NewGuid(),
                MaxWidthOfImageInRowHeaderCell = maxWidthOfImageInRowHeaderCell,
                MaxHeightOfImageInRowHeaderCell = maxHeightOfImageInRowHeaderCell,
                MaxWidthOfImageInTblRowPopUpWindow = maxWidthOfImageInTblRowPopUpWindow,
                MaxHeightOfImageInTblRowPopUpWindow = maxHeightOfImageInTblRowPopUpWindow,
                WidthOfTblRowPopUpWindow = widthOfTblRowPopUpWindow
            };
            DataContext.GetTable<TblDimension>().InsertOnSubmit(theTblDimension);
            DataContext.SubmitChanges();
            return theTblDimension.TblDimensionsID;

        }

        /// <summary>
        /// Add a text field (that is, specific text for a field for a particular row).
        /// </summary>
        /// <param name="fieldID">The field for which we are providing text</param>
        /// <param name="text">The text</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns></returns>
        public TextField AddTextField(Field field, string text, string link, bool indexSearchWords)
        {
            text = text.Trim();
            TextField theTextField = new TextField
            {
                TextFieldID = Guid.NewGuid(),
                Field = field,
                Text = text,
                Link = link,
                Status = (Byte)StatusOfObject.Active
            };
            DataContext.GetTable<TextField>().InsertOnSubmit(theTextField);
            DataContext.RegisterObjectToBeInserted(theTextField);
            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + field.TblRowID);
            return theTextField;
        }

        public Guid AddTextFieldDefinition(Guid FieldDefinitionID, bool includeText, bool includeLink, bool searchable)
        {
            if (!includeText && !includeLink)
                throw new Exception("Text field must include text, link, or both.");
            TextFieldDefinition theTextFieldDefinition = new TextFieldDefinition
            {
                TextFieldDefinitionID = Guid.NewGuid(),
                FieldDefinitionID = FieldDefinitionID,
                IncludeText = includeText,
                IncludeLink = includeLink,
                Searchable = searchable,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<TextFieldDefinition>().InsertOnSubmit(theTextFieldDefinition);
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + DataContext.GetTable<FieldDefinition>().Single(f => f.FieldDefinitionID == FieldDefinitionID).Tbl.PointsManagerID);
            return theTextFieldDefinition.TextFieldDefinitionID;
        }

        /// <summary>
        /// Add a new prediction rating universe. Adds an administration Tbl to the universe.
        /// </summary>
        /// <param name="defaultRatingGroupAttributesID">The default rating group attributes for Tbls</param>
        /// <param name="PointsTrustRulesID">The counting rules for the universe</param>
        /// <param name="name">The name of the universe</param>
        /// <param name="creator">The user who created the universe</param>
        /// <param name="status">The status of the object and table</param>
        /// <returns>The id of the added object</returns>
        public Guid AddPointsManager(Guid domainID, String name, Guid? creator)
        {

            PointsManager thePointsManagers = new PointsManager
            {
                PointsManagerID = Guid.NewGuid(),
                DomainID = domainID,
                TrustTrackerUnit = AddTrustTrackerUnit(),
                CurrentPeriodDollarSubsidy = 0,
                EndOfDollarSubsidyPeriod = null,
                NextPeriodDollarSubsidy = 0,
                NextPeriodLength = null,
                NumPrizes = 0,
                MinimumPayment = 0,
                TotalUserPoints = 0,
                CurrentUserPoints = 0,
                CurrentPointsToCount = -99999,
                NumUsersMeetingUltimateStandard = 0,
                NumUsersMeetingCurrentStandard = 0,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<PointsManager>().InsertOnSubmit(thePointsManagers);
            DataContext.SubmitChanges();
            
            CacheManagement.InvalidateCacheDependency("DomainID" + domainID);
            CacheManagement.InvalidateCacheDependency("TopicsMenu");

            return thePointsManagers.PointsManagerID;
        }

        public TrustTrackerForChoiceInGroup AddTrustTrackerForChoiceInGroup(User theUser, ChoiceInGroup theChoiceInGroup, Tbl theTbl)
        {
            TrustTrackerForChoiceInGroup theTracker = new TrustTrackerForChoiceInGroup
            {
                TrustTrackerForChoiceInGroupID = Guid.NewGuid(),
                User = theUser,
                ChoiceInGroup = theChoiceInGroup,
                Tbl = theTbl,
                SumAdjustmentPctTimesRatingMagnitude = 0,
                SumRatingMagnitudes = 0,
                TrustLevelForChoice = 1.0F
            };

            DataContext.GetTable<TrustTrackerForChoiceInGroup>().InsertOnSubmit(theTracker);
            DataContext.RegisterObjectToBeInserted(theTracker);

            return theTracker;
        }

        public TrustTrackerForChoiceInGroupsUserRatingLink AddTrustTrackerForChoiceInGroupsUserRatingLink(UserRating theUserRating, TrustTrackerForChoiceInGroup theTrustTrackerForChoiceInGroup)
        {
            TrustTrackerForChoiceInGroupsUserRatingLink theLink = new TrustTrackerForChoiceInGroupsUserRatingLink
            {
                TrustTrackerForChoiceInGroupsUserRatingLinkID = Guid.NewGuid(),
                UserRating = theUserRating,
                TrustTrackerForChoiceInGroup = theTrustTrackerForChoiceInGroup
            };

            DataContext.GetTable<TrustTrackerForChoiceInGroupsUserRatingLink>().InsertOnSubmit(theLink);
            DataContext.RegisterObjectToBeInserted(theLink);

            return theLink;
        }



        /// <summary>
        /// add the user information to the User Info Table
        /// </summary>
        /// <param name="userID"></param> ID of the New user added
        /// <param name="firstName"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <param name="workphone"></param>
        /// <param name="homephone"></param>
        /// <param name="mobilephone"></param>
        /// <param name="password"></param>
        /// <param name="Zipcode"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        public void AddUserInfo(Guid userID, string firstName, string lastname, string email, string address1, string address2, string workphone, string homephone, string mobilephone, string password, string zipcode, string city, string state, string country)
        {

            UserInfo theUserInfo = new UserInfo
            {
                UserInfoID = Guid.NewGuid(),
                UserID = userID,
                FirstName = firstName,
                LastName = lastname,
                Email = email,
                Address1 = address1,
                Address2 = address2,
                WorkPhone = workphone,
                HomePhone = homephone,
                MobilePhone = mobilephone,
                Password = password,
                ZipCode = zipcode,
                City = city,
                State = state,
                Country = country,
                IsVerified = false

            };

            DataContext.GetTable<UserInfo>().InsertOnSubmit(theUserInfo);
            DataContext.SubmitChanges();
        }

        /// <summary>
        /// Add a user rights object, defining the rights that a user (or the default user)
        /// has in a particular universe. 
        /// </summary>
        /// <param name="userID">The user, or null to define rights of users not otherwise specified</param>
        /// <param name="pointsManagerID">The universe, or null if none, to which this applies</param>
        /// <param name="mayView">Can view the ratings and predictions</param>
        /// <param name="mayPredict">Can make predictions</param>
        /// <param name="mayAddTbls">Can add Tbls</param>
        /// <param name="mayResolveRatings">Can resolve ratings and rating groups</param>
        /// <param name="mayChangeTblRows">Can change entities' fields, add table rows, etc.</param>
        /// <param name="mayChangeChoiceGroups">Can add to and inactivate choice group entries</param>
        /// <param name="mayChangeCharacteristics">Can change rating characteristics, group attributes</param>
        /// <param name="mayChangeColumns">Can add/delete table columns and groups</param>
        /// <param name="mayChangeUsersRights">Can add/delete users rights objects</param>
        /// <param name="mayAdjustPoints">Can make adjustments to users' points</param>
        /// <param name="mayChangeProposalSettings">Can change proposal settings</param>
        /// <param name="name">The name of this user settings object</param>
        /// <param name="creator">The creator of this user settings object (or null for system-created)</param>
        /// <param name="status">The status of the object and table</param>
        /// 
        public Guid AddUsersRights(Guid? userID, Guid? pointsManagerID, bool mayView, bool mayPredict, bool mayAddTbls,
            bool mayResolveRatings, bool mayChangeTblRows, bool mayChangeChoiceGroups, bool mayChangeCharacteristics,
            bool mayChangeColumns, bool mayChangeUsersRights, bool mayAdjustPoints, bool mayChangeProposalSettings,
            String name, Guid? creator)
        {
            UsersRight theUsersRights = new UsersRight
            {
                UsersRightsID = Guid.NewGuid(),
                UserID = userID,
                PointsManagerID = pointsManagerID,
                MayView = mayView,
                MayPredict = mayPredict,
                MayAddTbls = mayAddTbls,
                MayResolveRatings = mayResolveRatings,
                MayChangeTblRows = mayChangeTblRows,
                MayChangeChoiceGroups = mayChangeChoiceGroups,
                MayChangeCharacteristics = mayChangeCharacteristics,
                MayChangeCategories = mayChangeColumns,
                MayChangeUsersRights = mayChangeUsersRights,
                MayAdjustPoints = mayAdjustPoints,
                MayChangeProposalSettings = mayChangeProposalSettings,
                Name = name,
                Creator = creator,
                Status = (Byte)StatusOfObject.Proposed
            };
            DataContext.GetTable<UsersRight>().InsertOnSubmit(theUsersRights);
            DataContext.SubmitChanges();
            return theUsersRights.UsersRightsID;
        }

        public TrustTrackerUnit AddTrustTrackerUnit()
        {
            TrustTrackerUnit ttu = new TrustTrackerUnit()
                {
                    TrustTrackerUnitID = Guid.NewGuid(),
                    ExtendIntervalMultiplier = TrustTrackerUnit.DefaultExtendIntervalMultiplier,
                    MinUpdateIntervalSeconds = TrustTrackerUnit.DefaultMinUpdateIntervalSeconds,
                    MaxUpdateIntervalSeconds = TrustTrackerUnit.DefaultMaxUpdateIntervalSeconds,
                    ExtendIntervalWhenChangeIsLessThanThis = TrustTrackerUnit.DefaultExtendIntervalWhenChangeIsLessThanThis
                };
            DataContext.GetTable<TrustTrackerUnit>().InsertOnSubmit(ttu);

            // now, add trust trackers for the admin account
            User admin = DataContext.GetTable<User>().Single(x => x.Username == "admin");
            TrustTrackingBackgroundTasks.AddTrustTracker(DataContext, admin, ttu);

            return ttu;
        }

        public UniquenessLock AddUniquenessLock(object objectToHashIntoUniquenessLock, DateTime? deletionTime)
        {
            UniquenessLock ul = new UniquenessLock()
            {
                Id = MD5HashGenerator.GetDeterministicGuid(objectToHashIntoUniquenessLock), // Note: It's critical NOT to just create a regular Guid
                DeletionTime = deletionTime
            };
            DataContext.GetTable<UniquenessLock>().InsertOnSubmit(ul);
            return ul; 
        }

        /// </summary>
        /// Add a new user 
        /// <param name="userName"></param>
        /// <param name="superUser"></param>
        /// <param name="status"></param>
        /// <param name="firstName"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <param name="workphone"></param>
        /// <param name="homephone"></param>
        /// <param name="mobilephone"></param>
        /// <param name="password"></param>
        /// <param name="Zipcode"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <returns>the ID of the user added</returns>
        public User AddUser(String userName, string email, string password, bool isSuperUser = false, bool profileAlreadyAdded = false)
        {
            IUserProfileInfo theUserProfileInfo;
            if (profileAlreadyAdded)
                theUserProfileInfo = UserProfileCollection.LoadByUsername(userName);
            else
                theUserProfileInfo = UserProfileCollection.CreateUser(userName, password, email);
            User theUser = new User {
                UserID = Guid.NewGuid(),
                Username = userName,
                SuperUser = isSuperUser,
                WhenCreated = TestableDateTime.Now,
                Status = (Byte)StatusOfObject.Active
            };

            DataContext.GetTable<User>().InsertOnSubmit(theUser);
            DataContext.SubmitChanges();

            theUserProfileInfo.SetProperty("UserID", theUser.UserID);

            return theUser;
        }

        public Guid AddUserReturnId(String userName, bool isSuperUser, string email, string password, bool profileAlreadyAdded)
        {
            return AddUser(userName, email, password, isSuperUser, profileAlreadyAdded).UserID;
        }


        public Guid AddUserAction(Guid userActionID, string text, bool superUser)
        {
            UserAction theUserAction = new UserAction
            {
                UserActionID = userActionID, // i.e. deterministic IDs
                Text = text,
                SuperUser = superUser
            };
            DataContext.GetTable<UserAction>().InsertOnSubmit(theUserAction);
            DataContext.SubmitChanges();
            return theUserAction.UserActionID;
        }

        public VolatilityTblRowTracker AddVolatilityTblRowTracker(TblRow theTblRow, VolatilityDuration theTimeFrame)
        {
            DateTime currentTime = TestableDateTime.Now;
            VolatilityTblRowTracker theTracker = new VolatilityTblRowTracker
            {
                VolatilityTblRowTrackerID = Guid.NewGuid(),
                TblRow = theTblRow,
                DurationType = (byte) theTimeFrame,
                TotalMovement = 0,
                DistanceFromStart = 0,
                Pushback = 0,
                PushbackProportion = 0
            };
            DataContext.GetTable<VolatilityTblRowTracker>().InsertOnSubmit(theTracker);
            DataContext.RegisterObjectToBeInserted(theTracker);
            new FastAccessVolatilityUpdateInfo() { TimeFrame = (VolatilityDuration)theTracker.DurationType, Value = 0 }.AddToTblRow(theTblRow);
            return theTracker;
        }

        public VolatilityTracker AddVolatilityTracker(RatingGroup theRatingGroup,  VolatilityDuration theTimeFrame)
        {
            DateTime currentTime = TestableDateTime.Now;
            VolatilityTblRowTracker theVolatilityTblRowTracker = theRatingGroup.TblRow.VolatilityTblRowTrackers.Single(vet => vet.DurationType == (int) theTimeFrame);
            TimeSpan theTimeSpan = VolatilityTracking.GetTimeSpanForVolatilityTiming(theTimeFrame);
            VolatilityTracker theTracker = new VolatilityTracker
            {
                VolatilityTrackerID = Guid.NewGuid(),
                RatingGroup = theRatingGroup,
                DurationType = (byte) theTimeFrame,
                EndTime = currentTime,
                StartTime = currentTime - theTimeSpan,
                VolatilityTblRowTracker = theVolatilityTblRowTracker,
                TotalMovement = 0,
                DistanceFromStart = 0,
                Pushback = 0,
                PushbackProportion = 0
            };
            DataContext.GetTable<VolatilityTracker>().InsertOnSubmit(theTracker);
            DataContext.RegisterObjectToBeInserted(theTracker);
            return theTracker;
        }
    }
}
