using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<bool> CreateTeamAsync(Team team);
        Task<Team> UpdateTeamAsync(Team team);
        Task<bool> DeleteTeamAsync(int id);

        // Event assignment
        Task<bool> AssignTeamToEventAsync(int teamId, int eventId);
        Task<List<Team>> GetTeamsForEventAsync(int eventId);
    }
}
