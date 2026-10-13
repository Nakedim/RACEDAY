using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("RaceEntries")]
    public class RaceEntries
    {
        [Key]
        public int RaceEntryID { get; set; }

        public int RaceID { get; set; }
        [ForeignKey("RaceID")]
        public Races Race { get; set; } = null!;

        public int ParticipantID { get; set; }
        [ForeignKey("ParticipantID")]
        public Participant Participant { get; set; } = null!;

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Categories Categories { get; set; } = null!; // Pluralized to match your Fluent API (re.Categories)

        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }
    }

