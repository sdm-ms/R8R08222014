namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TblRowFieldDisplay
    {
        public TblRowFieldDisplay()
        {
            TblRows = new HashSet<TblRow>();
        }

        public int TblRowFieldDisplayID { get; set; }

        public string Row { get; set; }

        public string PopUp { get; set; }

        public string TblRowPage { get; set; }

        public bool ResetNeeded { get; set; }

        public virtual ICollection<TblRow> TblRows { get; set; }
    }
}
