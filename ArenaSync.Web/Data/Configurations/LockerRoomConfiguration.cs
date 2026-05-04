using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class LockerRoomConfiguration : IEntityTypeConfiguration<LockerRoom>
{
    public void Configure(EntityTypeBuilder<LockerRoom> builder)
    {
        builder.ToTable("LockerRooms");

        // Primary key
        builder.HasKey(lr => lr.Id);

        // Properties
        builder.Property(lr => lr.RoomNumber)
            .IsRequired();

        // Relationship: LockerRoom belongs to a Venue
        builder.HasOne(lr => lr.Venue)
            .WithMany(v => v.LockerRooms)
            .HasForeignKey(lr => lr.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: LockerRoom has many TeamAssignments
        builder.HasMany(lr => lr.Assignments)
            .WithOne(ta => ta.LockerRoom)
            .HasForeignKey(ta => ta.LockerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
