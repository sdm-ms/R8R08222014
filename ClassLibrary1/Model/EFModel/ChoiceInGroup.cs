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
            ChoiceInGroupsDependentOnThisChoiceInGroup = new HashSet<ChoiceInGroup>();
            SearchWordChoices = new HashSet<SearchWordChoice>();
            TrustTrackerForChoiceInGroups = new HashSet<TrustTrackerForChoiceInGroup>();
        }

        public Guid ChoiceInGroupID { get; set; }

        public Guid ChoiceGroupID { get; set; }

        public int ChoiceNum { get; set; }

        [Required]
        [StringLength(50)]
        public string ChoiceText { get; set; }

        public Guid? ActiveOnDeterminingGroupChoiceInGroupID { get; set; }

        public byte Status { get; set; }

        public virtual ChoiceGroup ChoiceGroup { get; set; }

        public virtual ICollection<ChoiceInField> ChoiceInFields { get; set; }

        public virtual ICollection<ChoiceInGroup> ChoiceInGroupsDependentOnThisChoiceInGroup { get; set; }

        public virtual ChoiceInGroup ChoiceInGroupOnWhichThisChoiceInGroupIsDependent { get; set; }

        public virtual ICollection<SearchWordChoice> SearchWordChoices { get; set; }

        public virtual ICollection<TrustTrackerForChoiceInGroup> TrustTrackerForChoiceInGroups { get; set; }
    }
}
