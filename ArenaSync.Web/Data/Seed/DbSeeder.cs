using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Apply migrations automatically (optional but common)
            context.Database.Migrate();

            SeedTeams(context);
            SeedAttendees(context);
            SeedParticipatesIn(context);
            SeedRegistersFor(context);

            context.SaveChanges();
        }

        private static void SeedTeams(ApplicationDbContext context)
        {
            if (context.Teams.Any()) return;

            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Jayhawks", Manager = "Alice", Email = "jayhawks@team.com", Phone = "785-555-1101", PlayerCount = 20 },
                new Team { Id = 2, Name = "Wildcats", Manager = "Bob", Email = "wildcats@team.com", Phone = "785-555-1102", PlayerCount = 20 },
                new Team { Id = 3, Name = "Shockers", Manager = "Carla", Email = "shockers@team.com", Phone = "785-555-1103", PlayerCount = 20 },
                new Team { Id = 4, Name = "Gorillas", Manager = "David", Email = "gorillas@team.com", Phone = "785-555-1104", PlayerCount = 20 },
                new Team { Id = 5, Name = "Hornets", Manager = "Eve", Email = "hornets@team.com", Phone = "785-555-1105", PlayerCount = 20 },
                new Team { Id = 6, Name = "Chiefs", Manager = "Frank", Email = "chiefs@team.com", Phone = "785-555-1201", PlayerCount = 20 },
                new Team { Id = 7, Name = "Royals", Manager = "Grace", Email = "royals@team.com", Phone = "785-555-1202", PlayerCount = 20 },
                new Team { Id = 8, Name = "Sporting KC", Manager = "Henry", Email = "sportingkc@team.com", Phone = "785-555-1203", PlayerCount = 20 },
                new Team { Id = 9, Name = "Current", Manager = "Ivy", Email = "current@team.com", Phone = "785-555-1204", PlayerCount = 20 },
                new Team { Id = 10, Name = "Mavericks", Manager = "Jack", Email = "mavericks@team.com", Phone = "785-555-1205", PlayerCount = 20 }
            };

            context.Teams.AddRange(teams);
        }


        private static void SeedAttendees(ApplicationDbContext context)
        {
            if (context.Attendees.Any()) return;

            var attendees = Enumerable.Range(1, 20).Select(i =>
                new Attendee
                {
                    Id = i,
                    Name = $"Attendee {i}",
                    Email = $"attendee{i}@mail.com",
                    Phone = $"785-555-200{i:D2}"
                }
            );

            context.Attendees.AddRange(attendees);
        }

        private static void SeedParticipatesIn(ApplicationDbContext context)
        {
            if (context.ParticipatesIn.Any()) return;

            var participates = new List<ParticipatesIn>
            {
                new ParticipatesIn { TeamId = 1, EventId = 1 },
                new ParticipatesIn { TeamId = 2, EventId = 1 },

                new ParticipatesIn { TeamId = 3, EventId = 2 },
                new ParticipatesIn { TeamId = 4, EventId = 2 },

                new ParticipatesIn { TeamId = 5, EventId = 3 },
                new ParticipatesIn { TeamId = 6, EventId = 3 },

                new ParticipatesIn { TeamId = 7, EventId = 4 },
                new ParticipatesIn { TeamId = 8, EventId = 4 },

                new ParticipatesIn { TeamId = 9, EventId = 5 },
                new ParticipatesIn { TeamId = 10, EventId = 5 }
            };

            context.ParticipatesIn.AddRange(participates);
        }

        private static void SeedRegistersFor(ApplicationDbContext context)
        {
            if (context.RegistersFor.Any()) return;

            var registrations = new List<RegistersFor>();

            // Event 1: attendees 1–6
            registrations.AddRange(
                Enumerable.Range(1, 6).Select(i =>
                    new RegistersFor { AttendeeId = i, EventId = 1 }
                )
            );

            // Event 2: attendees 4–10
            registrations.AddRange(
                Enumerable.Range(4, 7).Select(i =>
                    new RegistersFor { AttendeeId = i, EventId = 2 }
                )
            );

            // Event 3: attendees 8–14
            registrations.AddRange(
                Enumerable.Range(8, 7).Select(i =>
                    new RegistersFor { AttendeeId = i, EventId = 3 }
                )
            );

            // Event 4: attendees 10–16
            registrations.AddRange(
                Enumerable.Range(10, 7).Select(i =>
                    new RegistersFor { AttendeeId = i, EventId = 4 }
                )
            );

            // Event 5: attendees 15–20
            registrations.AddRange(
                Enumerable.Range(15, 6).Select(i =>
                    new RegistersFor { AttendeeId = i, EventId = 5 }
                )
            );
            

            context.RegistersFor.AddRange(registrations);
        }
    }
}
