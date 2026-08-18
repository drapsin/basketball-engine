using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Referee;
using nba_mvc.Services.Referee;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefereeController : ControllerBase
    {
        private readonly IRefereeService _refereeService;

        public RefereeController(IRefereeService refereeService)
        {
            _refereeService = refereeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RefereeDto>>> GetAll()
        {
            var referees = await _refereeService.GetAllAsync();
            return Ok(referees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RefereeDto>> GetById(Guid id)
        {
            var referee = await _refereeService.GetByIdAsync(id);
            if (referee is null) return NotFound();
            return Ok(referee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RefereeDto>> Create(RefereeCreateDto dto)
        {
            var created = await _refereeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, RefereeUpdateDto dto)
        {
            var success = await _refereeService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _refereeService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}