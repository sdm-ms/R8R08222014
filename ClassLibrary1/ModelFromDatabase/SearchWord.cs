//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClassLibrary1.ModelFromDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class SearchWord
    {
        public SearchWord()
        {
            this.SearchWordChoices = new HashSet<SearchWordChoice>();
            this.SearchWordHierarchyItems = new HashSet<SearchWordHierarchyItem>();
            this.SearchWordTblRowNames = new HashSet<SearchWordTblRowName>();
            this.SearchWordTextFields = new HashSet<SearchWordTextField>();
        }
    
        public int SearchWordID { get; set; }
        public string TheWord { get; set; }
    
        public virtual ICollection<SearchWordChoice> SearchWordChoices { get; set; }
        public virtual ICollection<SearchWordHierarchyItem> SearchWordHierarchyItems { get; set; }
        public virtual ICollection<SearchWordTblRowName> SearchWordTblRowNames { get; set; }
        public virtual ICollection<SearchWordTextField> SearchWordTextFields { get; set; }
    }
}
