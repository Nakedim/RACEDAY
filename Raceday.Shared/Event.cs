using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raceday.Shared
{
    public class Event
    {
        public int EventId { get; set; } // Primary Key
        public string Title { get; set; }

        // FOREIGN KEY
        public int CategoryId { get; set; }

        // Optional: Navigation property for display purposes
        //public Category? Category { get; set; };
    }
}
