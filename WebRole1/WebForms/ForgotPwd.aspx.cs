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
using System.Net.Mail;
using System.Diagnostics;

public partial class ForgotPwd : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PasswordRecovery1.Focus();
        Page.Form.DefaultButton = PasswordRecovery1.FindControl("UserNameContainerID$SubmitButton").UniqueID;
    }

    protected void PasswordRecovery1_SendingMail(object sender, MailMessageEventArgs e)
    {
        System.Diagnostics.Trace.TraceInformation("sendingmail");
        SmtpClient mySmtpClient = new SmtpClient();
        mySmtpClient.EnableSsl = true;
        mySmtpClient.Send(e.Message);
        e.Cancel = true;

    }

    protected void PasswordRecovery1_SendMailError(object sender, SendMailErrorEventArgs e)
    {
        System.Diagnostics.Trace.TraceError("sendmailerror");
    }

    protected void PasswordRecovery1_UserLookupError(object sender, EventArgs e)
    {
        System.Diagnostics.Trace.TraceInformation("userlookuperror");
    }
    
}
