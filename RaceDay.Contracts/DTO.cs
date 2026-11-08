using Microsoft.IdentityModel.SecurityTokenService;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace RaceDay.Contracts;

// encapsulate and transfer data between different layers of an application or across a network

// RegisterRequest, LoginRequest, AuthResponse
// ProfileDto, UpdateProfileRequest
// CreateEventRequest, UpdateEventRequest, EventDto
// CreateCategoryRequest, CategoryDto
// CreateEnrolmentRequest, UpdateEnrolmentStatusRequest, EnrolmentDto
// CreateResultRequest, ResultDto
// UploadResponse for Part 3 image uploads

public class RegisterDTO
{




    [Required]
    public required string FirstName { get; set; }

    [Required]
    public string Surname { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "A user role must be assigned.")]
    public required string ProfileRole { get; set; }

}



public class LoginDTO
{

    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
   

public class RaceDTO

{
    public int RaceID { get; set; }
    [Required(ErrorMessage = "Race name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Race name must be between 3 and 100 characters.")]
    public string RaceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Race date and time are required.")]
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }
    public int EventID { get; set; }
    [Required(ErrorMessage = "Location is required.")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
    public string Location { get; set; } = string.Empty;
    public int Distance { get; set; }


}

public class ResultsRequestDTO
{
    public int ResultsID { get; set; }
    public int ParticipantID { get; set; }

    [Required(ErrorMessage = "Finish Time is required.")]
    [StringLength(200, ErrorMessage = "FinishTime cannot be empty")]
    public DateTime FinishTime { get; set; }

    [Required(ErrorMessage = "Position is required.")]
    [StringLength(200, ErrorMessage = "Position cannot be empty")]
    public string Position { get; set; } = string.Empty;
}

public class ParticipantProfileDTO
{
     public int ParticipantID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Location { get; set; } = string.Empty;
    
}

public class OrganiserProfileDTO
{
    public int OrganiserID { get; set; }
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string Surname { get; set; } = string.Empty;

}

public class OrganiserDTO
{


    public int OrganiserID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

}

public class CategoriesDTO
{
 
    public string CategoryName { get; set; } = string.Empty;


}

public class CreateRaceEventDTO
{

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int OrganiserID { get; set; }
        public int CategoryID { get; set; } 
        public string EventType { get; set; } = string.Empty; 
    
}

