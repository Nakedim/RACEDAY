using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;

namespace RaceDay.Api.Models
{

    public enum ProfileRole
    {
        Organiser,
        Participant
    }
    public class ApplicationUser: IdentityUser
        {
         public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public ProfileRole ProfileRole { get; set; } = ProfileRole.Participant;
    }

}
