namespace ArenaSync.Web.Models
{
    public class LockerRoom
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }
        public List<TeamAssignments> TeamAssignments { get; set; } = new();
    }
}