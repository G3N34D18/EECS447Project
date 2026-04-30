using ArenaSync.Web.Models;

namespace ArenaSync.Web.Dtos
{
    public class DashboardSummaryDto
    {
        public int TotalEvents { get; set; }
        public int TotalTeams { get; set; }
        public int TotalVendors { get; set; }
        public int TotalAttendees { get; set; }
        public int UpcomingEvents { get; set; }
        public int PendingTeamAssignments { get; set; }
        public int PendingVendorAssignments { get; set; }
        public int PendingAssignments => PendingTeamAssignments + PendingVendorAssignments;
        public int PendingTeamEventRequests { get; set; }
        public int CriticalConflicts { get; set; }
        public int WarningConflicts { get; set; }
        public List<DashboardEventDto> RecentEvents { get; set; } = new();
        public List<ConflictReportRowDto> CriticalAlerts { get; set; } = new();
    }

    public class DashboardEventDto
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TeamCount { get; set; }
        public int AttendeeCount { get; set; }
        public int VendorCount { get; set; }
    }
}
