using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models; // adjust namespace if Event lives elsewhere

namespace ArenaSync.Web.Helpers
{
    public static class EventFormHelpers
    {
        public static DateTime CombineDateTime(DateOnly date, int hour12, int minute, string meridiem)
        {
            var hour24 = hour12 % 12;
            if (string.Equals(meridiem, "PM", StringComparison.OrdinalIgnoreCase))
            {
                hour24 += 12;
            }

            return date.ToDateTime(new TimeOnly(hour24, minute));
        }

        public static void PopulateFormFromEvent(EventFormModel form, Event ev)
        {
            form.Name = ev.Name;
            form.Description = ev.Description ?? string.Empty;
            form.VenueId = ev.VenueId;

            form.StartDate = DateOnly.FromDateTime(ev.StartTime);
            form.StartHour = ev.StartTime.Hour % 12 == 0 ? 12 : ev.StartTime.Hour % 12;
            form.StartMinute = ev.StartTime.Minute;
            form.StartMeridiem = ev.StartTime.Hour >= 12 ? "PM" : "AM";

            form.EndDate = DateOnly.FromDateTime(ev.EndTime);
            form.EndHour = ev.EndTime.Hour % 12 == 0 ? 12 : ev.EndTime.Hour % 12;
            form.EndMinute = ev.EndTime.Minute;
            form.EndMeridiem = ev.EndTime.Hour >= 12 ? "PM" : "AM";
        }
    }
}