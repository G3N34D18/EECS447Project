namespace ArenaSync.Web.Dtos
{
    public class VenueOccupancyReportDto
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int TotalLockerRooms { get; set; }
        public int TotalVendorBooths { get; set; }
        public List<VenueOccupancyEventDto> Events { get; set; } = new();
    }

    public class VenueOccupancyEventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TeamCount { get; set; }
        public int AssignedLockers { get; set; }
        public int VendorCount { get; set; }
        public int AssignedBooths { get; set; }
        public int AttendeeCount { get; set; }
    }
}
