using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ClassLibrary1.Model;

using System.Web.Security;

namespace WebRole1.CommonControl
{

    public partial class MyPointsSidebar : System.Web.UI.UserControl
    {
        public PointsTotal ThePointsTotal;
        public PointsManager ThePointsManager;
        public RaterooDataManipulation TheDataAccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            ThePaymentGuaranteeInfo.ThePointsManager = ThePointsManager;
            ThePaymentGuaranteeInfo.TheUser = ThePointsTotal.User;
            ThePaymentGuaranteeInfo.ThePointsTotal = ThePointsTotal;
            ThePaymentGuaranteeInfo.Location = PaymentGuaranteeInfo.PaymentGuaranteeInfoLocation.MyPointsSidebar;
            ThePaymentGuaranteeInfo.TheDataAccess = TheDataAccess;
            if (ThePointsManager != null)
            {
                MyPointsSidebarDiv.Attributes.Add("data-ID", ThePointsManager.PointsManagerID.ToString());
                SetInfo(new MyPointsSidebarInfo(ThePointsManager, ThePointsTotal));
            }
        }

        protected void SetInfo(MyPointsSidebarInfo info)
        {
            CurrentPeriodContent.Text = info.CurrentPeriod;
            CurrentPrizeInfoContent.Text = info.CurrentPrizeInfo;
            PointsThisPeriodContent.Text = info.PointsThisPeriod;
            PendingPointsThisPeriodContent.Text = info.PendingPointsThisPeriod;
            ScoredRatingsContent.Text = info.ScoredRatings;
            PointsPerRatingContent.Text = info.PointsPerRating;
        }

    }
}