namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChoiceInGroup
    {
        public ChoiceInGroup()
        {
            ChoiceInFields = new HashSet<ChoiceInField>();
            ChoiceInGroups1 = new HashSet<ChoiceInGroup>();
            SearchWordChoices = new HashSet<SearchWordChoice>();
            TrustTrackerForChoiceInGroups = new HashSet<TrustTrackerForChoiceInGroup>();
        }

        public int ChoiceInGroupID { get; set; }

        public int ChoiceGroupID { get; set; }

        public int ChoiceNum { get; set; }

        [Required]
        [StringLength(50)]
        public string ChoiceText { get; set; }

        public int? ActiveOnDeterminingGroupChoiceInGroupID { get; set; }

        public byte Status { get; set; }

        public virtual ChoiceGroup ChoiceGroup { get; set; }

        public virtual ICollection<ChoiceInField> ChoiceInFields { get; set; }

        public virtual ICollection<ChoiceInGroup> ChoiceInGroups1 { get; set; }

        public virtual ChoiceInGroup ChoiceInGroup1 { get; set; }

        public virtual ICollection<SearchWordChoice> SearchWordChoices { get; set; }

        public virtual ICollection<TrustTrackerForChoiceInGroup> TrustTrackerForChoiceInGroups { get; set; }
    }
}
