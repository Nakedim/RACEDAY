using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{
    [Table("Events")]
    public class Events
    {
        [Key]
        public int EventID { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public int OrganiserID { get; set; }
        public int CategoryID { get; set; }
    }
}
