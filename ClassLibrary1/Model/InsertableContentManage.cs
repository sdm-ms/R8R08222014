using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;

using ClassLibrary1.Model;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insertableContentId"></param>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        /// <param name="isTextOnly"></param>
        /// <param name="overridable"></param>
        /// <param name="location"></param>
        /// <param name="isActivate"></param>
        public void ChangeInsertableContents(int insertableContentId, string name, string contents, bool isTextOnly, bool overridable, InsertableLocation location, bool isActivate)
        {
            // To do: Make it so that this is called by executing changes, rather than directly by ActionProcess
            InsertableContent theInsertableContent = DataContext.GetTable<InsertableContent>().Single(x => x.InsertableContentID == insertableContentId);
            theInsertableContent.Name = name;
            theInsertableContent.Content = contents;
            theInsertableContent.IsTextOnly = isTextOnly;
            theInsertableContent.Location = (short) location;
            theInsertableContent.Overridable = overridable;
            if (isActivate)
                theInsertableContent.Status = (Byte)StatusOfObject.Active;
            else
                theInsertableContent.Status = (Byte)StatusOfObject.Unavailable;
            DataContext.SubmitChanges();
            CacheManagement.InvalidateCacheDependency("InsertableContent");
        }

        public bool AutomaticInsertableContentForPrizesOn()
        {
            return false;
        }

        public void UpdateAutomaticInsertableContentForPointsManager(int pointsManagerID, decimal currentPeriodDollarSubsidy, int? numPrizes, DateTime? endOfDollarSubsidyPeriod)
        {
            if (!AutomaticInsertableContentForPrizesOn())
                return; // this feature has been superceded.
            InsertableContent existingPrizeInfo = DataContext.GetTable<InsertableContent>().SingleOrDefault(x => x.PointsManagerID == pointsManagerID && x.Name == "Current prize info"  && x.Status == (byte) StatusOfObject.Active);
            string content = "";
            if (currentPeriodDollarSubsidy > 0 && endOfDollarSubsidyPeriod != null)
            {
                content = PaymentGuarantees.GetPrizePoolString(currentPeriodDollarSubsidy, numPrizes, endOfDollarSubsidyPeriod, false, true);
            }
            if (existingPrizeInfo == null)
            {
                if (content != "")
                {
                    int newID = AddInsertableContents("Current prize info", null, pointsManagerID, null, content, true, false, InsertableLocation.TopOfViewTblContent);
                    SetStatusOfObject(newID, TypeOfObject.InsertableContent, StatusOfObject.Active);
                }
            }
            else
            {
                if (content == "")
                    DataContext.GetTable<InsertableContent>().DeleteOnSubmit(existingPrizeInfo);
                else
                {
                    if (endOfDollarSubsidyPeriod == null)
                        throw new Exception("End of dollar subsidy period must be specified.");
                    existingPrizeInfo.Content = content;
                    SetStatusOfObject(existingPrizeInfo.InsertableContentID, TypeOfObject.InsertableContent, StatusOfObject.Active);
                }
            }
            DataContext.SubmitChanges();
        }

    }
}