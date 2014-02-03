using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MoreStrings;
using System.Text;
using System.Web.Routing;
using System.Diagnostics;
using ClassLibrary1.Model;

namespace WebApplication1.CommonControl
{
    public partial class PMSlideshow : System.Web.UI.UserControl
    {
        public PMRoutingInfoMainContent theLocation;
        public IRaterooDataContext RaterooDB;
        public void Setup(PMRoutingInfoMainContent location, IRaterooDataContext theDataContext)
        {
            theLocation = location ;
            RaterooDB = theDataContext;
        }

        public class TblRowRatingSummary
        {
            public TblRow theTblRow;
            public List<CategoryAndRating> theInfo;
        }

        protected List<TblRowRatingSummary> GetRatingsOfBestData()
        {
            List<TblRow> theTblRows = PickTblRows.GetMostHighlyRatedTblRowsFromVariousTbls(RaterooDB,theLocation, 7, 8, 4);
            if (theTblRows == null || !theTblRows.Any())
                return new List<TblRowRatingSummary>();
            List<TblRowRatingSummary> entityRatingSummary =
                (from e in theTblRows
                 let mgs = e.RatingGroups
                        .Where(
                         mg => mg.TblColumn.Status == (int) (StatusOfObject.Active) && 
                                mg.TblColumn.TblTab == e.Tbl.TblTabs.
                                 OrderBy(cg => cg.NumInTbl)
                                 .First() && mg.RatingGroupAttribute.Name.StartsWith("Rating"))
                                 .Select(y => new CategoryAndRating { 
                                     Category = y.TblColumn.Name, 
                                     ColumnID = y.TblColumnID,
                                     Rating = y.CurrentValueOfFirstRating ?? 0})
                                 .ToList()
                 select new TblRowRatingSummary {
                     theTblRow = e,
                     theInfo = mgs
                 }).ToList();
            foreach (var summary in entityRatingSummary)
            {
                foreach (var item in summary.theInfo)
                {
                    item.Rating = Convert.ToDecimal(PMNumberandTableFormatter.FormatAsSpecified(item.Rating, 1, item.ColumnID));
                }
            }
            return entityRatingSummary.ToList();
        }

        protected List<RatingGroup> GetRatingOverTimesData()
        {
            RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
            List<TblRow> theTblRows = PickTblRows.GetMostActiveTblRowsFromVariousTbls(RaterooDB, VolatilityDuration.oneDay, theLocation, 7, 8, 2);
            if (theTblRows == null || !theTblRows.Any())
                return new List<RatingGroup>();
            List<RatingGroup> ratingGroupList = 
                (from e in theTblRows
                let mgDefault = e.RatingGroups.SingleOrDefault(
                        mg => mg.TblColumnID ==
                            e.Tbl.TblTabs.
                                OrderBy(cg => cg.NumInTbl)
                                .First().DefaultSortTblColumnID)
                let mgMostActive = e.RatingGroups.OrderByDescending(mg => mg.VolatilityTrackers.Single(x => x.DurationType == (int) VolatilityDuration.oneDay).Pushback).FirstOrDefault()
                let mgBest = mgMostActive ?? mgDefault
                where mgBest != null
                select mgBest
                ).ToList();               
            return ratingGroupList;
        }

