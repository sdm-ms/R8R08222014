namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Field
    {
        public Field()
        {
            AddressFields = new HashSet<AddressField>();
            ChoiceFields = new HashSet<ChoiceField>();
            DateTimeFields = new HashSet<DateTimeField>();
            NumberFields = new HashSet<NumberField>();
            TextFields = new HashSet<TextField>();
        }

        public Guid FieldID { get; set; }

        public Guid TblRowID { get; set; }

        public Guid FieldDefinitionID { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<AddressField> AddressFields { get; set; }

        public virtual ICollection<ChoiceField> ChoiceFields { get; set; }

        public virtual ICollection<DateTimeField> DateTimeFields { get; set; }

        public virtual FieldDefinition FieldDefinition { get; set; }

        public virtual ICollection<NumberField> NumberFields { get; set; }

        public virtual ICollection<TextField> TextFields { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
