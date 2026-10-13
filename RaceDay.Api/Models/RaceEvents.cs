using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("RaceEvents")]
    public class RaceEvent
    {
        [Key]
        public int EventID { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public int OrganiserID { get; set; }
        public int CategoryID { get; set; }
        public int Distance { get; set; }
        public string EventType { get; set; }


        [ForeignKey("OrganiserID")]
        public Organiser Organiser { get; set; } = null!;

        // Navigation property for children races
        public ICollection<Races> Races { get; set; } = new List<Races>();
    }
}
