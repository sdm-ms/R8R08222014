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



public partial class CommonControl_ItemPath : System.Web.UI.UserControl
{
    public HierarchyItem theHierarchyItem { get; set; }
    public Tbl theTbl { get; set; }
    public TblRow theTblRow { get; set; }
    public TblColumn theTblColumn { get; set; }
    public Rating theRating { get; set; }
    public bool WithHtml = true;
    public string TheItemPathString;
    public bool SuppressTable = false;
    public bool SuppressRow = false;

    public CommonControl_ItemPath()
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PMItemPath theItemPath = new PMItemPath();
        if (theHierarchyItem != null)
            theItemPath.Setup(theHierarchyItem);
        else
            theItemPath.Setup(theTbl, theTblRow, theRating, theTblColumn);
        theItemPath.Suppress(SuppressTable, SuppressRow);
        TheItemPathString = theItemPath.GetItemPath(WithHtml);
        TheLiteral.Text = TheItemPathString;
    }
}
