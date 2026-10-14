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


        //The idea is to have id always for every category since the list is rigid

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Categories>().HasData(
        //        new Categories { CategoryID = 1, CategoryName = "Marathon" },
        //        new Categories { CategoryID = 2, CategoryName = "Walking" },
        //        new Categories { CategoryID = 3, CategoryName = "Track and Field" }
        //    );
        //}
    }
}