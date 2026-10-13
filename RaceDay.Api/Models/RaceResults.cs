using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("Results")]
    public class RaceResults
    {
        [Key]
        public int ResultID { get; set; }

        public int RaceID { get; set; }

        [ForeignKey("RaceID")]
        public Races race { get; set; } = null!; 
        public int ParticipantID { get; set; }
        [ForeignKey("ParticipantID")]
        public Participant Participant { get; set; } = null!;

        [Required]
        public string FinishTime { get; set; } = string.Empty;

        public int Position { get; set; }

    }
}
