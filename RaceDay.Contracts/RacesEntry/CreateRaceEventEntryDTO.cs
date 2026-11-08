using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.RacesEntry
{
    public class CreateRaceEventEntryDTO
    {

        public int RaceEventID { get; set; }
        public int ParticipantID { get; set; }
        public int OrganiserID { get; set; }
        public int CategoryID { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    }
}
