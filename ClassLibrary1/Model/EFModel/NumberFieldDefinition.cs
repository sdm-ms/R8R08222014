namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NumberFieldDefinition
    {
        public Guid NumberFieldDefinitionID { get; set; }

        public Guid FieldDefinitionID { get; set; }

        public decimal? Minimum { get; set; }

        public decimal? Maximum { get; set; }

        public byte DecimalPlaces { get; set; }

        public byte Status { get; set; }

        public virtual FieldDefinition FieldDefinition { get; set; }
    }
}
