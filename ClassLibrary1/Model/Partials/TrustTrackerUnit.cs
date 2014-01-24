using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1.Model
{
    public partial class TrustTrackerUnit
    {
        public const decimal DefaultExtendIntervalWhenChangeIsLessThanThis = 0.05m;
        public const int DefaultMinUpdateIntervalSeconds = 600; // Ten minutes
        public const int DefaultMaxUpdateIntervalSeconds = 1209600; // Two weeks
        public const decimal DefaultExtendIntervalMultiplier = 1.3m;
        public const float IntervalUpdateFactor = 1.1f;

        // The following is deleted, because we now initialize the value in PMAddObjects
        //partial void OnCreated()
        //{
        //    // I don't think that I was seeing this value mis-initialized, but I'm setting it here anyways
        //    if (this.ExtendIntervalWhenChangeIsLessThanThis == 0)
        //        this.ExtendIntervalWhenChangeIsLessThanThis = DefaultExtendIntervalWhenChangeIsLessThanThis;
        //    // Even though I have a default value for ExtendIntervalMultiplier defined in the database, I was
        //    //  still seeing it iniitalized to zero.  So make sure it's not zero here.
        //    if (this.ExtendIntervalMultiplier == 0)
        //        this.ExtendIntervalMultiplier = DefaultExtendIntervalMultiplier;
        //    // This value was 0 in TrustTracker.ensureUpdateIntervalIsGreaterThanMin.  We need it not to be.
        //    if (this.MinUpdateIntervalSeconds == 0)
        //        this.MinUpdateIntervalSeconds = DefaultMinUpdateIntervalSeconds;
        //    if (this.MaxUpdateIntervalSeconds == 0)
        //        this.MaxUpdateIntervalSeconds = DefaultMaxUpdateIntervalSeconds;
        //}
    }
}
