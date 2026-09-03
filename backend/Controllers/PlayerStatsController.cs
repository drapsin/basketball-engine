using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Stats;
using nba_mvc.Services.Stats;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerStatsController : ControllerBase
    {
        private readonly IPlayerStatsService _playerStatsService;

        public PlayerStatsController(IPlayerStatsService playerStatsService)
        {
            _playerStatsService = playerStatsService;
        }

        [HttpGet("{playerId}/career")]
        public async Task<ActionResult<PlayerCareerStatsDto>> GetPlayerCareer(Guid playerId)
        {
            var stats = await _playerStatsService.GetPlayerCareerStatsAsync(playerId);
            if (stats is null) return NotFound();
            return Ok(stats);
        }

        [HttpGet("team/{teamId}/career")]
        public async Task<ActionResult<TeamCareerStatsDto>> GetTeamCareer(Guid teamId)
        {
            var stats = await _playerStatsService.GetTeamCareerStatsAsync(teamId);
            if (stats is null) return NotFound();
            return Ok(stats);
        }

        [HttpGet("leaders")]
        public async Task<ActionResult<List<LeagueLeaderDto>>> GetLeaders([FromQuery] string category = "points", [FromQuery] int limit = 10)
        {
            var leaders = await _playerStatsService.GetLeagueLeadersAsync(category, limit);
            return Ok(leaders);
        }
    }
}