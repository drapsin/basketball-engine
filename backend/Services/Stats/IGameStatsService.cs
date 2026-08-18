using nba_mvc.Dtos.Stats;

namespace nba_mvc.Services.Stats
{
    public interface IGameStatsService
    {
        Task<List<PlayerBoxScoreDto>> GetBoxScoreAsync(Guid gameId);
        Task<List<PlayByPlayEntryDto>> GetPlayByPlayAsync(Guid gameId);
        Task<GameStateDto?> GetGameStateAsync(Guid gameId);
    }
}