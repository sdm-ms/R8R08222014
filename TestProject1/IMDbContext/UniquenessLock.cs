namespace TestProject1.IMDbContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UniquenessLock
    {
        public UniquenessLock()
        {
            UniquenessLockReferences = new HashSet<UniquenessLockReference>();
        }

           [Key]
        public Guid Id { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<UniquenessLockReference> UniquenessLockReferences { get; set; }
    }
}
