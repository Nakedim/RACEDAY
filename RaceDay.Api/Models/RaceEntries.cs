using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{

    using System;
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

            [ForeignKey(nameof(RaceID))]
            public Races Race { get; set; } = null!;

            public int ParticipantID { get; set; }

            [ForeignKey(nameof(ParticipantID))]
            public Participant Participant { get; set; } = null!;

            public int CategoryID { get; set; }

            [ForeignKey(nameof(CategoryID))]
            public Categories Categories { get; set; } = null!;

            public DateTime EntryDate { get; set; } = DateTime.UtcNow;
        }
    }
}
