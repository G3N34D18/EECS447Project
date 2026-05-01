using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class ParticipationService : IParticipationService
    {
        private readonly ApplicationDbContext _context;

        public ParticipationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Assign team to event via ParticipatesIn.
        public async Task<bool> AssignTeamToEventAsync(int teamId, int eventId)
        {
            // Already assigned to this event
            bool alreadyAssigned = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == teamId && p.EventId == eventId);
            if (alreadyAssigned) return false;

            _context.ParticipatesIn.Add(new ParticipatesIn
            {
                TeamId = teamId,
                EventId = eventId
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // Return only events this team isn't already participating in.
        public async Task<List<Event>> GetAvailableEventsForTeamAsync(int teamId)
        {
            var assignedEventIds = await _context.ParticipatesIn
                .Where(p => p.TeamId == teamId)
                .Select(p => p.EventId)
                .ToHashSetAsync();

            return await _context.Events
                .Where(e => !assignedEventIds.Contains(e.Id))
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }
    }
}
