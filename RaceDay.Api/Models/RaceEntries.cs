   
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace RaceDay.Api.Models
    {
        [Table("RaceEntries")]
        public class RaceEntries
        {
            [Key]
        public int RaceEntryID { get; set; }
        // Foreign key to RaceEvent
        public int RaceEventID { get; set; }

        [ForeignKey(nameof(RaceEventID))]
        public virtual RaceEvent RaceEvent { get; set; } = null!;

        // Foreign key to ApplicationUser
        public int ParticipantID { get; set; } 

        [ForeignKey(nameof(ParticipantID))]
        public virtual ApplicationUser Participant { get; set; } = null!;

        // Foreign key to Category
        public int CategoryID { get; set; }

        [ForeignKey(nameof(CategoryID))]
        public virtual Categories Category { get; set; } = null!;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        
        }

    }

