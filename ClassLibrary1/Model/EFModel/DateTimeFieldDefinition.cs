namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DateTimeFieldDefinition
    {
        public Guid DateTimeFieldDefinitionID { get; set; }

        public Guid FieldDefinitionID { get; set; }

        public bool IncludeDate { get; set; }

        public bool IncludeTime { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual FieldDefinition FieldDefinition { get; set; }
    }
}
