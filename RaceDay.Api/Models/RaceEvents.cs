using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("RaceEvents")]
    public class RaceEvent
    {
        [Key]
        public int EventID { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string EventType { get; set; } = string.Empty;

        // 1. Foreign Key property matching the ApplicationUser identity string key
        public string OrganiserID { get; set; } = string.Empty;

        // 2. Navigation property pointing directly to your custom ApplicationUser model
        [ForeignKey("OrganiserID")]
        public ApplicationUser Organiser { get; set; } = null!;

        // 3. Clean navigation collection connecting an event to its registered entries
        public ICollection<RaceEntries> Entries { get; set; } = new List<RaceEntries>();
    }
}
