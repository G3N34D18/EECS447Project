using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL TEAMS
        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        // GET TEAM BY ID
        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Assignments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // CREATE TEAM (with duplicate name check)
        public async Task<bool> CreateTeamAsync(Team team)
        {
            // Duplicate name check
            bool exists = await _context.Teams
                .AnyAsync(t => t.Name.ToLower() == team.Name.ToLower());

            if (exists)
                return false;

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return true;
        }

        // UPDATE TEAM 
        public async Task<Team> UpdateTeamAsync(Team team)
        {
            var existingTeam = await _context.Teams.FindAsync(team.Id);
            if (existingTeam == null) return null;
            existingTeam.Name = team.Name;
            existingTeam.Manager = team.Manager;
            existingTeam.Email = team.Email;
            existingTeam.Phone = team.Phone;
            existingTeam.PlayerCount = team.PlayerCount;
            await _context.SaveChangesAsync();
            return existingTeam;
        }

        // DELETE TEAM
        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        // ASSIGN TEAM TO EVENT (with duplicate check)
        public async Task<bool> AssignTeamToEventAsync(int teamId, int eventId)
        {
            // Check if assignment already exists
            bool exists = await _context.ParticipatesIn
                .AnyAsync(p => p.TeamId == teamId && p.EventId == eventId);

            if (exists)
                return false;

            // Ensure event has fewer than 2 teams
            int teamCount = await _context.ParticipatesIn
                .CountAsync(p => p.EventId == eventId);

            if (teamCount >= 2)
                return false;

            var assignment = new ParticipatesIn
            {
                TeamId = teamId,
                EventId = eventId
            };

            _context.ParticipatesIn.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        // GET TEAMS FOR EVENT
        public async Task<List<Team>> GetTeamsForEventAsync(int eventId)
        {
            return await _context.ParticipatesIn
                .Where(p => p.EventId == eventId)
                .Include(p => p.Team)
                .Select(p => p.Team)
                .ToListAsync();
        }
    }
}
