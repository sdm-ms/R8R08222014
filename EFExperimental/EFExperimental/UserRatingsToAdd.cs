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
    
    public partial class UserRatingsToAdd
    {
        public int UserRatingsToAddID { get; set; }
        public int UserID { get; set; }
        public int TopRatingGroupID { get; set; }
        public byte[] UserRatingHierarchy { get; set; }
    
        public virtual RatingGroup RatingGroup { get; set; }
        public virtual User User { get; set; }
    }
}
