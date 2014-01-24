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
using System.Collections.Generic;
using GoogleGeocoder;
using System.Xml.Schema;
using System.IO;
using System.Xml;
using System.Xml.Serialization;




using ClassLibrary1.Misc;
using ClassLibrary1.Model;

public partial class Admin_Tbl_user_control_changeTblImportExport : System.Web.UI.UserControl
{
    internal int TblID;
    internal RaterooDataAccess DataAccess = new RaterooDataAccess();
    FilterRules theFilterRules;

    protected void Page_Load(object sender, EventArgs e)
    {
        PMRoutingInfoMainContent Location = PMRouting.IncomingMainContent(Page.RouteData, DataAccess.RaterooDB);
        Tbl theTbl = Location.lastItemInHierarchy.Tbl;
        TblID = theTbl.TblID;
        FieldsBox.Setup(TblID,null,FieldsBoxMode.filterWithoutButton);
        LoadFilterRules();
        if (!Page.IsPostBack)
            ChkIncValues.Checked = false;

    }

    public IQueryable<TblRow> GetFilteredQuery()
    {
        return theFilterRules.GetFilteredQueryAsTblRows(DataAccess.RaterooDB, null);
    }

    public void LoadFilterRules()
    {
        theFilterRules = FieldsBox.GetFilterRules(!ChkInActivate.Checked, false);
    }

    protected void BtnConvert_Click(object sender, EventArgs e)
    {
        try
        {

            DateTime theTime = TestableDateTime.Now;
            string theFullTimeString = TestableDateTime.Now.ToString("yyMMdd HHmmss");
            string idName = "Table " + TblID.ToString() + " " + theFullTimeString;
            RaterooFile excelFile = new RaterooFile("convert", idName + ".xls");
            excelFile.CreateTemporary();
            RaterooFile xmlFile = new RaterooFile("convert", idName + ".xml");
            xmlFile.CreateTemporary();

            if (ConvertFileUpload.HasFile)
            {
                if (ConvertFileUpload.PostedFile.ContentType != "application/vnd.ms-excel")
                {
                    TblRowPopUp.MsgString = "Please provide an Excel file.";
                    TblRowPopUp.Show();
                    return;
                }

                ConvertFileUpload.SaveAs(excelFile.GetPathToLocalFile()); 
            }
            else
            {
                TblRowPopUp.MsgString = "Please provide an Excel file.";
                TblRowPopUp.Show();
                return;
            }
            ImportExport myImportExport = new ImportExport(DataAccess.RaterooDB.GetTable<Tbl>().Single(x => x.TblID == TblID));
            myImportExport.ConvertExcelFileToXML(excelFile.GetPathToLocalFile(), xmlFile.GetPathToLocalFile());
            xmlFile.DownloadToUserBrowser(Response);

        }
        catch (Exception ex)
        {
            TblRowPopUp.MsgString = ex.Message;
            TblRowPopUp.Show();
        }

    }

    protected void BtnImport_Click(object sender, EventArgs e)
    {
        try
        {
            PMActionProcessor Obj = new PMActionProcessor();
            string fileLocation = "";

            if (XMLFileUpload.HasFile)
            {
                if (XMLFileUpload.PostedFile.ContentType != "text/xml")
                {
                    TblRowPopUp.MsgString = "Please provide a XML file.";
                    TblRowPopUp.Show();
                    return;
                }
                DateTime theTime = TestableDateTime.Now;
                string theFullTimeString = TestableDateTime.Now.ToString("yyMMdd HHmmss");
                string idName = "Table " + TblID.ToString() + " " + theFullTimeString;
                RaterooFile tempFile = new RaterooFile("temporary", idName + ".xml");
                tempFile.CreateTemporary();
                fileLocation = tempFile.GetPathToLocalFile();
                XMLFileUpload.SaveAs(fileLocation);
            }
            else
            {
                TblRowPopUp.MsgString = "Please provide a XML file.";
                TblRowPopUp.Show();
                return;
            }
            ImportExport myImportExport = new ImportExport(DataAccess.RaterooDB.GetTable<Tbl>().Single(x => x.TblID ==TblID));
            string errorMessage = "";
            bool IsValid = myImportExport.IsXmlValid(fileLocation, ref errorMessage);
            if (IsValid == false)
            {
                TblRowPopUp.MsgString = "The XML file could not be validated. The error message is: " + errorMessage;
                TblRowPopUp.Show();
                return;
            }

            int userID = (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            User theUser = DataAccess.RaterooDB.GetTable<User>().Single(x => x.UserID == userID);

            myImportExport.PerformImport(fileLocation, userID, ChkIncValues.Checked && theUser.SuperUser);
            

        }
        catch (Exception ex)
        {
            TblRowPopUp.MsgString = ex.Message;
            TblRowPopUp.Show();
        }

    }

    protected void BtnDownLoadXSD_Click(object sender, EventArgs e)
    {
        try
        {
            ImportExport myImportExport = new ImportExport(DataAccess.RaterooDB.GetTable<Tbl>().Single(c => c.TblID == TblID));
            myImportExport.CreateXSDFile();
            RaterooFile xsdFile = myImportExport.GetXSDFileReference();
            xsdFile.DownloadToUserBrowser(Response);
        }
        catch (Exception ex)
        {
            TblRowPopUp.MsgString = ex.Message;
            TblRowPopUp.Show();
        }
    }


    // Enitity  to XML file code start from here 

    protected void BtnExport_Click(object sender, EventArgs e)
    {
        LoadFilterRules();
        IQueryable<TblRow> theQuery = GetFilteredQuery();

        DateTime theTime = TestableDateTime.Now;
        string theFullTimeString = TestableDateTime.Now.ToString("yyMMdd HHmmss");
        string idName = "Table " + TblID.ToString() + " " + theFullTimeString;
        RaterooFile exportFile = new RaterooFile("export", idName + ".xml");
        exportFile.CreateTemporary();
        string xmlFileName = exportFile.GetPathToLocalFile();
        ImportExport myImportExport = new ImportExport(DataAccess.RaterooDB.GetTable<Tbl>().Single(x => x.TblID == TblID));
        myImportExport.PerformExport(xmlFileName, theQuery, ChkIncValues.Checked);
        exportFile.DownloadToUserBrowser(Response);
        exportFile.DeleteTemporary();
    }



   
}
