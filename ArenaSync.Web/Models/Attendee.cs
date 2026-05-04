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
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        public List<RegistersFor> Registrations { get; set; } = new();
    }
}