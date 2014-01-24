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
using System.Collections.Generic;
using GoogleGeocoder;


public partial class CommonControl_PageButtons : System.Web.UI.UserControl
{
    public delegate void OnButtonClick();
    public event OnButtonClick okHandler;

    public delegate void OnButtonCancle();
    public event OnButtonCancle cnlHandler;

    

    // Click Event of Implement Button
    protected void BtnImplement_Click(object sender, EventArgs e)
    {
        try
        {
            if (okHandler != null)
                okHandler();


        }
        catch (Exception ex)
        {
            PopUp.MsgString = ex.Message;
            PopUp.Show();
        }
        

    }

    // Click Event of Cancel Button
    protected void BtnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            if (cnlHandler != null)
                cnlHandler();



        }
        catch (Exception ex)
        {
            PopUp.MsgString = ex.Message;
            PopUp.Show();
        }
    }
    


   
}
