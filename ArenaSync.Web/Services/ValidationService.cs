// -----------------------------------------------------------------------------
// File: ValidationService.cs
// Project: ArenaSync.Web
// Purpose: Batch 4 — Business-rule validation that goes beyond data annotations.
//          Returns a list of human-readable error strings; an empty list means
//          the operation is valid.
// -----------------------------------------------------------------------------

using ArenaSync.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(ApplicationDbContext context, ILogger<ValidationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ── Team assignment ───────────────────────────────────────────────────

        public async Task<List<string>> ValidateTeamAssignmentAsync(int teamId, int eventId, int lockerId)
        {
            var errors = new List<string>();

            var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId);
            if (!teamExists)
                errors.Add($"Team with ID {teamId} does not exist.");

            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity is null)
            {
                errors.Add($"Event with ID {eventId} does not exist.");
                return errors; // no point continuing
            }

            var lockerExists = await _context.LockerRooms.AnyAsync(l => l.Id == lockerId);
            if (!lockerExists)
                errors.Add($"Locker room with ID {lockerId} does not exist.");

            // Check for duplicate team assignment to the same event
            var alreadyAssigned = await _context.TeamAssignments
                .AnyAsync(a => a.TeamId == teamId && a.EventId == eventId);
            if (alreadyAssigned)
                errors.Add("This team is already assigned to the selected event.");

            // Check if the locker room is already taken for this event
            var lockerTaken = await _context.TeamAssignments
                .AnyAsync(a => a.LockerId == lockerId && a.EventId == eventId);
            if (lockerTaken)
                errors.Add("The selected locker room is already assigned to another team for this event.");

            return errors;
        }

        // ── Vendor booth assignment ───────────────────────────────────────────

        public async Task<List<string>> ValidateVendorBoothAssignmentAsync(int vendorId, int boothId, int eventId)
        {
            var errors = new List<string>();

            var vendorExists = await _context.Vendors.AnyAsync(v => v.Id == vendorId);
            if (!vendorExists)
                errors.Add($"Vendor with ID {vendorId} does not exist.");

            var boothExists = await _context.VendorBooths.AnyAsync(b => b.Id == boothId);
            if (!boothExists)
                errors.Add($"Vendor booth with ID {boothId} does not exist.");

            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
            {
                errors.Add($"Event with ID {eventId} does not exist.");
                return errors;
            }

            // Vendor already assigned to this event
            var vendorAlreadyAssigned = await _context.VendorAssignments
                .AnyAsync(a => a.VendorId == vendorId && a.EventId == eventId);
            if (vendorAlreadyAssigned)
                errors.Add("This vendor is already assigned to the selected event.");

            // Booth already assigned for this event
            var boothTaken = await _context.VendorAssignments
                .AnyAsync(a => a.BoothId == boothId && a.EventId == eventId);
            if (boothTaken)
                errors.Add("The selected booth is already assigned to another vendor for this event.");

            return errors;
        }

        // ── Attendee registration ─────────────────────────────────────────────

        public async Task<List<string>> ValidateAttendeeRegistrationAsync(int attendeeId, int eventId)
        {
            var errors = new List<string>();

            var attendeeExists = await _context.Attendees.AnyAsync(a => a.Id == attendeeId);
            if (!attendeeExists)
                errors.Add($"Attendee with ID {attendeeId} does not exist.");

            var eventEntity = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity is null)
            {
                errors.Add($"Event with ID {eventId} does not exist.");
                return errors;
            }

            // Duplicate registration check
            var alreadyRegistered = await _context.RegistersFor
                .AnyAsync(r => r.AttendeeId == attendeeId && r.EventId == eventId);
            if (alreadyRegistered)
                errors.Add("This attendee is already registered for the selected event.");

            // Venue capacity check
            if (eventEntity.Venue is not null)
            {
                int currentCount = eventEntity.Registrations.Count;
                if (currentCount >= eventEntity.Venue.Capacity)
                    errors.Add($"This event has reached its venue capacity of {eventEntity.Venue.Capacity} attendees.");
            }

            return errors;
        }

        // ── Event date validation ─────────────────────────────────────────────

        public async Task<List<string>> ValidateEventDatesAsync(
            int venueId, DateTime startTime, DateTime endTime, int? excludeEventId = null)
        {
            var errors = new List<string>();

            if (endTime <= startTime)
            {
                errors.Add("End time must be after start time.");
                return errors; // no point checking overlap with invalid times
            }

            if (startTime < DateTime.Now.AddMinutes(-5))
                errors.Add("Start time cannot be in the past.");

            // Check for overlapping events at the same venue
            var query = _context.Events
                .Where(e => e.VenueId == venueId
                         && e.StartTime < endTime
                         && e.EndTime > startTime);

            if (excludeEventId.HasValue)
                query = query.Where(e => e.Id != excludeEventId.Value);

            var overlap = await query.Select(e => e.Name).FirstOrDefaultAsync();
            if (overlap is not null)
                errors.Add($"This venue is already booked during that time by \"{overlap}\".");

            return errors;
        }
    }
}
