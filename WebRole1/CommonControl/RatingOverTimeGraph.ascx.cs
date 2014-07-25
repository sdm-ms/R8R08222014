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

using System.Web.UI.DataVisualization.Charting;
using System.Globalization;
using System.Collections.Generic;

using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public class RatingOverTimeInfoForRenderControl
{
    public Guid? RatingGroupID;
    public Guid? SpecificRatingID;
}

public partial class RatingOverTimeGraph : System.Web.UI.UserControl
{
    public RatingOverTimeInfoForRenderControl theRatingOverTimeInfo { get; set; }

    internal bool SuppressDrilledInSeriesName = false;

    internal R8RDataAccess DataAccess = new R8RDataAccess();
    public Guid? RatingGroupID { get; set; }
    public Guid? SpecificRatingID { get; set; }
    public bool AxesBasedOnData;

    public void Manual_Setup(Guid? ratingGroupID, Guid? specificRatingID)
    {
        AxesBasedOnData = true;
        RatingGroupID = ratingGroupID;
        SpecificRatingID = specificRatingID;
        if (SpecificRatingID != null)
        {
            Rating specificRatingRequested = DataAccess.R8RDB.GetTable<Rating>().SingleOrDefault(m => m.RatingID == SpecificRatingID);
            RatingGroupID = specificRatingRequested.RatingGroupID;
        }
        ViewState["RatingGroupID"] = RatingGroupID;
        ViewState["SpecificRatingID"] = SpecificRatingID;
        Further_Setup();
    }

    // The following is necessary only when the chart is not being added to the control hierarchy and is being rendered with RenderControl.
    public void Manual_Setup_For_Chart_Being_Cached()
    {
        LoadTimeIntervals();
        LoadChart();
        TimeFrameDdl.Visible = false;
        BackButton.Visible = false;
        SuppressDrilledInSeriesName = true;
        DrilledInSeriesName.Text = "";
        DrilledInSeriesName.Visible = false;
    }

