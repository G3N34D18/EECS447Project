using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Tests.Services;

public class FacilityServiceTests
{
    [Fact]
    public async Task CreateLockerRoomAsync_rejects_duplicate_room_number_in_same_venue()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new FacilityService(context);

        var created = await service.CreateLockerRoomAsync(new LockerRoom
        {
            RoomNumber = 101,
            VenueId = 1
        });

        Assert.False(created);
        Assert.Equal(1, await context.LockerRooms.CountAsync(l => l.VenueId == 1 && l.RoomNumber == 101));
    }

    [Fact]
    public async Task DeleteLockerRoomAsync_rejects_locker_with_assignments()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
        await context.SaveChangesAsync();
        var service = new FacilityService(context);

        var deleted = await service.DeleteLockerRoomAsync(id: 1);

        Assert.False(deleted);
        Assert.True(await context.LockerRooms.AnyAsync(l => l.Id == 1));
    }

    [Fact]
    public async Task CreateVendorBoothAsync_rejects_duplicate_booth_number_in_same_venue()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new FacilityService(context);

        var created = await service.CreateVendorBoothAsync(new VendorBooth
        {
            BoothNumber = 10,
            VenueId = 1
        });

        Assert.False(created);
        Assert.Equal(1, await context.VendorBooths.CountAsync(b => b.VenueId == 1 && b.BoothNumber == 10));
    }

    [Fact]
    public async Task DeleteVendorBoothAsync_rejects_booth_with_assignments()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        context.VendorAssignments.Add(new VendorAssignment { VendorId = 1, EventId = 1, BoothId = 1 });
        await context.SaveChangesAsync();
        var service = new FacilityService(context);

        var deleted = await service.DeleteVendorBoothAsync(id: 1);

        Assert.False(deleted);
        Assert.True(await context.VendorBooths.AnyAsync(b => b.Id == 1));
    }
}
