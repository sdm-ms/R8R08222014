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
using MoreStrings;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{

    public class RaterooDataAccess
    {
        static DataContextManagement myDataContextManagement = null;

        public RaterooDataAccess()
        {
            myDataContextManagement = new DataContextManagement();
        }

        public IRaterooDataContext RaterooDB
        {
            get
            {
                return myDataContextManagement.GetDataContext(true, true);
            }
        }

        // Data access methods -- each of the following returns an object
        // from the database. The comments below discuss each field from those
        // objects.

        /// <summary>
        /// A field containing an address and a longitude and latitude indicator 
        /// AddressFieldID: The id of this object
        /// FieldID: The id of the field
        /// AddressString: The address, in text format, as reformatted by Google's geocoding service
        /// Latitude: The latitude (decimal)
        /// Longitude: The longitude (decimal)
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public AddressField GetAddressField(int theID)
        {

            return RaterooDB.GetTable<AddressField>().Single(x => x.AddressFieldID == theID);
        }

        public TblColumnFormatting GetTblColumnFormatting(int theID)
        {
            return RaterooDB.GetTable<TblColumnFormatting>().Single(x => x.TblColumnFormattingID == theID);
        }

        /// <summary>
        /// Returns a specified category descriptor object. For example, in a baseball prediction rating,
        /// a category group might be "Fielding statistics," and a category descriptor might be "Errors."
        /// TblColumnID: the id number
        /// TblTabID: The TblTab of which this is a member.
        /// DefaultRatingGroupAttributesID: The rating group attributes which, by default, should be
        /// assigned to a rating group for this category descriptor for a particular entity.
        /// CategoryNum: The order within the group (need not be unique).
        /// Abbreviation: An abbreviation that can be used within tables.
        /// Name: The name of the category descriptor.
        /// </summary>
        /// <param name="theID">The id of the TblColumn to be returned.</param>
        /// <returns>The specified TblColumn</returns>
        public TblColumn GetTblColumn(int theID)
        {
            return RaterooDB.GetTable<TblColumn>().Single(x => x.TblColumnID == theID);
        }

        /// <summary>
        /// Returns a TblTab object. For example, in an economic statistics prediction rating,
        /// category groups might be "Labor Statistics" and "Trade Statistics."
        /// TblTabID: The id number of the category group.
        /// TblID: The Tbl to which this TblTab belongs (e.g., an economic statistics Tbl)
        /// NumInTbl: The number (from 1 to the maximum) within the Tbl.
        /// Name: The name of the TblTab.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public TblTab GetTblTab(int theID)
        {
            return RaterooDB.GetTable<TblTab>().Single(x => x.TblTabID == theID);
        }

        /// <summary>
        /// Returns a changes group object. This object groups together potential changes to the database, and links them
        /// with a rating (if any) used to determine whether to allow the changes.
        /// ChangesGroupID: The id of this object
        /// PointsManagerID: The universe for which a change is proposed
        /// TblID: The Tbl for which a change is proposed, if the Tbl already exists, otherwise null
        /// Creator: The user who proposed the change
        /// MakeChangeRatingID: The rating (within a rating group) corresponding to whether the change should be approved, or null if none
        /// RewardRatingID: The rating (within a rating group) corresponding to what reward should be given
        /// StatusOfChanges: The status of the change. If the status changes, then the objects in the group will have their
        /// status changed too.
        /// ScheduleApprovalOrRejection: The time at which a change is on track for approval or rejection will be approved, if
        /// the threshold is not crossed.
        /// ScheduleImplementation: Once a change has been approved, it is scheduled for implementation at this time.
        /// </summary>
        /// <param name="theID">The id of the object</param>
        /// <returns>A changes group object (grouping together potential changes to the database)</returns>
        public ChangesGroup GetChangesGroup(int theID)
        {
            return RaterooDB.GetTable<ChangesGroup>().Single(x => x.ChangesGroupID == theID);
        }

        /// <summary>
        /// Returns an object specifying how a rating or group should be resolved if the change is approved
        /// ChangesResolveRatingOrGroupID: The id of this object
        /// ChangeGroupID: The id of the change group that this change is a part (may be the only one)
        /// RatingGroupID: The rating group to resolve, or null if we're only resolving a rating
        /// RatingID: The rating to resolve, or the winning rating if a rating group is being resolved, or null if a rating 
        /// group is being resolved in some other way.
        /// ResolveByUnwinding: 1 if unresolved predictions should end up with 0 points
        /// ResolveAtCurrentUserRating: 1 if unresolved predictions should be resolved at the current prediction
        /// ResolveByMakingRatingWinner: 1 if we should resolve the rating group by making this rating a winner (and other ratings losers).
        /// If choosing this option, both RatingGroupID and RatingGroup must be specified.
        /// UnresolveFromRound: Nonzero if instead of resolving predictions we should unresolve them beginning at this round
        /// ReresolveFromRound: Nonzero if we should resolve predictions that are already resolved beginning at this round
        /// ResolutionValueForRating: If resolving only a rating, the resolution value for this rating.
        /// </summary>
        /// <param name="theID">The id of the object</param>
        /// <returns>The changes resolve rating or group object</returns>
        //public ChangesResolveRatingOrGroup GetChangesResolveRatingOrGroup(int theID)
        //{
        //    return RaterooDB.GetTable<ChangesResolveRatingOrGroup>().Single(x => x.ChangesResolveRatingOrGroupID == theID);
        //}

        /// <summary>
        /// Returns an object that specifies some change to the database, for example the addition, deletion, or replacement
        /// of an object, or a change in name or other attribute of an object.
        /// ChangesStatusOfObjectID: The id of this object
        /// ChangesGroupID: The id of the changes group that this change is part of
        /// ChangeType: The type of change being affected (enumerated above). Other fields determine the nature of the change.
        /// AddObject: 1 if a new object is to be added
        /// DeleteObject: 1 if the existing object is to be deleted
        /// ReplaceObject: 1 if the existing object's fields should be changed to the new object's
        /// ChangeName: 1 if the name of the object is to be changed
        /// ChangeOther: 1 if some other change is to be made
        /// ChangeSetting1: This bit may be used for specific objects to convey info about how to do the change
        /// ChangeSetting2: This bit may be used for specific objects to convey info about how to do the change
        /// MayAffectRunningRating: This change should be delayed before implementation, because it may affect a 
        /// running rating (and we need to prevent manipulation)
        /// NewName: The new name of the object, if any
        /// NewObject: The id of the new object, if any
        /// ExistingObject: The id of the existing object, if any
        /// NewValueBoolean: A boolean value for some ChangeOther changes.
        /// NewValueInteger: An integer value for some ChangeOther changes.
        /// NewValueDecimal: A decimal value for some ChangeOther changes.
        /// NewValueText: A text value for some ChangeOther changes.
        /// NewValueDateTime: A datetime value for DateTime changes.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ChangesStatusOfObject GetChangesStatusOfObject(int theID)
        {
            return RaterooDB.GetTable<ChangesStatusOfObject>().Single(x => x.ChangesStatusOfObjectID == theID);
        }

        /// <summary>
        /// Returns an object representing a type of field that allows 
        /// the user to select one or more choice from a group of options.
        /// The choices are stored in ChoiceInFields objects, which point back to this ChoiceField.
        /// ChoiceFieldID: The id number of the choice field.
        /// FieldID: The id number of the corresponding Field object
        /// 
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ChoiceField GetChoiceField(int theID)
        {
            return RaterooDB.GetTable<ChoiceField>().Single(x => x.ChoiceFieldID == theID);
        }

        /// <summary>
        /// Returns an object representing a grouping of choies for a ChoiceField. The ChoiceInGroups objects
        /// point to these objects.
        /// ChoiceGroupID: The id of this choice group.
        /// AllowMultipleSelections: 1 if the user can pick more than 1, 0 if the user must pick only 1.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ChoiceGroup GetChoiceGroup(int theID)
        {
            return RaterooDB.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID == theID);
        }

        /// <summary>
        /// Returns an object that contains further information for a field descriptor that is a choice group,
        /// and that connects the field descriptor to the choice group.
        /// ChoiceGroupFieldDefinitionID: The id of this object.
        /// ChoiceGroupID: The id of the corresponding choice group.
        /// FieldDefinitionID: The id of the corresponding field descriptor.
        /// DependentOnChoiceGroupFieldDefinitionID: If not null, then the CGFDID of a field that this choice group is dependent on.
        /// A choice group is dependent on another when the values displayed depend on the value of the corresponding
        /// field. For example, one choice group might represent "Divisions" and include "NL East," "NL Central," etc.
        /// Another choice group might represent "Teams" but if the "NL East" division were selected, then only the
        /// teams in the "NL East" (e.g., the "New York Mets" but not the "San Diego Padres") would be displayed.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ChoiceGroupFieldDefinition GetChoiceGroupFieldDefinition(int theID)
        {
            return RaterooDB.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.ChoiceGroupFieldDefinitionID == theID);
        }

        /// <summary>
        /// Returns a ChoiceInField object, which indicates a choice made from a ChoiceGroup 
        /// for a particular field for a particular entity. For example, the ChoiceInField might 
        /// represent "New York Mets" if the ChoiceGroup consists of baseball teams and the particular
        /// baseball player being described in the fields is a New York Met. If multiple
        /// selections are allowed, then there can be more than one ChoiceInField object for a 
        /// ChoiceGroup.
        /// ChoiceInFieldID: The id of this object.
        /// ChoiceFieldID: The id of the corresponding choice field.
        /// ChoiceInGroupID: The id of the ChoiceInGroup object that is being selected (e.g., corresponding
        /// to the "New York Mets" choice in the teams choice group).
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ChoiceInField GetChoiceInField(int theID)
        {
            return RaterooDB.GetTable<ChoiceInField>().Single(x => x.ChoiceInFieldID == theID);
        }

        /// <summary>
        /// Returns an object representing a choice that may be selectable from a ChoiceGroup.
        /// For example, "New York Mets" might be chosen from a group of baseball teams.
        /// ChoiceInGroupID: The id of this object
        /// ChoiceGroupID: The id of the ChoiceGroup for which this is a choice (e.g., "Teams").
        /// ChoiceNum: The number of the choice (from 1 to the maximum)
        /// ChoiceText: A string representing this choice (e.g., "New York Mets").
        /// ActiveInGeneral: 1 if this is generally available as a choice, 0 if it has been retired (but still may exist for entities that already refer to it)
        /// ActiveOnDeterminingGroupChoiceInGroupID: If the corresponding ChoiceGroupFieldDefinition has DependentOnChoiceGroupFieldDefinitionID value set, 
        /// then this choice will be active only if the value of the choice group or number field specified in
        /// DependentOnChoiceGroupFieldDefinitionID is set to the value specified here.
        /// 
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ChoiceInGroup GetChoiceInGroup(int theID)
        {
            return RaterooDB.GetTable<ChoiceInGroup>().Single(x => x.ChoiceInGroupID == theID);
        }

        /// <summary>
        /// Returns a Tbl object. For example, "Pitchers" might be a Tbl in a "Baseball" prediction universe.
        /// TblID: The id of the Tbl
        /// PointsManagerID: The universe in which the Tbl is a part.
        /// DefaultRatingGroupAttributesID: The default attributes for any rating group for an entity in this Tbl. This will
        /// be copied by default to the category descriptors for the category groups in this Tbl, but can be overridden for a
        /// particular category descriptor.
        /// Name: The name of the object
        /// Creator: The user that created the object.
        /// TblTabWord: The word that describes a category group in this Tbl. For example, "Type of statistics" might be
        /// used for a baseball Tbl. Alternatives might be "Year" or "Type of statistics and year".
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public Tbl GetTbl(int theID)
        {
            return RaterooDB.GetTable<Tbl>().Single(x => x.TblID == theID);
        }
    /// <summary>
    /// Returns a comment object
    /// </summary>
    /// <param name="theID"></param>
    /// <returns></returns>
        public Comment GetComment(int theID)
        {
            return RaterooDB.GetTable<Comment>().Single(x => x.CommentsID == theID);

        }


        /// <summary>
        /// A field containing a date and/or time
        /// DateTimeID: The id of this object
        /// FieldID: The id of the field
        /// DateTime: The date and time (which is used depends on the DateTimeFieldDefinition)
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public DateTimeField GetDateTimeField(int theID)
        {
            return RaterooDB.GetTable<DateTimeField>().Single(x => x.DateTimeFieldID == theID);
        }

        /// <summary>
        /// Returns an object that contains further information for a field descriptor that is a date/time field.
        /// DateTimeFieldDefinitionID: The id of this object.
        /// FieldDefinitionID: The id of the corresponding field descriptor.
        /// IncludeDate: 1 if this field should include date information.
        /// IncludeTime: 1 if this field should include time information
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public DateTimeFieldDefinition GetDateTimeFieldDefinition(int theID)
        {
            return RaterooDB.GetTable<DateTimeFieldDefinition>().Single(x => x.DateTimeFieldDefinitionID == theID);
        }

        /// <summary>
        /// Returns a Domain object. The domain is the highest-level container for
        /// prediction ratings (containing universes, which in turn contain Tbls).
        /// For example, a sports domain might include baseball, basketball, and soccer universes.
        /// The Domain object includes settings indicating which web sites this domain
        /// and the universes within it should be displayed.
        /// DomainID: The id of this object
        /// ActiveUserRatingWebsite: 1 if this should be active in the website devoted to predictions.
        /// ActiveRatingWebsite: 1 if this should be active in the website devoted to ratings.
        /// ActiveBuyingWebsite: 1 if this should be active in the website devoted to evaluating purchases.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public Domain GetDomain(int theID)
        {
            return RaterooDB.GetTable<Domain>().Single(x => x.DomainID == theID);
        }

        /// <summary>
        /// Returns an TblRow object. For example, in a baseball players rating, the entity might be a
        /// baseball player (or, if the Tbl is of hitting statistics, a particular batter).
        /// TblRowID: Thd id number of the entity.
        /// TblID: The Tbl in which this entity is a member. (For example, the name of a 
        /// baseball player for whom various statistics are being predicted in a baseball prediction rating.)
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public TblRow GetTblRow(int theID)
        {
            return RaterooDB.GetTable<TblRow>().Single(x => x.TblRowID == theID);
        }

        /// <summary>
        /// Returns a Field object, which relates an entity to a piece of information about it.
        /// For example, this object might connect a baseball player "Hank Aaron" to a field descriptor 
        /// "Country of Origin." Note that this does not actually contain the information within the field.
        /// Rather, a TextField, NumberField, ChoiceField object will contain the actual information and refer back
        /// to this object. There will be one Field object created for each information field for each entity.
        /// </summary>
        /// FieldID: The id of this field object
        /// TblRowID: The id of the entity object
        /// FieldDefinitionID: The id of the field descriptor object.
        /// <param name="theID"></param>
        /// <returns></returns>
        public Field GetField(int theID)
        {
            return RaterooDB.GetTable<Field>().Single(x => x.FieldID == theID);
        }

        /// <summary>
        /// Returns a FieldDefinition object, which identifies a type of information to be kept about
        /// entities in a Tbl. 
        /// FieldDefinitionID: The id of the field descriptor object
        /// TblID: The id of the Tbl (e.g., "Hitting Statistics") whose entities (e.g., "Players") have fields being described.
        /// FieldNum: The number of the field (from 1 to the maximum, need not be unique or complete)
        /// FieldName: The name of the field (e.g., "Country of origin")
        /// FieldType: An enumerated type indicating what type of information this is (e.g., text, number, choice group, location, date, etc.)
        /// If the field type is a choice group, then there will be a ChoiceGroupFieldDefinition object with further information.
        /// UseAsFilter: If 1, then when this Tbl is being displayed, the user should be allowed to filter the data by selecting
        /// only entities who match a range of values within this field.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public FieldDefinition GetFieldDefinition(int theID)
        {
            return RaterooDB.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == theID);
        }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="theID"></param>
    /// <returns></returns>
        public InsertableContent GetInsertableContents(int theID)
        {
            return RaterooDB.GetTable<InsertableContent>().Single(x => x.InsertableContentID == theID);
        }
        /// <summary>
        /// return an InvitedUser object representing a Invited User detail
        /// ActivationNumber: a Unique number assigned to each invited user
        /// IsRegistered: 1 if a user has registred
        /// MayView: 1 if user may view the prediction ratings. If this is set to 0 for the default user,
        /// only specified users may view the ratings.
        /// MayPredict: 1 if user may enter predictions.
        /// MayAddTbls: 1 if user may define new Tbls to add.
        /// MayResolveRatings: 1 if user may resolve ratings (i.e., set the final values, or declare the winning outcome)
        /// MayChangeTblRows: 1 if user may add new entities, delete new entities, and change fields for entities
        /// MayChangeChoiceGroups: 1 if user may inactivate choices in choice groups and add new choices
        /// MayChangeCharacteristics: 1 if user may change rating grop attributes, rating phases, subsidy density groups,
        /// and other attributes of ratings. Note that if attributes are changed for running ratings, then the ratings
        /// must end and then be restarted.
        /// MayChangeCategories: 1 if user may delete or add categories.
        /// MayChangeUsersRights: 1 if user may change these rights for self or other users.
        /// MayAdjustPoints: 1 if user may adjust other users' points.
        /// MayChangeProposalSettings: 1 if user may change ProposalSettings values.
        /// </summary>
        /// <param name="activationNumber">Unique activation number of invited user</param>
        /// <returns></returns>
        public InvitedUser GetInvitedUser(int activationNumber)
        {
            return RaterooDB.GetTable<InvitedUser>().Single(x => x.ActivationNumber == activationNumber);
        }
        /// <summary>
        /// Returns an object representing a particular rating. A rating is always a member of a rating
        /// group, but there can be just one rating in a rating group. For example, a rating might predict
        /// the probability that an event will happen. In that case, the rating may be the only member
        /// of its group. Alternatively, several ratings might represent the probability that each of a 
        /// number of horses will win a race, and then all of those ratings will be in the rating group.
        /// A rating can "own" a rating group when the rating forecast itself can be viewed as the sum
        /// of a number of other rating forecasts. For example, a rating can predict the probability that
        /// a NL East team will win the World Series, and it can then own a rating group consisting of each
        /// of the individual NL East teams.
        /// RatingID: The id of this Rating object.
        /// RatingGroupID: The rating group to which this rating is a member.
        /// RatingCharacteristicsID: The characteristics of this rating (i.e., the rules determining
        /// how the rating will operate).
        /// RatingStatusID: The current status of the rating (e.g., the phase, the current prediction, etc.)
        /// OwnedRatingID: If not null, then a rating group that is below this rating in the hierarchy.
        /// NumInGroup: The number of this rating within the rating group that contains it 
        /// (from 1 to the total ratings in the containing group).
        /// Name: The name of the rating (e.g., "X wins").
        /// Creator: The UserID of the user who created the rating.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        
        public Rating GetRating(int theID)
        {
            return RaterooDB.GetTable<Rating>().Single(x => x.RatingID == theID);
        }

        /// <summary>
        /// Returns a RatingCharacteristic object, dictating the rules governing how the rating works.
        /// Each rating group has a RatingCharacteristics object, but this can be overridden for a 
        /// particular rating in the group.
        /// RatingCharacteristicsID: The id of this object.
        /// RatingPhaseGroupID: The rating phases for this rating (determining how this rating
        /// operates over time).
        /// PointsTrustRulesID: The counting rules for this rating (specifying whether users' predictions
        /// will affect the consensus predictions).
        /// SubsidyDensityRangeGroupID: The subsidy density range (affecting how many points users
        /// make or lose over different parts of the prediction spectrum).
        /// MinimumUserRating: The lowest allowable prediction for this rating.
        /// MaximumUserRating: The maximum allowable prediction for this rating.
        /// DecimalPlaces: The number of decimal places to display on the main page for this rating.
        /// Name: The name of this rating characteristics object. It's useful to have a name for
        /// user interface purposes, so that users can select from a group of rating characteristics.
        /// Creator: The name of the creator of the rating.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingCharacteristic GetRatingCharacteristic(int theID)
        {
            return RaterooDB.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID == theID);
        }

        /// <summary>
        /// Returns a RatingCondition object, specifying how a rating or planned rating should be
        /// conditional on another rating. When the condition rating's value is not within a specified
        /// range, the conditional rating's points resolutions will be at 0. This object refers to
        /// rating plans and ratings. The condition will only matter once both rating fields are non-null.
        /// RatingConditionID: The id of this object
        /// ConditionalRatingPlanID: The id of a rating plan for the conditional rating, or null
        /// ConditionRatingPlanID: The id of a rating plan for the condition rating, or null
        /// ConditionalRatingID: The id of the conditional rating, or null
        /// ConditionRatingID: The id of the condition rating or null
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingCondition GetRatingCondition(int theID)
        {
            return RaterooDB.GetTable<RatingCondition>().Single(x => x.RatingConditionID == theID);
        }

        /// <summary>
        /// Returns an object representing a group of ratings (corresponding to a particular entity
        /// and category). For example, in the baseball ratings, this might represent a single rating
        /// predicting batting average (the category descriptor) for a particular player (the entity).
        /// RatingGroupID: The id of this object.
        /// RatingGroupAttributesID: An object specifying attributes for this group.
        /// TblRowID: The entity corresponding to this rating group.
        /// TblColumnID: The category within which this rating group falls.
        /// Activated: Whether this rating group has been activated.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingGroup GetRatingGroup(int theID)
        {
            return RaterooDB.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theID);
        }

        /// <summary>
        /// Returns an object representing the attributes of a rating group. Note that while a rating
        /// group always contains ratings, a rating group can also be owned by a rating.
        /// RatingGroupAttributesID: The id of this object.
        /// RatingCharacteristicsID: The characteristics for any rating within this group.
        /// ConstrainedSum: If not null, then the ratings in this group must exceed the minimum
        /// prediction by a total of this amount. For example, in ratings predicting the probabilities
        /// of a group of events, this will generally be 100. Note that the constrained sum will be carried
        /// all the way down the rating hierarchy.
        /// Name: The name of the rating group
        /// Creator: The creator of the rating group
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingGroupAttribute GetRatingGroupAttributes(int theID)
        {
            return RaterooDB.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == theID);
        }

        /// <summary>
        /// Returns an object representing a particular phase of the rating.
        /// RatingPhaseID: The id of this object.
        /// RatingPhaseGroupID: The group of which this phase is a member (representing all planned
        /// phases of the rating).
        /// NumberInGroup: The number of this phase within the group of phases (1 to the maximum)
        /// SubsidyLevel: The level of the rating subsidy for this phase.
        /// ScoringRule: The type of scoring rule used for this rating (as enumerated above).
        /// Timed: 1 if this is a timed phase, 0 if it goes on indefinitely
        /// BaseTimingOnSpecificTime: 1 if the phase will end at a specific time (EndTime)
        /// EndTime: The earliest time at which this phase will end (if BaseTimingOnSpecificTime is 1)
        /// RunTime: The amount of time from the start of this phase before the earliest
        /// time at which this phase will end (if BaseTimingOnSpecificTime is 0)
        /// HalfLifeForResolution: A half life that determines the total amount of time between the
        /// earliest possible ending of the phase and the actual ending of the phase.
        /// RepeatIndefinitely: If 1, then the next phase will be based on this same object.
        /// RepeatNTimes: If not null, then this phase should be repeated this many times.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingPhase GetRatingPhase(int theID)
        {
            return RaterooDB.GetTable<RatingPhase>().Single(x => x.RatingPhaseID == theID);
        }

        /// <summary>
        /// Returns an object representing the group of rating phases for a rating.
        /// RatingPhaseGroupID: The id of this object
        /// NumPhases: The number of phases in the group
        /// Name: The name of this rating phase group
        /// Creator: The user who created this, if any.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingPhaseGroup GetRatingPhaseGroup(int theID)
        {
            return RaterooDB.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == theID);
        }

       
        
        /// <summary>
        /// Returns an object representing a plan to create a rating. 
        /// RatingPlansID: The id for this plan
        /// RatingGroupAttributesID: The attributes for the rating group containing this rating
        /// NumInGroup: The number of the rating within the containing rating group.
        /// OwnedRatingGroupAttributes: If not null, the attributes for a rating group that will be
        /// beneat this group in the hierarchy.
        /// DefaultUserRating: If not null, the default prediction at the start of the rating.
        /// Name: The name the rating will have
        /// Creator: The user who created the rating, if any
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public RatingPlan GetRatingPlan(int theID)
        {
            return RaterooDB.GetTable<RatingPlan>().Single(x => x.RatingPlansID == theID);
        }


        

        /// <summary>
        /// A field containing a decimal number (for example, the age of a person about whom predictions 
        /// are being made)
        /// NumberFieldID: The id of this object
        /// FieldID: The id of the field
        /// Number: The number
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public NumberField GetNumberField(int theID)
        {
            return RaterooDB.GetTable<NumberField>().Single(x => x.NumberFieldID == theID);
        }
        
        /// <summary>
        /// Returns an object that contains further information for a field descriptor that is a number.
        /// NumberFieldDefinitionID: The id of this object.
        /// FieldDefinitionID: The id of the corresponding field descriptor.
        /// Minimum: The minimum value of a number in this field
        /// Maximum: The maximum value of a number in this field
        /// DecimalPlaces: The number of decimal places to display
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public NumberFieldDefinition GetNumberFieldDefinition(int theID)
        {
            return RaterooDB.GetTable<NumberFieldDefinition>().Single(x => x.NumberFieldDefinitionID == theID);
        }

        /// <summary>
        /// Returns an object representing special rating group attributes to be used for a particular 
        /// entity and category, instead of the default rating group attributes that ordinarily would be
        /// used for that category.
        /// OverrideCharacteristicsID: The id of this object.
        /// RatingGroupAttributesID: The id of the rating group attributes to use
        /// TblRowID: The entity for which we want to use these characteristics
        /// TblColumnID: The category for which we wish to override the characteristics
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public OverrideCharacteristic GetOverrideCharacteristics(int theID)
        {
            return RaterooDB.GetTable<OverrideCharacteristic>().Single(x => x.OverrideCharacteristicsID == theID);
        }

        /// <summary>
        /// Returns an object representing an adjustment to the points that a user has in a particular
        /// prediction rating universe.
        /// PointsAdjustmentID: The id of this object
        /// UserID: The user affected
        /// PointsManagerID: The universe for which the user's points are affected (Users have different points
        /// totals in each universe)
        /// Reason: The reason for the change (as enumerated in PointsChangesReasons)
        /// TotalAdjustment: Adjustment to total points (i.e., points earned, even if cashed)
        /// CurrentAdjustment: Adjustment to currently available points
        /// CashValue: If not null, this is the amount of cash to be exchanged for points deducted
        /// WhenMade: The time this adjustment was made
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public PointsAdjustment GetPointsAdjustment(int theID)
        {
            return RaterooDB.GetTable<PointsAdjustment>().Single(x => x.PointsAdjustmentID == theID);
        }

        /// <summary>
        /// Returns an object representing the number of points that a user has in a particular
        /// prediction ratings universe.
        /// PointsTotalID: The id of this object
        /// UserID: The user affected
        /// PointsManagerID: The universe to which these points belong (Users have different points
        /// totals in each universe)
        /// CurrentPoints: The current number of points the user has
        /// TotalPoints: The total number of points that the user has earned (including points that
        /// the user has cashed).
        /// MaxLoss: The maximum loss that the user can receive based on current predictions
        /// PendingPoints: Points that are pending for the user, i.e. if all ratings ended immediately
        /// at their current values.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public PointsTotal GetPointsTotal(int theID)
        {
            return RaterooDB.GetTable<PointsTotal>().Single(x => x.PointsTotalID == theID);
        }

        /// <summary>
        /// Returns an object representing a prediction in a single rating. UserRatings are always
        /// stored in prediction groups (though a group could have just one rating)
        /// UserRatingID: The id of this object
        /// UserRatingGroupID: The group to which this prediction belongs
        /// RatingID: The rating for which this prediction is being made
        /// UserID: The user making the prediction
        /// RoundNum: The round number (see the rating status object) in which the prediction was made
        /// PreviousUserRating: The immediately preceding prediction
        /// EnteredUserRating: The prediction entered by the user
        /// NewUserRating: The consensus prediction of the rating after processing of the user's prediction.
        /// Note that this will be the same as EnteredUserRating if the user's predictions don't count
        /// because the user has not yet earned enough points.
        /// MaxLoss: The maximum loss that the user could suffer from this prediction
        /// MaxGain: The maximum gain that the user could receive
        /// Resolved: 1 if the prediction has been resolved (i.e., points have been determined based
        /// on a later prediction or the final value of the rating)
        /// CurrentUserRatingOrFinalValue: The current rating prediction (or, if the rating has ended,
        /// its final value). Note that this isn't updated after every prediction, but is updated as
        /// an idle task (see IdleTaskUpdateARating).
        /// ResolvedOrPendingPoints: The points that the user has or will receive based on the current
        /// prediction. Note that this isn't updated after every prediction, but is updated as
        /// an idle task (see IdleTaskUpdateARating).
        /// PendingMaxLoss: The max loss that the user could now suffer (0 if the rating is resolved
        /// and so no loss is pending)
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public UserRating GetUserRating(int theID)
        {
            return RaterooDB.GetTable<UserRating>().Single(x => x.UserRatingID == theID);
        }

        /// <summary>
        /// Returns an object representing a group of predictions. When a user makes a single
        /// prediction in a rating contained within a rating group, predictions may be automatically
        /// generated for multiple ratings within the group.
        /// UserRatingGroupID: The id of this object.
        /// RatingGroupID: The id of a rating group containing the predictions.
        /// MaxLoss: The maximum loss possible from all predictions in this group.
        /// MaxGain: The maximum gain possible from all predictions in this group.
        /// Resolved: 1 if this rating has been resolved.
        /// ResolvedOrPendingPoints: The number of points already resolved or, if the rating is not
        /// resolved, pending for this prediction group.
        /// WhenMade: The time at which the prediction group was created (and predictions were made)
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public UserRatingGroup GetUserRatingGroup(int theID)
        {
            return RaterooDB.GetTable<UserRatingGroup>().Single(x => x.UserRatingGroupID == theID);
        }

        /// <summary>
        /// Returns an object indicating whether users can make proposals to effect
        /// various kinds of administrative actions and specifying parameters for any
        /// ratings that determine whether such proposals are accepted.
        /// ProposalSettingsID: The id of this object.
        /// PointsManagerID: The id of the prediction ratings universe.
        /// TblID: The id of the Tbl. If null, then these are the default settings
        /// for the universe as a whole.
        /// UsersMayProposeAddingTbls: If 1, users can propose entire new Tbls (including associated rating group attributes)
        /// UsersMayProposeResolvingRatings: If 1, users, can propose to resolve ratings and rating groups.
        /// UsersMayProposeChangingTblRows: If 1, users may add, change, and delete entities.
        /// UsersMayProposeChangingChoiceGroups: If 1, users may inactivate choices in choice groups or add new choices
        /// UsersMayProposeChangingCharacteristics: If 1, users may change rating group attributes, characteristics, subisdy
        /// density ranges, etc. Note that these changes would be effected by terminating and restarting all ratings.
        /// UsersMayProposeChangingCategories: If 1, users may propose changing category groups and descriptors.
        /// UsersMayPropeChangingUsersRights: If 1, users may propose changing rights of particular or default users
        /// UsersMayProposeAdjustingPoints: If 1, users may propose adjusting particular users' points.
        /// UsersMayProposeChangingProposalSettings: If 1, users may propose changing settings such as these.
        /// MinValueToApprove: The minimum value that must be met before proposed changes can be approved (e.g., 90)
        /// MaxValueToReject: The maximum value that can be met before proposed changes can be rejected.
        /// MinTimePastThreshold: For a change to be accepted or rejected, the threshold must be exceeded for at 
        /// least this many seconds 
        /// MinProportionOfThisTime: The threshold will be considered exceeded over a span of time if it is exceeded
        /// for at least this proportion of the time.
        /// MinAdditionalTimeForRewardRating: The reward rating will run at least this much time past the approval rating.
        /// HalfLifeForRewardRating: The reward rating's running time will include an added amount based on this half life to prevent manipulation.
        /// MaxBonusForProposal: The maximum points a user whose proposal is accepted may receive (depending on the reward rating). 
        /// MaxPenaltyForRejection: The maximum points a user whose proposal is rejected loses (depending on the reward rating). A user must have at least
        /// this many points to make a proposal.
        /// SubsidyForApprovalRating: The subsidy level for the rating determining whether a proposal 
        /// is accepted.
        /// SubsidyForRewardRating: The subsidy level for the rating determining the reward or penalty for the user who made the proposal.
        /// HalfLifeForResolvingAtFinalValue: If changes are made to rating characteristics, or if proposals
        /// are made to resolve ratings at their final value (rather than at a particular value), then this
        /// specifies a half life before the changes will go into effect (to prevent last minute user manipulation).
        /// RequiredPointsToMakeProposal: The required number of points a user must have to make a proposal. This should be quite low
        /// at first, but probably increased later to prevent users from manipulating the rating by using some accounts
        /// to make proposals and some accounts to reject the proposals.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public ProposalSetting GetProposalSettings(int theID)
        {
            return RaterooDB.GetTable<ProposalSetting>().Single(x => x.ProposalSettingsID == theID);
        }

        /// <summary>
        /// Returns an object representing a subsidy adjustment. This object indicates a factor
        /// by which the subsidy for whatever rating phase is pending should be multiplied by
        /// for a given time period. The last subsidy adjustment for any given point in time
        /// controls if there are multiple subsidy adjustments. This is useful when adding ratings
        /// to running rating groups (typically providing a lower subsidy temporarily, and zero
        /// subsidy for the rating that was split off (e.g., when "Field" is split into "Smith" and "Field."
        /// SubsidyAdjustmentID: The id of this object
        /// RatingGroupPhaseStatusID: The rating phase status object that this object effectively modifies
        /// SubsidyAdjustmentFactor: The number by which to multiply the subsidy
        /// EffectiveTime: The time beginning at which this factor should be applied
        /// EndingTime: The last time at which this factor should be applied, or null to apply it indefinitely
        /// </summary>
        /// <param name="theID">The id of this object</param>
        /// <returns>A subsidy adjustment objrct</returns>
        public SubsidyAdjustment GetSubsidyAdjustment(int theID)
        {
            return RaterooDB.GetTable<SubsidyAdjustment>().Single(x => x.SubsidyAdjustmentID == theID);
        }

        /// <summary>
        /// Returns an object representing a subsidy density range. For example, a subsidy 
        /// density range might indicate that for predictions between 0.1 and 0.3, there should 
        /// be twice the normal subsidy. 
        /// SubsidyDensityRangeID: The id of this object
        /// SubsidyDensityRangeGroupID: The id of the group containing it
        /// RangeBottom: The bottom of the range (must be between 0 and 1 and < RangeTop)
        /// RangeTop: The top of the range (must be between 0 and 1 and > RangeBottom)
        /// LiquidityFactor: The relative amount of subsidy for this range. Higher numbers
        /// mean more subsidy. 
        /// CumDensityBottom: The cumulative density of the group up to the bottom of this range.
        /// For example, if RangeBottom is 0.1, and another subsidy density range spans 0 to 0.1
        /// and has liquidity factor 2, then this would be 0.2.
        /// CumDensityTop: The cumulative density of the group up to the top of this range.
        /// This always equals CumDensityBottom + (RangeTop - RangeBottom) * LiquidityFactor
        /// 
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public SubsidyDensityRange GetSubsidyDensityRange(int theID)
        {
            return RaterooDB.GetTable<SubsidyDensityRange>().Single(x => x.SubsidyDensityRangeID == theID);
        }

        /// <summary>
        /// SubsidyDensityRangeGroup
        /// SubsidyDensityRangeGroupID: The id of this object
        /// CumDensityTotal: The total cumulative density of the group (i.e., CumDensityTop
        /// for the range with RangeTop equal to 1.0). The routines used to calculate points
        /// normalize for this, so doubling the liquidity factor of all ranges will have no
        /// effect on points.
        /// Name: The name of this subsidy density range group.
        /// Creator: The user who created this subsidy density range group, or null.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public SubsidyDensityRangeGroup GetSubsidyDensityRangeGroup(int theID)
        {
            return RaterooDB.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID == theID);
        }

        /// <summary>
        /// A field containing a string. For example, in a baseball prediction universe, this
        /// might be "Place of birth"
        /// TextFieldID: The id of this object
        /// FieldID: The id of the field
        /// Text: The string
        /// Link: An optional link that the user can click on
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public TextField GetTextField(int theID)
        {
            return RaterooDB.GetTable<TextField>().Single(x => x.TextFieldID == theID);
        }

        public TextFieldDefinition GetTextFieldDefinition(int theID)
        {
            return RaterooDB.GetTable<TextFieldDefinition>().Single(x => x.TextFieldDefinitionID == theID);
        }

        /// <summary>
        /// Returns an object representing a prediction rating universe. Each universe has its
        /// own point scoring, and can consist of one or more Tbls. For example, a baseball
        /// universe might have a hitting statistics Tbl, a pitching statistics Tbl,
        /// a game outcome Tbl, a World Series Tbl, etc.
        /// PointsManagerID: The id for this object
        /// DefaultRatingGroupAttributesID: The default attributes for a rating group in this universe.
        /// PointsTrustRulesID: The counting rules for the universe
        /// SpecializedSiteNum: If this universe is used in a specialized stand-alone web site (for example,
        /// a web site devoted solely to baseball forecasting, or a universe for a particular company's prediction
        /// ratings), then a number indicating the web site number.
        /// CurrentPeriodDollarSubsidy: How many dollars will be distributed in this period
        /// EndOfDollarSubsidyPeriod: When does the current period end? (or null, if there is no dollar subsidy)
        /// NextPeriodDollarSubsidy: The subsidy for the next period (and, if unchanged, each after that), or null if none
        /// NextPeriodLength: The length in seconds of the next dollar subsidy period.
        /// MinimumPayment: The minimum amount of cash one must earn to receive anything
        /// NumPrizes: If this is nonzero, then the dollar subsidy will be split among this many prizes. In
        /// this case, MinimumPayment will be interpreted as the minimum number of points needed to have a
        /// chance of qualifying. Points in effect buy lottery tickets.
        /// TotalUserPoints: The total number of user points in this universe.
        /// CurrentUserPoints: The current number of user points in this universe (total - cashed in points)
        /// NumUsersMeetingUltimateStandard: The number of users who meet the UltimatePointsToCount standard.
        /// NumUsersMeetingCurrentStandard: The number of users who meet the current standard (see below)
        /// CurrentPointsToCount: The number of points that is currently needed for a user's predictions to count.
        /// Name: The name of this universe (e.g., "Baseball")
        /// Creator: The user who created this universe
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public PointsManager GetPointsManager(int theID)
        {
            return RaterooDB.GetTable<PointsManager>().Single(x => x.PointsManagerID == theID);
        }

        /// <summary>
        /// Returns an object representing a user. This object (or another one) needs to 
        /// be augmented to include information about the user (username, password, address,
        /// email address, etc.)
        /// UserID: The id of the user.
        /// Username: The username of the user.
        /// SuperUser: 1 if user is superuser else 0.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public User GetUser(int theID)
        {
            User theUser = RaterooDB.GetTable<User>().SingleOrDefault(x => x.UserID == theID);
            if (theUser == null)
                throw new Exception("Your user account could not be found in the database. Please try logging in again.");
            return theUser;
        }

        /// <summary>
        /// Returns an object representing either the rights of a particular user or of
        /// a default user.
        /// UserRightsID: The id of this object
        /// UserID: The id of the user, or null if these are the default rights.
        /// PointsManagerID: The id of the prediction ratings universe affected. Each universe has its own user rights.
        /// MayView: 1 if user may view the prediction ratings. If this is set to 0 for the default user,
        /// only specified users may view the ratings.
        /// MayPredict: 1 if user may enter predictions.
        /// MayAddTbls: 1 if user may define new Tbls to add.
        /// MayResolveRatings: 1 if user may resolve ratings (i.e., set the final values, or declare the winning outcome)
        /// MayChangeTblRows: 1 if user may add new entities, delete new entities, and change fields for entities
        /// MayChangeChoiceGroups: 1 if user may inactivate choices in choice groups and add new choices
        /// MayChangeCharacteristics: 1 if user may change rating grop attributes, rating phases, subsidy density groups,
        /// and other attributes of ratings. Note that if attributes are changed for running ratings, then the ratings
        /// must end and then be restarted.
        /// MayChangeCategories: 1 if user may delete or add categories.
        /// MayChangeUsersRights: 1 if user may change these rights for self or other users.
        /// MayAdjustPoints: 1 if user may adjust other users' points.
        /// MayChangeProposalSettings: 1 if user may change ProposalSettings values.
        /// </summary>
        /// <param name="theID"></param>
        /// <returns></returns>
        public UsersRight GetUsersRights(int theID)
        {
            return RaterooDB.GetTable<UsersRight>().Single(x => x.UsersRightsID == theID);
        }


       
        
        
    /// <summary>
   /// Returns an object representing a UserInfo.
   /// UserId: Id of user.
   /// FirstName: First name of user.
   /// LastName: Last name of the user.
   /// Emal: email id of user.
   /// Address1: Address of the user.
   /// Address2: Address of the user.
   /// City: City name of the user.
   /// State: State name of the user.
   /// Country: Country name of the user.
   /// HomePhone: Home phone number of the user.
   /// MobilePhone: Mobile number of the user.
   /// WorkPhone: Work phone number of the user.
   /// IsVerified:1 if user ids verified else 0.
   /// Password: Password of the user.
   /// 
    /// </summary>
    /// <param name="theID"></param>
    /// <returns></returns>
       

        public UserInfo GetUserInfo(int theID)
        {

            return RaterooDB.GetTable<UserInfo>().Single(x => x.UserID == theID);

            
        }
        public AdministrationRightsGroup GetAdministrationRightsGroup(int theID)
        {
            return RaterooDB.GetTable<AdministrationRightsGroup>().Single(x => x.AdministrationRightsGroupID == theID);
        }

        public AdministrationRight GetAdministrationRight(int theID)
        {
            return RaterooDB.GetTable<AdministrationRight>().Single(x => x.AdministrationRightID == theID);
        }


        public UsersAdministrationRightsGroup GetUsersAdministrationRightsGroup(int theID)
        {
            return RaterooDB.GetTable<UsersAdministrationRightsGroup>().Single(x => x.UsersAdministrationRightsGroupID == theID);
        }


        public ProposalEvaluationRatingSetting GetProposalEvaluationRatingSetting(int theID)
        {
            return RaterooDB.GetTable<ProposalEvaluationRatingSetting>().Single(x => x.ProposalEvaluationRatingSettingsID == theID);
        }


        public RewardRatingSetting GetRewardRatingSetting(int theID)
        {
            return RaterooDB.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == theID);
        }

        public UserAction GetUserAction(int theID)
        {
            return RaterooDB.GetTable<UserAction>().Single(x => x.UserActionID == theID);
        }


        /// <summary>
        /// Returns the choices in a choice group. For example, if the choice group consists of MLB teams, it will return a data
        /// structure complete with the team names and the corresponding database table entries.
        /// </summary>
        /// <param name="choiceGroupID">The id of the choice group</param>
        /// <param name="availableChoicesOnly">True, if only available choices are to be returned. Use true if displaying choices for viewing ratings,
        /// and false if displaying to allow inspection or or changes to the choice group specifically</param>
        /// <param name="determiningGroupValue">If non-null only the choices with the ActiveOnDeterminingGroupChoiceInGroupID equal to this number
        /// will be returned.</param>
        /// <returns></returns>
        public ChoiceGroupData GetChoiceGroupData(int choiceGroupID, bool availableChoicesOnly, int? determiningGroupValue)
        {
            ChoiceGroupData theData = new ChoiceGroupData();
            var theChoicesInGroup = RaterooDB.GetTable<ChoiceInGroup>()
                .Where(cig => cig.ChoiceGroupID == choiceGroupID
                    && ((availableChoicesOnly && (StatusOfObject)cig.Status == StatusOfObject.Active)
                        || (!availableChoicesOnly && (StatusOfObject)cig.Status != StatusOfObject.Proposed))
                    && (determiningGroupValue == null || cig.ActiveOnDeterminingGroupChoiceInGroupID == determiningGroupValue))
                .OrderBy(cig => cig.ChoiceNum);
            foreach (ChoiceInGroup theChoice in theChoicesInGroup)
                theData.AddChoiceToGroup(theChoice.ChoiceText, theChoice.ActiveOnDeterminingGroupChoiceInGroupID, (StatusOfObject)theChoice.Status == StatusOfObject.Active, theChoice.ChoiceInGroupID);
            return theData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserLoginResult CheckValidUser(string username, string password)
        {
            UserInfo theUserInfo = RaterooDB.GetTable<UserInfo>().SingleOrDefault(user => user.User.Username == username);

            if (theUserInfo == null)
                return UserLoginResult.UsernameUnknown;
            if (theUserInfo.Password != password)
                return UserLoginResult.PasswordInvalid;
            if (theUserInfo.IsVerified == false)
                return UserLoginResult.AccountNotVerified;
            return UserLoginResult.Valid;
        }

        internal UsersRight GetUsersRightsUsingCache(int userID, int pointsManagerID)
        {
            string cacheKey = "UsersRightForPointsManager" + userID + "," + pointsManagerID;
            UsersRight theUserRight = PMCacheManagement.GetItemFromCache(cacheKey) as UsersRight;
            if (theUserRight == null)
            {
                theUserRight = RaterooDB.GetTable<UsersRight>().SingleOrDefault(ur => ur.UserID == userID && ur.PointsManagerID == pointsManagerID && ur.Status == (Byte)StatusOfObject.Active);
                if (theUserRight == null)
                    theUserRight = RaterooDB.GetTable<UsersRight>().SingleOrDefault(ur => ur.User == null && ur.PointsManagerID == pointsManagerID && ur.Status == (Byte)StatusOfObject.Active);
                PMCacheManagement.AddItemToCache(cacheKey, new string[] { }, theUserRight, new TimeSpan(0, 10, 0));
            }
            return theUserRight;
        }

        public UserRatingResult ConfirmUserRatingRightsForTableCell(int? userID, RatingGroup topRatingGroup)
        {
            if (userID == null)
                return new UserRatingResult("Please login before entering a rating.");

            if (topRatingGroup == null)
                return new UserRatingResult("The specified table cell was not found in the database.");
            int pointsManagerID = topRatingGroup.TblRow.Tbl.PointsManagerID;
            UsersRight theUserRight = GetUsersRightsUsingCache((int) userID, pointsManagerID);
            if (theUserRight == null)
                return new UserRatingResult("The database could not be read to check your privileges.");
            if (theUserRight.MayPredict == false)
                return new UserRatingResult("You do not have privileges to enter this rating.");
            //if (!UserCounts(pointsManagerID, (int) userID))
            //{ // See if user has already made predictions here.
            //    bool priorUserRatingExists = RaterooDB.GetTable<UserRating>().Any(p => p.UserID == userID && p.Rating.TopmostRatingGroupID == topRatingGroup.RatingGroupID && p.NewUserRating != p.EnteredUserRating);
            //    if (priorUserRatingExists)
            //        return new UserRatingResult("You cannot make another rating on this table cell until Rateroo trusts your changes on this table or until the points for that rating have been resolved.");
            //}
            return new UserRatingResult();
        }

        public bool UserIsTrustedAtLeastSomewhatToEnterRatings(int pointsManagerID, int userID)
        {
            string cacheKey = "UserIsTrustedSomewhat" + userID + "," + pointsManagerID;
            bool? userIsTrustedAtLeastSomewhat = PMCacheManagement.GetItemFromCache(cacheKey) as bool?;
            if (userIsTrustedAtLeastSomewhat == null)
            {
                PointsManager thePointsManager = GetPointsManager(pointsManagerID);
                TrustTrackerUnit trustTrackerUnit = thePointsManager.TrustTrackerUnit;
                if (trustTrackerUnit == null || trustTrackerUnit.TrustTrackers == null)
                    return false;
                TrustTracker theTrustTracker = trustTrackerUnit.TrustTrackers.SingleOrDefault(x => x.UserID == userID);
                userIsTrustedAtLeastSomewhat = theTrustTracker != null; // We shouldn't need to check OverallTrustLevel; it is possible we won't move the rating, but a new userrating can still be entered && theTrustTracker.OverallTrustLevel > 0;
                PMCacheManagement.AddItemToCache(cacheKey, new string[] { }, userIsTrustedAtLeastSomewhat, new TimeSpan(0, 10, 0));
            }
            return (bool)userIsTrustedAtLeastSomewhat;
        }

        public bool UserIsTrustedToMakeDatabaseChanges(int pointsManagerID, int userID)
        {
            string cacheKey = "UserCounts" + userID + "," + pointsManagerID;
            bool? userCounts = PMCacheManagement.GetItemFromCache(cacheKey) as bool?;
            if (userCounts == null)
            {
                User theUser = GetUser(userID);
                PointsManager thePointsManager = GetPointsManager(pointsManagerID);
                userCounts = UserIsTrustedToMakeDatabaseChanges(thePointsManager, theUser);
                PMCacheManagement.AddItemToCache(cacheKey, new string[] { }, userCounts, new TimeSpan(0, 10, 0));
            }
            return (bool)userCounts;
        }

        public bool UserIsTrustedToMakeDatabaseChanges(PointsManager thePointsManager, User theUser, PointsTotal thePointsTotal = null)
        {
            if (theUser.SuperUser)
                return true;
            if (theUser.Username == "badrater")
                return false; // useful for testing purposes -- password is "badrater"
            if (thePointsTotal == null)
                thePointsTotal = theUser.PointsTotals.SingleOrDefault(pt => pt.PointsManager == thePointsManager && pt.User == theUser); 
            if (thePointsTotal == null)
                thePointsTotal = new PointsTotal(); // all points are zero

            bool userIsTrusted = true; // we are now making it so that all users are trusted to make database changes, but some might automatically be rolled back (theRules.TrustPointsRatioTotalsMinForAutoTrust != null && theUser.TrustPointsRatioTotals >= theRules.TrustPointsRatioTotalsMinForAutoTrust) || thePointsTotal.TrustPointsRatio >= 1 || (thePointsTotal.TrustPointsRatio * Math.Max(thePointsManager.CurrentPointsToCount, (decimal)0.0001) / Math.Max(theRules.UltimatePointsToCount, (decimal)0.0001) >= 1) || (thePointsTotal.TrustPoints >= thePointsManager.CurrentPointsToCount);
            return userIsTrusted;
        }


        public static bool AllowNullOrUserID0UserForTestingAndInitialBuild = false; 

        /// <summary>
        /// Check the user right
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="theAction"></param>
        /// <param name="proposalOnly"></param>
        /// <param name="pointsManagerID"></param>
        /// <param name="TblID"></param>
        /// <returns></returns>
        public bool CheckUserRights(int? userID, UserActionOldList theAction, bool proposalOnly, int? pointsManagerID, int? TblID)
        {
            if (AllowNullOrUserID0UserForTestingAndInitialBuild && (userID == null || userID == 0))
                return true;

            bool returnVal = false;

            if (pointsManagerID == null && TblID != null)
            {
                string key = "Tbl" + TblID;
                Tbl theTbl = RaterooDB.TempCacheGet(key) as Tbl;
                if (theTbl == null)
                {
                    theTbl = RaterooDB.GetTable<Tbl>().Single(c => c.TblID == TblID);
                    RaterooDB.TempCacheAdd(key, theTbl);
                }
                pointsManagerID = theTbl.PointsManagerID;
            }
            if (userID == null || userID == 0)
            { // Anonymous user
                if (theAction == UserActionOldList.View)
                {
                    UsersRight theRights = RaterooDB.GetTable<UsersRight>().SingleOrDefault(ur => ur.PointsManagerID == pointsManagerID && ur.Status == (Byte) StatusOfObject.Active && ur.User == null);
                    if (theRights != null)
                        returnVal = theRights.MayView;
                }
            }
            else if (GetUser((int)userID).SuperUser)
                returnVal = true;
            else
            {
                // See if there are user rights for this particular user; otherwise, get generic user rights for universe.
                UsersRight theRights = RaterooDB.GetTable<UsersRight>().SingleOrDefault(ur => ur.UserID == userID && ur.PointsManagerID == pointsManagerID && ur.Status == (Byte)StatusOfObject.Active);
                if (theRights == null)
                    theRights = RaterooDB.GetTable<UsersRight>().SingleOrDefault(ur => ur.UserID == null && ur.PointsManagerID == pointsManagerID && ur.Status == (byte)StatusOfObject.Active);

                if (theRights == null)
                    throw new Exception("Internal error -- user rights are not specified for universe " + pointsManagerID);
                // See if the user has the relevant right. Note that this would also entail the right to make a proposal.
                switch (theAction)
                {
                    case UserActionOldList.View:
                        returnVal = theRights.MayView;
                        break;
                    case UserActionOldList.Predict:
                        returnVal = theRights.MayPredict;
                        break;
                    case UserActionOldList.AddTblsAndChangePointsManagers:
                        returnVal = theRights.MayAddTbls;
                        break;
                    case UserActionOldList.ResolveRatings:
                        returnVal = theRights.MayResolveRatings;
                        break;
                    case UserActionOldList.ChangeTblRows:
                        returnVal = theRights.MayChangeTblRows;
                        break;
                    case UserActionOldList.ChangeChoiceGroups:
                        returnVal = theRights.MayChangeChoiceGroups;
                        break;
                    case UserActionOldList.ChangeCharacteristics:
                        returnVal = theRights.MayChangeCharacteristics;
                        break;
                    case UserActionOldList.ChangeCategories:
                        returnVal = theRights.MayChangeCategories;
                        break;
                    case UserActionOldList.ChangeUsersRights:
                        returnVal = theRights.MayChangeUsersRights;
                        break;
                    case UserActionOldList.AdjustPoints:
                        returnVal = theRights.MayAdjustPoints;
                        break;
                    case UserActionOldList.ChangeProposalSettings:
                        returnVal = theRights.MayChangeProposalSettings;
                        break;
                    case UserActionOldList.Other:
                        returnVal = false;
                        break;
                }
                if (returnVal == false && proposalOnly == true)
                {
                    if (TblID == null && pointsManagerID == null)
                        return false;
                    ProposalSetting theProposalSettings = null;
                    if (TblID != null)
                        theProposalSettings = RaterooDB.GetTable<ProposalSetting>().SingleOrDefault(ps => ps.TblID == TblID);
                    if (theProposalSettings == null)
                        theProposalSettings = RaterooDB.GetTable<ProposalSetting>().SingleOrDefault(ps => ps.PointsManagerID == pointsManagerID);
                    if (theProposalSettings == null)
                        throw new Exception("Internal error -- rights to create proposals not specified in database.");
                    switch (theAction)
                    {
                        case UserActionOldList.View:
                            returnVal = false; // can't propose views
                            break;
                        case UserActionOldList.Predict:
                            returnVal = false; // can't propose predictions
                            break;
                        case UserActionOldList.AddTblsAndChangePointsManagers:
                            returnVal = theProposalSettings.UsersMayProposeAddingTbls;
                            break;
                        case UserActionOldList.ResolveRatings:
                            returnVal = theProposalSettings.UsersMayProposeResolvingRatings;
                            break;
                        case UserActionOldList.ChangeTblRows:
                            returnVal = theProposalSettings.UsersMayProposeChangingTblRows;
                            break;
                        case UserActionOldList.ChangeChoiceGroups:
                            returnVal = theProposalSettings.UsersMayProposeChangingChoiceGroups;
                            break;
                        case UserActionOldList.ChangeCharacteristics:
                            returnVal = theProposalSettings.UsersMayProposeChangingCharacteristics;
                            break;
                        case UserActionOldList.ChangeCategories:
                            returnVal = theProposalSettings.UsersMayProposeChangingCategories;
                            break;
                        case UserActionOldList.ChangeUsersRights:
                            returnVal = theProposalSettings.UsersMayProposeChangingUsersRights;
                            break;
                        case UserActionOldList.AdjustPoints:
                            returnVal = theProposalSettings.UsersMayProposeAdjustingPoints;
                            break;
                        case UserActionOldList.ChangeProposalSettings:
                            returnVal = theProposalSettings.UsersMayProposeChangingProposalSettings;
                            break;
                        case UserActionOldList.Other:
                            returnVal = false;
                            break;
                    } // switch on actions

                } // checking if there are proposal rights
            } // we're not dealing with an anonymous user or a superuser

            return returnVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pointsManagerID"></param>
        /// <returns></returns>
        public bool PointsManagerAdministrationLinkVisible(int userID, int pointsManagerID)
        {
            bool retval = false;

            retval = CheckUserRights(userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, null);
            if (retval == true)
            {
                return retval;
            }
            retval = CheckUserRights(userID, UserActionOldList.AdjustPoints, false, pointsManagerID, null);
            if (retval == true)
            {
                return retval;
            }
            retval = CheckUserRights(userID, UserActionOldList.ChangeProposalSettings, false, pointsManagerID, null);
            if (retval == true)
            {
                return retval;
            }
            retval = CheckUserRights(userID, UserActionOldList.ChangeUsersRights, false, pointsManagerID, null);
            if (retval == true)
            {
                return retval;
            }
            return retval;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="TblID"></param>
        /// <returns></returns>
        public bool TblAdministrationLinkVisible(int userID, int TblID)
        {
            bool retval = false;
            Tbl theTbl = RaterooDB.GetTable<Tbl>().Single(c => c.TblID == TblID);
            if (theTbl.Name == "Changes")
                return false;
            int pointsManagerID = theTbl.PointsManagerID;

            retval = CheckUserRights(userID, UserActionOldList.ChangeCategories, false, pointsManagerID, TblID);
            if (retval == true)
            {
                return retval;
            }
            retval = CheckUserRights(userID, UserActionOldList.ChangeChoiceGroups, false, pointsManagerID, TblID);
            if (retval == true)
            {
                return retval;
            }
            //retval = CheckUserRights(userID, UserActionOldList.ChangeTblRows, false, pointsManagerID, TblID);
            //if (retval == true)
            //{
            //    return retval;
            //}
            return retval;
        }

        /// <summary>
        /// Returns the rating hierarchy data based on the current status of the marktes for the entity and the category.
        /// </summary>
        /// <param name="theTblRowID"></param>
        /// <param name="theTblColumnID"></param>
        /// <returns></returns>
        public RatingHierarchyData GetRatingHierarchyDataForTblRowCategory(int theTblRowID, int theTblColumnID, ref int? ratingGroupID)
        {
            RatingHierarchyData theData = new RatingHierarchyData();
            ratingGroupID = GetRatingGroupForTblRowCategory(theTblRowID, theTblColumnID);
            if (ratingGroupID != null)
                AddRatingGroupToRatingHierarchyData((int)ratingGroupID, ref theData, 1);
            return theData;
        }

        public RatingHierarchyData GetRatingHierarchyDataForRatingGroup(int ratingGroupID)
        {
            RatingHierarchyData theData = new RatingHierarchyData();
            AddRatingGroupToRatingHierarchyData((int)ratingGroupID, ref theData, 1);
            return theData;
        }

        /// <summary>
        /// Returns the rating group attributes id for the entity for the specified category descriptor.
        /// </summary>
        /// <param name="theTblRowID">The entity</param>
        /// <param name="theTblColumnID">The category descriptor</param>
        /// <returns></returns>
        public int GetRatingGroupAttributesForTblRowAndColumn(int theTblRowID, int theTblColumnID)
        {
            throw new NotImplementedException(); // we have disabled this functionality for now.
            //TblColumn tblColumn = RaterooDB.GetTable<TblColumn>().Single(cd => cd.TblColumnID == theTblColumnID); // TODO: Check Cache
            //var overrideCharacteristics = RaterooDB.GetTable<OverrideCharacteristic>().SingleOrDefault(oc => oc.TblRowID == theTblRowID && oc.TblColumnID == theTblColumnID && oc.Status == (Byte)StatusOfObject.Active);
            //if (overrideCharacteristics == null)
            //    return tblColumn.DefaultRatingGroupAttributesID;
            //else
            //    return overrideCharacteristics.RatingGroupAttributesID;
        }
        /// <summary>
        /// Returns the topmost rating group for an entity for the specified category descriptor.
        /// </summary>
        /// <param name="theTblRowID"></param>
        /// <param name="theTblColumnID"></param>
        /// <returns></returns>
        public int? GetRatingGroupForTblRowCategory(int theTblRowID, int theTblColumnID)
        {
            var theRatingGroups = RaterooDB.GetTable<RatingGroup>().Where(mg => (mg.Status == (Byte)StatusOfObject.Active || mg.Status == (Byte)StatusOfObject.DerivativelyUnavailable) &&
                                            mg.TblRowID == theTblRowID && mg.TblColumnID == theTblColumnID
                                            && (mg.TypeOfRatingGroup == (Byte)RatingGroupTypes.probabilitySingleOutcome || mg.TypeOfRatingGroup == (Byte)RatingGroupTypes.singleDate || mg.TypeOfRatingGroup == (Byte)RatingGroupTypes.singleNumber
                                                || mg.TypeOfRatingGroup == (Byte)RatingGroupTypes.probabilityHierarchyTop || mg.TypeOfRatingGroup == (Byte)RatingGroupTypes.hierarchyNumbersTop || mg.TypeOfRatingGroup == (Byte)RatingGroupTypes.probabilityMultipleOutcomes));
            if (theRatingGroups.Any())
                return theRatingGroups.Single().RatingGroupID;
            else
                return null;
        }

        /// <summary>
        /// Adds a rating group to rating hierarchy routine. A helper routine for GetRatingHierarchyDataForTblRowCategory,
        /// which calls itself recursively for owned rating groups.
        /// </summary>
        /// <param name="ratingGroupID">The rating group whose data should be added to the hierarchy</param>
        /// <param name="theData">The rating hierarchy data so far</param>
        /// <param name="hierarchyLevel">The level at which this rating group should be added.</param>
        public void AddRatingGroupToRatingHierarchyData(int ratingGroupID, ref RatingHierarchyData theData, int hierarchyLevel)
        {
            var theRatings = RaterooDB.GetTable<Rating>().Where(m => m.RatingGroupID == ratingGroupID).Select(m => new
            {
                Name = m.Name,
                RatingID = m.RatingID,
                CurrentUserRatingOrFinalValue = m.CurrentValue,
                DecimalPlaces = m.RatingCharacteristic.DecimalPlaces,
                MinVal = m.RatingCharacteristic.MinimumUserRating,
                MaxVal = m.RatingCharacteristic.MaximumUserRating,
                IsDate = m.RatingGroup.TypeOfRatingGroup == (Byte) RatingGroupTypes.singleDate,
                OwnedRatingGroupID = m.OwnedRatingGroupID
            }
                                );
            foreach (var theRating in theRatings)
            {
                theData.Add(theRating.Name, theRating.RatingID, theRating.CurrentUserRatingOrFinalValue, hierarchyLevel, theRating.DecimalPlaces, theRating.MinVal, theRating.MaxVal, theRating.IsDate,  "");
                if (theRating.OwnedRatingGroupID != null)
                    AddRatingGroupToRatingHierarchyData((int)theRating.OwnedRatingGroupID, ref theData, hierarchyLevel + 1);
            }
        }

        public void GetDefaultSortForTblTab(int TblTabID, ref int? TblColumnToSort, ref bool sortOrderAscending)
        {
            TblTab theTblTab = RaterooDB.GetTable<TblTab>().Single(cg => cg.TblTabID == TblTabID);
            TblColumnToSort = theTblTab.DefaultSortTblColumnID;
            if (TblColumnToSort != null)
            {
                int? theTblColumnToSort = TblColumnToSort;
                TblColumn theSortCD = RaterooDB.GetTable<TblColumn>().SingleOrDefault(cd => cd.TblColumnID == theTblColumnToSort && cd.TblTabID == TblTabID && cd.Status == (Byte)StatusOfObject.Active);
                if (theSortCD == null)
                    TblColumnToSort = null;
                else
                    sortOrderAscending = theSortCD.DefaultSortOrderAscending;
            }
        }

      

    }

}
