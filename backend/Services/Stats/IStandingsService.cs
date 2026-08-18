using nba_mvc.Dtos.Stats;
using nba_mvc.Models;

namespace nba_mvc.Services.Stats
{
    public interface IStandingsService
    {
        Task<List<TeamStandingDto>> GetStandingsAsync();
    }
}