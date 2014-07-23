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
        public Guid CommentID { get; set; }

        public Guid TblRowID { get; set; }

        public Guid UserID { get; set; }

        [Required]
        public string CommentTitle { get; set; }

        [Required]
        public string CommentText { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? LastDeletedDate { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual TblRow TblRow { get; set; }

        public virtual User User { get; set; }
    }
}
