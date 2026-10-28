using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("Organiser")]
    public class Organiser
    {
        [Key]
        [Column("OrganiserID")]
        public int OrganiserID { get; set; }

        public int ApplicationUserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public string FirstName { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public ICollection<RaceEvent> RaceEvents { get; set; } = new List<RaceEvent>();
    }
}
