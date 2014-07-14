namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        public int UserInfoID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(250)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Address1 { get; set; }

        [Required]
        [StringLength(200)]
        public string Address2 { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string HomePhone { get; set; }

        [Required]
        [StringLength(50)]
        public string MobilePhone { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string State { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        public bool IsVerified { get; set; }

        public virtual User User { get; set; }
    }
}
