using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.apiResponseprofile
{
    public class ApiResponseProfile
    {
        public string Token { get; set; } = string.Empty;

        public UserProfileInfo? User { get; set; }
    }

    public class UserProfileInfo
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
