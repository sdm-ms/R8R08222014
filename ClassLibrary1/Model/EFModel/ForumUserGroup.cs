namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumUserGroup
    {
        [Key]
        public int GroupID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
    }
}
