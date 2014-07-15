namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordChoice
    {
        public int SearchWordChoiceID { get; set; }

        public int ChoiceInGroupID { get; set; }

        public int SearchWordID { get; set; }

        public virtual ChoiceInGroup ChoiceInGroup { get; set; }

        public virtual SearchWord SearchWord { get; set; }
    }
}
