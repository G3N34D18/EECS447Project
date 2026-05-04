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

        [Require