using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Models
{
    public class Attendee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]