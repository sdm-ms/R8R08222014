namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChoiceInField
    {
        public Guid ChoiceInFieldID { get; set; }

        public Guid ChoiceFieldID { get; set; }

        public Guid ChoiceInGroupID { get; set; }

        public byte Status { get; set; }

        public virtual ChoiceField ChoiceField { get; set; }

        public virtual ChoiceInGroup ChoiceInGroup { get; set; }
    }
}
