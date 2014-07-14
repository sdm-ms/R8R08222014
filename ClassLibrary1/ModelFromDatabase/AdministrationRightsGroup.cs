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
    
    public partial class AdministrationRightsGroup
    {
        public AdministrationRightsGroup()
        {
            this.AdministrationRights = new HashSet<AdministrationRight>();
            this.UsersAdministrationRightsGroups = new HashSet<UsersAdministrationRightsGroup>();
        }
    
        public int AdministrationRightsGroupID { get; set; }
        public Nullable<int> PointsManagerID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Creator { get; set; }
        public byte Status { get; set; }
    
        public virtual ICollection<AdministrationRight> AdministrationRights { get; set; }
        public virtual ICollection<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups { get; set; }
        public virtual PointsManager PointsManager { get; set; }
    }
}
