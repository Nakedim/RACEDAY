using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{
    [Table("AuthUsers")]
    public class AuthUsers
    {
        [Key]
        public int UserID { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;


    }
}
