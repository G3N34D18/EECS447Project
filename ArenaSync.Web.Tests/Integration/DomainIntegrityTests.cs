using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ArenaSync.Web.Tests.Integration;

public class DomainIntegrityTests
{
    [Fact]
    public async Task DeleteVenueAsync_rejects_venue_with_events_without_throwing()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        var service = new VenueService(ctx);

        var deleted = await service.DeleteVenueAsync(1);

        Assert.False(deleted);
        Assert.NotNull(await ctx.Venues.FindAsync(1));
        Assert.True(await ctx.Events.AnyAsync(e => e.VenueId == 1));
    }

    [Fact]
    public async Task DeleteVenueAsync_rejects_venue_with_facilities_even_without_events()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.Venues.Add(new Venue
        {
            Id = 10,
            Name = "Training Annex",
            Address = "10 Practice Lane",
            Capacity = 500
        });
        ctx.LockerRooms.Add(new LockerRoom { Id = 10, RoomNumber = 301, VenueId = 10 });
        ctx.VendorBooths.Add(new VendorBooth { Id = 10, BoothNumber = 301, VenueId = 10 });
        await ctx.SaveChangesAsync();
        var service = new VenueService(ctx);

        var deleted = await service.DeleteVenueAsync(10);

        Assert.False(deleted);
        Assert.NotNull(await ctx.Venues.FindAsync(10));
        Assert.True(await ctx.LockerRooms.AnyAsync(l => l.VenueId == 10));
        Assert.True(await ctx.VendorBooths.AnyAsync(b => b.VenueId == 10));
    }

    [Fact]
    public async Task DeleteVenueAsync_deletes_empty_venue()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.Venues.Add(new Venue
        {
            Id = 10,
            Name = "Empty Pavilion",
            Address = "99 Quiet Road",
            Capacity = 250
        });
        await ctx.SaveChangesAsync();
        var service = new VenueService(ctx);

        var deleted = await service.DeleteVenueAsync(10);

        Assert.True(deleted);
        Assert.Null(await ctx.Venues.FindAsync(10));
    }

    [Fact]
    public async Task DeleteTeamAsync_rejects_team_with_participation_assignment_or_requests()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        ctx.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
        ctx.TeamEventRequests.Add(new TeamEventRequest
        {
            TeamId = 1,
            Type = TeamEventRequestType.Reassignment,
            SourceEventId = 1,
            TargetEventId = 2,
            Reason = "Schedule issue",
            Status = RequestStatus.Pending
        });
        ctx.TeamReassignmentRequests.Add(new TeamReassignmentRequest
        {
            TeamId = 1,
            RequestedEventId = 2,
            Reason = "Legacy request",
            Status = RequestStatus.Pending
        });
        await ctx.SaveChangesAsync();
        var service = new TeamService(ctx);

        var deleted = await service.DeleteTeamAsync(1);

        Assert.False(deleted);
        Assert.NotNull(await ctx.Teams.FindAsync(1));
        Assert.True(await ctx.ParticipatesIn.AnyAsync(p => p.TeamId == 1));
        Assert.True(await ctx.TeamEventRequests.AnyAsync(r => r.TeamId == 1));
    }

    [Fact]
    public async Task DeleteTeamAsync_deletes_unused_team()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.Teams.Add(new Team
        {
            Id = 10,
            Name = "Unused Team",
            Manager = "Sam",
            Email = "unused@example.com",
            Phone = "555-9910",
            PlayerCount = 8
        });
        await ctx.SaveChangesAsync();
        var service = new TeamService(ctx);

        var deleted = await service.DeleteTeamAsync(10);

        Assert.True(deleted);
        Assert.Null(await ctx.Teams.FindAsync(10));
    }

    [Fact]
    public async Task DeleteAttendeeAsync_rejects_attendee_with_event_registration()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.Attendees.Add(new Attendee
        {
            Id = 10,
            Name = "Registered Guest",
            Email = "registered@example.com",
            Phone = "555-1010"
        });
        ctx.RegistersFor.Add(new RegistersFor { AttendeeId = 10, EventId = 1 });
        await ctx.SaveChangesAsync();
        var service = new AttendeeService(ctx);

        var deleted = await service.DeleteAttendeeAsync(10);

        Assert.False(deleted);
        Assert.NotNull(await ctx.Attendees.FindAsync(10));
        Assert.True(await ctx.RegistersFor.AnyAsync(r => r.AttendeeId == 10));
    }

    [Fact]
    public async Task DeleteAttendeeAsync_deletes_unregistered_attendee()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.Attendees.Add(new Attendee
        {
            Id = 10,
            Name = "Walkup Guest",
            Email = "walkup@example.com",
            Phone = "555-2020"
        });
        await ctx.SaveChangesAsync();
        var service = new AttendeeService(ctx);

        var deleted = await service.DeleteAttendeeAsync(10);

        Assert.True(deleted);
        Assert.Null(await ctx.Attendees.FindAsync(10));
    }

    [Fact]
    public async Task DeleteVendorAsync_rejects_vendor_with_event_or_booth_dependencies()
    {
        using var db = new SqliteTestDatabase();
        await using var ctx = db.CreateContext();
        await TestData.SeedCoreAsync(ctx);
        ctx.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        ctx.VendorAssignments.Add(new VendorAssignment { VendorId = 1, EventId = 1, BoothId = 1 });
        await ctx.SaveChangesAsync();
        var service = new VendorService(ctx);

        var deleted = await service.DeleteVendorAsync(1);

        Assert.False(deleted);
        Assert.NotNull(await ctx.Vendors.FindAsync(1));
        Assert.True(await ctx.SuppliesAt.AnyAsync(s => s.VendorId == 1));
        Assert.True(await ctx.VendorAssignments.AnyAsync(a => a.VendorId == 1));
    }
}
