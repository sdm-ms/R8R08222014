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
using ClassLibrary1.EFModel;

public partial class Admin_Tbl_user_control_changeTblImportExport : System.Web.UI.UserControl
{
    internal Guid TblID;
    internal R8RDataAccess DataAccess = new R8RDataAccess();
    FilterRules theFilterRules;

    protected void Page_Load(object sender, EventArgs e)
    {
        RoutingInfoMainContent Location = Routing.IncomingMainContent(Page.RouteData, DataAccess.R8RDB);
        Tbl theTbl = Location.lastItemInHierarchy.Tbl;
        TblID = theTbl.TblID;
        FieldsBox.Setup(TblID,null,FieldsBoxMode.filterWithoutButton);
        LoadFilterRules();
        if (!Page.IsPostBack)
            ChkIncValues.Checked = false;

    }

    public IQueryable<TblRow> GetFilteredQuery()
    {
        return theFilterRules.GetFilteredQueryAsTblRows(DataAccess.R8RDB, null);
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
            R8RFile excelFile = new R8RFile("convert", idName + ".xls");
            excelFile.CreateTemporary();
            R8RFile xmlFile = new R8RFile("convert", idName + ".xml");
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
            ImportExport myImportExport = new ImportExport(DataAccess.R8RDB.GetTable<Tbl>().Single(x => x.TblID == TblID));
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
            ActionProcessor Obj = new ActionProcessor();
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
                R8RFile tempFile = new R8RFile("temporary", idName + ".xml");
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
            ImportExport myImportExport = new ImportExport(DataAccess.R8RDB.GetTable<Tbl>().Single(x => x.TblID ==TblID));
            string errorMessage = "";
            bool IsValid = myImportExport.IsXmlValid(fileLocation, ref errorMessage);
            if (IsValid == false)
            {
                TblRowPopUp.MsgString = "The XML file could not be validated. The error message is: " + errorMessage;
                TblRowPopUp.Show();
                return;
            }

            Guid userID = (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            User theUser = DataAccess.R8RDB.GetTable<User>().Single(x => x.UserID == userID);

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
            ImportExport myImportExport = new ImportExport(DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == TblID));
            myImportExport.CreateXSDFile();
            R8RFile xsdFile = myImportExport.GetXSDFileReference();
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
        R8RFile exportFile = new R8RFile("export", idName + ".xml");
        exportFile.CreateTemporary();
        string xmlFileName = exportFile.GetPathToLocalFile();
        ImportExport myImportExport = new ImportExport(DataAccess.R8RDB.GetTable<Tbl>().Single(x => x.TblID == TblID));
        myImportExport.PerformExport(xmlFileName, theQuery, ChkIncValues.Checked);
        exportFile.DownloadToUserBrowser(Response);
        exportFile.DeleteTemporary();
    }



   
}
