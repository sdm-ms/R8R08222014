using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClassLibrary1.Misc;
using System.Web.UI.WebControls;

using System.IO;
using System.Web.Security;

namespace ClassLibrary1.Model
{
    public enum ConditionalGuaranteeUserCanApplyStatus
    {
        noConditionalGuaranteesAllowed,
        maximumGuaranteesAlreadyAwardedAndUserIsNew,
        maximumGuaranteesAlreadyAwardedButUserCanApplyAndWait,
        conditionalGuaranteeApplicationAlreadyPending,
        conditionalGuaranteeAlreadyActive,
        userIsntTrusted,
        canApplyWithDocumentation,
        canApplyWithoutDocumentation
    }

    public partial class R8RDataManipulation
    {

        public void GuaranteeSettings(int pointsManagerID, decimal dollarValuePerPoint, decimal discountForGuarantees, decimal maximumTotalGuarantees, bool conditionalGuaranteesAvailableForNewUsers, bool allowApplicationsWhenNoConditionalGuaranteesAvailable, bool conditionalGuaranteesAvailableForExistingUsers, int conditionalGuaranteeTimeBlockInHours, decimal maximumGuaranteePaymentPerHour)
        {
            PointsManager thePointsManager = DataContext.NewOrSingle<PointsManager>(x => x.PointsManagerID == pointsManagerID);
            thePointsManager.DollarValuePerPoint = dollarValuePerPoint;
            thePointsManager.DiscountForGuarantees = discountForGuarantees;
            thePointsManager.MaximumTotalGuarantees = maximumTotalGuarantees;
            thePointsManager.AllowApplicationsWhenNoConditionalGuaranteesAvailable = allowApplicationsWhenNoConditionalGuaranteesAvailable;
            thePointsManager.ConditionalGuaranteesAvailableForNewUsers = conditionalGuaranteesAvailableForNewUsers;
            thePointsManager.ConditionalGuaranteesAvailableForExistingUsers = conditionalGuaranteesAvailableForExistingUsers;
            thePointsManager.ConditionalGuaranteeTimeBlockInHours = conditionalGuaranteeTimeBlockInHours;
            thePointsManager.MaximumGuaranteePaymentPerHour = maximumGuaranteePaymentPerHour;
        }

    }

    public static class PaymentGuarantees
    {

        public static string GetPrizePoolString(decimal currentPeriodDollarSubsidy, int? numPrizes, DateTime? endOfDollarSubsidyPeriod, bool guaranteesAvailable, bool indicateEndOfPeriod)
        {
            string content;
            if (currentPeriodDollarSubsidy == 0)
            {
                if (guaranteesAvailable)
                    content = "Payments available for this table based on past success and time worked.";
                else
                    content = "Cash prizes not currently available for this table.";
            }
            content = "Prize pool: $" + currentPeriodDollarSubsidy.ToString() + " total";
            if (numPrizes != null && numPrizes != 0)
                content += ", divided among " + numPrizes + " prizes";
            if (indicateEndOfPeriod)
                content += ", for ratings by: " + ((DateTime)endOfDollarSubsidyPeriod).ToShortDateString();
            return content;
        }

        public static decimal MaximumGuaranteesAvailable(PointsManager thePointsManager)
        {
            return Math.Max(thePointsManager.MaximumTotalGuarantees - thePointsManager.TotalUnconditionalGuaranteesEarnedEver - thePointsManager.TotalConditionalGuaranteesEarnedEver - thePointsManager.TotalConditionalGuaranteesPending, 0);
        }

