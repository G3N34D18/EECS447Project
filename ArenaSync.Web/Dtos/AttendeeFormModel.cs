// -----------------------------------------------------------------------------
// File: AttendeeFormModel.cs
// Project: ArenaSync.Web
// Purpose: Form DTO used by Batch 2 for creating and editing Attendee records.
// Notes: Separates UI validation from EF Core models to maintain clean Razor
//        Pages. Batch 2 adds attendee registration and event‑level workflows;
//        this DTO supports the core CRUD operations for Attendees.
// -----------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class AttendeeFormModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, StringLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
