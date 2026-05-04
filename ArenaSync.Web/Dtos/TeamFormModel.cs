// -----------------------------------------------------------------------------
// File: TeamFormModel.cs
// Project: ArenaSync.Web
// Purpose: Form DTO used by Batch 2 for creating and editing Team records.
// Notes: Keeps Razor Pages clean by isolating validation rules from EF entities.
//        Batch 2 introduces team management, event participation, and assignment
//        workflows; this DTO supports the core CRUD operations for Teams.
// -----------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class TeamFormModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Manager { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Range(1, 100)]
        public int PlayerCount { get; set; }
    }
}
