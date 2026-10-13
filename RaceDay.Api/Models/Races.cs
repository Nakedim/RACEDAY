using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{

    [Table("Races")]
    public class Races
    {
        [Key]
        public int RaceID { get; set; }
        public string RaceName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int EventID { get; set; }
        public string Location { get; set; } = string.Empty;

        [ForeignKey("EventID")]
        public RaceEvent Event { get; set; } = null!;

        public ICollection<RaceEntries> RaceEntries { get; set; } = new List<RaceEntries>();

    }
}