        // If there is no pending conditional guarantee, we calculate the guaranteed payment the user is entitled to
        // based on the number of hours the user has worked this reward period and the user's projected points per hour
        // (based on past performance). This amount can never go down within a reward period. We will give guaranteed payments
        // only up to the maximum permitted.
        public static void CalculateGuaranteedPaymentsEarnedThisRewardPeriod(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                return;
            if (thePointsTotal.PendingConditionalGuaranteeTotalHoursAtStart != null) // pending conditional guarantee
                return;
            decimal maximumGuaranteesAvailable = MaximumGuaranteesAvailable(thePointsTotal.PointsManager);
            if (maximumGuaranteesAvailable <= 0)
                return;
            decimal pointsAlreadyEarnedProjection = (thePointsTotal.ProjectedPointsPerHour ?? 0) * thePointsTotal.TotalTimeThisRewardPeriod;
            if (pointsAlreadyEarnedProjection < 0)
                pointsAlreadyEarnedProjection = 0;
            decimal possibleNewGuarantee = pointsAlreadyEarnedProjection * thePointsTotal.PointsManager.DollarValuePerPoint * thePointsTotal.PointsManager.DiscountForGuarantees;
            possibleNewGuarantee = Math.Min(possibleNewGuarantee, maximumGuaranteesAvailable); // we can't make a guarantee that's bigger than the maximum guarantee available
            possibleNewGuarantee = Math.Min(possibleNewGuarantee, thePointsTotal.PointsManager.MaximumGuaranteePaymentPerHour * thePointsTotal.TotalTimeThisRewardPeriod); // don't promise more per hour than this
            if (possibleNewGuarantee > thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod)
            { // guaranteed payments in a reward period can never go down.
                decimal increment = thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod - possibleNewGuarantee;
                thePointsTotal.PointsManager.TotalUnconditionalGuaranteesEarnedEver += increment;
                thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod = possibleNewGuarantee;
            }
        }


        public static ConditionalGuaranteeUserCanApplyStatus UserCanApplyForConditionalGuarantee(PointsTotal thePointsTotal, PointsManager thePointsManager)
        {
            bool userIsNew = thePointsTotal == null || thePointsTotal.TotalPoints == 0 && thePointsTotal.PendingPoints == 0 && thePointsTotal.PendingConditionalGuaranteePayment == null;
            if (thePointsManager.MaximumTotalGuarantees == 0) // guarantees have never been available here
                return ConditionalGuaranteeUserCanApplyStatus.noConditionalGuaranteesAllowed;
            if (userIsNew && !thePointsManager.ConditionalGuaranteesAvailableForNewUsers)
                return ConditionalGuaranteeUserCanApplyStatus.noConditionalGuaranteesAllowed;
            if (!userIsNew && !thePointsManager.ConditionalGuaranteesAvailableForExistingUsers)
                return ConditionalGuaranteeUserCanApplyStatus.noConditionalGuaranteesAllowed;
            if (userIsNew && thePointsTotal != null && thePointsTotal.PendingConditionalGuaranteeApplication != null)
                return ConditionalGuaranteeUserCanApplyStatus.conditionalGuaranteeApplicationAlreadyPending;
            decimal maximumGuaranteesAvailable = MaximumGuaranteesAvailable(thePointsManager);
            if (maximumGuaranteesAvailable == 0)
            {
                if (!thePointsManager.AllowApplicationsWhenNoConditionalGuaranteesAvailable)
                    return ConditionalGuaranteeUserCanApplyStatus.noConditionalGuaranteesAllowed;
                return userIsNew ? ConditionalGuaranteeUserCanApplyStatus.maximumGuaranteesAlreadyAwardedAndUserIsNew : ConditionalGuaranteeUserCanApplyStatus.maximumGuaranteesAlreadyAwardedButUserCanApplyAndWait;
            }
            if (thePointsTotal != null)
            {
                if (thePointsTotal.PendingConditionalGuaranteeTotalHoursAtStart != null)
                    return ConditionalGuaranteeUserCanApplyStatus.conditionalGuaranteeAlreadyActive;
                if (!userIsNew && (thePointsTotal.ProjectedPointsPerHour == null || thePointsTotal.ProjectedPointsPerHour <= 0))
                    return ConditionalGuaranteeUserCanApplyStatus.userIsntTrusted;
            }
            if (userIsNew)
                return ConditionalGuaranteeUserCanApplyStatus.canApplyWithDocumentation;
            else
                return ConditionalGuaranteeUserCanApplyStatus.canApplyWithoutDocumentation;
        }

