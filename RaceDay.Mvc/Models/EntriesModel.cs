
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class EntriesModel
    {
        [Required(ErrorMessage = "Please select a race event.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid event.")]
        public int RaceEventID { get; set; }

        public int ParticipantID { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryID { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
