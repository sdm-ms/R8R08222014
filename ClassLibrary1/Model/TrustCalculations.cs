using ClassLibrary1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1.Model
{
    public static class TrustCalculations
    {

        public static int NumPerfectScoresToGiveNewUser = 5;

        public static double GetOverallTrustLevelWithNewUserCredit(TrustTrackerStat noWeightingStat, int actualContributingUserInteractions)
        {
            double averageContributionToDenominator;
            if (actualContributingUserInteractions == 0)
                averageContributionToDenominator = 0;
            else
                averageContributionToDenominator = noWeightingStat.Trust_Denom / (double)actualContributingUserInteractions;
            double newNumerator = noWeightingStat.Trust_Numer + (double)NumPerfectScoresToGiveNewUser * averageContributionToDenominator * 1.0;
            double newDenominator = noWeightingStat.Trust_Denom + (double)NumPerfectScoresToGiveNewUser * averageContributionToDenominator;
            if (newDenominator == 0)
                return 1.0F;
            return newNumerator / newDenominator;
        }

        public static double GetUserInteractionWeightInCalculatingTrustTotal(UserInteractionStat noWeightingStat, UserInteraction theUserInteraction)
        {
            if (theUserInteraction.NumTransactions == 0)
                return 0.0;
            return
                    (noWeightingStat.SumWeights / (double)theUserInteraction.NumTransactions)
                        *
                /* giving somewhat more weight when there have been more transactions, but at least some weight as soon as there have been 2 transactions */
                        (double)Math.Log((double)(theUserInteraction.NumTransactions + 1), 10.0)
                        *
                        ConstrainLatestUserTrust(theUserInteraction.LatestUserEgalitarianTrust);
        }

        /// <summary>
        /// Returns the WeightInCalculatingTrustTotal that should exist, based on the LatestUserEgalitarianTrustAtLastWeightUpdate. Useful primarily for testing.
        /// </summary>
        /// <param name="noWeightingStat"></param>
        /// <param name="theUserInteraction"></param>
        /// <returns></returns>
        public static double GetLastUpdatedUserInteractionWeightInCalculatingTrustTotal(UserInteractionStat noWeightingStat, UserInteraction theUserInteraction)
        {
            if (theUserInteraction.NumTransactions == 0)
                return 0.0;
            return
                    (noWeightingStat.SumWeights / (double)theUserInteraction.NumTransactions)
                        *
                /* giving somewhat more weight when there have been more transactions, but at least some weight as soon as there have been 2 transactions */
                        (double)Math.Log((double)(theUserInteraction.NumTransactions + 1), 10.0)
                        *
                        (double) ConstrainLatestUserTrust(theUserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate ?? theUserInteraction.LatestUserEgalitarianTrust);
        }

        public static double ConstrainLatestUserTrust(double unconstrainedTrust)
        {
            return Constrain(unconstrainedTrust, AdjustmentFactorCalc.MinimumRetrospectiveAdjustmentFactor, AdjustmentFactorCalc.MaximumRetrospectiveAdjustmentFactor);
        }


        public static float LogBase(decimal number, decimal theBase)
        {
            if (number <= 0.0001M)
                number = 0.0001M; // avoid potential errors, math precision not needed here
            return Math.Max((float)0, ((float)(Math.Log((double)number) / Math.Log((double)theBase))));
        }

        public static double Constrain(double value, double minValue, double maxValue)
        {
            if (value < minValue)
                return minValue;
            if (value > maxValue)
                return maxValue;
            return value;
        }

        public static float Constrain(float value, float minValue, float maxValue)
        {
            if (value < minValue)
                return minValue;
            if (value > maxValue)
                return maxValue;
            return value;
        }

        public static decimal Constrain(decimal value, decimal minValue, decimal maxValue)
        {
            if (value < minValue)
                return minValue;
            if (value > maxValue)
                return maxValue;
            return value;
        }

    }
}
