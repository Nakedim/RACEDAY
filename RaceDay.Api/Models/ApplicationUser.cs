using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
        public class ApplicationUser: IdentityUser
        {
             public string FirstName { get; set; }
             public string Surname { get; set; }
            public ProfileRole profileRole { get; internal set; }
        }

}
