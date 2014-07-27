namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Forums")]
    public partial class Forum
    {
        public int ForumID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public bool Premoderated { get; set; }

        public int GroupID { get; set; }

        public bool MembersOnly { get; set; }

        public int OrderByNumber { get; set; }

        public bool RestrictTopicCreation { get; set; }

        [StringLength(50)]
        public string IconFile { get; set; }
    }
}
