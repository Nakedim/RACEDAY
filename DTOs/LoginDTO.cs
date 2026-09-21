using System.ComponentModel.DataAnnotations;

namespace RACEDAY.DTOs
{
    public class LoginDTO
    {
        
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHashed { get; set; }
    }
}
