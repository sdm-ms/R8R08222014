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
using System.Text;
using ClassLibrary1.Model;


public partial class User_Control_InsertableContent : System.Web.UI.UserControl
{
    int? DomainID { get; set; }
    int? PointsManagerID { get; set; }
    int? TblID { get; set; }
    InsertableLocation Location { get; set; }
    public bool ContainsContent { get; internal set; }

    internal R8RDataAccess DataAccess;

    public void Setup(int? domainID, int? pointsManagerID, int? tblID, InsertableLocation location, R8RDataAccess theDataAccess)
    {
        DomainID = domainID;
        PointsManagerID = pointsManagerID;
        TblID = tblID;
        Location = location;
        DataAccess = theDataAccess;
        LoadContent();
    }

    protected void LoadContent()
    {
        string cacheKey = DomainID.ToString() + "," + PointsManagerID.ToString() + "," + TblID.ToString() + "," + Location.ToString();
        string theContent;
        theContent = (string) CacheManagement.GetItemFromCache(cacheKey);
        if (theContent == null)
        {
            if (DataAccess == null)
                DataAccess = new R8RDataAccess();
            theContent = GetContentString();
            string[] dependencies = { "InsertableContent" };
            CacheManagement.AddItemToCache(cacheKey, dependencies, theContent);
        }
        ContainsContent = theContent != "";
        if (ContainsContent)
            AnnouncementDiv.InnerHtml = theContent;
    }

    internal List<InsertableContent> GetInsertableContents()
    {
        int totalSoFar = 0;
        var TblContent = DataAccess.R8RDB.GetTable<InsertableContent>().Where(
                x => x.TblID == TblID
                && x.Location == (short) this.Location
                && x.Status == (byte)StatusOfObject.Active).ToList();
        totalSoFar += TblContent.Count();
        var universeContent = DataAccess.R8RDB.GetTable<InsertableContent>().Where(
                x => x.PointsManagerID == PointsManagerID
                && (!x.Overridable || totalSoFar == 0)
                && x.Location == (short) this.Location
                && x.Status == (byte)StatusOfObject.Active).ToList();
        totalSoFar += universeContent.Count();
        var domainContent = DataAccess.R8RDB.GetTable<InsertableContent>().Where(
                x => x.DomainID == DomainID
                && (!x.Overridable || totalSoFar == 0)
                && x.Location == (short) this.Location
                && x.Status == (byte)StatusOfObject.Active).ToList();
        totalSoFar += domainContent.Count();
        var everywhereContent = DataAccess.R8RDB.GetTable<InsertableContent>().Where(
                x => x.TblID == null && x.PointsManagerID == null && x.DomainID == null
                && (!x.Overridable || totalSoFar == 0)
                && x.Location == (short) this.Location
                && x.Status == (byte)StatusOfObject.Active).ToList();
        List<InsertableContent> theList = everywhereContent.Concat(domainContent).Concat(universeContent).Concat(TblContent).ToList();
        return theList;
    }

    internal string GetContentString()
    {
        StringBuilder theBuilder = new StringBuilder();
        List<InsertableContent> theList = GetInsertableContents();
        foreach (InsertableContent theContent in theList)
        {
            if (theContent.IsTextOnly)
            {
                theBuilder.Append("<span>");
                theBuilder.Append(theContent.Content);
                theBuilder.Append("</span>");
            }
            else
                theBuilder.Append(theContent.Content);
            if (theContent != theList[theList.Count - 1])
                theBuilder.Append("<br>");
        }
        string theString = theBuilder.ToString();
        return theString;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

}