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

            public int ParticipantID { get; set; }

            [Required]
            public string FinishTime { get; set; } = string.Empty;

            public int Position { get; set; }

            // Navigation Properties
            [ForeignKey(nameof(RaceID))]
            public virtual Races Race { get; set; } = null!;

            [ForeignKey(nameof(ParticipantID))]
            public virtual Participant Participant { get; set; } = null!;
        }

    }

