using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RACEDAY.Models;
namespace RACEDAY.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }
            //tables
            //UserID, Username, PasswordHash, Email
            public DbSet<AuthUsers> authUsers { get; set; }

    }
}