        [DebuggerStepThrough]
        public string GetRatingsOfBestGraphAsString(TblRowRatingSummary theSummary)
        {
            string cacheString = "RatingsOfBest" + theLocation.GetHashString() + theSummary.GetHashString();
            try
            {
                string theRatingOfBestString = PMCacheManagement.GetItemFromCache(cacheString) as string;
                if (theRatingOfBestString == null || theRatingOfBestString == "")
                {
                    //System.Diagnostics.Trace.TraceInformation("Ratings of Best Graph " );
                    string theGraphString;
                    try
                    {
                        // This works sometimes, but for mysterious reasons not always, so we have a backup. Note that we still need to run this code, since it produces the information we need in the catch loop.
                        theGraphString = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/CommonControl/RatingsSummaryGraph.ascx", "TheInfo", theSummary.theInfo);

                        //Control theControl = LoadControl("~/CommonControl/RatingsSummaryGraph.ascx");
                        //RatingsSummaryGraph theGraph = theControl as RatingsSummaryGraph;
                        //theGraph.Manual_Setup(theSummary.theInfo);

                        //System.Diagnostics.Trace.TraceInformation("RenderControl approach successful " + HttpContext.Current.Cache["MostRecentChartKey"] as string);
                    }
                    catch
                    {
                        // Super-hacky approach: Given that we can't seem to render the control, we just build the img element on the fly, using a cached indication of
                        // the name of the file that was produced, by placing this name in the cache in the blob image handler.
                        // <img style="height: 296px; width: 430px; border-width: 0px;" alt="" src="/ChartImg.axd?i=chart_0_9.png&amp;g=f6e6b12e1d29418e822df963737ef61d" id="Chart1">
                        string theBlobKey = HttpContext.Current.Cache["MostRecentChartKey"] as string;
                        theGraphString = String.Format("<img style=\"height: 296px; width: 430px; border-width: 0px;\" alt=\"\" src=\"/ChartImg.axd?i={0}\" id=\"C{0}\">", theBlobKey);
                        //System.Diagnostics.Trace.TraceInformation("theGraphString for " + HttpContext.Current.Cache["MostRecentChartKey"] as string + ": " + theGraphString);
                    }
                    theRatingOfBestString = (GetImageString(theSummary.theTblRow) ?? "") + theGraphString;
                    PMCacheManagement.AddItemToCache(cacheString, new string[] { }, theRatingOfBestString, new TimeSpan(0, 10, 0));
                }
                //System.Diagnostics.Trace.TraceInformation("chart " + theRatingOfBestString);
                return theRatingOfBestString;
            }
            catch
            {
                return "";
            }
        }

        [DebuggerStepThrough]
        public string GetRatingOverTimeGraphAsString(RatingGroup theRatingGroup)
        {
            try
            {
                string cacheString = "RatingOverTime" + theLocation.GetHashString() + theRatingGroup.RatingGroupID;
                string theRatingOverTimeString = PMCacheManagement.GetItemFromCache(cacheString) as string;
                if (theRatingOverTimeString == null || theRatingOverTimeString == "")
                {
                    //System.Diagnostics.Trace.TraceInformation("Ratings over Time Graph ");
                    string theGraphString;
                    try
                    {
                        // This works sometimes, but for mysterious reasons not always, so we have a backup.

                        RatingOverTimeInfoForRenderControl theInfo = new RatingOverTimeInfoForRenderControl();
                        theInfo.RatingGroupID = theRatingGroup.RatingGroupID;
                        theGraphString = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/CommonControl/RatingOverTimeGraph.ascx", "theRatingOverTimeInfo", theInfo);

                        //Control theControl = LoadControl("~/CommonControl/RatingOverTimeGraph.ascx");
                        //RatingOverTimeGraph theGraph = theControl as RatingOverTimeGraph;
                        //theGraph.Manual_Setup(theRatingGroup.RatingGroupID, null);
                        //theGraph.Manual_Setup_For_Chart_Being_Cached();
                        //theGraphString = theGraph.MyRenderControl();
                        //System.Diagnostics.Trace.TraceInformation("RenderControl approach successful " + HttpContext.Current.Cache["MostRecentChartKey"] as string);
                    }
                    catch
                    {
                        // Super-hacky approach: Given that we can't seem to render the control, we just build the img element on the fly, using a cached indication of
                        // the name of the file that was produced, by placing this name in the cache in the blob image handler.
                        // <img style="height: 296px; width: 430px; border-width: 0px;" alt="" src="/ChartImg.axd?i=chart_0_9.png&amp;g=f6e6b12e1d29418e822df963737ef61d" id="Chart1">
                        string theBlobKey = HttpContext.Current.Cache["MostRecentChartKey"] as string;
                        theGraphString = String.Format("<img style=\"height: 296px; width: 430px; border-width: 0px;\" alt=\"\" src=\"/ChartImg.axd?i={0}\" id=\"C{0}\">", theBlobKey);
                        //System.Diagnostics.Trace.TraceInformation("theGraphString for " + HttpContext.Current.Cache["MostRecentChartKey"] as string + ": " + theGraphString);
                    }

                    theRatingOverTimeString = (GetImageString(theRatingGroup.TblRow) ?? "") + theGraphString;
                    PMCacheManagement.AddItemToCache(cacheString, new string[] { }, theRatingOverTimeString, new TimeSpan(0, 10, 0));
                }
                return theRatingOverTimeString;
            }
            catch
            {
                return "";
            }
        }

