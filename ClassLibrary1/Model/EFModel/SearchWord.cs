namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWord
    {
        public SearchWord()
        {
            SearchWordChoices = new HashSet<SearchWordChoice>();
            SearchWordHierarchyItems = new HashSet<SearchWordHierarchyItem>();
            SearchWordTblRowNames = new HashSet<SearchWordTblRowName>();
            SearchWordTextFields = new HashSet<SearchWordTextField>();
        }

        public Guid SearchWordID { get; set; }

        [Required]
        public string TheWord { get; set; }

        public virtual ICollection<SearchWordChoice> SearchWordChoices { get; set; }

        public virtual ICollection<SearchWordHierarchyItem> SearchWordHierarchyItems { get; set; }

        public virtual ICollection<SearchWordTblRowName> SearchWordTblRowNames { get; set; }

        public virtual ICollection<SearchWordTextField> SearchWordTextFields { get; set; }
    }
}