    protected void Automatic_Setup()
    {
        if (ViewState["SpecificRatingID"] != null)
        {
            SpecificRatingID = new Guid(ViewState["SpecificRatingID"].ToString());
            Rating specificRatingRequested = DataAccess.R8RDB.GetTable<Rating>().SingleOrDefault(m => m.RatingID == SpecificRatingID);
            RatingGroupID = specificRatingRequested.RatingGroupID;
            Further_Setup();
        }
        else if (ViewState["RatingGroupID"] != null)
        {
            RatingGroupID = new Guid(ViewState["RatingGroupID"].ToString());
            SpecificRatingID = null;
            Further_Setup();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (theRatingOverTimeInfo != null)
        {
            Manual_Setup(theRatingOverTimeInfo.RatingGroupID, theRatingOverTimeInfo.SpecificRatingID);
            Manual_Setup_For_Chart_Being_Cached();
        }
        else if (!Page.IsPostBack)
            LoadTimeIntervals();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        LoadChart(); // Wait until everything else has happened to do the real work.
    }


    protected void Further_Setup()
    {

        if (RatingGroupID == null)
            return;

        RatingGroup theRatingGroup = DataAccess.R8RDB.GetTable<RatingGroup>().SingleOrDefault(mg => mg.RatingGroupID == RatingGroupID);
        if (theRatingGroup == null)
            return;

        bool canViewPage = DataAccess.CheckUserRights((Guid?)(Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID"), UserActionType.View, false, null, theRatingGroup.TblRow.TblID);
        if (!canViewPage)
            Routing.Redirect(Response, new RoutingInfo(RouteID.Login));

    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        if (SpecificRatingID != null)
        {
            Rating theRating = DataAccess.R8RDB.GetTable<Rating>().Single(m => m.RatingID == SpecificRatingID);
            RatingGroupID = theRating.RatingGroupID;
            SpecificRatingID = null;
        }
        else
        {
            Rating theRating = DataAccess.R8RDB.GetTable<Rating>().SingleOrDefault(m => m.OwnedRatingGroupID == RatingGroupID);
            if (theRating != null)
            {
                RatingGroupID = theRating.RatingGroupID;
                SpecificRatingID = null;
            }
        }
        ViewState["RatingGroupID"] = RatingGroupID;
        ViewState["SpecificRatingID"] = SpecificRatingID;
    }

    protected void TimeFrameDdl_SelectedIndexChanged(object sender, EventArgs e)
    {
        Automatic_Setup();
    }

    public class TimeFrame
    {
        public string Name {get ; set;}
        public string TheTimeSpan {get; set;}
    }

    protected void LoadTimeIntervals()
    {
        List<TimeFrame> theTimeFrames = new List<TimeFrame> { 
            new TimeFrame { Name = "Lifetime", TheTimeSpan = (5000*24*60*60).ToString() },
            new TimeFrame { Name = "Last 90 days", TheTimeSpan = (90*24*60*60).ToString() },
            new TimeFrame { Name = "Last 30 days", TheTimeSpan = (30*24*60*60).ToString() },
            new TimeFrame { Name = "Last 7 days", TheTimeSpan = (7*24*60*60).ToString() },
            new TimeFrame { Name = "Last day", TheTimeSpan = (24*60*60).ToString() },
            new TimeFrame { Name = "Last hour", TheTimeSpan = (60*60).ToString() },
            new TimeFrame { Name = "Last 30 minutes", TheTimeSpan = (30*60).ToString() },
            new TimeFrame { Name = "Last 15 minutes", TheTimeSpan = (15*60).ToString() },
            new TimeFrame { Name = "Last minute", TheTimeSpan = (60).ToString() }
        };
        TimeFrameDdl.DataSource = theTimeFrames;
        TimeFrameDdl.DataBind();
    }

    protected void LoadBackButton()
    {
        if (SpecificRatingID != null)
        {
            BackButton.Visible = true;
        }
        else
        {
            Rating theOwningRating = DataAccess.R8RDB.GetTable<Rating>().SingleOrDefault(m => m.OwnedRatingGroupID == RatingGroupID);
            if (theOwningRating == null)
                BackButton.Visible = false;
            else
            {
                BackButton.Visible = true;
            }
        }
    }

    protected void ClearChart()
    {
        var existingSeries = Chart1.Series.ToList();
        foreach (var existingSer in existingSeries)
            Chart1.Series.Remove(existingSer);
    }

    protected void LoadChart()
    {
        LoadBackButton();
        ClearChart();
        
        int numberSeconds = Convert.ToInt32(TimeFrameDdl.SelectedValue);
        TimeSpan theTimeSpan;
        DateTime firstDateTime;
        theTimeSpan = new TimeSpan(0, 0, numberSeconds);
        firstDateTime = TestableDateTime.Now - theTimeSpan;

        // See if this has been resolved (if this is set to lifetime)
        DateTime lastDateTime = TestableDateTime.Now;
        if (numberSeconds == 5000 * 24 * 60 * 60)
        {
            Guid topmostRatingGroupID = DataAccess.R8RDB.GetTable<Rating>().First(m => m.RatingGroupID == RatingGroupID).TopmostRatingGroupID;
            RatingGroupResolution theResolution = DataAccess.R8RDB.GetTable<RatingGroupResolution>()
                .Where(mg => mg.RatingGroupID == topmostRatingGroupID)
                .OrderByDescending(mg => mg.ExecutionTime)
                .ThenByDescending(mg => mg.WhenCreated)
                .FirstOrDefault();
            if (theResolution != null && theResolution.CancelPreviousResolutions == false)
                lastDateTime = theResolution.EffectiveTime;
        }

        var theUserRatingData = DataAccess.R8RDB.GetTable<UserRating>()
            .Where(p => p.Rating.RatingGroupID == RatingGroupID && p.UserRatingGroup.WhenCreated >= firstDateTime)
            .Where(p => SpecificRatingID == null || p.RatingID == SpecificRatingID)
            .Where(p => lastDateTime == null || p.UserRatingGroup.WhenCreated <= lastDateTime)
            .Where(p => p.NewUserRating != null)
            .Select(p => new { RatingID = p.Rating.RatingID, NumInGroup = p.Rating.NumInGroup, SeriesName = p.Rating.Name, OwnedRatingGroupID = p.Rating.OwnedRatingGroupID, Date = p.UserRatingGroup.WhenCreated, Value = (decimal) p.NewUserRating })
            .OrderBy(p => p.NumInGroup)
            .ThenBy(p => p.Date)
            .ToList();

        if (!theUserRatingData.Any())
        { // No predictions in this time -- add most recent one.
            theUserRatingData = DataAccess.R8RDB.GetTable<Rating>()
               .Where(p => p.RatingGroupID == RatingGroupID)
               .Where(p => SpecificRatingID == null || p.RatingID == SpecificRatingID)
               .Select(p => new { RatingID = p.RatingID, NumInGroup = p.NumInGroup, SeriesName = p.Name, OwnedRatingGroupID = p.OwnedRatingGroupID, Date = firstDateTime, Value = p.CurrentValue ?? 0 })
               .OrderBy(p => p.NumInGroup)
               .ThenBy(p => p.Date)
               .ToList();
        }

        var theSerieses = from p in theUserRatingData
                          group p by new { p.RatingID, p.SeriesName, p.NumInGroup, p.OwnedRatingGroupID } into mySeries
                          where mySeries.Count() > 0
                          select new { SeriesInfo = mySeries.Key, NumInSeries = mySeries.Count(), UserRatingData = mySeries };

        if (!AxesBasedOnData)
        {
            var theRatings = DataAccess.R8RDB.GetTable<Rating>().Where(m => m.RatingGroupID == RatingGroupID);
            decimal minPermissibleValue = theRatings.Select(m => m.RatingCharacteristic.MinimumUserRating).Min();
            decimal maxPermissibleValue = theRatings.Select(m => m.RatingCharacteristic.MaximumUserRating).Max();
            Chart1.ChartAreas["ChartArea1"].AxisY.Minimum = (double)minPermissibleValue;
            Chart1.ChartAreas["ChartArea1"].AxisY.Maximum = (double)maxPermissibleValue;
        }

        //Chart1.ChartAreas["ChartArea1"].AxisX.Minimum = firstDateTime.ToOADate();
        //Chart1.ChartAreas["ChartArea1"].AxisX.Minimum = TestableDateTime.Now.ToOADate();

        int maxNumberPoints = 0;
        if (theSerieses.Any())
            maxNumberPoints = theSerieses.Select(x => x.NumInSeries).Max();

        //DateTime earliestDate = firstDateTime;
        //if (theUserRatingData.Any())
        //    earliestDate = theUserRatingData.Select(x => x.Date).Min();
        //if (earliestDate > firstDateTime)
        //{
        //    firstDateTime = earliestDate;
        //    theTimeSpan = TestableDateTime.Now - firstDateTime;
        //}

        foreach (var aSeries in theSerieses.ToList())
        {
            Guid theRatingID = aSeries.SeriesInfo.RatingID;
            decimal? theEarlierValue = null;
            UserRating theEarlierUserRating = DataAccess.R8RDB.GetTable<UserRating>()
                .Where(p => p.Rating.RatingGroupID == RatingGroupID && p.UserRatingGroup.WhenCreated < firstDateTime && p.RatingID == theRatingID)
                .OrderByDescending(p => p.UserRatingGroup.WhenCreated).FirstOrDefault();
            if (theEarlierUserRating != null)
                theEarlierValue = theEarlierUserRating.NewUserRating;

            Series series = new Series(aSeries.SeriesInfo.SeriesName);
            var xValues = aSeries.UserRatingData.Select(x => x.Date).ToList();
            if (theEarlierValue != null)
                xValues.Insert(0, firstDateTime); // Add a point for beginning of time period
            xValues.Add(lastDateTime); // add a point for the present (or resolution time)
            var yValues = aSeries.UserRatingData.Select(y => y.Value).ToList();
            if (theEarlierValue != null)
                yValues.Insert(0, (decimal)theEarlierValue);
            yValues.Add(yValues[yValues.Count - 1]); // the current prediction is still the last new one
            if (maxNumberPoints > 200)
                series.ChartType = SeriesChartType.FastLine;
            else
                series.ChartType = SeriesChartType.StepLine;
            series.BorderWidth = 3;
            series.ShadowOffset = 2;
            series.Points.DataBindXY(xValues, yValues);

            // Add series into the chart's series Tbl
            Chart1.Series.Add(series);

        }

        DrilledInSeriesName.Visible = false;
        if (theSerieses.Count() <= 1)
        { // don't use a legend for just one series   
            if (Chart1.Legends.Any())
            {
                var theLegend = Chart1.Legends.First();
                Chart1.Legends.Remove(theLegend);
            }
            if (theSerieses.Count() == 1)
            {
                if (!SuppressDrilledInSeriesName)
                {
                    DrilledInSeriesName.Text = theSerieses.FirstOrDefault().SeriesInfo.SeriesName;
                    DrilledInSeriesName.Visible = true;
                }
            }
        }
        else
        { // Add tooltips to allow drilling down
            
            var seriesWithOwnedGroups = theSerieses.Where(x => x.SeriesInfo.OwnedRatingGroupID != null);
            foreach (var ownerSeries in seriesWithOwnedGroups)
            {
                Chart1.Series[ownerSeries.SeriesInfo.SeriesName].LegendPostBackValue = "MG," + ownerSeries.SeriesInfo.OwnedRatingGroupID;
                string toolTip = "Click for more detail on " + ownerSeries.SeriesInfo.SeriesName;
                Chart1.Series[ownerSeries.SeriesInfo.SeriesName].LegendToolTip = toolTip;
            }

            var seriesWithoutOwnedGroups = theSerieses.Where(x => x.SeriesInfo.OwnedRatingGroupID == null);
            foreach (var thisSeries in seriesWithoutOwnedGroups)
            {
                Chart1.Series[thisSeries.SeriesInfo.SeriesName].LegendPostBackValue = "M," + thisSeries.SeriesInfo.RatingID + "," + RatingGroupID;
                string toolTip = "Click to view only " + thisSeries.SeriesInfo.SeriesName;
                Chart1.Series[thisSeries.SeriesInfo.SeriesName].LegendToolTip = toolTip;
            }
        }

        if (theTimeSpan < new TimeSpan(5, 0, 0))
            Chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "g";
        else
            Chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "d";

        // Chart1.ChartAreas["ChartArea1"].AxisX.TextOrientation = TextOrientation.Horizontal;
        Chart1.ChartAreas["ChartArea1"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
    }

    protected void Chart1_Click(object sender, ImageMapEventArgs e)
    {
        string[] theStrings = e.PostBackValue.Split(',');
        if (theStrings[0] == "MG")
        {
            RatingGroupID = new Guid(theStrings[1]);
            SpecificRatingID = null;
        }
        else
        {
            SpecificRatingID = new Guid(theStrings[1]);
            RatingGroupID = new Guid(theStrings[2]);
        }
        ViewState["RatingGroupID"] = RatingGroupID;
        ViewState["SpecificRatingID"] = SpecificRatingID;
        //LoadChart();
    }
}
