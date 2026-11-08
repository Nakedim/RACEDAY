using Microsoft.IdentityModel.SecurityTokenService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.RacesEntry
{
    public class RaceEntriesDTO
    {
        [Key]
        public int RaceEntryID { get; set; }

        public int RaceID { get; set; }
        // Optional: Add Race navigation property if needed

        public int ParticipantID { get; set; }
        [ForeignKey("ParticipantID")]
        public virtual Participants Participant { get; set; } = null!; // Relational link

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual CategoriesDTO Category { get; set; } = null!; // Relational link
        public string ParticipantName { get; set; } = string.Empty;

        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }
}
