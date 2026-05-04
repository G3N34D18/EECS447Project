using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;

namespace ArenaSync.Web.Tests.Integration;

public class ReportingIntegrationTests
{
    [Fact]
    public async Task Reporting_pipeline_rolls_database_state_into_dashboard_and_conflict_report()
    {
        using var database = new SqliteTestDatabase();
        await using var context = database.CreateContext();
        await TestData.SeedCoreAsync(context);
        context.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
        await context.SaveChangesAsync();
        var service = new ReportingService(context, new AssignmentService(context));

        var dashboard = await service.GetDashboardSummaryAsync();
        var conflicts = await service.GetConflictSummaryAsync(eventId: 1);

        Assert.Equal(1, dashboard.PendingTeamAssignments);
        Assert.Contains(conflicts, c => c.Type == "Missing Locker");
    }
}
