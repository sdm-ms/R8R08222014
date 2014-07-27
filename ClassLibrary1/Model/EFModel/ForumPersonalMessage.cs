namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumPersonalMessage
    {
        [Key]
        public int MessageID { get; set; }

        public int FromUserID { get; set; }

        public int ToUserID { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Body { get; set; }

        public DateTime CreationDate { get; set; }

        public bool New { get; set; }
    }
}
