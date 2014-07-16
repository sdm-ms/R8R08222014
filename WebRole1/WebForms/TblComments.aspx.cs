using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


namespace WebApplication1
{
    public partial class TblComments : System.Web.UI.Page
    {
        bool CanPredict;
        bool CanAdminister;
        bool CanEditFields;
        RoutingInfoMainContent theLocation;
        R8RDataAccess DataAccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataAccess = new R8RDataAccess();
            theLocation = Routing.IncomingMainContent(Page.RouteData, null);
            CommentsContent.theTblOrNullForRowOnly = theLocation.theTbl;
            ItemPath1.theTbl = theLocation.theTbl;
            DetermineUserRights();
        }

        protected void DetermineUserRights()
        {
            Guid SubtopicId = theLocation.theTbl.PointsManagerID;

            CanPredict = false;
            CanAdminister = false;
            CanEditFields = false;
            if (ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
            {
                Guid UserId = (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
                bool canView = DataAccess.CheckUserRights(UserId, UserActionType.View, false, SubtopicId, theLocation.theTbl.TblID);
                if (!canView)
                    Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
                // Checking user rights to predict
                CanPredict = DataAccess.CheckUserRights(UserId, UserActionType.Predict, false, SubtopicId, theLocation.theTbl.TblID);
                CanAdminister = DataAccess.CheckUserRights(UserId, UserActionType.ResolveRatings, false, SubtopicId, theLocation.theTbl.TblID);
                CanEditFields = DataAccess.CheckUserRights(UserId, UserActionType.ChangeTblRows, false, SubtopicId, theLocation.theTbl.TblID);
                CommentsContent.UserCanProposeComments = true;
                CommentsContent.UserCanAddComments = CanPredict;
                CommentsContent.UserCanDeleteComments = CanEditFields;
            }
        }
    }
}