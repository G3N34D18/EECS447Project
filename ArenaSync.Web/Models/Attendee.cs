using System.ComponentModel.DataAnnotations;
namespace ArenaSync.Web.Models
{ 
    public class Attendee
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        public List<RegistersFor> Registrations { get; set; } = new();
    }
}