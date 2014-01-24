using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ClassLibrary1.Model;
using System.Web.Routing;

namespace WebRole1.CommonControl
{
    public partial class PaymentGuaranteeInfo : System.Web.UI.UserControl
    {
        public User TheUser;
        public PointsManager ThePointsManager;
        public PaymentGuaranteeInfoLocation Location;
        public PointsTotal ThePointsTotal;
        public RaterooDataManipulation TheDataAccess;

        public enum PaymentGuaranteeInfoLocation
        {
            MyPointsPage,
            GuaranteePage,
            MyPointsSidebar
        }

        internal ConditionalGuaranteeUserCanApplyStatus status;

        protected void Page_Load(object sender, EventArgs e)
        {
            ErrorMessage.Text = "";
            ErrorMessage.Visible = false;
            if (ThePointsTotal == null)
                ThePointsTotal = TheUser.PointsTotals.SingleOrDefault(x => x.PointsManager == ThePointsManager);
            status = PMPaymentGuarantees.UserCanApplyForConditionalGuarantee(ThePointsTotal, ThePointsManager);
            if (Location == PaymentGuaranteeInfoLocation.GuaranteePage && ThePointsTotal != null)
            {
                if (status == ConditionalGuaranteeUserCanApplyStatus.canApplyWithoutDocumentation || status == ConditionalGuaranteeUserCanApplyStatus.maximumGuaranteesAlreadyAwardedButUserCanApplyAndWait)
                {
                    PMPaymentGuarantees.InitiateConditionalGuaranteeForExistingUser(ThePointsTotal);
                }
            }
            UpdateInfo();
        }

        protected void UpdateInfo()
        {
            string spanOpening;
            if (Location == PaymentGuaranteeInfoLocation.MyPointsSidebar)
                spanOpening = "<span id=\"guaranteeDollars\">"; // need unique id to make this updatable
            else
                spanOpening = "<span>"; // don't want id because we could have more than one on same page
            string useThisIfUserNotTrusted = "";
            string useThisIfGuaranteesNotAvailable = "";
            string prefix = "";
            switch (Location)
            {
                case PaymentGuaranteeInfoLocation.GuaranteePage:
                    useThisIfGuaranteesNotAvailable = "Guaranteed payments are not currently available for this table.";
                    useThisIfUserNotTrusted = "You are not yet trusted enough to receive guaranteed payments. You may, however, work on the table to earn Rateroo's trust.";
                    prefix = "Guaranteed payment: ";
                    break;
                case PaymentGuaranteeInfoLocation.MyPointsPage:
                    useThisIfGuaranteesNotAvailable = "N/A";
                    useThisIfUserNotTrusted = "N/A";
                    break;
                case PaymentGuaranteeInfoLocation.MyPointsSidebar:
                    prefix = "My Guaranteed: ";
                    Apply.Text = "Apply";
                    break;
            }
            string paymentGuaranteeStatusString = PMPaymentGuarantees.PaymentGuaranteeStatusString(ThePointsTotal, ThePointsManager, useThisIfGuaranteesNotAvailable, useThisIfUserNotTrusted, prefix);
            CurrentInfo.Text = spanOpening + paymentGuaranteeStatusString + "</span>";
            //if (paymentGuaranteeStatusString != "")
            //    CurrentInfo.Text += "<br/>";
            if (status != ConditionalGuaranteeUserCanApplyStatus.canApplyWithDocumentation && status != ConditionalGuaranteeUserCanApplyStatus.canApplyWithoutDocumentation && status != ConditionalGuaranteeUserCanApplyStatus.maximumGuaranteesAlreadyAwardedButUserCanApplyAndWait)
            {
                Apply.Visible = false;
                DocumentationUpload.Visible = false;
                DocumentationUploadInstructions.Visible = false;
            }
            else
            {
                Apply.Visible = true;
                if (Location == PaymentGuaranteeInfoLocation.MyPointsSidebar)
                { // event won't fire, so set postback event directly
                    //string urlString;
                    //RouteValueDictionary parameters;
                    //PMRouting.OutgoingGetRoute(new PMRoutingInfoMainContent(ThePointsManager.Tbls.First(), null, null, false, false, false, false, true), out urlString, out parameters );
                    //Apply.PostBackUrl = "/" + new PMRoutingInfoMainContent(ThePointsManager.Tbls.First(x => x.Name != "Changes"), null, null, false, false, false, false, true).GetHierarchyRoutingParameterString(); // "~/Guarantees";
                    PMRoutingInfoMainContent route = PMRouting.IncomingMainContent(Page.RouteData, GetIRaterooDataContext.New(false, false));
                    Apply.PostBackUrl = route.lastItemInHierarchy.RouteToHere + "/Guarantees";
                }
                if (Location == PaymentGuaranteeInfoLocation.GuaranteePage)
                {
                    DocumentationUpload.Visible = Location == PaymentGuaranteeInfoLocation.GuaranteePage && status == ConditionalGuaranteeUserCanApplyStatus.canApplyWithDocumentation;
                    DocumentationUploadInstructions.Visible = true;
                    DocumentationUploadInstructions.Text = "<br/><span>To apply for a guaranteed payment, please submit a resume or other information about yourself, and your application will be considered. If you prefer not to do so, you can also earn money simply by entering ratings. Once enough time has passed for Rateroo to evaluate your performance, you will be eligible for available payment guarantees without documentation.</span><br/>";
                }
                else
                {
                    DocumentationUpload.Visible = false;
                    DocumentationUploadInstructions.Visible = false;
                }
            }
        }

        protected void Apply_Click(object sender, EventArgs e)
        {
            if (Location == PaymentGuaranteeInfoLocation.GuaranteePage)
            {
                if (status == ConditionalGuaranteeUserCanApplyStatus.canApplyWithDocumentation)
                {
                    try
                    {
                        PMPaymentGuarantees.UploadConditionalGuaranteeApplicationForNewUser(TheDataAccess, TheUser, ThePointsManager, DocumentationUpload);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage.Text = ex.Message;
                        ErrorMessage.Visible = true;
                    }
                }
            }
            else
            {
                Tbl theTbl = ThePointsManager.Tbls.First(x => x.Name != "Changes");
                PMRouting.Redirect(Response, new PMRoutingInfoMainContent(theTbl, null, null,false,false,false,false,true));
            }
            UpdateInfo();
        }
    }
}