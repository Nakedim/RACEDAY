using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Mvc.Models
{
    public class EnterResultsModel
    {
        public int ResultID { get; set; }
        public int RaceID { get; set; }
            public int ParticipantID { get; set; }
            [Required]
            public DateTime FinishTime { get; set; } = DateTime.UtcNow;
            [Required]
            public int Position { get; set; }
       
    }

    }



