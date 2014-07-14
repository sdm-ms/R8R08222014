namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Domain
    {
        public Domain()
        {
            InsertableContents = new HashSet<InsertableContent>();
            PointsManagers = new HashSet<PointsManager>();
        }

        public int DomainID { get; set; }

        public bool ActiveRatingWebsite { get; set; }

        [Required]
        public string Name { get; set; }

        public int? TblDimensionsID { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<InsertableContent> InsertableContents { get; set; }

        public virtual ICollection<PointsManager> PointsManagers { get; set; }

        public virtual TblDimension TblDimension { get; set; }
    }
}
