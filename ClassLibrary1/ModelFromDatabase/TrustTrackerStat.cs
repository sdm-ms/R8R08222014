//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClassLibrary1.ModelFromDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrustTrackerStat
    {
        public TrustTrackerStat()
        {
            this.UserInteractionStats = new HashSet<UserInteractionStat>();
        }
    
        public int TrustTrackerStatID { get; set; }
        public int TrustTrackerID { get; set; }
        public short StatNum { get; set; }
        public double TrustValue { get; set; }
        public double Trust_Numer { get; set; }
        public double Trust_Denom { get; set; }
        public double SumUserInteractionStatWeights { get; set; }
    
        public virtual TrustTracker TrustTracker { get; set; }
        public virtual ICollection<UserInteractionStat> UserInteractionStats { get; set; }
    }
}
