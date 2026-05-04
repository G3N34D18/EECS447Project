// -----------------------------------------------------------------------------
// File: ValidationTests.cs
// Project: ArenaSync.Web.Tests
// Purpose: Batch 4 — Integration tests confirming that invalid entity data is
//          caught by data-annotation validation and that the ValidationService
//          returns appropriate error messages.
// -----------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ArenaSync.Web.Tests.Integration;

public class ValidationTests : IDisposable
{
    private readonly SqliteTestDatabase _db;

    public ValidationTests()
    {
        _db = new SqliteTestDatabase();
        using var ctx = _db.CreateContext();
        TestData.SeedCoreAsync(ctx).GetAwaiter().GetResult();
    }

    public void Dispose() => _db.Dispose();

    // ── Helper: validate a model using DataAnnotations ─────────────────────────

    private static IList<ValidationResult> Validate(object model)
    {
        var results  = new List<ValidationResult>();
        var ctx      = new ValidationContext(model);
        Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
        return results;
    }

    // ── TeamFormModel validation ──────────────────────────────────────────────

    [Fact]
    public void TeamFormModel_Valid_PassesValidation()
    {
        var model = new TeamFormModel
        {
            Name        = "Thunder Hawks",
            Manager     = "Jordan Smith",
            Email       = "thunder@team.com",
            Phone       = "913-555-0101",
            PlayerCount = 15
        };
        Assert.Empty(Validate(model));
    }

    [Fact]
    public void TeamFormModel_MissingName_FailsValidation()
    {
        var model = new TeamFormModel
        {
            Name        = "",
            Manager     = "Jordan Smith",
            Email       = "thunder@team.com",
            Phone       = "913-555-0101",
            PlayerCount = 15
        };
        var results = Validate(model);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(TeamFormModel.Name)));
    }

    [Fact]
    public void TeamFormModel_InvalidEmail_FailsValidation()
    {
        var model = new TeamFormModel
        {
            Name        = "Thunder Hawks",
            Manager     = "Jordan Smith",
            Email       = "not-an-email",
            Phone       = "913-555-0101",
            PlayerCount = 15
        };
        var results = Validate(model);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(TeamFormModel.Email)));
    }

    [Fact]
    public void TeamFormModel_ZeroPlayerCount_FailsValidation()
    {
        var model = new TeamFormModel
        {
            Name        = "Thunder Hawks",
            Manager     = "Jordan Smith",
            Email       = "thunder@team.com",
            Phone       = "913-555-0101",
            PlayerCount = 0
        };
        var results = Validate(model);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(TeamFormModel.PlayerCount)));
    }

    // ── AttendeeFormModel validation ──────────────────────────────────────────

    [Fact]
    public void AttendeeFormModel_Valid_PassesValidation()
    {
        var model = new AttendeeFormModel
        {
            Name  = "Jane Doe",
            Email = "jane@example.com",
            Phone = "913-555-0200"
        };
        Assert.Empty(Validate(model));
    }

    [Fact]
    public void AttendeeFormModel_MissingEmail_FailsValidation()
    {
        var model = new AttendeeFormModel
        {
            Name  = "Jane Doe",
            Email = "",
            Phone = "913-555-0200"
        };
        var results = Validate(model);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AttendeeFormModel.Email)));
    }

    // ── VenueFormModel validation ─────────────────────────────────────────────

    [Fact]
    public void VenueFormModel_Valid_PassesValidation()
    {
        var model = new VenueFormModel
        {
            Name     = "Grand Arena",
            Address  = "123 Main St",
            Capacity = 5000
        };
        Assert.Empty(Validate(model));
    }

    [Fact]
    public void VenueFormModel_ZeroCapacity_FailsValidation()
    {
        var model = new VenueFormModel
        {
            Name     = "Grand Arena",
            Address  = "123 Main St",
            Capacity = 0
        };
        var results = Validate(model);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(VenueFormModel.Capacity)));
    }

    // ── EventFormModel validation ─────────────────────────────────────────────

    [Fact]
    public void EventFormModel_Valid_PassesValidation()
    {
        var model = new EventFormModel
        {
            Name       = "Summer Classic",
            VenueId    = 1,
            StartDate  = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            StartHour  = 9,
            StartMinute = 0,
            StartMeridiem = "AM",
            EndDate    = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            EndHour    = 5,
            EndMinute  = 0,
            EndMeridiem = "PM"
        };
        Assert.Empty(Validate(model));
    }

    [Fact]
    public void EventFormModel_MissingName_FailsValidation()
    {
        var model = new EventFormModel
        {
            Name    = "",
            VenueId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            EndDate   = DateOnly.FromDateTime(DateTime.Today.AddDays(7))
        };
        var results = Validate(model);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EventFormModel.Name)));
    }

    // ── ValidationService integration ─────────────────────────────────────────

    [Fact]
    public async Task ValidationService_CreateInvalidTeamAssignment_ReturnsErrors()
    {
        var svc = new ValidationService(_db.CreateContext(), NullLogger<ValidationService>.Instance);

        // Non-existent team, event, locker
        var errors = await svc.ValidateTeamAssignmentAsync(999, 999, 999);
        Assert.NotEmpty(errors);
        Assert.True(errors.Count >= 2, "Should have multiple errors for multiple invalid IDs.");
    }

    [Fact]
    public async Task ValidationService_ValidEventDates_ReturnsNoErrors()
    {
        var svc = new ValidationService(_db.CreateContext(), NullLogger<ValidationService>.Instance);

        // Venue 2 has only one event (event 5, days+11), so days+20 is free
        var start  = DateTime.Now.AddDays(20).Date.AddHours(10);
        var end    = start.AddHours(3);
        var errors = await svc.ValidateEventDatesAsync(venueId: 2, startTime: start, endTime: end);
        Assert.Empty(errors);
    }
}
