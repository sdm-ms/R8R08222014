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
    
    public partial class TblRowStatusRecord
    {
        public int RecordId { get; set; }
        public int TblRowId { get; set; }
        public System.DateTime TimeChanged { get; set; }
        public bool Adding { get; set; }
        public bool Deleting { get; set; }
    
        public virtual TblRow TblRow { get; set; }
    }
}
