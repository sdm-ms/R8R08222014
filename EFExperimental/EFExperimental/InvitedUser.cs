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
    
    public partial class InvitedUser
    {
        public int ActivationNumber { get; set; }
        public string EmailId { get; set; }
        public bool MayView { get; set; }
        public bool MayPredict { get; set; }
        public bool MayAddTbls { get; set; }
        public bool MayResolveRatings { get; set; }
        public bool MayChangeTblRows { get; set; }
        public bool MayChangeChoiceGroups { get; set; }
        public bool MayChangeCharacteristics { get; set; }
        public bool MayChangeCategories { get; set; }
        public bool MayChangeUsersRights { get; set; }
        public bool MayAdjustPoints { get; set; }
        public bool MayChangeProposalSettings { get; set; }
        public bool IsRegistered { get; set; }
    }
}
