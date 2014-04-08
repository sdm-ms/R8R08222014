using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

public partial class MyAccount : System.Web.UI.Page
{
    IUserProfileInfo theUserProfileInfo = ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (theUserProfileInfo == null)
        {
            Routing.Redirect(Response, new RoutingInfoLoginRedirect(Routing.Outgoing(new RoutingInfo(RouteID.MyAccount))));
            return;
        }
        if (!Page.IsPostBack)
        {
            FirstName.Text = (string) theUserProfileInfo.GetProperty("FirstName");
            LastName.Text = (string) theUserProfileInfo.GetProperty("LastName");
            EmailAddress.Text = theUserProfileInfo.Email;
            Address1.Text = (string) theUserProfileInfo.GetProperty("Address1");
            Address2.Text = (string) theUserProfileInfo.GetProperty("Address2");
            City.Text = (string) theUserProfileInfo.GetProperty("City");
            State.Text = (string) theUserProfileInfo.GetProperty("State");
            ZipCode.Text = (string) theUserProfileInfo.GetProperty("ZipCode");
            Country.Text = (string) theUserProfileInfo.GetProperty("Country");
            HomePhone.Text = (string) theUserProfileInfo.GetProperty("HomePhone");
            WorkPhone.Text = (string) theUserProfileInfo.GetProperty("WorkPhone");
        }
    }

    protected void CancelChanges_Click(object sender, EventArgs e)
    {
        Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
        return;
    }

    protected void SubmitChanges_Click(object sender, EventArgs e)
    {
        if (!(HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null))
        {
            Routing.Redirect(Response, new RoutingInfoLoginRedirect(Routing.Outgoing(new RoutingInfo(RouteID.MyAccount))));
            return;
        }
        if (FirstName.Text != (string)(theUserProfileInfo.GetProperty("FirstName")))
            theUserProfileInfo.SetProperty("FirstName", FirstName.Text);
        if (LastName.Text != (string)(theUserProfileInfo.GetProperty("LastName")))
            theUserProfileInfo.SetProperty("LastName", LastName.Text);
        if (theUserProfileInfo.Email != EmailAddress.Text)
        {
            theUserProfileInfo.Email = EmailAddress.Text;
        }
        if (Address1.Text != (string)theUserProfileInfo.GetProperty("Address1"))
            theUserProfileInfo.SetProperty("Address1", Address1.Text, false);
        if (Address2.Text != (string)theUserProfileInfo.GetProperty("Address2"))
            theUserProfileInfo.SetProperty("Address2", Address2.Text, false);
        if (City.Text != (string)theUserProfileInfo.GetProperty("City"))
            theUserProfileInfo.SetProperty("City", City.Text, false);
        if (State.Text != (string)theUserProfileInfo.GetProperty("State"))
            theUserProfileInfo.SetProperty("State", State.Text, false);
        if (ZipCode.Text != (string)theUserProfileInfo.GetProperty("ZipCode"))
            theUserProfileInfo.SetProperty("ZipCode", ZipCode.Text, false);
        if (Country.Text != (string)theUserProfileInfo.GetProperty("Country"))
            theUserProfileInfo.SetProperty("Country", Country.Text, false);
        if (WorkPhone.Text != (string)theUserProfileInfo.GetProperty("WorkPhone"))
            theUserProfileInfo.SetProperty("WorkPhone", WorkPhone.Text, false);
        if (HomePhone.Text != (string)theUserProfileInfo.GetProperty("HomePhone"))
            theUserProfileInfo.SetProperty("HomePhone", HomePhone.Text, false);
        theUserProfileInfo.SavePropertyChanges();
        Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
    }
}
