using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{
    [Table("Results")]
    public class RaceResults
    {
        [Key]
        public int ResultID { get; set; }
        public int ParticipantID { get; set; }

        public int Position { get; set; }

        public TimeOnly? finishTime { get; set; }
        public int RaceID { get; set; }



    }
}
