using Microsoft.AspNetCore.Mvc;
using nba_mvc.Services.Simulation;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IGameSimulationStateStore _stateStore;

        public SimulationController(IGameSimulationStateStore stateStore)
        {
            _stateStore = stateStore;
        }

        [HttpPost("{gameId}/start")]
        public IActionResult Start(Guid gameId)
        {
            var started = _stateStore.TryStart(gameId);
            if (!started) return Conflict(new { message = "Simulation already running for this game." });
            return Ok(new { message = "Simulation started." });
        }

        [HttpPost("{gameId}/pause")]
        public IActionResult Pause(Guid gameId)
        {
            var success = _stateStore.Pause(gameId);
            if (!success) return NotFound(new { message = "No active simulation for this game." });
            return Ok(new { message = "Simulation paused." });
        }

        [HttpPost("{gameId}/resume")]
        public IActionResult Resume(Guid gameId)
        {
            var success = _stateStore.Resume(gameId);
            if (!success) return NotFound(new { message = "No active simulation for this game." });
            return Ok(new { message = "Simulation resumed." });
        }

        [HttpPost("{gameId}/stop")]
        public IActionResult Stop(Guid gameId)
        {
            var success = _stateStore.Stop(gameId);
            if (!success) return NotFound(new { message = "No active simulation for this game." });
            return Ok(new { message = "Simulation stopped." });
        }

        [HttpGet("{gameId}/status")]
        public IActionResult Status(Guid gameId)
        {
            var state = _stateStore.Get(gameId);
            if (state is null) return NotFound(new { message = "No active simulation for this game." });
            return Ok(state);
        }
    }
}