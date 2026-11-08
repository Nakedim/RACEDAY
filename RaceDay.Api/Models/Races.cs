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

        public int CategoryID { get; set; }

        [ForeignKey(nameof(CategoryID))]
        public Categories? Category { get; set; }

        public DateTime StartTime { get; set; }

        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public RaceEvent Event { get; set; } = null!;

        public string Location { get; set; } = string.Empty;

        public int Distance { get; set; }


        public string? OrganiserID { get; set; }
    }
}
