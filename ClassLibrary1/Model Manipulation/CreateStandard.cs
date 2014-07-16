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

using ClassLibrary1.EFModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Diagnostics;
using System.Text;
using GoogleGeocoder;

using System.Threading;

using System.IO;

using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public class StandardObjectsForEachPointsManager
    {
        public Guid eventRGA;
        public Guid ratingRGA;
        public Guid ratingNegPosRGA;
        public Guid eventShortRGA;
        public Guid ratingShortRGA;
        public Guid eventVeryShortRGA;
        public Guid ratingVeryShortRGA;
        public Guid eventLongRGA;
        public Guid ratingLongRGA;
        public Guid eventVeryLongRGA;
        public Guid ratingVeryLongRGA;
        public Guid theRatingPhaseStandardID;
        public Guid theRatingPhaseShortID;
        public Guid theRatingPhaseVeryShortID;
        public Guid theRatingPhaseLongID;
        public Guid theRatingPhaseVeryLongID;
    }

    public abstract class CreateStandardBase
    {
        protected ActionProcessor Action;

        internal Dictionary<int, StandardObjectsForEachPointsManager> standardObjectsForPointsManager = new Dictionary<int, StandardObjectsForEachPointsManager>();

        protected Domain standardDomain;
        protected HierarchyItem sportsHierarchy;
        protected HierarchyItem entertainmentHierarchy;
        protected HierarchyItem internetHierarchy;
        protected HierarchyItem governmentHierarchy;
        protected HierarchyItem consumerHierarchy;
        protected HierarchyItem newsHierarchy;

        protected int superUser;

        protected int? myChangesGroup;

        protected int visibleWithNameFieldDisplay;
        protected int visibleWithNoNameFieldDisplay;
        protected int doNotShowThisField;
        protected int continuationWithNameFieldDisplay;
        protected int continuationWithNoNameFieldDisplay;
        protected int pictureFieldDisplayTopRight;
        protected int pictureFieldDisplayInOrder;

        protected int visibleWithNameFieldDisplayLarger;
        protected int visibleWithNoNameFieldDisplayLarger;
        protected int doNotShowThisFieldLarger;
        protected int continuationWithNameFieldDisplayLarger;
        protected int continuationWithNoNameFieldDisplayLarger;
        protected int pictureFieldDisplayTopRightLarger;
        protected int pictureFieldDisplayInOrderLarger;

        protected int visibleWithNameFieldDisplaySmaller;
        protected int visibleWithNoNameFieldDisplaySmaller;
        protected int doNotShowThisFieldSmaller;
        protected int continuationWithNameFieldDisplaySmaller;
        protected int continuationWithNoNameFieldDisplaySmaller;
        protected int pictureFieldDisplayTopRightSmaller;
        protected int pictureFieldDisplayInOrderSmaller;

        public CreateStandardBase()
        {
            Action = new ActionProcessor();
            CreateStandardFieldDefinitionDisplaySettings();
        }

        protected int CreateRatingPhaseGroupShort()
        {
            List<RatingPhaseData> thePhases = new List<RatingPhaseData>();
            RatingPhaseData oneHourMinimumPhases = new RatingPhaseData(100, ScoringRules.SquareRoot, true, false, null, 60 * 60 * 1 * 1, 60 * 30 * 1 * 1, true, null);
            thePhases.Add(oneHourMinimumPhases);
            return Action.RatingPhaseGroupCreate(thePhases, "One hour minimum phases", true, true, superUser, null);
        }

        protected int CreateRatingPhaseGroupVeryShort()
        {
            List<RatingPhaseData> thePhases = new List<RatingPhaseData>();
            RatingPhaseData tenMinuteMinimumPhases = new RatingPhaseData(100, ScoringRules.SquareRoot, true, false, null, 60 * 10 * 1 * 1, 60 * 3 * 24 * 1, true, null);
            thePhases.Add(tenMinuteMinimumPhases);
            return Action.RatingPhaseGroupCreate(thePhases, "Ten minute minimum phases", true, true, superUser, null);
        }

        protected int CreateRatingPhaseGroupABitShort()
        {
            List<RatingPhaseData> thePhases = new List<RatingPhaseData>();
            RatingPhaseData oneDayMinimumPhases = new RatingPhaseData(100, ScoringRules.SquareRoot, true, false, null, 60 * 60 * 24 * 1, 60 * 60 * 12 * 1, true, null);
            thePhases.Add(oneDayMinimumPhases);
            return Action.RatingPhaseGroupCreate(thePhases, "One day minimum phases", true, true, superUser, null);
        }

        protected int CreateRatingPhaseGroupStandard()
        {
            List<RatingPhaseData> thePhases = new List<RatingPhaseData>();
            RatingPhaseData threeDayMinimumPhases = new RatingPhaseData(100, ScoringRules.SquareRoot, true, false, null, 60 * 60 * 24 * 3, 60 * 60 * 24 * 1, true, null);
            thePhases.Add(threeDayMinimumPhases);
            return Action.RatingPhaseGroupCreate(thePhases, "Three day minimum phases", true, true, superUser, null);
        }

        protected int CreateRatingPhaseGroupLong()
        {
            List<RatingPhaseData> thePhases = new List<RatingPhaseData>();
            RatingPhaseData fourteenDayMinimumPhases = new RatingPhaseData(100, ScoringRules.SquareRoot, true, false, null, 60*60*24*14, 60*60*24*2, true, null);
            thePhases.Add(fourteenDayMinimumPhases);
            return Action.RatingPhaseGroupCreate(thePhases, "Two week minimum phases", true, true, superUser, null);
        }


        protected int CreateRatingPhaseGroupVeryLong()
        {
            List<RatingPhaseData> thePhases = new List<RatingPhaseData>();
            RatingPhaseData sixtyDayMinimumPhases = new RatingPhaseData(100, ScoringRules.SquareRoot, true, false, null, 60 * 60 * 24 * 60, 60 * 60 * 24 * 7, true, null);
            thePhases.Add(sixtyDayMinimumPhases);
            return Action.RatingPhaseGroupCreate(thePhases, "Two months minimum phases", true, true, superUser, null);
        }

        protected int CreateTblTab(string name, Guid TblID)
        {
            return Action.TblTabCreate(TblID, name, true, true, superUser, null);
        }

        protected int CreateTblColumn(string abbreviation, string name, string explanation, int TblTab, int defaultRatingGroupAttributes, bool useAsFilter, bool sortable, bool sortAsc, bool trackTrustWithinTableColumn, string widthStyle = "wv10")
        {
            int theCD = Action.TblColumnCreate(TblTab, defaultRatingGroupAttributes, abbreviation, name, explanation, widthStyle, trackTrustWithinTableColumn, true, true, superUser, null);
            Action.TblColumnChangeSortOptions(theCD, useAsFilter, sortable, sortAsc, true, superUser, null);
            return theCD;
        }

        protected Guid CreateChoiceGroup(Guid pointsManagerID, ChoiceGroupData theChoiceGroupData, int choiceGroupSettings, Guid? dependentOnChoiceGroupID, string name)
        {
            Guid theChoiceGroup = Action.ChoiceGroupCreate(pointsManagerID, theChoiceGroupData, choiceGroupSettings, dependentOnChoiceGroupID, true, true, superUser, null, name);
            return theChoiceGroup;
        }

        public Guid CreateChoiceGroup(Guid pointsManagerID, string[] theChoices, string name, int? choiceGroupSettings = null)
        {
            if (choiceGroupSettings == null)
                choiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false,true,false,false,false,true,true,true);
            ChoiceGroupData theChoiceGroupData = new ChoiceGroupData();
            foreach (var choice in theChoices)
                theChoiceGroupData.AddChoiceToGroup(choice);
            Guid theChoiceGroupID = CreateChoiceGroup(pointsManagerID, theChoiceGroupData, (int)choiceGroupSettings, null, name);
            return theChoiceGroupID;
        }

        public Guid CreateDependentChoiceGroup(Guid pointsManagerID, Guid choiceGroupThisIsDependentOnID, DepItemsLevel[] theDepItems, string name, int? choiceGroupSettings = null)
        {
            if (choiceGroupSettings == null)
                choiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, true, false, false, false, true, true, true);

            ChoiceGroupData theChoiceGroupData = new ChoiceGroupData();
            foreach (var depItemSet in theDepItems)
            {
                int existingChoice = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == depItemSet.DependentOnString && x.ChoiceGroupID == choiceGroupThisIsDependentOnID).ChoiceInGroupID;
                foreach (var item in depItemSet.NewItems)
                {
                    theChoiceGroupData.AddChoiceToGroup(item, existingChoice);
                }
            }
            Guid theChoiceGroupID = CreateChoiceGroup(pointsManagerID, theChoiceGroupData, (int)choiceGroupSettings, choiceGroupThisIsDependentOnID, name);
            return theChoiceGroupID;
        }

        public List<int> CreateHierarchyChoiceGroup(Guid pointsManagerID, DepItems[] theHierarchy, List<string> name, int? choiceGroupSettings = null)
        {
            List<int> choiceGroupIDs = new List<int>();
            List<DepItemsLevel[]> theDepItemsLevels = new List<DepItemsLevel[]>();
            int hierarchyLevel = 0;
            bool keepGoing = true;
            while (keepGoing)
            {
                theDepItemsLevels.Add(DepItems.GetLevel(theHierarchy, hierarchyLevel));
                keepGoing = theDepItemsLevels[hierarchyLevel].Any();
                if (keepGoing)
                {
                    Guid choiceGroupID;
                    if (hierarchyLevel == 0)
                        choiceGroupID = CreateChoiceGroup(pointsManagerID, theDepItemsLevels[0].Select(x => x.DependentOnString).ToArray(), name[hierarchyLevel], choiceGroupSettings);
                    else
                        choiceGroupID = CreateDependentChoiceGroup(pointsManagerID, choiceGroupIDs[hierarchyLevel - 1], theDepItemsLevels[hierarchyLevel - 1], name[hierarchyLevel], choiceGroupSettings);
                    choiceGroupIDs.Add(choiceGroupID);
                }
                hierarchyLevel++;
            }
            return choiceGroupIDs;
        }

        protected void CreateFieldDefinitionDisplaySettings(Guid fdID, int displayInAllSetting)
        {
            Action.FieldDefinitionChangeDisplaySettings(fdID, displayInAllSetting, displayInAllSetting, displayInAllSetting, true, superUser, null);
        }

        protected void CreateFieldDefinitionDisplaySettings(Guid fdID, int displayInTableSetting, int displayInPopUpSetting, int displayInTblRowPageSetting)
        {
            Action.FieldDefinitionChangeDisplaySettings(fdID, displayInTableSetting, displayInPopUpSetting, displayInTblRowPageSetting, true, superUser, null);
        }

        protected void CreateStandardFieldDefinitionDisplaySettings()
        {
            visibleWithNameFieldDisplay = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, true, true, false, false, true);
            visibleWithNoNameFieldDisplay = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, true, false, false, false, true);
            doNotShowThisField = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, false, false, false, false);
            continuationWithNameFieldDisplay = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, true, false, false, true);
            continuationWithNoNameFieldDisplay = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, false, false, false, true);
            pictureFieldDisplayTopRight = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, true, false, false, false, false, false, true);
            pictureFieldDisplayInOrder = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, true, false, false, false, false, true);

            visibleWithNameFieldDisplayLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, true, true, false, false, true);
            visibleWithNoNameFieldDisplayLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, true, false, false, false, true);
            doNotShowThisFieldLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, false, false, false, false);
            continuationWithNameFieldDisplayLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, true, false, false, true);
            continuationWithNoNameFieldDisplayLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, false, false, false, true);
            pictureFieldDisplayTopRightLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, true, false, false, false, false, false, true);
            pictureFieldDisplayInOrderLarger = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, true, false, false, false, false, true);
            
            visibleWithNameFieldDisplaySmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, true, true, false, false, true);
            visibleWithNoNameFieldDisplaySmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, true, false, false, false, true);
            doNotShowThisFieldSmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, false, false, false, false);
            continuationWithNameFieldDisplaySmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, true, false, false, true);
            continuationWithNoNameFieldDisplaySmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, false, false, false, false, false, true);
            pictureFieldDisplayTopRightSmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, true, false, false, false, false, false, true);
            pictureFieldDisplayInOrderSmaller = FieldsDisplaySettingsMask.GetFieldDisplaySetting(false, false, false, true, false, false, false, false, true);
        }


        protected int CreateFieldDefinition(Guid TblID, string fieldName, FieldTypes theFieldType, bool useAsFilter)
        {
            int theCD = Action.FieldDefinitionCreate(TblID, fieldName, theFieldType, useAsFilter, true, true, superUser, null);
            return theCD;
        }

        protected int CreateFieldDefinition(Guid TblID, string fieldName, FieldTypes theFieldType, bool useAsFilter, bool includeDate, bool includeTime)
        {
            int theCD = Action.FieldDefinitionCreate(TblID, fieldName, theFieldType, useAsFilter, includeDate, includeTime, true, true, superUser, null);
            return theCD;
        }


        protected int CreateFieldDefinition(Guid TblID, string fieldName, FieldTypes theFieldType, bool useAsFilter, bool includeText, bool includeLink, bool searchable)
        {
            int theCD = Action.FieldDefinitionCreate(TblID, fieldName, theFieldType, useAsFilter, includeText, includeLink, searchable, true, true, superUser, null);
            return theCD;
        }

        protected int CreateFieldDefinition(Guid TblID, string fieldName, FieldTypes theFieldType, bool useAsFilter, Guid choiceGroupID, Guid? dependentOnChoiceGroupFieldDefinitionID)
        {
            Guid choiceGroupFieldDefinitionID = 0;
            return CreateFieldDefinition(TblID, fieldName, theFieldType, useAsFilter, choiceGroupID, dependentOnChoiceGroupFieldDefinitionID, ref choiceGroupFieldDefinitionID);
        }
 

        protected int CreateFieldDefinition(Guid TblID, string fieldName, FieldTypes theFieldType, bool useAsFilter, Guid choiceGroupID, Guid? dependentOnChoiceGroupFieldDefinitionID, ref int choiceGroupFieldDefinitionFD)
        {
            int theFD = Action.FieldDefinitionCreate(TblID, fieldName, theFieldType, useAsFilter, choiceGroupID, dependentOnChoiceGroupFieldDefinitionID, true, true, superUser, null);
            choiceGroupFieldDefinitionFD = Action.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == theFD).ChoiceGroupFieldDefinitionID;
            return theFD;
        }

        protected int CreateFieldDefinition(Guid TblID, string fieldName, FieldTypes theFieldType, bool useAsFilter, decimal? minimum, decimal? maximum, short decimalPlaces)
        {
            int theCD = Action.FieldDefinitionCreate(TblID, fieldName, theFieldType, useAsFilter, minimum, maximum, decimalPlaces, true, true, superUser, null);
            return theCD;
        }

        protected int CreateEvent(string abbrevName, string fullName, string explanation, int TblTab, int myRatingPhase, bool useAsFilter, bool sortable, bool sortAsc)
        {
            Guid thePointsManagerID = Action.DataContext.GetTable<TblTab>().Single(c => c.TblTabID == TblTab).Tbl.PointsManagerID;
            int theCD = CreateTblColumn(abbrevName, fullName, explanation, TblTab, standardObjectsForPointsManager[thePointsManagerID].eventRGA, useAsFilter, sortable, sortAsc, false);
            Action.TblColumnFormattingCreate(theCD, "", "%", false, null, null, null, "", "", true, true, superUser, null);
            return theCD;
        }

        protected int CreateRating(string abbrevName, string fullName, string explanation, int TblTab, int myRatingPhase, bool useAsFilter, bool sortable, bool sortAsc, bool trackTrustWithinTableColumn = false)
        {
            Guid thePointsManagerID = Action.DataContext.GetTable<TblTab>().Single(c => c.TblTabID == TblTab).Tbl.PointsManagerID;
            int theCD = CreateTblColumn(abbrevName, fullName, explanation, TblTab, standardObjectsForPointsManager[thePointsManagerID].ratingRGA, useAsFilter, sortable, sortAsc, false);
            Action.TblColumnFormattingCreate(theCD, "", "", false, (decimal) 9.9, (decimal) 9.99, null, "", "", true, true, superUser, null);
            return theCD;
        }

        protected int CreateRatingNegativeToPositive(string abbrevName, string fullName, string explanation, int TblTab, int myRatingPhase, bool useAsFilter, bool sortable, bool sortAsc, bool trackTrustWithinTableColumn = false)
        {
            Guid thePointsManagerID = Action.DataContext.GetTable<TblTab>().Single(c => c.TblTabID == TblTab).Tbl.PointsManagerID;
            int theCD = CreateTblColumn(abbrevName, fullName, explanation, TblTab, standardObjectsForPointsManager[thePointsManagerID].ratingNegPosRGA, useAsFilter, sortable, sortAsc, trackTrustWithinTableColumn);
            Action.TblColumnFormattingCreate(theCD, "", "", false, null, null, null, "", "", true, true, superUser, null);
            return theCD;
        }

        protected int CreateDollarForecastOrRating(string abbrevName, string fullName, string explanation, int TblTab, int myRatingPhase, int decimalPlaces, double min, double max, string widthStyle, bool useAsFilter, bool sortable, bool sortAsc, bool isForecastRatherThanRating, bool trackTrustWithinTableColumn = true)
        {
            int theRatingCharacteristic = Action.RatingCharacteristicsCreate(myRatingPhase,null,(decimal) min,(decimal) max,(short) decimalPlaces,abbrevName,true, true,superUser,null);
            Guid thePointsManagerID = Action.DataContext.GetTable<TblTab>().Single(c => c.TblTabID == TblTab).Tbl.PointsManagerID;
            int theRGA = Action.RatingGroupAttributesCreate(theRatingCharacteristic, null, abbrevName, RatingGroupTypes.singleNumber, null, fullName, !isForecastRatherThanRating, isForecastRatherThanRating ? 0.5M : 0, true, true, superUser, null, thePointsManagerID);
            int theSDRG = Action.SubsidyDensityRangeGroupLogarithmicCreate(10M, "Dollar forecast", true, true, superUser, null);
            int theCD = CreateTblColumn(abbrevName, fullName, explanation, TblTab, theRGA, useAsFilter, sortable, sortAsc, trackTrustWithinTableColumn, widthStyle);
            Action.TblColumnFormattingCreate(theCD, "$", "", false, null, null, null, "", "", true, true, superUser, null);
            return theCD;
        }


        protected int CreateStat(string abbrevName, string fullName, string explanation, int TblTab, int myRatingPhase, int decimalPlaces, double min, double max, bool useAsFilter, bool sortable, bool sortAsc, TblColumnFormatting theFormatting, bool trackTrustWithinTableColumn = false)
        {
            int theRatingCharacteristic = Action.RatingCharacteristicsCreate(myRatingPhase,null,(decimal) min,(decimal) max,(short) decimalPlaces,abbrevName,true, true,superUser,null);
            Guid thePointsManagerID = Action.DataContext.GetTable<TblTab>().Single(c => c.TblTabID == TblTab).Tbl.PointsManagerID;
            int theRGA = Action.RatingGroupAttributesCreate(theRatingCharacteristic, null, abbrevName, RatingGroupTypes.singleNumber, null, fullName, true, true, superUser, null, thePointsManagerID);
            int theCD = CreateTblColumn(abbrevName, fullName, explanation, TblTab, theRGA, useAsFilter, sortable, sortAsc, trackTrustWithinTableColumn);
            if (theFormatting != null)
                Action.TblColumnFormattingCreate(theCD, theFormatting.Prefix, theFormatting.Suffix, theFormatting.OmitLeadingZero, theFormatting.ExtraDecimalPlaceAbove, theFormatting.ExtraDecimalPlace2Above, theFormatting.ExtraDecimalPlace3Above, theFormatting.SuppStylesHeader, theFormatting.SuppStylesMain, true, true, superUser, null);
            return theCD;
        }

        protected void SetColumnAsDefaultSort(Guid TblTabID, Guid TblColumnID)
        {
            Action.TblTabChangeDefaultSort(TblTabID, TblColumnID, true, superUser, null);
        }

        protected void BeginImport(string filename, Guid TblID)
        {
            //ImportExport myImportExport = new ImportExport(Action.R8RDB.GetTable<Tbl>().Single(x => x.TblID == TblID));
            //string sourceFileLoc = Server.MapPath("~/sourcedata/" + filename);
            //string errorMessage = "";
            //if (System.IO.File.Exists(sourceFileLoc))
            //{
            //    bool IsValid = myImportExport.IsXmlValid(sourceFileLoc, ref errorMessage);
            //    if (IsValid == false)
            //        throw new Exception("The XML file could not be validated. The error message is: " + errorMessage);
            //    if (IsValid == true)
            //        myImportExport.PerformImport(sourceFileLoc, superUser, false);
            //}
            //else
            //    Trace.TraceError("Source file " + sourceFileLoc + " doesn't exist.");
        }

        protected HierarchyItem CreateHierarchyItem(HierarchyItem higherItem, Guid? associatedTblID, bool includeInMenu, string name)
        {
            Tbl associatedTbl = null;
            if (associatedTblID != null)
                associatedTbl = Action.DataContext.NewOrFirst<Tbl>(x => x.TblID == associatedTblID);
            HierarchyItem theHierarchyItem = Action.HierarchyItemCreate(higherItem, associatedTbl, includeInMenu, name, superUser, null);
            if (associatedTblID != null)
            {
                Tbl rewardTable = Action.DataContext.NewOrFirst<Tbl>(x => x.PointsManager == theHierarchyItem.Tbl.PointsManager && x.Name.StartsWith("Changes"));
                if (rewardTable != null)
                    Action.HierarchyItemCreate(theHierarchyItem, rewardTable, false, "Changes", superUser, null);
            }
            return theHierarchyItem;
        }

        protected int CreateTbl(string name, string rowName, string tblRowAdditionCriteria, int defaultRGA, int theRatingPhase, int? theSubsidyDensityRange, int? theRatingCondition, Guid thePointsManagerID, bool oneRatingPerRatingGroup = true, bool allowOverrideOfCharacteristics = false, string widthStyleNameCol="wf225", string widthStyleNumCol="wf35")
        {
            int myChangesGroup = Action.ChangesGroupCreate(null, null, superUser, null, null, null, null);
            Guid theTblID = Action.TblCreate(thePointsManagerID, defaultRGA, "table column group", true, true, superUser, null, name, allowOverrideOfCharacteristics, oneRatingPerRatingGroup, rowName, tblRowAdditionCriteria, true, true, widthStyleNameCol, widthStyleNumCol);
            return theTblID;
        }

        protected int CreatePointsManager(string name, int? theSubsidyDensityRange, int? theRatingCondition, Guid theDomainID, bool createRewardTbl, bool visibleToPublic = true, decimal probabilityHighStakes = 0.01M, decimal highStakesMultiplierSecret = 100M, decimal highStakesMultiplierKnown = 10M)
        {
            int myChangesGroup = Action.ChangesGroupCreate(null, null, superUser, null, null, null, null);
            Guid pointsManagerID = Action.PointsManagerCreate(theDomainID, null, true, true, superUser, null, name);
            CreateStandardObjectsForPointsManager(pointsManagerID);

            if (createRewardTbl)
            {
                Action.ChangesTblCreate(pointsManagerID, -10M, 10M, 60 * 60 * 24 * 2, 60 * 60 * 24 * 1, 0.05M, 20M, 1000, true, superUser, null);
                if (visibleToPublic)
                    Action.UsersRightsCreate(null, pointsManagerID, true, true, false, true, true, false, false, false, false, false, false, "Partial administrative privileges", true, true, superUser, null);
                else
                    Action.UsersRightsCreate(null, pointsManagerID, false, false, false, false, false, false, false, false, false, false, false, "No privileges", true, true, superUser, null);
            }
            return pointsManagerID;
        }

        protected int CreateDomain(string name, int theRatingPhase, int? theSubsidyDensityRange, int? theRatingCondition)
        {
            int myChangesGroup = Action.ChangesGroupCreate(null, null, superUser, null, null, null, null);
            Guid theDomainID = Action.DomainCreate(true, true, false, true, true, superUser, null, name);
            Guid theTblDimensionsID = Action.DataContext.GetTable<TblDimension>().FirstOrDefault().TblDimensionsID;
            Action.DomainChangeAppearance(theDomainID, theTblDimensionsID, true, superUser, null);
            return theDomainID;
        }

        public virtual void Create()
        {
            superUser = Action.DataContext.GetTable<User>().Single(u => u.Username == "admin").UserID;
            myChangesGroup = Action.ChangesGroupCreate(null, null, superUser, null, null, null, null);

            standardDomain = Action.DataContext.GetTable<Domain>().SingleOrDefault(d => d.Name == "R8RStandard");
            if (standardDomain == null)
            { // all this will only run once
                Guid domainID = Action.DomainCreate(true, true, false, true, true, superUser, null, "R8RStandard");
                standardDomain = Action.DataContext.GetTable<Domain>().Single(d => d.Name == "R8RStandard");
                sportsHierarchy = CreateHierarchyItem(null, null, true, "Sports");
                entertainmentHierarchy = CreateHierarchyItem(null, null, true, "Entertainment");
                internetHierarchy = CreateHierarchyItem(null, null, true, "Internet");
                governmentHierarchy = CreateHierarchyItem(null, null, true, "Government");
                consumerHierarchy = CreateHierarchyItem(null, null, true, "Consumer");
                newsHierarchy = CreateHierarchyItem(null, null, true, "News");
            }
            else
            {
                standardDomain = Action.DataContext.GetTable<Domain>().Single(d => d.Name == "R8RStandard");
                sportsHierarchy = Action.DataContext.GetTable<HierarchyItem>().Single(h => h.HierarchyItemName == "Sports");
                entertainmentHierarchy = Action.DataContext.GetTable<HierarchyItem>().Single(h => h.HierarchyItemName == "Entertainment");
                internetHierarchy = Action.DataContext.GetTable<HierarchyItem>().Single(h => h.HierarchyItemName == "Internet");
                governmentHierarchy = Action.DataContext.GetTable<HierarchyItem>().Single(h => h.HierarchyItemName == "Government");
                consumerHierarchy = Action.DataContext.GetTable<HierarchyItem>().Single(h => h.HierarchyItemName == "Consumer");
                newsHierarchy = Action.DataContext.GetTable<HierarchyItem>().Single(h => h.HierarchyItemName == "News");
            }


        }

        public void CreateStandardObjectsForPointsManager(Guid pointsManagerID)
        {
            StandardObjectsForEachPointsManager so = new StandardObjectsForEachPointsManager();

            so.theRatingPhaseStandardID = CreateRatingPhaseGroupStandard();
            int eventRating = Action.RatingCharacteristicsCreate(so.theRatingPhaseStandardID, null, 0, 100, 1, "Event", true, true, superUser, null);
            so.eventRGA = Action.RatingGroupAttributesCreate(eventRating, null, "Event", RatingGroupTypes.probabilitySingleOutcome, null, "Event", false, (decimal)0.5, true, true, superUser, null, pointsManagerID);
            int ratingRating = Action.RatingCharacteristicsCreate(so.theRatingPhaseStandardID, null, 0, 10, 1, "Rating", true, true, superUser, null);
            so.ratingRGA = Action.RatingGroupAttributesCreate(ratingRating, null, "Rating", RatingGroupTypes.singleNumber, null, "Rating", true, 0, true, true, superUser, null, pointsManagerID);
            int ratingNegPosRating = Action.RatingCharacteristicsCreate(so.theRatingPhaseStandardID, null, -10, 10, 1, "Range", true, true, superUser, null);
            so.ratingNegPosRGA = Action.RatingGroupAttributesCreate(ratingNegPosRating, null, "Range", RatingGroupTypes.singleNumber, null, "Range", true, true, superUser, null, pointsManagerID);

            so.theRatingPhaseShortID = CreateRatingPhaseGroupShort();
            int eventRatingShort = Action.RatingCharacteristicsCreate(so.theRatingPhaseShortID, null, 0, 100, 1, "Event", true, true, superUser, null);
            so.eventShortRGA = Action.RatingGroupAttributesCreate(eventRatingShort, null, "Event", RatingGroupTypes.probabilitySingleOutcome, null, "Event", false, (decimal)0.5, true, true, superUser, null, pointsManagerID);
            int ratingRatingShort = Action.RatingCharacteristicsCreate(so.theRatingPhaseShortID, null, 0, 10, 1, "Rating", true, true, superUser, null);
            so.ratingShortRGA = Action.RatingGroupAttributesCreate(ratingRatingShort, null, "Rating", RatingGroupTypes.singleNumber, null, "Rating", true, 0, true, true, superUser, null, pointsManagerID);


            so.theRatingPhaseVeryShortID = CreateRatingPhaseGroupVeryShort();
            int eventRatingVeryShort = Action.RatingCharacteristicsCreate(so.theRatingPhaseVeryShortID, null, 0, 100, 1, "Event", true, true, superUser, null);
            so.eventVeryShortRGA = Action.RatingGroupAttributesCreate(eventRatingVeryShort, null, "Event", RatingGroupTypes.probabilitySingleOutcome, null, "Event", false, (decimal)0.5, true, true, superUser, null, pointsManagerID);
            int ratingRatingVeryShort = Action.RatingCharacteristicsCreate(so.theRatingPhaseVeryShortID, null, 0, 10, 1, "Rating", true, true, superUser, null);
            so.ratingVeryShortRGA = Action.RatingGroupAttributesCreate(ratingRatingVeryShort, null, "Rating", RatingGroupTypes.singleNumber, null, "Rating", true, 0, true, true, superUser, null, pointsManagerID);


            so.theRatingPhaseLongID = CreateRatingPhaseGroupLong();
            int eventRatingLong = Action.RatingCharacteristicsCreate(so.theRatingPhaseLongID, null, 0, 100, 1, "Event", true, true, superUser, null);
            so.eventLongRGA = Action.RatingGroupAttributesCreate(eventRatingLong, null, "Event", RatingGroupTypes.probabilitySingleOutcome, null, "Event", false, (decimal)0.5, true, true, superUser, null, pointsManagerID);
            int ratingRatingLong = Action.RatingCharacteristicsCreate(so.theRatingPhaseLongID, null, 0, 10, 1, "Rating", true, true, superUser, null);
            so.ratingLongRGA = Action.RatingGroupAttributesCreate(ratingRatingLong, null, "Rating", RatingGroupTypes.singleNumber, null, "Rating", true, 0, true, true, superUser, null, pointsManagerID);


            so.theRatingPhaseVeryLongID = CreateRatingPhaseGroupVeryLong();
            int eventRatingVeryLong = Action.RatingCharacteristicsCreate(so.theRatingPhaseLongID, null, 0, 100, 1, "Event", true, true, superUser, null);
            so.eventVeryLongRGA = Action.RatingGroupAttributesCreate(eventRatingLong, null, "Event", RatingGroupTypes.probabilitySingleOutcome, null, "Event", false, (decimal)0.5, true, true, superUser, null, pointsManagerID);
            int ratingRatingVeryLong = Action.RatingCharacteristicsCreate(so.theRatingPhaseLongID, null, 0, 10, 1, "Rating", true, true, superUser, null);
            so.ratingVeryLongRGA = Action.RatingGroupAttributesCreate(ratingRatingLong, null, "Rating", RatingGroupTypes.singleNumber, null, "Rating", true, 0, true, true, superUser, null, pointsManagerID);

            standardObjectsForPointsManager.Add(pointsManagerID, so);
        }
    }

    public class Baseball : CreateStandardBase
    {

        Guid leagueChoiceGroupID;
        Guid divisionChoiceGroupID;
        Guid teamChoiceGroupID;
        Guid positionChoiceGroupID;
        Guid batsAndThrowsChoiceGroupID;

        public Baseball() : base()
        {
        }

        protected void CreateHittingStats(int basicStatsGroup, int moreStatsGroup, int myRatingPhase)
        {
            CreateStat("AB","At bats","",basicStatsGroup,myRatingPhase,0,0,800,true,true,false, null);
            CreateStat("H", "Hits", "", basicStatsGroup, myRatingPhase, 0, 0, 325, true, true, false, null);
            CreateStat("R", "Runs", "", basicStatsGroup, myRatingPhase, 0, 0, 220, true, true, false, null);
            TblColumnFormatting formatWithoutLeadingZero = NumberandTableFormatter.GetBlankCDFormatting();
            formatWithoutLeadingZero.OmitLeadingZero = true;
            CreateStat("AVG", "Average", "", basicStatsGroup, myRatingPhase, 3, 0, 1, true, true, false, formatWithoutLeadingZero);
            CreateStat("HR", "Home runs", "", basicStatsGroup, myRatingPhase, 0, 0, 90, true, true, false, null);
            CreateStat("RBI", "Runs batted in", "", basicStatsGroup, myRatingPhase, 0, 0, 225, true, true, false, null);
            CreateStat("2B", "Doubles", "", moreStatsGroup, myRatingPhase, 0, 0, 80, false, true, false, null);
            CreateStat("3B", "Triples", "", moreStatsGroup, myRatingPhase, 0, 0, 40, false, true, false, null);
            CreateStat("BB", "Bases on balls", "", moreStatsGroup, myRatingPhase, 0, 0, 300, false, true, false, null);
            CreateStat("SO", "Strikeouts", "", moreStatsGroup, myRatingPhase, 0, 0, 250, false, true, false, null);
            CreateStat("OBP", "On base percentage", "", moreStatsGroup, myRatingPhase, 3, 0, 1, true, true, false, formatWithoutLeadingZero);
            CreateStat("SLG", "Slugging percentage", "", moreStatsGroup, myRatingPhase, 3, 0, 4, true, true, false, formatWithoutLeadingZero);
        }

        protected void CreatePitchingStats(int basicStatsGroup, int moreStatsGroup, int myRatingPhase)
        {
            CreateStat("W", "Wins", "", basicStatsGroup, myRatingPhase, 0, 0, 35, true, true, false, null);
            CreateStat("L", "Losses", "", basicStatsGroup, myRatingPhase, 0, 0, 35, true, true, false, null);
            CreateStat("ERA", "Earned run average", "", basicStatsGroup, myRatingPhase, 2, 0, 12, true, true, true, null);
            CreateStat("G", "Games", "", basicStatsGroup, myRatingPhase, 0, 0, 100, true, true, false, null);
            CreateStat("SV", "Saves", "", basicStatsGroup, myRatingPhase, 0, 0, 85, true, true, false, null);
            CreateStat("IP", "Innings pitched", "", basicStatsGroup, myRatingPhase, 0, 0, 300, true, true, false, null);
            CreateStat("H", "Hits", "", moreStatsGroup, myRatingPhase, 0, 0, 300, false, true, false, null);
            CreateStat("R", "Runs", "", moreStatsGroup, myRatingPhase, 0, 0, 160, false, true, false, null);
            CreateStat("HR", "Home runs", "", moreStatsGroup, myRatingPhase, 0, 0, 50, false, true, false, null);
            CreateStat("BB", "Bases on balls", "", moreStatsGroup, myRatingPhase, 0, 0, 140, false, true, false, null);
            CreateStat("SO", "Strikeouts", "", moreStatsGroup, myRatingPhase, 0, 0, 300, true, true, false, null);
        }

        protected void CreateStandingsStats(int basicStatsGroup, int myRatingPhase)
        {
            CreateStat("W", "Wins", "The number of team wins for the year", basicStatsGroup, myRatingPhase, 0, 0, 130, false, true, false, null);
            CreateStat("L", "Losses", "The number of team losses for the year", basicStatsGroup, myRatingPhase, 0, 0, 130, false, true, false, null);
            CreateStat("GA/GB", "Games ahead or behind", "The number of games by which the team wins the division (positive number) or falls short (negative number)", basicStatsGroup, myRatingPhase, 1, -50, 30, false, true, false, null);
            CreateEvent("Playoff", "Makes playoffs", "The percentage probability that the team makes the playoffs.", basicStatsGroup, myRatingPhase, false, true, false);
            CreateEvent("Pennant", "Pennant winners", "The percentage probability that the team reaches the World Series.", basicStatsGroup, myRatingPhase, false, true, false);
            int wsChamps = CreateEvent("WS Champs", "World Series Champs", "The percentage probability that the team wins the World Series.", basicStatsGroup, myRatingPhase, false, true, false);
            SetColumnAsDefaultSort(basicStatsGroup, wsChamps);
        }

        protected void CreateBaseballChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();

            ChoiceGroupData theLeagueChoiceGroup = new ChoiceGroupData();
            theLeagueChoiceGroup.AddChoiceToGroup("American League");
            theLeagueChoiceGroup.AddChoiceToGroup("National League");
            leagueChoiceGroupID = CreateChoiceGroup(pointsManagerID, theLeagueChoiceGroup, choiceGroupStandardSettings, null, "League");
            int choiceAL = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "American League" && x.ChoiceGroupID == leagueChoiceGroupID).ChoiceInGroupID;
            int choiceNL = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "National League" && x.ChoiceGroupID == leagueChoiceGroupID).ChoiceInGroupID;

            ChoiceGroupData theDivisionChoiceGroup = new ChoiceGroupData();
            theDivisionChoiceGroup.AddChoiceToGroup("AL East", choiceAL);
            theDivisionChoiceGroup.AddChoiceToGroup("AL Central", choiceAL);
            theDivisionChoiceGroup.AddChoiceToGroup("AL West", choiceAL);
            theDivisionChoiceGroup.AddChoiceToGroup("NL East", choiceNL);
            theDivisionChoiceGroup.AddChoiceToGroup("NL Central", choiceNL);
            theDivisionChoiceGroup.AddChoiceToGroup("NL West", choiceNL);
            int divisionChoiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, false, false, false, false, true, false, false);
            divisionChoiceGroupID = CreateChoiceGroup(pointsManagerID, theDivisionChoiceGroup, divisionChoiceGroupSettings, leagueChoiceGroupID, "Division");
            int choiceALEast = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "AL East" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceALCentral = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "AL Central" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceALWest = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "AL West" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceNLEast = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "NL East" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceNLCentral = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "NL Central" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceNLWest = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "NL West" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            
            ChoiceGroupData theTeamChoiceGroup = new ChoiceGroupData();
            theTeamChoiceGroup.AddChoiceToGroup("Baltimore Orioles", choiceALEast);
            theTeamChoiceGroup.AddChoiceToGroup("Boston Red Sox", choiceALEast);
            theTeamChoiceGroup.AddChoiceToGroup("New York Yankees", choiceALEast);
            theTeamChoiceGroup.AddChoiceToGroup("Tampa Bay Rays", choiceALEast);
            theTeamChoiceGroup.AddChoiceToGroup("Toronto Blue Jays", choiceALEast);
            theTeamChoiceGroup.AddChoiceToGroup("Chicago White Sox", choiceALCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Cleveland Indians", choiceALCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Detroit Tigers", choiceALCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Kansas City Royals", choiceALCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Minnesota Twins", choiceALCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Los Angeles Angels", choiceALWest);
            theTeamChoiceGroup.AddChoiceToGroup("Oakland Athletics", choiceALWest);
            theTeamChoiceGroup.AddChoiceToGroup("Seattle Mariners", choiceALWest);
            theTeamChoiceGroup.AddChoiceToGroup("Texas Rangers", choiceALWest);
            theTeamChoiceGroup.AddChoiceToGroup("Atlanta Braves", choiceNLEast);
            theTeamChoiceGroup.AddChoiceToGroup("Florida Marlins", choiceNLEast);
            theTeamChoiceGroup.AddChoiceToGroup("New York Mets", choiceNLEast);
            theTeamChoiceGroup.AddChoiceToGroup("Philadelphia Phillies", choiceNLEast);
            theTeamChoiceGroup.AddChoiceToGroup("Washington Nationals", choiceNLEast);
            theTeamChoiceGroup.AddChoiceToGroup("Chicago Cubs", choiceNLCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Cincinnati Reds", choiceNLCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Houston Astros", choiceNLCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Milwaukee Brewers", choiceNLCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Pittsburgh Pirates", choiceNLCentral);
            theTeamChoiceGroup.AddChoiceToGroup("St. Louis Cardinals", choiceNLCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Arizona Diamondbacks", choiceNLWest);
            theTeamChoiceGroup.AddChoiceToGroup("Colorado Rockies", choiceNLWest);
            theTeamChoiceGroup.AddChoiceToGroup("Los Angeles Dodgers", choiceNLWest);
            theTeamChoiceGroup.AddChoiceToGroup("San Diego Padres", choiceNLWest);
            theTeamChoiceGroup.AddChoiceToGroup("San Francisco Giants", choiceNLWest);
            int teamChoiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, true, false, false, false, true, true, false);
            teamChoiceGroupID = CreateChoiceGroup(pointsManagerID, theTeamChoiceGroup, teamChoiceGroupSettings, divisionChoiceGroupID, "Team");

            ChoiceGroupData thePositionChoiceGroup = new ChoiceGroupData();
            thePositionChoiceGroup.AddChoiceToGroup("P");
            thePositionChoiceGroup.AddChoiceToGroup("C");
            thePositionChoiceGroup.AddChoiceToGroup("1B");
            thePositionChoiceGroup.AddChoiceToGroup("2B");
            thePositionChoiceGroup.AddChoiceToGroup("3B");
            thePositionChoiceGroup.AddChoiceToGroup("SS");
            thePositionChoiceGroup.AddChoiceToGroup("OF");
            thePositionChoiceGroup.AddChoiceToGroup("DH");
            int choicePosition = ChoiceGroupSettingsMask.GetChoiceGroupSetting(true, false, false, false, false, false, false, false);
            positionChoiceGroupID = CreateChoiceGroup(pointsManagerID, thePositionChoiceGroup, choicePosition, null, "Position");

            ChoiceGroupData theBatsAndThrowsChoiceGroup = new ChoiceGroupData();
            theBatsAndThrowsChoiceGroup.AddChoiceToGroup("R");
            theBatsAndThrowsChoiceGroup.AddChoiceToGroup("L");
            theBatsAndThrowsChoiceGroup.AddChoiceToGroup("S");
            batsAndThrowsChoiceGroupID = CreateChoiceGroup(pointsManagerID, theBatsAndThrowsChoiceGroup, choiceGroupStandardSettings, null, "Bats/Throws");
        }

        protected void CreateHittingOrPitchingFields(Guid TblID, bool isPitching)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            //int lastNameFD = CreateFieldDefinition(TblID, "Last Name", FieldTypes.TextField, false);
            //CreateFieldDefinitionDisplaySettings(lastNameFD, doNotShowThisField);
            //int firstNameFD = CreateFieldDefinition(TblID, "First Name", FieldTypes.TextField, false);
            //CreateFieldDefinitionDisplaySettings(firstNameFD, doNotShowThisField);
            int leagueCGFD = 0;
            int leagueFD = CreateFieldDefinition(TblID, "League", FieldTypes.ChoiceField, true, leagueChoiceGroupID, null, ref leagueCGFD);
            CreateFieldDefinitionDisplaySettings(leagueFD, doNotShowThisField);
            int divisionCGFD = 0;
            int divisionFD = CreateFieldDefinition(TblID, "Division", FieldTypes.ChoiceField, true, divisionChoiceGroupID, leagueCGFD, ref divisionCGFD);
            CreateFieldDefinitionDisplaySettings(divisionFD, doNotShowThisField);
            int teamFD = CreateFieldDefinition(TblID, "Team", FieldTypes.ChoiceField, true, teamChoiceGroupID, divisionCGFD);
            CreateFieldDefinitionDisplaySettings(teamFD, doNotShowThisField, visibleWithNoNameFieldDisplay, visibleWithNoNameFieldDisplay);
            if (!isPitching)
                CreateFieldDefinition(TblID, "Position", FieldTypes.ChoiceField, true, positionChoiceGroupID, null);
            int uniformFD = CreateFieldDefinition(TblID, "Uniform Number", FieldTypes.TextField, false, true, false, false);
            if (!isPitching)
                CreateFieldDefinition(TblID, "Bats", FieldTypes.ChoiceField, true, batsAndThrowsChoiceGroupID, null);
            int throwsFD = CreateFieldDefinition(TblID, "Throws", FieldTypes.ChoiceField, false, batsAndThrowsChoiceGroupID, null);
            CreateFieldDefinitionDisplaySettings(throwsFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay);
            int heightFD = CreateFieldDefinition(TblID, "Height", FieldTypes.TextField, false, true, false, false);
            int weightFD = CreateFieldDefinition(TblID, "Weight", FieldTypes.NumberField, false, 0, null, 0);
            CreateFieldDefinitionDisplaySettings(weightFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay); 
            int birthDateFD = CreateFieldDefinition(TblID, "Date of Birth", FieldTypes.DateTimeField, false, true, false);
            int birthPlaceFD = CreateFieldDefinition(TblID, "Birthplace", FieldTypes.TextField, false, true, false, false);
            CreateFieldDefinitionDisplaySettings(birthPlaceFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay);
            int debutDateFD = CreateFieldDefinition(TblID, "MLB Debut", FieldTypes.DateTimeField, false, true, false);
            int playerPageFD = CreateFieldDefinition(TblID, "Player Page", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(playerPageFD, doNotShowThisField, doNotShowThisField, visibleWithNoNameFieldDisplay);
        }

        protected void CreateHittingFields(Guid TblID)
        {
            CreateHittingOrPitchingFields(TblID, false);
        }

        protected void CreatePitchingFields(Guid TblID)
        {
            CreateHittingOrPitchingFields(TblID, true);
        }

        protected void CreateStandingsFields(Guid TblID)
        {
            int leagueCGFD = 0;
            int leagueFD = CreateFieldDefinition(TblID, "League", FieldTypes.ChoiceField, true, leagueChoiceGroupID, null, ref leagueCGFD);
            CreateFieldDefinitionDisplaySettings(leagueFD, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
            int divisionCGFD = 0;
            int divisionFD = CreateFieldDefinition(TblID, "Division", FieldTypes.ChoiceField, true, divisionChoiceGroupID, leagueCGFD, ref divisionCGFD);
            CreateFieldDefinitionDisplaySettings(leagueFD, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
        }

        public override void Create()
        {
            base.Create();

            HierarchyItem baseballItem = CreateHierarchyItem(sportsHierarchy, null, true, "Baseball");
            Guid thePointsManagerID;

            thePointsManagerID = CreatePointsManager("BaseballHitters", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].eventRGA;
            CreateBaseballChoiceGroups(thePointsManagerID);
            Guid hittersTblID = CreateTbl("Hitters", "Hitter", "Any non-pitcher who at any time this year has been on a 25- or 40-man roster is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem hitters = CreateHierarchyItem(baseballItem, hittersTblID, true, "Hitters");
            CreateHittingFields(hittersTblID);
            int hittersBasicStatsGroup = CreateTblTab("2011 Statistics", hittersTblID);
            int hittersMoreStatsGroup = CreateTblTab("More 2011 Statistics", hittersTblID);
            CreateHittingStats(hittersBasicStatsGroup, hittersMoreStatsGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("hitters.xml", hittersTblID);

            thePointsManagerID = CreatePointsManager("BaseballPitchers", null, null, standardDomain.DomainID, true);
            CreateBaseballChoiceGroups(thePointsManagerID);
            Guid pitchersTblID = CreateTbl("Pitchers", "Pitcher", "Any pitcher who at any time this year has been on a 25- or 40-man roster is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem pitchers = CreateHierarchyItem(baseballItem, pitchersTblID, true, "Pitchers");
            CreatePitchingFields(pitchersTblID);
            int pitchersBasicStatsGroup = CreateTblTab("2011 Statistics", pitchersTblID);
            int pitchersMoreStatsGroup = CreateTblTab("More 2011 Statistics", pitchersTblID);
            CreatePitchingStats(pitchersBasicStatsGroup, pitchersMoreStatsGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("pitchers.xml", pitchersTblID);

            thePointsManagerID = CreatePointsManager("BaseballStandings", null, null, standardDomain.DomainID, true);
            CreateBaseballChoiceGroups(thePointsManagerID); 
            Guid standingsTblID = CreateTbl("Standings", "Team", "Only new major league ballclubs playing in the current year are eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem baseballStandings = CreateHierarchyItem(baseballItem, standingsTblID, true, "Standings");
            CreateStandingsFields(standingsTblID);
            int standingsGroup = CreateTblTab("Basic", standingsTblID);
            CreateStandingsStats(standingsGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("teams.xml", standingsTblID);
        }
        
    }

    public class Hockey : CreateStandardBase
    {
        Guid conferenceChoiceGroupID;
        Guid divisionChoiceGroupID;
        Guid teamChoiceGroupID;
        Guid positionChoiceGroupID;
        Guid catchesAndShootsChoiceGroupID;

        public Hockey()
            : base()
        {
        }

        protected void CreatePlayerStats(int basicStatsGroup, int moreStatsGroup, int myRatingPhase)
        {
            CreateStat("GP", "Games Played", "", basicStatsGroup, myRatingPhase, 0, 0, 65, true, true, false, null);
            CreateStat("G", "Goals", "", basicStatsGroup, myRatingPhase, 0, 0, 100, true, true, false, null);
            CreateStat("A", "Assists", "", basicStatsGroup, myRatingPhase, 0, 0, 220, true, true, false, null);
            CreateStat("P", "Points", "", basicStatsGroup, myRatingPhase, 0, 0, 250, true, true, false, null);
            CreateStat("+/-", "Plus/Minus", "", basicStatsGroup, myRatingPhase, 0, -200, 200, true, true, false, null);
            CreateStat("PIM", "Penalty Minutes", "", moreStatsGroup, myRatingPhase, 0, 0, 500, true, true, false, null);
            CreateStat("S", "Shots on Goal", "", moreStatsGroup, myRatingPhase, 0, 0, 600, true, true, false, null);
            CreateStat("S%", "Shooting Percentage", "", moreStatsGroup, myRatingPhase, 0, 0, 100, true, true, false, null);
            CreateStat("ATOI", "Average Time on Ice", "", moreStatsGroup, myRatingPhase, 0, 0, 60, true, true, false, null);
        }

        protected void CreateGoalieStats(int basicStatsGroup, int myRatingPhase)
        {
            CreateStat("W", "Wins", "", basicStatsGroup, myRatingPhase, 0, 0, 50, true, true, false, null);
            CreateStat("L", "Losses", "", basicStatsGroup, myRatingPhase, 0, 0, 50, true, true, false, null);
            // CreateStat("GA", "Goals Against", "", basicStatsGroup, myRatingPhase, 0, 0, 250, true, true, true, null);
            CreateStat("GAA", "Goals Against Average", "", basicStatsGroup, myRatingPhase, 2, 0, 7, true, true, false, null);
            // CreateStat("SA", "Shots Against", "", basicStatsGroup, myRatingPhase, 0, 0, 2500, true, true, false, null);
            CreateStat("S", "Saves", "", basicStatsGroup, myRatingPhase, 0, 0, 2200, true, true, false, null);

            TblColumnFormatting formatWithoutLeadingZero = NumberandTableFormatter.GetBlankCDFormatting();
            formatWithoutLeadingZero.OmitLeadingZero = true;
            CreateStat("SV%", "Save Percentage", "", basicStatsGroup, myRatingPhase, 3, 0, 1, false, true, false, formatWithoutLeadingZero);
        }

        protected void CreateStandingsStats(int basicStatsGroup, int myRatingPhase)
        {
            CreateStat("W", "Wins", "The number of team wins for the year", basicStatsGroup, myRatingPhase, 0, 0, 130, false, true, false, null);
            CreateStat("L", "Losses", "The number of team losses for the year", basicStatsGroup, myRatingPhase, 0, 0, 130, false, true, false, null); 
            CreateStat("OT", "Overtime Losses", "Overtime/Shootout Losses", basicStatsGroup, myRatingPhase, 0, 0, 130, false, true, false, null); 
            CreateStat("PTS", "Points", "The number of team points for the year", basicStatsGroup, myRatingPhase, 0, 0, 130, false, true, false, null);

            CreateEvent("Playoff", "Makes playoffs", "The percentage probability that the team makes the playoffs.", basicStatsGroup, myRatingPhase, false, true, false);
            int scChamps = CreateEvent("Champs", "Stanley Cup Champions", "The percentage probability that the team wins the Stanley Cup.", basicStatsGroup, myRatingPhase, false, true, false);
            SetColumnAsDefaultSort(basicStatsGroup, scChamps);
        }

        protected void CreateHockeyChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();

            ChoiceGroupData theConferenceChoiceGroup = new ChoiceGroupData();
            theConferenceChoiceGroup.AddChoiceToGroup("Eastern Conference");
            theConferenceChoiceGroup.AddChoiceToGroup("Western Conference");
            conferenceChoiceGroupID = CreateChoiceGroup(pointsManagerID, theConferenceChoiceGroup, choiceGroupStandardSettings, null, "Conference");
            int choiceEC = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Eastern Conference" && x.ChoiceGroupID == conferenceChoiceGroupID).ChoiceInGroupID;
            int choiceWC = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Western Conference" && x.ChoiceGroupID == conferenceChoiceGroupID).ChoiceInGroupID;

            ChoiceGroupData theDivisionChoiceGroup = new ChoiceGroupData();
            theDivisionChoiceGroup.AddChoiceToGroup("Atlantic", choiceEC);
            theDivisionChoiceGroup.AddChoiceToGroup("Northeast", choiceEC);
            theDivisionChoiceGroup.AddChoiceToGroup("Southeast", choiceEC);
            theDivisionChoiceGroup.AddChoiceToGroup("Central", choiceWC);
            theDivisionChoiceGroup.AddChoiceToGroup("Northwest", choiceWC);
            theDivisionChoiceGroup.AddChoiceToGroup("Pacific", choiceWC);
            int divisionChoiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, false, false, false, false, true, false, false);
            divisionChoiceGroupID = CreateChoiceGroup(pointsManagerID, theDivisionChoiceGroup, divisionChoiceGroupSettings, conferenceChoiceGroupID, "Division");
            int choiceAtlantic = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Atlantic" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceNortheast = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Northeast" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceSoutheast = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Southeast" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceCentral = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Central" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choiceNorthwest = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Northwest" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;
            int choicePacific = Action.DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceText == "Pacific" && x.ChoiceGroupID == divisionChoiceGroupID).ChoiceInGroupID;

            ChoiceGroupData theTeamChoiceGroup = new ChoiceGroupData();
            theTeamChoiceGroup.AddChoiceToGroup("New Jersey Devils", choiceAtlantic);
            theTeamChoiceGroup.AddChoiceToGroup("New York Islanders", choiceAtlantic);
            theTeamChoiceGroup.AddChoiceToGroup("New York Rangers", choiceAtlantic);
            theTeamChoiceGroup.AddChoiceToGroup("Philadelphia Flyers", choiceAtlantic);
            theTeamChoiceGroup.AddChoiceToGroup("Pittsburgh Penguins", choiceAtlantic);
            theTeamChoiceGroup.AddChoiceToGroup("Boston Bruins", choiceNortheast);
            theTeamChoiceGroup.AddChoiceToGroup("Buffalo Sabres", choiceNortheast);
            theTeamChoiceGroup.AddChoiceToGroup("Montreal Canadiens", choiceNortheast);
            theTeamChoiceGroup.AddChoiceToGroup("Ottawa Senators", choiceNortheast);
            theTeamChoiceGroup.AddChoiceToGroup("Toronto Maple Leafs", choiceNortheast);
            theTeamChoiceGroup.AddChoiceToGroup("Atlanta Thrashers", choiceSoutheast);
            theTeamChoiceGroup.AddChoiceToGroup("Carolina Hurricanes", choiceSoutheast);
            theTeamChoiceGroup.AddChoiceToGroup("Florida Panthers", choiceSoutheast);
            theTeamChoiceGroup.AddChoiceToGroup("Tampa Bay Lightning", choiceSoutheast);
            theTeamChoiceGroup.AddChoiceToGroup("Washington Capitals", choiceSoutheast);
            theTeamChoiceGroup.AddChoiceToGroup("Chicago Blackhawks", choiceCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Columbus Blue Jackets", choiceCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Detroit Red Wings", choiceCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Nashville Predators", choiceCentral);
            theTeamChoiceGroup.AddChoiceToGroup("St. Louis Blues", choiceCentral);
            theTeamChoiceGroup.AddChoiceToGroup("Calgary Flames", choiceNorthwest);
            theTeamChoiceGroup.AddChoiceToGroup("Colorado Avalanche", choiceNorthwest);
            theTeamChoiceGroup.AddChoiceToGroup("Edmonton Oilers", choiceNorthwest);
            theTeamChoiceGroup.AddChoiceToGroup("Minnesota Wild", choiceNorthwest);
            theTeamChoiceGroup.AddChoiceToGroup("Vancouver Canucks", choiceNorthwest);
            theTeamChoiceGroup.AddChoiceToGroup("Anaheim Ducks", choicePacific);
            theTeamChoiceGroup.AddChoiceToGroup("Dallas stars", choicePacific);
            theTeamChoiceGroup.AddChoiceToGroup("Los Angeles Kings", choicePacific);
            theTeamChoiceGroup.AddChoiceToGroup("Phoenix Coyotes", choicePacific);
            theTeamChoiceGroup.AddChoiceToGroup("San Jose Sharks", choicePacific);
            int teamChoiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, true, false, false, false, true, true, false);
            teamChoiceGroupID = CreateChoiceGroup(pointsManagerID, theTeamChoiceGroup, teamChoiceGroupSettings, divisionChoiceGroupID, "Team");

            ChoiceGroupData thePositionChoiceGroup = new ChoiceGroupData();
            thePositionChoiceGroup.AddChoiceToGroup("G");
            thePositionChoiceGroup.AddChoiceToGroup("C");
            thePositionChoiceGroup.AddChoiceToGroup("L");
            thePositionChoiceGroup.AddChoiceToGroup("R");
            thePositionChoiceGroup.AddChoiceToGroup("D");
            int choicePosition = ChoiceGroupSettingsMask.GetChoiceGroupSetting(true, false, false, false, false, false, false, false);
            positionChoiceGroupID = CreateChoiceGroup(pointsManagerID, thePositionChoiceGroup, choiceGroupStandardSettings, null, "Position");

            ChoiceGroupData theCatchesAndShootsChoiceGroup = new ChoiceGroupData();
            theCatchesAndShootsChoiceGroup.AddChoiceToGroup("R");
            theCatchesAndShootsChoiceGroup.AddChoiceToGroup("L");
            catchesAndShootsChoiceGroupID = CreateChoiceGroup(pointsManagerID, theCatchesAndShootsChoiceGroup, choiceGroupStandardSettings, null, "Shoots/Catches");
        }

        protected void CreatePlayerOrGoalieFields(Guid TblID, bool isGoalie)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            //int lastNameFD = CreateFieldDefinition(TblID, "Last Name", FieldTypes.TextField, false);
            //CreateFieldDefinitionDisplaySettings(lastNameFD, doNotShowThisField);
            //int firstNameFD = CreateFieldDefinition(TblID, "First Name", FieldTypes.TextField, false);
            //CreateFieldDefinitionDisplaySettings(firstNameFD, doNotShowThisField);
            int conferenceCGFD = 0;
            int conferenceFD = CreateFieldDefinition(TblID, "Conference", FieldTypes.ChoiceField, true, conferenceChoiceGroupID, null, ref conferenceCGFD);
            CreateFieldDefinitionDisplaySettings(conferenceFD, doNotShowThisField);
            int divisionCGFD = 0;
            int divisionFD = CreateFieldDefinition(TblID, "Division", FieldTypes.ChoiceField, true, divisionChoiceGroupID, conferenceCGFD, ref divisionCGFD);
            CreateFieldDefinitionDisplaySettings(divisionFD, doNotShowThisField);
            int teamFD = CreateFieldDefinition(TblID, "Team", FieldTypes.ChoiceField, true, teamChoiceGroupID, divisionCGFD);
            CreateFieldDefinitionDisplaySettings(teamFD, doNotShowThisField, visibleWithNoNameFieldDisplay, visibleWithNoNameFieldDisplay);
            if (!isGoalie)
                CreateFieldDefinition(TblID, "Position", FieldTypes.ChoiceField, true, positionChoiceGroupID, null);
            int uniformFD = CreateFieldDefinition(TblID, "Uniform Number", FieldTypes.TextField, false, true, false, false);
            int catchesFD;
            int shootsFD;
            if (isGoalie)
            {
                catchesFD = CreateFieldDefinition(TblID, "Catches", FieldTypes.ChoiceField, false, catchesAndShootsChoiceGroupID, null);
                CreateFieldDefinitionDisplaySettings(catchesFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay);
            }
            else
            {
                shootsFD = CreateFieldDefinition(TblID, "Shoots", FieldTypes.ChoiceField, false, catchesAndShootsChoiceGroupID, null);
                CreateFieldDefinitionDisplaySettings(shootsFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay);
            }
            int heightFD = CreateFieldDefinition(TblID, "Height", FieldTypes.TextField, false, true, false, false);
            int weightFD = CreateFieldDefinition(TblID, "Weight", FieldTypes.NumberField, false, 0, null, 0);
            CreateFieldDefinitionDisplaySettings(weightFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay);
            int birthDateFD = CreateFieldDefinition(TblID, "Date of Birth", FieldTypes.DateTimeField, false, true, false);
            int birthPlaceFD = CreateFieldDefinition(TblID, "Birthplace", FieldTypes.TextField, false, true, false, false);
            CreateFieldDefinitionDisplaySettings(birthPlaceFD, doNotShowThisField, continuationWithNameFieldDisplay, continuationWithNameFieldDisplay);
            int playerPageFD = CreateFieldDefinition(TblID, "Player Page", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(playerPageFD, doNotShowThisField, doNotShowThisField, visibleWithNoNameFieldDisplay);
        }

        protected void CreatePlayerFields(Guid TblID)
        {
            CreatePlayerOrGoalieFields(TblID, false);
        }

        protected void CreateGoalieFields(Guid TblID)
        {
            CreatePlayerOrGoalieFields(TblID, true);
        }

        protected void CreateStandingsFields(Guid TblID)
        {
            int conferenceCGFD = 0;
            int conferenceFD = CreateFieldDefinition(TblID, "Conference", FieldTypes.ChoiceField, true, conferenceChoiceGroupID, null, ref conferenceCGFD);
            CreateFieldDefinitionDisplaySettings(conferenceFD, doNotShowThisField);
            int divisionCGFD = 0;
            int divisionFD = CreateFieldDefinition(TblID, "Division", FieldTypes.ChoiceField, true, divisionChoiceGroupID, conferenceCGFD, ref divisionCGFD);
            CreateFieldDefinitionDisplaySettings(divisionFD, doNotShowThisField);
        }

        public override void Create()
        {
            base.Create();
            Guid thePointsManagerID;


            HierarchyItem hockeyItem = CreateHierarchyItem(sportsHierarchy, null, true, "Hockey");

            thePointsManagerID = CreatePointsManager("HockeyPlayers", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].eventRGA;
            CreateHockeyChoiceGroups(thePointsManagerID);
            Guid playersTblID = CreateTbl("Players", "Player", "Any non-goalie who has been on an NHL roster is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem theHierarchyItem = CreateHierarchyItem(hockeyItem, playersTblID, true, "Players");
            CreatePlayerFields(playersTblID);
            int playersBasicStatsGroup = CreateTblTab("2009-10 Stats", playersTblID);
            int playersMoreStatsGroup = CreateTblTab("More 2009-10 Stats", playersTblID);
            CreatePlayerStats(playersBasicStatsGroup, playersMoreStatsGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("hockeyplayers.xml", playersTblID);

            thePointsManagerID = CreatePointsManager("HockeyGoalies", null, null, standardDomain.DomainID, true);
            CreateHockeyChoiceGroups(thePointsManagerID);
            Guid goaliesTblID = CreateTbl("Goalies", "Goalie", "Any goalie who at any time this year has been on an NHL roster is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            theHierarchyItem = CreateHierarchyItem(hockeyItem, goaliesTblID, true, "Goalies");
            CreateGoalieFields(goaliesTblID);
            int goaliesBasicStatsGroup = CreateTblTab("2011-12 Statistics", goaliesTblID);
            // int goaliesMoreStatsGroup = CreateTblTab("More 2011-12 Statistics", goaliesTblID);
            CreateGoalieStats(goaliesBasicStatsGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("hockeygoalies.xml", goaliesTblID);

            thePointsManagerID = CreatePointsManager("HockeyStandings", null, null, standardDomain.DomainID, true);
            CreateHockeyChoiceGroups(thePointsManagerID);
            Guid standingsTblID = CreateTbl("Standings", "Team", "Only NHL teams playing in the current year are eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            theHierarchyItem = CreateHierarchyItem(hockeyItem, standingsTblID, true, "Standings");
            CreateStandingsFields(standingsTblID);
            int standingsGroup = CreateTblTab("Basic", standingsTblID);
            CreateStandingsStats(standingsGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("hockeyteams.xml", standingsTblID);
        }

    }

    public class RealEstate : CreateStandardBase
    {
        Guid listingTypeChoiceGroupID;

        public RealEstate() : base()
        {
        }

        protected void CreateMain(int ratingGroup, int? saleGroup, int myRatingPhase)
        {
            int overall = CreateRating("Overall", "Overall", "The overall quality of the property, all factors considered", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
            CreateRating("Curb Appeal", "Curb Appeal", "The appearance of the property from outside", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Interior", "Interior Quality Overall", "The quality of the interior of the property", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Kitchen", "Kitchen Quality", "The overall quality of the kitchen", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Bedroom", "Master Bedroom Quality", "The overall quality of the master bedroom", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Bathrooms", "Bathrooms Quality", "The overall quality of the bathrooms", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Convenience", "Convenience of Location", "The convenience of the neighborhood, in terms of accessibility to shopping, workplaces, attractions, etc.", ratingGroup, myRatingPhase, true, true, false);
            if (saleGroup != null)
            {
                CreateDollarForecastOrRating("Price if Sold", "Price if sold", "The price at which the property will sell if it is sold before being withdrawn from the rating", (Guid)saleGroup, myRatingPhase, 0, 0, 100000000, "wv20", true, true, false, true);
                CreateEvent("30 days", "Sale Within 30 Days", "The probability that the property will sell within 30 days of its original listing date", (Guid)saleGroup, myRatingPhase, false, true, false);
                CreateEvent("90 days", "Sale Within 90 Days", "The probability that the property will sell within 90 days of its original listing date", (Guid)saleGroup, myRatingPhase, false, true, false);
                CreateEvent("1 year", "Sale Within 1 Year", "The probability that the property will sell within one year of its original listing date", (Guid)saleGroup, myRatingPhase, false, true, false);
                CreateEvent("Ever", "Sale Before Withdrawal", "The probability that the property will sell before being withdrawn from the rating", (Guid)saleGroup, myRatingPhase, false, true, false);
            }
        }

        protected void CreateRealEstateChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();

            ChoiceGroupData theListingTypeChoiceGroup = new ChoiceGroupData();
            theListingTypeChoiceGroup.AddChoiceToGroup("For Sale");
            theListingTypeChoiceGroup.AddChoiceToGroup("For Rent");
            theListingTypeChoiceGroup.AddChoiceToGroup("Foreclosure");
            listingTypeChoiceGroupID = CreateChoiceGroup(pointsManagerID, theListingTypeChoiceGroup, choiceGroupStandardSettings, null, "Listing Type");
        }

        protected void CreateRealEstateFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int addressFD = CreateFieldDefinition(TblID, "Address", FieldTypes.AddressField, true);
            int listingCGFD = 0;
            int listingTypeFD = CreateFieldDefinition(TblID, "Listing Type", FieldTypes.ChoiceField, true, listingTypeChoiceGroupID, null, ref listingCGFD);
            int listPriceFD = CreateFieldDefinition(TblID, "List Price", FieldTypes.NumberField, true, 0, 1000000000, 0);
            int bedroomsFD = CreateFieldDefinition(TblID, "# Bedrooms", FieldTypes.NumberField, true, 0, 50, 0);
            int bathroomsFD = CreateFieldDefinition(TblID, "# Bathrooms", FieldTypes.NumberField, true, 0, 50, 1);
            int dateListedFD = CreateFieldDefinition(TblID, "Date Listed", FieldTypes.DateTimeField, true, true, false);
            int yearBuiltFD = CreateFieldDefinition(TblID, "Year Built", FieldTypes.TextField, true, true, false, false);
            int agentFD = CreateFieldDefinition(TblID, "Agent", FieldTypes.TextField, true, true, false, false);
            int brokerFD = CreateFieldDefinition(TblID, "Broker", FieldTypes.TextField, true, true, false, false);
            int schoolsFD = CreateFieldDefinition(TblID, "Schools", FieldTypes.TextField, false, true, false, false);
            int featuresFD = CreateFieldDefinition(TblID, "Features", FieldTypes.TextField, false, true, false, false);
            int linkFD = CreateFieldDefinition(TblID, "Link", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(linkFD, doNotShowThisField, doNotShowThisField, visibleWithNameFieldDisplay);
            int mlsListingFD = CreateFieldDefinition(TblID, "MLS #", FieldTypes.TextField, false, true, false, false);
            int idFD = CreateFieldDefinition(TblID, "ID #", FieldTypes.TextField, false, true, false, false);
            CreateFieldDefinitionDisplaySettings(idFD, doNotShowThisField);
        }

        public override void Create()
        {
            base.Create();

            HierarchyItem realEstateHierarchy = CreateHierarchyItem(consumerHierarchy, null, true, "Real Estate");

            Guid thePointsManagerID = CreatePointsManager("Real Estate For Sale", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].ratingRGA;
            CreateRealEstateChoiceGroups(thePointsManagerID);
            Guid forSaleTblID = CreateTbl("For Sale", "Property", "Any property that is on the rating in the regions covered by R8R is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem hierarchy = CreateHierarchyItem(realEstateHierarchy, forSaleTblID, true, "For Sale");
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateRealEstateFields(forSaleTblID);
            int ratingGroup = CreateTblTab("Ratings", forSaleTblID);
            int salesGroup = CreateTblTab("Sales Forecasts", forSaleTblID);
            CreateMain(ratingGroup, salesGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);

            thePointsManagerID = CreatePointsManager("Real Estate For Rent", null, null, standardDomain.DomainID, true);
            CreateRealEstateChoiceGroups(thePointsManagerID);
            Guid forRentTblID = CreateTbl("For Rent", "Property", "Any property that is available for rent in the regions covered by R8R is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            hierarchy = CreateHierarchyItem(realEstateHierarchy, forRentTblID, true, "For Rent");
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateRealEstateFields(forRentTblID);
            int ratingGroupRent = CreateTblTab("Ratings", forRentTblID);
            CreateMain(ratingGroupRent, null, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);

            BeginImport("realEstateForSale.xml", forSaleTblID);
            BeginImport("realEstateForRent.xml", forRentTblID);
        }
    }

    public class News : CreateStandardBase
    {

        public News()
            : base()
        {
        }

        protected void CreateAnswerColumn(int ratingGroup, int myRatingPhase)
        {
            int singleColumn = CreateEvent("Answer","Answer","Depending on the question, the answer may be a rating (0-10), a yes-no answer (0%-100%), a multiple choice answer (with each choice 0%-100%), or some other range of numbers.",ratingGroup,myRatingPhase,false,false,false);
        }

        protected void CreateQuestionChoiceGroups(Guid pointsManagerID)
        {
        }

        protected void CreateQuestionFields(Guid TblID)
        {
        }


        private int CreateNewsTable(int defaultType, Guid thePointsManagerID)
        {
            CreateQuestionChoiceGroups(thePointsManagerID);
            Guid newsTblID = CreateTbl("News", "Question", "Any question as important as those listed is eligible, and slightly less important questions should be added once reasonable answers to existing questions exist.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID, false, true);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateQuestionFields(newsTblID);
            int ratingGroup = CreateTblTab("Main", newsTblID);
            CreateAnswerColumn(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            return newsTblID;
        }

        public void CreateNewsTableAndHierarchy(string tableName)
        {
            Guid thePointsManagerID = CreatePointsManager(tableName, null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].eventRGA;
            Guid TblID = CreateNewsTable(defaultType, thePointsManagerID);
            HierarchyItem hierarchyItem = CreateHierarchyItem(newsHierarchy, TblID, true, tableName);
        }

        public override void Create()
        {
            base.Create();

            string[] theTables = { "Business-General", "Business-Markets", "Miscellaneous-Europe","Miscellaneous-US","Miscellaneous-World","Politics-Europe","Politics-US","Politics-World","Sports-US","Sports-World","Technology"};
            foreach (var table in theTables)
                CreateNewsTableAndHierarchy(table);
        }

    }

    public class PrivatePages : CreateStandardBase
    {

        public PrivatePages()
            : base()
        {
        }

        protected void CreateAnswerColumn(int ratingGroup, int myRatingPhase)
        {
            int singleColumn = CreateEvent("Answer", "Answer", "Depending on the question, the answer may be a rating (0-10), a yes-no answer (0%-100%), a multiple choice answer (with each choice 0%-100%), or some other range of numbers.", ratingGroup, myRatingPhase, false, false, false);
        }

        protected void CreateQuestionChoiceGroups(Guid pointsManagerID)
        {
        }

        protected void CreateQuestionFields(Guid TblID)
        {
        }


        private int CreatePrivatePagesTable(string tableName, int defaultType, Guid thePointsManagerID)
        {
            CreateQuestionChoiceGroups(thePointsManagerID);
            Guid PrivatePagesTblID = CreateTbl(tableName, "Question", "Any question as important as those listed is eligible, and slightly less important questions should be added once reasonable answers to existing questions exist.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID, false, true);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateQuestionFields(PrivatePagesTblID);
            int ratingGroup = CreateTblTab("Main", PrivatePagesTblID);
            CreateAnswerColumn(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            return PrivatePagesTblID;
        }

        public void CreatePrivatePagesTableAndHierarchy(string tableName)
        {
            Guid thePointsManagerID = CreatePointsManager(tableName, null, null, standardDomain.DomainID, true, false, 0.0M);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].eventRGA;
            Guid TblID = CreatePrivatePagesTable(tableName, defaultType, thePointsManagerID);
            HierarchyItem hierarchyItem = CreateHierarchyItem(null, TblID, false, tableName);
        }

        public override void Create()
        {
            base.Create();

            string[] theTables = { "Patent Litigation", "Corporations Class 2011" };
            foreach (var table in theTables)
                CreatePrivatePagesTableAndHierarchy(table);
        }

    }


    public class Entertainment : CreateStandardBase
    {
        Guid theSexChoiceGroupID;

        public Entertainment()
            : base()
        {
        }

        protected void CreateActorsMain(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Popularity", "Current popularity", "The current popularity of this actor or actress", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
            CreateRating("Long-term", "Long-term popularity", "How popular this actor or actress is likely to be over the long term", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Ability", "Acting ability", "The acting ability of this actor or actress", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Hot", "Hotness", "The physical attractiveness and general hotness of this actor or actress.", ratingGroup, myRatingPhase, false, true, false);
        }

        protected void CreateActorsChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();
            ChoiceGroupData theSexChoiceGroup = new ChoiceGroupData();
            theSexChoiceGroup.AddChoiceToGroup("Female");
            theSexChoiceGroup.AddChoiceToGroup("Male");
            theSexChoiceGroupID = CreateChoiceGroup(pointsManagerID, theSexChoiceGroup, choiceGroupStandardSettings, null, "Sex");
        }

        protected void CreateActorsFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int sexCGFD = 0;
            int sexFD = CreateFieldDefinition(TblID, "Sex", FieldTypes.ChoiceField, true, theSexChoiceGroupID, null, ref sexCGFD);
            int dateFD = CreateFieldDefinition(TblID, "Date of Birth", FieldTypes.DateTimeField, true, true, false);
            int imdbFD = CreateFieldDefinition(TblID, "IMDb", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(imdbFD, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
        }


        private int CreateActors(int defaultType, Guid thePointsManagerID)
        {
            CreateActorsChoiceGroups(thePointsManagerID);
            Guid actorsTblID = CreateTbl("Actors and Actresses", "Actor/Actress", "Any actor about as prominent as those included here is eligible, and slightly less prominent actors are eligible once ratings on included actors become relatively stable.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateActorsFields(actorsTblID);
            int ratingGroup = CreateTblTab("Ratings", actorsTblID);
            CreateActorsMain(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("actors.xml", actorsTblID);
            return actorsTblID;
        }

        protected void CreateRealityStarsMain(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Popularity", "Current popularity", "The current popularity of this reality show star", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
            CreateRating("Long-term", "Long-term popularity", "How popular this reality show star is likely to be over the long term", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Talent", "Talent", "The talent of this reality show star, if any", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Hot", "Hotness", "The physical attractiveness and general hotness of this reality show star", ratingGroup, myRatingPhase, false, true, false);
        }

        protected void CreateRealityStarsChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();
        }

        protected void CreateRealityStarsFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int sexCGFD = 0;
            int sexFD = CreateFieldDefinition(TblID, "Sex", FieldTypes.ChoiceField, true, theSexChoiceGroupID, null, ref sexCGFD);
            int dateFD = CreateFieldDefinition(TblID, "Date of Birth", FieldTypes.DateTimeField, true, true, false);
            int wikiFD = CreateFieldDefinition(TblID, "Wikipedia", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(wikiFD, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
        }

        private int CreateRealityStars(int defaultType, Guid thePointsManagerID)
        {
            CreateRealityStarsChoiceGroups(thePointsManagerID);
            Guid RealityTblID = CreateTbl("Reality Stars", "Reality Star", "Any reality show star about as prominent as those included here is eligible, and slightly less prominent reality show stars are eligible once ratings on included reality show stars become relatively stable.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateRealityStarsFields(RealityTblID);
            int ratingGroup = CreateTblTab("Ratings", RealityTblID);
            CreateRealityStarsMain(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("RealityStars.xml", RealityTblID);
            return RealityTblID;
        }

        protected void CreateMoviesMain(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Overall", "Overall", "The overall quality of this movie", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
            CreateRating("Film Buffs", "Film Buffs", "How much film buffs like this movie", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Action", "Action", "How exciting is the action in this movie", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Romantic", "Romance", "How romantic is this movie", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Funny", "Funny", "How funny is this movie", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Kids", "Kids", "How appropriate is this movie for young children", ratingGroup, myRatingPhase, false, true, false);
        }

        protected void CreateMoviesChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();
        }

        protected void CreateMoviesFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int dateFD = CreateFieldDefinition(TblID, "Release Date", FieldTypes.DateTimeField, true, true, false);
            int webFD = CreateFieldDefinition(TblID, "Website", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(webFD, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
        }


        private int CreateMovies(int defaultType, Guid thePointsManagerID)
        {
            CreateMoviesChoiceGroups(thePointsManagerID);
            Guid MoviesTblID = CreateTbl("Movies", "Movie", "Any movie that is currently playing is eligible to be added. Once ratings for current movies are relatively stable, significant older and classic movies can be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateMoviesFields(MoviesTblID);
            int ratingGroup = CreateTblTab("Ratings", MoviesTblID);
            CreateMoviesMain(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("Movies.xml", MoviesTblID);
            return MoviesTblID;
        }



        public override void Create()
        {
            base.Create();

            Guid thePointsManagerID = CreatePointsManager("Actors", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].ratingRGA;
            Guid TblID = CreateActors(defaultType, thePointsManagerID);
            HierarchyItem actorsItem = CreateHierarchyItem(entertainmentHierarchy, TblID, true, "Actors");
            thePointsManagerID = CreatePointsManager("RealityStars", null, null, standardDomain.DomainID, true);
            TblID = CreateRealityStars(defaultType, thePointsManagerID);
            HierarchyItem realityStarsItem = CreateHierarchyItem(entertainmentHierarchy, TblID, true, "Reality Stars");

            thePointsManagerID = CreatePointsManager("Movies", null, null, standardDomain.DomainID, true);
            TblID = CreateMovies(defaultType, thePointsManagerID);
            HierarchyItem moviesItem = CreateHierarchyItem(entertainmentHierarchy, TblID, true, "Movies");
        }

    }


    public class SimpleTestTable : CreateStandardBase
    {

        public SimpleTestTable()
            : base()
        {
        }

        protected void CreateSimpleTestMain(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Test", "Test name", "The current popularity of this actor or actress", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
        }

        
        private int CreateSimpleTest(int defaultType, Guid thePointsManagerID)
        {
            Guid simpleTestTblID = CreateTbl("Simple Tests", "Test item", "Any test item can be used here", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            int ratingGroup = CreateTblTab("Ratings", simpleTestTblID);
            CreateSimpleTestMain(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("simpletest.xml", simpleTestTblID);
            return simpleTestTblID;
        }

        bool useExtraLongRatingPhaseGroup = false;
        public void UseExtraLongRatingPhaseGroup()
        {
            useExtraLongRatingPhaseGroup = true;
        }

        public override void Create()
        {
            base.Create();

            Guid thePointsManagerID = CreatePointsManager("SimpleTest", null, null, standardDomain.DomainID, true);
            int defaultType = useExtraLongRatingPhaseGroup ? standardObjectsForPointsManager[thePointsManagerID].ratingVeryLongRGA : standardObjectsForPointsManager[thePointsManagerID].ratingRGA;
            Guid TblID = CreateSimpleTest(defaultType, thePointsManagerID);

            HierarchyItem testHierarchy = CreateHierarchyItem(null, null, true, "Test Hierarchy");
            HierarchyItem testItem = CreateHierarchyItem(testHierarchy, TblID, true, "Test Items");
        }

    }


    public class SimpleEventTestTable : CreateStandardBase
    {
        public SimpleEventTestTable()
            : base()
        {
        }

        protected void CreateSimpleTestMain(int tblTab, int myRatingPhase)
        {
            int overall = CreateEvent("Test", "Test name", "Probability of something", tblTab, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(tblTab, overall);
        }


        private int CreateSimpleTest(int defaultType, Guid thePointsManagerID)
        {
            Guid simpleTestTblID = CreateTbl("Simple Event Tests", "Test item", "Any test item can be used here", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            int tblTab = CreateTblTab("Ratings", simpleTestTblID);
            CreateSimpleTestMain(tblTab, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            return simpleTestTblID;
        }


        public override void Create()
        {
            base.Create();

            Guid thePointsManagerID = CreatePointsManager("SimpleEventTest", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].ratingRGA;
            Guid TblID = CreateSimpleTest(defaultType, thePointsManagerID);

            HierarchyItem testHierarchy = CreateHierarchyItem(null, null, true, "Test Event Hierarchy");
            HierarchyItem testItem = CreateHierarchyItem(testHierarchy, TblID, true, "Test Event Items");
        }

    }


    public class Government : CreateStandardBase
    {
        int choiceGroupStandardSettings;
        Guid theSexChoiceGroupID;
        Guid thePartyChoiceGroupID;
        Guid thePoliticianSinglePositionChoiceGroupID;
        Guid thePoliticianMultiplePositionChoiceGroupID; 
        Guid fedOrStateChoiceGroupID; 
        Guid jurisdictionChoiceGroupID; 
        Guid judgePositionChoiceGroupID;
        Guid courtChoiceGroupID;
        string[] states = { "Alabama", "Alaska", "American Samoa", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "District of Columbia", "Florida", "Georgia", "Guam", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Northern Marianas Islands", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Puerto Rico", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Virgin Islands", "Washington", "West Virginia", "Wisconsin", "Wyoming" };

        public Government() : base()
        {
        }

        protected void CreateSharedChoiceGroupsForPeople(Guid pointsManagerID)
        {
            choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, true, true, true, true, false, true, true);
            int choiceGroupStandardSettingsNoAlphabetize = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, false, true, true, true, false, false, true);

            ChoiceGroupData theSexChoiceGroup = new ChoiceGroupData();
            theSexChoiceGroup.AddChoiceToGroup("Female");
            theSexChoiceGroup.AddChoiceToGroup("Male");
            theSexChoiceGroupID = CreateChoiceGroup(pointsManagerID, theSexChoiceGroup, choiceGroupStandardSettings, null, "Sex");

            ChoiceGroupData thePartyChoiceGroup = new ChoiceGroupData();
            thePartyChoiceGroup.AddChoiceToGroup("Democratic");
            thePartyChoiceGroup.AddChoiceToGroup("Republican");
            thePartyChoiceGroup.AddChoiceToGroup("Other party");
            thePartyChoiceGroup.AddChoiceToGroup("Independent/Unaffiliated");
            thePartyChoiceGroupID = CreateChoiceGroup(pointsManagerID, thePartyChoiceGroup, choiceGroupStandardSettingsNoAlphabetize, null, "Party");
        }

        protected void CreatePoliticianColumns(int ratingGroup, int myRatingPhase)
        {
            int power = CreateRating("Power", "Power", "Overall power", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, power);
            int overall = CreateRating("Effectiveness", "Effectiveness", "Effectiveness as a legislator or leader, placing aside political views", ratingGroup, myRatingPhase, true, true, false);
            int politics = CreateRating("Politics", "Political skills", "Political skills, in obtaining both positions and legislative goals", ratingGroup, myRatingPhase, true, true, false);
            CreateRatingNegativeToPositive("Social", "Social issues", "How liberal/conservative this person is on social issues (-10 = most liberal, 10 = most conservative)", ratingGroup, myRatingPhase, true, true, false);
            CreateRatingNegativeToPositive("Economic", "Economic issues", "How liberal/conservative this person is on economic issues (-10 = most liberal, 10 = most conservative)", ratingGroup, myRatingPhase, true, true, false);
            CreateRatingNegativeToPositive("Security", "Security issues", "How liberal/conservative this person is on national security and domestic security issues (-10 = most liberal, 10 = most conservative)", ratingGroup, myRatingPhase, true, true, false);
        }

        protected void CreatePoliticianChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettingsNoAlphabetize = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, false, true, true, true, false, false, true);
            int choiceGroupMultipleSettingsNoAlphabetize = ChoiceGroupSettingsMask.GetChoiceGroupSetting(true, false, true, true, true, false, false, true);

            ChoiceGroupData thePositionChoiceGroup = new ChoiceGroupData();
            thePositionChoiceGroup.AddChoiceToGroup("U.S. President");
            thePositionChoiceGroup.AddChoiceToGroup("U.S. Senate");
            thePositionChoiceGroup.AddChoiceToGroup("U.S. House");
            thePositionChoiceGroup.AddChoiceToGroup("U.S. executive branch");
            thePositionChoiceGroup.AddChoiceToGroup("Governor");
            thePositionChoiceGroup.AddChoiceToGroup("State legislature");
            thePositionChoiceGroup.AddChoiceToGroup("State executive branch");
            thePositionChoiceGroup.AddChoiceToGroup("County government");
            thePositionChoiceGroup.AddChoiceToGroup("Mayor");
            thePositionChoiceGroup.AddChoiceToGroup("City government");
            thePoliticianSinglePositionChoiceGroupID = CreateChoiceGroup(pointsManagerID, thePositionChoiceGroup, choiceGroupStandardSettingsNoAlphabetize, null, "Position");
            thePoliticianMultiplePositionChoiceGroupID = CreateChoiceGroup(pointsManagerID, thePositionChoiceGroup, choiceGroupMultipleSettingsNoAlphabetize, null, "Position");

            ChoiceGroupData theSexChoiceGroup = new ChoiceGroupData();
            theSexChoiceGroup.AddChoiceToGroup("Female");
            theSexChoiceGroup.AddChoiceToGroup("Male");
            theSexChoiceGroupID = CreateChoiceGroup(pointsManagerID, theSexChoiceGroup, choiceGroupStandardSettingsNoAlphabetize, null, "Sex");

            ChoiceGroupData thePartyChoiceGroup = new ChoiceGroupData();
            thePartyChoiceGroup.AddChoiceToGroup("Democratic");
            thePartyChoiceGroup.AddChoiceToGroup("Republican");
            thePartyChoiceGroup.AddChoiceToGroup("Other party");
            thePartyChoiceGroup.AddChoiceToGroup("Independent");
            thePartyChoiceGroupID = CreateChoiceGroup(pointsManagerID, thePartyChoiceGroup, choiceGroupStandardSettingsNoAlphabetize, null, "Party");
        }

        protected void CreatePoliticianFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int linkFD = CreateFieldDefinition(TblID, "Link", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(linkFD, visibleWithNoNameFieldDisplay, doNotShowThisField, visibleWithNoNameFieldDisplay);
            int positionCGFD = 0;
            int currentPosition = CreateFieldDefinition(TblID, "Current position", FieldTypes.ChoiceField, true, thePoliticianSinglePositionChoiceGroupID, null, ref positionCGFD);
            int pastPositionCGFD = 0;
            int pastPosition = CreateFieldDefinition(TblID, "Past position(s)", FieldTypes.ChoiceField, true, thePoliticianMultiplePositionChoiceGroupID, null, ref pastPositionCGFD);
            int positionSoughtCGFD = 0;
            int positionSought = CreateFieldDefinition(TblID, "Position sought or possible", FieldTypes.ChoiceField, true, thePoliticianSinglePositionChoiceGroupID, null, ref positionSoughtCGFD);
            int partyCGFD = 0;
            int party = CreateFieldDefinition(TblID, "Party", FieldTypes.ChoiceField, true, thePartyChoiceGroupID, null, ref partyCGFD);
            int sexCGFD = 0;
            int sex = CreateFieldDefinition(TblID, "Sex", FieldTypes.ChoiceField, true, theSexChoiceGroupID, null, ref sexCGFD);

            int birthDateFD = CreateFieldDefinition(TblID, "Date of Birth", FieldTypes.DateTimeField, false, true, false);

        }

        Guid politiciansTblID;
        private void CreatePoliticianTbl(int defaultType, Guid thePointsManagerID)
        {
            politiciansTblID = CreateTbl("Politicians", "Politician", "Any polician who is as prominent as the average politician already included may be added. Politicians of somewhat lower prominence should be added only once the existing politicians listed have almost all received careful ratings.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreatePoliticianFields(politiciansTblID);
            int ratingGroup = CreateTblTab("Ratings", politiciansTblID);
            CreatePoliticianColumns(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("politicians.xml", politiciansTblID);
        }


        protected void CreateJudgeColumns(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Overall", "Overall performance", "Overall performance, placing aside political views", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
            CreateRating("Demeanor", "Demeanor and in-court performance", "Judicial demeanor and effectiveness in court", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Opinions", "Written opinions", "Quality, thoroughness, and thoughtfulness, placing aside political views", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Fairness", "Fairness", "Judicial fairness and commitment to the law above personal or political preference", ratingGroup, myRatingPhase, true, true, false);
            CreateRatingNegativeToPositive("Social", "Social issues", "How liberal/conservative this person is on social issues (-10 = most liberal, 10 = most conservative)", ratingGroup, myRatingPhase, true, true, false);
            CreateRatingNegativeToPositive("Economic", "Economic issues", "How liberal/conservative this person is on economic issues (-10 = most liberal, 10 = most conservative)", ratingGroup, myRatingPhase, true, true, false);
            CreateRatingNegativeToPositive("Criminal", "Criminal justice issues", "How liberal/conservative this person is on criminal justice issues (-10 = most liberal, 10 = most conservative)", ratingGroup, myRatingPhase, true, true, false);
        }

        protected void CreateJudgeChoiceGroups(Guid pointsManagerID)
        {
            string[] fedJurs = { "National", "First Circuit", "Second Circuit", "Third Circuit", "Fourth Circuit", "Fifth Circuit", "Sixth Circuit", "Seventh Circuit", "Eighth Circuit", "Ninth Circuit", "Tenth Circuit", "Eleventh Circuit", "D.C. Circuit", "Other" };
            string[] nationalCourts = { "Supreme Court", "Federal Circuit", "Ct. Intl. Trade", "Administrative Courts", "Tax Court", "Fed. Claims", "Other" };
            string[] Cir1Cts = { "First Circuit", "D. Me.", "D. Mass.", "D.N.H.", "D.P.R.", "D.R.I." };
            string[] Cir2Cts = { "Second Circuit", "D. Conn.", "E.D.N.Y.", "N.D.N.Y.", "S.D.N.Y.", "W.D.N.Y.", "D. Vt."};
            string[] Cir3Cts = { "Third Circuit", "D. Del.", "D.N.J.", "E.D. Pa.", "M.D. Pa.", "W.D. Pa.", "D.V.I." };
            string[] Cir4Cts = { "Fourth Circuit", "D. Md.", "E.D.N.C.", "M.D.N.C.", "W.D.N.C.", "D.S.C.", "E.D. Va.", "W.D. Va.", "N.D.W. Va.", "S.D.W. Va." };
            string[] Cir5Cts = { "Fifth Circuit", "E.D. La.", "M.D. La.", "W.D. La.", "N.D. Miss.", "S.D. Miss.", "E.D. Tex.", "N.D. Tex.", "S.D. Tex.", "W.D. Tex." };
            string[] Cir6Cts = { "Sixth Circuit", "E.D. Ky.", "W.D. Ky.", "E.D. Mich.", "W.D. Mich.", "N.D. Ohio", "S.D. Ohio", "E.D. Tenn.", "M.D. Tenn.", "W.D. Tenn."};
            string[] Cir7Cts = { "Seventh Circuit", "C.D. Ill.", "N.D. Ill.", "S.D. Ill.", "N.D. Ind.", "S.D. Ind.", "E.D. Wis.", "W.D. Wis."};
            string[] Cir8Cts = { "Eighth Circuit", "E.D. Ark.", "W.D. Ark.", "N.D. Iowa", "S.D. Iowa", "D. Minn.", "E.D. Mo.", "W.D. Mo.", "D. Neb.", "D.N.D.", "D.S.D."};
            string[] Cir9Cts = { "Ninth Circuit", "D. Alaska", "D. Ariz.", "C.D. Cal.", "E.D. Cal.", "N.D. Cal.", "S.D. Cal.", "D. Guam", "D. Haw.", "D. Idaho", "D. Mont.", "D.N. Mar. Is.", "D. Nev.",  "D. Ore.", "E.D. Wash.", "W.D. Wash."};
            string[] Cir10Cts = { "Tenth Circuit", "D. Col.", "D. Kan.", "D.N.M.", "E.D. Okla.", "W.D. Okla.", "N.D. Okla.", "D. Utah", "D. Wyo."};
            string[] Cir11Cts = { "Eleventh Circuit", "M.D. Ala.", "N.D. Ala.", "S.D. Ala.", "M.D. Fla.", "N.D. Fla.", "S.D. Fla.", "M.D. Ga.", "N.D. Ga.", "S.D. Ga." };
            string[] CirDCCts = { "D.C. Circuit", "D.D.C." };
            string[] CirOther = { "Other" };

            string[] statePositions = { "State highest court judge", "State appellate judge", "State trial judge", "State administrative judge" };
            string[] federalPositions = { "Supreme Court Justice", "Federal appellate judge", "Federal trial judge", "Federal administrative judge", "Federal bankruptcy judge", "Other" };

            DepItems[] theHierarchy = { 
                                          new DepItems("Federal", fedJurs),
                                          new DepItems("State", states)
                                      };

            int noAlphabetizeChoiceGroupSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, false, false, false, false, true, false, false);
            List<int> choiceGroupIDs = CreateHierarchyChoiceGroup(pointsManagerID, theHierarchy, new List<string> { "Federal/State", "Jurisdiction" }, noAlphabetizeChoiceGroupSettings);
            fedOrStateChoiceGroupID = choiceGroupIDs[0];
            jurisdictionChoiceGroupID = choiceGroupIDs[1];

            judgePositionChoiceGroupID = CreateDependentChoiceGroup(pointsManagerID, fedOrStateChoiceGroupID, new DepItemsLevel[] { new DepItemsLevel("Federal", federalPositions), new DepItemsLevel("State", statePositions) }, "Position", noAlphabetizeChoiceGroupSettings);
            courtChoiceGroupID = CreateDependentChoiceGroup(pointsManagerID, jurisdictionChoiceGroupID, new DepItemsLevel[] {
                new DepItemsLevel("National", nationalCourts),
                new DepItemsLevel("First Circuit", Cir1Cts),
                new DepItemsLevel("Second Circuit", Cir2Cts),
                new DepItemsLevel("Third Circuit", Cir3Cts),
                new DepItemsLevel("Fourth Circuit", Cir4Cts),
                new DepItemsLevel("Fifth Circuit", Cir5Cts),
                new DepItemsLevel("Sixth Circuit", Cir6Cts),
                new DepItemsLevel("Seventh Circuit", Cir7Cts),
                new DepItemsLevel("Eighth Circuit", Cir8Cts),
                new DepItemsLevel("Ninth Circuit", Cir9Cts),
                new DepItemsLevel("Tenth Circuit", Cir10Cts),
                new DepItemsLevel("Eleventh Circuit", Cir11Cts),
                new DepItemsLevel("D.C. Circuit", CirDCCts),
                new DepItemsLevel("Other", CirOther)}, "Court", noAlphabetizeChoiceGroupSettings);
        }

        protected void CreateJudgeFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int linkFD = CreateFieldDefinition(TblID, "Link", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(linkFD, visibleWithNoNameFieldDisplay, doNotShowThisField, visibleWithNoNameFieldDisplay);
            int fedOrStateCGFD = 0;
            int fedOrState = CreateFieldDefinition(TblID, "Federal/State", FieldTypes.ChoiceField, true, fedOrStateChoiceGroupID, null, ref fedOrStateCGFD);
            int positionCGFD = 0;
            int currentPosition = CreateFieldDefinition(TblID, "Current position", FieldTypes.ChoiceField, true, judgePositionChoiceGroupID, fedOrStateCGFD, ref positionCGFD);
            int jurisdictionCGFD = 0;
            int jurisdiction = CreateFieldDefinition(TblID, "Jurisdiction", FieldTypes.ChoiceField, true, jurisdictionChoiceGroupID, fedOrStateCGFD, ref jurisdictionCGFD);
            int courtCGFD = 0;
            int court = CreateFieldDefinition(TblID, "Court", FieldTypes.ChoiceField, true, courtChoiceGroupID, jurisdictionCGFD, ref courtCGFD);
            int partyCGFD = 0;
            int party = CreateFieldDefinition(TblID, "Party", FieldTypes.ChoiceField, true, thePartyChoiceGroupID, null, ref partyCGFD);
            int sexCGFD = 0;
            int sex = CreateFieldDefinition(TblID, "Sex", FieldTypes.ChoiceField, true, theSexChoiceGroupID, null, ref sexCGFD);
            int birthDateFD = CreateFieldDefinition(TblID, "Date of Birth", FieldTypes.DateTimeField, false, true, false);

        }

        Guid judgesTblID;
        private void CreateJudgeTbl(int defaultType, Guid thePointsManagerID)
        {
            judgesTblID = CreateTbl("Judges", "Judge", "Any polician who is as prominent as the average Judge already included may be added. Judges of somewhat lower prominence should be added only once the existing Judges listed have almost all received careful ratings.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateJudgeFields(judgesTblID);
            int ratingGroup = CreateTblTab("Ratings", judgesTblID);
            CreateJudgeColumns(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("Judges.xml", judgesTblID);
        }


        Guid supremeCourtTblID;
        int supremeCourtHierarchyRGA;
        private void CreateSupremeCourtTbl(Guid thePointsManagerID)
        {
            supremeCourtTblID = CreateTbl("Supreme Court", "Case or Issue", "Any case accepted for Supreme Court hearing or significant issue in such a case is eligible to be included", standardObjectsForPointsManager[thePointsManagerID].eventRGA, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID, false);
            CreateSupremeCourtFields(supremeCourtTblID);
            int TblTab = CreateTblTab("Main", supremeCourtTblID);
            CreateSupremeCourtColumns(thePointsManagerID, TblTab, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("supremecourt.xml", supremeCourtTblID);
        }

        protected void CreateSupremeCourtFields(Guid TblID)
        {
            int caseName = CreateFieldDefinition(TblID, "Case name", FieldTypes.TextField, true, true, true, true);
            CreateFieldDefinitionDisplaySettings(caseName, doNotShowThisField, doNotShowThisField, doNotShowThisField);
            int docketNumber = CreateFieldDefinition(TblID, "Docket number", FieldTypes.TextField, true, true, false, true);
            CreateFieldDefinitionDisplaySettings(docketNumber, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
            int oralArgumentDate = CreateFieldDefinition(TblID, "Oral argument date", FieldTypes.DateTimeField, true, true, false);
            CreateFieldDefinitionDisplaySettings(oralArgumentDate, doNotShowThisField, visibleWithNameFieldDisplay, visibleWithNameFieldDisplay);
            int issue = CreateFieldDefinition(TblID, "Issue", FieldTypes.TextField, true, true, false, true);
            CreateFieldDefinitionDisplaySettings(issue, visibleWithNoNameFieldDisplay);
        }

        private void CreateSupremeCourtColumns(Guid pointsManagerID, int TblTab, Guid ratingPhaseID)
        {
            int probYes = CreateEvent("Yes", "Probability of \"Yes\"", "The percentage chance that the issue listed (such as whether the case will be affirmed) will have an answer of yes. The final value may be between 0 and 100 in the event of a split decision or an ambiguous opinion. If a majority of the Court does not decide an issue, then still pending ratings will be cancelled.", TblTab, ratingPhaseID, true, true, false);

            int votesRating = Action.RatingCharacteristicsCreate(standardObjectsForPointsManager[pointsManagerID].theRatingPhaseStandardID, null, 0, 9, 1, "Votes", true, true, superUser, null);
            int indVoteRating = Action.RatingCharacteristicsCreate(standardObjectsForPointsManager[pointsManagerID].theRatingPhaseStandardID, null, 0, 1, 1, "Individual Vote", true, true, superUser, null);

            RatingHierarchyData theHierarchy = new RatingHierarchyData();
            // Note: The following was changed after creation of the database to reflect change in Supreme Court membership. See DatabaseTransitions.cs for transition code.
            theHierarchy.Add("Total votes", 9, 1, "Total votes");
            theHierarchy.Add("Yes votes", 4.5M, 2, "Total expected yes votes");
            theHierarchy.Add("Roberts", 0.5M, 3, "Probability Roberts votes yes");
            theHierarchy.Add("Scalia", 0.5M, 3, "Probability Scalia votes yes");
            theHierarchy.Add("Kennedy", 0.5M, 3, "Probability Kennedy votes yes");
            theHierarchy.Add("Thomas", 0.5M, 3, "Probability Thomas votes yes");
            theHierarchy.Add("Ginsburg", 0.5M, 3, "Probability Ginsburg votes yes");
            theHierarchy.Add("Breyer", 0.5M, 3, "Probability Breyer votes yes");
            theHierarchy.Add("Alito", 0.5M, 3, "Probability Alito votes yes");
            theHierarchy.Add("Sotomayor", 0.5M, 3, "Probability Sotomayor votes yes");
            theHierarchy.Add("Kagan", 0.5M, 3, "Probability Kagan votes yes");
            theHierarchy.Add("No votes", 4.5M, 2, "Total expected no votes");
            var yesVotesItem = theHierarchy.RatingHierarchyEntries.Single(x => x.RatingName == "Yes votes");
            var replacement = new List<ActionProcessor.RatingCharacteristicsHierarchyOverride> { new ActionProcessor.RatingCharacteristicsHierarchyOverride { theEntryForRatingGroupWhoseMembersWillHaveDifferentCharacteristics = yesVotesItem, theReplacementCharacteristicsID = indVoteRating} };
            supremeCourtHierarchyRGA = Action.RatingGroupAttributesCreate(votesRating, replacement, null, null, theHierarchy, "Supreme Court vote totals", RatingGroupTypes.hierarchyNumbersTop, "Supreme Court vote totals", false, false, new List<RatingHierarchyEntry> { yesVotesItem }, (decimal)0.5, true, true, superUser, myChangesGroup, pointsManagerID);
            int votesColumn = CreateTblColumn("Votes", "Number of expected votes", "The value for a Justice should be set equal to the probability (between 0 and 1) of a yes vote. The final value of a Justice's yes vote may be between 0 and 1 in the event of a split decision or an ambiguous opinion. Total voting should be less than 9 when a Justice may not participate. If a Justice is replaced in a Term, the successor's vote will count for the original Justice.", TblTab, supremeCourtHierarchyRGA, false, true, false, true,"wf225");
        }

        public override void Create()
        {
            base.Create();
            Guid pointsManagerID = CreatePointsManager("Politicians", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[pointsManagerID].ratingRGA;
            CreateSharedChoiceGroupsForPeople(pointsManagerID);
            CreatePoliticianChoiceGroups(pointsManagerID);
            CreatePoliticianTbl(defaultType, pointsManagerID);
            HierarchyItem hierarchyItem = CreateHierarchyItem(governmentHierarchy, politiciansTblID, true, "Politicians");
            pointsManagerID = CreatePointsManager("Judges", null, null, standardDomain.DomainID, true);
            CreateSharedChoiceGroupsForPeople(pointsManagerID);
            CreateJudgeChoiceGroups(pointsManagerID);
            CreateJudgeTbl(defaultType, pointsManagerID);
            hierarchyItem = CreateHierarchyItem(governmentHierarchy, judgesTblID, true, "Judges");
            pointsManagerID = CreatePointsManager("Supreme Court", null, null, standardDomain.DomainID, true);
            CreateSupremeCourtTbl(pointsManagerID);
            hierarchyItem = CreateHierarchyItem(governmentHierarchy, supremeCourtTblID, true, "Supreme Court");
        }

    }


    public class Blogs : CreateStandardBase
    {
        Guid topicChoiceGroupID;

        public Blogs()
            : base()
        {
        }


        protected void CreateMain(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Overall", "Overall quality", "The overall quality of the blog", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall);
            CreateRating("General interest", "General interest", "How interesting the blog is to an average reader of blogs", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Special interest", "Specialized interest", "How interesting the blog is to someone with a particular interest in the subject matter of the blog", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Creativity", "Creativity", "How creative the blog is", ratingGroup, myRatingPhase, true, true, false);
            CreateRating("Objectivity", "Objectivity", "How objective the information on the blog is", ratingGroup, myRatingPhase, true, true, false);

        }

        protected void CreateBlogChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(true, true, true, true, true, false, true, true);

            ChoiceGroupData theTopicChoiceGroup = new ChoiceGroupData();
            theTopicChoiceGroup.AddChoiceToGroup("Academics");
            theTopicChoiceGroup.AddChoiceToGroup("Arts & Culture");
            theTopicChoiceGroup.AddChoiceToGroup("Business");
            theTopicChoiceGroup.AddChoiceToGroup("Children");
            theTopicChoiceGroup.AddChoiceToGroup("Computers");
            theTopicChoiceGroup.AddChoiceToGroup("Entertainment");
            theTopicChoiceGroup.AddChoiceToGroup("Food");
            theTopicChoiceGroup.AddChoiceToGroup("Gadgets");
            theTopicChoiceGroup.AddChoiceToGroup("Government");
            theTopicChoiceGroup.AddChoiceToGroup("Health");
            theTopicChoiceGroup.AddChoiceToGroup("Hobbies");
            theTopicChoiceGroup.AddChoiceToGroup("Internet");
            theTopicChoiceGroup.AddChoiceToGroup("Law");
            theTopicChoiceGroup.AddChoiceToGroup("News");
            theTopicChoiceGroup.AddChoiceToGroup("Personal");
            theTopicChoiceGroup.AddChoiceToGroup("Politics");
            theTopicChoiceGroup.AddChoiceToGroup("Religion");
            theTopicChoiceGroup.AddChoiceToGroup("Science");
            theTopicChoiceGroup.AddChoiceToGroup("Sports");
            theTopicChoiceGroup.AddChoiceToGroup("Technology");
            theTopicChoiceGroup.AddChoiceToGroup("Teen");
            topicChoiceGroupID = CreateChoiceGroup(pointsManagerID, theTopicChoiceGroup, choiceGroupStandardSettings, null, "Topic");
        }

        protected void CreateBlogFields(Guid TblID)
        {
            int linkFD = CreateFieldDefinition(TblID, "Link", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(linkFD, visibleWithNoNameFieldDisplay, doNotShowThisField, visibleWithNoNameFieldDisplay);
            int topicCGFD = 0;
            int cuisineFD = CreateFieldDefinition(TblID, "Topic", FieldTypes.ChoiceField, true, topicChoiceGroupID, null, ref topicCGFD);
        }

        public override void Create()
        {
            base.Create();
            Guid thePointsManagerID = CreatePointsManager("Blogs", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].ratingRGA;
            CreateBlogChoiceGroups(thePointsManagerID);
            Guid blogsTblID = CreateTbl("Blogs", "Blog", "Any blog that is as prominent as the average blog already included may be added. Blogs of somewhat lower prominence, even though they may end up being higher rated than more prominent blogs, should be added only once the existing blogs listed have almost all received careful ratings.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem blogsItem = CreateHierarchyItem(internetHierarchy, blogsTblID, true, "Blogs");

            thePointsManagerID = CreatePointsManager("Blog posts", null, null, standardDomain.DomainID, true);
            CreateBlogChoiceGroups(thePointsManagerID);
            Guid blogPostsTblID = CreateTbl("Blog posts", "Blog post", "Any blog post or similar item of potentially general interest may be added. The number of posts that qualify as being of sufficiently general interest should grow only once there are enough users so that existing posts are receiving sufficiently careful ratings.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            HierarchyItem blogPostsItem = CreateHierarchyItem(internetHierarchy, blogPostsTblID, true, "Blog Posts");
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            CreateBlogFields(blogsTblID);
            int ratingGroup = CreateTblTab("Ratings", blogsTblID);
            CreateMain(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            int ratingGroupPosts = CreateTblTab("Ratings", blogPostsTblID);
            BeginImport("blogs.xml", blogsTblID);
        }
    }


    public class Restaurants : CreateStandardBase
    {
        Guid theCuisineChoiceGroupID; 
        Guid theSubCuisineChoiceGroupID;
        Guid theFeaturesChoiceGroupID;

        public Restaurants()
            : base()
        {
        }


        protected void CreateMain(int ratingGroup, int myRatingPhase)
        {
            int overall = CreateRating("Overall", "Overall quality", "The overall quality of the restaurant, not taking into account price", ratingGroup, myRatingPhase, true, true, false);
            SetColumnAsDefaultSort(ratingGroup, overall); //NOTE: If adding back in, uncomment SetCategoryAsDefaultSort below.
            int food = CreateRating("Food", "Quality of food", "The taste, creativity, and plating of the food", ratingGroup, myRatingPhase, false, true, false);
            //SetColumnAsDefaultSort(ratingGroup, food);
            //int taste = CreateRating("Taste", "Taste of food", "How good the food tastes", ratingGroup, myRatingPhase, false, true, false);
            //CreateRating("Creativity", "Creativity of food", "The overall creativity of the food, including plating and selection of dishes and ingredients", ratingGroup, myRatingPhase, false, true, false);
            //CreateRating("Service", "Quality of service", "The knowledge and attentiveness of the service.", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Atmosphere", "Atmosphere and decor", "The overall quality of the decor and atmosphere", ratingGroup, myRatingPhase, false, true, false);
            CreateDollarForecastOrRating("Price", "Price of a meal", "The average price of a meal (dinner if different meals are served), including the food, drink, and tip that an average diner would buy", ratingGroup, myRatingPhase, 2, 0, 1000, "wv15", true, true, false, false);
            //CreateStat("Speed", "Speed in minutes", "The average number of minutes that a diner will spend in the restaurant (dinner if different meals are served)", ratingGroup, myRatingPhase, 0, 0, 300, false, true, true, null);
        }

        protected void CreateAdditional(int ratingGroup, int myRatingPhase)
        {
            int plating = CreateRating("Plating", "Presentation and plating", "The attractiveness of the dishes", ratingGroup, myRatingPhase, false, true, false);
            int clientele = CreateRating("Clientele", "Clientele", "The quality of the clientele (taking into account friendliness, hipness, attractiveness, etc.)", ratingGroup, myRatingPhase, false, true, false);
            CreateRating("Variety", "Choices and menu size", "The number and range of menu offerings", ratingGroup, myRatingPhase, false, true, false);
            CreateStat("Speed", "Speed in minutes", "The average number of minutes that a diner will spend in the restaurant (dinner if different meals are served)", ratingGroup, myRatingPhase, 0, 0, 300, false, true, true, null);
        }

        protected void CreateRestaurantChoiceGroups(Guid pointsManagerID)
        {
            int choiceGroupStandardSettings = ChoiceGroupSettingsMask.GetStandardSetting();
            ChoiceGroupData theCuisineChoiceGroup = new ChoiceGroupData();
            theCuisineChoiceGroup.AddChoiceToGroup("American");
            theCuisineChoiceGroup.AddChoiceToGroup("Asian");
            theCuisineChoiceGroup.AddChoiceToGroup("Bakery");
            theCuisineChoiceGroup.AddChoiceToGroup("Bar / Pub / Brewery");
            theCuisineChoiceGroup.AddChoiceToGroup("Barbecue");
            theCuisineChoiceGroup.AddChoiceToGroup("Breakfast / Brunch");
            theCuisineChoiceGroup.AddChoiceToGroup("Burgers");
            theCuisineChoiceGroup.AddChoiceToGroup("Cafe / Coffee Shop");
            theCuisineChoiceGroup.AddChoiceToGroup("Cafeteria");
            theCuisineChoiceGroup.AddChoiceToGroup("Cajun / Creole");
            theCuisineChoiceGroup.AddChoiceToGroup("California");
            theCuisineChoiceGroup.AddChoiceToGroup("Chinese");
            theCuisineChoiceGroup.AddChoiceToGroup("Contemporary");
            theCuisineChoiceGroup.AddChoiceToGroup("Continental / European");
            theCuisineChoiceGroup.AddChoiceToGroup("Cuban");
            theCuisineChoiceGroup.AddChoiceToGroup("Deli / Sandwiches");
            theCuisineChoiceGroup.AddChoiceToGroup("Dessert / Ice Cream");
            theCuisineChoiceGroup.AddChoiceToGroup("Diner");
            theCuisineChoiceGroup.AddChoiceToGroup("Doughnuts");
            theCuisineChoiceGroup.AddChoiceToGroup("Eclectic");
            theCuisineChoiceGroup.AddChoiceToGroup("Family");
            theCuisineChoiceGroup.AddChoiceToGroup("Fast Food");
            theCuisineChoiceGroup.AddChoiceToGroup("Fine Dining");
            theCuisineChoiceGroup.AddChoiceToGroup("French");
            theCuisineChoiceGroup.AddChoiceToGroup("German");
            theCuisineChoiceGroup.AddChoiceToGroup("Greek");
            theCuisineChoiceGroup.AddChoiceToGroup("Hawaiian");
            theCuisineChoiceGroup.AddChoiceToGroup("Hot Dogs");
            theCuisineChoiceGroup.AddChoiceToGroup("Indian / Pakistani");
            theCuisineChoiceGroup.AddChoiceToGroup("Irish");
            theCuisineChoiceGroup.AddChoiceToGroup("Italian");
            theCuisineChoiceGroup.AddChoiceToGroup("Japanese");
            theCuisineChoiceGroup.AddChoiceToGroup("Juice");
            theCuisineChoiceGroup.AddChoiceToGroup("Korean");
            theCuisineChoiceGroup.AddChoiceToGroup("Latin American");
            theCuisineChoiceGroup.AddChoiceToGroup("Mediterranean");
            theCuisineChoiceGroup.AddChoiceToGroup("Mexican");
            theCuisineChoiceGroup.AddChoiceToGroup("Middle Eastern");
            theCuisineChoiceGroup.AddChoiceToGroup("Other");
            theCuisineChoiceGroup.AddChoiceToGroup("Pizza");
            theCuisineChoiceGroup.AddChoiceToGroup("Seafood");
            theCuisineChoiceGroup.AddChoiceToGroup("Soup");
            theCuisineChoiceGroup.AddChoiceToGroup("Southern");
            theCuisineChoiceGroup.AddChoiceToGroup("Southwestern");
            theCuisineChoiceGroup.AddChoiceToGroup("Steaks");
            theCuisineChoiceGroup.AddChoiceToGroup("Sushi");
            theCuisineChoiceGroup.AddChoiceToGroup("Tea House");
            theCuisineChoiceGroup.AddChoiceToGroup("Tex-Mex");
            theCuisineChoiceGroup.AddChoiceToGroup("Thai");
            theCuisineChoiceGroup.AddChoiceToGroup("Vegetarian");
            theCuisineChoiceGroup.AddChoiceToGroup("Vietnamese");
            theCuisineChoiceGroupID = CreateChoiceGroup(pointsManagerID, theCuisineChoiceGroup, choiceGroupStandardSettings, null, "Cuisine");

            Guid americanID = Action.DataAccess.R8RDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceGroupID == theCuisineChoiceGroupID && x.ChoiceText == "American").ChoiceInGroupID;
            Guid asianID = Action.DataAccess.R8RDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceGroupID == theCuisineChoiceGroupID && x.ChoiceText == "Asian").ChoiceInGroupID;
            Guid latinAmericanID = Action.DataAccess.R8RDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceGroupID == theCuisineChoiceGroupID && x.ChoiceText == "Latin American").ChoiceInGroupID;
            Guid indianPakistaniID = Action.DataAccess.R8RDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceGroupID == theCuisineChoiceGroupID && x.ChoiceText == "Indian / Pakistani").ChoiceInGroupID;

            int choiceGroupDependentSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(false, true, false, false, false, false, true, false);
            ChoiceGroupData theSubCuisineChoiceGroup = new ChoiceGroupData();
            theSubCuisineChoiceGroup.AddChoiceToGroup("New England", americanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("California", americanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Tex-Mex", americanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Midwestern", americanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Chinese", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Japanese", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Loatian", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Thai", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Vietnamese", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Sichuan", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Korean (South)", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Korean (North)", asianID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Brazilian", latinAmericanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Argentenian", latinAmericanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Peruvian", latinAmericanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Chilean", latinAmericanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Other Latin American", latinAmericanID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Indian (North)", indianPakistaniID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Indian (South)", indianPakistaniID);
            theSubCuisineChoiceGroup.AddChoiceToGroup("Pakistani", indianPakistaniID);
            theSubCuisineChoiceGroupID = CreateChoiceGroup(pointsManagerID, theSubCuisineChoiceGroup, choiceGroupDependentSettings, theCuisineChoiceGroupID, "Subcuisine");


            int choiceGroupMultipleSettings = ChoiceGroupSettingsMask.GetChoiceGroupSetting(true, true, false, false, false, false, true, false);
            ChoiceGroupData theFeaturesChoiceGroup = new ChoiceGroupData();
            theFeaturesChoiceGroup.AddChoiceToGroup("Accepts credit cards");
            theFeaturesChoiceGroup.AddChoiceToGroup("Vegetarian-friendly");
            theFeaturesChoiceGroup.AddChoiceToGroup("Independently owned");
            theFeaturesChoiceGroup.AddChoiceToGroup("Open late");
            theFeaturesChoiceGroup.AddChoiceToGroup("24 hours");
            theFeaturesChoiceGroup.AddChoiceToGroup("Allows pets");
            theFeaturesChoiceGroup.AddChoiceToGroup("Buffet-style");
            theFeaturesChoiceGroupID = CreateChoiceGroup(pointsManagerID, theFeaturesChoiceGroup, choiceGroupMultipleSettings , null, "Features");
        }



        protected void CreateRestaurantFields(Guid TblID)
        {
            int photoFD = CreateFieldDefinition(TblID, "Photo", FieldTypes.TextField, false, false, true, false);
            CreateFieldDefinitionDisplaySettings(photoFD, doNotShowThisField, pictureFieldDisplayTopRight, pictureFieldDisplayTopRight);
            int addressFD = CreateFieldDefinition(TblID, "Address", FieldTypes.AddressField, true);
            CreateFieldDefinitionDisplaySettings(addressFD, doNotShowThisField, visibleWithNoNameFieldDisplay, visibleWithNoNameFieldDisplay);
            int cuisineCGFD = 0;
            int cuisineFD = CreateFieldDefinition(TblID, "Cuisine", FieldTypes.ChoiceField, true, theCuisineChoiceGroupID, null, ref cuisineCGFD);
            int subcuisineCGFD = 0;
            int subcuisineFD = CreateFieldDefinition(TblID, "Subcuisine", FieldTypes.ChoiceField, true, theSubCuisineChoiceGroupID, cuisineCGFD, ref subcuisineCGFD);
            int featuresCGFD = 0;
            Guid featuresID = CreateFieldDefinition(TblID, "Features", FieldTypes.ChoiceField, true, theFeaturesChoiceGroupID, null, ref featuresCGFD);
            int phoneFD = CreateFieldDefinition(TblID, "Phone", FieldTypes.TextField, false, true, false, false);
            int sourceFD = CreateFieldDefinition(TblID, "Source", FieldTypes.TextField, false, true, true, false);
            int yearFD = CreateFieldDefinition(TblID, "Year Opened", FieldTypes.NumberField, true, 1750, 2050, 0);
        }

        public override void Create()
        {
            base.Create();
            Guid thePointsManagerID = CreatePointsManager("Restaurants", null, null, standardDomain.DomainID, true);
            int defaultType = standardObjectsForPointsManager[thePointsManagerID].ratingRGA;
            CreateRestaurantChoiceGroups(thePointsManagerID);
            Guid restaurantsTblID = CreateTbl("Restaurants", "Restaurant", "Any restaurant in the region covered by R8R is eligible to be added.", defaultType, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID, null, null, thePointsManagerID);
            //Action.TblChangeStyles(propertiesTblID, "mainTableHeadingSmall", "mainTableSmall", true, superUser, null);
            HierarchyItem hierarchy = CreateHierarchyItem(consumerHierarchy, restaurantsTblID, true, "Restaurants");
            CreateRestaurantFields(restaurantsTblID);
            int ratingGroup = CreateTblTab("Main", restaurantsTblID);
            CreateMain(ratingGroup, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            int ratingGroup2 = CreateTblTab("Additional", restaurantsTblID);
            CreateAdditional(ratingGroup2, standardObjectsForPointsManager[thePointsManagerID].theRatingPhaseStandardID);
            BeginImport("restaurants.xml", restaurantsTblID);
        }
    }



}
