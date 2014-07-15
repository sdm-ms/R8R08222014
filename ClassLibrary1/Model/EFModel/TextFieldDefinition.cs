namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TextFieldDefinition
    {
        public Guid TextFieldDefinitionID { get; set; }

        public Guid FieldDefinitionID { get; set; }

        public bool IncludeText { get; set; }

        public bool IncludeLink { get; set; }

        public bool Searchable { get; set; }

        public byte Status { get; set; }

        public virtual FieldDefinition FieldDefinition { get; set; }
    }
}
