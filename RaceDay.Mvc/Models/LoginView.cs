using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class LoginView
    {
        [Required, EmailAddress]
        public string Username { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
