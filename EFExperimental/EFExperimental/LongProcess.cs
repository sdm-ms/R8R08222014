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
    
    public partial class LongProcess
    {
        public int LongProcessID { get; set; }
        public int TypeOfProcess { get; set; }
        public Nullable<int> Object1ID { get; set; }
        public Nullable<int> Object2ID { get; set; }
        public int Priority { get; set; }
        public byte[] AdditionalInfo { get; set; }
        public Nullable<int> ProgressInfo { get; set; }
        public bool Started { get; set; }
        public bool Complete { get; set; }
        public bool ResetWhenComplete { get; set; }
        public Nullable<int> DelayBeforeRestart { get; set; }
        public Nullable<System.DateTime> EarliestRestart { get; set; }
    }
}