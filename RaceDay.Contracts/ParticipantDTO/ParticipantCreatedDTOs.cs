using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.Participant
{


    public class ParticipantCreatedDTOs
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Location { get; set; }


    }
}