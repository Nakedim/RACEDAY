using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.CreateEventRequest_
{
   public class EventsCreateDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        public required string Title { get; set; }

        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public required string Location { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryID { get; set; } // Keeps its integer value as it maps to a categories table

        [Required(ErrorMessage = "Event Type is required.")]
        public required string EventType { get; set; }


        [Required(ErrorMessage = "Organiser ID is required.")]
        public required int OrganiserID { get; set; }

    }
}
