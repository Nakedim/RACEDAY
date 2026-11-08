using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.EntryRequest
{
    public class CreateRaceEntryDTO
    {
        public int RaceEntryID { get; set; }

        public int RaceEventID { get; set; }

        public string ParticipantID { get; set; } = string.Empty;

        public int RaceID { get; set; }

        public int CategoryID { get; set; }

        public DateTime RegistrationDate { get; set; }=DateTime.UtcNow;

    }
}
