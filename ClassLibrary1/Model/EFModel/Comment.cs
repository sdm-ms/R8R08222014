namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comment
    {
        [Key]
        public int CommentsID { get; set; }

        public int TblRowID { get; set; }

        public int UserID { get; set; }

        [Required]
        public string CommentTitle { get; set; }

        [Required]
        public string CommentText { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? LastDeletedDate { get; set; }

        public byte Status { get; set; }

        public virtual TblRow TblRow { get; set; }

        public virtual User User { get; set; }
    }
}
