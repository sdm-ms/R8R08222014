namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordTextField
    {
        public Guid SearchWordTextFieldID { get; set; }

        public Guid TextFieldID { get; set; }

        public Guid SearchWordID { get; set; }

        public virtual SearchWord SearchWord { get; set; }

        public virtual TextField TextField { get; set; }
    }
}
