using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class VendorAssignmentConfiguration : IEntityTypeConfiguration<VendorAssignment>
{
    public void Configure(EntityTypeBuilder<VendorAssignment> builder)
    {
        builder.ToTable("VendorAssignments");

        // Composite key: vendor + event + booth must be unique
        builder.HasKey(va => new { va.VendorId, va.EventId, va.BoothId });

        // Unique: a vendor can only have ONE booth per event
        builder.HasIndex(va => new { va.EventId, va.VendorId })
            .IsUnique();

        // Unique: a booth can only be assigned to ONE vendor per event
        builder.HasIndex(va => new { va.EventId, va.BoothId })
            .IsUnique();

        // Relationship: Vendor → VendorAssignments
        builder.HasOne(va => va.Vendor)
            .WithMany(v => v.VendorAssignments)
            .HasForeignKey(va => va.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Event → VendorAssignments
        builder.HasOne(va => va.Event)
            .WithMany(e => e.VendorAssignments)
            .HasForeignKey(va => va.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: VendorBooth → VendorAssignments
        builder.HasOne(va => va.Booth)
            .WithMany(b => b.VendorAssignments)
            .HasForeignKey(va => va.BoothId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
