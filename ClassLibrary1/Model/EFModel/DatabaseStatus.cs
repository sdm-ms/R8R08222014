namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DatabaseStatus
    {
        [Key]
        public Guid DatabaseStatusID { get; set; }

        public bool PreventChanges { get; set; }
    }
}
