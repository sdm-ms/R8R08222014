﻿using ClassLibrary1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1.Model
{
    public static class PMTrustCalculations
    {

        public static float GetUserInteractionWeightInCalculatingTrustTotal(UserInteractionStat noWeightingStat, UserInteraction theUserInteraction)
        {
            if (theUserInteraction.NumTransactions == 0)
                return 0.0F;
            return
                    (noWeightingStat.SumWeights / (float)theUserInteraction.NumTransactions)
                        *
                /* giving somewhat more weight when there have been more transactions, but at least some weight as soon as there have been 2 transactions */
                        (float)Math.Log((double)(theUserInteraction.NumTransactions + 1), 10.0)
                        *
                        ConstrainLatestUserTrust(theUserInteraction.LatestUserEgalitarianTrust);
        }

        /// <summary>
        /// Returns the WeightInCalculatingTrustTotal that should exist, based on the LatestUserEgalitarianTrustAtLastWeightUpdate. Useful primarily for testing.
        /// </summary>
        /// <param name="noWeightingStat"></param>
        /// <param name="theUserInteraction"></param>
        /// <returns></returns>
        public static float GetLastUpdatedUserInteractionWeightInCalculatingTrustTotal(UserInteractionStat noWeightingStat, UserInteraction theUserInteraction)
        {
            if (theUserInteraction.NumTransactions == 0)
                return 0.0F;
            return
                    (noWeightingStat.SumWeights / (float)theUserInteraction.NumTransactions)
                        *
                /* giving somewhat more weight when there have been more transactions, but at least some weight as soon as there have been 2 transactions */
                        (float)Math.Log((double)(theUserInteraction.NumTransactions + 1), 10.0)
                        *
                        ConstrainLatestUserTrust(theUserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate ?? theUserInteraction.LatestUserEgalitarianTrust);
        }
        
        public static float ConstrainLatestUserTrust(float unconstrainedTrust)
        {
            return Constrain(unconstrainedTrust, PMAdjustmentFactor.MinimumRetrospectiveAdjustmentFactor, PMAdjustmentFactor.MaximumRetrospectiveAdjustmentFactor);
        }


        public static float LogBase(decimal number, decimal theBase)
        {
            if (number <= 0)
                return 0; // avoid potential errors, math precision not needed here
            return Math.Max((float)0, ((float)(Math.Log((double)number) / Math.Log((double)theBase))));
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