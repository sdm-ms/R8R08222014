namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NumberField
    {
        public Guid NumberFieldID { get; set; }

        public Guid FieldID { get; set; }

        public decimal? Number { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual Field Field { get; set; }
    }
}
