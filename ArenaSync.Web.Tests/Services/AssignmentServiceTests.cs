using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Tests.Services;

public class AssignmentServiceTests
{
    [Fact]
    public async Task AssignTeamToLockerAsync_assigns_registered_team_to_available_locker()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new AssignmentService(context);

        var result = await service.AssignTeamToLockerAsync(teamId: 1, eventId: 1, lockerId: 1);

        Assert.True(result.Succeeded);
        Assert.True(await context.TeamAssignments.AnyAsync(a => a.TeamId == 1 && a.EventId == 1 && a.LockerId == 1));
    }

    [Fact]
    public async Task AssignTeamToLockerAsync_rejects_team_not_registered_for_event()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new AssignmentService(context);

        var result = await service.AssignTeamToLockerAsync(teamId: 1, eventId: 1, lockerId: 1);

        Assert.False(result.Succeeded);
        Assert.Contains("before assigning a locker", result.Message);
        Assert.Empty(context.TeamAssignments);
    }

    [Fact]
    public async Task AssignTeamToLockerAsync_rejects_locker_from_different_venue()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new AssignmentService(context);

        var result = await service.AssignTeamToLockerAsync(teamId: 1, eventId: 1, lockerId: 3);

        Assert.False(result.Succeeded);
        Assert.Contains("different venue", result.Message);
        Assert.Empty(context.TeamAssignments);
    }

    [Fact]
    public async Task AssignTeamToLockerAsync_rejects_locker_already_used_for_event()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.AddRange(
            new ParticipatesIn { TeamId = 1, EventId = 1 },
            new ParticipatesIn { TeamId = 2, EventId = 1 });
        context.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
        await context.SaveChangesAsync();
        var service = new AssignmentService(context);

        var result = await service.AssignTeamToLockerAsync(teamId: 2, eventId: 1, lockerId: 1);

        Assert.False(result.Succeeded);
        Assert.Contains("already assigned", result.Message);
    }

    [Fact]
    public async Task AssignVendorToBoothAsync_assigns_vendor_to_booth_and_attaches_vendor_to_event()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new AssignmentService(context);

        var result = await service.AssignVendorToBoothAsync(vendorId: 1, eventId: 1, boothId: 1);

        Assert.True(result.Succeeded);
        Assert.True(await context.VendorAssignments.AnyAsync(a => a.VendorId == 1 && a.EventId == 1 && a.BoothId == 1));
        Assert.True(await context.SuppliesAt.AnyAsync(s => s.VendorId == 1 && s.EventId == 1));
    }

    [Fact]
    public async Task AssignVendorToBoothAsync_rejects_booth_from_different_venue()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new AssignmentService(context);

        var result = await service.AssignVendorToBoothAsync(vendorId: 1, eventId: 1, boothId: 3);

        Assert.False(result.Succeeded);
        Assert.Contains("different venue", result.Message);
        Assert.Empty(context.VendorAssignments);
    }

    [Fact]
    public async Task GetAssignmentConflictsAsync_reports_missing_locker_and_booth()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new AssignmentService(context);

        var conflicts = await service.GetAssignmentConflictsAsync(eventId: 1);

        Assert.Contains(conflicts, c => c.Type == "Missing Locker" && c.AffectedEntity == "Falcons");
        Assert.Contains(conflicts, c => c.Type == "Missing Booth" && c.AffectedEntity == "Snack Stand");
    }
}
