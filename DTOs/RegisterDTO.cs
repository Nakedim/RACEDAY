using System.ComponentModel.DataAnnotations;

namespace RACEDAY.DTOs
{
    public class RegisterDTO
    {


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "select atleast two characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string UserRole { get; set; }



    }
}