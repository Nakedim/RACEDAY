using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.ParticipantDTO
{
    public class ParticipanregisterDTO
    {
     
            [Required]
            public required string FirstName { get; set; }

            [Required]
            public string Surname { get; set; } = string.Empty;

           [Phone]    
           public string PhoneNumber { get; set; } = string.Empty;

            public string Location { get; set; } = string.Empty;

        [Range(12, 120)]
        public int Age { get; set; }

            [Required(ErrorMessage = "Email address is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address format.")]
            public required string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Password must be at least 2 characters long.")]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
            [Required(ErrorMessage = "Confirm password is required.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public required string ConfirmPassword { get; set; }

          

        }
    }

