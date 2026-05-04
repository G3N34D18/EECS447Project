using ArenaSync.Web.Dtos;

namespace ArenaSync.Web.Services
{
    public interface IReportingService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<EventRosterDto?> GetEventRosterAsync(int eventId);
        Task<TeamScheduleReportDto?> GetTeamScheduleAsync(int teamId);
        Task<VenueOccupancyReportDto?> GetVenueOccupancyAsync(int venueId);
        Task<List<ConflictReportRowDto>> GetConflictSummaryAsync(int? eventId = null);
        Task<string> ExportReportToCsvAsync(ReportType type, int? entityId = null);
    }
}
