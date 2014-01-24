using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace ClassLibrary1.Model
{

    /// <summary>
    /// Summary description for StringConstants
    /// </summary>
    public sealed class StringConstants
    {



        public StringConstants()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static readonly string StringNotYetStarted = "Not yet started";
        public static readonly string StringSuspendedDirectly = "Suspended directly";
        public static readonly string StringActive = "Active";
        public static readonly string StringSuspendedHigherLevel = "Suspended hierarchically";
        public static readonly string StringEnded = "Ended";
        public static readonly string StringSelect = "--Select--";
        public static readonly string StringStart = "Start";
        public static readonly string StringPause = "Pause";
        public static readonly string StringEnd = "End";
        public static readonly string StringResume = "Resume";
        public static readonly string StringTradingStatusUpdatedSuccessfully = "Trading Status updated successfully";
        public static readonly string StringTblCreatedSuccessfully = "Tbl created successfully";
        public static readonly string StringUntitled = "Untitled";
        public static readonly string StringDefaultUserRightCreatedSuccessfully = "Default users right created successfully";
        public static readonly string StringPointsManagerCreatedSuccessfully = "PointsManager created successfully";
        public static readonly string StringUserCreatedSuccessfully = "User created successfully";
        public static readonly string StringMailUserIdPassword = "<br/>Your login name and password has been emailed to you.  <br/>";
        public static readonly string StringThanks = "Thank you for Registering in UserRating Rating System";
        public static readonly string StringMailHost = "mail.xevoke.co.uk";//"smtp.rateroo.com"; //
        public static readonly string StringMailFromAddress = "info@xevoke.com";
        public static readonly string StringDomainCreatedSuccessfully = "Domain created successfully";
        public static readonly string StringPointAdjustmentCreated = "Point adjustment created for specified user";
        public static readonly string StringUserDoesNotExist = "User name does not exists";
        public static readonly string StringProposalSettingCreated = "Proposal setting created successfully";
        public static readonly string StringWrongPassword = "Either userid or password is wrong";
        public static readonly string StringEmailConfirmation = "<br/>Your login name and password has been emailed to you.  <br/>";
        public static readonly string StringLoginInformation = "Your login information for prediction rating system";
        public static readonly string StringPointsTrustRuleChanged = "Counting rule changed for universe";
        public static readonly string StringDefaultRatingGroupAttributeChanged = "Default rating group attribute changed successfully";
        public static readonly string StringDollarSubsidyChanged = "Dollar subsidy changed successfully";
        public static readonly string StringUserRightCreated = "Users Right Created successfully";
        public static readonly string StringVerifyRegistration = "Your Registration has verified";
        public static readonly string StringChangeTblTabDescriptorName = "Name Of Category Group descriptor Changed";
        public static readonly string StringChangeTblTabName = "Category Group Name Changed";
        public static readonly string StringChangeTblTabWord = "Category Group Word changed";
        public static readonly string StringCategoryDescriptoeCreated = "Category Descriptor Created";
        public static readonly string StringCategoryDescriptoeChanged = "Category Descriptor Changed";
        public static readonly string StringCreateTblTab = "Category Group Created";
        public static readonly string StringDeleteTblColumn = "Category descriptor Deleted";
        public static readonly string StringDeleteTblTab = "Category Group Deleted";
        public static readonly string StringChangeCategoryAbrr = "Category Abbriviation Changed";
        public static readonly string StringTblRowCreate = "TblRow Created";
        public static readonly string StringTblRowDelete = "TblRow deleted";
        public static readonly string StringFieldCreate = "Field created";
        public static readonly string StringAddressField = "AddressField";
        public static readonly string StringChoiceField = "ChoiceField";
        public static readonly string StringDateTimeField = "DateTimeField";
        public static readonly string StringNumberField = "NumberField";
        public static readonly string StringTextField = "TextField";
        public static readonly string StringFieldDefinitionCreate = "Field Descriptor Created";
        public static readonly string StringFieldDefinitionDelete = "Field descriptor Deleted";
        public static readonly string StringRatingGroupAttributeCreate = "Rating Group Attribute created";
        public static readonly string StringAllItem = "All Item";
        public static readonly decimal DefaultSubsidyLevel = 1000;
        public static readonly int DefaultRunTime = 2419200;
        public static readonly int DefaultHalfLifeForResolution = 172800;
        public static readonly int DefaultRepeatTimes = 1;
        public static readonly string StringNoItem = "There is no item to display.";
        public static readonly int PageSize = 5;
        public static readonly string StringNoRight = "Your account is not authorized to view this page.";
        public static readonly string StringInvitation = "Invitation to join prediction rating system.";
        public static readonly string Stringunlogeduser = "You Must LogIn to make edit to this page.";
        public static readonly string Stringnopredict = "Your account is not authorized to make edits to this page.";
        public static readonly string Stringprediction = "Win points by clicking on numbers that you think are wrong and entering the numbers that you think are right.";
        public static readonly string StringDomainChange = "Domain Change Successfully";
        public static readonly string StringAnounceCreated = "Anouncement created Successfully";
        public static readonly string StringAnounceChange = "Anouncement Changed Successfully";
        public static readonly string StringcategroupwordCreated = "TblTblTabWord Created Successfully";
        public static readonly string StringChoiceFieldCreated = "ChoiceField Created";
        public static readonly string StringSortOptionCreated = "Default Sort Option Save Successfully";





    }

}