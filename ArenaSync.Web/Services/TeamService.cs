using ArenaSync.Web.Data;
using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class TeamService : ITeamService
    {
        private static readonly TimeSpan ResignationCutoff = TimeSpan.FromHours(48);
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL TEAMS
        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        // GET TEAM BY ID
        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.Event)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.LockerRoom)
                .Include(t => t.ParticipatesIn)
                    .ThenInclude(p => p.Event)
                        .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // CREATE TEAM (with duplicate name check)
        public async Task<bool> CreateTeamAsync(Team team)
        {
            // Duplicate name check
            bool exists = await _context.Teams
                .AnyAsync(t => t.Name.ToLower() == team.Name.ToLower());

            if (exists)
                return false;

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return true;
        }

        // UPDATE TEAM 
        public async Task<Team> UpdateTeamAsync(Team team)
        {
            var existingTeam = await _context.Teams.FindAsync(team.Id);
            if (existingTeam == null) return null;
            existingTeam.Name = team.Name;
            existingTeam.Manager = team.Manager;
            existingTeam.Email = team.Email;
            existingTeam.Phone = team.Phone;
            existingTeam.PlayerCount = team.PlayerCount;
            await _context.SaveChangesAsync();
            return existingTeam;
        }

        // DELETE TEAM
        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

            var hasDependencies =
                await _context.ParticipatesIn.AnyAsync(p => p.TeamId == id)
                || await _context.TeamAssignments.AnyAsync(a => a.TeamId == id)
                || await _context.TeamEventRequests.AnyAsync(r => r.TeamId == id)
                || await _context.TeamReassignmentRequests.AnyAsync(r => r.TeamId == id);

            if (hasDependencies)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        // ASSIGN TEAM TO EVENT (with duplicate check)
        public async Task<bool> AssignTeamToEventAsync(int teamId, int eventId)
        {
            var exists = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == teamId && p.EventId == eventId);

            if (exists)
                return false;

            var hasOverlap = (await GetOverlappingEventsForTeamAsync(teamId, eventId)).Any();
            if (hasOverlap)
                return false;

            var assignment = new ParticipatesIn
            {
                TeamId = teamId,
                EventId = eventId
            };

            _context.ParticipatesIn.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Event>> GetOverlappingEventsForTeamAsync(int teamId, int targetEventId, int? excludedEventId = null)
        {
            var targetEvent = await _context.Events.FindAsync(targetEventId);
            if (targetEvent is null)
            {
                return new List<Event>();
            }

            return await _context.ParticipatesIn
                .Where(p => p.TeamId == teamId && p.EventId != targetEventId)
                .Where(p => excludedEventId == null || p.EventId != excludedEventId.Value)
                .Include(p => p.Event)
                .Select(p => p.Event!)
                .Where(e => e.StartTime < targetEvent.EndTime && targetEvent.StartTime < e.EndTime)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        // GET TEAMS FOR EVENT
        public async Task<List<Team>> GetTeamsForEventAsync(int eventId)
        {
            return await _context.ParticipatesIn
                .Where(p => p.EventId == eventId)
                .Include(p => p.Team)
                .Select(p => p.Team)
                .ToListAsync();
        }

        public async Task<AssignmentResult> ResignTeamFromEventAsync(int teamId, int eventId)
        {
            var participation = await _context.ParticipatesIn
                .Include(p => p.Event)
                .FirstOrDefaultAsync(p => p.TeamId == teamId && p.EventId == eventId);

            if (participation is null)
            {
                return AssignmentResult.Failure("This team is not registered for that event.");
            }

            if (participation.Event is not null && participation.Event.StartTime <= DateTime.Now.Add(ResignationCutoff))
            {
                return AssignmentResult.Failure("Teams can resign only when the event is more than 48 hours away.");
            }

            var lockerAssignments = await _context.TeamAssignments
                .Where(a => a.TeamId == teamId && a.EventId == eventId)
                .ToListAsync();

            _context.TeamAssignments.RemoveRange(lockerAssignments);
            _context.ParticipatesIn.Remove(participation);
            await _context.SaveChangesAsync();

            return AssignmentResult.Success("Team resigned from the event.");
        }

        public async Task<AssignmentResult> SubmitReassignmentRequestAsync(int teamId, int sourceEventId, int targetEventId, string reason)
        {
            if (sourceEventId == targetEventId)
            {
                return AssignmentResult.Failure("Choose a different event for reassignment.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return AssignmentResult.Failure("A reason is required.");
            }

            var isRegisteredForSource = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == teamId && p.EventId == sourceEventId);
            if (!isRegisteredForSource)
            {
                return AssignmentResult.Failure("This team must already be registered for the source event.");
            }

            var targetExists = await _context.Events.AnyAsync(e => e.Id == targetEventId);
            if (!targetExists)
            {
                return AssignmentResult.Failure("Requested event could not be found.");
            }

            var duplicate = await _context.TeamEventRequests.AnyAsync(r =>
                r.TeamId == teamId
                && r.Type == TeamEventRequestType.Reassignment
                && r.SourceEventId == sourceEventId
                && r.TargetEventId == targetEventId
                && r.Status == RequestStatus.Pending);

            if (duplicate)
            {
                return AssignmentResult.Failure("A pending reassignment request for these events already exists.");
            }

            _context.TeamEventRequests.Add(new TeamEventRequest
            {
                TeamId = teamId,
                Type = TeamEventRequestType.Reassignment,
                SourceEventId = sourceEventId,
                TargetEventId = targetEventId,
                Reason = reason.Trim()
            });

            await _context.SaveChangesAsync();
            return AssignmentResult.Success("Reassignment request submitted.");
        }

        public async Task<AssignmentResult> SubmitLateResignationRequestAsync(int teamId, int sourceEventId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return AssignmentResult.Failure("A reason is required.");
            }

            var participation = await _context.ParticipatesIn
                .Include(p => p.Event)
                .FirstOrDefaultAsync(p => p.TeamId == teamId && p.EventId == sourceEventId);

            if (participation is null)
            {
                return AssignmentResult.Failure("This team is not registered for that event.");
            }

            if (participation.Event is not null && participation.Event.StartTime > DateTime.Now.Add(ResignationCutoff))
            {
                return AssignmentResult.Failure("This event is outside the late resignation window. The team can resign directly.");
            }

            var duplicate = await _context.TeamEventRequests.AnyAsync(r =>
                r.TeamId == teamId
                && r.Type == TeamEventRequestType.LateResignation
                && r.SourceEventId == sourceEventId
                && r.Status == RequestStatus.Pending);

            if (duplicate)
            {
                return AssignmentResult.Failure("A pending resignation request for this event already exists.");
            }

            _context.TeamEventRequests.Add(new TeamEventRequest
            {
                TeamId = teamId,
                Type = TeamEventRequestType.LateResignation,
                SourceEventId = sourceEventId,
                Reason = reason.Trim()
            });

            await _context.SaveChangesAsync();
            return AssignmentResult.Success("Late resignation request submitted.");
        }

        public async Task<AssignmentResult> SubmitOverlapParticipationRequestAsync(int teamId, int targetEventId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return AssignmentResult.Failure("A reason is required.");
            }

            var alreadyRegistered = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == teamId && p.EventId == targetEventId);
            if (alreadyRegistered)
            {
                return AssignmentResult.Failure("This team is already registered for that event.");
            }

            var overlaps = await GetOverlappingEventsForTeamAsync(teamId, targetEventId);
            if (!overlaps.Any())
            {
                return AssignmentResult.Failure("This event does not overlap the team's current schedule. Register the team directly.");
            }

            var duplicate = await _context.TeamEventRequests.AnyAsync(r =>
                r.TeamId == teamId
                && r.Type == TeamEventRequestType.OverlapParticipation
                && r.TargetEventId == targetEventId
                && r.Status == RequestStatus.Pending);

            if (duplicate)
            {
                return AssignmentResult.Failure("A pending overlap participation request for this event already exists.");
            }

            _context.TeamEventRequests.Add(new TeamEventRequest
            {
                TeamId = teamId,
                Type = TeamEventRequestType.OverlapParticipation,
                TargetEventId = targetEventId,
                Reason = reason.Trim()
            });

            await _context.SaveChangesAsync();
            return AssignmentResult.Success("Overlap participation request submitted.");
        }

        public async Task<List<TeamEventRequest>> GetPendingRequestsForTeamAsync(int teamId)
        {
            return await _context.TeamEventRequests
                .Include(r => r.SourceEvent)
                .Include(r => r.TargetEvent)
                .Where(r => r.TeamId == teamId && r.Status == RequestStatus.Pending)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<TeamEventRequest>> GetPendingTeamEventRequestsAsync()
        {
            return await _context.TeamEventRequests
                .Include(r => r.Team)
                    .ThenInclude(t => t.ParticipatesIn)
                        .ThenInclude(p => p.Event)
                .Include(r => r.SourceEvent)
                .Include(r => r.TargetEvent)
                .Where(r => r.Status == RequestStatus.Pending)
                .OrderBy(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<AssignmentResult> ApproveTeamEventRequestAsync(int requestId)
        {
            var request = await _context.TeamEventRequests
                .Include(r => r.Team)
                    .ThenInclude(t => t.ParticipatesIn)
                        .ThenInclude(p => p.Event)
                .Include(r => r.SourceEvent)
                .Include(r => r.TargetEvent)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request is null)
            {
                return AssignmentResult.Failure("Team event request could not be found.");
            }

            if (request.Status != RequestStatus.Pending)
            {
                return AssignmentResult.Failure("Only pending requests can be approved.");
            }

            var result = request.Type switch
            {
                TeamEventRequestType.Reassignment => await ApplyReassignmentRequestAsync(request),
                TeamEventRequestType.LateResignation => await ApplyLateResignationRequestAsync(request),
                TeamEventRequestType.OverlapParticipation => await ApplyOverlapParticipationRequestAsync(request),
                _ => AssignmentResult.Failure("Unsupported request type.")
            };

            if (!result.Succeeded)
            {
                return result;
            }

            request.Status = RequestStatus.Approved;
            request.ResolvedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return AssignmentResult.Success("Team event request approved.");
        }

        public async Task<AssignmentResult> DenyTeamEventRequestAsync(int requestId)
        {
            var request = await _context.TeamEventRequests.FindAsync(requestId);
            if (request is null)
            {
                return AssignmentResult.Failure("Team event request could not be found.");
            }

            if (request.Status != RequestStatus.Pending)
            {
                return AssignmentResult.Failure("Only pending requests can be denied.");
            }

            request.Status = RequestStatus.Denied;
            request.ResolvedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return AssignmentResult.Success("Team event request denied.");
        }

        private async Task<AssignmentResult> ApplyReassignmentRequestAsync(TeamEventRequest request)
        {
            if (request.SourceEventId is null || request.TargetEventId is null)
            {
                return AssignmentResult.Failure("Reassignment requests require a source and target event.");
            }

            var sourceParticipation = await _context.ParticipatesIn
                .FirstOrDefaultAsync(p => p.TeamId == request.TeamId && p.EventId == request.SourceEventId.Value);
            if (sourceParticipation is null)
            {
                return AssignmentResult.Failure("The team is no longer registered for the source event.");
            }

            var targetExists = await _context.Events.AnyAsync(e => e.Id == request.TargetEventId.Value);
            if (!targetExists)
            {
                return AssignmentResult.Failure("The target event no longer exists.");
            }

            var lockerAssignments = await _context.TeamAssignments
                .Where(a => a.TeamId == request.TeamId && a.EventId == request.SourceEventId.Value)
                .ToListAsync();
            _context.TeamAssignments.RemoveRange(lockerAssignments);
            _context.ParticipatesIn.Remove(sourceParticipation);

            var alreadyInTarget = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == request.TeamId && p.EventId == request.TargetEventId.Value);
            if (!alreadyInTarget)
            {
                _context.ParticipatesIn.Add(new ParticipatesIn
                {
                    TeamId = request.TeamId,
                    EventId = request.TargetEventId.Value
                });
            }

            return AssignmentResult.Success();
        }

        private async Task<AssignmentResult> ApplyLateResignationRequestAsync(TeamEventRequest request)
        {
            if (request.SourceEventId is null)
            {
                return AssignmentResult.Failure("Late resignation requests require a source event.");
            }

            var participation = await _context.ParticipatesIn
                .FirstOrDefaultAsync(p => p.TeamId == request.TeamId && p.EventId == request.SourceEventId.Value);
            if (participation is null)
            {
                return AssignmentResult.Failure("The team is no longer registered for this event.");
            }

            var lockerAssignments = await _context.TeamAssignments
                .Where(a => a.TeamId == request.TeamId && a.EventId == request.SourceEventId.Value)
                .ToListAsync();
            _context.TeamAssignments.RemoveRange(lockerAssignments);
            _context.ParticipatesIn.Remove(participation);

            return AssignmentResult.Success();
        }

        private async Task<AssignmentResult> ApplyOverlapParticipationRequestAsync(TeamEventRequest request)
        {
            if (request.TargetEventId is null)
            {
                return AssignmentResult.Failure("Overlap participation requests require a target event.");
            }

            var targetExists = await _context.Events.AnyAsync(e => e.Id == request.TargetEventId.Value);
            if (!targetExists)
            {
                return AssignmentResult.Failure("The target event no longer exists.");
            }

            var alreadyRegistered = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == request.TeamId && p.EventId == request.TargetEventId.Value);
            if (!alreadyRegistered)
            {
                _context.ParticipatesIn.Add(new ParticipatesIn
                {
                    TeamId = request.TeamId,
                    EventId = request.TargetEventId.Value
                });
            }

            return AssignmentResult.Success();
        }
    }
}
