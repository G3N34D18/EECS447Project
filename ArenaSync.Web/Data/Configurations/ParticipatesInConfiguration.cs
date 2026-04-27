using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class ParticipatesInConfiguration : IEntityTypeConfiguration<ParticipatesIn>
{
    public void Configure(EntityTypeBuilder<ParticipatesIn> builder)
    {
        builder.ToTable("ParticipatesIn");

        // Composite key: a team can only participate once per event
        builder.HasKey(pi => new { pi.TeamId, pi.EventId });

        // Team → ParticipatesIn (many-to-many join)
        builder.HasOne(pi => pi.Team)
            .WithMany(t => t.ParticipatesIn)
            .HasForeignKey(pi => pi.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Event → ParticipatesIn (many-to-many join)
        builder.HasOne(pi => pi.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(pi => pi.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
