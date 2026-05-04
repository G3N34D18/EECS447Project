using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Team name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Manager name is required.")]
        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters.")]
        public string Manager { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Player count must be between 1 and 1000.")]
        public int PlayerCount { get; set; }

        public List<ParticipatesIn> ParticipatesIn { get; set; } = new();
        public List<TeamAssignment> Assignments { get; set; } = new();
    }
}