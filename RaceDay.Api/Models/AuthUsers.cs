using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
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
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHashed { get; set; } = string.Empty;
        [Required]
        public ProfileRole profileRole { get; set; } = ProfileRole.Participant;
        // Optional Navigation Properties (makes fetching profile data easier in EF Core)
        //public virtual Participant? Participant { get; set; }
        //public virtual Organiser? Organiser { get; set; }


    }
}
