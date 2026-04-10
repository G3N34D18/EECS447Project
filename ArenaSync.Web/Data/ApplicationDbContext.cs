using Microsoft.EntityFrameworkCore;
using ArenaSync.Web.Models;

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
        public DbSet<TeamAssignments> TeamAssignments { get; set; }
        public DbSet<VendorAssignment> VendorAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // composite keys for many-to-many relationships
            modelBuilder.Entity<ParticipatesIn>()
                .HasKey(pi => new { pi.TeamId, pi.EventId });
            
            modelBuilder.Entity<RegistersFor>()
                .HasKey(rf => new { rf.AttendeeId, rf.EventId });

            modelBuilder.Entity<SuppliesAt>()
                .HasKey(sa => new { sa.VendorId, sa.EventId });
            
            // assignment of composite key
            modelBuilder.Entity<TeamAssignments>()
                .HasKey(ta => new { ta.TeamId, ta.EventId, ta.LockerId });
            
            modelBuilder.Entity<VendorAssignment>()
                .HasKey(va => new { va.VendorId, va.EventId, va.BoothId });

            modelBuilder.Entity<VendorAssignment>()
                .HasIndex(va => new { va.EventId, va.VendorId })
                .IsUnique();

            modelBuilder.Entity<VendorAssignment>()
                .HasIndex(va => new { va.EventId, va.BoothId })
                .IsUnique();

            // team assignments: each team can only have one locker room 
            // per event, and each locker room can only be assigned to one 
            // team per event
            modelBuilder.Entity<TeamAssignments>()
                .HasIndex(ta => new { ta.EventId, ta.TeamId})
                .IsUnique();

            modelBuilder.Entity<TeamAssignments>()
                .HasIndex(ta => new { ta.EventId, ta.LockerId })
                .IsUnique();

            // vendor booth assignments: each vendor can only have one 
            // booth per event, and each booth can only be assigned 
            // to one vendor per event
            modelBuilder.Entity<VendorAssignment>()
                .HasOne(va => va.Booth)
                .WithMany(b => b.VendorAssignments)
                .HasForeignKey(va => va.BoothId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VendorAssignment>()
                .HasOne(va => va.Event)
                .WithMany(e => e.VendorAssignments)
                .HasForeignKey(va => va.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VendorAssignment>()
                .HasOne(va => va.Vendor)
                .WithMany(v => v.Assignments)
                .HasForeignKey(va => va.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}