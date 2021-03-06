//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFExperimental
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserInteraction
    {
        public UserInteraction()
        {
            this.UserInteractionStats = new HashSet<UserInteractionStat>();
        }
    
        public int UserInteractionID { get; set; }
        public int TrustTrackerUnitID { get; set; }
        public int OrigRatingUserID { get; set; }
        public int LatestRatingUserID { get; set; }
        public int NumTransactions { get; set; }
        public double LatestUserEgalitarianTrust { get; set; }
        public double WeightInCalculatingTrustTotal { get; set; }
        public Nullable<double> LatestUserEgalitarianTrustAtLastWeightUpdate { get; set; }
    
        public virtual TrustTrackerUnit TrustTrackerUnit { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<UserInteractionStat> UserInteractionStats { get; set; }
    }
}
