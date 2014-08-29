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
    
    public partial class RatingCharacteristic
    {
        public RatingCharacteristic()
        {
            this.Ratings = new HashSet<Rating>();
            this.RatingGroupAttributes = new HashSet<RatingGroupAttribute>();
        }
    
        public int RatingCharacteristicsID { get; set; }
        public int RatingPhaseGroupID { get; set; }
        public Nullable<int> SubsidyDensityRangeGroupID { get; set; }
        public decimal MinimumUserRating { get; set; }
        public decimal MaximumUserRating { get; set; }
        public byte DecimalPlaces { get; set; }
        public string Name { get; set; }
        public Nullable<int> Creator { get; set; }
        public byte Status { get; set; }
    
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<RatingGroupAttribute> RatingGroupAttributes { get; set; }
        public virtual RatingPhaseGroup RatingPhaseGroup { get; set; }
        public virtual SubsidyDensityRangeGroup SubsidyDensityRangeGroup { get; set; }
        public virtual User User { get; set; }
    }
}