        public string GetImageString(TblRow theTblRow)
        {
            FieldsDisplaySettingsMask myMask = new FieldsDisplaySettingsMask();
            var theTextField = theTblRow.Fields
                            .Where(f => (f.FieldDefinition.DisplayInPopupSettings & myMask.DisplayInTopRightCorner) == myMask.DisplayInTopRightCorner)
                            .SelectMany(x => x.TextFields)
                            .FirstOrDefault(x => x.Link.EndsWith("jpg") || x.Link.EndsWith("jpeg") || x.Link.EndsWith("gif") || x.Link.EndsWith("png") || x.Link.EndsWith("bmp") || x.Link.Contains("yimg.com/nimage"));
            if (theTextField != null)
            {
                string linkName;
                if (theTextField.Link.StartsWith("http://"))
                    linkName = theTextField.Link;
                else
                    linkName = "http://" + theTextField.Link;
                StringBuilder imageForProminentDisplay = new StringBuilder();
                imageForProminentDisplay.Append("<img src=\"");
                imageForProminentDisplay.Append(linkName);
                imageForProminentDisplay.Append("\" class=\"imageInField imageFloat picInSlideshow\" ");
                imageForProminentDisplay.Append(" style=\"picInSlideshow" + 180.ToString() +  "px; max-height:" + 300.ToString() + "px;\" ");
                imageForProminentDisplay.Append(" />");
                return imageForProminentDisplay.ToString();
             }
            return null;
        }

        public string GetItemPathString(RatingGroup theRatingGroup)
        {
            PMItemPath theItemPath = new PMItemPath();
            theItemPath.Setup(null, theRatingGroup.TblRow,null,theRatingGroup.TblColumn);
            string theItemPathString = "<div class=\"headTxt headTxtSmaller\" >" + theItemPath.GetItemPath(true) + "</div>";
            return theItemPathString;
        }

        public string GetItemPathString(TblRow theTblRow)
        {
            PMItemPath theItemPath = new PMItemPath();
            theItemPath.Setup(null, theTblRow, null, null);
            string theItemPathString = "<div class=\"headTxt headTxtSmaller\" >" + theItemPath.GetItemPath(true) + "</div>";
            return theItemPathString;
        }

        public string GetSingleImage(string src)
        {
            string theString = "<table><tr ><td><img src=\"{0}\"/></td></tr></table>";
            theString = String.Format(theString, src);
            return theString;
        }

        public string GetImageWithHeading(string headString, string src)
        {
            return GetCombinedItemString(headString, GetSingleImage(src));
        }

        public string GetThreeStepString(string image1, string step1, string image2, string step2, string image3, string step3)
        {
            string theString = "<table id=\"description\" class=\"borderlessSpacious mainPresentationText\" ><tr ><td><img src=\"{0}\"  /></td><td>{1}</td></tr><tr><td><img src=\"{2}\" /></td><td>{3}</td></tr><tr><td><img src=\"{4}\" /></td><td>{5}</td></tr></table>";
            theString = String.Format(theString, image1, step1, image2, step2, image3, step3);
            return theString;
        }

