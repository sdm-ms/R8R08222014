using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.Linq.Mapping;
using System.Data.Linq.Provider;
using System.Data.Linq.SqlClient;
using System.Data.OleDb;
using System.Data.ProviderBase;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;


using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using MoreStrings;

namespace ClassLibrary1.Model
{
    /// <summary>
    /// TradingStatus data type
    /// This specifies the trading status of a PointsManager, Tbl, TblRow, RatingGroup, and Rating. Trading can proceed only 
    /// if all are Active.
    /// </summary>
    public enum TradingStatus
    {
        NotYetStarted,
        Active,
        SuspendedDirectly,      // Trading has been suspended at this level.
        SuspendedHigherLevel,   // Trading was active but is suspended because of a suspension at higher level (e.g., Tbl)
        Ended
    }

    /// <summary>
    /// StatusOfChanges data type
    /// This is used in the StatusOfChanges field of the ChangesGroup to keep track of proposed changes to the database.
    /// </summary>
    public enum StatusOfChanges
    {
        NotYetProposed,         // User has defined this change group, but hasn't proposed using it.
        Proposed,               // User has proposed this change group.
        OnTrackReject,          // Based on a rating determination, this change group is on track for rejection (but that could change).
        OnTrackAccept,          // Based on a rating determination, this change group is on track for acceptance (but that could change).
        Rejected,               // This change group has been rejected. The user who created it could repropose it later.
        Accepted,               // This change group has been accepted, but processing still needs to occur before it can be converted to active (sometimes, after a delay whose exact length is hidden from users to prevent manipulation)
        Implemented,            // This change group has been implemented.
        Failed                  // Tried to implement this change group, but an error occurred.
    }

    /// <summary>
    /// UserLoginResult data type
    /// This is used to specify the result of the check of the user's username and password.
    /// </summary>
    public enum UserLoginResult
    {
        Valid,
        UsernameUnknown,
        PasswordInvalid,
        AccountNotVerified
    }

    /// <summary>
    /// UserAction data type
    /// This is used to specify the type of action the user wants to perform. This is passed to the CheckUserRights
    /// function to determine whether the user can do the relevant action.
    /// </summary>
    public enum UserActionType
    {
        View,
        Predict,
        AddTblsAndChangePointsManagers,
        ResolveRatings,
        ChangeTblRows,
        ChangeChoiceGroups,
        ChangeCharacteristics,
        ChangeColumns,
        ChangeUsersRights,
        AdjustPoints,
        ChangeProposalSettings,
        Other
    }

    /// Objects that are proposed, meanwhile, have a Status field that is set to Proposed,
    /// and then once the changes including the objects are approved to Active.
    /// For some objects (now, ChoiceInGroup objects), an Active object can later be set to Unavailable.
    /// We don't just use the StatusOfChanges object because there might be multiple such statuses for a single change.
    /// Once set to Status of Active, an object will stay that way even if another change including it is rejected.
    /// Note that some objects do not have a Status field. This is because they are always created without an approval
    /// process. This includes Ratings (but note that Ratings are only created based on RatingPlans), RatingGroupPhaseStatus,
    /// RatingStatus, PointsTotals, UserRating, and UserRatingGroups. In addition, the tables that are used to keep track
    /// of proposed additions of other objects (all of which start with the word "Changes") do not have a Status field.
    public enum StatusOfObject
    {
        Proposed,
        Active,
        DerivativelyUnavailable, // higher in hierarchy object is inactive
        Unavailable,
        AboutToBeReplaced // temporary status to be changed to unavailable
    }

