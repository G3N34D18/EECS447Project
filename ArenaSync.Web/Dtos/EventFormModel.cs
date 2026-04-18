// -----------------------------------------------------------------------------
// File: EventFormModel.cs
// Project: ArenaSync.Web
// Purpose: Form DTO used by Batch 1 event create/edit pages.
// Notes: Date/time strings are kept separately in Razor pages so native
//        datetime-local inputs can be used cleanly.
// -----------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class EventFormModel
    {
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(200, ErrorMessage = "Event name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event start date is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Event end date is required.")]
        public DateTime EndTime { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a venue.")]
        public int VenueId { get; set; }
    }
}