using Microsoft.AspNet.Identity.EntityFramework;
using ClassLibrary1.Model;

namespace WebRole1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // public string Email { get; set; } // Not needed -- already in base IdentityUser class
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ConnectionString.GetUserProfileDatabaseConnectionString(), throwIfV1Schema: false)
        {
        }
    }
}