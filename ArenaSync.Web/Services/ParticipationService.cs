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

        // Assign team to event via ParticipatesIn, enforcing max 2 teams per event
        public async Task<bool> AssignTeamToEventAsync(int teamId, int eventId)
        {
            // Already assigned to this event
            bool alreadyAssigned = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == teamId && p.EventId == eventId);
            if (alreadyAssigned) return false;

            // Event already has 2 teams
            int teamCount = await _context.ParticipatesIn
                .CountAsync(p => p.EventId == eventId);
            if (teamCount >= 2) return false;

            _context.ParticipatesIn.Add(new ParticipatesIn
            {
                TeamId = teamId,
                EventId = eventId
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // Return only events this team isn't already participating in
        // and that still have room for another team (< 2 assigned)
        public async Task<List<Event>> GetAvailableEventsForTeamAsync(int teamId)
        {
            var assignedEventIds = await _context.ParticipatesIn
                .Where(p => p.TeamId == teamId)
                .Select(p => p.EventId)
                .ToHashSetAsync();

            var fullEventIds = await _context.ParticipatesIn
                .GroupBy(p => p.EventId)
                .Where(g => g.Count() >= 2)
                .Select(g => g.Key)
                .ToHashSetAsync();

            return await _context.Events
                .Where(e => !assignedEventIds.Contains(e.Id) && !fullEventIds.Contains(e.Id))
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }
    }
}