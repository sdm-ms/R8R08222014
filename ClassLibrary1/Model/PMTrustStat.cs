using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1.Model
{
    public enum TrustStat
    {
        NoExtraWeighting = 0,
        LargeDeltaRatings = 1,
        SmallDeltaRatings = 2,
        LastDayVolatility = 3,
        LastHourVolatility = 4,
        Extremeness = 5,
        CurrentRatingQuestionable = 6,
        PercentPreviousRatings = 7
    }
}
