namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChoiceGroupFieldDefinition
    {
        public ChoiceGroupFieldDefinition()
        {
            ChoiceGroupFieldDefinitions1 = new HashSet<ChoiceGroupFieldDefinition>();
        }

        public Guid ChoiceGroupFieldDefinitionID { get; set; }

        public Guid ChoiceGroupID { get; set; }

        public Guid FieldDefinitionID { get; set; }

        public Guid? DependentOnChoiceGroupFieldDefinitionID { get; set; }

        public bool TrackTrustBasedOnChoices { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual ChoiceGroup ChoiceGroup { get; set; }

        public virtual FieldDefinition FieldDefinition { get; set; }

        public virtual ICollection<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions1 { get; set; }

        public virtual ChoiceGroupFieldDefinition ChoiceGroupFieldDefinition1 { get; set; }
    }
}
