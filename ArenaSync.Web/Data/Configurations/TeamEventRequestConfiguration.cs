using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArenaSync.Web.Data.Configurations;

public class TeamEventRequestConfiguration : IEntityTypeConfiguration<TeamEventRequest>
{
    public void Configure(EntityTypeBuilder<TeamEventRequest> builder)
    {
        builder.ToTable("TeamEventRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.ResolutionNote)
            .HasMaxLength(1000);

        builder.HasOne(r => r.Team)
            .WithMany()
            .HasForeignKey(r => r.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.SourceEvent)
            .WithMany()
            .HasForeignKey(r => r.SourceEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.TargetEvent)
            .WithMany()
            .HasForeignKey(r => r.TargetEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
