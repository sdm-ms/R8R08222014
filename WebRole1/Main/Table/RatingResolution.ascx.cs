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

using ClassLibrary1.Misc;
using ClassLibrary1.Model;

public partial class Main_Table_RatingGroupResolution : System.Web.UI.UserControl
{
    public int RatingGroupID { get; set; }
    public int UserID { get; set; }
    public bool CanResolve = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        Setup();
    }

    protected void Setup()
    {
        RaterooDataManipulation DataAccess = new RaterooDataManipulation();

        if (RatingGroupID == 0)
            throw new Exception("Rating group must be specified before rating resolution can be shown.");
        RatingGroup ratingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == RatingGroupID);
        bool ratingGroupIsConcluded = DataAccess.RatingGroupIsResolved(ratingGroup);
        RatingGroupResolution theResolution = DataAccess.DataContext.GetTable<RatingGroupResolution>()
            .Where(mg => mg.RatingGroupID == RatingGroupID)
            .OrderByDescending(mg => mg.ExecutionTime)
            .ThenByDescending(mg => mg.RatingGroupResolutionID)
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
        RaterooDataManipulation DataAccess = new RaterooDataManipulation();
        RatingGroup theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == RatingGroupID);
        PMRouting.Redirect(Response, new PMRoutingInfoMainContent(theRatingGroup.TblRow.Tbl, theRatingGroup.TblRow, theRatingGroup.TblColumn));
    }

    protected void UndoConclude_Click(object sender, EventArgs e)
    {
        if (!CanResolve)
            return;
        RaterooDataManipulation DataAccess = new RaterooDataManipulation();
        PMActionProcessor theActionProcessor = new PMActionProcessor();
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
            PMActionProcessor theActionProcessor = new PMActionProcessor();
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
            RaterooDataManipulation DataAccess = new RaterooDataManipulation();
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