        public static string PaymentGuaranteeStatusString(PointsTotal thePointsTotal, PointsManager thePointsManager, string useThisIfGuaranteesNotAvailable, string useThisIfUserNotTrusted, string prefix)
        {
            ConditionalGuaranteeUserCanApplyStatus status = UserCanApplyForConditionalGuarantee(thePointsTotal, thePointsManager);
            if (thePointsTotal == null || thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod == 0)
            {
                if (status == ConditionalGuaranteeUserCanApplyStatus.maximumGuaranteesAlreadyAwardedAndUserIsNew)
                    return useThisIfGuaranteesNotAvailable;
                if (status == ConditionalGuaranteeUserCanApplyStatus.userIsntTrusted)
                    return useThisIfUserNotTrusted;
            }
            string returnString = prefix;
            returnString += "$" + (thePointsTotal == null ? "0" : thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod.ToString());
            if (thePointsTotal != null && thePointsTotal.PendingConditionalGuaranteePayment != null && thePointsTotal.PendingConditionalGuaranteePayment > 0)
            {
                decimal totalHoursRequired = Math.Max(thePointsTotal.PendingConditionalGuaranteeTotalHoursNeeded ?? 0 - thePointsTotal.PendingConditionalGuaranteeTotalHoursAtStart ?? 0, 0);
                string timeString = (int) totalHoursRequired + ":" + ((int) ((totalHoursRequired - (int) totalHoursRequired) * (decimal) 60)).ToString();
                decimal remainingPayment = Math.Max(thePointsTotal.PendingConditionalGuaranteePayment ?? 0 - thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade ?? 0, 0);
                string additionalString = "";
                if ((thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade ?? 0) > 0)
                    additionalString += " of guarantee still available";
                returnString += "; $" + remainingPayment + additionalString + " after working " + timeString + " more";
            }
            if (status == ConditionalGuaranteeUserCanApplyStatus.canApplyWithDocumentation)
                returnString += ""; // "; upload resume to apply for guarantee";
            else if (status == ConditionalGuaranteeUserCanApplyStatus.conditionalGuaranteeApplicationAlreadyPending)
                returnString += "; guarantee application pending";
            else if (thePointsTotal.RequestConditionalGuaranteeWhenAvailableTimeRequestMade != null)
                returnString += "; guarantee application pending";
            return returnString;
        }

        internal static Tuple<string, string>[] contentTypesAndFileExtensions = new Tuple<string, string>[] { 
            Tuple.Create("text/plain", ".txt"), Tuple.Create("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx"), Tuple.Create("application/msword", ".doc"), Tuple.Create("application/pdf", ".pdf") };


        internal static string GetContentTypeForFileExtension(string fileExtension)
        {
            Tuple<string, string> theTuple = contentTypesAndFileExtensions.SingleOrDefault(x => x.Item2 == fileExtension);
            if (theTuple == null)
                return null;
            else
                return theTuple.Item1;
        }

        internal static string GetFileExtensionForContentType(string contentType)
        {
            Tuple<string, string> theTuple = contentTypesAndFileExtensions.SingleOrDefault(x => x.Item1 == contentType);
            if (theTuple == null)
                return null;
            else
                return theTuple.Item2;
        }

        public static void UploadConditionalGuaranteeApplicationForNewUser(R8RDataManipulation dataAccess, User theUser, PointsManager thePointsManager, FileUpload fileUpload)
        {
            if (!fileUpload.HasFile)
                throw new Exception("No file was uploaded.");

            if (fileUpload.PostedFile.FileName.Length > 40)
                throw new Exception("File name too long.");

            string[] reservedChars = { "$", "&", "+", ",", "/", ":", ";", "=", "?", "@" };
            if (reservedChars.Any(x => fileUpload.PostedFile.FileName.Contains(x)))
                throw new Exception("File name cannot contain special characters (e.g., commas, dollar signs, etc.).");

            string fileExtension = GetFileExtensionForContentType(fileUpload.PostedFile.ContentType);
            if (fileExtension == null)
                throw new Exception("Unsupported file type. Only Text files (.txt), Microsoft Word (.doc, .docx), and Adobe Acrobat (.pdf) files are supported.");

            PointsTotal thePointsTotal = theUser.PointsTotals.SingleOrDefault(x => x.PointsManager == thePointsManager);
            if (thePointsTotal == null)
                thePointsTotal = dataAccess.AddPointsTotal(theUser, thePointsManager);

            try
            {
                thePointsTotal.PointsManager.ConditionalGuaranteeApplicationsReceived++;
                int applicationNumber = thePointsTotal.PointsManager.ConditionalGuaranteeApplicationsReceived;
                dataAccess.DataContext.SubmitChanges();

                string filename = applicationNumber.ToString() + "_" + fileUpload.PostedFile.FileName;
                thePointsTotal.PendingConditionalGuaranteeApplication = filename;

                R8RFile storedFile = new R8RFile("guaranteeapplications", filename);
                storedFile.CreateTemporary();
                string fileLocation = storedFile.GetPathToLocalFile();
                fileUpload.SaveAs(fileLocation);
                storedFile.StorePermanently();
                storedFile.DeleteTemporary();

                dataAccess.DataContext.SubmitChanges();
            }
            catch
            {
                throw new Exception("Sorry, the file could not be uploaded, because an internal error occurred.");
            }
        }

