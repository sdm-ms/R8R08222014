namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChoiceField
    {
        public ChoiceField()
        {
            ChoiceInFields = new HashSet<ChoiceInField>();
        }

        public int ChoiceFieldID { get; set; }

        public int FieldID { get; set; }

        public byte Status { get; set; }

        public virtual Field Field { get; set; }

        public virtual ICollection<ChoiceInField> ChoiceInFields { get; set; }
    }
}
