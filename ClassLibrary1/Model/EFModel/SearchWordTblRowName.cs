namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordTblRowName
    {
        public Guid SearchWordTblRowNameID { get; set; }

        public Guid TblRowID { get; set; }

        public Guid SearchWordID { get; set; }

        public virtual SearchWord SearchWord { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
