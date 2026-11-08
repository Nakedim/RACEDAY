
using RaceDay.Api.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{



    [Table("Participant")]
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }
     
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Location { get; set; } = string.Empty;

        public int ApplicationUserId { get; set; }

        // 3. Explicitly links this participant record to the core ApplicationUser account entity
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser User { get; set; } = null!;
    }
    


    }
    

