using nba_mvc.Dtos.Stats;

namespace nba_mvc.Services.Stats
{
    public interface IPlayerStatsService
    {
        Task<PlayerCareerStatsDto?> GetPlayerCareerStatsAsync(Guid playerId);
        Task<TeamCareerStatsDto?> GetTeamCareerStatsAsync(Guid teamId);
        Task<List<LeagueLeaderDto>> GetLeagueLeadersAsync(string category, int limit);
    }
}