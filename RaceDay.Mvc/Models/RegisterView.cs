using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class RegisterView
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string Surname { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required, DataType(DataType.PhoneNumber)] public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)] public DateTime DateOfBirth { get; set; }
        [Required] public string Role { get; set; } = "Participant";

    }
}
