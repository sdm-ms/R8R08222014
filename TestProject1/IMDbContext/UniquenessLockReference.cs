namespace TestProject1.IMDbContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UniquenessLockReference
    {
           [Key]
        public Guid Id { get; set; }

        public Guid? UniquenessLockID { get; set; }

        public virtual UniquenessLock UniquenessLock { get; set; }
    }
}
