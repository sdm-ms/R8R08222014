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

using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class Main_Table_RatingGroupResolution : System.Web.UI.UserControl
{
    public Guid RatingGroupID { get; set; }
    public Guid UserID { get; set; }
    public bool CanResolve = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        Setup();
    }

    protected void Setup()
    {
        R8RDataManipulation DataAccess = new R8RDataManipulation();

        if (RatingGroupID == new Guid())
            throw new Exception("Rating group must be specified before rating resolution can be shown.");
        RatingGroup ratingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == RatingGroupID);
        bool ratingGroupIsConcluded = DataAccess.RatingGroupIsResolved(ratingGroup);
        RatingGroupResolution theResolution = DataAccess.DataContext.GetTable<RatingGroupResolution>()
            .Where(mg => mg.RatingGroupID == RatingGroupID)
            .OrderByDescending(mg => mg.ExecutionTime)
            .ThenByDescending(mg => mg.WhenCreated)
            .FirstOrDefault();

        string collapsedText = "Status: ";
        if (ratingGroupIsConcluded)
        {
            concludeRegion.Visible = false;
            undoConcludeRegion.Visible = true;
            if (theResolution != null)
            {
                collapsedText += "This table cell was concluded at " + ((DateTime)theResolution.ExecutionTime).ToString();
                if (theResolution.EffectiveTime.ToString() != theResolution.ExecutionTime.ToString())
                    collapsedText += ", retroactively effective at " + ((DateTime)theResolution.EffectiveTime).ToString();
                collapsedText += ".";
                ConcludeInfoShort.Text = "Status: Inactive";
            }
        }
        else
        {
            if (theResolution != null)
            {
                collapsedText += "This table cell was previously resolved, but that resolution was canceled at " + theResolution.ExecutionTime + ".";
            }
            else
                collapsedText += "This table cell remains active.";
            undoConcludeRegion.Visible = false;
            concludeRegion.Visible = true;
            ConcludeInfoShort.Text = "Status: Active";
        }
        ConcludeInfo.Text = collapsedText;
        // CollapsiblePanel.ExpandedText = "Status Details";
        if (!CanResolve)
        {
            concludeRegion.Visible = false;
            undoConcludeRegion.Visible = false;
        }
    }


    protected void RefreshPage()
    {
        R8RDataManipulation DataAccess = new R8RDataManipulation();
        RatingGroup theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == RatingGroupID);
        Routing.Redirect(Response, new RoutingInfoMainContent(theRatingGroup.TblRow.Tbl, theRatingGroup.TblRow, theRatingGroup.TblColumn));
    }

    protected void UndoConclude_Click(object sender, EventArgs e)
    {
        if (!CanResolve)
            return;
        R8RDataManipulation DataAccess = new R8RDataManipulation();
        ActionProcessor theActionProcessor = new ActionProcessor();
        RatingGroup theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == RatingGroupID);
        theActionProcessor.ResolveRatingGroup(theRatingGroup, true, true, false, TestableDateTime.Now, UserID, null);
        RefreshPage();
    }

    protected void Conclude_Click(object sender, EventArgs e)
    {
        if (!CanResolve)
            return;
        try
        {
            ActionProcessor theActionProcessor = new ActionProcessor();
            DateTime? timeOfResolution;
            if (TimingOptions.SelectedIndex == 0)
                timeOfResolution = TestableDateTime.Now;
            else
            {
                timeOfResolution = TheDateAndTime.theDateTime;
                if (timeOfResolution == null)
                    throw new Exception("A valid date and time must be specified.");
                else if (timeOfResolution > TestableDateTime.Now)
                    throw new Exception("You cannot specify a date in the future.");
                timeOfResolution = (DateTime)timeOfResolution;
            }
            bool assignPoints = PointsRule.SelectedIndex == 0;
            R8RDataManipulation DataAccess = new R8RDataManipulation();
            RatingGroup theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == RatingGroupID);
            theActionProcessor.ResolveRatingGroup(theRatingGroup, true, false, !assignPoints, timeOfResolution, UserID, null);
            RefreshPage();
        }
        catch (Exception ex)
        {
            PopUp.MsgString = ex.Message;
            PopUp.Show();
        }
    }
}
