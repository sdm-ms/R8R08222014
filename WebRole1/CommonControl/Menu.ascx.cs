using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Caching;

using Subgurim.Controls.GoogleMaps;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


public partial class CommonControl_Menu : System.Web.UI.UserControl
{
    StringBuilder myStringBuilder;
    R8RDataAccess myDataAccess;

    bool OpenAllInitially = false;
    RoutingInfoMainContent theLocation;
    string cacheString;


    protected void Page_Load(object sender, EventArgs e)
    {
        //String csname1 = "TopicsMenuScript";
        //// We must (1) use ScriptManager (rather than client side call) for this to work after partial page update; and
        //// (2) use window setTimeout trick to overcome bug with dropdownlists in Firefox 3.0.
        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), csname1, "window.setTimeout(\"treeSetup()\",0);", true);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        myDataAccess = new R8RDataAccess();
        try
        {
            theLocation = Routing.IncomingMainContent(Page.RouteData, myDataAccess.R8RDB);
        }
        catch
        {
            theLocation = new RoutingInfoMainContent();
        }
        cacheString = "CachedTopicsMenu" + theLocation.GetHashCode();
        object cachedObject = (string)CacheManagement.GetItemFromCache(cacheString);
        if (cachedObject == null)
            BuildMenu();
        else
            MenuLiteral.Text = (string)cachedObject;
    }

    protected void BuildMenu()
    {
        myStringBuilder = new StringBuilder();
        myStringBuilder.Append("<div id=\"mytopicstree\" class=\"topicstree\" >");
        myStringBuilder.Append("<ul>");
        myStringBuilder.Append(AddHierarchyItems(null));
        myStringBuilder.Append("</ul></div>");
        MenuLiteral.Text = myStringBuilder.ToString();
        string[] myDependencies = { "TopicsMenu" };
        CacheManagement.AddItemToCache(cacheString, myDependencies, MenuLiteral.Text);
    }

    protected void AppendHtmlAttribute(StringBuilder theStringBuilder, string attributeName, string attributeValue, bool mustBeNonempty)
    {
        if (mustBeNonempty && attributeValue == "")
            return;
        theStringBuilder.Append(" ");
        theStringBuilder.Append(attributeName);
        theStringBuilder.Append("=\"");
        theStringBuilder.Append(attributeValue);
        theStringBuilder.Append("\"");
    }

    protected string GetListItemText(string id, string text, string url, string containedList, bool keepOpen)
    {
        StringBuilder shortStringBuilder = new StringBuilder();
        shortStringBuilder.Append("<li");
        if ((OpenAllInitially || keepOpen) && containedList != "")
            shortStringBuilder.Append(" class=\"open\" ");
        AppendHtmlAttribute(shortStringBuilder, "id", id, true);
        shortStringBuilder.Append(">");
        if (url == "")
        {
            shortStringBuilder.Append("<span>");
            shortStringBuilder.Append(text);
            shortStringBuilder.Append("</span>");
        }
        else
        {
            shortStringBuilder.Append("<a");
            AppendHtmlAttribute(shortStringBuilder, "href", url, true);
            AppendHtmlAttribute(shortStringBuilder, "class", containedList != "" ? "tablemultiple" : "tablesingle", true);
            shortStringBuilder.Append(">");
            shortStringBuilder.Append(text);
            shortStringBuilder.Append("</a>");
        }
        if (containedList != "")
        {
            shortStringBuilder.Append("<ul>");
            shortStringBuilder.Append(containedList);
            shortStringBuilder.Append("</ul>");
        }
        shortStringBuilder.Append("</li>");
        return shortStringBuilder.ToString();
    }

    protected string AddHierarchyItems(HierarchyItem higherItem)
    {
        List<HierarchyItem> theHierarchyItems;
        if (higherItem == null)
            theHierarchyItems = myDataAccess.R8RDB.GetTable<HierarchyItem>().Where(x => x.ParentHierarchyItemID == null && x.IncludeInMenu).OrderBy(x => x.HierarchyItemName).ToList();
        else
            theHierarchyItems = myDataAccess.R8RDB.GetTable<HierarchyItem>().Where(x => x.ParentHierarchyItemID == higherItem.HierarchyItemID && x.IncludeInMenu).OrderBy(x => x.HierarchyItemName).ToList();
        string listItems = "";
        foreach (var item in theHierarchyItems)
        {
            listItems += AddHierarchyItems(item);
        }
        if (higherItem == null)
            return listItems;
        else
            return GetListItemText("LIHIER" + higherItem.HierarchyItemID.ToString(), higherItem.HierarchyItemName, higherItem.RouteToHere, listItems, (theLocation.theHierarchy == null) ? false : theLocation.theHierarchy.Contains(higherItem));
    }

}
