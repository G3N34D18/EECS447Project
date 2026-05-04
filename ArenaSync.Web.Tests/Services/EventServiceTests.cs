using ArenaSync.Web.Models;
using ArenaSync.Web.Services;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ArenaSync.Web.Tests.Services;

public class EventServiceTests
{
    [Fact]
    public async Task DeleteEventAsync_removes_all_event_dependencies_before_deleting_event()
    {
        using var db = new SqliteTestDatabase();
        await using (var ctx = db.CreateContext())
        {
            await TestData.SeedCoreAsync(ctx);

            ctx.Attendees.Add(new Attendee
            {
                Id = 1,
                Name = "Jordan Fan",
                Email = "jordan@example.com",
                Phone = "555-0301"
            });

            ctx.ParticipatesIn.Add(new ParticipatesIn { TeamId = 1, EventId = 1 });
            ctx.RegistersFor.Add(new RegistersFor { AttendeeId = 1, EventId = 1 });
            ctx.SuppliesAt.Add(new SuppliesAt { VendorId = 1, EventId = 1 });
            ctx.TeamAssignments.Add(new TeamAssignment { TeamId = 1, EventId = 1, LockerId = 1 });
            ctx.VendorAssignments.Add(new VendorAssignment { VendorId = 1, EventId = 1, BoothId = 1 });
            ctx.TeamEventRequests.Add(new TeamEventRequest
            {
                TeamId = 1,
                Type = TeamEventRequestType.Reassignment,
                SourceEventId = 1,
                TargetEventId = 2,
                Reason = "Schedule issue",
                Status = RequestStatus.Pending
            });
            ctx.TeamEventRequests.Add(new TeamEventRequest
            {
                TeamId = 2,
                Type = TeamEventRequestType.OverlapParticipation,
                TargetEventId = 1,
                Reason = "Separate roster",
                Status = RequestStatus.Pending
            });
            ctx.TeamReassignmentRequests.Add(new TeamReassignmentRequest
            {
                TeamId = 3,
                RequestedEventId = 1,
                Reason = "Legacy request",
                Status = RequestStatus.Pending
            });

            await ctx.SaveChangesAsync();
        }

        await using (var ctx = db.CreateContext())
        {
            var service = new EventService(ctx, NullLogger<EventService>.Instance);

            var deleted = await service.DeleteEventAsync(1);

            Assert.True(deleted);
        }

        await using (var ctx = db.CreateContext())
        {
            Assert.Null(await ctx.Events.FindAsync(1));
            Assert.DoesNotContain(ctx.ParticipatesIn, p => p.EventId == 1);
            Assert.DoesNotContain(ctx.RegistersFor, r => r.EventId == 1);
            Assert.DoesNotContain(ctx.SuppliesAt, s => s.EventId == 1);
            Assert.DoesNotContain(ctx.TeamAssignments, a => a.EventId == 1);
            Assert.DoesNotContain(ctx.VendorAssignments, a => a.EventId == 1);
            Assert.DoesNotContain(ctx.TeamEventRequests, r => r.SourceEventId == 1 || r.TargetEventId == 1);
            Assert.DoesNotContain(ctx.TeamReassignmentRequests, r => r.RequestedEventId == 1);
        }
    }
}
