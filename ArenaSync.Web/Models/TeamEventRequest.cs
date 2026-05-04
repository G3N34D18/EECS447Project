namespace ArenaSync.Web.Models
{
    public class TeamEventRequest
    {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public TeamEventRequestType Type { get; set; }

        public int? SourceEventId { get; set; }
        public Event? SourceEvent { get; set; }

        public int? TargetEventId { get; set; }
        public Event? TargetEvent { get; set; }

        public string Reason { get; set; } = string.Empty;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public string ResolutionNote { get; set; } = string.Empty;
    }

    public enum TeamEventRequestType
    {
        Reassignment,
        LateResignation,
        OverlapParticipation
    }
}
