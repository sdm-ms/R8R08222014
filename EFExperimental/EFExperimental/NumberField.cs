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
    
    public partial class NumberField
    {
        public int NumberFieldID { get; set; }
        public int FieldID { get; set; }
        public Nullable<decimal> Number { get; set; }
        public byte Status { get; set; }
    
        public virtual Field Field { get; set; }
    }
}
