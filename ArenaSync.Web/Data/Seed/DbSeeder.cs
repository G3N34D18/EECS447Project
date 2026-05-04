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
                new() { Name = "Allen Fieldhouse",   Address = "1651 Naismith Dr,