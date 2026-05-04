using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class LockerRoomFormModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Room number must be greater than 0.")]
        public int RoomNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A venue is required.")]
        public int VenueId { get; set; }
    }
}
