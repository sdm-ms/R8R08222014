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
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;

public partial class NewUser : System.Web.UI.Page
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //var completeAccountButton = FindControl("CreateUserWizardStep1");
        //this.Form.DefaultButton = completeAccountButton.UniqueID;
    }

    protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
    {
            ActionProcessor theActionProcessor = new ActionProcessor();
            TextBox UserNameTextBox = (TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("UserName");
            TextBox EmailTextBox = (TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email");
            IUserProfileInfo theUserProfileInfo = UserProfileCollection.LoadByUsername(UserNameTextBox.Text);
            Guid userID = theActionProcessor.UserAdd(theUserProfileInfo.Username, false, "", "", true);
    }

    protected void CreateUserWizard1_CreatingUser(object sender, LoginCancelEventArgs e)
    {

    }

    protected void CompleteWizardStep1_Activate(object sender, EventArgs e)
    {
        Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
    }
        
}
