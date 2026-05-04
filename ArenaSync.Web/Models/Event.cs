using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A venue must be selected.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid venue must be selected.")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        public List<TeamAssignment> Assignments { get; set; } = new();
        public List<VendorAssignment> VendorAssignments { get; set; } = new(); 
        public List<RegistersFor> Registrations { get; set; } = new();  
        public List<ParticipatesIn> Participants { get; set; } = new();
        public List<SuppliesAt> Suppliers { get; set; } = new();
    }
}