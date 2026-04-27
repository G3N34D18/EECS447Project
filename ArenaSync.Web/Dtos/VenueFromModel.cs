// -----------------------------------------------------------------------------
// File: VenueFormModel.cs
// Project: ArenaSync.Web
// Purpose: Form DTO used by Batch 1 venue create/edit pages.
// Notes: Keeps Razor pages clean and isolates validation rules from EF entities.
// -----------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class VenueFormModel
    {
        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(200, ErrorMessage = "Venue name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
        public string Address { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }
    }
}
