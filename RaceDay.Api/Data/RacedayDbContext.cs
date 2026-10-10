using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
namespace RaceDay.Api.Data
{
    public class RacedayDbContext : DbContext
    {
     
        public RacedayDbContext(DbContextOptions<RacedayDbContext> options) : base(options)
        {
        }

  
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Organisers> Organisers { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<RaceEntries> RaceEntries { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }
        public DbSet<Races> Races { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