        public string GetThreeStepString(string step1, string step2, string step3)
        {
            return GetThreeStepString("images/step01_btn.jpg", step1, "images/step02_btn.jpg", step2, "images/step03_btn.jpg", step3);
        }

        public string GetOverallInstructions()
        {
            string step1 = "Select a topic from the list at left to view ratings.";
            string step2 = "To enter your own ratings, log in. Then, click on any rating, enter your rating, and click the check mark.";
            string step3 = "If others agree with your ratings, you&rsquo;ll win points, making you eligible for specified cash prizes.";
            string threeStep = GetThreeStepString(step1, step2, step3);
            return GetCombinedItemString("Welcome to Rateroo!", threeStep);
        }


        public string EnterRatingsInstructions()
        {
            string step1 = "Create an account and log in.";
            string step2 = "Click on any table cell you think is wrong.";
            string step3 = "Enter your rating, and click the check mark.";
            string threeStep = GetThreeStepString("images/login.png", step1, "images/rate1.png", step2, "images/rate3.png",  step3);
            return GetCombinedItemString("How to Enter Ratings", threeStep);
        }

        public string MakeMoneyInstructions()
        {
            string step1 = "Find a topic that interests you. Points you earn make you eligible for cash!";
            string step2 = "You can earn points by entering accurate ratings that other users agree with.";
            string step3 = "Once Rateroo trusts you, you can also earn points by improving the database and rating others' changes.";
            string threeStep = GetThreeStepString(step1, step2, step3);
            return GetCombinedItemString("How to Earn Money on Rateroo", threeStep);
        }


        public string UnderstandingPointsInstructions()
        {
            string step1 = "Whether you win or lose points depends on the rating at later points in time.";
            string step2 = "If the later rating is closer to your rating than to the rating before yours, you win points.";
            string step3 = "Untrusted ratings do not count in determining whether you win points.";
            string threeStep = GetThreeStepString("images/yellow_star.jpg", step1, "images/yellow_star.jpg", step2, "images/yellow_star.jpg", step3);
            return GetCombinedItemString("Understanding Rateroo Points", threeStep);
        }

        public string UnderstandingRaterooTrust()
        {
            string step1 = "Ratings with an asterisk (*) are by untrusted users.";
            string step2 = "When trusted users enters the same or a similar rating, the asterisk disappears, and both may win points if the rating sticks.";
            string step3 = "Untrusted users can convince trusted users by entering comments explaining their ratings.";
            string threeStep = GetThreeStepString("images/yellow_star.jpg", step1, "images/yellow_star.jpg", step2, "images/yellow_star.jpg", step3);
            return GetCombinedItemString("Trusted & Untrusted Ratings", threeStep);
        }


        public string HighStakesRatings()
        {
            string step1 = "Some table rows are randomly selected for peer review. You can use the Filter menu to view these rows.";
            string step2 = "Ratings made during this period -- and even more so, just before this period -- will result in larger points won or lost.";
            string step3 = "Your total points may depend a lot on the few ratings you make just before a table row is selected for peer review.";
            string threeStep = GetThreeStepString("images/yellow_star.jpg", step1, "images/yellow_star.jpg", step2, "images/yellow_star.jpg", step3);
            return GetCombinedItemString("Peer Review Ratings", threeStep);
        }

        public string RatingTips()
        {
            string step1 = "You'll generally get the most points by carefully rating things that haven't been rated yet.";
            string step2 = "Move existing ratings only a little bit, unless you're confident others will agree.";
            string step3 = "Look at the comments to see whether ratings that seem strange actually make sense.";
            string threeStep = GetThreeStepString("images/yellow_star.jpg", step1, "images/yellow_star.jpg", step2, "images/yellow_star.jpg", step3);
            return GetCombinedItemString("Rating Tips", threeStep);
        }

