using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.ResultsdTO
{
    public class ResultsRequestResponseDTO
    {
        
        public int RaceID { get; set; }
        public string ParticipantID { get; set; } = string.Empty;
        public DateTime FinishTime { get; set; } 
        public int Position { get; set; }
    }
}
