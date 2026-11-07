using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.EntryRequest
{
    public class RaceEntryRequestDTO
    {
        public int RaceEntryID { get; set; }
        public int RaceID { get; set; }
        public int ParticipantID { get; set; }
        public int CategoryID { get; set; }
        public DateTime EntryDate { get; set; }
    }
}

