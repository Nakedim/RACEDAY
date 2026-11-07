using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.RaceRequestDTO
{
    public class RaceDTO

    {
        public string RaceName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Race date and time are required.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
        public int EventID { get; set; }
        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; } = string.Empty;
        public int Distance { get; set; }


    }
}
