using ArenaSync.Web.Data;
using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public AssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeamAssignment>> GetTeamAssignmentsForEventAsync(int eventId)
        {
            return await _context.TeamAssignments
                .Where(ta => ta.EventId == eventId)
                .Include(ta => ta.Team)
                .Include(ta => ta.Event)
                    .ThenInclude(e => e.Venue)
                .Include(ta => ta.LockerRoom)
                    .ThenInclude(lr => lr.Venue)
                .OrderBy(ta => ta.Team!.Name)
                .ToListAsync();
        }

        public async Task<List<VendorAssignment>> GetVendorAssignmentsForEventAsync(int eventId)
        {
            return await _context.VendorAssignments
                .Where(va => va.EventId == eventId)
                .Include(va => va.Vendor)
                .Include(va => va.Event)
                    .ThenInclude(e => e!.Venue)
                .Include(va => va.Booth)
                    .ThenInclude(b => b!.Venue)
                .OrderBy(va => va.Vendor!.Name)
                .ToListAsync();
        }

        public async Task<List<LockerRoom>> GetAvailableLockerRoomsForEventAsync(int eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity is null)
            {
                return new List<LockerRoom>();
            }

            var assignedLockerIds = await _context.TeamAssignments
                .Where(ta => ta.EventId == eventId)
                .Select(ta => ta.LockerId)
                .ToListAsync();

            return await _context.LockerRooms
                .Where(lr => lr.VenueId == eventEntity.VenueId && !assignedLockerIds.Contains(lr.Id))
                .OrderBy(lr => lr.RoomNumber)
                .ToListAsync();
        }

        public async Task<List<VendorBooth>> GetAvailableVendorBoothsForEventAsync(int eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity is null)
            {
                return new List<VendorBooth>();
            }

            var assignedBoothIds = await _context.VendorAssignments
                .Where(va => va.EventId == eventId)
                .Select(va => va.BoothId)
                .ToListAsync();

            return await _context.VendorBooths
                .Where(b => b.VenueId == eventEntity.VenueId && !assignedBoothIds.Contains(b.Id))
                .OrderBy(b => b.BoothNumber)
                .ToListAsync();
        }

        public async Task<AssignmentResult> AssignTeamToLockerAsync(int teamId, int eventId, int lockerId)
        {
            var conflict = await CheckTeamLockerConflictAsync(eventId, teamId, lockerId);
            if (conflict is not null)
            {
                return AssignmentResult.Failure(conflict.Description);
            }

            _context.TeamAssignments.Add(new TeamAssignment
            {
                TeamId = teamId,
                EventId = eventId,
                LockerId = lockerId
            });

            try
            {
                await _context.SaveChangesAsync();
                return AssignmentResult.Success("Team locker assignment saved.");
            }
            catch (DbUpdateException)
            {
                return AssignmentResult.Failure("The assignment conflicts with an existing locker assignment.");
            }
        }

        public async Task<AssignmentResult> AssignVendorToBoothAsync(int vendorId, int eventId, int boothId)
        {
            var conflict = await CheckVendorBoothConflictAsync(eventId, vendorId, boothId);
            if (conflict is not null)
            {
                return AssignmentResult.Failure(conflict.Description);
            }

            var suppliesAt = await _context.SuppliesAt.FindAsync(vendorId, eventId);
            if (suppliesAt is null)
            {
                _context.SuppliesAt.Add(new SuppliesAt
                {
                    VendorId = vendorId,
                    EventId = eventId
                });
            }

            _context.VendorAssignments.Add(new VendorAssignment
            {
                VendorId = vendorId,
                EventId = eventId,
                BoothId = boothId
            });

            try
            {
                await _context.SaveChangesAsync();
                return AssignmentResult.Success("Vendor booth assignment saved.");
            }
            catch (DbUpdateException)
            {
                return AssignmentResult.Failure("The assignment conflicts with an existing vendor booth assignment.");
            }
        }

        public async Task<bool> RemoveTeamLockerAssignmentAsync(int teamId, int eventId, int lockerId)
        {
            var assignment = await _context.TeamAssignments.FindAsync(teamId, eventId, lockerId);
            if (assignment is null)
            {
                return false;
            }

            _context.TeamAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveVendorBoothAssignmentAsync(int vendorId, int eventId, int boothId)
        {
            var assignment = await _context.VendorAssignments.FindAsync(vendorId, eventId, boothId);
            if (assignment is null)
            {
                return false;
            }

            _context.VendorAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AssignmentConflictDto?> CheckTeamLockerConflictAsync(int eventId, int teamId, int lockerId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity is null)
            {
                return Critical("Missing Event", "Selected event could not be found.", $"Event {eventId}");
            }

            var team = await _context.Teams.FindAsync(teamId);
            if (team is null)
            {
                return Critical("Missing Team", "Selected team could not be found.", $"Team {teamId}");
            }

            var lockerRoom = await _context.LockerRooms.FindAsync(lockerId);
            if (lockerRoom is null)
            {
                return Critical("Missing Locker Room", "Selected locker room could not be found.", $"Locker {lockerId}");
            }

            if (lockerRoom.VenueId != eventEntity.VenueId)
            {
                return Critical(
                    "Venue Mismatch",
                    "The selected locker room belongs to a different venue than this event.",
                    $"Locker {lockerRoom.RoomNumber}");
            }

            var participates = await _context.ParticipatesIn
                .AnyAsync(pi => pi.EventId == eventId && pi.TeamId == teamId);

            if (!participates)
            {
                return Warning(
                    "Team Not In Event",
                    "Assign the team to this event before assigning a locker room.",
                    team.Name);
            }

            var existingTeamAssignment = await _context.TeamAssignments
                .Include(ta => ta.LockerRoom)
                .FirstOrDefaultAsync(ta => ta.EventId == eventId && ta.TeamId == teamId);

            if (existingTeamAssignment is not null)
            {
                return Warning(
                    "Team Already Assigned",
                    $"This team already has locker room {existingTeamAssignment.LockerRoom.RoomNumber} for this event.",
                    team.Name);
            }

            var existingLockerAssignment = await _context.TeamAssignments
                .Include(ta => ta.Team)
                .FirstOrDefaultAsync(ta => ta.EventId == eventId && ta.LockerId == lockerId);

            if (existingLockerAssignment is not null)
            {
                return Warning(
                    "Locker Occupied",
                    $"Locker room {lockerRoom.RoomNumber} is already assigned to {existingLockerAssignment.Team?.Name ?? "another team"}.",
                    $"Locker {lockerRoom.RoomNumber}");
            }

            return null;
        }

        public async Task<AssignmentConflictDto?> CheckVendorBoothConflictAsync(int eventId, int vendorId, int boothId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity is null)
            {
                return Critical("Missing Event", "Selected event could not be found.", $"Event {eventId}");
            }

            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor is null)
            {
                return Critical("Missing Vendor", "Selected vendor could not be found.", $"Vendor {vendorId}");
            }

            var booth = await _context.VendorBooths.FindAsync(boothId);
            if (booth is null)
            {
                return Critical("Missing Booth", "Selected vendor booth could not be found.", $"Booth {boothId}");
            }

            if (booth.VenueId != eventEntity.VenueId)
            {
                return Critical(
                    "Venue Mismatch",
                    "The selected booth belongs to a different venue than this event.",
                    $"Booth {booth.BoothNumber}");
            }

            var existingVendorAssignment = await _context.VendorAssignments
                .Include(va => va.Booth)
                .FirstOrDefaultAsync(va => va.EventId == eventId && va.VendorId == vendorId);

            if (existingVendorAssignment is not null)
            {
                return Warning(
                    "Vendor Already Assigned",
                    $"This vendor already has booth {existingVendorAssignment.Booth?.BoothNumber} for this event.",
                    vendor.Name);
            }

            var existingBoothAssignment = await _context.VendorAssignments
                .Include(va => va.Vendor)
                .FirstOrDefaultAsync(va => va.EventId == eventId && va.BoothId == boothId);

            if (existingBoothAssignment is not null)
            {
                return Warning(
                    "Booth Occupied",
                    $"Booth {booth.BoothNumber} is already assigned to {existingBoothAssignment.Vendor?.Name ?? "another vendor"}.",
                    $"Booth {booth.BoothNumber}");
            }

            return null;
        }

        public async Task<List<AssignmentConflictDto>> GetAssignmentConflictsAsync(int eventId)
        {
            var conflicts = new List<AssignmentConflictDto>();
            var eventEntity = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity is null)
            {
                conflicts.Add(Critical("Missing Event", "Selected event could not be found.", $"Event {eventId}"));
                return conflicts;
            }

            var teamParticipations = await _context.ParticipatesIn
                .Where(pi => pi.EventId == eventId)
                .Include(pi => pi.Team)
                .ToListAsync();
            var teamAssignments = await GetTeamAssignmentsForEventAsync(eventId);

            foreach (var participation in teamParticipations)
            {
                if (!teamAssignments.Any(ta => ta.TeamId == participation.TeamId))
                {
                    conflicts.Add(Warning(
                        "Missing Locker",
                        "This team is participating in the event but does not have a locker room.",
                        participation.Team?.Name ?? $"Team {participation.TeamId}"));
                }
            }

            foreach (var assignment in teamAssignments)
            {
                if (assignment.LockerRoom.VenueId != eventEntity.VenueId)
                {
                    conflicts.Add(Critical(
                        "Locker Venue Mismatch",
                        "A team is assigned to a locker room at a different venue than the event.",
                        assignment.Team?.Name ?? $"Team {assignment.TeamId}"));
                }
            }

            AddDuplicateConflicts(
                conflicts,
                teamAssignments.GroupBy(ta => ta.TeamId).Where(g => g.Count() > 1),
                "Duplicate Team Locker",
                "A team has more than one locker assignment for this event.",
                g => g.First().Team?.Name ?? $"Team {g.Key}");

            AddDuplicateConflicts(
                conflicts,
                teamAssignments.GroupBy(ta => ta.LockerId).Where(g => g.Count() > 1),
                "Duplicate Locker Use",
                "A locker room has more than one team assignment for this event.",
                g => $"Locker {g.First().LockerRoom.RoomNumber}");

            var eventVendors = await _context.SuppliesAt
                .Where(sa => sa.EventId == eventId)
                .Include(sa => sa.Vendor)
                .ToListAsync();
            var vendorAssignments = await GetVendorAssignmentsForEventAsync(eventId);

            foreach (var supplier in eventVendors)
            {
                if (!vendorAssignments.Any(va => va.VendorId == supplier.VendorId))
                {
                    conflicts.Add(Warning(
                        "Missing Booth",
                        "This vendor is attached to the event but does not have a booth assignment.",
                        supplier.Vendor?.Name ?? $"Vendor {supplier.VendorId}"));
                }
            }

            foreach (var assignment in vendorAssignments)
            {
                if (assignment.Booth?.VenueId != eventEntity.VenueId)
                {
                    conflicts.Add(Critical(
                        "Booth Venue Mismatch",
                        "A vendor is assigned to a booth at a different venue than the event.",
                        assignment.Vendor?.Name ?? $"Vendor {assignment.VendorId}"));
                }
            }

            AddDuplicateConflicts(
                conflicts,
                vendorAssignments.GroupBy(va => va.VendorId).Where(g => g.Count() > 1),
                "Duplicate Vendor Booth",
                "A vendor has more than one booth assignment for this event.",
                g => g.First().Vendor?.Name ?? $"Vendor {g.Key}");

            AddDuplicateConflicts(
                conflicts,
                vendorAssignments.GroupBy(va => va.BoothId).Where(g => g.Count() > 1),
                "Duplicate Booth Use",
                "A booth has more than one vendor assignment for this event.",
                g => $"Booth {g.First().Booth?.BoothNumber}");

            return conflicts
                .OrderByDescending(c => c.Severity)
                .ThenBy(c => c.Type)
                .ToList();
        }

        private static AssignmentConflictDto Critical(string type, string description, string affectedEntity)
            => CreateConflict(AssignmentConflictSeverity.Critical, type, description, affectedEntity);

        private static AssignmentConflictDto Warning(string type, string description, string affectedEntity)
            => CreateConflict(AssignmentConflictSeverity.Warning, type, description, affectedEntity);

        private static AssignmentConflictDto CreateConflict(
            AssignmentConflictSeverity severity,
            string type,
            string description,
            string affectedEntity)
        {
            return new AssignmentConflictDto
            {
                Severity = severity,
                Type = type,
                Description = description,
                AffectedEntity = affectedEntity
            };
        }

        private static void AddDuplicateConflicts<TKey, TAssignment>(
            List<AssignmentConflictDto> conflicts,
            IEnumerable<IGrouping<TKey, TAssignment>> groups,
            string type,
            string description,
            Func<IGrouping<TKey, TAssignment>, string> affectedEntity)
        {
            foreach (var group in groups)
            {
                conflicts.Add(Critical(type, description, affectedEntity(group)));
            }
        }
    }
}
