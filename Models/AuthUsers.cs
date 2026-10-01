using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{
    public enum ProfileRole
    {
        Organiser,
        Participant
    }
    [Table("AuthUsers")]
    public class AuthUsers
    {
        [Key]
        public int UserID { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public ProfileRole profileRole { get; set; } = ProfileRole.Participant;


    }
}
