using ArenaSync.Web.Dtos;
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
        Task<List<Event>> GetOverlappingEventsForTeamAsync(int teamId, int targetEventId, int? excludedEventId = null);
        Task<List<Team>> GetTeamsForEventAsync(int eventId);
        Task<AssignmentResult> ResignTeamFromEventAsync(int teamId, int eventId);
        Task<AssignmentResult> SubmitReassignmentRequestAsync(int teamId, int sourceEventId, int targetEventId, string reason);
        Task<AssignmentResult> SubmitLateResignationRequestAsync(int teamId, int sourceEventId, string reason);
        Task<AssignmentResult> SubmitOverlapParticipationRequestAsync(int teamId, int targetEventId, string reason);
        Task<List<TeamEventRequest>> GetPendingRequestsForTeamAsync(int teamId);
        Task<List<TeamEventRequest>> GetPendingTeamEventRequestsAsync();
        Task<AssignmentResult> ApproveTeamEventRequestAsync(int requestId);
        Task<AssignmentResult> DenyTeamEventRequestAsync(int requestId);
    }
}
