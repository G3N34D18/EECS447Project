using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

namespace ArenaSync.Web.Data.Configurations;

public class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
{
    public void Configure(EntityTypeBuilder<Attendee> builder)
    {
        builder.ToTable("Attendees");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasMany(a => a.Registrations)
            .WithOne(rf => rf.Attendee)
            .HasForeignKey(rf => rf.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
