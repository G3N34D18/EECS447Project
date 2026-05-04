using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IParticipationService
    {
        Task<bool> AssignTeamToEventAsync(int teamId, int eventId);
        Task<List<Event>> GetAvailableEventsForTeamAsync(int teamId);
    }
}