    /// <summary>
    /// Enumerate the type of object. This allows us, e.g., to use a single field in ChangesStatusOfObject to refer
    /// to any of many possible objects.
    /// </summary>
    public enum TypeOfObject
    {
        BeforeFirst,            // A dummy value so that we can check ranges
        AddressField,
        AdministrationRight,
        AdministrationRightsGroup,
        TblColumn,
        TblColumnFormatting,
        TblTab,
        ChoiceField,
        ChoiceGroup,
        ChoiceGroupFieldDefinition,
        ChoiceInGroup,
        ChoiceInField,
        Tbl,
        Comment,
        CSSClasses,
        CSSTbls,
        CSSPlaces,
        DateTimeField,
        DateTimeFieldDefinition,
        Domain,
        TblRow,
        FieldDefinition,
        Field,
        HierarchyItem,
        InsertableContent,
        Rating,
        RatingCharacteristics,
        RatingCondition,
        RatingGroupAttributes,
        RatingGroup,
        RatingPhaseGroup,
        RatingPhase,
        RatingPlan,
        RatingGroupResolution,
        NumberField,
        NumberFieldDefinition,
        OverrideCharacteristics,
        PointsAdjustment,
        ProposalEvaluationRatingSettings,
        ProposalSettings,
        RewardRatingSettings,
        SubsidyAdjustment,
        SubsidyDensityRangeGroup,
        SubsidyDensityRange,
        TblDimensions,
        TextField,
        TextFieldDefinition,
        PointsManager,
        User,
        UsersActions,
        UsersAdministrationRightsGroup,
        UsersRights,
        AfterLast                   // A dummy value so that we can check ranges
    }

    /// <summary>
    /// ScoringRules data type
    /// Enumerates the different types of scoring rules for ratings
    /// </summary>
    public enum ScoringRules
    {
        Linear,
        SquareRoot,
        CubicRoot,
        FourthRoot,
        Square,
        Cubic,
        FourthPower,
        Quadratic,
        Logarithmic
    }

    /// <summary>
    /// PointsChangesReasons
    /// Enumerates the different reasons that a user's points may change.
    /// </summary>
    public enum PointsAdjustmentReason
    {
        ReasonUnknown,
        RatingsUpdate,
        PointsCashed,
        RewardForUserDatabaseChange,
        PenaltyForUnsuccessfulChange,
        AdministrativeChange
    };


    ///<summary>
    ///TblColumnInfo 
    /// stores basic information about  a table column.
    /// </summary>
    [TypeConverter(typeof(TblColumnInfoConverter))]
    public class TblColumnInfo
    {
        public TblColumnInfo() { }
        public TblColumnInfo(Guid tblColumnID, string tblColumnName, bool defaultSortOrderAsc, bool sortable)
        {
            TblColumnID = tblColumnID;
            TblColumnName = TblColumnName;
            DefaultSortOrderAsc = defaultSortOrderAsc;
            Sortable = sortable;
        }
        public Guid TblColumnID { get; set; }
        public string TblColumnName { get; set; }
        public bool DefaultSortOrderAsc { get; set; }
        public bool Sortable { get; set; }
    }

