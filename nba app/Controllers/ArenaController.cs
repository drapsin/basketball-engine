using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Arena;
using nba_mvc.Services.Arena;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArenaController : ControllerBase
    {
        private readonly IArenaService _arenaService;

        public ArenaController(IArenaService arenaService)
        {
            _arenaService = arenaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArenaDto>>> GetAll()
        {
            var arenas = await _arenaService.GetAllAsync();
            return Ok(arenas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArenaDto>> GetById(Guid id)
        {
            var arena = await _arenaService.GetByIdAsync(id);
            if (arena is null) return NotFound();
            return Ok(arena);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArenaDto>> Create(ArenaCreateDto dto)
        {
            var created = await _arenaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ArenaUpdateDto dto)
        {
            var success = await _arenaService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _arenaService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}