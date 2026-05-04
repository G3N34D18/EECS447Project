namespace ArenaSync.Web.Dtos
{
    public enum AssignmentConflictSeverity
    {
        Info,
        Warning,
        Critical
    }

    public class AssignmentConflictDto
    {
        public AssignmentConflictSeverity Severity { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AffectedEntity { get; set; } = string.Empty;
    }
}
