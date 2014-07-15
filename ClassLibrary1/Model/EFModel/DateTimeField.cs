namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DateTimeField
    {
        public int DateTimeFieldID { get; set; }

        public int FieldID { get; set; }

        public DateTime? DateTime { get; set; }

        public byte Status { get; set; }

        public virtual Field Field { get; set; }
    }
}
