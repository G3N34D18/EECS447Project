namespace ArenaSync.Web.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }= string.Empty;
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        public List<TeamAssignments> Assignments { get; set; } = new(); 
    }
}