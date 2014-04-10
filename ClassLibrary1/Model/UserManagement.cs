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

using StringEnumSupport;

using System.Diagnostics;
using System.Globalization;
using System.Web.Profile;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{

    public enum UserActionsList
    {
        [StringValue("View predictions and other content")]
        View = 1,
        [StringValue("Add predictions")]
        Predict,
        [StringValue("Domain: Create (SuperUser Only)")]
        DomainCreate,
        [StringValue("Domain: Change miscellaneous settings (SuperUser Only)")]
        DomainChangeSettings,
        [StringValue("Domain: Insert content for users to view(SuperUser Only)")]
        DomainInsertContent,
        [StringValue("Domain: Change default styling (SuperUser Only)")]
        DomainChangeDefaultStyling,
        [StringValue("Domain: Create new universe (SuperUser Only)")]
        DomainCreatePointsManager,
        [StringValue("Domain: Delete universe (SuperUser Only)")]
        DomainDeletePointsManager,
        [StringValue("Domain: Undelete universe (SuperUser Only)")]
        DomainUndeletePointsManager,
        [StringValue("PointsManager: Change default rating type")]
        PointsManagerChangeDefaultRatingType,
        [StringValue("PointsManager: Change default Tbl")]
        PointsManagerChangeDefaultTbl,
        [StringValue("PointsManager: Change name of universe")]
        PointsManagerChangeName,
        [StringValue("PointsManager: Change default styling")]
        PointsManagerChangeDefaultStyling,
        [StringValue("PointsManager: Change rules for determining which users are trusted")]
        PointsManagerChangePointsTrustRules,
        [StringValue("PointsManager: Change subsidy information (SuperUser Only)")]
        PointsManagerChangeSubsidyInformation,
        [StringValue("PointsManager: Set default proposal rating settings")]
        PointsManagerSetProposalRatingSettings,
        [StringValue("PointsManager: Set default reward rating settings")]
        PointsManagerSetRewardRatingSettings,
        [StringValue("PointsManager: Start, pause, and resume trading")]
        PointsManagerChangeTradingStatus,
        [StringValue("PointsManager: Insert content for users to view")]
        PointsManagerInsertContent,
        [StringValue("PointsManager: Create Tbl")]
        PointsManagerCreateTbl,
        [StringValue("PointsManager: Delete Tbl")]
        PointsManagerDeleteTbl,
        [StringValue("PointsManager: Undelete Tbl")]
        PointsManagerUndeleteTbl,
        [StringValue("Tbl: Change name")]
        TblChangeName,
        [StringValue("Tbl: Change name")]
        TblChangeTblTabWord,
        [StringValue("Tbl: Change string used to define a group of categories")]
        TblChangeNameForTblRow,
        [StringValue("Tbl: Change string used to define the name of each entity in the Tbl")]
        TblCreateGroupOfCategories,
        [StringValue("Tbl: Add a new group of categories")]
        TblDeleteGroupOfCategories,
        [StringValue("Tbl: Delete a group of categories")]
        TblUndeleteGroupOfCategories,
        [StringValue("Tbl: Undelete a group of categories")]
        TblCreateCategory,
        [StringValue("Tbl: Create a new category")]
        TblDeleteCategory,
        [StringValue("Tbl: Delete a category")]
        TblUndeleteCategory,
        [StringValue("Tbl: Undelete a category")]
        TblCategoryChangeAbbreviation,
        [StringValue("Tbl: Change the name of a category")]
        TblCategoryChangeName,
        [StringValue("Tbl: Start, pause, and resume trading")]
        TblChangeTradingStatus,
        [StringValue("Tbl: Create a conditional relationship between two categories")]
        TblCreateConditionalRelationship,
        [StringValue("Tbl: Insert content for users to view")]
        TblInsertContent,
        [StringValue("Tbl: Change styling")]
        TblChangeStyling,
        [StringValue("Fields: Define a group of choices for a field")]
        FieldsDefineChoiceGroup,
        [StringValue("Fields: Modify a group of choices")]
        FieldsModifyChoiceGroup,
        [StringValue("Fields: Define a new field to be added to all entities")]
        FieldsCreateNewFieldDefinition,
        [StringValue("Fields: Change the name of a field")]
        FieldsChangeFieldDefinitionName,
        [StringValue("Fields: Change whether a field can be used as a filter")]
        FieldsChangeFieldDefinitionFilterStatus,
        [StringValue("Fields: Change how the information from a field will be displayed")]
        FieldsChangeFieldDefinitionDisplaySettings,
        [StringValue("Fields: Delete a field from all entities")]
        FieldsDeleteFieldDefinition,
        [StringValue("Fields: Undelete a field previously deleted from all entities")]
        FieldsUndeleteFieldDefinition,
        [StringValue("Fields: Edit the values of a fields for specific entities")]
        FieldsEditValues,
        [StringValue("Fields: Add a tag")]
        FieldsAddTag,
        [StringValue("TblRows: Change the name of an entity")]
        TblRowsChangeName,
        [StringValue("TblRows: Create a new entity and associated ratings")]
        TblRowsCreate,
        [StringValue("TblRows: Delete an entity")]
        TblRowsDelete,
        [StringValue("TblRows: Undelete a previously deleted entity")]
        TblRowsUndelete,
        [StringValue("TblRows: Create comments on an entity")]
        TblRowsCreateComments,
        [StringValue("TblRows: Delete comments on an entity")]
        TblRowsDeleteComments,
        [StringValue("Ratings: Define rating types")]
        RatingsDefineTypes,
        [StringValue("Ratings: Split a contingency in a rating into multiple contingencies")]
        RatingsSplit,
        [StringValue("Ratings: Allow override of rating type for a particular rating")]
        RatingsAllowOverrideOfType,
        [StringValue("Ratings: Create a condition for a rating")]
        RatingsCreateCondition,
        [StringValue("Ratings: Resolve predictions at particular values")]
        RatingsResolveAtParticularValues,
        [StringValue("Ratings: Resolve by unwinding all pending predictions")]
        RatingsResolveByUnwinding,
        [StringValue("Ratings: Unresolve previously resolved predictions")]
        RatingsUnresolve,
        [StringValue("Users: Add points to or subtract points from a user's account")]
        UsersAllowPointsAdjustments,
        [StringValue("Users: Create an administration rights group")]
        UsersCreateAdministrationRightsGroup,
        [StringValue("Users: Define administration rights group")]
        UsersChangeAdministrationRightsGroupSettings,
        [StringValue("Users: Specify the rights of particular users")]
        UsersSpecifyUsersRights,
        [StringValue("Users: Add a new user to Rateroo (SuperUser only)")]
        UsersAddNewUserToSystem,
        [StringValue("Users: Invite a new user to the universe")]
        UsersAddNewUserToPointsManager,
        [StringValue("Users: Set user verification status (SuperUser only)")]
        UserSetVerificationStatus,
        [StringValue("Users: Change account settings (SuperUser only)")]
        UserChangeAccountSettings,
        PastLast /* Shows 1 more than count */
    }

        /// <summary>
        /// Summary description for RaterooSupport
        /// </summary>
        public partial class RaterooDataManipulation
        {

            internal int? GetAdministrationRightsGroup(int? userID, int? pointsManagerID)
            {
                UsersAdministrationRightsGroup theGroup;

                theGroup = DataContext.GetTable<UsersAdministrationRightsGroup>().SingleOrDefault(uarg => uarg.UserID == userID && uarg.PointsManagerID == pointsManagerID && uarg.Status == (Byte)StatusOfObject.Active);
                if (theGroup != null)
                    return theGroup.AdministrationRightsGroupID;
                if (userID != null) // Look for administration rights group for the default user
                {
                    theGroup = DataContext.GetTable<UsersAdministrationRightsGroup>().SingleOrDefault(uarg => uarg.UserID == null && uarg.PointsManagerID == pointsManagerID && uarg.Status == (Byte)StatusOfObject.Active);
                    if (theGroup != null)
                        return theGroup.AdministrationRightsGroupID;
                }
                return null;
            }

            internal AdministrationRight GetAdministrationRight(int? userID, int? pointsManagerID, UserActionsList theAction)
            {
                int? administrationRightsGroup = GetAdministrationRightsGroup(userID, pointsManagerID);
                // first look for a matching administration right
                AdministrationRight theRight = DataContext.GetTable<AdministrationRight>().SingleOrDefault(ar => ar.Status == (Byte)StatusOfObject.Active && ar.AdministrationRightsGroupID == administrationRightsGroup && ar.UserActionID == (Byte)theAction);
                if (theRight != null)
                    return theRight;
                theRight = DataContext.GetTable<AdministrationRight>().SingleOrDefault(ar => ar.Status == (Byte)StatusOfObject.Active && ar.AdministrationRightsGroupID == administrationRightsGroup && ar.UserActionID == null); // the default user right
                return theRight;
            }

            // Delete the other CheckUserRights routine, and replace all references with calls to this one.
            public bool CheckUserRightsNew(int? userID, UserActionsList theAction, bool proposalOnly, bool seekingReward, int? pointsManagerID, int? TblID)
            {
                if (pointsManagerID == null && TblID != null)
                {
                    pointsManagerID = DataContext.GetTable<Tbl>().SingleOrDefault(x => x.TblID == TblID).PointsManagerID;
                }
                if (userID == null && (theAction != UserActionsList.View))
                { // Anonymous user -- can at most view a Tbl.
                    return false;
                }
                else
                {
                    AdministrationRight theRight = GetAdministrationRight(userID, pointsManagerID, theAction);
                    if (theRight == null)
                        return false;
                    if (theAction == UserActionsList.View || theAction == UserActionsList.Predict)
                    {
                        proposalOnly = false;
                        seekingReward = false;
                    }
                    bool correctTypeOfAction = (!proposalOnly && theRight.AllowUserToMakeImmediateChanges) || (proposalOnly && theRight.AllowUserToMakeProposals);
                    bool correctRewardSeeking = (!seekingReward && theRight.AllowUserNotToSeekRewards) || (seekingReward && theRight.AllowUserToSeekRewards);
                    return correctTypeOfAction && correctRewardSeeking;
                }
            }


            public static string GetRandomPasswordUsingGUID(int length)
            {
                // Get the GUID
                string guidResult = System.Guid.NewGuid().ToString();

                // Remove the hyphens
                guidResult = guidResult.Replace("-", string.Empty);

                // Make sure length is valid
                if (length <= 0 || length > guidResult.Length)
                    throw new ArgumentException("Length must be between 1 and " + guidResult.Length);

                // Return the first length bytes
                return guidResult.Substring(0, length);
            }

            public static UserAccessInfo GetUserAccessInfoForCurrentUser()
            {
                DateTime debugTiming = TestableDateTime.Now;

                IUserProfileInfo theUser = ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser();
                if (theUser == null)
                    return new UserAccessInfo { userName = "", passwordForWebService = "" };


                if ((string)theUser.GetProperty("LastPageLoadDateTime") == null || (string)theUser.GetProperty("LastPageLoadDateTime") == "")
                {
                    theUser.SetProperty("LastPageLoadDateTime", TestableDateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), false);
                    theUser.SetProperty("PasswordForWebService", GetRandomPasswordUsingGUID(12), false);
                    //Trace.WriteLine("Setting brand new PasswordForWebService " + existing_pc["PasswordForWebService"]);
                }
                DateTime theLastPageLoadDateTime;
                try
                {
                    theLastPageLoadDateTime = DateTime.ParseExact((string)theUser.GetProperty("LastPageLoadDateTime"), "yyyy-MM-dd HH:mm tt", CultureInfo.InvariantCulture);
                }
                catch
                {
                    theLastPageLoadDateTime = TestableDateTime.Now - new TimeSpan(24, 0, 0);
                }
                if (TestableDateTime.Now - theLastPageLoadDateTime > new TimeSpan(24, 0, 0))
                {
                    //Trace.WriteLine("Time lapsed -- LastPageLoadDateTime was " + theLastPageLoadDateTime.ToLongTimeString() + " is " + TestableDateTime.Now.ToLongTimeString() + " so Setting new PasswordForWebService " + existing_pc["PasswordForWebService"]);
                    theUser.SetProperty("PasswordForWebService", GetRandomPasswordUsingGUID(12), false);
                    theUser.SetProperty("LastPageLoadDateTime", TestableDateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), false);
                }
                else
                {
                    //Trace.TraceInformation("using passwordforwebservice " + existing_pc["PasswordForWebService"]);
                }

                theUser.SavePropertyChanges();

                Trace.TraceInformation("GetUserAccessInfo time elapsed " + (TestableDateTime.Now - debugTiming).ToString());

                return new UserAccessInfo { userName = theUser.Username, passwordForWebService = (string)theUser.GetProperty("PasswordForWebService") };
            }



            /// <summary>
            /// 
            /// </summary>
            /// <param name="userid"></param>
            /// <param name="isVerified"></param>
            public void SetUserVerificationStatus(int userid, bool isVerified)
            {
                UserInfo theUserInfo = DataContext.GetTable<UserInfo>().Single(user => user.UserID == userid);
                theUserInfo.IsVerified = isVerified;
                DataContext.SubmitChanges();
            }


        }
    }