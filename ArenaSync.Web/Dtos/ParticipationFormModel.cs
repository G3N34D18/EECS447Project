using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class ParticipationFormModel
    {
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Please select an event.")]
        public int? EventId { get; set; }
    }
}