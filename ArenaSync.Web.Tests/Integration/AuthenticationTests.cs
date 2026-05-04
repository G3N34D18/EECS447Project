// -----------------------------------------------------------------------------
// File: AuthenticationTests.cs
// Project: ArenaSync.Web.Tests
// Purpose: Batch 4 — Integration tests for ASP.NET Core Identity user and role
//          management using an in-memory SQLite store.
//          NOTE: These tests wire up a real UserManager / RoleManager with the
//          SQLite-backed ApplicationDbContext so no SQL Server is needed.
// -----------------------------------------------------------------------------

using ArenaSync.Web.Data;
using ArenaSync.Web.Tests.TestSupport;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ArenaSync.Web.Tests.Integration;

public class AuthenticationTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly IServiceProvider _services;

    public AuthenticationTests()
    {
        _db = new SqliteTestDatabase();

        // Build a minimal DI container with Identity services backed by the
        // in-memory SQLite context
        var services = new ServiceCollection();

        services.AddLogging(lb => lb.AddDebug());

        // Provide the existing SQLite context
        services.AddScoped<ApplicationDbContext>(_ => _db.CreateContext());

        services.AddIdentityCore<IdentityUser>(options =>
            {
                options.Password.RequireDigit           = true;
                options.Password.RequiredLength         = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase       = false;
                options.User.RequireUniqueEmail         = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        _services = services.BuildServiceProvider();
    }

    public void Dispose() => _db.Dispose();

    private UserManager<IdentityUser>   GetUserManager()   => _services.GetRequiredService<UserManager<IdentityUser>>();
    private RoleManager<IdentityRole>   GetRoleManager()   => _services.GetRequiredService<RoleManager<IdentityRole>>();

    // ── Role creation ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRole_Admin_Succeeds()
    {
        var rm     = GetRoleManager();
        var result = await rm.CreateAsync(new IdentityRole("Admin"));
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateRole_Duplicate_Fails()
    {
        var rm = GetRoleManager();
        await rm.CreateAsync(new IdentityRole("Manager"));
        var duplicate = await rm.CreateAsync(new IdentityRole("Manager"));
        Assert.False(duplicate.Succeeded);
    }

    // ── User creation ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateUser_ValidCredentials_Succeeds()
    {
        var um     = GetUserManager();
        var user   = new IdentityUser { UserName = "newuser@test.com", Email = "newuser@test.com", EmailConfirmed = true };
        var result = await um.CreateAsync(user, "Password1!");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateUser_WeakPassword_Fails()
    {
        var um     = GetUserManager();
        var user   = new IdentityUser { UserName = "weakpwd@test.com", Email = "weakpwd@test.com" };
        var result = await um.CreateAsync(user, "abc"); // too short, no digit
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_Fails()
    {
        var um = GetUserManager();
        var u1 = new IdentityUser { UserName = "dup@test.com", Email = "dup@test.com", EmailConfirmed = true };
        await um.CreateAsync(u1, "Password1!");

        var u2 = new IdentityUser { UserName = "dup@test.com", Email = "dup@test.com" };
        var result = await um.CreateAsync(u2, "Password1!");
        Assert.False(result.Succeeded);
    }

    // ── Role assignment ───────────────────────────────────────────────────────

    [Fact]
    public async Task AssignRole_ToUser_Succeeds()
    {
        var um = GetUserManager();
        var rm = GetRoleManager();

        await rm.CreateAsync(new IdentityRole("Viewer"));

        var user = new IdentityUser { UserName = "viewer@test.com", Email = "viewer@test.com", EmailConfirmed = true };
        await um.CreateAsync(user, "Password1!");

        var result = await um.AddToRoleAsync(user, "Viewer");
        Assert.True(result.Succeeded);

        var isInRole = await um.IsInRoleAsync(user, "Viewer");
        Assert.True(isInRole);
    }

    [Fact]
    public async Task AssignRole_NonExistentRole_Fails()
    {
        var um   = GetUserManager();
        var user = new IdentityUser { UserName = "norole@test.com", Email = "norole@test.com", EmailConfirmed = true };
        await um.CreateAsync(user, "Password1!");

        // Role "GhostRole" does not exist → should fail
        await Assert.ThrowsAnyAsync<Exception>(() =>
            um.AddToRoleAsync(user, "GhostRole"));
    }

    // ── Password verification ─────────────────────────────────────────────────

    [Fact]
    public async Task CheckPassword_CorrectPassword_ReturnsTrue()
    {
        var um   = GetUserManager();
        var user = new IdentityUser { UserName = "pwcheck@test.com", Email = "pwcheck@test.com", EmailConfirmed = true };
        await um.CreateAsync(user, "Correct123!");

        var valid = await um.CheckPasswordAsync(user, "Correct123!");
        Assert.True(valid);
    }

    [Fact]
    public async Task CheckPassword_WrongPassword_ReturnsFalse()
    {
        var um   = GetUserManager();
        var user = new IdentityUser { UserName = "pwwrong@test.com", Email = "pwwrong@test.com", EmailConfirmed = true };
        await um.CreateAsync(user, "Correct123!");

        var valid = await um.CheckPasswordAsync(user, "WrongPassword!");
        Assert.False(valid);
    }

    // ── FindByEmail ───────────────────────────────────────────────────────────

    [Fact]
    public async Task FindByEmail_ExistingUser_ReturnsUser()
    {
        var um   = GetUserManager();
        var user = new IdentityUser { UserName = "findme@test.com", Email = "findme@test.com", EmailConfirmed = true };
        await um.CreateAsync(user, "Password1!");

        var found = await um.FindByEmailAsync("findme@test.com");
        Assert.NotNull(found);
        Assert.Equal("findme@test.com", found.Email);
    }

    [Fact]
    public async Task FindByEmail_NonExistentUser_ReturnsNull()
    {
        var um    = GetUserManager();
        var found = await um.FindByEmailAsync("ghost@test.com");
        Assert.Null(found);
    }
}
