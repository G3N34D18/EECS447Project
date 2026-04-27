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
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int VenueId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(1, 12)]
        public int StartHour { get; set; } = 7;

        [Range(0, 59)]
        public int StartMinute { get; set; } = 0;

        [Required]
        public string StartMeridiem { get; set; } = "PM";

        [Required]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(1, 12)]
        public int EndHour { get; set; } = 9;

        [Range(0, 59)]
        public int EndMinute { get; set; } = 0;

        [Required]
        public string EndMeridiem { get; set; } = "PM";
    }
}