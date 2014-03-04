//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFExperimental
{
    using System;
    using System.Collections.Generic;
    
    public partial class FieldDefinition
    {
        public FieldDefinition()
        {
            this.ChoiceGroupFieldDefinitions = new HashSet<ChoiceGroupFieldDefinition>();
            this.DateTimeFieldDefinitions = new HashSet<DateTimeFieldDefinition>();
            this.Fields = new HashSet<Field>();
            this.NumberFieldDefinitions = new HashSet<NumberFieldDefinition>();
            this.TextFieldDefinitions = new HashSet<TextFieldDefinition>();
        }
    
        public int FieldDefinitionID { get; set; }
        public int TblID { get; set; }
        public int FieldNum { get; set; }
        public string FieldName { get; set; }
        public int FieldType { get; set; }
        public bool UseAsFilter { get; set; }
        public bool AddToSearchWords { get; set; }
        public int DisplayInTableSettings { get; set; }
        public int DisplayInPopupSettings { get; set; }
        public int DisplayInTblRowPageSettings { get; set; }
        public byte Status { get; set; }
    
        public virtual ICollection<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions { get; set; }
        public virtual ICollection<DateTimeFieldDefinition> DateTimeFieldDefinitions { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<NumberFieldDefinition> NumberFieldDefinitions { get; set; }
        public virtual ICollection<TextFieldDefinition> TextFieldDefinitions { get; set; }
        public virtual Tbl Tbl { get; set; }
    }
}
