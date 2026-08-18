using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Coach;
using nba_mvc.Services.Coach;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoachController : ControllerBase
    {
        private readonly ICoachService _coachService;

        public CoachController(ICoachService coachService)
        {
            _coachService = coachService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoachDto>>> GetAll()
        {
            var coaches = await _coachService.GetAllAsync();
            return Ok(coaches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoachDto>> GetById(Guid id)
        {
            var coach = await _coachService.GetByIdAsync(id);
            if (coach is null) return NotFound();
            return Ok(coach);
        }

        [HttpGet("by-team/{teamId}")]
        public async Task<ActionResult<CoachDto>> GetByTeamId(Guid teamId)
        {
            var coach = await _coachService.GetByTeamIdAsync(teamId);
            if (coach is null) return NotFound();
            return Ok(coach);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CoachDto>> Create(CoachCreateDto dto)
        {
            var created = await _coachService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, CoachUpdateDto dto)
        {
            var success = await _coachService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _coachService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}