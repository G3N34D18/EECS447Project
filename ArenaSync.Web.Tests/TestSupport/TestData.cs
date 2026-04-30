using ArenaSync.Web.Data;
using ArenaSync.Web.Models;

namespace ArenaSync.Web.Tests.TestSupport;

public static class TestData
{
    public static async Task SeedCoreAsync(ApplicationDbContext context)
    {
        var venue = new Venue
        {
            Id = 1,
            Name = "Main Arena",
            Address = "100 Arena Way",
            Capacity = 10000
        };

        var secondVenue = new Venue
        {
            Id = 2,
            Name = "Practice Facility",
            Address = "200 Court Lane",
            Capacity = 2000
        };

        var now = DateTime.Now;
        var events = new[]
        {
            new Event
            {
                Id = 1,
                Name = "Morning Classic",
                VenueId = 1,
                StartTime = now.AddDays(10).Date.AddHours(9),
                EndTime = now.AddDays(10).Date.AddHours(11),
                Description = "Morning event"
            },
            new Event
            {
                Id = 2,
                Name = "Afternoon Showcase",
                VenueId = 1,
                StartTime = now.AddDays(10).Date.AddHours(14),
                EndTime = now.AddDays(10).Date.AddHours(16),
                Description = "Afternoon event"
            },
            new Event
            {
                Id = 3,
                Name = "Overlapping Invitational",
                VenueId = 1,
                StartTime = now.AddDays(10).Date.AddHours(10),
                EndTime = now.AddDays(10).Date.AddHours(12),
                Description = "Overlapping event"
            },
            new Event
            {
                Id = 4,
                Name = "Late Window Event",
                VenueId = 1,
                StartTime = now.AddHours(24),
                EndTime = now.AddHours(26),
                Description = "Event inside resignation cutoff"
            },
            new Event
            {
                Id = 5,
                Name = "Other Venue Event",
                VenueId = 2,
                StartTime = now.AddDays(11).Date.AddHours(9),
                EndTime = now.AddDays(11).Date.AddHours(11),
                Description = "Other venue event"
            }
        };

        var teams = new[]
        {
            new Team { Id = 1, Name = "Falcons", Manager = "Avery", Email = "falcons@example.com", Phone = "555-0101", PlayerCount = 12 },
            new Team { Id = 2, Name = "Rockets", Manager = "Blair", Email = "rockets@example.com", Phone = "555-0102", PlayerCount = 14 },
            new Team { Id = 3, Name = "Tigers", Manager = "Casey", Email = "tigers@example.com", Phone = "555-0103", PlayerCount = 16 }
        };

        var vendors = new[]
        {
            new Vendor { Id = 1, Name = "Snack Stand", Type = "Food", Location = "North Concourse", Phone = "555-0201" },
            new Vendor { Id = 2, Name = "Merch Hub", Type = "Retail", Location = "Gate A", Phone = "555-0202" }
        };

        var lockers = new[]
        {
            new LockerRoom { Id = 1, RoomNumber = 101, VenueId = 1 },
            new LockerRoom { Id = 2, RoomNumber = 102, VenueId = 1 },
            new LockerRoom { Id = 3, RoomNumber = 201, VenueId = 2 }
        };

        var booths = new[]
        {
            new VendorBooth { Id = 1, BoothNumber = 10, VenueId = 1 },
            new VendorBooth { Id = 2, BoothNumber = 11, VenueId = 1 },
            new VendorBooth { Id = 3, BoothNumber = 20, VenueId = 2 }
        };

        context.Venues.AddRange(venue, secondVenue);
        context.Events.AddRange(events);
        context.Teams.AddRange(teams);
        context.Vendors.AddRange(vendors);
        context.LockerRooms.AddRange(lockers);
        context.VendorBooths.AddRange(booths);

        await context.SaveChangesAsync();
    }
}
