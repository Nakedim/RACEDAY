using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Mvc.Models
{
    public class RaceResultModel
    {
 

        public int RaceID { get; set; }
        public int ParticipantID { get; set; }
        public DateTime FinishTime { get; set; } = DateTime.UtcNow;

        public int Position { get; set; }

  
       
    }
}
