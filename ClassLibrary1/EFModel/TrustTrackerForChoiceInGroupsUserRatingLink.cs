namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TrustTrackerForChoiceInGroupsUserRatingLink
    {
        public int TrustTrackerForChoiceInGroupsUserRatingLinkID { get; set; }

        public int UserRatingID { get; set; }

        public int TrustTrackerForChoiceInGroupID { get; set; }

        public virtual TrustTrackerForChoiceInGroup TrustTrackerForChoiceInGroup { get; set; }

        public virtual UserRating UserRating { get; set; }
    }
}
