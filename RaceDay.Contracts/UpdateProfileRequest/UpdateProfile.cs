using System.ComponentModel.DataAnnotations;

namespace RaceDay.Contracts.UpdateProfileRequest;

public class UpdateProfileRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Bio { get; set; } // Optional field

    public string? ProfilePictureUrl { get; set; } // Optional field
}
