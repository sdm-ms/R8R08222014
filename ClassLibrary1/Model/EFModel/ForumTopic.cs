namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumTopic
    {
        [Key]
        public int TopicID { get; set; }

        public int ForumID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(255)]
        public string Subject { get; set; }

        public bool Visible { get; set; }

        public int LastMessageID { get; set; }

        public int IsSticky { get; set; }

        public bool IsClosed { get; set; }

        public int ViewsCount { get; set; }

        public int RepliesCount { get; set; }
    }
}
