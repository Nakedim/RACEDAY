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

        // DbSets representing your database tables
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<RaceEvent> RaceEvents { get; set; }
        public DbSet<RaceEntries> RaceEntries { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }
        public DbSet<Races> Races { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Crucial: Base call ensures Identity table mappings are built properly first
            base.OnModelCreating(modelBuilder);

            // 1. Explicitly isolate Table names to eliminate shared-table collision bugs
            modelBuilder.Entity<Organiser>().ToTable("Organisers");
            modelBuilder.Entity<Participant>().ToTable("Participants");
            modelBuilder.Entity<RaceEvent>().ToTable("RaceEvents");
            modelBuilder.Entity<RaceEntries>().ToTable("RaceEntries");
            modelBuilder.Entity<RaceResults>().ToTable("RaceResults");
            modelBuilder.Entity<Races>().ToTable("Races");
            modelBuilder.Entity<Categories>().ToTable("Categories");

            // 2. Setup Organiser -> RaceEvents relationship
            modelBuilder.Entity<RaceEvent>()
                .HasOne(re => re.Organiser)
                .WithMany(o => o.RaceEvents)
                .HasForeignKey(re => re.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Setup Participant -> Entries relationship
            modelBuilder.Entity<RaceEntries>()
                .HasOne(e => e.Participant)
                .WithMany()
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Setup RaceEvent -> Entries relationship
            //modelBuilder.Entity<RaceEntries>()
            //    .HasOne(e => e.RaceEvent)
            //    .WithMany(re => re.Entries)
            //    .HasForeignKey(e => e.RaceEventID)
            //    .OnDelete(DeleteBehavior.Cascade);

            // 5. Setup Participant -> RaceResults relationship
            modelBuilder.Entity<RaceResults>()
                .HasOne(r => r.Participant)
                .WithMany()
                .HasForeignKey(r => r.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Setup Race -> RaceResults relationship
            modelBuilder.Entity<RaceResults>()
                .HasOne(r => r.Race)
                .WithMany()
                .HasForeignKey(r => r.RaceID)
                .OnDelete(DeleteBehavior.Cascade);

            // 7. Setup Category -> Races relationship
            //modelBuilder.Entity<Races>()
            //    .HasOne(r => r.Category)
            //    .WithMany()
            //    .HasForeignKey(r => r.CategoryID)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
