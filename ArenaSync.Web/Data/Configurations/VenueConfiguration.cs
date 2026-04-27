using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArenaSync.Web.Data.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues");

        builder.HasKey(v => v.Id);

        builder.Property(V => V.Name).IsRequired().HasMaxLength(200);

        builder.Property(V => V.Address).IsRequired().HasMaxLength(500);

        builder.Property(V => V.Capacity).IsRequired();

        builder.HasMany(v => v.Events).WithOne(e => e.Venue).HasForeignKey(e => e.VenueId).OnDelete(DeleteBehavior.Restrict);
    }
}