        public static void DownloadConditionalGuaranteeApplicationForNewUser(System.Web.HttpResponse theResponse, PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null || thePointsTotal.PendingConditionalGuaranteeApplication == null)
                throw new Exception("This user does not have a pending conditional guarantee application.");

            R8RFile storedFile = new R8RFile("guaranteeapplications", thePointsTotal.PendingConditionalGuaranteeApplication);
            string contentType = GetContentTypeForFileExtension(Path.GetExtension(storedFile.GetPathToLocalFile()));
            storedFile.DownloadToUserBrowser(theResponse, contentType);
        }

        public static void RejectConditionalGuaranteeForNewUser(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                throw new Exception("User does not have a pending conditional guarantee application.");
            R8RFile storedFile = new R8RFile("guaranteeapplications", thePointsTotal.PendingConditionalGuaranteeApplication);
            storedFile.DeletePermanently();
            NotifyUserConditionalGuaranteeHasBeenRejected(thePointsTotal);
        }

        public static void ApproveConditionalGuaranteeForNewUser(PointsTotal thePointsTotal, decimal approvedPaymentSubjectToReduction)
        {
            if (thePointsTotal == null)
                throw new Exception("User does not have a pending conditional guarantee application.");
            if (UserCanApplyForConditionalGuarantee(thePointsTotal,thePointsTotal.PointsManager) != ConditionalGuaranteeUserCanApplyStatus.conditionalGuaranteeApplicationAlreadyPending)
                throw new Exception("No advance guarantee application is pending.");
            R8RFile storedFile = new R8RFile("guaranteeapplications", thePointsTotal.PendingConditionalGuaranteeApplication);
            storedFile.DeletePermanently();
            StartConditionalGuarantee(thePointsTotal, approvedPaymentSubjectToReduction);
            NotifyUserConditionalGuaranteeHasBeenGranted(thePointsTotal);

        }

        public static void RequestConditionalGuaranteeWhenAvailable(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                throw new Exception("R8R cannot grant guarantees when available for brand new users.");
            if (thePointsTotal.RequestConditionalGuaranteeWhenAvailableTimeRequestMade == null)
                thePointsTotal.RequestConditionalGuaranteeWhenAvailableTimeRequestMade = TestableDateTime.Now;
        }

        public static bool InitiateConditionalGuaranteeForExistingUser(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                throw new Exception("R8R cannot initiate conditional guarantee for brand new user without documentation.");
            ConditionalGuaranteeUserCanApplyStatus canApplyStatus = UserCanApplyForConditionalGuarantee(thePointsTotal, thePointsTotal.PointsManager);
            if (canApplyStatus != ConditionalGuaranteeUserCanApplyStatus.canApplyWithoutDocumentation)
            {
                bool userIsNew = thePointsTotal.TotalPoints == 0 && thePointsTotal.PendingPoints == 0 && thePointsTotal.PendingConditionalGuaranteePayment == null;
                if (canApplyStatus == ConditionalGuaranteeUserCanApplyStatus.maximumGuaranteesAlreadyAwardedButUserCanApplyAndWait)
                    RequestConditionalGuaranteeWhenAvailable(thePointsTotal);
                return false;
            }
            StartConditionalGuarantee(thePointsTotal,(thePointsTotal.ProjectedPointsPerHour ?? 0) * thePointsTotal.PointsManager.DollarValuePerPoint * (decimal) thePointsTotal.PointsManager.ConditionalGuaranteeTimeBlockInHours);
            return true;
        }

