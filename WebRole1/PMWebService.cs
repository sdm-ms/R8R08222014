using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Web.Security;
using System.IO;
using System.Xml.Serialization;
using System.Web.Profile;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;


namespace PMWebServices
{
    /// <summary>
    /// Summary description for PMWebService
    /// </summary>
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService()]
    public class PMWebService : System.Web.Services.WebService
    {

        public PMWebService()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public TablePopulateResponse PopulateTableInitially(string theTableInfoString, int visibleRowsEstimate, int additionalRows, int rowsToSkip, bool includeHeaderRow)
        {
            return PMTableLoading.PopulateTableInitially(theTableInfoString, visibleRowsEstimate, additionalRows, rowsToSkip, includeHeaderRow);
        }

        [WebMethod]
        public TablePopulateResponse PopulateTableSpecificRows(string theTableInfoString, int firstRow, int numRows,  int visibleRowsEstimate, int firstRowIfResetting)
        {
            return PMTableLoading.PopulateTableSpecificRows(theTableInfoString, firstRow, numRows, visibleRowsEstimate, firstRowIfResetting);
        }

        public UserRatingResponse ConfirmUserAccessInfo(UserAccessInfo theUserAccessInfo)
        {
            IUserProfileInfo theUser = UserProfileCollection.LoadByUsername(theUserAccessInfo.userName);
            if (theUser == null)
            {
                UserRatingResponse theResponse = new UserRatingResponse();
                theResponse.result = new UserRatingResult("You must login before you can enter ratings.");
                return theResponse;
            }
            if ((string)theUser.GetProperty("PasswordForWebService") != theUserAccessInfo.passwordForWebService)
            {
                UserRatingResponse theResponse = new UserRatingResponse();
                theResponse.result = new UserRatingResult("For security purposes, please reload the web page. If you continue to receive this error, please log out and log in again.");
                return theResponse;
            }
            return null;
        }
        
        [WebMethod]
        public List<UserRatingResponse> ProcessRatingsBulk(UserAccessInfo theUserAccessInfo, List<List<RatingAndUserRatingString>> theUserRatings)
        {
            List<UserRatingResponse> theResponses = new List<UserRatingResponse>();
            UserRatingResponse theResponse = ConfirmUserAccessInfo(theUserAccessInfo);
            PMActionProcessor actionProcessor = new PMActionProcessor();
            actionProcessor.ResetDataContexts();
            List<int> specifiedRatingIDs = new List<int>();
            foreach (var userRatingsStringList in theUserRatings)
                RatingAndUserRatingStringConverter.AddRatingIDsToList(userRatingsStringList, specifiedRatingIDs);


            User theUser = actionProcessor.DataContext.GetTable<User>().Single(u => u.Username == theUserAccessInfo.userName);
            List<Rating> ratingsEntered = RatingsAndRelatedInfoLoader.Load(actionProcessor.DataContext, specifiedRatingIDs, theUser);
            bool oneRatingPerRatingGroup = ratingsEntered.First().RatingGroup.TblRow.Tbl.OneRatingPerRatingGroup;
            //actionProcessor.RaterooDB.GetTable<SetUserRatingAddingLoadOption>();
            if (theResponse != null)
            {
                foreach (var ur in theUserRatings)
                    theResponses.Add(theResponse);
            }
            else
            {
                foreach (var ur in theUserRatings)
                    theResponses.Add(AddRelatedUserRatings(actionProcessor, theUser, ur, oneRatingPerRatingGroup ? ratingsEntered : null));
            }
            return theResponses;
        }

        [WebMethod]
        public UserRatingResponse ProcessRatings(UserAccessInfo theUserAccessInfo, List<RatingAndUserRatingString> theUserRatings)
        {
            return ProcessRatingsBulk(theUserAccessInfo, new List<List<RatingAndUserRatingString>>() { theUserRatings }).First();
        }

        private UserRatingResponse AddRelatedUserRatings(PMActionProcessor actionProcessor, User theUser, List<RatingAndUserRatingString> theUserRatings, List<Rating> allRatingsInAffectedCells = null)
        {
            UserRatingResponse theResponse = new UserRatingResponse();
            actionProcessor.UserRatingsAddFromService(allRatingsInAffectedCells, theUserRatings, theUser, ref theResponse);
            return theResponse;
        }

        [WebMethod]
        public UserRatingResponse GetUpdatedRatings(string ratingGroupIDString)
        {
            PMActionProcessor myAction = new PMActionProcessor();
            UserRatingResponse theResponse = myAction.GetUpdatedRatings(ratingGroupIDString);
            return theResponse;
        }

        [WebMethod]
        public UserRatingResponse GetUpdatedRatingsMultiple(List<string> ratingIDStrings)
        {
            PMActionProcessor myAction = new PMActionProcessor();
            UserRatingResponse theResponse = myAction.GetUpdatedRatingsMultiple(ratingIDStrings);
            return theResponse;
        }

        [WebMethod]
        public MyPointsSidebarInfo GetSidebarInfo(string pointsManagerIDString, UserAccessInfo theUserAccessInfo)
        {
            int pointsManagerID = Convert.ToInt32(pointsManagerIDString);
            UserRatingResponse theResponse = ConfirmUserAccessInfo(theUserAccessInfo);
            if (theResponse != null)
                return null;
            PMActionProcessor myAction = new PMActionProcessor();
            PointsManager thePointsManager = myAction.DataContext.GetTable<PointsManager>().SingleOrDefault(x => x.PointsManagerID == pointsManagerID);
            if (thePointsManager == null)
                return null;
            PointsTotal thePointsTotal = myAction.DataContext.GetTable<PointsTotal>().SingleOrDefault(x => x.User.Username == theUserAccessInfo.userName && x.PointsManagerID == pointsManagerID);
            return new MyPointsSidebarInfo(thePointsManager, thePointsTotal);
        }
    }

}