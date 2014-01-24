using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;


namespace WebApplication1
{
    public partial class TblComments : System.Web.UI.Page
    {
        bool CanPredict;
        bool CanAdminister;
        bool CanEditFields;
        PMRoutingInfoMainContent theLocation;
        RaterooDataAccess DataAccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataAccess = new RaterooDataAccess();
            theLocation = PMRouting.IncomingMainContent(Page.RouteData, null);
            CommentsContent.theTblOrNullForRowOnly = theLocation.theTbl;
            ItemPath1.theTbl = theLocation.theTbl;
            DetermineUserRights();
        }

        protected void DetermineUserRights()
        {
            int SubtopicId = theLocation.theTbl.PointsManagerID;

            CanPredict = false;
            CanAdminister = false;
            CanEditFields = false;
            if (ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
            {
                int UserId = (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
                bool canView = DataAccess.CheckUserRights(UserId, UserActionOldList.View, false, SubtopicId, theLocation.theTbl.TblID);
                if (!canView)
                    PMRouting.Redirect(Response, new PMRoutingInfo(PMRouteID.Login));
                // Checking user rights to predict
                CanPredict = DataAccess.CheckUserRights(UserId, UserActionOldList.Predict, false, SubtopicId, theLocation.theTbl.TblID);
                CanAdminister = DataAccess.CheckUserRights(UserId, UserActionOldList.ResolveRatings, false, SubtopicId, theLocation.theTbl.TblID);
                CanEditFields = DataAccess.CheckUserRights(UserId, UserActionOldList.ChangeTblRows, false, SubtopicId, theLocation.theTbl.TblID);
                CommentsContent.UserCanProposeComments = true;
                CommentsContent.UserCanAddComments = CanPredict;
                CommentsContent.UserCanDeleteComments = CanEditFields;
            }
        }
    }
}