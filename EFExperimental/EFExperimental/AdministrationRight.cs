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
    
    public partial class AdministrationRight
    {
        public int AdministrationRightID { get; set; }
        public int AdministrationRightsGroupID { get; set; }
        public Nullable<int> UserActionID { get; set; }
        public bool AllowUserToMakeImmediateChanges { get; set; }
        public bool AllowUserToMakeProposals { get; set; }
        public bool AllowUserToSeekRewards { get; set; }
        public bool AllowUserNotToSeekRewards { get; set; }
        public byte Status { get; set; }
    
        public virtual AdministrationRightsGroup AdministrationRightsGroup { get; set; }
        public virtual UserAction UserAction { get; set; }
    }
}
