
using System.ComponentModel.DataAnnotations;


namespace RaceDay.Contracts.ResultsDTO
{
    public class ResultsRequestDTO
    {

        [Required]
        public int RaceID { get; set; } 

        [Required] // Added Required attribute to ensure a valid identifier is passed for database checks
        public int ParticipantID { get; set; } 

        [Required]
        public DateTime FinishTime { get; set; }

        [Required]
        public int Position { get; set; } 
    }
}
