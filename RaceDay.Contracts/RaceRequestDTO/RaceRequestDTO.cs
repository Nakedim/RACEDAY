using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.RaceRequestDTO
{
    public class RaceRequestDTO
    {
        public int RaceID { get; set; }
   
        public string RaceName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int EventID { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Distance { get; set; }
    }
}
