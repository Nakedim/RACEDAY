using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Models.RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    public class RacedayDbContext : IdentityDbContext<ApplicationUser>
    {
        public RacedayDbContext(DbContextOptions<RacedayDbContext> options) : base(options)
        {
        }

        // 1. Move your DbSets here (directly inside the class body)
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Organiser> Organiser { get; set; }
        public DbSet<RaceEvent> RaceEvents { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }
        public DbSet<Races> Races { get; set; }
        public DbSet<RaceEntries> RaceEntries { get; set; }
        public DbSet<Categories> Categories { get; set; }

        
    }
}