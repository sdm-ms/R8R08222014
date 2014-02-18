using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Profile;
using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.Model;

namespace ClassLibrary1.Misc
{
    public interface IUserProfileInfo
    {
        void DeleteUser(bool deleteAllRelatedData);
        string Email { get; set; }
        object GetProperty(string propertyName);
        void SavePropertyChanges();
        void SetProperty(string propertyName, object propertyValue, bool save = true);
        IUserProfileInfo LoadByUsername(string username);
        IUserProfileInfo CreateUser(string username, string password, string email);
        bool UnlockUser();
        string Username { get; set; }
    }

    public class AnonymousUser : IUserProfileInfo
    {
        public void DeleteUser(bool deleteAllRelatedData)
        {
        }

        public string Email
        {
            get
            {
                return "N/A";
            }
            set
            {
            }
        }

        public object GetProperty(string propertyName)
        {
            if (propertyName == "UserID")
                return 0;
            return "N/A";
        }

        public void SavePropertyChanges()
        {
        }

        public void SetProperty(string propertyName, object propertyValue, bool save = true)
        {
        }

        public IUserProfileInfo LoadByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public IUserProfileInfo CreateUser(string username, string password, string email)
        {
            return this;
        }

        public bool UnlockUser()
        {
            return true;
        }

        public string Username
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public static class UseFakeUserProfileInfo 
    {
        public static bool UseFake { get { return !GetIRaterooDataContext.UseRealDatabase; } } //  !RoleEnvironment.IsAvailable; } } // i.e., are we unit testing?
    }

    public static class UserProfileCollection
    {

        public static List<IUserProfileInfo> GetAllUsers()
        {
            if (UseFakeUserProfileInfo.UseFake)
                return FakeUserProfileCollection.GetAllUsers();
            else
                return RealUserProfileCollection.GetAllUsers();
        }

        public static IUserProfileInfo GetCurrentUser()
        {
            if (UseFakeUserProfileInfo.UseFake)
                return FakeUserProfileCollection.GetCurrentUser();
            else
                return RealUserProfileCollection.GetCurrentUser();
        }

        public static void DeleteAllUsers()
        {
            if (UseFakeUserProfileInfo.UseFake)
                FakeUserProfileCollection.DeleteAllUsers();
            else
                RealUserProfileCollection.DeleteAllUsers();
        }

        public static IUserProfileInfo LoadByUsername(string username)
        {
            if (UseFakeUserProfileInfo.UseFake)
                return new FakeUserProfileInfo().LoadByUsername(username);
            else
                return new RealUserProfileInfo().LoadByUsername(username);
        }

        public static IUserProfileInfo CreateUser(string username, string password, string email)
        {
            if (UseFakeUserProfileInfo.UseFake)
                return new FakeUserProfileInfo().CreateUser(username, password, email);
            else
                return new RealUserProfileInfo().CreateUser(username, password, email);
        }
    }

    public static class FakeUserProfileCollection
    {
        public static List<IUserProfileInfo> theList = new List<IUserProfileInfo>();

        public static List<IUserProfileInfo> GetAllUsers()
        {
            return theList;
        }

        public static void Add(IUserProfileInfo theUserProfile)
        {
            theList.Add(theUserProfile);
        }

        public static void Delete(IUserProfileInfo theUserProfile)
        {
            theList.Remove(theUserProfile);
        }

        public static IUserProfileInfo GetCurrentUser()
        {
            return null;
        }

        public static void DeleteAllUsers()
        {
        }
    }

    public class FakeUserProfileInfo : IUserProfileInfo
    {
        Dictionary<string,object> properties = new Dictionary<string,object>();

        public void DeleteUser(bool deleteAllRelatedData)
        {
            FakeUserProfileCollection.Delete(this);
        }
        public string Email { get; set; }
        public object GetProperty(string propertyName)
        {
            return properties[propertyName];
        }
        public void SavePropertyChanges()
        {
        }
        public void SetProperty(string propertyName, object propertyValue, bool save = true)
        {
            properties[propertyName] = propertyValue;
        }

        public IUserProfileInfo LoadByUsername(string username)
        {
            Username = username;
            return this;
        }
        public IUserProfileInfo CreateUser(string username, string password, string email)
        {
            Username = username;
            Email = email;
            return this;
        }
        public bool UnlockUser()
        {
            return true;
        }
        public string Username { get; set; }
    }

    public static class RealUserProfileCollection
    {
        public static List<IUserProfileInfo> GetAllUsers()
        {
            MembershipUserCollection theCollection = new MembershipUserCollection();
            List<IUserProfileInfo> theList = new List<IUserProfileInfo>();
            foreach (var item in theCollection)
            {
                string userName = ((MembershipUser)item).UserName;
                theList.Add(UserProfileCollection.LoadByUsername(userName));
            }
            return theList;
        }

        public static IUserProfileInfo GetCurrentUser()
        {
            MembershipUser theUser = Membership.GetUser();
            if (theUser == null)
                return new AnonymousUser();
            return UserProfileCollection.LoadByUsername(theUser.UserName);
        }

        public static void DeleteAllUsers()
        {
            var allUsers = GetAllUsers();
            foreach (var user in allUsers)
                user.DeleteUser(true);
            System.Web.Profile.ProfileManager.DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption.All, TestableDateTime.Now);
        }
    }

    public class RealUserProfileInfo : IUserProfileInfo
    {
        public string Username { get; set; }
        internal ProfileBase Profile {get; set;}

        public RealUserProfileInfo()
        {
        }

        public IUserProfileInfo LoadByUsername(string username)
        {
            Username = username;
            if (Profile == null && GetUser() != null)
                Profile = ProfileBase.Create(Username);
            return this;
        }

        public IUserProfileInfo CreateUser(string username, string password, string email)
        {
            Membership.CreateUser(username, password, email);
            Username = username;
            Profile = ProfileBase.Create(Username);
            return this;
        }

        public string Email
        {
            get
            {
                return GetUser().Email;
            }
            set
            {
                MembershipUser theMU = GetUser();
                theMU.Email = value;
                Membership.UpdateUser(theMU);
            }
        }

        public object GetProperty(string propertyName)
        {
            if (Profile == null)
                throw new Exception("Internal error: Invalid attempt to access user's profile.");
            return Profile[propertyName];
        }

        public void SetProperty(string propertyName, object propertyValue, bool save = true)
        {
            if (Profile == null)
                throw new Exception("Internal error: Invalid attempt to access user's profile.");
            Profile[propertyName] = propertyValue;
            if (save)
                SavePropertyChanges();
        }

        public void SavePropertyChanges()
        {
            Profile.Save();
        }

        internal MembershipUser GetUser()
        {
            return Membership.GetUser(Username);
        }

        public void DeleteUser(bool deleteAllRelatedData)
        {
            Membership.DeleteUser(Username, deleteAllRelatedData);
            Username = "";
        }

        public bool UnlockUser()
        {
            return GetUser().UnlockUser();
        }
    }
}
