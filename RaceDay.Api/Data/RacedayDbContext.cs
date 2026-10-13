using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
   
  
    
    public class RacedayDbContext: IdentityDbContext<ApplicationUser>
    
    {

public RacedayDbContext(
    DbContextOptions<RacedayDbContext>options
    ) : base(options) { }           
       
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<RaceEvent> RaceEvents { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }
        public DbSet<Races> Races { get; set; }
        public DbSet<RaceEntries> RaceEntries { get; set; }
        public DbSet<Categories> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Ignore<Microsoft.AspNetCore.Mvc.Filters.FilterDescriptor>();
            builder.Ignore<Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource>();
        }
    }
}
