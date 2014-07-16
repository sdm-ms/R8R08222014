namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumPollOption
    {
        [Key]
        public int OptionID { get; set; }

        public int PollID { get; set; }

        [Required]
        [StringLength(50)]
        public string OptionText { get; set; }
    }
}
