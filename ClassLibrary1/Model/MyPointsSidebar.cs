using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Model
{

    public class MyPointsSidebarInfo
    {
        public string CurrentPeriod;
        public string CurrentPrizeInfo;
        public string CurrentInfo;
        public string PointsThisPeriod;
        public string PendingPointsThisPeriod;
        public string ScoredRatings;
        public string PointsPerRating;

        public MyPointsSidebarInfo(PointsManager thePointsManager, PointsTotal thePointsTotal)
        {

            CurrentPeriod = thePointsManager.EndOfDollarSubsidyPeriod == null ? "<b>Current period</b>" : ("<b>Current period (ends " + ((DateTime)thePointsManager.EndOfDollarSubsidyPeriod).ToShortDateString() + ")</b>");
            CurrentPrizeInfo = PaymentGuarantees.GetPrizePoolString(thePointsManager.CurrentPeriodDollarSubsidy, thePointsManager.NumPrizes, thePointsManager.EndOfDollarSubsidyPeriod, PaymentGuarantees.MaximumGuaranteesAvailable(thePointsManager) > 0, false);
            CurrentInfo = PaymentGuarantees.PaymentGuaranteeStatusString(thePointsTotal, thePointsManager, "", "", "My Guaranteed: ");
            PointsThisPeriod = "My Points: " + ((thePointsTotal == null) ? "0" : ((int)thePointsTotal.CurrentPoints).ToString());
            PendingPointsThisPeriod = "My Pending Points: " + ((thePointsTotal == null) ? "0" : ((int)thePointsTotal.PendingPoints).ToString());
            ScoredRatings = "My Ratings Scored: " + ((thePointsTotal == null) ? "0" : thePointsTotal.NumPendingOrFinalizedRatings.ToString());
            PointsPerRating = "My Points Per Rating: " + ((thePointsTotal == null) ? "0" : thePointsTotal.PointsPerRating.ToString());
        }
    }

}
