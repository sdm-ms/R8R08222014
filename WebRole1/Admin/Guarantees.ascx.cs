using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


namespace WebRole1.Admin
{
    public partial class Guarantees : System.Web.UI.UserControl
    {
        public ActionProcessor theActionProcessor;
        public Guid pointsManagerID;

        public void Setup(int thePointsManagerID)
        {
            pointsManagerID = thePointsManagerID;
            theActionProcessor = new ActionProcessor();
            PointsManager thePointsManager = theActionProcessor.DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == pointsManagerID);
            if (!Page.IsPostBack)
            {
                dollarValuePerPoint.Text = thePointsManager.DollarValuePerPoint.ToString();
                discountForGuarantees.Text = thePointsManager.DiscountForGuarantees.ToString();
                maximumTotalGuarantees.Text = thePointsManager.MaximumTotalGuarantees.ToString();
                maximumGuaranteePaymentPerHour.Text = thePointsManager.MaximumGuaranteePaymentPerHour.ToString();
                totalUnconditionalGuaranteesEarnedEver.Text = thePointsManager.TotalUnconditionalGuaranteesEarnedEver.ToString();
                totalConditionalGuaranteesEarnedEver.Text = thePointsManager.TotalConditionalGuaranteesEarnedEver.ToString();
                allowApplicationsWhenNoConditionalGuaranteesAvailable.Text = thePointsManager.AllowApplicationsWhenNoConditionalGuaranteesAvailable.ToString();
                conditionalGuaranteesAvailableForNewUsers.Text = thePointsManager.ConditionalGuaranteesAvailableForNewUsers.ToString();
                conditionalGuaranteesAvailableForExistingUsers.Text = thePointsManager.ConditionalGuaranteesAvailableForExistingUsers.ToString();
                conditionalGuaranteeTimeBlockInHours.Text = thePointsManager.ConditionalGuaranteeTimeBlockInHours.ToString();
            }
        }


        protected void ChangeSettings(object sender, EventArgs e)
        {
            decimal DollarValuePerPoint = Convert.ToDecimal(dollarValuePerPoint.Text);
            decimal DiscountForGuarantees = Convert.ToDecimal(discountForGuarantees.Text);
            decimal MaximumTotalGuarantees = Convert.ToDecimal(maximumTotalGuarantees.Text);
            decimal MaximumGuaranteePaymentPerHour = Convert.ToDecimal(maximumGuaranteePaymentPerHour.Text.ToString());
            bool AllowApplicationsWhenNoConditionalGuaranteesAvailable = Convert.ToBoolean(allowApplicationsWhenNoConditionalGuaranteesAvailable.Text);
            bool ConditionalGuaranteesAvailableForNewUsers = Convert.ToBoolean(conditionalGuaranteesAvailableForNewUsers.Text);
            bool ConditionalGuaranteesAvailableForExistingUsers = Convert.ToBoolean(conditionalGuaranteesAvailableForExistingUsers.Text);
            int ConditionalGuaranteeTimeBlockInHours = Convert.ToInt32(conditionalGuaranteeTimeBlockInHours.Text);

            theActionProcessor.PointsManagerGuaranteeSettings(pointsManagerID, DollarValuePerPoint, DiscountForGuarantees, MaximumTotalGuarantees, AllowApplicationsWhenNoConditionalGuaranteesAvailable, ConditionalGuaranteesAvailableForNewUsers, ConditionalGuaranteesAvailableForExistingUsers, ConditionalGuaranteeTimeBlockInHours, MaximumGuaranteePaymentPerHour, true, (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"), null);
        }
    }
}