using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Tests.Services;

public class VendorServiceTests
{
    [Fact]
    public async Task CreateVendorAsync_rejects_duplicate_name()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new VendorService(context);

        var created = await service.CreateVendorAsync(new Vendor
        {
            Name = "Snack Stand",
            Type = "Food",
            Location = "South Concourse",
            Phone = "555-9999"
        });

        Assert.False(created);
        Assert.Equal(1, await context.Vendors.CountAsync(v => v.Name == "Snack Stand"));
    }

    [Fact]
    public async Task AddVendorToEventAsync_adds_supplies_at_record()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new VendorService(context);

        var added = await service.AddVendorToEventAsync(vendorId: 1, eventId: 1);

        Assert.True(added);
        Assert.True(await context.SuppliesAt.AnyAsync(s => s.VendorId == 1 && s.EventId == 1));
    }

    [Fact]
    public async Task DeleteVendorAsync_rejects_vendor_with_assignments()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new VendorService(context);

        var deleted = await service.DeleteVendorAsync(id: 1);

        Assert.False(deleted);
        Assert.True(await context.Vendors.AnyAsync(v => v.Id == 1));
    }

    [Fact]
    public async Task RemoveVendorFromEventAsync_removes_booth_assignment_too()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        context.VendorAssignments.Add(new VendorAssignment { VendorId = 1, EventId = 1, BoothId = 1 });
        await context.SaveChangesAsync();
        var service = new VendorService(context);

        var removed = await service.RemoveVendorFromEventAsync(vendorId: 1, eventId: 1);

        Assert.True(removed);
        Assert.False(await context.SuppliesAt.AnyAsync(s => s.VendorId == 1 && s.EventId == 1));
        Assert.False(await context.VendorAssignments.AnyAsync(a => a.VendorId == 1 && a.EventId == 1));
    }
}
