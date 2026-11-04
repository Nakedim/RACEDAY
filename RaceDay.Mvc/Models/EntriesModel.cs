using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Mvc.Models
{
    public class EntriesModel
    { 
     
        public int RaceEventID { get; set; }
        public int ParticipantID { get; set; }
        public int CategoryID { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
