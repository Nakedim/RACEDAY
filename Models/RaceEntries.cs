using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{
    [Table("RaceEntries")]
    public class RaceEntries
    {
        [Key]
        public int EntryID { get; set; }
        public int ParticipantID { get; set; }
        public int RaceID { get; set; }
        public int EventID { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
