using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Tests.Services;

public class TeamServiceTests
{
    [Fact]
    public async Task AssignTeamToEventAsync_registers_team_for_non_overlapping_event()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new TeamService(context);

        var assigned = await service.AssignTeamToEventAsync(teamId: 1, eventId: 1);

        Assert.True(assigned);
        Assert.True(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 1));
    }

    [Fact]
    public async Task AssignTeamToEventAsync_rejects_overlapping_event_without_request()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new TeamService(context);

        var assigned = await service.AssignTeamToEventAsync(teamId: 1, eventId: 3);

        Assert.False(assigned);
        Assert.False(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 3));
    }

    [Fact]
    public async Task SubmitOverlapParticipationRequestAsync_creates_pending_request_for_overlap()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new TeamService(context);

        var result = await service.SubmitOverlapParticipationRequestAsync(
            teamId: 1,
            targetEventId: 3,
            reason: "Different squad is attending.");

        Assert.True(result.Succeeded);
        var request = await context.TeamEventRequests.SingleAsync();
        Assert.Equal(TeamEventRequestType.OverlapParticipation, request.Type);
        Assert.Equal(1, request.TeamId);
        Assert.Equal(3, request.TargetEventId);
        Assert.Equal(RequestStatus.Pending, request.Status);
    }

    [Fact]
    public async Task ResignTeamFromEventAsync_removes_participation_and_locker_assignment_outside_cutoff()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
        await context.SaveChangesAsync();
        var service = new TeamService(context);

        var result = await service.ResignTeamFromEventAsync(teamId: 1, eventId: 1);

        Assert.True(result.Succeeded);
        Assert.False(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 1));
        Assert.False(await context.TeamAssignments.AnyAsync(a => a.TeamId == 1 && a.EventId == 1));
    }

    [Fact]
    public async Task ResignTeamFromEventAsync_rejects_direct_resignation_inside_cutoff()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 4 });
        await context.SaveChangesAsync();
        var service = new TeamService(context);

        var result = await service.ResignTeamFromEventAsync(teamId: 1, eventId: 4);

        Assert.False(result.Succeeded);
        Assert.True(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 4));
    }

    [Fact]
    public async Task SubmitLateResignationRequestAsync_creates_request_inside_cutoff()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 4 });
        await context.SaveChangesAsync();
        var service = new TeamService(context);

        var result = await service.SubmitLateResignationRequestAsync(
            teamId: 1,
            sourceEventId: 4,
            reason: "Travel emergency.");

        Assert.True(result.Succeeded);
        var request = await context.TeamEventRequests.SingleAsync();
        Assert.Equal(TeamEventRequestType.LateResignation, request.Type);
        Assert.Equal(4, request.SourceEventId);
        Assert.Equal(RequestStatus.Pending, request.Status);
    }

    [Fact]
    public async Task SubmitReassignmentRequestAsync_requires_source_registration()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        var service = new TeamService(context);

        var result = await service.SubmitReassignmentRequestAsync(
            teamId: 1,
            sourceEventId: 1,
            targetEventId: 2,
            reason: "Need a later slot.");

        Assert.False(result.Succeeded);
        Assert.Empty(context.TeamEventRequests);
    }

    [Fact]
    public async Task ApproveTeamEventRequestAsync_moves_team_for_reassignment()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.TeamEventRequests.Add(new TeamEventRequest
        {
            TeamId = 1,
            Type = TeamEventRequestType.Reassignment,
            SourceEventId = 1,
            TargetEventId = 2,
            Reason = "Need a later slot."
        });
        await context.SaveChangesAsync();
        var requestId = await context.TeamEventRequests.Select(r => r.Id).SingleAsync();
        var service = new TeamService(context);

        var result = await service.ApproveTeamEventRequestAsync(requestId);

        Assert.True(result.Succeeded);
        Assert.False(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 1));
        Assert.True(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 2));
        Assert.Equal(RequestStatus.Approved, await context.TeamEventRequests.Select(r => r.Status).SingleAsync());
    }

    [Fact]
    public async Task DenyTeamEventRequestAsync_leaves_participation_unchanged()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        context.TeamEventRequests.Add(new TeamEventRequest
        {
            TeamId = 1,
            Type = TeamEventRequestType.Reassignment,
            SourceEventId = 1,
            TargetEventId = 2,
            Reason = "Need a later slot."
        });
        await context.SaveChangesAsync();
        var requestId = await context.TeamEventRequests.Select(r => r.Id).SingleAsync();
        var service = new TeamService(context);

        var result = await service.DenyTeamEventRequestAsync(requestId);

        Assert.True(result.Succeeded);
        Assert.True(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 1));
        Assert.False(await context.ParticipatesIn.AnyAsync(p => p.TeamId == 1 && p.EventId == 2));
        Assert.Equal(RequestStatus.Denied, await context.TeamEventRequests.Select(r => r.Status).SingleAsync());
    }
}