        internal static void StartConditionalGuarantee(PointsTotal thePointsTotal, decimal approvedPaymentSubjectToReduction)
        {
            if (approvedPaymentSubjectToReduction < 0)
                approvedPaymentSubjectToReduction = 0;
            decimal maximum = thePointsTotal.PointsManager.MaximumGuaranteePaymentPerHour * (decimal) thePointsTotal.PointsManager.ConditionalGuaranteeTimeBlockInHours;
            if (approvedPaymentSubjectToReduction > maximum)
                approvedPaymentSubjectToReduction = maximum;
            thePointsTotal.PendingConditionalGuaranteeApplication = null;
            thePointsTotal.PendingConditionalGuaranteePayment = approvedPaymentSubjectToReduction; // no negative conditional guarantee payments
            thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade = 0;
            thePointsTotal.PendingConditionalGuaranteeTotalHoursAtStart = thePointsTotal.TotalTimeEver;
            thePointsTotal.PendingConditionalGuaranteeTotalHoursNeeded = thePointsTotal.TotalTimeEver + (decimal)thePointsTotal.PointsManager.ConditionalGuaranteeTimeBlockInHours;
            thePointsTotal.RequestConditionalGuaranteeWhenAvailableTimeRequestMade = null;
        }

        internal static void CancelConditionalGuarantee(PointsTotal thePointsTotal)
        {
            thePointsTotal.PendingConditionalGuaranteeApplication = null;
            thePointsTotal.PendingConditionalGuaranteePayment = null;
            thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade = null;
            thePointsTotal.PendingConditionalGuaranteeTotalHoursAtStart = null;
            thePointsTotal.PendingConditionalGuaranteeTotalHoursNeeded = null;
            thePointsTotal.RequestConditionalGuaranteeWhenAvailableTimeRequestMade = null;
        }

        public static void CheckSatisfactionOfConditionalGuarantee(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                return;
            if (thePointsTotal.PendingConditionalGuaranteePayment != null && thePointsTotal.PendingConditionalGuaranteeTotalHoursNeeded != null && thePointsTotal.TotalTimeEver >= (decimal)thePointsTotal.PendingConditionalGuaranteeTotalHoursNeeded)
                EndConditionalGuaranteeOnSatisfactionOfRequirement(thePointsTotal);
        }

        internal static void EndConditionalGuaranteeOnSatisfactionOfRequirement(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                throw new Exception("Conditional guarantee cannot be satisfied for brand new users.");
            thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod += Math.Max((thePointsTotal.PendingConditionalGuaranteePayment ?? 0) - (thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade ?? 0), 0);
            CancelConditionalGuarantee(thePointsTotal);
            CalculateGuaranteedPaymentsEarnedThisRewardPeriod(thePointsTotal);
        }


        // At the end of the reward period, earned dollars (including at least the guaranteed payments) is made. If there is a pending conditional guarantee, then we need to keep track of the amount of payment already made on the conditional guarantee (i.e., amount of payment over the guaranteed payments already earned), by incrementing PaymentAlreadyMadeOnConditionalGuarantee. If the amount is greater than the conditional guarantee, then we end the conditional guarantee.
        internal static void CheckPartialPaymentOnConditionalGuaranteeAtEndOfRewardPeriod(PointsTotal thePointsTotal, decimal amountBeingPaidToUser)
        {
            if (thePointsTotal == null || thePointsTotal.PendingConditionalGuaranteePayment == null)
                return;
            if (amountBeingPaidToUser > thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod)
            {
                decimal amountPaidOverPriorGuarantees = amountBeingPaidToUser - thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod;
                decimal remainingSizeOfConditionalGuarantee = (decimal)thePointsTotal.PendingConditionalGuaranteePayment - (thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade ?? 0);
                if (amountPaidOverPriorGuarantees >= remainingSizeOfConditionalGuarantee)
                    CancelConditionalGuarantee(thePointsTotal); // R8R's obligation is ended
                else
                {
                    decimal amountRemaining = remainingSizeOfConditionalGuarantee - amountPaidOverPriorGuarantees;
                    thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade = (thePointsTotal.PendingConditionalGuaranteePaymentAlreadyMade ?? 0) + amountPaidOverPriorGuarantees;
                }
            }
        }

