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
    
    public partial class TextFieldDefinition
    {
        public int TextFieldDefinitionID { get; set; }
        public int FieldDefinitionID { get; set; }
        public bool IncludeText { get; set; }
        public bool IncludeLink { get; set; }
        public bool Searchable { get; set; }
        public byte Status { get; set; }
    
        public virtual FieldDefinition FieldDefinition { get; set; }
    }
}
