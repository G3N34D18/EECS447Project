using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendors");

        // Primary key
        builder.HasKey(v => v.Id);

        // Properties
        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(v => v.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Location)
            .HasMaxLength(200);

        builder.Property(v => v.Phone)
            .HasMaxLength(20);

        // Relationship: Vendor → SuppliesAt (many-to-many join)
        builder.HasMany(v => v.SuppliesAt)
            .WithOne(sa => sa.Vendor)
            .HasForeignKey(sa => sa.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Vendor → VendorAssignments (event-specific booth assignments)
        builder.HasMany(v => v.VendorAssignments)
            .WithOne(va => va.Vendor)
            .HasForeignKey(va => va.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
