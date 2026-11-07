
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class LoginView
    {
        private string _email = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email
        {
            get => _email;
            set => _email = value?.Trim() ?? string.Empty;
        }

        // Compatibility with existing controller and API client.
        public string Username
        {
            get => Email;
            set => Email = value;
        }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}

