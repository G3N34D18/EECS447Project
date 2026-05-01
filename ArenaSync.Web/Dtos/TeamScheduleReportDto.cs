namespace ArenaSync.Web.Dtos
{
    public class TeamScheduleReportDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public List<TeamScheduleEventDto> Events { get; set; } = new();
    }

    public class TeamScheduleEventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LockerRoom { get; set; } = "Unassigned";
    }
}
