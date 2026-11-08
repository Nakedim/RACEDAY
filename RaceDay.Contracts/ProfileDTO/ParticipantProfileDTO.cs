using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceDay.Contracts;

namespace RaceDay.Contracts.ProfileDTO
{
    public class ParticipantProfileDTO
    {

        public int Id { get; set; }

        // Using = string.Empty; fixes the non-nullable underline warning
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Using string? allows these fields to safely be null without warnings
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    }
}

