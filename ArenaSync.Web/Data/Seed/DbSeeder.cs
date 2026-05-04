// -----------------------------------------------------------------------------
// File: DbSeeder.cs
// Project: ArenaSync.Web
// Purpose: Batch 4 — Complete database seeding with test users (Admin / Manager /
//          Viewer roles), venues, events, teams, attendees, vendors, locker
//          rooms, booths, and sample assignments.
//          Called on startup in Development environment only.
// NOTE: Never sets explicit Id values — all primary keys are SQL Server IDENTITY
//       and are looked up dynamically by name after each insert.
// -----------------------------------------------------------------------------

using ArenaSync.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Data
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DbSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Apply pending migrations
                await _context.Database.MigrateAsync();

                await SeedRolesAsync();
                await SeedUsersAsync();
                await SeedVenuesAsync();
                await SeedEventsAsync();
                await SeedLockerRoomsAsync();
                await SeedVendorBoothsAsync();
                await SeedTeamsAsync();
                await SeedAttendeesAsync();
                await SeedVendorsAsync();
                await SeedTeamAssignmentsAsync();
                await SeedVendorAssignmentsAsync();
                await SeedParticipatesInAsync();
                await SeedRegistersForAsync();

                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        // ── Roles ─────────────────────────────────────────────────────────────

        private async Task SeedRolesAsync()
        {
            string[] roles = ["Admin", "Manager", "Viewer"];
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Created role: {Role}", role);
                }
            }
        }

        // ── Users ─────────────────────────────────────────────────────────────

        private async Task SeedUsersAsync()
        {
            var users = new[]
            {
                (Email: "admin@arenasync.com",    Password: "Admin123!", Role: "Admin"),
                (Email: "manager@arenasync.com",  Password: "Manager123!", Role: "Manager"),
                (Email: "viewer@arenasync.com",   Password: "Viewer123!", Role: "Viewer"),
            };

            foreach (var (email, password, role) in users)
            {
                if (await _userManager.FindByEmailAsync(email) is null)
                {
                    var user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        _logger.LogInformation("Seeded user {Email} with role {Role}.", email, role);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to seed user {Email}: {Errors}",
                            email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        // ── Venues ────────────────────────────────────────────────────────────

        private async Task SeedVenuesAsync()
        {
            if (await _context.Venues.AnyAsync()) return;

            // Do NOT set Id — SQL Server IDENTITY generates it
            var venues = new List<Venue>
            {
                new() { Name = "Allen Fieldhouse",   Address = "1651 Naismith Dr, Lawrence, KS 66045",   Capacity = 16300 },
                new() { Name = "T-Mobile Center",    Address = "1407 Grand Blvd, Kansas City, MO 64106",  Capacity = 19500 },
                new() { Name = "Intrust Bank Arena", Address = "500 E Waterman St, Wichita, KS 67202",   Capacity = 15000 },
            };

            _context.Venues.AddRange(venues);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} venues.", venues.Count);
        }

        // ── Events ────────────────────────────────────────────────────────────

        private async Task SeedEventsAsync()
        {
            if (await _context.Events.AnyAsync()) return;

            // Look up venue IDs by name — they were generated by SQL Server IDENTITY
            int? VenueId(string name) =>
                _context.Venues
                    .Where(v => v.Name == name)
                    .Select(v => (int?)v.Id)
                    .FirstOrDefault();

            var allen    = VenueId("Allen Fieldhouse");
            var tmobile  = VenueId("T-Mobile Center");
            var intrust  = VenueId("Intrust Bank Arena");

            if (allen is null || tmobile is null || intrust is null)
            {
                _logger.LogWarning("Skipping event seeding — required venues not found.");
                return;
            }

            var base_ = DateTime.Now.Date.AddDays(14); // two weeks from now

            // Do NOT set Id — SQL Server IDENTITY generates it
            var events = new List<Event>
            {
                new() { Name = "Spring Invitational",   VenueId = allen.Value,   StartTime = base_.AddDays(0).AddHours(9),   EndTime = base_.AddDays(0).AddHours(17),  Description = "Annual spring tournament." },
                new() { Name = "Regional Championship", VenueId = tmobile.Value, StartTime = base_.AddDays(2).AddHours(10),  EndTime = base_.AddDays(2).AddHours(18),  Description = "Regional finals event." },
                new() { Name = "Youth League Showcase", VenueId = intrust.Value, StartTime = base_.AddDays(5).AddHours(8),   EndTime = base_.AddDays(5).AddHours(14),  Description = "Youth team showcase day." },
                new() { Name = "Pro Exhibition Match",  VenueId = allen.Value,   StartTime = base_.AddDays(7).AddHours(19),  EndTime = base_.AddDays(7).AddHours(22),  Description = "Pro-level exhibition game." },
                new() { Name = "End-of-Season Gala",    VenueId = tmobile.Value, StartTime = base_.AddDays(10).AddHours(18), EndTime = base_.AddDays(10).AddHours(23), Description = "Closing ceremony and awards." },
            };

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} events.", events.Count);
        }

        // ── Locker Rooms ──────────────────────────────────────────────────────

        private async Task SeedLockerRoomsAsync()
        {
            if (await _context.LockerRooms.AnyAsync()) return;

            // Look up venue IDs by name — actual IDs are IDENTITY-generated by SQL Server
            var venueIds = await _context.Venues
                .OrderBy(v => v.Id)
                .Select(v => v.Id)
                .ToListAsync();

            if (venueIds.Count == 0)
            {
                _logger.LogWarning("Skipping locker room seeding — no venues found.");
                return;
            }

            // Do NOT set Id — SQL Server IDENTITY generates it
            var rooms = new List<LockerRoom>();
            foreach (var venueId in venueIds)
                for (int roomNum = 1; roomNum <= 4; roomNum++)
                    rooms.Add(new LockerRoom { RoomNumber = roomNum, VenueId = venueId });

            _context.LockerRooms.AddRange(rooms);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} locker rooms.", rooms.Count);
        }

        // ── Vendor Booths ─────────────────────────────────────────────────────

        private async Task SeedVendorBoothsAsync()
        {
            if (await _context.VendorBooths.AnyAsync()) return;

            // Look up venue IDs by name — actual IDs are IDENTITY-generated by SQL Server
            var venueIds = await _context.Venues
                .OrderBy(v => v.Id)
                .Select(v => v.Id)
                .ToListAsync();

            if (venueIds.Count == 0)
            {
                _logger.LogWarning("Skipping vendor booth seeding — no venues found.");
                return;
            }

            // Do NOT set Id — SQL Server IDENTITY generates it
            var booths = new List<VendorBooth>();
            foreach (var venueId in venueIds)
                for (int boothNum = 1; boothNum <= 6; boothNum++)
                    booths.Add(new VendorBooth { BoothNumber = boothNum, VenueId = venueId });

            _context.VendorBooths.AddRange(booths);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} vendor booths.", booths.Count);
        }

        // ── Teams ─────────────────────────────────────────────────────────────

        private async Task SeedTeamsAsync()
        {
            if (await _context.Teams.AnyAsync()) return;

            // Do NOT set Id — SQL Server IDENTITY generates it
            var teams = new List<Team>
            {
                new() { Name = "Jayhawks",    Manager = "Alice Johnson",  Email = "jayhawks@team.com",   Phone = "785-555-1101", PlayerCount = 20 },
                new() { Name = "Wildcats",    Manager = "Bob Smith",      Email = "wildcats@team.com",   Phone = "785-555-1102", PlayerCount = 18 },
                new() { Name = "Shockers",    Manager = "Carla Davis",    Email = "shockers@team.com",   Phone = "785-555-1103", PlayerCount = 22 },
                new() { Name = "Gorillas",    Manager = "David Lee",      Email = "gorillas@team.com",   Phone = "785-555-1104", PlayerCount = 19 },
                new() { Name = "Hornets",     Manager = "Eve Martinez",   Email = "hornets@team.com",    Phone = "785-555-1105", PlayerCount = 21 },
                new() { Name = "Chiefs",      Manager = "Frank Wilson",   Email = "chiefs@team.com",     Phone = "785-555-1201", PlayerCount = 20 },
                new() { Name = "Royals",      Manager = "Grace Taylor",   Email = "royals@team.com",     Phone = "785-555-1202", PlayerCount = 17 },
                new() { Name = "Sporting KC", Manager = "Henry Brown",    Email = "sportingkc@team.com", Phone = "785-555-1203", PlayerCount = 23 },
                new() { Name = "Current",     Manager = "Ivy Anderson",   Email = "current@team.com",    Phone = "785-555-1204", PlayerCount = 20 },
                new() { Name = "Mavericks",   Manager = "Jack Thompson",  Email = "mavericks@team.com",  Phone = "785-555-1205", PlayerCount = 18 },
            };

            _context.Teams.AddRange(teams);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} teams.", teams.Count);
        }

        // ── Attendees ─────────────────────────────────────────────────────────

        private async Task SeedAttendeesAsync()
        {
            if (await _context.Attendees.AnyAsync()) return;

            var firstNames = new[] { "Emma","Liam","Olivia","Noah","Ava","William","Sophia","James","Isabella","Oliver",
                                     "Mia","Benjamin","Charlotte","Elijah","Amelia","Lucas","Harper","Mason","Evelyn","Logan" };
            var lastNames  = new[] { "Smith","Johnson","Williams","Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez",
                                     "Hernandez","Lopez","Gonzalez","Wilson","Anderson","Thomas","Taylor","Moore","Jackson","Martin" };

            // Do NOT set Id — SQL Server IDENTITY generates it
            var attendees = Enumerable.Range(0, 20).Select(i =>
                new Attendee
                {
                    Name  = $"{firstNames[i]} {lastNames[i]}",
                    Email = $"{firstNames[i].ToLower()}.{lastNames[i].ToLower()}@mail.com",
                    Phone = $"785-555-{2000 + i:D4}"
                }
            ).ToList();

            _context.Attendees.AddRange(attendees);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} attendees.", attendees.Count);
        }

        // ── Vendors ───────────────────────────────────────────────────────────

        private async Task SeedVendorsAsync()
        {
            if (await _context.Vendors.AnyAsync()) return;

            // Do NOT set Id — SQL Server IDENTITY generates it
            var vendors = new List<Vendor>
            {
                new() { Name = "Arena Eats",       Type = "Food",        Location = "Concourse A", Phone = "913-555-3001" },
                new() { Name = "Sports Gear Co.",  Type = "Merchandise", Location = "Main Lobby",  Phone = "913-555-3002" },
                new() { Name = "Hydration Plus",   Type = "Beverage",    Location = "Concourse B", Phone = "913-555-3003" },
                new() { Name = "Fan Zone Apparel", Type = "Merchandise", Location = "Gate 3",      Phone = "913-555-3004" },
                new() { Name = "KC Pretzels",      Type = "Food",        Location = "Concourse C", Phone = "913-555-3005" },
            };

            _context.Vendors.AddRange(vendors);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} vendors.", vendors.Count);
        }

        // ── Team Assignments ──────────────────────────────────────────────────

        private async Task SeedTeamAssignmentsAsync()
        {
            if (await _context.TeamAssignments.AnyAsync()) return;

            // Look up venue IDs by name so we can then look up locker rooms correctly
            int? VenueId(string name) =>
                _context.Venues
                    .Where(v => v.Name == name)
                    .Select(v => (int?)v.Id)
                    .FirstOrDefault();

            // Look up locker room IDs by (VenueId, RoomNumber) — IDs were generated by SQL Server
            int? LockerRoom(int venueId, int roomNum) =>
                _context.LockerRooms
                    .Where(l => l.VenueId == venueId && l.RoomNumber == roomNum)
                    .Select(l => (int?)l.Id)
                    .FirstOrDefault();

            // Look up event IDs by name
            int? EventId(string name) =>
                _context.Events
                    .Where(e => e.Name == name)
                    .Select(e => (int?)e.Id)
                    .FirstOrDefault();

            // Look up team IDs by name
            int? TeamId(string name) =>
                _context.Teams
                    .Where(t => t.Name == name)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefault();

            // Pre-resolve venue IDs by name
            var allenId   = VenueId("Allen Fieldhouse");
            var tmobileId = VenueId("T-Mobile Center");
            var intrustId = VenueId("Intrust Bank Arena");

            if (allenId is null || tmobileId is null || intrustId is null)
            {
                _logger.LogWarning("Skipping TeamAssignment seeding — required venues not found.");
                return;
            }

            var assignments = new List<TeamAssignment>();

            void Add(string teamName, string eventName, int venueId, int roomNum)
            {
                int? evId     = EventId(eventName);
                int? teamId   = TeamId(teamName);
                int? lockerId = LockerRoom(venueId, roomNum);
                if (evId is null || teamId is null || lockerId is null)
                {
                    _logger.LogWarning("Skipping assignment for {Team}/{Event} — missing FK.", teamName, eventName);
                    return;
                }
                // Do NOT set Id — SQL Server IDENTITY generates it
                assignments.Add(new TeamAssignment { EventId = evId.Value, TeamId = teamId.Value, LockerId = lockerId.Value });
            }

            Add("Jayhawks", "Spring Invitational",   allenId.Value,   1);
            Add("Wildcats", "Spring Invitational",   allenId.Value,   2);
            Add("Shockers", "Regional Championship", tmobileId.Value, 1);
            Add("Gorillas", "Regional Championship", tmobileId.Value, 2);
            Add("Hornets",  "Youth League Showcase", intrustId.Value, 1);
            Add("Chiefs",   "Youth League Showcase", intrustId.Value, 2);

            if (assignments.Count > 0)
            {
                _context.TeamAssignments.AddRange(assignments);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} team assignments.", assignments.Count);
            }
        }

        // ── Vendor Assignments ────────────────────────────────────────────────

        private async Task SeedVendorAssignmentsAsync()
        {
            if (await _context.VendorAssignments.AnyAsync()) return;

            // Look up venue IDs by name so we can then look up booths correctly
            int? VenueId(string name) =>
                _context.Venues
                    .Where(v => v.Name == name)
                    .Select(v => (int?)v.Id)
                    .FirstOrDefault();

            // Look up booth IDs by (VenueId, BoothNumber) — IDs were generated by SQL Server
            int? Booth(int venueId, int boothNum) =>
                _context.VendorBooths
                    .Where(b => b.VenueId == venueId && b.BoothNumber == boothNum)
                    .Select(b => (int?)b.Id)
                    .FirstOrDefault();

            int? VendorId(string name) =>
                _context.Vendors
                    .Where(v => v.Name == name)
                    .Select(v => (int?)v.Id)
                    .FirstOrDefault();

            int? EventId(string name) =>
                _context.Events
                    .Where(e => e.Name == name)
                    .Select(e => (int?)e.Id)
                    .FirstOrDefault();

            // Pre-resolve venue IDs by name
            var allenId   = VenueId("Allen Fieldhouse");
            var tmobileId = VenueId("T-Mobile Center");
            var intrustId = VenueId("Intrust Bank Arena");

            if (allenId is null || tmobileId is null || intrustId is null)
            {
                _logger.LogWarning("Skipping VendorAssignment seeding — required venues not found.");
                return;
            }

            var assignments = new List<VendorAssignment>();

            void Add(string vendorName, string eventName, int venueId, int boothNum)
            {
                int? vendorId = VendorId(vendorName);
                int? evId     = EventId(eventName);
                int? boothId  = Booth(venueId, boothNum);
                if (vendorId is null || evId is null || boothId is null)
                {
                    _logger.LogWarning("Skipping vendor assignment for {Vendor}/{Event} — missing FK.", vendorName, eventName);
                    return;
                }
                assignments.Add(new VendorAssignment { VendorId = vendorId.Value, EventId = evId.Value, BoothId = boothId.Value });
            }

            Add("Arena Eats",       "Spring Invitational",   allenId.Value,   1);
            Add("Sports Gear Co.",  "Spring Invitational",   allenId.Value,   2);
            Add("Hydration Plus",   "Regional Championship", tmobileId.Value, 1);
            Add("Fan Zone Apparel", "Regional Championship", tmobileId.Value, 2);
            Add("KC Pretzels",      "Youth League Showcase", intrustId.Value, 1);

            if (assignments.Count > 0)
            {
                _context.VendorAssignments.AddRange(assignments);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} vendor assignments.", assignments.Count);
            }
        }

        // ── ParticipatesIn ────────────────────────────────────────────────────

        private async Task SeedParticipatesInAsync()
        {
            if (await _context.ParticipatesIn.AnyAsync()) return;

            // Look up IDs by name so we work regardless of what IDs were auto-generated
            var teamIds  = await _context.Teams.OrderBy(t => t.Id).Select(t => t.Id).ToListAsync();
            var eventIds = await _context.Events.OrderBy(e => e.Id).Select(e => e.Id).ToListAsync();

            if (teamIds.Count < 10 || eventIds.Count < 5)
            {
                _logger.LogWarning("Skipping ParticipatesIn seeding — not enough teams or events.");
                return;
            }

            var participates = new List<ParticipatesIn>
            {
                new() { TeamId = teamIds[0],  EventId = eventIds[0] },
                new() { TeamId = teamIds[1],  EventId = eventIds[0] },
                new() { TeamId = teamIds[2],  EventId = eventIds[1] },
                new() { TeamId = teamIds[3],  EventId = eventIds[1] },
                new() { TeamId = teamIds[4],  EventId = eventIds[2] },
                new() { TeamId = teamIds[5],  EventId = eventIds[2] },
                new() { TeamId = teamIds[6],  EventId = eventIds[3] },
                new() { TeamId = teamIds[7],  EventId = eventIds[3] },
                new() { TeamId = teamIds[8],  EventId = eventIds[4] },
                new() { TeamId = teamIds[9],  EventId = eventIds[4] },
            };

            _context.ParticipatesIn.AddRange(participates);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} participation records.", participates.Count);
        }

        // ── RegistersFor ──────────────────────────────────────────────────────

        private async Task SeedRegistersForAsync()
        {
            if (await _context.RegistersFor.AnyAsync()) return;

            // Look up IDs by insertion order so we work regardless of auto-generated values
            var attendeeIds = await _context.Attendees.OrderBy(a => a.Id).Select(a => a.Id).ToListAsync();
            var eventIds    = await _context.Events.OrderBy(e => e.Id).Select(e => e.Id).ToListAsync();

            if (attendeeIds.Count < 20 || eventIds.Count < 5)
            {
                _logger.LogWarning("Skipping RegistersFor seeding — not enough attendees or events.");
                return;
            }

            var registrations = new List<RegistersFor>();

            // Event 0: attendees 0–5
            for (int i = 0; i <= 5; i++)
                registrations.Add(new RegistersFor { AttendeeId = attendeeIds[i], EventId = eventIds[0] });
            // Event 1: attendees 3–9
            for (int i = 3; i <= 9; i++)
                registrations.Add(new RegistersFor { AttendeeId = attendeeIds[i], EventId = eventIds[1] });
            // Event 2: attendees 7–13
            for (int i = 7; i <= 13; i++)
                registrations.Add(new RegistersFor { AttendeeId = attendeeIds[i], EventId = eventIds[2] });
            // Event 3: attendees 9–15
            for (int i = 9; i <= 15; i++)
                registrations.Add(new RegistersFor { AttendeeId = attendeeIds[i], EventId = eventIds[3] });
            // Event 4: attendees 14–19
            for (int i = 14; i <= 19; i++)
                registrations.Add(new RegistersFor { AttendeeId = attendeeIds[i], EventId = eventIds[4] });

            _context.RegistersFor.AddRange(registrations);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} registration records.", registrations.Count);
        }
    }
}
