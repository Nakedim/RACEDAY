using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RACEDAY.Models
{



    [Table("AuthUsers")]
    public class AuthUsers
    {
        [Key]
        public int UserID { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;


    }

    [Table("Participant")]
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Location { get; set; } = string.Empty;
        public int? UserID { get; set; }

    }
    [Table("Organiser")]
    public class Organiser
    {
        [Key]
        public int OrganiserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;



    }
    [Table("Event")]
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public int OrganiserID { get; set; }
        public int CategoryID { get; set; }
    }
    [Table("RaceEntries")]
    public class RaceEntries
    {
        [Key]
        public int EntryID { get; set; }
        public int ParticipantID { get; set; }
        public int RaceID { get; set; }
        public int EventID { get; set; }
        public DateTime EntryDate { get; set; }
    }
    [Table("Races")]
    public class Races
    {
        [Key]
        public int RaceID { get; set; }
        public string RaceName { get; set; } = string.Empty;  
        public DateTime StartTime { get; set; }
        public int EventID { get; set; }
        public string Location { get; set; } = string.Empty;

    }

    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;


    }
    [Table("Result")]
    public class Result
    {
        [Key]
        public int ResultID { get; set; }
        public int ParticipantID { get; set; }

        public int Position { get; set; }

        public TimeOnly? finishTime { get; set; }
        public int RaceID { get; set; }
        


    }
}