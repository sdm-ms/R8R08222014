namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordTextField
    {
        public int SearchWordTextFieldID { get; set; }

        public int TextFieldID { get; set; }

        public int SearchWordID { get; set; }

        public virtual SearchWord SearchWord { get; set; }

        public virtual TextField TextField { get; set; }
    }
}
