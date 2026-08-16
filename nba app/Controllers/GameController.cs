using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Game;
using nba_mvc.Dtos.Stats;
using nba_mvc.Services.Game;
using nba_mvc.Services.Stats;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IGameStatsService _gameStatsService;

        public GameController(IGameService gameService, IGameStatsService gameStatsService)
        {
            _gameService = gameService;
            _gameStatsService = gameStatsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAll()
        {
            var games = await _gameService.GetAllAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetById(Guid id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game is null) return NotFound();
            return Ok(game);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<GameDetailDto>> GetDetailById(Guid id)
        {
            var game = await _gameService.GetDetailByIdAsync(id);
            if (game is null) return NotFound();
            return Ok(game);
        }

        [HttpGet("by-team/{teamId}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetByTeamId(Guid teamId)
        {
            var games = await _gameService.GetByTeamIdAsync(teamId);
            return Ok(games);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<GameDetailDto>> Create(GameCreateDto dto)
        {
            try
            {
                var created = await _gameService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetDetailById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(Guid id, GameUpdateDto dto)
        {
            try
            {
                var success = await _gameService.UpdateAsync(id, dto);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _gameService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/boxscore")]
        public async Task<ActionResult<List<PlayerBoxScoreDto>>> GetBoxScore(Guid id)
        {
            var boxScore = await _gameStatsService.GetBoxScoreAsync(id);
            return Ok(boxScore);
        }

        [HttpGet("{id}/playbyplay")]
        public async Task<ActionResult<List<PlayByPlayEntryDto>>> GetPlayByPlay(Guid id)
        {
            var playByPlay = await _gameStatsService.GetPlayByPlayAsync(id);
            return Ok(playByPlay);
        }

        [HttpGet("{id}/state")]
        public async Task<ActionResult<GameStateDto>> GetGameState(Guid id)
        {
            var state = await _gameStatsService.GetGameStateAsync(id);
            if (state is null) return NotFound();
            return Ok(state);
        }

        [HttpPost("{id}/finish")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> FinishGame(Guid id)
        {
            var success = await _gameService.FinishGameAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}