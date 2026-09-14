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
        [Compare("Password", ErrorMessage = "The password must match")]
        public string Password { get; set; }



    }
}