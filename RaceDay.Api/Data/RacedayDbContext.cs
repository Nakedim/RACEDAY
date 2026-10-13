using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    public class RacedayDbContext : IdentityDbContext<ApplicationUser>
    {
        public RacedayDbContext(DbContextOptions<RacedayDbContext> options) : base(options)
        {
        }

        // 1. Move your DbSets here (directly inside the class body)
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<RaceEvent> RaceEvents { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }
        public DbSet<Races> Races { get; set; }
        public DbSet<RaceEntries> RaceEntries { get; set; }
        public DbSet<Categories> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Organiser -> RaceEvents Relationship (One-to-Many)
            modelBuilder.Entity<RaceEvent>()
                .HasOne(static e => e.Organiser)
                .WithMany(static o => o.RaceEvents)
                .HasForeignKey(static e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. RaceEvents -> Races Relationship (One-to-Many)
            modelBuilder.Entity<Races>()
                .HasOne(static r => r.Event)
                .WithMany(static e => e.Races)
                .HasForeignKey(static r => r.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. RaceEntries Junction Configuration


            modelBuilder.Entity<RaceEntries>()
                .HasOne(static re => re.Race)
                .WithMany(static r => r.RaceEntries)
                .HasForeignKey(static re => re.RaceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RaceEntries>()
                .HasOne(static re => re.Participant)
                .WithMany() // Leaves navigation optional on Participant class
                .HasForeignKey(static re => re.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<RaceEntries>()
            //    .HasOne(static re => re.Categories)
            //    .WithMany(static c => c.RaceEntries)
            //    .HasForeignKey(static re => re.CategoryID)
            //    .OnDelete(DeleteBehavior.Restrict);

            // 4. Results Configuration
            modelBuilder.Entity<RaceResults>()
                .HasOne(static res => res.race)
                .WithMany()
                .HasForeignKey(static res => res.RaceID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}