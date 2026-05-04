using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Tests.Services;

public class ReportingServiceTests
{
    [Fact]
    public async Task GetDashboardSummaryAsync_returns_counts_and_pending_assignment_totals()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        await SeedReportScenarioAsync(context);
        var service = CreateService(context);

        var summary = await service.GetDashboardSummaryAsync();

        Assert.Equal(5, summary.TotalEvents);
        Assert.Equal(3, summary.TotalTeams);
        Assert.Equal(2, summary.TotalVendors);
        Assert.Equal(2, summary.TotalAttendees);
        Assert.Equal(1, summary.PendingTeamAssignments);
        Assert.Equal(1, summary.PendingVendorAssignments);
        Assert.Equal(2, summary.PendingAssignments);
        Assert.Equal(1, summary.PendingTeamEventRequests);
        Assert.NotEmpty(summary.RecentEvents);
    }

    [Fact]
    public async Task GetEventRosterAsync_returns_teams_attendees_vendors_and_assignments()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        await SeedAssignedRosterAsync(context);
        var service = CreateService(context);

        var roster = await service.GetEventRosterAsync(eventId: 1);

        Assert.NotNull(roster);
        Assert.Equal("Morning Classic", roster.EventName);
        Assert.Contains(roster.Teams, t => t.TeamName == "Falcons" && t.LockerRoom == "Room 101");
        Assert.Contains(roster.Attendees, a => a.Name == "Alex Guest");
        Assert.Contains(roster.Vendors, v => v.VendorName == "Snack Stand" && v.Booth == "Booth 10");
    }

    [Fact]
    public async Task GetTeamScheduleAsync_returns_participating_events_and_locker_assignments()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        await SeedAssignedRosterAsync(context);
        var service = CreateService(context);

        var schedule = await service.GetTeamScheduleAsync(teamId: 1);

        Assert.NotNull(schedule);
        Assert.Equal("Falcons", schedule.TeamName);
        Assert.Contains(schedule.Events, e => e.EventName == "Morning Classic" && e.LockerRoom == "Room 101");
    }

    [Fact]
    public async Task GetVenueOccupancyAsync_returns_resource_and_event_usage_counts()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        await SeedAssignedRosterAsync(context);
        var service = CreateService(context);

        var occupancy = await service.GetVenueOccupancyAsync(venueId: 1);

        Assert.NotNull(occupancy);
        Assert.Equal(2, occupancy.TotalLockerRooms);
        Assert.Equal(2, occupancy.TotalVendorBooths);
        var eventUsage = Assert.Single(occupancy.Events, e => e.EventId == 1);
        Assert.Equal(1, eventUsage.TeamCount);
        Assert.Equal(1, eventUsage.AssignedLockers);
        Assert.Equal(1, eventUsage.VendorCount);
        Assert.Equal(1, eventUsage.AssignedBooths);
        Assert.Equal(1, eventUsage.AttendeeCount);
    }

    [Fact]
    public async Task GetConflictSummaryAsync_reports_missing_locker_and_booth_conflicts()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var conflicts = await service.GetConflictSummaryAsync(eventId: 1);

        Assert.Contains(conflicts, c => c.Type == "Missing Locker" && c.AffectedEntity == "Falcons");
        Assert.Contains(conflicts, c => c.Type == "Missing Booth" && c.AffectedEntity == "Snack Stand");
    }

    [Fact]
    public async Task ExportReportToCsvAsync_exports_escaped_event_roster_csv()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        await SeedAssignedRosterAsync(context);
        context.Attendees.Add(new Attendee
        {
            Id = 3,
            Name = "Comma, Guest",
            Email = "comma@example.com",
            Phone = "555-0303"
        });
        context.RegistersFor.Add(new RegistersFor { AttendeeId = 3, EventId = 1 });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var csv = await service.ExportReportToCsvAsync(ReportType.EventRoster, entityId: 1);

        Assert.StartsWith("Section,Name,Detail,Assignment", csv);
        Assert.Contains("\"Comma, Guest\"", csv);
        Assert.Contains("Snack Stand", csv);
    }

    private static ReportingService CreateService(ArenaSync.Web.Data.ApplicationDbContext context)
        => new(context, new AssignmentService(context));

    private static async Task SeedReportScenarioAsync(ArenaSync.Web.Data.ApplicationDbContext context)
    {
        context.Attendees.AddRange(
            new Attendee { Id = 1, Name = "Alex Guest", Email = "alex@example.com", Phone = "555-0301" },
            new Attendee { Id = 2, Name = "Blair Guest", Email = "blair@example.com", Phone = "555-0302" });
        context.RegistersFor.Add(new RegistersFor { AttendeeId = 1, EventId = 1 });
        context.ParticipatesIn.AddRange(
            new ParticipatesIn { TeamId = 1, EventId = 1 },
            new ParticipatesIn { TeamId = 2, EventId = 1 });
        context.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        context.TeamEventRequests.Add(new TeamEventRequest
        {
            TeamId = 1,
            Type = TeamEventRequestType.Reassignment,
            SourceEventId = 1,
            TargetEventId = 2,
            Reason = "Need a later slot."
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedAssignedRosterAsync(ArenaSync.Web.Data.ApplicationDbContext context)
    {
        context.Attendees.Add(new Attendee { Id = 1, Name = "Alex Guest", Email = "alex@example.com", Phone = "555-0301" });
        context.RegistersFor.Add(new RegistersFor { AttendeeId = 1, EventId = 1 });
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
        context.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
        context.VendorAssignments.Add(new VendorAssignment { VendorId = 1, EventId = 1, BoothId = 1 });
        await context.SaveChangesAsync();
    }
}
