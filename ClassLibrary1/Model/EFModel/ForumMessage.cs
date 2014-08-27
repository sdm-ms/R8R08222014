namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumMessage
    {
        [Key]
        public int MessageID { get; set; }

        public int TopicID { get; set; }

        public int UserID { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Body { get; set; }

        public DateTime CreationDate { get; set; }

        public bool Visible { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        public int Rating { get; set; }
    }
}
