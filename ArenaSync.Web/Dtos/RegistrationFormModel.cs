using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class RegistrationFormModel
    {
        [Required]
        public int AttendeeId { get; set; }

        [Required(ErrorMessage = "Please select an event.")]
        public int EventId { get; set; }
    }
}
