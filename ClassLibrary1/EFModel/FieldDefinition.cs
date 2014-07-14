namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FieldDefinition
    {
        public FieldDefinition()
        {
            ChoiceGroupFieldDefinitions = new HashSet<ChoiceGroupFieldDefinition>();
            DateTimeFieldDefinitions = new HashSet<DateTimeFieldDefinition>();
            Fields = new HashSet<Field>();
            NumberFieldDefinitions = new HashSet<NumberFieldDefinition>();
            TextFieldDefinitions = new HashSet<TextFieldDefinition>();
        }

        public int FieldDefinitionID { get; set; }

        public int TblID { get; set; }

        public int FieldNum { get; set; }

        [Required]
        [StringLength(50)]
        public string FieldName { get; set; }

        public int FieldType { get; set; }

        public bool UseAsFilter { get; set; }

        public bool AddToSearchWords { get; set; }

        public int DisplayInTableSettings { get; set; }

        public int DisplayInPopupSettings { get; set; }

        public int DisplayInTblRowPageSettings { get; set; }

        public byte Status { get; set; }

        public int NumNonNull { get; set; }

        public double ProportionNonNull { get; set; }

        public bool UsingNonSparseColumn { get; set; }

        public bool ShouldUseNonSparseColumn { get; set; }

        public virtual ICollection<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions { get; set; }

        public virtual ICollection<DateTimeFieldDefinition> DateTimeFieldDefinitions { get; set; }

        public virtual ICollection<Field> Fields { get; set; }

        public virtual ICollection<NumberFieldDefinition> NumberFieldDefinitions { get; set; }

        public virtual ICollection<TextFieldDefinition> TextFieldDefinitions { get; set; }

        public virtual Tbl Tbl { get; set; }
    }
}
