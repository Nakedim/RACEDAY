using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
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

    }
}
