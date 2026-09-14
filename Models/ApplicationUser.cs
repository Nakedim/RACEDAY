using Microsoft.AspNetCore.Identity;

namespace RACEDAY.Models
{
    public class ApplicationUser: IdentityUser
    {
         public string FirstName { get; set; }
         public string Surname { get; set; }
    }
}
