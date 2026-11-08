using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.CreateEventRequest_
{
    public class UpdateEventDTO
    {
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string OrganiserID { get; set; }
        public int CategoryID { get; set; }
        public string EventType { get; set; } = string.Empty;

    }
}
