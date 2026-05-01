namespace ArenaSync.Web.Dtos
{
    public class ConflictReportRowDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public AssignmentConflictSeverity Severity { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AffectedEntity { get; set; } = string.Empty;
    }
}
