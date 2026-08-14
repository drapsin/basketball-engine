using nba_mvc.Dtos.Stats;

namespace nba_mvc.Services.Stats
{
    public interface IGameStatsService
    {
        Task<List<PlayerBoxScoreDto>> GetBoxScoreAsync(Guid gameId);
    }
}