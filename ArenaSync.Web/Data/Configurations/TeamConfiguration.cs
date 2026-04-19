using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        // Table name (optional)
        builder.ToTable("Teams");

        // Keys
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Manager)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Phone)
            .IsRequired()
            .HasMaxLength(20);

        // Relationships
        builder.HasMany(t => t.ParticipatesIn)
            .WithOne(pi => pi.Team)
            .HasForeignKey(pi => pi.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Assignments)
            .WithOne(ta => ta.Team)
            .HasForeignKey(ta => ta.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
