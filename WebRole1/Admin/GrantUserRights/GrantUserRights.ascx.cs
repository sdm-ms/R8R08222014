using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


namespace WebApplication1.Admin.GrantUserRights
{
    public partial class GrantUserRights : System.Web.UI.UserControl
    {
        public ActionProcessor theActionProcessor;
        public Guid pointsManagerID;

        public void Setup(Guid thePointsManagerID)
        {
            pointsManagerID = thePointsManagerID;
            theActionProcessor = new ActionProcessor();
            existingUsers.Text = "Currently authorized users: " + GetExistingUsersString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetExistingUsersString()
        {
            string theString = "";
            var theUsers = theActionProcessor.DataContext.GetTable<UsersRight>().Where(x => x.PointsManagerID == pointsManagerID && x.MayView && x.MayPredict && x.User != null && x.Status == (int) StatusOfObject.Active).Select(x => x.User.Username).ToList();
            for (int i = 0; i < theUsers.Count(); i++)
            {
                theString += theUsers[i];
                if (i < theUsers.Count() - 1)
                    theString += ", ";
            }
            return theString;
        }

        protected string[] GetUserNames()
        {
            return usernames.Text.Split(',').Select(x => x.Trim()).ToArray();
        }

        protected List<User> GetUsers()
        {
            string[] usernames = GetUserNames();
            List<User> theList = new List<User>();
            foreach (string username in usernames)
            {
                User aUser = theActionProcessor.DataContext.GetTable<User>().SingleOrDefault(x => x.Username == username);
                if (aUser != null)
                    theList.Add(aUser);
            }
            return theList;
        }

        protected void SetAuthorized(bool authorize)
        {

            Guid CurrentUserID = (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            List<User> theList = GetUsers();
            foreach (User theUser in theList)
            {
                if (authorize)
                    theActionProcessor.UsersRightsCreate(theUser.UserID, pointsManagerID, true, true, false, true, true, false, false, false, false, false, false, "Partial administrative privileges", true, true, CurrentUserID, null);
                else
                    theActionProcessor.UsersRightsCreate(theUser.UserID, pointsManagerID, false, false, false, false, false, false, false, false, false, false, false, "No privileges", true, true, CurrentUserID, null);
            }
            Setup(pointsManagerID);
            usernames.Text = "";
        }

        protected void Authorize_Click(object sender, EventArgs e)
        {
            SetAuthorized(true);
        }
        protected void Deauthorize_Click(object sender, EventArgs e)
        {
            SetAuthorized(false);
        }
    }
}