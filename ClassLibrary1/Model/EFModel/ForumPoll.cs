namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumPoll
    {
        [Key]
        public int PollID { get; set; }

        public int TopicID { get; set; }

        [Required]
        [StringLength(255)]
        public string Question { get; set; }
    }
}
