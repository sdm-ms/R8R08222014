namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordTblRowName
    {
        public int SearchWordTblRowNameID { get; set; }

        public int TblRowID { get; set; }

        public int SearchWordID { get; set; }

        public virtual SearchWord SearchWord { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
