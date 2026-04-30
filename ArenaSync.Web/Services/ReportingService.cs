using System.Text;
using ArenaSync.Web.Data;
using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class ReportingService : IReportingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAssignmentService _assignmentService;

        public ReportingService(ApplicationDbContext context, IAssignmentService assignmentService)
        {
            _context = context;
            _assignmentService = assignmentService;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var conflicts = await GetConflictSummaryAsync();

            return new DashboardSummaryDto
            {
                TotalEvents = await _context.Events.CountAsync(),
                TotalTeams = await _context.Teams.CountAsync(),
                TotalVendors = await _context.Vendors.CountAsync(),
                TotalAttendees = await _context.Attendees.CountAsync(),
                UpcomingEvents = await _context.Events.CountAsync(e => e.StartTime > DateTime.Now),
                PendingTeamAssignments = await _context.ParticipatesIn
                    .CountAsync(p => !_context.TeamAssignments.Any(a => a.EventId == p.EventId && a.TeamId == p.TeamId)),
                PendingVendorAssignments = await _context.SuppliesAt
                    .CountAsync(s => !_context.VendorAssignments.Any(a => a.EventId == s.EventId && a.VendorId == s.VendorId)),
                PendingTeamEventRequests = await _context.TeamEventRequests.CountAsync(r => r.Status == RequestStatus.Pending),
                CriticalConflicts = conflicts.Count(c => c.Severity == AssignmentConflictSeverity.Critical),
                WarningConflicts = conflicts.Count(c => c.Severity == AssignmentConflictSeverity.Warning),
                CriticalAlerts = conflicts
                    .Where(c => c.Severity == AssignmentConflictSeverity.Critical)
                    .Take(5)
                    .ToList(),
                RecentEvents = await GetDashboardEventsAsync()
            };
        }

        public async Task<EventRosterDto?> GetEventRosterAsync(int eventId)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity is null)
            {
                return null;
            }

            var teamParticipations = await _context.ParticipatesIn
                .Where(p => p.EventId == eventId)
                .Include(p => p.Team)
                .ToListAsync();

            var attendeeRegistrations = await _context.RegistersFor
                .Where(r => r.EventId == eventId)
                .Include(r => r.Attendee)
                .ToListAsync();

            var suppliers = await _context.SuppliesAt
                .Where(s => s.EventId == eventId)
                .Include(s => s.Vendor)
                .ToListAsync();

            var teamAssignments = await _assignmentService.GetTeamAssignmentsForEventAsync(eventId);
            var vendorAssignments = await _assignmentService.GetVendorAssignmentsForEventAsync(eventId);

            return new EventRosterDto
            {
                EventId = eventEntity.Id,
                EventName = eventEntity.Name,
                VenueName = eventEntity.Venue?.Name ?? "Unknown venue",
                StartTime = eventEntity.StartTime,
                EndTime = eventEntity.EndTime,
                Description = eventEntity.Description,
                Teams = teamParticipations
                    .Where(p => p.Team is not null)
                    .OrderBy(p => p.Team.Name)
                    .Select(p =>
                    {
                        var assignment = teamAssignments.FirstOrDefault(a => a.TeamId == p.TeamId);
                        return new EventRosterTeamDto
                        {
                            TeamId = p.TeamId,
                            TeamName = p.Team.Name,
                            Manager = p.Team.Manager,
                            PlayerCount = p.Team.PlayerCount,
                            LockerRoom = assignment is null ? "Unassigned" : $"Room {assignment.LockerRoom.RoomNumber}"
                        };
                    })
                    .ToList(),
                Attendees = attendeeRegistrations
                    .Where(r => r.Attendee is not null)
                    .OrderBy(r => r.Attendee.Name)
                    .Select(r => new EventRosterAttendeeDto
                    {
                        AttendeeId = r.AttendeeId,
                        Name = r.Attendee.Name,
                        Email = r.Attendee.Email,
                        Phone = r.Attendee.Phone
                    })
                    .ToList(),
                Vendors = suppliers
                    .Where(s => s.Vendor is not null)
                    .OrderBy(s => s.Vendor!.Name)
                    .Select(s =>
                    {
                        var assignment = vendorAssignments.FirstOrDefault(a => a.VendorId == s.VendorId);
                        return new EventRosterVendorDto
                        {
                            VendorId = s.VendorId,
                            VendorName = s.Vendor!.Name,
                            Type = s.Vendor.Type,
                            Booth = assignment?.Booth is null ? "Unassigned" : $"Booth {assignment.Booth.BoothNumber}"
                        };
                    })
                    .ToList()
            };
        }

        public async Task<TeamScheduleReportDto?> GetTeamScheduleAsync(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team is null)
            {
                return null;
            }

            var participations = await _context.ParticipatesIn
                .Where(p => p.TeamId == teamId)
                .Include(p => p.Event)
                    .ThenInclude(e => e.Venue)
                .OrderBy(p => p.Event.StartTime)
                .ToListAsync();

            var assignments = await _context.TeamAssignments
                .Where(a => a.TeamId == teamId)
                .Include(a => a.LockerRoom)
                .ToListAsync();

            return new TeamScheduleReportDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                Manager = team.Manager,
                Email = team.Email,
                Phone = team.Phone,
                PlayerCount = team.PlayerCount,
                Events = participations
                    .Where(p => p.Event is not null)
                    .Select(p =>
                    {
                        var assignment = assignments.FirstOrDefault(a => a.EventId == p.EventId);
                        return new TeamScheduleEventDto
                        {
                            EventId = p.EventId,
                            EventName = p.Event.Name,
                            VenueName = p.Event.Venue?.Name ?? "Unknown venue",
                            StartTime = p.Event.StartTime,
                            EndTime = p.Event.EndTime,
                            LockerRoom = assignment is null ? "Unassigned" : $"Room {assignment.LockerRoom.RoomNumber}"
                        };
                    })
                    .ToList()
            };
        }

        public async Task<VenueOccupancyReportDto?> GetVenueOccupancyAsync(int venueId)
        {
            var venue = await _context.Venues
                .Include(v => v.LockerRooms)
                .Include(v => v.VendorBooths)
                .FirstOrDefaultAsync(v => v.Id == venueId);

            if (venue is null)
            {
                return null;
            }

            var events = await _context.Events
                .Where(e => e.VenueId == venueId)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            var eventIds = events.Select(e => e.Id).ToList();
            var teamCounts = await CountByEventAsync(_context.ParticipatesIn.Where(p => eventIds.Contains(p.EventId)).Select(p => p.EventId));
            var attendeeCounts = await CountByEventAsync(_context.RegistersFor.Where(r => eventIds.Contains(r.EventId)).Select(r => r.EventId));
            var vendorCounts = await CountByEventAsync(_context.SuppliesAt.Where(s => eventIds.Contains(s.EventId)).Select(s => s.EventId));
            var lockerCounts = await CountByEventAsync(_context.TeamAssignments.Where(a => eventIds.Contains(a.EventId)).Select(a => a.EventId));
            var boothCounts = await CountByEventAsync(_context.VendorAssignments.Where(a => eventIds.Contains(a.EventId)).Select(a => a.EventId));

            return new VenueOccupancyReportDto
            {
                VenueId = venue.Id,
                VenueName = venue.Name,
                Address = venue.Address,
                Capacity = venue.Capacity,
                TotalLockerRooms = venue.LockerRooms.Count,
                TotalVendorBooths = venue.VendorBooths.Count,
                Events = events.Select(e => new VenueOccupancyEventDto
                {
                    EventId = e.Id,
                    EventName = e.Name,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    TeamCount = GetCount(teamCounts, e.Id),
                    AssignedLockers = GetCount(lockerCounts, e.Id),
                    VendorCount = GetCount(vendorCounts, e.Id),
                    AssignedBooths = GetCount(boothCounts, e.Id),
                    AttendeeCount = GetCount(attendeeCounts, e.Id)
                }).ToList()
            };
        }

        public async Task<List<ConflictReportRowDto>> GetConflictSummaryAsync(int? eventId = null)
        {
            var eventsQuery = _context.Events
                .Include(e => e.Venue)
                .AsQueryable();

            if (eventId.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.Id == eventId.Value);
            }

            var events = await eventsQuery
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            var rows = new List<ConflictReportRowDto>();
            foreach (var eventEntity in events)
            {
                var conflicts = await _assignmentService.GetAssignmentConflictsAsync(eventEntity.Id);
                rows.AddRange(conflicts.Select(c => new ConflictReportRowDto
                {
                    EventId = eventEntity.Id,
                    EventName = eventEntity.Name,
                    VenueName = eventEntity.Venue?.Name ?? "Unknown venue",
                    Severity = c.Severity,
                    Type = c.Type,
                    Description = c.Description,
                    AffectedEntity = c.AffectedEntity
                }));
            }

            return rows
                .OrderByDescending(r => r.Severity)
                .ThenBy(r => r.EventName)
                .ThenBy(r => r.Type)
                .ToList();
        }

        public async Task<string> ExportReportToCsvAsync(ReportType type, int? entityId = null)
        {
            return type switch
            {
                ReportType.EventRoster => await ExportEventRosterAsync(RequireEntityId(entityId, "event")),
                ReportType.TeamSchedule => await ExportTeamScheduleAsync(RequireEntityId(entityId, "team")),
                ReportType.VenueOccupancy => await ExportVenueOccupancyAsync(RequireEntityId(entityId, "venue")),
                ReportType.ConflictSummary => ExportConflictSummary(await GetConflictSummaryAsync(entityId)),
                _ => string.Empty
            };
        }

        private async Task<List<DashboardEventDto>> GetDashboardEventsAsync()
        {
            var events = await _context.Events
                .Include(e => e.Venue)
                .OrderBy(e => e.StartTime)
                .Take(5)
                .ToListAsync();

            var eventIds = events.Select(e => e.Id).ToList();
            var teamCounts = await CountByEventAsync(_context.ParticipatesIn.Where(p => eventIds.Contains(p.EventId)).Select(p => p.EventId));
            var attendeeCounts = await CountByEventAsync(_context.RegistersFor.Where(r => eventIds.Contains(r.EventId)).Select(r => r.EventId));
            var vendorCounts = await CountByEventAsync(_context.SuppliesAt.Where(s => eventIds.Contains(s.EventId)).Select(s => s.EventId));

            return events.Select(e => new DashboardEventDto
            {
                EventId = e.Id,
                Name = e.Name,
                VenueName = e.Venue?.Name ?? "Unknown venue",
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                TeamCount = GetCount(teamCounts, e.Id),
                AttendeeCount = GetCount(attendeeCounts, e.Id),
                VendorCount = GetCount(vendorCounts, e.Id)
            }).ToList();
        }

        private static async Task<Dictionary<int, int>> CountByEventAsync(IQueryable<int> eventIds)
        {
            return await eventIds
                .GroupBy(id => id)
                .Select(g => new { EventId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.EventId, g => g.Count);
        }

        private static int GetCount(Dictionary<int, int> counts, int eventId)
            => counts.TryGetValue(eventId, out var count) ? count : 0;

        private async Task<string> ExportEventRosterAsync(int eventId)
        {
            var report = await GetEventRosterAsync(eventId);
            if (report is null)
            {
                return string.Empty;
            }

            var rows = new List<string[]> { new[] { "Section", "Name", "Detail", "Assignment" } };
            rows.AddRange(report.Teams.Select(t => new[] { "Team", t.TeamName, t.Manager, t.LockerRoom }));
            rows.AddRange(report.Attendees.Select(a => new[] { "Attendee", a.Name, a.Email, a.Phone }));
            rows.AddRange(report.Vendors.Select(v => new[] { "Vendor", v.VendorName, v.Type, v.Booth }));
            return BuildCsv(rows);
        }

        private async Task<string> ExportTeamScheduleAsync(int teamId)
        {
            var report = await GetTeamScheduleAsync(teamId);
            if (report is null)
            {
                return string.Empty;
            }

            var rows = new List<string[]> { new[] { "Event", "Venue", "Start", "End", "Locker" } };
            rows.AddRange(report.Events.Select(e => new[]
            {
                e.EventName,
                e.VenueName,
                e.StartTime.ToString("s"),
                e.EndTime.ToString("s"),
                e.LockerRoom
            }));
            return BuildCsv(rows);
        }

        private async Task<string> ExportVenueOccupancyAsync(int venueId)
        {
            var report = await GetVenueOccupancyAsync(venueId);
            if (report is null)
            {
                return string.Empty;
            }

            var rows = new List<string[]> { new[] { "Event", "Start", "Teams", "Assigned Lockers", "Vendors", "Assigned Booths", "Attendees" } };
            rows.AddRange(report.Events.Select(e => new[]
            {
                e.EventName,
                e.StartTime.ToString("s"),
                e.TeamCount.ToString(),
                e.AssignedLockers.ToString(),
                e.VendorCount.ToString(),
                e.AssignedBooths.ToString(),
                e.AttendeeCount.ToString()
            }));
            return BuildCsv(rows);
        }

        private static string ExportConflictSummary(List<ConflictReportRowDto> conflicts)
        {
            var rows = new List<string[]> { new[] { "Event", "Venue", "Severity", "Type", "Affected Entity", "Description" } };
            rows.AddRange(conflicts.Select(c => new[]
            {
                c.EventName,
                c.VenueName,
                c.Severity.ToString(),
                c.Type,
                c.AffectedEntity,
                c.Description
            }));
            return BuildCsv(rows);
        }

        private static int RequireEntityId(int? entityId, string entityName)
        {
            return entityId ?? throw new ArgumentException($"A {entityName} must be selected for this report.");
        }

        private static string BuildCsv(IEnumerable<string[]> rows)
        {
            var builder = new StringBuilder();
            foreach (var row in rows)
            {
                builder.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }

            return builder.ToString();
        }

        private static string EscapeCsv(string value)
        {
            value ??= string.Empty;
            return value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r')
                ? $"\"{value.Replace("\"", "\"\"")}\""
                : value;
        }
    }
}
