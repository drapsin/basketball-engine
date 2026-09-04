using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Services.Simulation;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IGameSimulationStateStore _stateStore;
        private readonly IGameSimulationEngine _engine;

        public SimulationController(IGameSimulationStateStore stateStore, IGameSimulationEngine engine)
        {
            _stateStore = stateStore;
            _engine = engine;
        }

        [HttpPost("{gameId}/start")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Start(Guid gameId)
        {
            var isAutomatic = User.IsInRole("Admin");
            var started = _stateStore.TryStart(gameId, isAutomatic);
            if (!started) return Conflict(new { message = "Simulation already running for this game." });
            return Ok(new { message = "Simulation started.", automatic = isAutomatic });
        }

        [HttpPost("{gameId}/pause")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Pause(Guid gameId)
        {
            var success = _stateStore.Pause(gameId);
            if (!success) return NotFound(new { message = "No active simulation for this game." });
            return Ok(new { message = "Simulation paused." });
        }

        [HttpPost("{gameId}/resume")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Resume(Guid gameId)
        {
            var success = _stateStore.Resume(gameId);
            if (!success) return NotFound(new { message = "No active simulation for this game." });
            return Ok(new { message = "Simulation resumed." });
        }

        [HttpPost("{gameId}/stop")]
        [Authorize(Roles = "Admin,Manager")]
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

        [HttpGet("active")]
        public IActionResult GetActiveGames()
        {
            var states = _stateStore.GetAllStates();
            return Ok(states);
        }

        [HttpPost("{gameId}/instant")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SimulateInstant(Guid gameId)
        {
            if (_stateStore.Get(gameId) != null)
                return Conflict(new { message = "This game is currently live. Stop it before simulating instantly." });

            var success = await _engine.SimulateInstantAsync(gameId);
            if (!success)
                return BadRequest(new { message = "Could not simulate — game not found, has no players assigned, or is already finished." });

            return Ok(new { message = "Game simulated instantly." });
        }
    }
}