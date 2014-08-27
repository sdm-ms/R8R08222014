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
using System.IO;

public partial class Download : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strRequest = Request.QueryString["file"];
        if (strRequest != "")
        {
            string path = Server.MapPath(strRequest);
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            if (file.Exists)
            {
                Response.Clear();

                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());

                Response.ContentType = "application/octet-stream";
                Response.WriteFile(file.FullName);
                Response.Flush();
                Response.End();
               
               
               
            }



        }
        else
        {

            
        }
    }
   
}
