namespace ArenaSync.Web.Models
{
    public class TeamReassignmentRequest
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;
        public int RequestedEventId { get; set; }
        public Event RequestedEvent { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    public enum RequestStatus { Pending, Approved, Denied }
}