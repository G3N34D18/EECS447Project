using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class TeamAssignmentsConfiguration : IEntityTypeConfiguration<TeamAssignment>
{
    public void Configure(EntityTypeBuilder<TeamAssignment> builder)
    {
        builder.ToTable("TeamAssignments");

        // Composite key: a team can only have one locker room per event
        builder.HasKey(ta => new { ta.TeamId, ta.EventId, ta.LockerId });

        // Unique: each team can only be assigned once per event
        builder.HasIndex(ta => new { ta.EventId, ta.TeamId })
            .IsUnique();

        // Unique: each locker room can only be assigned once per event
        builder.HasIndex(ta => new { ta.EventId, ta.LockerId })
            .IsUnique();

        // Relationship: Team → TeamAssignments
        builder.HasOne(ta => ta.Team)
            .WithMany(t => t.Assignments)
            .HasForeignKey(ta => ta.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Event → TeamAssignments
        builder.HasOne(ta => ta.Event)
            .WithMany(e => e.Assignments)
            .HasForeignKey(ta => ta.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: LockerRoom → TeamAssignments
        builder.HasOne(ta => ta.LockerRoom)
            .WithMany(lr => lr.Assignments)
            .HasForeignKey(ta => ta.LockerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
