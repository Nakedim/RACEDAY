using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class RaceImage
    {

        [Key]
        public int Id { get; set; } // Your primary key

        [Required]
        public string ImageUrl { get; set; } = string.Empty; // Holds the Azure Blob link

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
