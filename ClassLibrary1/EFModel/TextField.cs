namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TextField
    {
        public TextField()
        {
            SearchWordTextFields = new HashSet<SearchWordTextField>();
        }

        public int TextFieldID { get; set; }

        public int FieldID { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Link { get; set; }

        public byte Status { get; set; }

        public virtual Field Field { get; set; }

        public virtual ICollection<SearchWordTextField> SearchWordTextFields { get; set; }
    }
}
