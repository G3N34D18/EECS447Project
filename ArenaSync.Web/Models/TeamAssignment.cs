namespace ArenaSync.Web.Models
{
    public class TeamAssignment
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public int LockerId { get; set; }
        public LockerRoom LockerRoom { get; set; } = null!;
        public int TeamId { get; set; }
        public Team? Team { get; set; } = null;
    }
}