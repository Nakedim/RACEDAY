
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

       
        public required int OrganiserID { get; set; }

      
        [ForeignKey(nameof(OrganiserID))]
        public Organiser? Organiser { get; set; }

        public ICollection<RaceEntries> Entries { get; set; } = new List<RaceEntries>();
    }
}
