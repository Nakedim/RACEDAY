using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class RaceEventModel
    {
        public int EventID { get; set; }

        [Required(ErrorMessage = "Event title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Event type is required.")]
        public string EventType { get; set; } = string.Empty;

        // Set from the logged-in organiser
        public int OrganiserID { get; set; }

    }
}
