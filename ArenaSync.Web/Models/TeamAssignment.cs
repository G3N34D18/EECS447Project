namespace ArenaSync.Web.Models
{
    public class TeamAssignment
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public int LockerId { get; set; }
        public LockerRoom? LockerRoom { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }
    }
}