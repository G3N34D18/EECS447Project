// -----------------------------------------------------------------------------
// File: ValidationServiceTests.cs
// Project: ArenaSync.Web.Tests
// Purpose: Batch 4 — Unit tests for ValidationService business-rule logic.
//          Uses an in-memory SQLite database so no SQL Server is needed.
// -----------------------------------------------------------------------------

using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ArenaSync.Web.Tests.Services;

public class ValidationServiceTests : IDisposable
{
    private readonly SqliteTestDatabase _db;

    public ValidationServiceTests()
    {
        _db = new SqliteTestDatabase();
        using var ctx = _db.CreateContext();
        TestData.SeedCoreAsync(ctx).GetAwaiter().GetResult();
    }

    public void Dispose() => _db.Dispose();

    private ValidationService CreateService()
    {
        var context = _db.CreateContext();
        return new ValidationService(context, NullLogger<ValidationService>.Instance);
    }

    // ── ValidateTeamAssignmentAsync ───────────────────────────────────────────

    [Fact]
    public async Task ValidateTeamAssignment_HappyPath_ReturnsNoErrors()
    {
        var svc = CreateService();
        var errors = await svc.ValidateTeamAssignmentAsync(teamId: 1, eventId: 1, lockerId: 1);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ValidateTeamAssignment_TeamDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateTeamAssignmentAsync(teamId: 999, eventId: 1, lockerId: 1);
        Assert.Contains(errors, e => e.Contains("Team with ID 999"));
    }

