using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Services.ExternalData;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly INbaImportService _importService;

        public AdminController(INbaImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("import-nba-data")]
        public async Task<IActionResult> ImportNbaData()
        {
            var (teams, players) = await _importService.ImportRealNbaDataAsync();
            return Ok(new { message = $"Imported {teams} teams and {players} players.", teams, players });
        }
    }
}