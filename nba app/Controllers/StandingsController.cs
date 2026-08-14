using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Stats;
using nba_mvc.Services.Stats;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingsService _standingsService;

        public StandingsController(IStandingsService standingsService)
        {
            _standingsService = standingsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamStandingDto>>> GetStandings()
        {
            var standings = await _standingsService.GetStandingsAsync();
            return Ok(standings);
        }
    }
}