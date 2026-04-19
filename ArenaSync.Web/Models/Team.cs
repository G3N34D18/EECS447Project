using System.ComponentModel.DataAnnotations;
namespace ArenaSync.Web.Models

{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Manager { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Range(1,100)]
        public int PlayerCount { get; set; }
        public List<ParticipatesIn> ParticipatesIn { get; set; } = new();
        public List<TeamAssignments> Assignments { get; set; } = new();
    }
}