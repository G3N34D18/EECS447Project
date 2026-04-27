using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

namespace ArenaSync.Web.Data.Configurations;

public class RegistersForConfiguration : IEntityTypeConfiguration<RegistersFor>
{
    public void Configure(EntityTypeBuilder<RegistersFor> builder)
    {
        builder.ToTable("RegistersFor");

        // Composite key: an attendee can only register once per event
        builder.HasKey(rf => new { rf.AttendeeId, rf.EventId });

        // Attendee → RegistersFor (many-to-many join)
        builder.HasOne(rf => rf.Attendee)
            .WithMany(a => a.Registrations)
            .HasForeignKey(rf => rf.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Event → RegistersFor (many-to-many join)
        builder.HasOne(rf => rf.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(rf => rf.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
