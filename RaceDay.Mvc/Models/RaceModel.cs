using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Mvc.Models
{
    public class RaceModel
    {
        public string RaceName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        //public Categories? Category { get; set; }

        public DateTime StartTime { get; set; }

        public int EventID { get; set; }

      
        //public RaceEvent Event { get; set; } = null!;

        public string Location { get; set; } = string.Empty;

        public int Distance { get; set; }


        public string? OrganiserID { get; set; }
    }
}
