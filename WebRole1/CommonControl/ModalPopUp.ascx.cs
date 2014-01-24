using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

public partial class User_Control_ModalPopUp : System.Web.UI.UserControl
{
    string Msg;
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }
    public string MsgString
    {
        get
        {
            return Msg;
        }
        set
        {
            Msg = value;
        }
    }
    public void Show()
    {
        MpeMsg.DynamicContextKey = this.MsgString;
     
        MpeMsg.Show();
       
    }
}