    ///<summary>
    ///TblColumnInfoConverter
    /// Allow serialization of TblColumnInfo to viewstate
    /// </summary>
    public class TblColumnInfoConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            String state = value as String;
            if (state == null)
                return base.ConvertFrom(context, culture, value);
            String[] parts = state.Split('#');
            return new TblColumnInfo(new Guid(parts[0]), parts[1], Convert.ToBoolean(parts[2]), Convert.ToBoolean(parts[3]));
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentException("destinationType");
            TblColumnInfo fdi = value as TblColumnInfo;
            if (fdi != null)
                return fdi.TblColumnID.ToString() + "#" + fdi.TblColumnName + "#" + Convert.ToInt32(fdi.DefaultSortOrderAsc).ToString() + "#" + fdi.Sortable.ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

    }

    // An interface for the fields user controls. Any user control implementing this
    // interface defines a method that returns a filter rule.
    public interface IFilterField
    {
        R8RDataAccess DataAccess { get; set; }
        FieldsBoxMode Mode { get; set; }
        Guid? TblRowID { get; set; }
        Guid FieldDefinitionOrTblColumnID { get; set; }
        FilterRule GetFilterRule();
        FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup);
        bool InputDataValidatesOK(ref string errorMessage);
    }



    /// <summary>
    /// RatingGroupTypes
    /// Enumerates the different types of rating groups.
    /// </summary>
    public enum RatingGroupTypes
    {
        probabilityMultipleOutcomesHiddenHierarchy,
        probabilitySingleOutcome,
        probabilityMultipleOutcomes,
        probabilityHierarchyTop,
        probabilityHierarchyBelow,
        singleNumber,
        singleDate,
        hierarchyNumbersTop,
        hierarchyNumbersBelow
    }

    public static class RatingGroupTypesList
    {
        public static int[] hierarchyRatingGroupTypes = { (int)RatingGroupTypes.hierarchyNumbersBelow, (int)RatingGroupTypes.probabilityHierarchyBelow, (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy, (int)RatingGroupTypes.hierarchyNumbersTop, (int)RatingGroupTypes.hierarchyNumbersBelow };
        public static int[] singleItem = { (int)RatingGroupTypes.probabilitySingleOutcome, (int)RatingGroupTypes.singleNumber, (int)RatingGroupTypes.singleDate };
        public static int[] singleItemNotDate = { (int)RatingGroupTypes.probabilitySingleOutcome, (int)RatingGroupTypes.singleNumber };
        public static int[] notSingleItem = { (int)RatingGroupTypes.hierarchyNumbersBelow, (int)RatingGroupTypes.probabilityHierarchyBelow, (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy, (int)RatingGroupTypes.hierarchyNumbersTop, (int)RatingGroupTypes.hierarchyNumbersBelow, (int) RatingGroupTypes.probabilityMultipleOutcomes };
        public static int[] lowerHierarchy = { (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy, (int)RatingGroupTypes.probabilityHierarchyBelow, (int)RatingGroupTypes.hierarchyNumbersBelow };
    }

    /// <summary>
    /// Locations where contents to be inserted
    /// Enumerates the different locations.
    /// </summary>
    public enum InsertableLocation
    {

        TopOfViewTblContent,
        HomePageForSite,
        HomePageForPointsManager,
        AdvertisingArea,
        CorporateInfo

    }

    /// <summary>
    /// RatingPhaseData
    /// </summary>
    public class RatingPhaseData
    {
        public decimal SubsidyLevel;
        public ScoringRules ScoringRule;
        public bool Timed;
        public bool BaseTimingOnSpecificTime;
        public DateTime? EndTime;
        public int? RunTime;
        public int HalfLifeForResolution;
        public bool RepeatIndefinitely;
        public int? RepeatNTimes;

        public RatingPhaseData(
            decimal subsidyLevel,
            ScoringRules scoringRule,
            bool timed,
            bool baseTimingOnSpecificTime,
            DateTime? endTime,
            int? runTime,
            int halfLifeForResolution,
            bool repeatIndefinitely,
            int? repeatNTimes
            )
        {
            if (baseTimingOnSpecificTime && endTime == null)
                throw new Exception("End time was not specified.");
            SubsidyLevel = subsidyLevel;
            ScoringRule = scoringRule;
            Timed = timed;
            BaseTimingOnSpecificTime = baseTimingOnSpecificTime;
            EndTime = endTime;
            RunTime = runTime;
            HalfLifeForResolution = halfLifeForResolution;
            RepeatIndefinitely = repeatIndefinitely;
            RepeatNTimes = repeatNTimes;
        }
    }

    public class SubsidyDensityRangeData
    {
        public decimal RangeBottom;
        public decimal RangeTop;
        public decimal LiquidityFactor;

        public SubsidyDensityRangeData(decimal rangeBottom, decimal rangeTop, decimal liquidityFactor)
        {
            if (rangeBottom >= rangeTop)
                throw new Exception("The bottom of a subsidy density range must be below the top.");
            if (liquidityFactor <= 0)
                throw new Exception("The liquidity factor must be a positive number.");
            if (rangeBottom < 0 || rangeBottom > 1 || rangeTop < 0 || rangeTop > 1)
                throw new Exception("Each range must be within 0 to 1. These values will be mapped onto the prediction spectrum.");
            RangeBottom = rangeBottom;
            RangeTop = rangeTop;
            LiquidityFactor = liquidityFactor;
        }
    }

    /// <summary>
    /// Contains action type constants
    /// </summary>
    public class ActionType
    {
        /// <summary>
        /// Indicates the action was a creation of an tblRow
        /// </summary>
        public static char CREATE = 'C';
        /// <summary>
        /// Indicates the action was a deletion
        /// </summary>
        public static char DELETE = 'D';
        /// <summary>
        /// Indicates the action was an undeletion of the entity
        /// </summary>
        public static char UNDELETE = 'U';
    }
}