using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
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
            if (eventEntity.EndTime < eventEntity.StartTime)
            {
                throw new ArgumentException("End time cannot be earlier than start time.");
            }

            var venueExists = await _context.Venues.AnyAsync(v => v.Id == eventEntity.VenueId);
            if (!venueExists)
            {
                throw new ArgumentException("Selected venue does not exist.");
            }

            var existingEvent = await _context.Events.FindAsync(eventEntity.Id);
            if (existingEvent == null)
            {
                return null;
            }

            existingEvent.Name = eventEntity.Name;
            existingEvent.StartTime = eventEntity.StartTime;
            existingEvent.EndTime = eventEntity.EndTime;
            existingEvent.Description = eventEntity.Description;
            existingEvent.VenueId = eventEntity.VenueId;

            await _context.SaveChangesAsync();
            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return false;
            }

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}