    [Fact]
    public async Task ValidateTeamAssignment_EventDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateTeamAssignmentAsync(teamId: 1, eventId: 999, lockerId: 1);
        Assert.Contains(errors, e => e.Contains("Event with ID 999"));
    }

    [Fact]
    public async Task ValidateTeamAssignment_LockerDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateTeamAssignmentAsync(teamId: 1, eventId: 1, lockerId: 999);
        Assert.Contains(errors, e => e.Contains("Locker room with ID 999"));
    }

    [Fact]
    public async Task ValidateTeamAssignment_DuplicateAssignment_ReturnsError()
    {
        // Seed an existing assignment
        using var ctx = _db.CreateContext();
        ctx.TeamAssignments.Add(new ArenaSync.Web.Models.TeamAssignment { EventId = 1, TeamId = 1, LockerId = 1 });
        await ctx.SaveChangesAsync();

        var svc = CreateService();
        var errors = await svc.ValidateTeamAssignmentAsync(teamId: 1, eventId: 1, lockerId: 2);
        Assert.Contains(errors, e => e.Contains("already assigned"));
    }

    [Fact]
    public async Task ValidateTeamAssignment_LockerAlreadyTaken_ReturnsError()
    {
        // Seed another team using locker 1 at event 1
        using var ctx = _db.CreateContext();
        ctx.TeamAssignments.Add(new ArenaSync.Web.Models.TeamAssignment { EventId = 1, TeamId = 2, LockerId = 1 });
        await ctx.SaveChangesAsync();

        var svc = CreateService();
        // Team 1 tries to use locker 1 which is taken
        var errors = await svc.ValidateTeamAssignmentAsync(teamId: 1, eventId: 1, lockerId: 1);
        Assert.Contains(errors, e => e.Contains("locker room is already assigned"));
    }

    // ── ValidateVendorBoothAssignmentAsync ────────────────────────────────────

    [Fact]
    public async Task ValidateVendorBooth_HappyPath_ReturnsNoErrors()
    {
        var svc = CreateService();
        var errors = await svc.ValidateVendorBoothAssignmentAsync(vendorId: 1, boothId: 1, eventId: 1);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ValidateVendorBooth_VendorDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateVendorBoothAssignmentAsync(vendorId: 999, boothId: 1, eventId: 1);
        Assert.Contains(errors, e => e.Contains("Vendor with ID 999"));
    }

    [Fact]
    public async Task ValidateVendorBooth_BoothDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateVendorBoothAssignmentAsync(vendorId: 1, boothId: 999, eventId: 1);
        Assert.Contains(errors, e => e.Contains("Vendor booth with ID 999"));
    }

    [Fact]
    public async Task ValidateVendorBooth_EventDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateVendorBoothAssignmentAsync(vendorId: 1, boothId: 1, eventId: 999);
        Assert.Contains(errors, e => e.Contains("Event with ID 999"));
    }

    [Fact]
    public async Task ValidateVendorBooth_VendorAlreadyAtEvent_ReturnsError()
    {
        using var ctx = _db.CreateContext();
        ctx.VendorAssignments.Add(new ArenaSync.Web.Models.VendorAssignment { VendorId = 1, BoothId = 1, EventId = 1 });
        await ctx.SaveChangesAsync();

        var svc = CreateService();
        var errors = await svc.ValidateVendorBoothAssignmentAsync(vendorId: 1, boothId: 2, eventId: 1);
        Assert.Contains(errors, e => e.Contains("already assigned to the selected event"));
    }

    [Fact]
    public async Task ValidateVendorBooth_BoothAlreadyTaken_ReturnsError()
    {
        using var ctx = _db.CreateContext();
        ctx.VendorAssignments.Add(new ArenaSync.Web.Models.VendorAssignment { VendorId = 2, BoothId = 1, EventId = 1 });
        await ctx.SaveChangesAsync();

        var svc = CreateService();
        var errors = await svc.ValidateVendorBoothAssignmentAsync(vendorId: 1, boothId: 1, eventId: 1);
        Assert.Contains(errors, e => e.Contains("booth is already assigned"));
    }

    // ── ValidateAttendeeRegistrationAsync ────────────────────────────────────

    [Fact]
    public async Task ValidateAttendeeRegistration_HappyPath_ReturnsNoErrors()
    {
        // Seed an attendee
        using var ctx = _db.CreateContext();
        ctx.Attendees.Add(new ArenaSync.Web.Models.Attendee
        {
            Id = 100, Name = "Test User", Email = "test@mail.com", Phone = "555-0001"
        });
        await ctx.SaveChangesAsync();

        var svc = CreateService();
        var errors = await svc.ValidateAttendeeRegistrationAsync(attendeeId: 100, eventId: 1);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ValidateAttendeeRegistration_AttendeeDoesNotExist_ReturnsError()
    {
        var svc = CreateService();
        var errors = await svc.ValidateAttendeeRegistrationAsync(attendeeId: 999, eventId: 1);
        Assert.Contains(errors, e => e.Contains("Attendee with ID 999"));
    }

    [Fact]
    public async Task ValidateAttendeeRegistration_DuplicateRegistration_ReturnsError()
    {
        using var ctx = _db.CreateContext();
        ctx.Attendees.Add(new ArenaSync.Web.Models.Attendee { Id = 101, Name = "Dup User", Email = "dup@mail.com", Phone = "555-0002" });
        ctx.RegistersFor.Add(new ArenaSync.Web.Models.RegistersFor { AttendeeId = 101, EventId = 1 });
        await ctx.SaveChangesAsync();

        var svc = CreateService();
        var errors = await svc.ValidateAttendeeRegistrationAsync(attendeeId: 101, eventId: 1);
        Assert.Contains(errors, e => e.Contains("already registered"));
    }

    // ── ValidateEventDatesAsync ───────────────────────────────────────────────

    [Fact]
    public async Task ValidateEventDates_ValidRange_ReturnsNoErrors()
    {
        var svc = CreateService();
        var start = DateTime.Now.AddDays(30);
        var end   = start.AddHours(3);
        var errors = await svc.ValidateEventDatesAsync(venueId: 2, startTime: start, endTime: end);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ValidateEventDates_EndBeforeStart_ReturnsError()
    {
        var svc = CreateService();
        var start = DateTime.Now.AddDays(5).AddHours(10);
        var end   = start.AddHours(-2); // end is before start
        var errors = await svc.ValidateEventDatesAsync(venueId: 1, startTime: start, endTime: end);
        Assert.Contains(errors, e => e.Contains("End time must be after start time"));
    }

    [Fact]
    public async Task ValidateEventDates_OverlappingVenueBooking_ReturnsError()
    {
        // Event 1 is at venue 1, days+10 09:00–11:00; try to book 10:00–12:00 — overlap
        var svc = CreateService();
        var start = DateTime.Now.AddDays(10).Date.AddHours(10);
        var end   = start.AddHours(2);
        var errors = await svc.ValidateEventDatesAsync(venueId: 1, startTime: start, endTime: end);
        Assert.Contains(errors, e => e.Contains("already booked"));
    }

    [Fact]
    public async Task ValidateEventDates_ExcludeEventId_AllowsEditingOwnEvent()
    {
        // Editing event 2 "Afternoon Showcase" (venue 1, 14:00–16:00) should not report
        // a conflict with itself. Event 3 overlaps with the 09:00 slot, so we use the
        // 14:00 slot which is only occupied by Event 2.
        var svc = CreateService();
        var start = DateTime.Now.AddDays(10).Date.AddHours(14);
        var end   = start.AddHours(2);
        var errors = await svc.ValidateEventDatesAsync(venueId: 1, startTime: start, endTime: end, excludeEventId: 2);
        Assert.DoesNotContain(errors, e => e.Contains("already booked"));
    }
}
