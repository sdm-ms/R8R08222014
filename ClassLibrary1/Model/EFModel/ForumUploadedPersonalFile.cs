namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumUploadedPersonalFile
    {
        [Key]
        public int FileID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        public int MessageID { get; set; }

        public int UserID { get; set; }
    }
}
