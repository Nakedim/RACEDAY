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

            // FIXED: Changed from int to string to match ApplicationUser's string key type
            public string ParticipantID { get; set; } = string.Empty;

            [Required]
            public string FinishTime { get; set; } = string.Empty;

            public int Position { get; set; }

            // Navigation Properties
            [ForeignKey(nameof(RaceID))]
            public virtual Races Race { get; set; } = null!;

            [ForeignKey(nameof(ParticipantID))]
            public virtual ApplicationUser Participant { get; set; } = null!;
        }

}

