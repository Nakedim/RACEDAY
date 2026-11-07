namespace RaceDay.Mvc.Models
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
            public string Role { get; set; } = string.Empty; // Holds "Organiser" or "Participant"
        }
    }
