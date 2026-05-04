using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string Address { get; set; } = string.Empty;

        [Range(1, 1000000, ErrorMessage = "Capacity must be between 1 and 1,000,000.")]
        public int Capacity { get; set; }
        public List<LockerRoom> LockerRooms { get; set; } = new();
        public List<VendorBooth> VendorBooths { get; set; } = new();
        public List<Event> Events { get; set; } = new();
    }
}