        public static void EndOfRewardPeriodTasks(PointsTotal thePointsTotal, decimal amountBeingPaidToUser)
        {
            if (thePointsTotal == null)
                throw new Exception("Internal error: User should have a PointsTotal before the end of the reward period.");
            CheckPartialPaymentOnConditionalGuaranteeAtEndOfRewardPeriod(thePointsTotal, amountBeingPaidToUser);
            thePointsTotal.GuaranteedPaymentsEarnedThisRewardPeriod = 0;
            thePointsTotal.TotalTimeThisRewardPeriod = 0;
        }

        internal static string GetUserConditionalGuaranteeHasBeenGrantedString(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                throw new Exception("Internal error. PointsTotal should exist.");
            Tbl firstTable = thePointsTotal.PointsManager.Tbls.FirstOrDefault(x => x.Name != "Changes");
            string itemPath = ItemPathWrapper.GetItemPath(firstTable, true);
            string hoursRequired = thePointsTotal.PointsManager.ConditionalGuaranteeTimeBlockInHours.ToString();
            string dollarsPromised = (thePointsTotal.PendingConditionalGuaranteePayment ?? 0).ToString();
            string theString = String.Format("R8R.com has approved your application for a guaranteed payment. If you work for {0} hours on http://rateroo.com{1}, then you will receive at least ${2}. To see how much time you must still work, please visit http://rateroo.com/MyPoints.", hoursRequired, itemPath, dollarsPromised);
            return theString;
        }

        internal static string GetUserConditionalGuaranteeHasBeenRejectedString(PointsTotal thePointsTotal)
        {
            if (thePointsTotal == null)
                throw new Exception("Internal error. PointsTotal should exist.");
            Tbl firstTable = thePointsTotal.PointsManager.Tbls.FirstOrDefault(x => x.Name != "Changes");
            string itemPath = ItemPathWrapper.GetItemPath(firstTable, true);
            string additionalOpportunity;
            if (thePointsTotal.PointsManager.CurrentPeriodDollarSubsidy == 0)
                additionalOpportunity = "You may work on that table, but no payments are currently available.";
            else
            {
                if (thePointsTotal.PointsManager.NumPrizes == 0)
                    additionalOpportunity = "Prize money is, however, currently available for that table. You can work on that table, and then visit visit http://rateroo.com/MyPoints later to see if you have locked in a payment or if you are likely to receive a payment at the end of the prize period.";
                else
                    additionalOpportunity = "You may, however, work on that page to try to win a prize, and you can then visit http://rateroo.com/MyPoints to see if you've won.";
            }
            string theString = String.Format("R8R.com cannot approve a guaranteed payment for you to work on {0}. {1}", itemPath, additionalOpportunity);
            return theString;
        }

        internal static void SendEmail(PointsTotal thePointsTotal, string messageSubject, string messageBody)
        {
            if (thePointsTotal == null)
                throw new Exception("Internal error. PointsTotal should exist.");
            IUserProfileInfo theUser = UserProfileCollection.LoadByUsername(thePointsTotal.User.Username);
            Email.SendMessage(theUser.Email, messageSubject, messageBody);
        }


        public static void NotifyUserConditionalGuaranteeHasBeenGranted(PointsTotal thePointsTotal)
        {
            SendEmail(thePointsTotal, "Your R8R.com payment application", GetUserConditionalGuaranteeHasBeenGrantedString(thePointsTotal));
        }

        public static void NotifyUserConditionalGuaranteeHasBeenRejected(PointsTotal thePointsTotal)
        {
            SendEmail(thePointsTotal, "Your R8R.com payment application", GetUserConditionalGuaranteeHasBeenRejectedString(thePointsTotal));
        }

    }
}
