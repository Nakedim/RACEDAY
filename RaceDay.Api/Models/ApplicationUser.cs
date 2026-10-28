using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;

namespace RaceDay.Api.Models
{

    public enum ProfileRole
    {
        Organiser = 0,
        Participant =1
    }
    public class ApplicationUser : IdentityUser<int>
    {
        public string Location { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public int Age { get; set; }

        public ProfileRole profileRole { get; set; } = ProfileRole.Participant;
     
        
      
    }

}
