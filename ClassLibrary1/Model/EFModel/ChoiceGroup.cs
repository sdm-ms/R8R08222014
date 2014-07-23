namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChoiceGroup
    {
        public ChoiceGroup()
        {
            ChoiceGroupFieldDefinitions = new HashSet<ChoiceGroupFieldDefinition>();
            ChoiceInGroups = new HashSet<ChoiceInGroup>();
            ChoiceGroupsDependentOnThisChoiceGroup = new HashSet<ChoiceGroup>();
        }

        public Guid ChoiceGroupID { get; set; }

        public Guid PointsManagerID { get; set; }

        public bool AllowMultipleSelections { get; set; }

        public bool Alphabetize { get; set; }

        public bool InvisibleWhenEmpty { get; set; }

        public bool ShowTagCloud { get; set; }

        public bool PickViaAutoComplete { get; set; }

        public Guid? DependentOnChoiceGroupID { get; set; }

        public bool ShowAllPossibilitiesIfNoDependentChoice { get; set; }

        public bool AlphabetizeWhenShowingAllPossibilities { get; set; }

        public bool AllowAutoAddWhenAddingFields { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public Guid? Creator { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual ICollection<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions { get; set; }

        public virtual ICollection<ChoiceInGroup> ChoiceInGroups { get; set; }

        public virtual ICollection<ChoiceGroup> ChoiceGroupsDependentOnThisChoiceGroup { get; set; }

        public virtual ChoiceGroup ChoiceGroupOnWhichThisChoiceGroupIsDependent { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual User User { get; set; }
    }
}
