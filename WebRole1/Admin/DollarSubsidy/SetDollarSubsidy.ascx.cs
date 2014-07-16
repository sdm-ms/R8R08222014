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
using ClassLibrary1.EFModel;

public partial class Admin_DollarSubsidy_SetDollarSubsidy : System.Web.UI.UserControl
{
    ActionProcessor Obj = new ActionProcessor();
    public Guid? SubtopicId = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        Buttons.okHandler += new CommonControl_PageButtons.OnButtonClick(ImplementSetDollarSubsidy);
        Buttons.cnlHandler += new CommonControl_PageButtons.OnButtonCancle(cancelSetDollarSubsidy);
        try
        {
            Response.Buffer = true;
            Response.ExpiresAbsolute = TestableDateTime.Now.AddDays(-1d);
            Response.Expires = -1500;
            Response.CacheControl = "no-cache";
            if (!(HttpContext.Current.Profile != null && (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != 0))
            {
                Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
                return;
            }

        }
        catch (Exception ex)
        {
            User_Control_ModalPopUp PopUp = (User_Control_ModalPopUp)Buttons.FindControl("PopUp");
            PopUp.MsgString = ex.Message;
            PopUp.Show();
        }
    }
    public void SetupDollarSubsidy(Guid? PointsManagerID)
    {
        SubtopicId = PointsManagerID;

        var ObjPointsManager = Obj.DataAccess.GetPointsManager((Guid)SubtopicId);
        // Loading Default setting for universe
        TxtCurrentPeriodDollersubsidy.Text = ObjPointsManager.CurrentPeriodDollarSubsidy.ToString();
        TextBox Txtdate = (TextBox)TxtDateTime.FindControl("TxtDate");
        if (ObjPointsManager.EndOfDollarSubsidyPeriod != null)
        {
            Txtdate.Text = ObjPointsManager.EndOfDollarSubsidyPeriod.Value.ToShortDateString();
        }
        TxtNextPeriodDollerSubsidy.Text = ObjPointsManager.NextPeriodDollarSubsidy.ToString();
        TxtNextPeriodLength.Text = ObjPointsManager.NextPeriodLength.ToString();
        if (ObjPointsManager.NumPrizes == 0)
        {
            TxtMinPointsToQualify.Text = ObjPointsManager.MinimumPayment.ToString();
            RBtnGrantSubsidyAtRandom.Checked = false;
            RBtnGrantSubsidyToAllUser.Checked = true;
            TxtNumOfUsers.Enabled = true;
            TxtMinPointsToEligible.Enabled = true;
        }
        else
        {
            TxtNumOfUsers.Text = ObjPointsManager.NumPrizes.ToString();
            TxtMinPointsToEligible.Text = ObjPointsManager.MinimumPayment.ToString();
            TxtMinPointsToQualify.Enabled = true;
            RBtnGrantSubsidyAtRandom.Checked = true;
            RBtnGrantSubsidyToAllUser.Checked = false;
        }
    }

    protected void RBtnGrantSubsidyToAllUser_CheckedChanged(object sender, EventArgs e)
    {
        ShowUserControl();
        if (RBtnGrantSubsidyToAllUser.Checked)
        {
            TxtMinPointsToEligible.Text = "";
            TxtNumOfUsers.Text = "";
            TxtMinPointsToEligible.Enabled = true;
            TxtNumOfUsers.Enabled = true;
            TxtMinPointsToQualify.Enabled = true;
            RBtnGrantSubsidyAtRandom.Checked = false;
        }
    }
    protected void RBtnGrantSubsidyAtRandom_CheckedChanged(object sender, EventArgs e)
    {
        ShowUserControl();
        if (RBtnGrantSubsidyAtRandom.Checked)
        {
            TxtMinPointsToQualify.Text = "";
            TxtMinPointsToEligible.Enabled = true;
            TxtNumOfUsers.Enabled = true;
            TxtMinPointsToQualify.Enabled = true;
            RBtnGrantSubsidyToAllUser.Checked = false;
        }
    }

    public void ImplementSetDollarSubsidy()
    {
        // This Code sets new Setting for Dollar subsidy of a universe
        decimal? CurrentPeriodDollerSubsidy;
        if (TxtCurrentPeriodDollersubsidy.Text == "")
        {
            CurrentPeriodDollerSubsidy = null;
        }
        else
        {
            CurrentPeriodDollerSubsidy = Convert.ToDecimal(TxtCurrentPeriodDollersubsidy.Text);
        }
        DateTime? EndofDollerSubsidyPeriod;
        TextBox Txtdate = (TextBox)TxtDateTime.FindControl("TxtDate");
        if (Txtdate.Text == "")
        {
            EndofDollerSubsidyPeriod = null;
        }
        else
        {
            EndofDollerSubsidyPeriod = Convert.ToDateTime(Txtdate.Text);
        }

        decimal? NextPeriodDollerSubsidy;
        if (TxtNextPeriodDollerSubsidy.Text == "")
        {
            NextPeriodDollerSubsidy = null;
        }
        else
        {
            NextPeriodDollerSubsidy = Convert.ToDecimal(TxtNextPeriodDollerSubsidy.Text);
        }
        int? NextPeriodLength;
       if (TxtNextPeriodLength.Text == "")
        {
            NextPeriodLength = null;
        }
        else
        {
            NextPeriodLength = Convert.ToInt32(TxtNextPeriodLength.Text);

        }
        short? NumPrizes = null;
        decimal? MinPayment = null;
        if (RBtnGrantSubsidyAtRandom.Checked)
        {
           if (TxtNumOfUsers.Text.Trim() == "")
            {
                NumPrizes = null;
            }
            else
            {
                NumPrizes = Convert.ToInt16(TxtNumOfUsers.Text);
            }
            if (TxtMinPointsToEligible.Text.Trim() == "")
            {
                MinPayment = null;
            }
            else
            {
                MinPayment = Convert.ToDecimal(TxtMinPointsToEligible.Text);
            }

        }
        if (RBtnGrantSubsidyToAllUser.Checked)
        {
            NumPrizes = 0;
           if (TxtMinPointsToQualify.Text.Trim() == "")
            {
                MinPayment = null;
            }
            else
            {
                MinPayment = Convert.ToDecimal(TxtMinPointsToQualify.Text);
            }
        }
        bool DoItNow = true;

        Guid? ChangeGroupId = null;
        Guid UserId = (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        // Calling the routines
        Obj.PointsManagerChangeSettings((Guid)SubtopicId, CurrentPeriodDollerSubsidy, EndofDollerSubsidyPeriod, NextPeriodDollerSubsidy, NextPeriodLength, NumPrizes, MinPayment, DoItNow, UserId, ChangeGroupId);
        User_Control_ModalPopUp PopUp = (User_Control_ModalPopUp)Buttons.FindControl("PopUp");
        PopUp.MsgString = StringConstants.StringDollarSubsidyChanged;
        PopUp.Show();
        HideUserControl();
    }

    public void cancelSetDollarSubsidy()
    {
        HideUserControl();
    }
    public void ShowUserControl()
    {
        DollarSubsidyPopupPanel.Visible = true;
        EmptyButton.Visible = true;
        DollarSubsidyExtender.Show();
    }

    public void HideUserControl()
    {
        DollarSubsidyPopupPanel.Visible = false;
        EmptyButton.Visible = false;
        DollarSubsidyExtender.Hide();
    }
}
