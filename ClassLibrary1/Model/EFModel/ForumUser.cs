namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumUser
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Homepage { get; set; }

        [StringLength(255)]
        public string Interests { get; set; }

        public int PostsCount { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool Disabled { get; set; }

        [Required]
        [StringLength(50)]
        public string ActivationCode { get; set; }

        [StringLength(50)]
        public string AvatarFileName { get; set; }

        [StringLength(1000)]
        public string Signature { get; set; }

        public DateTime? LastLogonDate { get; set; }

        public int ReputationCache { get; set; }

        [StringLength(255)]
        public string OpenIdUserName { get; set; }
    }
}
