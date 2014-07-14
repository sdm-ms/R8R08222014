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
using System.Text.RegularExpressions;
using ClassLibrary1.Model;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!CheckJavaScriptHelper.IsJavascriptEnabled && HttpContext.Current.Profile != null) // clicking logout without javascript will direct us here without automatic logout, so let's log out
            FormsAuthentication.SignOut();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var theUserNameTextBox = Login1.FindControl("UserName");
        if (theUserNameTextBox != null)
            ((TextBox)theUserNameTextBox).Focus();
    }

    //public bool IsValidEmail(string strIn)
    //{
    //    // Return true if strIn is in valid e-mail format.
    //    return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    //}

    public void OnLoggedIn(object sender, EventArgs e)
    {
        RoutingInfoLoginRedirect theRedirect = Routing.IncomingLoginRedirect(Page.RouteData, null);
        Response.Redirect(theRedirect.redirectURL);
    }

    public void OnLoggingIn(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
    {
        //if (!IsValidEmail(Login1.UserName))
        //{
        //    Login1.InstructionText = "Enter a valid e-mail address.";
        //    Login1.InstructionTextStyle.ForeColor = System.Drawing.Color.RosyBrown;
        //    e.Cancel = true;
        //}
        //else
        //{
            Login1.InstructionText = String.Empty;
        //}
    }

    public void OnLoginError(object sender, EventArgs e)
    {
        Login1.HelpPageText = "Help with logging in...";
        Login1.PasswordRecoveryText = "Forgot your password?";
    }

}
