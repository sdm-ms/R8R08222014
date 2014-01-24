﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;


namespace WebRole1.Admin
{
    public partial class RewardRatingSettingScreen : System.Web.UI.UserControl
    {
        public PMActionProcessor theActionProcessor;
        public int pointsManagerID;

        public void Setup(int thePointsManagerID)
        {
            pointsManagerID = thePointsManagerID;
            theActionProcessor = new PMActionProcessor();
            PointsManager thePointsManager = theActionProcessor.DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == pointsManagerID);
            RewardRatingSetting currentSetting = thePointsManager.RewardRatingSettings.FirstOrDefault(x => x.Status == (int)StatusOfObject.Active);
            if (!Page.IsPostBack)
            {
                prob.Text = currentSetting.ProbOfRewardEvaluation.ToString();
                mult.Text = currentSetting.Multiplier.ToString();
                HighStakesProbability.Text = thePointsManager.HighStakesProbability.ToString();
                HighStakesKnownMultiplier.Text = thePointsManager.HighStakesKnownMultiplier.ToString();
                HighStakesSecretMultiplier.Text = thePointsManager.HighStakesSecretMultiplier.ToString();
                HighStakesNoviceOn.Text = thePointsManager.HighStakesNoviceOn.ToString();
                HighStakesNoviceNumAutomatic.Text = thePointsManager.HighStakesNoviceNumAutomatic.ToString();
                HighStakesNoviceNumOneThird.Text = thePointsManager.HighStakesNoviceNumOneThird.ToString();
                HighStakesNoviceNumOneTenth.Text = thePointsManager.HighStakesNoviceNumOneTenth.ToString();
                HighStakesNoviceTargetNum.Text = thePointsManager.HighStakesNoviceTargetNum.ToString();
                DatabaseChangeSelectHighStakesNoviceNumPct.Text = thePointsManager.DatabaseChangeSelectHighStakesNoviceNumPct.ToString();
            }
        }

        protected void ChangeSettings(object sender, EventArgs e)
        {
            decimal probVal = Convert.ToDecimal(prob.Text);
            decimal multVal = Convert.ToDecimal(mult.Text);
            decimal highStakesProbability = Convert.ToDecimal(HighStakesProbability.Text);
            decimal highStakesKnownMultiplier = Convert.ToDecimal(HighStakesKnownMultiplier.Text);
            decimal highStakesSecretMultiplier = Convert.ToDecimal(HighStakesSecretMultiplier.Text);
            bool highStakesNoviceOn = Convert.ToBoolean(HighStakesNoviceOn.Text);
            int highStakesNoviceNumAutomatic = Convert.ToInt32(HighStakesNoviceNumAutomatic.Text);
            int highStakesNoviceNumOneThird = Convert.ToInt32(HighStakesNoviceNumOneThird.Text);
            int highStakesNoviceNumOneTenth = Convert.ToInt32(HighStakesNoviceNumOneTenth.Text);
            int highStakesNoviceTargetNum = Convert.ToInt32(HighStakesNoviceTargetNum.Text);
            decimal databaseChangeSelectHighStakesNoviceNumPct = Convert.ToDecimal(DatabaseChangeSelectHighStakesNoviceNumPct.Text);
            theActionProcessor.RewardRatingSettingChange(pointsManagerID, probVal, multVal, (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"));
            theActionProcessor.PointsManagerHighStakesSettings(pointsManagerID, highStakesProbability, highStakesSecretMultiplier, highStakesKnownMultiplier, highStakesNoviceOn, highStakesNoviceNumAutomatic, highStakesNoviceNumOneThird, highStakesNoviceNumOneTenth, highStakesNoviceTargetNum, databaseChangeSelectHighStakesNoviceNumPct, true, (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"), null);
        }
    }
}