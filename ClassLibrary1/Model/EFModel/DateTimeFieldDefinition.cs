namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DateTimeFieldDefinition
    {
        public int DateTimeFieldDefinitionID { get; set; }

        public int FieldDefinitionID { get; set; }

        public bool IncludeDate { get; set; }

        public bool IncludeTime { get; set; }

        public byte Status { get; set; }

        public virtual FieldDefinition FieldDefinition { get; set; }
    }
}
