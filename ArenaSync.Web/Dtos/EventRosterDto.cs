namespace ArenaSync.Web.Dtos
{
    public class EventRosterDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<EventRosterTeamDto> Teams { get; set; } = new();
        public List<EventRosterAttendeeDto> Attendees { get; set; } = new();
        public List<EventRosterVendorDto> Vendors { get; set; } = new();
    }

    public class EventRosterTeamDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public string LockerRoom { get; set; } = "Unassigned";
    }

    public class EventRosterAttendeeDto
    {
        public int AttendeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class EventRosterVendorDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Booth { get; set; } = "Unassigned";
    }
}
