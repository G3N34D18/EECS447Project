using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArenaSync.Web.Models;

public class SuppliesAtConfiguration : IEntityTypeConfiguration<SuppliesAt>
{
    public void Configure(EntityTypeBuilder<SuppliesAt> builder)
    {
        builder.ToTable("SuppliesAt");

        // Composite key: a vendor can only supply an event once
        builder.HasKey(sa => new { sa.VendorId, sa.EventId });

        // Vendor → SuppliesAt (many-to-many join)
        builder.HasOne(sa => sa.Vendor)
            .WithMany(v => v.SuppliesAt)
            .HasForeignKey(sa => sa.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Event → SuppliesAt (many-to-many join)
        builder.HasOne(sa => sa.Event)
            .WithMany(e => e.Suppliers)
            .HasForeignKey(sa => sa.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
