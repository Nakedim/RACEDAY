using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("Organiser")]
    public class Organiser
    {
        [Key]
        public int OrganiserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;


        // is used to define a one-to-many relationship between the Organiser and RaceEvent entities
        public ICollection<RaceEvent> RaceEvents { get; set; } = new List<RaceEvent>();

    }
}