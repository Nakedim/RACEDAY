using Microsoft.EntityFrameworkCore;
using RACEDAY.Models;

namespace RACEDAY.Data
{

    public class RacedayDbContext : DbContext
    {
        public RacedayDbContext(DbContextOptions<RacedayDbContext> options) : base(options)
        {
        }

        public DbSet<Participant> Participants { get; set; }
        // Add other sets here (e.g., DbSet<AuthUser>, DbSet<Event>)
    }



}