        public string ImproveDatabaseInstructions()
        {
            string step1 = "Once Rateroo trusts you, you'll see buttons for adding new rows or changing information on existing ones.";
            string step2 = "Some changes you make will be randomly selected for other users to rate in the Changes table.";
            string step3 = "Those ratings determine how many points you'll earn or lose for the changes you've made.";
            string threeStep = GetThreeStepString("images/yellow_star.jpg", step1, "images/yellow_star.jpg", step2, "images/yellow_star.jpg", step3);
            return GetCombinedItemString("How to Improve the Rateroo Database", threeStep);
        }

        public List<string> GetAllInstructions()
        {
            return new List<string>() { GetImageMain(), EnterRatingsInstructions(), RatingTips(), MakeMoneyInstructions(), UnderstandingPointsInstructions(), HighStakesRatings(), UnderstandingRaterooTrust(), ImproveDatabaseInstructions() };
        }

        public string GetImageMain()
        {
            return GetImageWithHeading("Welcome to Rateroo!", "images/logolarge.png");
        }

        public List<string> GetImages()
        {
            return new List<string>() { GetImageMain() };
        }

        public string GetSubtablesList()
        {
            string renderedControl = MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/CommonControl/SubtablesList.ascx", "subtablesDataNeeded", new SubtablesList.SubtablesDataNeeded { location = theLocation, theDataContext = RaterooDB });
            return GetCombinedItemString(theLocation.lastItemInHierarchy.FullHierarchyWithHtml, renderedControl);
        }

        public string GetCombinedItemString(string theHead, string theBody)
        {
            if (theBody == null || theBody == "" || theHead == null || theHead == "")
                return "";
            return "<div style=\"width:630px;\"><div class=\"headTxt headTxtEffect\" >" + theHead + "</div><div class=\"afterHeadTxtEffect\">" + theBody + "</div></div>";
        }

        public List<string> GetRatingsOfBestGraphsAsStrings()
        {
            List<string> theStrings = GetRatingsOfBestData().Select(x => "<div>" + GetCombinedItemString("What&rsquo;s on Top", GetItemPathString(x.theTblRow) + GetRatingsOfBestGraphAsString(x)) + "</div>").Where(x => x != "<div></div>").ToList();
            return theStrings;
        }

        public List<string> GetRatingOverTimeGraphsAsStrings()
        {
            List<string> theStrings = GetRatingOverTimesData().Select(x => "<div>" + GetCombinedItemString("What&rsquo;s Up Or Down", GetItemPathString(x) + GetRatingOverTimeGraphAsString(x)) + "</div>").Where(x => x != "<div></div>").ToList();
            return theStrings;
        }

        public string AssembleSlideShowHtml()
        {
            List<string> basicContent;
            if (theLocation == null)
                basicContent = GetAllInstructions();
            else
                basicContent = new List<string> { GetSubtablesList() };
            List<string> mostVolatileRatingsContent = GetRatingOverTimeGraphsAsStrings();
            List<string> ratingsOfBestContent = GetRatingsOfBestGraphsAsStrings();
            List<string> mergedContent = PickTblRows.MergeLists(new List<List<string>> { mostVolatileRatingsContent, ratingsOfBestContent});
            List<string> mergedContent2 = PickTblRows.MergeLists(new List<List<string>> { basicContent, mergedContent});
            StringBuilder myStringBuilder = new StringBuilder();
            myStringBuilder.Append("<div class=\"slideshow slideshowInitialDisplay\">");
            foreach (var pieceOfContent in mergedContent2)
                myStringBuilder.Append(pieceOfContent);
            myStringBuilder.Append("</div>");
            return myStringBuilder.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            allSlideshowContent.Text = AssembleSlideShowHtml();
        }



        
    }
}