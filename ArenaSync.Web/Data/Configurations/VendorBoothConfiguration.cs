using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class VendorBoothConfiguration : IEntityTypeConfiguration<VendorBooth>
{
    public void Configure(EntityTypeBuilder<VendorBooth> builder)
    {
        builder.ToTable("VendorBooths");

        // Primary key
        builder.HasKey(b => b.Id);

        // Properties
        builder.Property(b => b.BoothNumber)
            .IsRequired();

        // Relationship: VendorBooth belongs to a Venue
        builder.HasOne(b => b.Venue)
            .WithMany(v => v.VendorBooths)
            .HasForeignKey(b => b.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: VendorBooth has many VendorAssignments
        builder.HasMany(b => b.VendorAssignments)
            .WithOne(va => va.Booth)
            .HasForeignKey(va => va.BoothId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
