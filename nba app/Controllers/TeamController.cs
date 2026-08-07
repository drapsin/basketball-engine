using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Team;
using nba_mvc.Models;
using nba_mvc.Services.Team;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll()
        {
            var teams = await _teamService.GetAllAsync();
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetById(Guid id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team is null) return NotFound();
            return Ok(team);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<TeamDetailDto>> GetDetailById(Guid id)
        {
            var team = await _teamService.GetDetailByIdAsync(id);
            if (team is null) return NotFound();
            return Ok(team);
        }

        [HttpGet("by-conference/{conference}")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetByConference(Conference conference)
        {
            var teams = await _teamService.GetByConferenceAsync(conference);
            return Ok(teams);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> Create(TeamCreateDto dto)
        {
            var created = await _teamService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TeamUpdateDto dto)
        {
            var success = await _teamService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _teamService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}