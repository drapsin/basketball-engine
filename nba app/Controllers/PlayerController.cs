using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Player;
using nba_mvc.Services.Player;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll()
        {
            var players = await _playerService.GetAllAsync();
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetById(Guid id)
        {
            var player = await _playerService.GetByIdAsync(id);
            if (player is null) return NotFound();
            return Ok(player);
        }

        [HttpGet("by-team/{teamId}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetByTeamId(Guid teamId)
        {
            var players = await _playerService.GetByTeamIdAsync(teamId);
            return Ok(players);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> Create(PlayerCreateDto dto)
        {
            var created = await _playerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PlayerUpdateDto dto)
        {
            var success = await _playerService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _playerService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}