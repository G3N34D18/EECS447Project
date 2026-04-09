namespace ArenaSync.Web.Models
{
    public class ParticipatesIn
    {
        public int TeamId { get; set; }
        public Team? Team { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}