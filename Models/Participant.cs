using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{

    [Table("Participant")]
    public class Participant
    {
        [Key]
        public int ParticipantID {  get; set; }
        public string FirstName {  get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public int Age {  get; set; }
        public string Location { get; set; } = string.Empty;
        public int? UserID { get; set; }

    }
}
