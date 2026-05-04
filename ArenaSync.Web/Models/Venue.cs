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
        [StringLength(500, ErrorMessage = "Address cann