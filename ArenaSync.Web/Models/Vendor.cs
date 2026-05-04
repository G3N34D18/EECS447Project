using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vendor name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vendor type is required.")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters.")]
        public string Type { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters.")]
        public string Location { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;
        public List<SuppliesAt> SuppliesAt { get; set; } = new();
        public List<VendorAssignment> VendorAssignments { get; set; } = new();

    }
}
