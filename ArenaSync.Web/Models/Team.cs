namespace ArenaSync.Web.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public List<ParticipatesIn> ParticipatesIn { get; set; } = new();
        public List<TeamAssignments> Assignments { get; set; } = new();
    }
}