using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArenaSync.Web.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventService> _logger;

        public EventService(ApplicationDbContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Venue)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateEventAsync(Event eventEntity)
        {
            if (eventEntity.EndTime < eventEntity.StartTime)
            {
                throw new ArgumentException("End time cannot be earlier than start time.");
            }

            var venueExists = await _context.Venues.AnyAsync(v => v.Id == eventEntity.VenueId);
            if (!venueExists)
            {
                throw new ArgumentException("Selected venue does not exist.");
            }

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<Event?> UpdateEventAsync(Event eventEntity)
        {
            // Fixed variable name to eventEntity
            _logger.LogInformation(
                "UpdateEventAsync called. Id = {Id}, StartTime = {StartTime:o}, EndTime = {EndTime:o}, VenueId = {VenueId}, Name = {Name}",
                eventEntity.Id, eventEntity.StartTime, eventEntity.EndTime, eventEntity.VenueId, eventEntity.Name);

            if (eventEntity.EndTime < eventEntity.StartTime)
            {
                _logger.LogWarning("Update validation failed for Id {Id}: End time is before start time.", eventEntity.Id);
                throw new ArgumentException("End time cannot be earlier than start time.");
            }

            var venueExists = await _context.Venues.AnyAsync(v => v.Id == eventEntity.VenueId);
            if (!venueExists)
            {
                _logger.LogWarning("Update validation failed for Id {Id}: Venue {VenueId} does not exist.", eventEntity.Id, eventEntity.VenueId);
                throw new ArgumentException("Selected venue does not exist.");
            }

            var existingEvent = await _context.Events.FindAsync(eventEntity.Id);

            // Fixed logic: Check if existingEvent is null
            _logger.LogInformation("Existing event lookup result for Id = {Id}: {Found}", eventEntity.Id, existingEvent != null);

            if (existingEvent == null)
            {
                return null;
            }

            // Map values
            existingEvent.Name = eventEntity.Name;
            existingEvent.StartTime = eventEntity.StartTime;
            existingEvent.EndTime = eventEntity.EndTime;
            existingEvent.Description = eventEntity.Description;
            existingEvent.VenueId = eventEntity.VenueId;

            _logger.LogInformation(
                "Saving updated event Id = {Id}. New StartTime = {StartTime:o}, New EndTime = {EndTime:o}",
                existingEvent.Id, existingEvent.StartTime, existingEvent.EndTime);

            await _context.SaveChangesAsync();

            _logger.LogInformation("SaveChangesAsync completed for Id = {Id}", existingEvent.Id);

            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return false;
            }

            // Remove all child records that reference this event before deleting it
            var participations = await _context.ParticipatesIn.Where(p => p.EventId == id).ToListAsync();
            _context.ParticipatesIn.RemoveRange(participations);

            var registrations = await _context.RegistersFor.Where(r => r.EventId == id).ToListAsync();
            _context.RegistersFor.RemoveRange(registrations);

            var teamAssignments = await _context.TeamAssignments.Where(a => a.EventId == id).ToListAsync();
            _context.TeamAssignments.RemoveRange(teamAssignments);

            var vendorAssignments = await _context.VendorAssignments.Where(a => a.EventId == id).ToListAsync();
            _context.VendorAssignments.RemoveRange(vendorAssignments);

            var suppliers = await _context.SuppliesAt.Where(s => s.EventId == id).ToListAsync();
            _context.SuppliesAt.RemoveRange(suppliers);

            var teamEventRequests = await _context.TeamEventRequests
                .Where(r => r.SourceEventId == id || r.TargetEventId == id)
                .ToListAsync();
            _context.TeamEventRequests.RemoveRange(teamEventRequests);

            var teamReassignmentRequests = await _context.TeamReassignmentRequests
                .Where(r => r.RequestedEventId == id)
                .ToListAsync();
            _context.TeamReassignmentRequests.RemoveRange(teamReassignmentRequests);

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
