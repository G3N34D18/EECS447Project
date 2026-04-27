using Microsoft.EntityFrameworkCore;
using ArenaSync.Web.Models;
using ArenaSync.Web.Data.Configurations;

namespace ArenaSync.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // core entities
        public DbSet<Team> Teams { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Vendor> Vendors { get; set; }  

        // physical structures
        public DbSet<LockerRoom> LockerRooms { get; set; }
        public DbSet<VendorBooth> VendorBooths { get; set; }
        
        // many-to-many relationships
        public DbSet<ParticipatesIn> ParticipatesIn { get; set; }
        public DbSet<RegistersFor> RegistersFor { get; set; }
        public DbSet<SuppliesAt> SuppliesAt { get; set; }

        // event specific assignments
        public DbSet<TeamAssignment> TeamAssignments { get; set; }
        public DbSet<VendorAssignment> VendorAssignments { get; set; }
        public DbSet<TeamReassignmentRequest> TeamReassignmentRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }
    }
}