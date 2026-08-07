using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.ActionEvent;
using nba_mvc.Services.ActionEvent;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionEventController : ControllerBase
    {
        private readonly IActionEventService _actionEventService;

        public ActionEventController(IActionEventService actionEventService)
        {
            _actionEventService = actionEventService;
        }

        [HttpGet("by-game/{gameId}")]
        public async Task<ActionResult<IEnumerable<ActionEventDto>>> GetByGameId(Guid gameId)
        {
            var events = await _actionEventService.GetByGameIdAsync(gameId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<ActionResult<ActionEventDto>> Create(ActionEventCreateDto dto)
        {
            var created = await _actionEventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByGameId), new { gameId = created.GameId }, created);
        }
    }
}